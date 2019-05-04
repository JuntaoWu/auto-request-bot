
import { Request, Response, NextFunction } from "express";
import { IncomingMessage } from 'http';
import * as jwt from 'jsonwebtoken';
import * as httpStatus from 'http-status';
import * as CryptoJS from 'crypto-js';
import { AES, CipherOption } from 'crypto-js';
import { InstanceType } from "typegoose";

import { config } from '../config/config';
import { APIError } from '../helpers/APIError';
import WxUserModel, { WxUser } from '../models/wxuser.model';

export let load = async (req, res, next) => {
    if (!req.params.unionId) {
        const err = new APIError("unionId not provided", httpStatus.BAD_REQUEST, true);
        return next(err);
    }
    const user = await WxUserModel.findOne({ unionId: req.params.unionId });
    return res.json({
        code: 0,
        message: "OK",
        data: user
    });
};

export default { load };