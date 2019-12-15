import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonConfig } from '../config/CommonConfig';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from '../../environments/environment';
import { User } from '../models/User';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  config: CommonConfig;
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User;
  photoUrl = new BehaviorSubject('../../assets/default.png');
  currentPhotoUrl = this.photoUrl.asObservable();

  constructor(private http: HttpClient) {
    this.config = new CommonConfig();
  }

  changeMemberPhoto(photoUrl) {
    this.photoUrl.next(photoUrl);
  }

  login(model: any) {
    return this.http
      .post(environment.baseUrl + environment.loginUrl, model)
      .pipe(
        map((resp: any) => {
          const user = resp;
          if (user) {
            localStorage.setItem('token', user.token);
            localStorage.setItem('user', JSON.stringify(user.user));
            this.currentUser = user.user;
            this.decodedToken = this.jwtHelper.decodeToken(user.token);
            this.currentUser.photoUrl = this.currentUser.photoUrl ? this.currentUser.photoUrl : '../../assets/default.png';
            this.changeMemberPhoto(this.currentUser.photoUrl);
          }
        })
      );
  }

  register(model: User) {
    return this.http.post(environment.baseUrl + environment.registerUrl, model);
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  }
}
