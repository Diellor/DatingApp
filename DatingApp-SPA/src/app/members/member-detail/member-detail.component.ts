import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/services/user.service';
import { User } from 'src/app/models/user';
import { AlertifyService } from 'src/app/services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryOptions, NgxGalleryImage, NgxGalleryAnimation } from 'ngx-gallery';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {

  user:User; 
  //We need this properties for gallery in our Photo tabs
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  constructor(private userService:UserService,private alertify:AlertifyService,private route:ActivatedRoute) { }

  ngOnInit() {
    //now we get the date from route thath we added in routes and created a Resolver
    this.route.data.subscribe(data=>{
      this.user = data['user']; //['user'] must be same that we passed from routes.ts
    })


    //this is how we want our gallery to look
  this.galleryOptions = [
    {
        width: '500px',
        height: '500px',
        imagePercent:100,
        //thumbnailColums- number of images underneth main(largest) image
        thumbnailsColumns: 4,
        imageAnimation:NgxGalleryAnimation.Slide,
        //this pervents user for clicking the screen and go in see the full image
        preview:false
    }
  ];

    //we need to provide this array of object with small medium and big photos 
    //we have the same picture for small medium and big
    this.galleryImages = this.getImages();
  
  }

  getImages(){
    const imageUrls = [];
    for(let i = 0;i< this.user.photos.length;i++){
      imageUrls.push({
        small:this.user.photos[i].url,
        medium:this.user.photos[i].url,
        big:this.user.photos[i].url,
        description: this.user.photos[i].description
      });
    }
    return imageUrls;
  }


  /* we dont need this method cuz we get the data from Route
  //members/3 we need to get the 3 and pass it as parameter so we inject activatedRoute
  loadUser(){
    //the route is string so this id will be string we add the + sign in front so it will force the Id to number
    this.userService.getUser(+this.route.snapshot.params['id']).subscribe((user:User)=>{
      this.user = user;
    },error=>{
     this.alertify.error(error);
    });
  }
  */
}
