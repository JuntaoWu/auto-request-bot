
import { Request, Response, NextFunction } from "express";
import { IncomingMessage } from 'http';
import * as jwt from 'jsonwebtoken';
import * as httpStatus from 'http-status';
import { createHash } from 'crypto';
import * as CryptoJS from 'crypto-js';
import { AES, CipherOption } from 'crypto-js';
import { InstanceType } from "typegoose";
import * as https from 'https';
import * as uuid from 'uuid/v4';

import { config } from '../config/config';
import { APIError } from '../helpers/APIError';
import WxUserModel, { WxUser } from '../models/wxuser.model';
import * as _ from 'lodash';

export async function load(req, res, next) {
    return res.json({
        error: false,
        code: 0,
        message: "OK",
        data: {
            imAccId: req.user.imAccId,
            imToken: req.user.imToken
        }
    });
}

export async function create(user: InstanceType<WxUser>): Promise<any> {
    const hostname = config.im.host;
    const path = `/nimserver/user/create.action`;
    console.log(hostname, path);

    const imAccId = uuid().replace(/-/g, '');

    const data = [];
    data.push({ key: 'accid', value: imAccId });
    data.push({ key: 'name', value: user.nickName });
    data.push({ key: 'icon', value: user.avatarUrl });

    const dataToPost = data.filter(item => !!item)
        .map(item => `${item.key}=${item.value}`)
        .join('&');

    return new Promise((resolve, reject) => {
        const request = https.request({
            hostname: hostname,
            port: 443,
            path: path,
            method: 'POST',
            headers: getGeneralHeaders(),
        }, (wxRes) => {
            console.log('response from im api.', path);
            let resData = '';
            wxRes.on('data', (chunk) => {
                resData += chunk;
            });
            wxRes.on('end', async () => {

                try {
                    const result = JSON.parse(resData);

                    let { code, info } = result;
                    if (!code || code !== 200) {
                        console.error(result);
                        return reject(result);
                    }
                    else {
                        user.imAccId = info.accid;
                        user.imToken = info.token;
                        await user.save();

                        return resolve(user);
                    }
                }
                catch (ex) {
                    return reject(ex);
                }
            });
        });

        request.end(dataToPost);
    });
}

function getGeneralHeaders() {
    const appKey = config.im.appKey;
    const appSecret = config.im.appSecret;
    const nonceStr = _.random(100000, 999999);
    const curTime = Math.floor(+new Date() / 1000);

    const hash = createHash('sha1');
    const checksum = hash.update(`${appSecret}${nonceStr}${curTime}`).digest('hex').toLowerCase();

    console.log(`AppKey: ${appKey}, Nonce: ${nonceStr}, CurTime: ${curTime}, CheckSum: ${checksum}`);

    return {
        'AppKey': config.im.appKey,
        'Nonce': nonceStr,
        'CurTime': curTime,
        'CheckSum': checksum,
        'Content-Type': 'application/x-www-form-urlencoded;charset=utf-8;'
    };
}

export default { load, create };
