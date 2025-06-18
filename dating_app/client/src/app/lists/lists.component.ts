import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';


import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { PaginationModule } from 'ngx-bootstrap/pagination';


import { LikesService } from '../_services/likes.service';
import { Member } from '../_models/Member';
import { MemberCardComponent } from "../members/member-card/member-card.component";

@Component({
  selector: 'app-lists',
  standalone: true,
  imports: [FormsModule, ButtonsModule, MemberCardComponent,PaginationModule],
  templateUrl: './lists.component.html',
  styleUrl: './lists.component.css'
})
export class ListsComponent implements OnInit,OnDestroy {
likeservice = inject(LikesService)
predicate ='liked';
pageNumber= 1;
pageSize=5;
ngOnInit(): void {
  this.loadlikes();
}
getTitle(){
  switch (this.predicate) {
    case 'liked':return 'Members You Like'
    case 'likedby':return 'Members Who likes you'  
    default: return 'Mutual'
  }
}

loadlikes(){

  this.likeservice.getlikes(this.predicate,this.pageNumber, this.pageSize)
}
PageChanged(event:any)
{
  if(this.pageNumber !== event.page){
    this.pageNumber = event.pageSize;
    this.loadlikes();
  }
}
ngOnDestroy(): void {
this.likeservice.paginatedResults.set(null)
}

}
