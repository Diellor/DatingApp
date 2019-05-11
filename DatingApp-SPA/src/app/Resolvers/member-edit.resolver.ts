import{Injectable} from '@angular/core';
import {Resolve, Router, ActivatedRouteSnapshot} from '@angular/router';
import { UserService } from '../services/user.service';
import { AuthService } from '../services/auth.service';
import { AlertifyService } from '../services/alertify.service';
import { User } from '../models/user';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class MemberEditResolver implements Resolve<User>{
    constructor(private userService:UserService,private authService:AuthService,private router:Router,private alertify:AlertifyService){}


    resolve(route:ActivatedRouteSnapshot) :Observable<User>{
        //we will pass the ID from our decoded token
        return this.userService.getUser(this.authService.decodedToken.nameid).pipe(
            catchError(error=>{
                this.alertify.error("Problem retrieving your dataa");
                this.router.navigate(['/members']);
                return of(null);
            })
        )
    }
}