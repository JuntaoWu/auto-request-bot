import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthStoreService {

  public _token: string;

  public get token() {
    return this._token;
  }

  private _loggedIn = false;

  public get loggedIn() {
    return this._loggedIn;
  }

  constructor() {
    const existingToken = sessionStorage.getItem('token');
    if (existingToken) {
      this._token = existingToken;
      this._loggedIn = true;
    }
  }

  public login(token) {
    this._token = token;
    this._loggedIn = true;
    sessionStorage.setItem('token', token);
  }
}
