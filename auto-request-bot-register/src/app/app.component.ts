import { Component, OnInit, NgZone } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';
import { Member } from './member.model';
import { LocationModel } from './location.model';
import { Observable, from, forkJoin, of } from 'rxjs';
import { zipAll, combineAll, switchMap } from 'rxjs/operators';

declare var wx;
declare var $;
declare var WeixinJSBridge;

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

    // tslint:disable-next-line:variable-name
    _internalOpenId: string;
    data: Member = {};
    locations: LocationModel[] = [];
    uploadQueue: string[] = [];
    mediaIds: string[] = [];

    constructor(private httpClient: HttpClient, private zone: NgZone) {
    }

    title = 'auto-request-bot-register';

    register() {
        this.httpClient.post(`${environment.arbHost}/api/member/register`, {
            ...this.data,
            internalOpenId: this.internalOpenId,
            mediaIds: this.mediaIds || [],
        }).subscribe((res: any) => {
            this.uploadQueue = [];
            this.mediaIds = [];
            if (res.code !== 0) {
                alert(res.message);
                return;
            }
            alert('数据保存成功');
            this.data = res.data;
            // tslint:disable-next-line:max-line-length
            // location.href = res.data.redirectUrl;
        });
    }

    ngOnInit(): void {
        if (!this.internalOpenId) {
            location.href = `${environment.arbHost}/api/member/authorize`;
            return;
        }

        this.httpClient.get(`${environment.arbHost}/api/location`).subscribe((res: any) => {
            if (res.code === 0) {
                this.locations = res.data;
                this.refresh();
            }
        }, (error => {
            console.error(error);
        }));

        this.httpClient.post(`${environment.arbHost}/api/member/createWxConfig`, {
            url: location.href.split('#')[0],
        }).subscribe((res: any) => {
            const signatureData = res.data;
            wx.config({
                debug: !environment.production, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
                appId: signatureData.appId, // 必填，公众号的唯一标识
                timestamp: signatureData.timestamp, // 必填，生成签名的时间戳
                nonceStr: signatureData.nonceStr, // 必填，生成签名的随机串
                signature: signatureData.signature, // 必填，签名
                jsApiList: ['chooseImage', 'previewImage', 'uploadImage', 'downloadImage', 'getLocalImgData'] // 必填，需要使用的JS接口列表
            });
        });
    }

    refresh() {
        if (!this.internalOpenId) {
            return;
        }

        this.httpClient.get(`${environment.arbHost}/api/member/register/${this.internalOpenId}`).subscribe((res: any) => {
            if (res.code === 0) {
                this.data = res.data;
                this.data.faceList = this.data.faceList || [];
            }
        });
    }

    nativeUpload($event) {
        console.log($event);
    }

    chooseImage() {
        if (!this.isWxBrowser) {
            return;
        }
        wx.chooseImage({
            count: 4, // 默认9
            sizeType: ['compressed'], // 可以指定是原图还是压缩图，默认二者都有
            sourceType: ['camera', 'album'], // 可以指定来源是相册还是相机，默认二者都有
            success: (res) => {
                const imgLocalIds: string[] = res.localIds;
                console.log(imgLocalIds);

                imgLocalIds.forEach(
                    imgLocalId => {
                        wx.getLocalImgData({ // 循环调用 getLocalImgData
                            localId: imgLocalId,
                            success: (localImageDataRes) => {
                                let localData: string = localImageDataRes.localData; // localData是图片的base64数据，可以用img标签显示
                                localData = localData.replace(/\r|\n/g, '')
                                    .replace('data:image/jgp', 'data:image/jpeg'); // iOS 系统里面得到的数据，类型为 image/jpg,因此需要替换一下
                                if (!localData.startsWith('data:image')) {
                                    // 判断是否有这样的头部
                                    // data:image/png;base64,
                                    localData = 'data:image/png;base64,' + localData;
                                }

                                this.zone.run(() => {
                                    this.uploadQueue.push(localData);
                                });
                            }
                        });
                    }
                );

                forkJoin(imgLocalIds.map(imgLocalId => {
                    return new Promise<string>((resolve, reject) => {
                        wx.uploadImage({
                            localId: imgLocalId, // 需要上传的图片的本地ID，由chooseImage接口获得
                            isShowProgressTips: 1, // 默认为1，显示进度提示
                            success: (uploadImageRes) => {
                                if (uploadImageRes.serverId.indexOf('wxLocalResource://') >= 0) {
                                    return reject(null);
                                }
                                const mediaId = uploadImageRes.serverId;
                                return resolve(mediaId);
                            }
                        });
                    });
                })).subscribe(mediaIds => {
                    this.mediaIds = (this.mediaIds || []).concat(mediaIds);
                });
            }
        });
    }

    get isWxBrowser() {
        return true;
    }

    get internalOpenId() {
        if (/internalOpenId=([^&#]*)/.test(location.search)) {
            this._internalOpenId = location.search.match(/internalOpenId=([^&#]*)/)[1];
        }
        return this._internalOpenId;
    }
}
