import { Component, computed, inject, input } from '@angular/core';
import { RouterLink } from '@angular/router';


import { LikesService } from '../../_services/likes.service';
import { Member } from '../../_models/Member';
import { PresenceService } from '../../_services/presence.service';

@Component({
  selector: 'app-member-card',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './member-card.component.html',
  styleUrl: './member-card.component.css'
})
export class MemberCardComponent {
member = input.required<Member>();
private likeservice = inject(LikesService);
private presenceservice = inject(PresenceService);
hasliked = computed(()=> this.likeservice.likesIds().includes(this.member().id));
isOnline = computed(()=> this.presenceservice.onlineUsers().includes(this.member().username));

  togglelikes(){
    this.likeservice.toggleLike(this.member().id).subscribe(
      {
        next: () =>{
          if (this.hasliked()){
            this.likeservice.likesIds.update(ids => ids.filter(x => x !== this.member().id ))
          }
          else{
            this.likeservice.likesIds.update(ids => [...ids,this.member().id])
          }
        }
      }
    )
  }
}
