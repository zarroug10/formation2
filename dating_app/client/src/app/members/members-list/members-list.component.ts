import { Component, inject, OnInit } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { MemberCardComponent } from "../member-card/member-card.component";
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { AccountService } from '../../_services/account.service';
import { UserParams } from '../../_models/UserParams';
import { FormsModule} from '@angular/forms';
import { ButtonsModule } from 'ngx-bootstrap/buttons';

@Component({
  selector: 'app-members-list',
  standalone: true,
  imports: [MemberCardComponent,PaginationModule,FormsModule,ButtonsModule],
  templateUrl: './members-list.component.html',
  styleUrl: './members-list.component.css'
})
export class MembersListComponent implements OnInit {

   memberservice = inject(MembersService)
   private accountservice = inject(AccountService)
   userParams = new UserParams(this.accountservice.currentUser());
   genderList = [{value:'male',display:'Males'},{value:'female',display:'Females'}]
  ngOnInit(): void {
    if(!this.memberservice.paginatedResults())
    this.loadMembers();
  }

  loadMembers(){
    this.memberservice.getMembers(this.userParams)
  }
  resetFilters(){
    this.userParams = new UserParams(this.accountservice.currentUser());
    this.loadMembers();
  }

  Pagechanged(event: any){
    if(this.userParams.PageNumber != event.page){
      this.userParams.PageNumber = event.page ;
      this.loadMembers();
    }
  }

}
