import { Injectable } from '@angular/core';
declare let alertify: any;
@Injectable({
  providedIn: 'root'
})

export class AlertifyService {
  constructor() { }

  confirm(message: string, okCallback: () => any) { //we provide okcallback function     //Dialog Box -> Are you sure?
    alertify.comfirm(message, function (e) {
      if (e)//e represents clicking OK
      {
        okCallback();
      } else {
        //if it clicks 
      }
        
    })
  }

  success(message: string) {
    alertify.success(message);
  }
  error(message: string) {
    alertify.error(message);
  }
  warning(message: string) {
    alertify.warning(message);
  }
  message(message: string) {
    alertify.message(message);
  }
}
