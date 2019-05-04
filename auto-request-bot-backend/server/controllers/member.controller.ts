
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

            return res.json({
                code: 0,
                message: "OK",
                data: docs
            });
        });
    }
    else {
        return res.json({
            code: 0,
            message: "OK",
            data: checkinList
        });
    }
};

export let updateCheckin = async (req, res, next) => {

    const checkInTime = new Date();

    await CheckInModel.findByIdAndUpdate(req.params.id, {
        status: req.body.status,
        checkInTime: checkInTime,
        message: req.body.message,
    }, (error, updatedCheckin) => {
        if (error) {
            return res.json({
                code: 500,
                message: "Remove Error",
            });
        }

        ((updatedCheckin.checkInTime) as any) = checkInTime;

        return res.json({
            code: 0,
            message: "OK",
            data: updatedCheckin,
        });
    });
};

export default { list, load, create, update, remove, checkin, updateCheckin };
