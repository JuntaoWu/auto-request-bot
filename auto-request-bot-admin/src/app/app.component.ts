import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { map, switchMap } from 'rxjs/operators';
import { Member } from './member.model';
import { Constants } from './constants';

declare var wx;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'auto-request-bot-admin';

  displayedColumns: string[] = ['nickName', 'contactName', 'avatarUrl', 'checkInTime', 'result', 'message', 'url', 'signatureStr',
    'operation'];
  private members: Member[];

  constructor(private httpClient: HttpClient) {

  }

  async checkFace(member: Member) {

    if (!member || member.result !== 'needface') {
      return;
    }

    const paramStrs = member.message.split('#'); // 'f8c2dcd8f86a49c6a451b899a08469cb#廖星程#0#510823199308040017#T001140703#86510201';

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

    const signatureData = JSON.parse(member.signatureStr);

    wx.config({
      debug: true, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
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
          wx.uploadImage({
            localId: imgLocalId[0], // 需要上传的图片的本地ID，由chooseImage接口获得
            isShowProgressTips: 1, // 默认为1，显示进度提示
            success: (uploadImageRes) => {
              if (res.serverId.indexOf('wxLocalResource://') >= 0) {
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

              this.httpClient.get(Constants.faceSignEndpoint, {
                params: { params: JSON.stringify(requestData) }
              }).subscribe((response) => {
                alert(JSON.stringify(response));
              });
            }
          });
        }
      });
    });
  }

  async ngOnInit() {
    const response = await this.httpClient.get<any>(`${Constants.arbHost}/api/member/checkin?type=0`).toPromise();

    if (!response || !response.data) {
      return;
    }

    this.members = (response.data as Member[]).map(member => {
      return {
        ...member,
        avatarUrl: Constants.arbHost + member.avatarUrl
      };
    });
  }
}
