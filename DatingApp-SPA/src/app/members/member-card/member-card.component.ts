import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/models/user';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  //We are going to pass the user from memberList Component to this component(Child component)
  @Input() user: User;
  constructor() { }

  ngOnInit() {
  }

}
