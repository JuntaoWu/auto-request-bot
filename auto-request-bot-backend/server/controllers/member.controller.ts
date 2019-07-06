
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

socket.onEvent.subscribe((data) => {
    console.log(`received socket event data`, data);
});

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

        res.cookie('internalOpenId', req.user.internalOpenId);
        let redirectUrl = decodeURIComponent(req.query.state || config.rootUrl);
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

        console.log('redirectTo:', redirectUrl);
        return res.redirect(redirectUrl);
    }
    catch (err) {
        console.error('Login failed,', err);
        return next(err);
    }
};

export let register = async (req: Request, res: Response, next: NextFunction) => {

    console.log('register:', req.body);

    const existingMember = await MemberModel.findOne({ internalOpenId: req.body.internalOpenId });

    if (!existingMember) {
        return res.json({
            code: 404,
            message: 'Member not found'
        });
    }

    existingMember.locationId = req.body.locationId;
    await existingMember.save();

    return res.json({
        code: 0,
        message: "OK",
        data: {
            redirectUrl: config.wx.checkInUrl
        }
    });
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

    let locations = await LocationModel.find();
    let checkInList = await CheckInModel.find({
        createdAt: {
            $gte: moment({ hour: 0 })
        },
        type: req.query.type
    });

    if (!checkInList || checkInList.length == 0) {

        let members = await MemberModel.find();

        socket.broadcast(SocketOp.PLAIN, {
            message: `check-in total ${members.length} records initializing.`
        });

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
                return res.json({
                    code: 500,
                    message: "Load check in list error",
                });
            }

            let result = docs.map(item => {
                let location = locations.find(loc => loc._id == item.locationId)
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
                    longitude: location.longitude,
                    latitude: location.latitude,
                    message: item.message
                }
            })

            return res.json({
                code: 0,
                message: "OK",
                data: result
            });
        });
    }
    else {
        let result = checkInList.map(item => {
            let location = locations.find(loc => loc._id == item.locationId)
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
                longitude: location.longitude,
                latitude: location.latitude,
                message: item.message,
                result: item.result,
                url: item.url,
                signatureStr: item.signatureStr
            }
        })
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
    if (req.body.result === "needface") {
        signatureStr = await needFace(req.body.url);
    }

    await CheckInModel.findByIdAndUpdate(req.params.id, {
        status: req.body.status,
        checkInTime: checkInTime.toLocaleString(),
        result: req.body.result,
        message: req.body.message,
        url: req.body.url,
        signatureStr: signatureStr
    }, (error, updatedCheckIn) => {
        if (error) {
            return res.json({
                code: 500,
                message: "update check-in error.",
            });
        }

        socket.broadcast(SocketOp.CHECK_IN_UPDATED, {
            id: req.params.id,
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

    console.log('existingMember:', existmember);

    if (!existmember) {
        let updateMembers = await MemberModel.find({ openId: null }).sort({ createdAt: -1 });
        if (updateMembers && updateMembers.length > 0) {
            let updateMember = updateMembers[0];

            console.log('updateMember before:', updateMember);

            updateMember.openId = data.openId;
            updateMember.userId = data.userId;

            console.log('updateMember after:', updateMember);

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
        else {
            return res.json({
                code: 404,
                message: "no data found",
                data: null
            });
        }
    }
    else {
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

        if (checkInModel.needChecked == NeedChecked.Need) {
            socket.broadcast(SocketOp.CHECK_IN_STARTED, {
                id: checkInModel._id,
                message: `check-in ${checkInModel._id} started.`,
            });
        }
        else {
            socket.broadcast(SocketOp.CHECK_IN_SKIP, {
                id: checkInModel._id,
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

export default { list, load, create, update, remove, checkin: checkIn, updateCheckin: updateCheckIn, checkStatus, locationList, getCheckIn, updateNeedCheckIn };
