
import { prop, Typegoose, ModelType, InstanceType, pre } from 'typegoose';

export enum CheckInType {
    CheckIn,
    CheckOut,
}

export enum CheckInStatus {
    Waiting,
    Success,
    Error,
    Activated,
    UnActive
}

export enum NeedChecked {
    Initial,
    NoNeed,
    Need
}

export class CheckIn extends Typegoose {
    @prop()
    openId: String;
    @prop()
    nickName: String;
    @prop()
    wechatId: String;
    @prop()
    contactName: String;
    @prop()
    telephone: String;
    @prop()
    locationId: String;
    @prop()
    avatarUrl: String;
    @prop({ default: CheckInStatus.Waiting })
    status: CheckInStatus;
    @prop()
    checkInTime: String;
    @prop({ default: CheckInType.CheckIn })
    type: CheckInType;
    @prop()
    result: String;
    @prop()
    message: String;
    @prop()
    url: String;
    @prop()
    signatureStr: String;
    @prop()
    createAt: Date;
    @prop()
    updatedAt: Date;
    @prop({ default: NeedChecked.Initial })
    needChecked: NeedChecked
}

const CheckInModel = new CheckIn().getModelForClass(CheckIn, {
    schemaOptions: {
        timestamps: true,
    },
});

export default CheckInModel;
