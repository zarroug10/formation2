import { Component, inject, OnInit } from '@angular/core';
import { MessageService } from '../_services/message.service';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { FormsModule } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';
import { Message } from '../_models/Message';
import { RouterLink } from '@angular/router';
import { PaginationModule } from 'ngx-bootstrap/pagination';

@Component({
  selector: 'app-messages',
  standalone: true,
  imports: [ButtonsModule,FormsModule,TimeagoModule,RouterLink,PaginationModule],
  templateUrl: './messages.component.html',
  styleUrl: './messages.component.css'
})
export class MessagesComponent implements OnInit {

messageservice = inject(MessageService);
container = 'Inbox'
pageNumber = 1;
pageSize = 5;
isOutbox = this.container === 'Outbox'

ngOnInit(): void {
this.loadMessages();
}
loadMessages(){
  this.messageservice.getMessages(this.pageNumber
    ,this.pageSize,this.container);
}

pageChanged(event: any){
  if (this.pageNumber != event.page)
  {
    this.pageNumber = event.page
    this.loadMessages();
  }
}
getRoute(message : Message){
  if (this.container === 'Outbox')
    return `/members/${message.recipientUsername}`
  else return  `/members/${message.senderUsername}`;
}

deleteMessage(id:number)
{
  this.messageservice.deleteMessage(id).subscribe(
    {
      next: ()=> {
        this.messageservice.paginatedResult.update(prev =>{
          if(prev && prev.items)
          {
            prev.items.splice(prev.items.findIndex(
              m => m.id === id
            ),1)
            return prev
          }
          return prev
        })
      }
    })
}
}
