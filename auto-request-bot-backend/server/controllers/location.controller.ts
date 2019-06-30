
import { Request, Response, NextFunction } from "express";
import { IncomingMessage } from 'http';
import * as jwt from 'jsonwebtoken';
import * as httpStatus from 'http-status';

import { config } from '../config/config';
import { APIError } from '../helpers/APIError';
import LocationModel, { Location } from '../models/location.model';

export let list = async (req, res, next) => {
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
