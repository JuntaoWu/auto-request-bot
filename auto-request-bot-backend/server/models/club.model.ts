
import { prop, Typegoose, ModelType, InstanceType, pre } from 'typegoose';

/**
 * Club Schema
 */
export class Club extends Typegoose {
    @prop()
    id: Number;
    @prop()
    province?: String;
    @prop()
    city?: String;
    @prop()
    district?: String;
    @prop()
    level?: Number;
    @prop()
    name?: String;
    @prop()
    time?: String;
    @prop()
    phone?: String;
    @prop()
    attr?: String;
    @prop()
    games?: Array<Number>;
}

const ClubModel = new Club().getModelForClass(Club, {
    schemaOptions: {
        timestamps: true,
    },
});

export default ClubModel;
