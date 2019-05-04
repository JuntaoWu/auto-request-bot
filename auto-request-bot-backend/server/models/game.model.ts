
import { prop, Typegoose, ModelType, InstanceType } from 'typegoose';

export enum GameStatus {
    Initialized,
    Ready,
    Completed,
}

/**
    ActorCount: Number,
    AppVersion: String,
    AppId: String,
    GameId: String,
    Region: String,
    State: {
        ActorCounter: Number,
        ActorList: [
            {
                ActorNr: Number,
                UserId: String,
                NickName: String,
                Binary: String,
                IsActive: Boolean,
                IsInactive: Boolean,
                DeactivationTime: String
            }
        ],
        Binary: Object,
        CheckUserOnJoin: Boolean,
        CustomProperties: Object,
        DeleteCacheOnLeave: Boolean,
        EmptyRoomTTL: Number,
        IsOpen: Boolean,
        IsVisible: Boolean,
        LobbyType: Number,
        LobbyProperties: Array,
        MaxPlayers: Number,
        PlayerTTL: Number,
        SuppressRoomEvents: Boolean,
        Slice: Number,
    },
    UserId: String,
    NickName: String
 */
class GameState {
    public ActorCounter: Number;
    public Binary: Object;
    public CheckUserOnJoin: Boolean;
    public CustomProperties: Object;
    public DeleteCacheOnLeave: Boolean;
    public EmptyRoomTTL: Number;
    public IsOpen: Boolean;
    public IsVisible: Boolean;
    public LobbyType: Number;
    public LobbyProperties: Array<String>;
    public MaxPlayers: Number;
    public PlayerTTL: Number;
    public SuppressRoomEvents: Boolean;
    public Slice: Number;
}

/**
 * Game Schema
 */
export class Game extends Typegoose {
    @prop()
    ActorCount: Number;
    @prop()
    AppVersion: String;
    @prop()
    AppId: String;
    @prop()
    GameId: String;
    @prop()
    Region: String;
    @prop()
    State: GameState;
    @prop()
    UserId: String;
    @prop()
    NickName: String;
    @prop()
    Players: Number;
    @prop()
    WinCamp: String;
    @prop()
    createdAt: Date;
    @prop()
    updatedAt: Date;
    @prop({ default: GameStatus.Initialized })
    Status: GameStatus;
}

const GameModel = new Game().getModelForClass(Game, {
    schemaOptions: {
        timestamps: true,
    },
});

export default GameModel;
