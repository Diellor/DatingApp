import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { AlertifyService } from '../services/alertify.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})

export class NavComponent implements OnInit {
  model: any = {}; //creating empty object that we will store our username and password that are given from form with

  //Injecting AuthService that we created, and Alertify Service
  constructor(private authService: AuthService, private alertify: AlertifyService) {

  }
  ngOnInit() {

  }
  login() {
    this.authService.login(this.model).subscribe(next => {
      this.alertify.success("Logged in successfully");
    }, error => {
        this.alertify.error(error);
    });
  }

  loggedIn() {
    return this.authService.loggedIn(); //now our token is checkt if its valid from authservice
  }

  logOut() {
    localStorage.removeItem('token');
    this.alertify.message("logged out");
  }
}
