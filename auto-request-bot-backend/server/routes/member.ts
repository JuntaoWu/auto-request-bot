import * as express from 'express';
import { Request, Response, NextFunction } from 'express';
import * as passport from 'passport';
import * as http from 'http';
import * as https from 'https';
import * as validate from 'express-validation';
import paramValidation from '../config/param-validation';

const router = express.Router();

import * as memberCtrl from '../controllers/member.controller';

router.get('/', memberCtrl.list);

router.get('/checkin', memberCtrl.checkin);

router.put('/checkin/:id', memberCtrl.updateCheckin);

router.get('/:id', memberCtrl.load);

router.post('/', memberCtrl.create);

router.put('/:id', memberCtrl.update);

router.delete('/:id', memberCtrl.remove);

export default router;
