import { Component, HostListener, inject, OnInit, ViewChild } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { RouterLink } from '@angular/router';


import { TabsModule } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { ToastrService } from 'ngx-toastr';


import { AccountService } from '../../_services/account.service';
import { Member } from '../../_models/Member';
import { MembersService } from '../../_services/members.service';
import { PhotoEditorComponent } from "../photo-editor/photo-editor.component";

@Component({
  selector: 'app-member-edit',
  standalone: true,
  imports: [FormsModule, RouterLink, TabsModule, PhotoEditorComponent,TimeagoModule,DatePipe],
  templateUrl: './member-edit.component.html',
  styleUrl: './member-edit.component.css'
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm?: NgForm;
  @HostListener('window:beforeunload', ['$event'])
  notify($event: any) {
    if (this.editForm?.dirty) {
      $event.returnValue = true;
    }
  }

  member?: Member;
  private accountservice = inject(AccountService);
  private memberservice = inject(MembersService);
  private toatr = inject(ToastrService);

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    var user = this.accountservice.currentUser();// returning the user if he exists this also can be null as it's a signal
    if (!user) return;
    //displaying the user info
    this.memberservice.getMember(user.username).subscribe({
      next: member => this.member = member
    }
    )
  }
  updateMember() {
    this.memberservice.updateMember(this.editForm?.value).subscribe(
      {
        next: _ => {
          this.toatr.success("Profile updated succussfully")
          this.editForm?.reset(this.member);
        }
      })
  }

  onMemberChange(event: Member) {
    this.member = event;
  }
}
