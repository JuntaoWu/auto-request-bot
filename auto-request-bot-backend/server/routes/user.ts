import * as express from 'express';
import { Request, Response, NextFunction } from 'express';
import * as passport from 'passport';
import * as http from 'http';
import * as https from 'https';
import * as validate from 'express-validation';
import paramValidation from '../config/param-validation';

const router = express.Router();

import wxUserCtrl from '../controllers/wxuser.controller';
import imController from '../controllers/im.controller';

router.post('/login-wxgame', passport.authenticate(['jwtWx', 'localWxGame']), wxUserCtrl.loginWxGame);

router.post('/login-native', passport.authenticate(['jwtWx', 'localNative']), wxUserCtrl.loginNative);

router.post('/authorize-wxgame', wxUserCtrl.authorizeWxGame);

router.get('/photon-login', (req: Request, res: Response, next: NextFunction) => {
    console.log('photon-login', req.query.userId);
    const userId: string = req.query.userId;

    if (!userId || userId === 'undefined') {
        console.log('No userId provided');
        // Anonymous user login via photon custom auth.
        // Return a new random userId.
        return res.json({
            ResultCode: 1,
            UserId: (Math.random() * 100000).toFixed()
        });
    }
    else if (userId.startsWith('debug')) {
        return res.json({
            ResultCode: 1,
            UserId: userId
        });
    }

    wxUserCtrl.load(req.query).then(async (user) => {
        console.log('photon-login completed:', user && user.userId);

        // if (user && (!user.imAccId || !user.imToken)) {
        //     // create im account now.
        //     // await imController.create(user).catch(error => {
        //     //     console.error('Error occurred while creating IM account', error);
        //     // });
        // }

        return res.json({
            ResultCode: user ? 1 : 2,
            UserId: user && user.userId,
        });
    });
});

export default router;
