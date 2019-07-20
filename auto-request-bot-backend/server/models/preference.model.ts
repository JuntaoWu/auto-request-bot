
import { prop, Typegoose, ModelType, InstanceType, pre } from 'typegoose';

/**
 * Preference Schema
 */
export class Preference extends Typegoose {
    @prop()
    platform: String;
    @prop()
    autoMode: Boolean;
    @prop()
    faceTest?: Boolean;
}

const PreferenceModel = new Preference().getModelForClass(Preference, {
    schemaOptions: {
        timestamps: true,
    },
});

export default PreferenceModel;
