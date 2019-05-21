import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { User } from 'src/app/models/user';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from 'src/app/services/alertify.service';
import { NgForm } from '@angular/forms';
import { UserService } from 'src/app/services/user.service';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  //We use ViewChild to access the form in thiscomponent.html
  @ViewChild('editForm') editForm:NgForm;
  user:User;
  //We use HostListener to prevent data loss (if user changes something and closes the window(or browser) an alert will be displayed)
  @HostListener('window:beforeunload',['$event'])
  unloadNotification($event:any){
    if(this.editForm.dirty){
      $event.returnValue = true;
    }
  }
 
  //ActivatedRoute for accessing our data 
  constructor(private route:ActivatedRoute,private alertify:AlertifyService,private userService:UserService,private authService:AuthService) { }

  ngOnInit() {
    this.route.data.subscribe(data=>{
      //assigning user with the data from our Route
      this.user = data['user'];
    });
  }
  updateUser(){
    this.userService.updateUser(this.authService.decodedToken.nameid,this.user).subscribe(next=>{
      this.alertify.success("pofile updated successfully");
      //now we have access to all form methods
      //this method resets the state of form as untouched, if we dont give reset(this.user) parameter
      //than it will reset the form and remove text inside 
      this.editForm.reset(this.user);
      //with the this.user parameter now this saves the text as the last saved changes
    },error=>{
      this.alertify.error(error);
    });

  }
  updateMainPhoto(photoUrl) //this url is passed from child component (photo-editor component)
  {
    this.user.photoUrl = photoUrl;
  }

}
