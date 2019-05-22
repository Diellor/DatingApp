import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { User } from '../models/user';
import {BehaviorSubject} from 'rxjs';



@Injectable({
  providedIn: 'root'
})

export class AuthService {

  baseUrl = environment.apiUrl+"auth/";
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser:User;
  //Creating BehaviourSubject and Setting up the default value 
  photoUrl = new BehaviorSubject<string>('../../assets/user.png');
  //becouse its observable we can subscribe this, so when this is updated the photo is updated in every component thats called
  currentPhotoUrl = this.photoUrl.asObservable();
  //let fullname: string = "dd";

  //injectin httpClient in constructor
  constructor(private http: HttpClient) {

  }
  //we call this method everytime the main photo is updated and pass the new pohoto url
  changeMemberPhoto(photoUrl:string){
    //next -> we pass the value and this will update the photoUrl with the one passed in parameters
    this.photoUrl.next(photoUrl);
  }


  //this takes as parameters our model object that comes from inputs in navbar
  login(model: any) {
    return this.http.post(this.baseUrl + 'login', model).pipe(
      map((response: any) => {
        const user = response; //token is inside user now
        if (user) { //check if there's something inside
          localStorage.setItem('token', user.token);
          //adding user
          localStorage.setItem('user',JSON.stringify(user.user));
          this.currentUser = user.user;
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.changeMemberPhoto(this.currentUser.photoUrl);
        }
      })
    );
  }

  register(model: any) {
    return this.http.post(this.baseUrl + 'register', model);
  }
  loggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token); //returns true if there's problem ex. token expired, there is no token etc. so we use !
    //if false than its OK token is available
  }
}
