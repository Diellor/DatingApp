import { Injectable } from "@angular/core";
import { MemberEditComponent } from "../members/member-edit/member-edit.component";
import { CanDeactivate } from "@angular/router";

@Injectable()
export class PreventUnsavedChanges implements CanDeactivate<MemberEditComponent>{
    //we need access to the form inside membereditComponent so we pass as parameter
    canDeactivate(component:MemberEditComponent){
        if(component.editForm.dirty){
            //If form is changed we will alert the user if he click OK he will navigate where he wanted to
            return confirm("Are you sure you want to continue ? Any unsaved changes will be lost");
        }
        //if form is not dirty we will let him continue anyway so we return true
        return true;
    }
}