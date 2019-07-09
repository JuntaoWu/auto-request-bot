
export enum CheckInStatus {
    Waiting,
    Success,
    Error,
    Actived,
    UnActive
}

export enum NeedChecked {
    Initial,
    NoNeed,
    Need
}

export interface Member {
    _id?: string;
    nickName?: string;
    wechatId?: string;
    contactName?: string;
    telephone?: string;
    avatarUrl?: string;
    status?: CheckInStatus;
    checkInTime?: string;
    result?: string;
    message?: string;
    url?: string;
    signatureStr?: string;
    locationId?: string;
    needChecked?: NeedChecked;
    faceList?: string[];
}
