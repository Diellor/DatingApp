import { Routes } from '@angular/router';
import { HomeComponent } from './Home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './guards/auth.guard';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './Resolvers/member-detail.resolver';
import { MemberListResolver } from './Resolvers/member-list.resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './Resolvers/member-edit.resolver';
import { PreventUnsavedChanges } from './guards/prevent-unsaved-changes.guard';

//Routes is array, and each Route is an object
export const appRoutes: Routes = [
  //provide path and component to tell which path matches which component
  //the ordering is importat when user navigates, it searches through these paths and if not found it returns to WildCard (home)
  { path: 'home', component: HomeComponent },
  //One way of guarding
  {
    path: '', //path is empty so it will be match members, messages or lists becouse empty+members = members
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      //All three routes will be protected by canActivate AuthGuard Attribute
      { path: 'members', component: MemberListComponent,resolve:{users:MemberListResolver} },
      //we add our resolver here, we add the object and this is how we will access data from our route
      {path:'members/:id',component:MemberDetailComponent,resolve:{user:MemberDetailResolver}},
      //we will not pass id here we will get ID from decoded Token  
      {path:'member/edit',component:MemberEditComponent, resolve:{user:MemberEditResolver},canDeactivate:[PreventUnsavedChanges]},
      { path: 'messages', component: MessagesComponent},
      { path: 'lists', component: ListsComponent},
    ]
  },

  //Other way of guarding
  /*
  { path: 'members', component: MemberListComponent, canActivate: [AuthGuard]},
  { path: 'messages', component: MessagesComponent, canActivate: [AuthGuard] },
  { path: 'lists', component: ListsComponent, canActivate: [AuthGuard] },
  */
  //WildCardRoute
  //IF Everything that does not match paths above will be redirect to home, **matches every request
  { path: '**', redirectTo: '', pathMatch: 'full' },

];
