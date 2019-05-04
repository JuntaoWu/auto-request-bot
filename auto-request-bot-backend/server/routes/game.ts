import * as express from 'express';
import { Request, Response, NextFunction } from 'express';
import * as passport from 'passport';
import * as http from 'http';
import * as https from 'https';
import * as validate from 'express-validation';
import paramValidation from '../config/param-validation';

const router = express.Router();

import * as gameCtrl from '../controllers/game.controller';

router.post('/create', (req, res, next) => {
    console.log("create:", req.body);

    return gameCtrl.create(req.body)
        .then(savedGame => {
            let result = {
                ResultCode: 0,
                Message: "OK"
            };
            return res.json(result);
        })
        .catch(e => next(e));
});

router.post('/load', (req, res, next) => {
    console.log("load:", req.query);

    gameCtrl.load(req.body.GameId)
        .then(game => {
            if (!game && !req.body.CreateIfNotExists) {
                return res.json({
                    ResultCode: 3,
                    Message: "Game not found and won't create",
                    State: "",
                });
            }
            else {
                let result = {
                    ResultCode: 0,
                    Message: "OK",
                    State: game && game.State,
                };
                console.log(result);
                return res.json(result);
            }
        })
        .catch(next);
});

router.post('/save', (req, res, next) => {
    console.log("save:", req.body);

    return gameCtrl.update(req.body.GameId, req.body)
        .then(savedGame => {
            let result = {
                ResultCode: 0,
                Message: "OK"
            };
            return res.json(result);
        })
        .catch(e => next(e));
});

router.post('/ready', (req, res, next) => {
    console.log("save:", req.query);

    return gameCtrl.ready(req.query.roomName, req.query.createdBy)
        .then(savedGame => {
            let result = {
                ResultCode: 0,
                Message: "OK"
            };
            return res.json(result);
        })
        .catch(e => next(e));
});

export default router;
