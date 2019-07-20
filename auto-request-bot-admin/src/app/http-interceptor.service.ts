import { Injectable } from '@angular/core';
import {
  HttpEvent, HttpInterceptor, HttpHandler, HttpRequest
} from '@angular/common/http';

import { Observable } from 'rxjs';
import { AuthStoreService } from './auth-store.service';

/** Pass untouched request through to the next request handler. */
@Injectable()
export class AuthHttpInterceptor implements HttpInterceptor {

  constructor(private authStore: AuthStoreService) {

  }

  intercept(req: HttpRequest<any>, next: HttpHandler):
    Observable<HttpEvent<any>> {
    const clonedReq = req.clone({
      setParams: {
        token: this.authStore.token
      }
    });
    return next.handle(clonedReq);
  }
}
