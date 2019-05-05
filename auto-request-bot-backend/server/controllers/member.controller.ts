
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

import * as file from 'fs';
import LocationModel from '../models/location.model';

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
    }else {
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

            let result = docs.map(item=>{
                let location = locations.find(loc=>loc._id == item.locationId)
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
                    latitude: location.latitude
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
        let result = checkinList.map(item=>{
            let location = locations.find(loc=>loc._id == item.locationId)
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
                latitude: location.latitude
            }
        })
        return res.json({
            code: 0,
            message: "OK",
            data:result
        });
    }
};

export let updateCheckin = async (req, res, next) => {

    const checkInTime = new Date();

    await CheckInModel.findByIdAndUpdate(req.params.id, {
        status: req.body.status,
        checkInTime: checkInTime.toLocaleString(),
        message: req.body.message,
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

export default { list, load, create, update, remove, checkin, updateCheckin };
