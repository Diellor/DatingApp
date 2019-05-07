 import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { AlertifyService } from '../services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})

export class NavComponent implements OnInit {
  model: any = {}; //creating empty object that we will store our username and password that are given from form with

  //Injecting AuthService that we created, and Alertify Service
  constructor(private authService: AuthService, private alertify: AlertifyService, private router: Router) {

  }
  ngOnInit() {

  }
  login() {
    this.authService.login(this.model).subscribe(next => {
      this.alertify.success("Logged in successfully");
      //add routing here or in third parameter thats for completed (when this method is completed) 
        //this.router.navigate(['/members']); - we can add it here to but we used method completed of subscribe
    }, error => {
        this.alertify.error(error);
      }, () => {
        this.router.navigate(['/members']); //when loggin in take user to members 
      });
  }

  loggedIn() {
    return this.authService.loggedIn(); //now our token is checkt if its valid from authservice
  }

  logOut() {
    localStorage.removeItem('token');
    this.alertify.message("logged out");
    this.router.navigate(['/home']);
  }
}
