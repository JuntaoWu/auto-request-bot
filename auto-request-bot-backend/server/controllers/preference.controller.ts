
import { Request, Response, NextFunction } from "express";
import { IncomingMessage } from 'http';
import * as jwt from 'jsonwebtoken';
import * as httpStatus from 'http-status';

import { config } from '../config/config';
import { APIError } from '../helpers/APIError';
import PreferenceModel, { Preference } from '../models/preference.model';

export let list = async (req, res, next) => {
    const data = await PreferenceModel.find();
    return res.json({
        code: 0,
        message: "OK",
        data: data
    });
};

export let load = async (req, res, next) => {
    const data = await PreferenceModel.findOne({ platform: req.params.platform });
    return res.json({
        code: 0,
        message: "OK",
        data: data
    });
};

export default { list, load };