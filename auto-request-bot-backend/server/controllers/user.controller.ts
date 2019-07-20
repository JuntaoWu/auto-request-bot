
import { Request, Response, NextFunction } from "express";
import { IncomingMessage } from 'http';
import * as jwt from 'jsonwebtoken';
import * as httpStatus from 'http-status';

import { config } from '../config/config';
import { APIError } from '../helpers/APIError';
import UserModel, { User } from '../models/user.model';
import { Md5 } from 'ts-md5/dist/md5';
import { hashSync, genSalt, genSaltSync } from 'bcrypt-nodejs';

export let register = async (req, res, next) => {
    let data = req.body;
    const passwordHash = hashSync(data.password);
    let existingUser = await UserModel.findOne({ username: data.username });
    if (!existingUser) {
        let user = new UserModel({
            username: data.username,
            password: passwordHash,
            telephone: data.telephone,
        });
        await user.save();
        return res.json({
            code: 0,
            message: "请联系管理员激活该用户",
            data: {
                username: req.body.username
            }
        });
    }
    else {
        return res.json({
            code: 500,
            message: "当前用户已经存在,不能重复注册"
        });
    }
};

export let login = async (req, res, next) => {
    const token = jwt.sign({
        username: req.user.username
    }, config.jwtSecret);
    return res.json({
        code: 0,
        message: 'OK',
        data: {
            token
        }
    });
}

export default { register, login };
