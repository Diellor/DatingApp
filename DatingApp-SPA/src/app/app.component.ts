import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { User } from './models/user';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'app';
  jwtHelper = new JwtHelperService();

  constructor(private authService: AuthService) {

  }
  ngOnInit() {
    const token = localStorage.getItem('token');
    //getting user
    const user:User = JSON.parse(localStorage.getItem('user')); //this comes as string so we parseit to User OBJ
    if (token) { //if token exists
      this.authService.decodedToken = this.jwtHelper.decodeToken(token);
    }
    if(user){
      this.authService.currentUser = user;
    }

  }
}
