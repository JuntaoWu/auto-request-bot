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
    this.httpClient.post(`${environment.arbHost}/api/member/register`, {
      internalOpenId,
      locationId
    }).subscribe((res: any) => {
      if (res.code !== 0) {
        alert(res.message);
        return;
      }

      // tslint:disable-next-line:max-line-length
      location.href = res.data.redirectUrl;
    });
  }

  ngOnInit(): void {

  }

}
