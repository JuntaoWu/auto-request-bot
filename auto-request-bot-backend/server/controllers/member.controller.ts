
import { Request, Response, NextFunction } from "express";
import { IncomingMessage } from 'http';
import * as jwt from 'jsonwebtoken';
import * as httpStatus from 'http-status';

import { config } from '../config/config';
import { APIError } from '../helpers/APIError';
import MemberModel, { Member, CheckInStatus } from '../models/member.model';
import CheckInModel, { CheckIn, CheckInType } from '../models/checkin.model';

import * as moment from 'moment';
import * as path from 'path';
import * as http from 'http';

import * as file from 'fs';
import LocationModel from '../models/location.model';

export let authorize = (req, res, next) => {
    const scope = 'snsapi_info';
    return res.redirect(`https://open.weixin.qq.com/connect/oauth2/authorize?appid=${config.wx.appId}&redirect_uri=${config.wx.redirectUrl}&response_type=code&scope=${scope}&state=${encodeURIComponent(req.query.state)}#wechat_redirect`);
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

        res.cookie('wxOpenId', req.user.openId);
        let redirectUrl = decodeURIComponent(req.query.state);
        console.log('state:', redirectUrl);
        if (/\?/.test(redirectUrl)) {
            redirectUrl += `&wxOpenId=${req.user.openId}`;
        }
        else if (/#/.test(redirectUrl)) {
            redirectUrl = redirectUrl.replace(/(.*)#([^#]*)/, `$1?wxOpenId=${req.user.openId}#$2`);
        }
        else {
            redirectUrl += `?wxOpenId=${req.user.openId}`;
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

export let checkin = async (req, res, next) => {

    if(!req.query.type) {
        req.query.type = 0;
    }

    let locations = await LocationModel.find();
    let checkinList = await CheckInModel.find({
        createdAt: {
            $gte: moment({ hour: 0 })
        },
        type: req.query.type
    });

    if (!checkinList || checkinList.length == 0) {
        let members = await MemberModel.find();
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
        let result = checkinList.map(item => {
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

export let updateCheckin = async (req, res, next) => {

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
    }, (error, updatedCheckin) => {
        if (error) {
            return res.json({
                code: 500,
                message: "Remove Error",
            });
        }

        ((updatedCheckin.checkInTime) as any) = checkInTime.toLocaleString();

        return res.json({
            code: 0,
            message: "OK",
            data: updatedCheckin,
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

export default { list, load, create, update, remove, checkin, updateCheckin };
