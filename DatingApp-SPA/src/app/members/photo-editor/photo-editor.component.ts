import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { Photo } from 'src/app/models/Photo';
import { AlertifyService } from 'src/app/services/alertify.service';
import { AuthService } from 'src/app/services/auth.service';
import { UserService } from 'src/app/services/user.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
//this Component is a child component of our member-edit Component
//The user that comes from member-edit has some photos so we retrieve thouse with Input


export class PhotoEditorComponent implements OnInit {
  @Input() photos: Photo[];
  //WE WILL OUTPUT a string  (photo url)
  @Output() getMemberPhotoChange = new EventEmitter<string>();
   uploader: FileUploader; //we need to provide a url (baseURL) here we initialize from method initilaizUploader()
   hasBaseDropZoneOver : boolean = false;
   baseUrl = environment.apiUrl;
   //for setting mainPhoto
   currentMain:Photo;
  constructor(private authService: AuthService,private userService:UserService,private alertify:AlertifyService) { }

  ngOnInit() {
    this.initializeUploader();
  }
  fileOverBase(e:any):void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader(){
    this.uploader = new FileUploader({
      url: this.baseUrl+'users/'+this.authService.decodedToken.nameid+'/photos',
      authToken:"Bearer "+localStorage.getItem('token'),
      isHTML5: true,
      //allow only image
      allowedFileType:['image'],
      removeAfterUpload:true,
      autoUpload:false, //we want user to click button to send this up
      maxFileSize: 10*1024*1024 //10 mb max
    });

    //Fixing the cors error
    this.uploader.onAfterAddingFile = (file) =>{
      //this file with credential is not the same as sending it wit Bearer authToken
      file.withCredentials = false;
    }

    //Show image immediatly after uploading
    this.uploader.onSuccessItem = (item,response,status,header)=>{
      if(response){
        //response is a string so we convert the string into an object cuz the photo is object
        const res: Photo = JSON.parse(response);
        //we build a photo object from object that returns from our server
        const photo = {
          id: res.id,
          url: res.url,
          dateAdded: res.dateAdded,
          description: res.description,
          isMain:res.isMain
        };
        this.photos.push(photo); //we push this photo in our photo array that comes from memeber-edit
      }
    };
  }
  
  setMainPhoto(photo: Photo){
    this.userService.setMainPhoto(this.authService.decodedToken.nameid,photo.id).subscribe(()=>{
      //fillter all photos except main and make it false 
      this.currentMain = this.photos.filter(p=>p.isMain === true)[0];
      this.currentMain.isMain = false;
      //Set true photo thats being passed as Main 
      photo.isMain = true;
      this.authService.changeMemberPhoto(photo.url);
      //Updating the current user and the one in localStorage with new MainPhoto
      this.authService.currentUser.photoUrl = photo.url;
      localStorage.setItem('user',JSON.stringify(this.authService.currentUser));
    //  this.getMemberPhotoChange.emit(photo.url);
      
    },error=>{
      this.alertify.error(error);
    });
  }
  //photoID, we call this method from our template .html we need to add click event there
  deletePhoto(id:number){
    this.alertify.confirm("Are you sure you want to delete this photo",()=>{
      this.userService.deletePhoto(this.authService.decodedToken.nameid,id).subscribe(()=>{
        //we need to remove photo from our photoarray
        this.photos.splice(this.photos.findIndex(p=>id ===id),1);
        this.alertify.success("Photo has been deleted");
      },error=>{
        this.alertify.error("Failed to delete the photot");
      });
    });
  
  }
}
