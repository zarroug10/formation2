import { Component, inject, OnDestroy, OnInit, ViewChild, viewChild } from '@angular/core';
import { Member } from '../../_models/Member';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { ActivatedRoute, Router } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TimeagoModule } from 'ngx-timeago';
import { DatePipe } from '@angular/common';
import { MemberMessagesComponent } from "../member-messages/member-messages.component";
import { Message } from '../../_models/Message';
import { MessageService } from '../../_services/message.service';
import { PresenceService } from '../../_services/presence.service';
import { AccountService } from '../../_services/account.service';
import { HubConnectionState } from '@microsoft/signalr';
import { MembersService } from '../../_services/members.service';

@Component({
  selector: 'app-member-details',
  standalone: true,
  imports: [TabsModule, GalleryModule, TimeagoModule,
    DatePipe, MemberMessagesComponent],
  templateUrl: './member-details.component.html',
  styleUrl: './member-details.component.css'
})
export class MemberDetailsComponent implements OnInit , OnDestroy {
  @ViewChild('memberTabs', { static: true }) memberTabs?: TabsetComponent
  private routes = inject(ActivatedRoute);
  private router = inject(Router);
   presenceservice = inject(PresenceService);
   messageservice = inject(MessageService)
  private accountservice = inject(AccountService);
  member: Member = {} as Member;
  images: GalleryItem[] = [];
  activeTab?: TabDirective;

  ngOnInit(): void {
    this.routes.data.subscribe({
      next: data => {
        this.member = data['member'];
        this.member && this.member.photos.map(p => {
          this.images.push(new ImageItem({ src: p.url, thumb: p.url }))
        })
      }
    })

    this.routes.queryParams.subscribe({
      next: params => {
        params['tab'] && this.SelectTab(params['tab'])
      }
    })
  }

  SelectTab(heading: string) {
    if (this.memberTabs) {
      const messageTab = this.memberTabs.tabs.find(x => x.heading === heading)
      if (messageTab) messageTab.active = true;
    }
  }

  onRouteParamsChange(){
    const user = this.accountservice.currentUser();
    if(!user) return;
    if(this.messageservice.hubConnection?.state === HubConnectionState.Connected && this.activeTab?.heading=== "Messages")
      this.messageservice.hubConnection.stop().then(()=>{
        this.messageservice.CreateHubConnection(user,this.member.username);
      })
  }

  OnTabActivated(data: TabDirective) {
    this.activeTab = data;
    this.router.navigate([],{
      relativeTo:this.routes,
      queryParams:{tab:this.activeTab.heading},
      queryParamsHandling:'merge'
    })
    if (this.activeTab.heading === 'Messages' && this.member) {
      const user = this.accountservice.currentUser();
      if(!user) return ;
      this.messageservice.CreateHubConnection(user,this.member.username);
    }else{
      this.messageservice.stopHubConnection();
    }
  }

  
  ngOnDestroy(): void {
    this.messageservice.stopHubConnection();
  }
}
