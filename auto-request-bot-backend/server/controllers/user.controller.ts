
import { Request, Response, NextFunction } from "express";
import { IncomingMessage } from 'http';
import * as jwt from 'jsonwebtoken';
import * as httpStatus from 'http-status';

import { config } from '../config/config';
import { APIError } from '../helpers/APIError';
import UserModel, { User } from '../models/user.model';
import { Md5 } from 'ts-md5/dist/md5';

export let register = async (req, res, next) => {
    let data = req.body;
    data.password = Md5.hashStr(data.password);
    let existuser = await UserModel.findOne({ username: data.username });
    if (!existuser) {
        let user = new UserModel(data);
        await user.save();
        return res.json({
            code: 0,
            message: "OK",
            data: user
        });
    } else {
        return res.json({
            code: 500,
            message: "当前用户已经存在,不能重复注册"
        });
    }
};

export let login = async (req, res, next) => {
    let data = req.body;
    data.password = Md5.hashStr(data.password);
    let existuser = await UserModel.findOne({ username: data.username,password:data.password });
    if(existuser){
        return res.json({
            code: 0,
            message: "OK",
            data: existuser
        });
    }else{
        return res.json({
            code: 500,
            message: "用户或密码错误,请重新输入",
            data: data
        });
    }
}

export default { register,login };
