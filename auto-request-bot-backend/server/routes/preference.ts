import * as express from 'express';
import { Request, Response, NextFunction } from 'express';
import * as passport from 'passport';
import * as http from 'http';
import * as https from 'https';
import * as validate from 'express-validation';
import paramValidation from '../config/param-validation';

const router = express.Router();

import * as preferenceCtrl from '../controllers/preference.controller';

router.get('/', preferenceCtrl.list);

router.get('/:platform', validate(paramValidation.loadPreference), preferenceCtrl.load);

export default router;
