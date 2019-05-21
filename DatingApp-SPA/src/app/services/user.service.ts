import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Observable } from "rxjs";
import { User } from "../models/user";

//to use get request we need to provide headers to send a bearer to the server
//so perkohesisht we will manualy create this header and provide it with our token to authentificate our request
//for getting users

//creating httpHeadersOptions
/*REMOVED cuz now we have httpinterceptr that we configured in app.module.ts (it automaticlly attaches the token)
const httpOptions = {
  headers: new HttpHeaders({
    'Authorization':'Bearer '+localStorage.getItem('token')
  })
};
*/
@Injectable({
  providedIn: "root"
})
export class UserService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) {}

  getUsers():Observable<User[]>{
    //the get method returns observable of type Object
    //so we specify that we return array of users in returntype and get method
    return this.http.get<User[]>(this.baseUrl+'users');
  }

  //the id gets here and passed in url to get the request
  getUser(id):Observable<User>{
    return this.http.get<User>(this.baseUrl+'users/'+id);
  }

  updateUser(id:number,user:User){
    return this.http.put(this.baseUrl+'users/'+id,user);
  }
  setMainPhoto(userId:number,id:number){
    //we pass empty object
    return this.http.post(this.baseUrl+'users/'+userId+"/photos/"+id+"/setMain",{});
  }
}
