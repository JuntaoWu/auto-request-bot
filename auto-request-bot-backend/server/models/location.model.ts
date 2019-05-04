
import { prop, Typegoose, ModelType, InstanceType, pre } from 'typegoose';

/**
 * Location Schema
 */
export class Location extends Typegoose {
    @prop()
    text: String;
    @prop()
    longitude: Number;
    @prop()
    latitude: Number;
}

const LocationModel = new Location().getModelForClass(Location, {
    schemaOptions: {
        timestamps: true,
    },
});

export default LocationModel;
