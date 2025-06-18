import { AfterContentInit, AfterViewChecked, Component, inject, input, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';


import { BsModalRef } from 'ngx-bootstrap/modal';
import { TimeagoModule } from 'ngx-timeago';


import { MessageService } from '../../_services/message.service';

@Component({
  selector: 'app-member-messages',
  standalone: true,
  imports: [TimeagoModule,FormsModule],
  templateUrl: './member-messages.component.html',
  styleUrl: './member-messages.component.css'
})
export class MemberMessagesComponent implements AfterViewChecked {
  @ViewChild('Messageform') Messageform?: NgForm;
  @ViewChild('scrollMe') scrollContainer?: any;
  messageservice = inject(MessageService);
  username = input.required<string>();
  messageContent = '';

  sendMessage() {
    this.messageservice.sendMessage(this.username(), this.messageContent).then(
      () => {
        this.Messageform?.reset();
        this.scrollToBottom();
      }
    )
  }

  ngAfterViewChecked(): void {
    this.scrollToBottom();
  }

  private scrollToBottom() {
    if (this.scrollContainer) {
      this.scrollContainer.nativeElement.scrollTop = this.scrollContainer.nativeElement.scrollHeight;
    }
  }
}
