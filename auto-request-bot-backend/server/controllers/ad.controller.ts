
import { Request, Response, NextFunction } from "express";
import { IncomingMessage } from 'http';
import * as jwt from 'jsonwebtoken';
import * as httpStatus from 'http-status';

import { config } from '../config/config';
import { APIError } from '../helpers/APIError';
import AdModel, { Ad } from '../models/ad.model';

export let list = async (req, res, next) => {
    const data = await AdModel.find();
    return res.json({
        code: 0,
        message: "OK",
        data: data
    });
};

export default { list };
