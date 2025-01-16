import { Component, inject, OnInit } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { Member } from '../../_models/Member';
import { ToastrService } from 'ngx-toastr';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TimeagoModule } from 'ngx-timeago';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-member-details',
  standalone: true,
  imports: [TabsModule,GalleryModule,TimeagoModule,DatePipe],
  templateUrl: './member-details.component.html',
  styleUrl: './member-details.component.css'
})
export class MemberDetailsComponent  implements OnInit {
  ngOnInit(): void {
   this.loadmember();
  }
  private memberService = inject(MembersService);
  private routes = inject(ActivatedRoute)
  private toastr = inject(ToastrService)
  member?: Member;
  images: GalleryItem[]=[];

  loadmember(){
    const username = this.routes.snapshot.paramMap.get('username');
    if(!username) return ;
    this.memberService.getMember(username)
    .subscribe(
      {
        next:memberdetails => {
          this.member = memberdetails;
          memberdetails.photos.map(p=>{
            this.images.push(new ImageItem({src:p.url , thumb:p.url}))
          })
        },
        error: error => this.toastr.error(error.error)
      }
    )
  }

}
