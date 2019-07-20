
import { Request, Response, NextFunction } from "express";
import { IncomingMessage } from 'http';
import * as jwt from 'jsonwebtoken';
import * as httpStatus from 'http-status';

import { config } from '../config/config';
import { APIError } from '../helpers/APIError';
import MemberModel, { Member, CheckInStatus } from '../models/member.model';
import CheckInModel, { CheckIn, CheckInType, NeedChecked } from '../models/checkin.model';

import * as moment from 'moment';
import * as path from 'path';
import * as http from 'http';

import * as file from 'fs';
import LocationModel from '../models/location.model';
import socket, { SocketOp } from '../config/socket-service';
import * as uuid from 'uuid/v4';
import { getJsApiTicket, getAccessTokenAsync } from '../helpers/ticket';
import { createHmac, createHash } from 'crypto';
import * as https from 'https';

socket.onEvent.subscribe((data) => {
    console.log(`received socket event data`, data);
});

const secureSignType = 'HMAC-256';
const sha1 = "sha1";

export let createWxConfig = async (req, res, next) => {
    const timestamp = Math.floor(+new Date() / 1000).toString();
    const nonceStr = uuid().replace(/-/g, "");
    const configParams = [];
    // configParams.push({ key: "appId", value: config.wx.appId });
    configParams.push({ key: "timestamp", value: timestamp });
    // !!!!!!! use noncestr for general API signature.
    configParams.push({ key: "noncestr", value: nonceStr });
    // configParams.push({ key: "nonceStr", value: nonceStr });
    //configParams.push({ key: "signType", value: secureSignType });
    configParams.push({ key: "url", value: decodeURIComponent(req.body.url) });

    const signature = await createWxSignatureAsync(configParams).catch(error => {
        console.error(error);
    });

    if (!signature) {
        const err = new APIError("createWxSignature error", httpStatus.INTERNAL_SERVER_ERROR);
        return next(err);
    }

    const result = {
        appId: config.wx.appId,
        timestamp: timestamp,
        nonceStr: nonceStr,
        signType: secureSignType,
        signature: signature
    };

    return res.json({
        code: 0,
        message: 'OK',
        data: result
    });
};

async function createWxSignatureAsync(payload) {
    const ticket = await getJsApiTicket().catch(error => {
        console.error(error);
    });
    if (!ticket) {
        return;
    }

    payload.push({ key: 'jsapi_ticket', value: ticket });
    const urlIndex = (payload as any[]).findIndex(i => i.key == 'url');
    if (urlIndex != -1) {
        payload[urlIndex].value = decodeURIComponent(payload[urlIndex].value);
    }

    const signature = await getSignatureBasedOnEnv(payload, sha1);
    return signature;
}

async function getSignatureBasedOnEnv(data, signType?: string) {
    let signature: string;
    // todo: for production use only.
    signature = getSignature(data, config.wx.appSecret, signType);
    return signature;
}

function getSignature(data: any[], apiKey: string, signType: string = secureSignType) {

    console.log("signType:", signType);
    signType = signType.toUpperCase();

    const dataToSign = data.filter(m => !!m && !!m.value).sort((l, r) => l.key < r.key ? -1 : 1).map(m => `${m.key}=${m.value}`).join('&').toString();
    const dataToSignWithApiKey = dataToSign + `&key=${apiKey}`;

    console.log('dataToSign:', dataToSign);
    console.log('dataToSignWithApiKey:', dataToSignWithApiKey);

    if (signType == secureSignType) {
        const hmac = createHmac('sha256', apiKey);
        const signature = hmac.update(dataToSign, 'ascii').digest('hex').toLowerCase();
        console.log('signature:', signature);
        return signature;
    }
    else {
        console.log('Real dataToSign is:', dataToSign);
        const signature = createHash('sha1').update(dataToSign).digest('hex').toLowerCase();
        console.log('signature:', signature);
        return signature;
    }
}

export let register = async (req, res, next) => {
    console.log('register:', req.body);
    const internalOpenId = req.body.internalOpenId;

    const member = await MemberModel.findOne({
        internalOpenId: internalOpenId
    });
    if (!member) {
        return res.json({
            code: 404,
            message: 'Member not found'
        });
    }

    const accessTokenResult = await getAccessTokenAsync();
    const faceList = req.body.mediaIds.filter(mediaId => {
        return !member.faceList || member.faceList.indexOf(mediaId) === -1;
    }).map(mediaId => {
        return `/static/face/${internalOpenId}-${mediaId}.jpg`;
    });
    req.body.mediaIds.forEach(mediaId => {
        const url = `http://file.api.weixin.qq.com/cgi-bin/media/get?access_token=${accessTokenResult.access_token}&media_id=${mediaId}`;
        const filePath = path.join(__dirname, `../../../static/face/${internalOpenId}-${mediaId}.jpg`);
        const stream = file.createWriteStream(filePath);
        const request = http.get(url, (response) => {
            response.pipe(stream);
            stream.on('finish', function () {
                console.log('file write finish');
                stream.close();  // close() is async, call cb after close completes.
            });
        }).on('error', function (err) { // Handle errors
            console.error(err);
            file.unlink(filePath, () => { }); // Delete the file async. (But we don't check the result)
        });
    });

    member.wechatId = req.body.wechatId;
    member.contactName = req.body.contactName;
    member.telephone = req.body.telephone;
    member.locationId = req.body.locationId;
    member.faceList = (member.faceList || []).concat(faceList);
    await member.save();

    return res.json({
        code: 0,
        message: 'OK',
        data: member
    });
}

export let authorize = (req, res, next) => {
    const scope = 'snsapi_userinfo';
    return res.redirect(`https://open.weixin.qq.com/connect/oauth2/authorize?appid=${config.wx.appId}&redirect_uri=${config.wx.redirectUrl}&response_type=code&scope=${scope}&state=${encodeURIComponent(req.query.state || '')}#wechat_redirect`);
};

export let login = async (req: Request, res: Response, next: NextFunction) => {

    try {
        let dbUser = await MemberModel.findOne({
            internalOpenId: req.user.internalOpenId,
        });

        if (!dbUser) {
            console.log('req.user', req.user);
            dbUser = new MemberModel(req.user);
            await dbUser.save();
        }

        const userLocation = dbUser.locationId ? await LocationModel.findById(dbUser.locationId) : null;

        let redirectUrl = config.rootUrl;

        if (req.query.state) {
            redirectUrl = userLocation ? userLocation.url.toString() : config.checkInUrl;
        }

        if (!req.query.state) {
            res.cookie('internalOpenId', req.user.internalOpenId);

            console.log('state:', redirectUrl);
            if (/\?/.test(redirectUrl)) {
                redirectUrl += `&internalOpenId=${req.user.internalOpenId}`;
            }
            else if (/#/.test(redirectUrl)) {
                redirectUrl = redirectUrl.replace(/(.*)#([^#]*)/, `$1?internalOpenId=${req.user.internalOpenId}#$2`);
            }
            else {
                redirectUrl += `?internalOpenId=${req.user.internalOpenId}`;
            }
        }
        console.log('redirectTo:', redirectUrl);

        return res.redirect(redirectUrl);
    }
    catch (err) {
        console.error('Login failed,', err);
        return next(err);
    }
};

export let list = async (req, res, next) => {
    const data = await MemberModel.find();
    return res.json({
        code: 0,
        message: "OK",
        data: data
    });
};

export let load = async (req, res, next) => {
    const data = await MemberModel.findById(req.params.id);
    return res.json({
        code: 0,
        message: "OK",
        data: data
    });
};

export let loadByInternalOpenId = async (req, res, next) => {
    const data = await MemberModel.findOne({ internalOpenId: req.params.internalOpenId });
    return res.json({
        code: 0,
        message: "OK",
        data: data
    });
};

export let create = async (req, res, next) => {

    let data = req.body;

    let existmember = await MemberModel.findOne({ openId: data.openId });
    if (!existmember) {
        const base64String = req.body.avatar.match(/data:(.*);base64,(.*)/)[2];

        const avatar = Buffer.from(base64String, "base64");
        const fileName = `/static/avatar/${data.openId}-${+new Date()}.jpg`;
        data.avatarUrl = fileName;

        let member = new MemberModel(data);

        file.writeFile(path.join(__dirname, `../../..${fileName}`), avatar, async (err) => {
            if (err) {
                return res.json({
                    code: 500,
                    message: err,
                });
            }

            await member.save();

            return res.json({
                code: 0,
                message: "OK",
                data: member
            });
        });
    } else {
        return res.json({
            code: 500,
            message: "当前用户已经存在,不能重复注册"
        });
    }

};

export let update = async (req, res, next) => {

    let data = req.body;

    const base64String = req.body.avatar.match(/data:(.*);base64,(.*)/)[2];

    const avatar = Buffer.from(base64String, "base64");
    const fileName = `/static/avatar/${data.openId}-${+new Date()}.jpg`;
    data.avatarUrl = fileName;

    file.writeFile(path.join(__dirname, `../../..${fileName}`), avatar, async (err) => {
        if (err) {
            return res.json({
                code: 500,
                message: err,
            });
        }

        await MemberModel.findByIdAndUpdate(req.params.id, data, (error, savedMember) => {
            if (error) {
                return res.json({
                    code: 500,
                    message: "Update Error",
                });
            }

            return res.json({
                code: 0,
                message: "OK",
                data: savedMember
            });
        });


    });
};

export let remove = async (req, res, next) => {
    const data = await MemberModel.findByIdAndDelete(req.params.id, (error, removedMember) => {
        if (error) {
            return res.json({
                code: 500,
                message: "Remove Error",
            });
        }

        return res.json({
            code: 0,
            message: "OK",
        });
    });
};

export let checkIn = async (req, res, next) => {

    if (!req.query.type) {
        req.query.type = 0;
    }

    const locationId = req.query.locationId;

    let locations = await LocationModel.find();
    let checkInList = await CheckInModel.find({
        createdAt: {
            $gte: moment({ hour: 0 })
        },
        type: req.query.type
    });

    let members = await MemberModel.find({
        status: CheckInStatus.Activated
    });

    if (!checkInList || checkInList.length == 0) {

        socket.broadcast(SocketOp.PLAIN, {
            message: `check-in total ${members.length} activated records initializing.`
        });

        checkInList = await CheckInModel.find({
            createdAt: {
                $gte: moment({ hour: 0 })
            },
            type: req.query.type
        });

        if (checkInList && checkInList.length) {
            let result = checkInList.filter(item => !locationId || item.locationId == locationId).map(item => {
                let location = locations.find(loc => loc._id == item.locationId);
                let originMember = members.find(member => member.openId == item.openId);
                return {
                    _id: item._id,
                    openId: item.openId,
                    nickName: item.nickName,
                    wechatId: item.wechatId,
                    contactName: item.contactName,
                    telephone: item.telephone,
                    locationId: item.locationId,
                    avatarUrl: item.avatarUrl,
                    status: item.status,
                    type: item.type,
                    checkInTime: item.checkInTime,
                    longitude: location && location.longitude,
                    latitude: location && location.latitude,
                    message: item.message,
                    faceList: originMember && originMember.faceList,
                };
            });

            socket.broadcast(SocketOp.PLAIN, {
                message: `check-in total ${members.length} activated records initialized successfully.`
            });

            return res.json({
                code: 0,
                message: "OK",
                data: result
            });
        }

        CheckInModel.insertMany(members.map(member => {
            return {
                openId: member.openId,
                nickName: member.nickName,
                wechatId: member.wechatId,
                contactName: member.contactName,
                telephone: member.telephone,
                locationId: member.locationId,
                avatarUrl: member.avatarUrl,
                status: CheckInStatus.Waiting,
                type: req.query.type,
                createdAt: new Date(),
                updateAt: new Date(),
            }
        }), (error, docs) => {
            if (error) {
                socket.broadcast(SocketOp.PLAIN, {
                    message: `check-in total ${members.length} activated records initializing failed.`
                });
                return res.json({
                    code: 500,
                    message: "insertMany check-in records error",
                });
            }

            let result = docs.filter(item => !locationId || item.locationId == locationId).map(item => {
                let location = locations.find(loc => loc._id == item.locationId);
                let originMember = members.find(member => member.openId == item.openId);
                return {
                    _id: item._id,
                    openId: item.openId,
                    nickName: item.nickName,
                    wechatId: item.wechatId,
                    contactName: item.contactName,
                    telephone: item.telephone,
                    locationId: item.locationId,
                    avatarUrl: item.avatarUrl,
                    status: item.status,
                    type: item.type,
                    checkInTime: item.checkInTime,
                    longitude: location && location.longitude,
                    latitude: location && location.latitude,
                    message: item.message,
                    faceList: originMember && originMember.faceList,
                };
            });

            socket.broadcast(SocketOp.PLAIN, {
                message: `check-in total ${members.length} activated records initialized successfully.`
            });

            return res.json({
                code: 0,
                message: "OK",
                data: result
            });
        });
    }
    else {
        let result = checkInList.filter(item => !locationId || item.locationId == locationId).map(item => {
            let location = locations.find(loc => loc._id == item.locationId);
            if (!location) {
                location = locations[0];
            }
            let originMember = members.find(member => member.openId == item.openId);
            return {
                _id: item._id,
                openId: item.openId,
                nickName: item.nickName,
                wechatId: item.wechatId,
                contactName: item.contactName,
                telephone: item.telephone,
                locationId: item.locationId,
                avatarUrl: item.avatarUrl,
                status: item.status,
                type: item.type,
                checkInTime: item.checkInTime,
                longitude: location && location.longitude,
                latitude: location && location.latitude,
                message: item.message,
                result: item.result,
                url: item.url,
                signatureStr: item.signatureStr,
                faceList: originMember && originMember.faceList,
            }
        });
        return res.json({
            code: 0,
            message: "OK",
            data: result
        });
    }
};

export let updateCheckIn = async (req, res, next) => {

    const checkInTime = new Date();
    let signatureStr = '';
    // if (req.body.result === "needface") {
    //     signatureStr = await needFace(req.body.url);
    // }
    console.log(`update check-in status: ${req.body.result}, message: ${req.body.message}`);

    await CheckInModel.findByIdAndUpdate(req.params.id, {
        status: req.body.status,
        checkInTime: checkInTime.toLocaleString(),
        result: req.body.result,
        message: req.body.message,
        url: req.body.url,
        signatureStr: signatureStr
    }, (error, updatedCheckIn) => {
        if (error) {
            socket.broadcast(SocketOp.PLAIN, {
                id: req.params.id,
                message: `check-in ${req.params.id} updating failed.`
            });

            return res.json({
                code: 500,
                message: "update check-in error.",
            });
        }

        socket.broadcast(SocketOp.CHECK_IN_UPDATED, {
            id: req.params.id,
            name: updatedCheckIn.contactName || updatedCheckIn.nickName,
            message: `check-in ${updatedCheckIn._id} updated.`
        });

        ((updatedCheckIn.checkInTime) as any) = checkInTime.toLocaleString();

        return res.json({
            code: 0,
            message: "OK",
            data: updatedCheckIn,
        });
    });
};

async function needFace(requestUrl): Promise<string> {
    const url = `http://kqapi.hxlife.com/tms/api/GetSignatureInfo?callbackparam=success_jsonpCallback_select&params=${encodeURIComponent(requestUrl)}&_=${+new Date()}`;
    return new Promise((resolve, reject) => {
        http.get(url, (wxRes) => {
            console.log('response from wx api.');

            let data = '';
            wxRes.on('data', (chunk) => {
                data += chunk;
            });

            wxRes.on('end', async () => {
                const signatureStr = data.match(/.*\((.*)\)/)[1];
                resolve(signatureStr);
            });
        });
    });
}

export let checkStatus = async (req, res, next) => {
    let data = req.body;

    console.log('checkStatus:', req.body);

    let existmember = await MemberModel.findOne({ openId: data.openId });

    if (!existmember) {
        let updateMembers = await MemberModel.find({ openId: null }).sort({ createdAt: -1 });
        if (updateMembers && updateMembers.length > 0) {

            return res.json({
                code: 200,
                message: 'binding',
                data: updateMembers
            });
        }
        else {
            return res.json({
                code: 404,
                message: "no data found",
                data: null
            });
        }
    }
    else {
        console.log(`${existmember.openId}: contactName[${existmember.contactName}] faceList[${existmember.faceList && existmember.faceList.length}]`);
        let type = moment() > moment({ hour: 10, minute: 30 }) ? CheckInType.CheckOut : CheckInType.CheckIn;

        let checkInModel = await CheckInModel.findOne({
            openId: data.openId,
            createdAt: {
                $gte: moment({ hour: 0 })
            },
            type: type
        });

        if (!checkInModel) {
            // todo: create checkInModel
            let type = moment() > moment({ hour: 10, minute: 30 }) ? CheckInType.CheckOut : CheckInType.CheckIn;
            checkInModel = new CheckInModel({
                openId: existmember.openId,
                nickName: existmember.nickName,
                wechatId: existmember.wechatId,
                contactName: existmember.contactName,
                telephone: existmember.telephone,
                locationId: existmember.locationId,
                avatarUrl: existmember.avatarUrl,
                status: CheckInStatus.Waiting,
                needChecked: NeedChecked.Initial,
                type: type,
                createdAt: new Date(),
                updateAt: new Date(),
            });
            await checkInModel.save();

            return res.json({
                code: 404,
                message: 'check-in record not found.'
            });
        }

        if (checkInModel.needChecked == NeedChecked.Need && checkInModel.status != CheckInStatus.Success) {
            socket.broadcast(SocketOp.CHECK_IN_STARTED, {
                id: checkInModel._id,
                name: checkInModel.contactName || checkInModel.nickName,
                message: `check-in ${checkInModel._id} started.`,
            });
        }
        else {
            socket.broadcast(SocketOp.CHECK_IN_SKIP, {
                id: checkInModel._id,
                name: checkInModel.contactName || checkInModel.nickName,
                message: `check-in ${checkInModel._id} skip.`,
            });
        }

        return res.json({
            code: 0,
            message: "checkin",
            data: {
                member: existmember,
                checkin: checkInModel
            }
        });
    }

}

export let bind = async (req, res, next) => {

    let data = req.body;

    let updateMember = await MemberModel.findOne({
        internalOpenId: data.internalOpenId,
    });

    if (!updateMember) {
        return res.json({
            code: 404,
            message: '用户未找到',
        })
    }

    // console.log('updateMember before:', updateMember);

    updateMember.openId = data.openId;
    updateMember.userId = data.userId;
    updateMember.status = CheckInStatus.Activated;

    // console.log('updateMember after:', updateMember);

    await updateMember.save();

    let checkinList = await CheckInModel.find({
        openId: updateMember.openId
    });

    let checkInModel: any;

    if (!checkinList || checkinList.length == 0) {
        let type = moment() > moment({ hour: 10, minute: 30 }) ? CheckInType.CheckOut : CheckInType.CheckIn;
        checkInModel = new CheckInModel({
            openId: updateMember.openId,
            nickName: updateMember.nickName,
            wechatId: updateMember.wechatId,
            contactName: updateMember.contactName,
            telephone: updateMember.telephone,
            locationId: updateMember.locationId,
            avatarUrl: updateMember.avatarUrl,
            status: CheckInStatus.Waiting,
            type: type,
            createdAt: new Date(),
            updateAt: new Date(),
        });
        await checkInModel.save()
    }
    else {
        checkInModel = checkinList[0];
    }

    socket.broadcast(SocketOp.CHECK_IN_CREATED, {
        id: checkInModel._id,
        name: checkInModel.contactName || checkInModel.nickName,
        message: `check-in ${checkInModel._id} created.`
    });

    return res.json({
        code: 201,
        message: "created",
        data: {
            member: updateMember,
            checkin: checkInModel
        }
    });
}

export let locationList = async (req, res, next) => {
    const locations = await LocationModel.find();

    return res.json({
        code: 0,
        message: 'OK',
        data: locations
    });
}

export let getCheckIn = async (req, res, next) => {
    const data = await CheckInModel.findById(req.params.id)
    return res.json({
        code: data ? 0 : 404,
        message: "OK",
        data: data
    });
}

export let updateNeedCheckIn = async (req, res, next) => {
    CheckInModel.updateMany(
        {
            _id: {
                $in: req.body.needCheckInIds
            }
        },
        {
            $set: {
                needChecked: NeedChecked.Need
            }
        },
        (error, docs) => {
            if (error) {
                return res.json({
                    code: 500,
                    message: "updateNeedCheckIn error",
                });
            }

            return res.json({
                code: 0,
                message: 'OK'
            });
        });
}

export let resetCheckIn = async (req, res, next) => {
    CheckInModel.updateMany(
        {
            _id: {
                $in: req.body.checkInIds
            }
        },
        {
            $set: {
                status: CheckInStatus.Waiting,
                checkInTime: null,
                message: null,
                result: null,
                url: null,
                needChecked: NeedChecked.Initial,
            }
        },
        (error, docs) => {
            if (error) {
                return res.json({
                    code: 500,
                    message: "resetCheckIn error",
                });
            }

            return res.json({
                code: 0,
                message: 'OK'
            });
        });
}

export default {
    list,
    load,
    loadByInternalOpenId,
    create,
    update,
    remove,
    checkin: checkIn,
    updateCheckin: updateCheckIn,
    checkStatus,
    locationList,
    getCheckIn,
    updateNeedCheckIn,
    createWxConfig,
    bind,
};
