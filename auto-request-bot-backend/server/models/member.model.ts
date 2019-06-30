
import { prop, Typegoose, ModelType, InstanceType, pre } from 'typegoose';

/**
 * Member Schema
 *      public string ID { get; set; }
        public Image avatar { get; set; }
        public string weixin_username { get; set; }
        public string username { get; set; }
        public string telephone { get; set; }
        public string weixin_number { get; set; }
        public string status { get; set; }
        public string registertime { get; set; }
 */

export enum CheckInStatus {
    Waiting,
    Success,
    Error,
    Activated,
    UnActive
}

export class Member extends Typegoose {
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
    @prop({ default: CheckInStatus.Activated })
    status: CheckInStatus;
    @prop()
    registerTime: String;
    @prop()
    internalOpenId: String;
    @prop()
    internalUnionId: String;
}

const MemberModel = new Member().getModelForClass(Member, {
    schemaOptions: {
        timestamps: true,
    },
});

export default MemberModel;
