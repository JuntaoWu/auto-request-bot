
import { prop, Typegoose, ModelType, InstanceType, pre } from 'typegoose';

/**
 * Ad Schema
 */
export class Ad extends Typegoose {
    @prop()
    src: String;
    @prop()
    href?: String;
}

const AdModel = new Ad().getModelForClass(Ad, {
    schemaOptions: {
        timestamps: true,
    },
});

export default AdModel;
