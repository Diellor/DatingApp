import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
@Injectable({
  providedIn: 'root'
})

export class AuthService {

  baseUrl = 'http://localhost:5000/api/auth/';

  //injectin httpClient in constructor
  constructor(private http: HttpClient) {

  }
  //this takes as parameters our model object that comes from inputs in navbar
  login(model: any) {
    return this.http.post(this.baseUrl + 'login', model).pipe(
      map((response: any) => {
        const user = response; //token is inside user now
        if (user) { //check if there's something inside
          localStorage.setItem('token', user.token);
        }
      })
    );
  }

  register(model: any) {
    return this.http.post(this.baseUrl + 'register', model);
  }
}
