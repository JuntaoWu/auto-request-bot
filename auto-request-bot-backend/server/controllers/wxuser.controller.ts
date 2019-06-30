import config from '../config/config';
import * as jwt from 'jsonwebtoken';
import { Request, Response, NextFunction } from 'express';
import APIError from '../helpers/APIError';
import WxUserModel, { WxUser } from '../models/wxuser.model';

import * as fs from 'fs';
import * as path from 'path';

export default {};
