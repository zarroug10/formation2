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
   genderList = [{value:'male',display:'Males'},{value:'female',display:'Females'}]
  ngOnInit(): void {
    if(!this.memberservice.paginatedResults())
    this.loadMembers();
  }

  loadMembers(){
    this.memberservice.getMembers()
  }
  resetFilters(){
    this.memberservice.resetUSerParamms();
    this.loadMembers();
  }

  Pagechanged(event: any){
    if(this.memberservice.userParams().PageNumber != event.page){
      this.memberservice.userParams().PageNumber = event.page ;

      this.loadMembers();
    }
  }


}
