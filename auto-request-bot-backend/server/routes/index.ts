import * as express from 'express';
import { Request, Response, NextFunction } from 'express';
import * as passport from 'passport';
import * as http from 'http';
import * as https from 'https';
import * as validate from 'express-validation';
import paramValidation from '../config/param-validation';

const router = express.Router();

import locationRouter from './location';
import userRouter from './user';
import versionRouter from './version';
import gameRouter from './game';
import clubRouter from './club';
import preferenceRouter from './preference';
import noticeRouter from './notice';
import adRouter from './ad';
import gameIconRouter from './gameicon';
import sharedRouter from './shared';
import imRouter from './im';

router.use('/location', locationRouter);
router.use('/users', userRouter);
router.use('/version', versionRouter);
router.use('/games/:gameId', gameRouter);
router.use('/clubs', clubRouter);
router.use('/preferences', preferenceRouter);
router.use('/notice', noticeRouter);
router.use('/ads', adRouter);
router.use('/gameIcons', gameIconRouter);
router.use('/shared', sharedRouter);
router.use('/im', imRouter);

export default router;
