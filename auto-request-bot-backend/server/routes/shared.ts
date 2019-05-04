import * as express from 'express';
import { Request, Response, NextFunction } from 'express';
import * as passport from 'passport';
import * as http from 'http';
import * as https from 'https';
import * as validate from 'express-validation';
import paramValidation from '../config/param-validation';

const router = express.Router();

import sharedCtrl from '../controllers/shared.controller';

router.get('/:unionId',
    validate(paramValidation.getShared),
    passport.authenticate("jwtService"),
    sharedCtrl.load);

export default router;
