
import { prop, Typegoose, ModelType, InstanceType, pre } from 'typegoose';

/**
 * Preference Schema
 */
export class Preference extends Typegoose {
    @prop()
    platform: String;
    @prop()
    showGuide: Boolean;
    @prop()
    showClub?: Boolean;
    @prop()
    showMore?: Boolean;
    @prop()
    showWeChatLogin?: Boolean;
    @prop()
    showAnonymousLogin?: Boolean;
}

const PreferenceModel = new Preference().getModelForClass(Preference, {
    schemaOptions: {
        timestamps: true,
    },
});

export default PreferenceModel;
