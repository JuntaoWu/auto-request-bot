import * as express from 'express';
import { Request, Response, NextFunction } from 'express';
import * as passport from 'passport';
import * as http from 'http';
import * as https from 'https';
import * as validate from 'express-validation';
import paramValidation from '../config/param-validation';

const router = express.Router();

import * as memberCtrl from '../controllers/member.controller';

router.get('/authorize', memberCtrl.authorize);

router.route('/login')
    .get(passport.authenticate("localWx", { failWithError: true }), memberCtrl.login);

router.route('/register')
    .post(memberCtrl.register);

router.get('/register/:internalOpenId', memberCtrl.loadByInternalOpenId);

router.post('/createWxConfig', memberCtrl.createWxConfig);

router.get('/', memberCtrl.list);

router.get('/checkin', memberCtrl.checkIn);

router.get('/checkin/:id',memberCtrl.getCheckIn);

router.put('/checkin/:id', memberCtrl.updateCheckIn);

router.get('/:id', memberCtrl.load);

router.post('/', memberCtrl.create);

router.put('/:id', memberCtrl.update);

router.delete('/:id', memberCtrl.remove);

router.post('/checkStatus',memberCtrl.checkStatus);

router.get('/locationList', memberCtrl.locationList);

router.post('/updateNeedCheckIn', memberCtrl.updateNeedCheckIn);

router.post('/resetCheckIn', memberCtrl.resetCheckIn);

export default router;
