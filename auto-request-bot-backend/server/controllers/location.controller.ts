
import { Request, Response, NextFunction } from "express";
import { IncomingMessage } from 'http';
import * as jwt from 'jsonwebtoken';
import * as httpStatus from 'http-status';

import { config } from '../config/config';
import { APIError } from '../helpers/APIError';
import LocationModel, { Location } from '../models/location.model';
import * as crypto from 'crypto';

export let list = async (req, res, next) => {
    const value = crypto.createHash('sha1')
        .update('jsapi_ticket=HoagFKDcsGMVCIY2vOjf9uTPfsSwVm1lk_4T7okp-th0j8lQcRrTQ0ZL4lsPwcHNAuoXS2b092SHvvRm6HEhMQ&nonceStr=6fc7c05f412c4a6e8ab84f00fc61aaf1&timestamp=1562650608&url=https://arb.hzsdgames.com/?internalOpenId=oA2BE1E2TcBfMpIowzj5G12gOFYE')
        .digest('hex').toLowerCase();
    console.log(value);
    const data = await LocationModel.find();
    return res.json({
        code: 0,
        message: "OK",
        data: data.map(item => {
            return {
                text: item.text,
                value: item._id,
                latitude: item.latitude,
                longitude: item.longitude
            };
        })
    });
};

export default { list };
