
import { Request, Response, NextFunction } from "express";
import { IncomingMessage } from 'http';
import * as jwt from 'jsonwebtoken';
import * as httpStatus from 'http-status';

import { config } from '../config/config';
import { APIError } from '../helpers/APIError';
import ClubModel, { Club } from '../models/club.model';

export let list = async (req, res, next) => {
    const data = await ClubModel.find();
    return res.json({
        code: 0,
        message: "OK",
        data: data
    });
};

export default { list };