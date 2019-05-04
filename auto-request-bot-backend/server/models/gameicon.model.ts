
import { prop, Typegoose, ModelType, InstanceType, pre } from 'typegoose';

/**
 * GameIcon Schema
 */
export class GameIcon extends Typegoose {
    @prop()
    iconId: Number;
    @prop()
    iconName: String;
    @prop()
    iconEnabled?: String;
    @prop()
    iconDisabled?: String;
}

const GameIconModel = new GameIcon().getModelForClass(GameIcon, {
    schemaOptions: {
        timestamps: true,
    },
});

export default GameIconModel;
