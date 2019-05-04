
import { prop, Typegoose, ModelType, InstanceType, pre } from 'typegoose';

/**
 * Member Schema
 *      public string ID { get; set; }
        public Image avatar { get; set; }
        public string weixin_uername { get; set; }
        public string username { get; set; }
        public string telephone { get; set; }
        public string weixin_number { get; set; }
        public string status { get; set; }
        public string registertiem { get; set; }
 */

export class Member extends Typegoose {
    @prop()
    avatar: String;
    @prop()
    weixin_uername: String;
    @prop()
    username: String;
    @prop()
    telephone: String;
    @prop()
    weixin_number: String;
    @prop()
    status: String;
    @prop()
    registertiem: String;
}

const MemberModel = new Member().getModelForClass(Member, {
    schemaOptions: {
        timestamps: true,
    },
});

export default MemberModel;
