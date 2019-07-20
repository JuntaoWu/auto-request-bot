
export enum CheckInStatus {
    Waiting,
    Success,
    Error,
    Activated,
    InActivated,
}

export enum NeedChecked {
    Initial,
    NoNeed,
    Need
}

export interface Member {
    _id: string;

    checkInTime: string;
    result: string;
    message: string;
    url: string;
    signatureStr: string;
    needChecked: NeedChecked;

    // combine member properties.
    internalOpenId: string;
    openId: string;

    wechatId: string;
    nickName: string;
    contactName: string;
    avatarUrl: string;
    telephone: string;
    locationId: string;
    status: CheckInStatus;
    createdAt: Date;
    faceList: string[];
}
