import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Member, CheckInStatus, NeedChecked } from './member.model';
import { environment } from '../environments/environment';

import * as VConsole from 'vconsole';
import { MatDialog, MatSnackBar } from '@angular/material';
import { MemberDetailComponent } from './member-detail/member-detail.component';
import { LocationModel } from './location-model.interface';
import { Preference } from './preference.interface';
import { AuthStoreService } from './auth-store.service';

declare var wx;
declare var $;
declare var WeixinJSBridge;

export interface CheckInServerResponse { result: string; message: string; reason: string; }

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
    vConsole = new VConsole();

    public CheckInStatus = CheckInStatus;

    public title = 'auto-request-bot-admin';

    public loggedIn = false;
    public username = '';
    public password = '';

    public checkInType = '0';
    public filterStatus = CheckInStatus.Activated;

    public displayedColumns: string[] = [
        'wechatId',
        'nickName',
        'contactName',
        'avatarUrl',
        'telephone',
        'locationId',
        'status',
        'createdAt',
        'faceList',
        'operation'
    ];

    public locations: LocationModel[];
    public preference: Preference = {
        manualMode: false,
        faceTest: false,
    };

    private _members: Member[];
    public get members() {
        return this._members;
    }
    public set members(value: Member[]) {
        this._members = value;
        this.filteredMembers = this._members.filter(m => {
            return !this.loggedIn || !this.filterStatus || m.status == this.filterStatus;
        });
    }

    public filteredMembers: Member[];

    public memberModel: Member;
    public checkInModel: Member;

    public checkInParams: {
        openid: string,
        userid: string,
        timestamp: string,
        nonce: string,
        trade_source: string,
        signature: string,
        attach: string,
        token: string,
    };

    constructor(
        private httpClient: HttpClient,
        public dialog: MatDialog,
        public snackBar: MatSnackBar,
        public authStore: AuthStoreService,
    ) {

    }

    reFilterMembers() {
        this.filteredMembers = this._members.filter(m => {
            return !this.loggedIn || !this.filterStatus || m.status == this.filterStatus;
        });
    }

    login() {
        this.httpClient.post(`${environment.arbHost}/api/usermanage/login`, {
            username: this.username,
            password: this.password
        }).subscribe((res: any) => {
            if (res.code === 0) {
                this.authStore.login(res.data.token);
                this.loggedIn = true;
                return this.refresh();
            }
            else {
                this.loggedIn = false;
            }
        });
    }

    openDialog(event, internalOpenId) {
        event && event.preventDefault();
        const dialogRef = this.dialog.open(MemberDetailComponent, {
            width: '480px',
            height: '98vh',
            data: { url: `${environment.arbHost}?internalOpenId=${internalOpenId}` }
        });

        dialogRef.afterClosed().subscribe(result => {
            console.log('The dialog was closed');
        });
    }

    getFaceUrl(face) {
        return face ? `${environment.arbHost}${face}` : '';
    }

    async checkIn(checkInModel: Member, location: LocationModel) {
        const {
            openid,
            userid,
            nonce,
            trade_source,
            signature,
            attach,
            timestamp
        } = this.checkInParams;

        const {
            longitude,
            latitude,
        } = location;

        const customParams = {
            openid,
            userid,
            timestamp,
            nonce,
            trade_source,
            signature,
            qrcodeid: attach,
            attentype: 'morning',
            longitude,
            latitude,
            cacheflag: '0'
        };
        const url = `http://kqapi.hxlife.com/tms/api/QRcodeSign`;
        const checkInResult: CheckInServerResponse = await this.checkInRequest(url, customParams);

        console.log(checkInResult);
        await this.toast(JSON.stringify(checkInResult));
        await this.updateCheckInStatus(checkInModel._id, checkInResult);

        if (checkInResult.result !== 'success') {
            await this.checkFace({
                ...checkInModel,
                ...checkInResult
            });
        }
    }

    async checkInRequest(url, params): Promise<any> {
        console.log(url, params);
        return $.ajax({
            url,
            // data: {pageUrl: location.href.split('#')[0]},
            type: 'get',
            // params意为参数，是自定义的，用以表明这是传给后台的数据。
            data: { params: JSON.stringify(params) },
            contentType: 'application/json; charset=utf-8',
            // 数据类型为jsonp，解决跨域问题。
            dataType: 'jsonp',
            // 自定义的jsonp回调函数名,默认为jQuery自动生成的随机函数
            jsonpCallback: 'success_jsonpCallback',
            // 传递给请求处理程序或页面的,用以获得jsonp回调函数名的参数名(默认为callback)
            jsonp: 'callbackparam'
        });
    }

    async testFace(checkInModel: Member) {
        if (!checkInModel) {
            return;
        }

        // tslint:disable-next-line:max-line-length
        // http://tms.ihxlife.com/tms/html/1_kqlr/sign.html?attach=00000000000000105723&agentCode=510129760&userid=510129760&openid=o43yZt-ax60RdEq2tRID2cKW7Mt4&timestamp=1555230703033&nonce=b422fca3-6745-45fb-942e-3277e4c2872f&trade_source=HXQYH&signature=C5B39A04405C819AB045BD54A3376D59

        // const signatureResponse = await this.httpClient
        //   .get<string>(`http://kqapi.hxlife.com/tms/api/GetSignatureInfo?params=${location.href.split('#')[0]}`).toPromise();

        await this.updateCheckInStatus(checkInModel._id, {
            result: 'needface',
            message: '测试打脸',
        });

        const signatureData = await this.needFace();

        wx.config({
            debug: false && !environment.production, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
            appId: signatureData.appid, // 必填，公众号的唯一标识
            timestamp: signatureData.timestamp, // 必填，生成签名的时间戳
            nonceStr: signatureData.nonceStr, // 必填，生成签名的随机串
            signature: signatureData.signature, // 必填，签名
            jsApiList: ['chooseImage', 'previewImage', 'uploadImage', 'downloadImage'] // 必填，需要使用的JS接口列表
        });

        wx.ready(() => {
            wx.chooseImage({
                count: 1, // 默认9
                sizeType: ['compressed'], // 可以指定是原图还是压缩图，默认二者都有
                sourceType: ['album'], // 可以指定来源是相册还是相机，默认二者都有
                success: (res) => {
                    const imgLocalId = res.localIds;
                    console.log(imgLocalId);
                    wx.uploadImage({
                        localId: imgLocalId[0], // 需要上传的图片的本地ID，由chooseImage接口获得
                        isShowProgressTips: 1, // 默认为1，显示进度提示
                        success: async (uploadImageRes) => {
                            if (uploadImageRes.serverId.indexOf('wxLocalResource://') >= 0) {
                                return;
                            }
                            const mediaId = uploadImageRes.serverId;
                            await this.updateCheckInStatus(checkInModel._id, {
                                result: 'success',
                                message: '测试打脸完成',
                            });
                        },
                        fail: async (error) => {
                            await this.handleError(checkInModel._id, error && JSON.stringify(error));
                        }
                    });
                },
                fail: async (error) => {
                    await this.handleError(checkInModel._id, error && JSON.stringify(error));
                },
                cancel: async () => {
                    await this.handleError(checkInModel._id, 'User canceled chooseImage');
                }
            });
        });
    }

    async checkFace(checkInModel: Member) {

        if (!checkInModel || checkInModel.result !== 'needface') {
            return;
        }

        console.log('Begin checkFace');

        const paramStrs = checkInModel.message.split('#'); // 'f8c2dcd8f86a49c6a451b899a08469cb#廖星程#0#510823199308040017#T001140703#86510201';

        const traceno = paramStrs[0];
        const name = paramStrs[1];
        const idnotype = paramStrs[2];
        const idno = paramStrs[3];
        const traincode = paramStrs[4];
        const managecom = paramStrs[5];

        // tslint:disable-next-line:max-line-length
        // http://tms.ihxlife.com/tms/html/1_kqlr/sign.html?attach=00000000000000105723&agentCode=510129760&userid=510129760&openid=o43yZt-ax60RdEq2tRID2cKW7Mt4&timestamp=1555230703033&nonce=b422fca3-6745-45fb-942e-3277e4c2872f&trade_source=HXQYH&signature=C5B39A04405C819AB045BD54A3376D59

        // const signatureResponse = await this.httpClient
        //   .get<string>(`http://kqapi.hxlife.com/tms/api/GetSignatureInfo?params=${location.href.split('#')[0]}`).toPromise();

        const signatureData = await this.needFace();

        // const signatureData = JSON.parse(member.signatureStr);

        console.log('Begin wx.config');

        wx.config({
            debug: !environment.production, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
            appId: signatureData.appid, // 必填，公众号的唯一标识
            timestamp: signatureData.timestamp, // 必填，生成签名的时间戳
            nonceStr: signatureData.nonceStr, // 必填，生成签名的随机串
            signature: signatureData.signature, // 必填，签名
            jsApiList: ['chooseImage', 'previewImage', 'uploadImage', 'downloadImage'] // 必填，需要使用的JS接口列表
        });

        wx.ready(() => {
            wx.chooseImage({
                count: 1, // 默认9
                sizeType: ['compressed'], // 可以指定是原图还是压缩图，默认二者都有
                sourceType: ['album'], // 可以指定来源是相册还是相机，默认二者都有
                success: (res) => {
                    const imgLocalId = res.localIds;

                    console.log('Begin wx.uploadImage');

                    wx.uploadImage({
                        localId: imgLocalId[0], // 需要上传的图片的本地ID，由chooseImage接口获得
                        isShowProgressTips: 1, // 默认为1，显示进度提示
                        success: (uploadImageRes) => {
                            if (uploadImageRes.serverId.indexOf('wxLocalResource://') >= 0) {
                                console.error('serverId contains wxLocalResource://');;
                                return;
                            }
                            const mediaId = uploadImageRes.serverId;

                            const requestData = {
                                mediaId,
                                traceno,
                                name,
                                idnotype,
                                idno,
                                traincode,
                                managecom
                            };

                            console.log('Begin submit face to faceSignEndpoint.');

                            $.ajax({
                                url: environment.faceSignEndpoint,
                                type: 'get',
                                // params意为参数，是自定义的，用以表明这是传给后台的数据。
                                data: { params: JSON.stringify(requestData) },
                                contentType: 'application/json; charset=utf-8',
                                // 数据类型为jsonp，解决跨域问题。
                                dataType: 'jsonp',
                                // 自定义的jsonp回调函数名,默认为jQuery自动生成的随机函数
                                jsonpCallback: 'success_jsonpCallback_select',
                                // 传递给请求处理程序或页面的,用以获得jsonp回调函数名的参数名(默认为callback)
                                jsonp: 'callbackparam',
                                timeout: 60000,
                                // 访问成功时的回调函数
                                success: async (response) => {
                                    console.log('End receiving faceSignEndpoint response');
                                    console.log(response);
                                    this.toast(response && JSON.stringify(response));
                                    await this.updateCheckInStatus(checkInModel._id, response);
                                },
                                error: async (error) => {
                                    console.error('End submit face to faceSignEndpoint with error.');
                                    await this.handleError(checkInModel._id, error && JSON.stringify(error));
                                }
                            });
                        },
                        fail: async (error) => {
                            await this.handleError(checkInModel._id, error && JSON.stringify(error));
                        }
                    });
                },
                fail: async (error) => {
                    await this.handleError(checkInModel._id, error && JSON.stringify(error));
                },
                cancel: async () => {
                    await this.handleError(checkInModel._id, 'User canceled chooseImage');
                }
            });
        });
    }

    async needFace(): Promise<any> {
        return $.ajax({
            url: `http://kqapi.hxlife.com/tms/api/GetSignatureInfo`,
            // data: {pageUrl: location.href.split('#')[0]},
            type: 'get',
            // params意为参数，是自定义的，用以表明这是传给后台的数据。
            data: { params: location.href.split('#')[0] },
            contentType: 'application/json; charset=utf-8',
            // 数据类型为jsonp，解决跨域问题。
            dataType: 'jsonp',
            // 自定义的jsonp回调函数名,默认为jQuery自动生成的随机函数
            jsonpCallback: 'success_jsonpCallback_select',
            // 传递给请求处理程序或页面的,用以获得jsonp回调函数名的参数名(默认为callback)
            jsonp: 'callbackparam'
        });
    }

    get needLogin() {
        return /honeypot/.test(location.pathname) && !this.loggedIn;
    }

    async ngOnInit() {
        if (this.authStore.loggedIn) {
            this.loggedIn = true;
        }

        await this.getPreference();
        await this.getLocationList();
        await this.checkStatus();
        // await this.refresh();
    }

    async getLocationList() {
        this.httpClient.get(`${environment.arbHost}/api/location`).subscribe((res: any) => {
            if (res.code !== 0) {
                this.toast('Unable to find Locations');
                return;
            }
            this.locations = res.data;
        });
    }

    async getPreference() {
        this.httpClient.get(`${environment.arbHost}/api/preference/honeypot`).subscribe((res: any) => {
            if (res.code !== 0) {
                this.toast('Unable to find Preference');
                return;
            }
            this.preference = res.data;
        });
    }

    /**
      * agentCode: "510129760"
      * attach: "00000000000000105723"
      * nonce: "b422fca3-6745-45fb-942e-3277e4c2872f"
        openid: "o43yZt-ax60RdEq2tRID2cKW7Mt4"
        signature: "C5B39A04405C819AB045BD54A3376D59"
        timestamp: "1555230703033"
        trade_source: "HXQYH"
        userid: "510129760"
     */

    async checkStatus() {

        const checkInParams: any = {};
        location.search.slice(1).split('&')
            .map(i => {
                const obj: any = {};
                obj.key = i.split('=')[0];
                obj.value = i.split('=')[1];
                return obj;
            }).forEach(m => {
                checkInParams[m.key] = m.value;
            });

        this.checkInParams = checkInParams;

        if (this.loggedIn) {
            return this.refresh();
        }

        if (this.needLogin) {
            return;
        }

        this.httpClient.post<any>(`${environment.arbHost}/api/member/checkStatus`, {
            openId: checkInParams.openid,
            userId: checkInParams.userid,
        }).subscribe(res => {
            if (res.code === 200) {
                // start binding:
                this.members = res.data;
                return;
            }

            if (res.code === 404 || !res.data.checkin) {
                this.toast(`用户未找到`);
                return;
            }

            this.memberModel = res.data.member;
            this.checkInModel = res.data.checkin;

            if (this.checkInModel.needChecked !== NeedChecked.Need
                || this.checkInModel.status === CheckInStatus.Success) {
                return this.closeWindow();
            }

            // if (this.checkInModel.result === 'needface') {
            //     this.checkFace(this.checkInModel);
            // } else {
            // todo: checkIn
            const location = this.locations.find(item => this.checkInModel.locationId === item.value);

            // debug faceTest
            if (this.preference.faceTest) {
                return this.testFace(this.checkInModel);
            }

            this.checkIn(this.checkInModel, location);
            // }
        });
    }

    bindUser(user: Member) {
        this.httpClient.post(`${environment.arbHost}/api/member/bind`, {
            internalOpenId: user.internalOpenId,
            openId: this.checkInParams.openid,
        }).subscribe((res: any) => {
            if (res.code === 404) {
                this.toast(res.message || '用户未找到');
            }
            else if (res.code === 201) {
                this.toast('用户绑定完成');
                this.closeWindow();
            }
            else {
                this.toast(res.message || '用户绑定失败');
            }
        });
    }

    async refresh() {
        const response = await this.httpClient.get<any>(`${environment.arbHost}/api/member`).toPromise();

        if (!response || !response.data) {
            return;
        }

        this.members = (response.data as Member[]).map(member => {
            return {
                ...member,
                avatarUrl: member.avatarUrl
                    && (member.avatarUrl.startsWith('http') ? member.avatarUrl : `${environment.arbHost}${member.avatarUrl}`)
            };
        });
    }

    async updateCheckInStatus(id, response) {
        const status = response.result === 'success' ? CheckInStatus.Success : CheckInStatus.Error;
        const updateCheckInResult = await this.httpClient.put<any>(`${environment.arbHost}/api/member/checkin/${id}`, {
            status,
            result: response.result,
            message: response.message,
            url: location.href.split('#')[0]
        }).toPromise();

        console.log('End updateCheckInResult', updateCheckInResult);

        if (status === CheckInStatus.Success) {
            return this.closeWindow();
        }

        if (this.checkInModel) {
            this.checkInModel.result = response.result;
            this.checkInModel.message = response.message;
        }

        // if (updateCheckInResult.code === 0) {
        //   this.refresh();
        // }
    }

    isAvailable(member: Member) {
        return member.result === 'needface' && member.url === location.href.split('#')[0];
    }

    async handleError(checkInId, message) {
        console.error(message);
        const snack = this.toast(message, '确定', this.preference.manualMode);
        if (this.preference.manualMode) {
            snack.onAction().subscribe(async _ => {
                await this.updateCheckInStatus(checkInId, {
                    result: 'fail',
                    message,
                });
            });
        } else {
            await this.updateCheckInStatus(checkInId, {
                result: 'fail',
                message,
            });
            this.closeWindow();
        }
    }

    toast(message, action = '确定', manual = false) {
        return this.snackBar.open(message, action, {
            duration: manual ? 0 : 2000,
        });
    }

    closeWindow() {
        setTimeout(() => {
            // tslint:disable-next-line:no-string-literal
            if (window['WeixinJSBridge']) {
                // tslint:disable-next-line:no-string-literal
                window['WeixinJSBridge'].call('closeWindow');
            }
        }, 1000);
    }
}
