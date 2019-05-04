import GameModel, { Game, GameStatus } from '../models/game.model';
import { Request, Response, NextFunction } from 'express';
import { IncomingMessage } from 'http';

export let load = async (gameId: string) => {
    return GameModel.findOne({ GameId: gameId });
};

export let create = async (body: any) => {
    const game = new GameModel(body);

    await game.save();
    return GameModel.findOne();
};

export let update = (gameId, body: Game) => {
    return load(gameId).then(async (game) => {
        if (game) {
            const tmp = game;
            game.ActorCount = body.ActorCount;

            game.State = body.State;
        }
        else {
            game = new GameModel(body);
        }
        return await game.save();
    });
};

export let remove = (params: any) => {
    return load(params).then((game) => game.remove());
};

export let ready = async (roomName: string, createdBy: string) => {
    if (!roomName || !createdBy) {
        console.log('GameId or UserId not provided');
        return;
    }
    const game = await GameModel.findOne({ GameId: roomName, UserId: createdBy });
    game.Status = GameStatus.Ready;

    return await game.save();
};

