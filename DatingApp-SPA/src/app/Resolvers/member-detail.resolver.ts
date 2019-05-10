import {Injectable} from '@angular/core';
import { User } from '../models/user';
import {Resolve, Router, ActivatedRouteSnapshot} from '@angular/router';
import { UserService } from '../services/user.service';
import { AlertifyService } from '../services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class MemberDetailResolver implements Resolve<User>{
    //With this resolver now we will get data from our Route not the member-detail onInit method 
    constructor(private userService:UserService,private router:Router,private alertify:AlertifyService){}

    //we need to implement resolve method
    resolve(route:ActivatedRouteSnapshot):Observable<User>{
        //we get the id from route parameter
        //when we use resolve this automaticlly subscribes to the method
        //but we need to catch errors and return the user back so we use pipe()
        return this.userService.getUser(route.params['id']).pipe(
            //all this is for catching the error
            catchError(error=>{
                this.alertify.error("Problem retrieving data");
                this.router.navigate(['/members']);
                //we return observable of null if theres problem
                return of(null);
            })
        )
    }
}