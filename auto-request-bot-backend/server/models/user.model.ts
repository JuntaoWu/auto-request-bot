
import { prop, Typegoose, ModelType, InstanceType, pre } from 'typegoose';


export class User extends Typegoose {
    @prop()
    username: String;
    @prop()
    telephone: String;
    @prop()
    password:String;
}

const UserModel = new User().getModelForClass(User, {
    schemaOptions: {
        timestamps: true,
    },
});

export default UserModel;
