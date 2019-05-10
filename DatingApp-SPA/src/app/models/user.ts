import { Photo } from "./Photo";

 export interface User {
     id:number; //conventional is to write lowecase
     username:string;
     knownAs:string;
     age:number;
     gender:string;
     created:Date;
     lastActive:DataCue;
     photoUrl:string;
     city:string;
     country:string;
     
     interests?:string; //this is optional property may or may not have contcrete value
     introduction?:string;
     lookingFor?:string;
     photos?:Photo[]; //create a Photo interface
}
