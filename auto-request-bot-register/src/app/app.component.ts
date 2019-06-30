import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  constructor(private httpClient: HttpClient) {

  }

  title = 'auto-request-bot-register';

  register() {
    const internalOpenId = location.search.match(/internalOpenId=([^&#]*)/)[1];
    const locationId = '5cc43f7bc290fb6154005242';
    this.httpClient.post(`${environment.arbHost}api/member/register`, {
      internalOpenId,
      locationId
    }).subscribe((res: any) => {
      if (res.code !== 0) {
        alert(res.message);
        return;
      }

      // tslint:disable-next-line:max-line-length
      location.href = 'http://qyhgateway.ihxlife.com/api/v1/other/query/authorize?timestamp=1546523890746&nonce=7150788195ff4a4fa0ae73d56a4245d0&trade_source=TMS&signature=D5CE85CD68327998A7C78EB0D48B806F&data=%7B%22redirectURL%22%3A%22http%3A%2F%2Ftms.ihxlife.com%2Ftms%2Fhtml%2F1_kqlr%2Fsign.html%22%2C%22attach%22%3A%2200000000000000105723%22%7D';
    });
  }

  ngOnInit(): void {

  }

}
