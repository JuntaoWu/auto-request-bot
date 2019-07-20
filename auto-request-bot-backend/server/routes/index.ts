import * as express from 'express';
import { Request, Response, NextFunction } from 'express';
import * as passport from 'passport';
import * as http from 'http';
import * as https from 'https';
import * as validate from 'express-validation';
import paramValidation from '../config/param-validation';

const router = express.Router();

import locationRouter from './location';
import memberRouter from './member';
import preferenceRouter from './preference';
import usermanageRouter from './user-manage'

router.use('/location', locationRouter);
router.use('/member', memberRouter);
router.use('/usermanage', usermanageRouter);
router.use('/preference', preferenceRouter);

export default router;
