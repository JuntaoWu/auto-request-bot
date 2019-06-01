
export enum CheckInStatus {
    Waiting,
    Success,
    Error,
    Actived,
    UnActive
}

export interface Member {
    nickName: string;
    wechatId: string;
    contactName: string;
    avatarUrl: string;
    status: CheckInStatus;
    checkInTime: string;
    result: string;
    message: string;
    url: string;
    signatureStr: string;
}
