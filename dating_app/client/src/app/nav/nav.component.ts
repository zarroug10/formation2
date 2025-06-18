import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';


import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ToastrService } from 'ngx-toastr';


import { AccountService } from '../_services/account.service';
import { HasRoleDirective } from '../_directives/has-role.directive';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [FormsModule,BsDropdownModule,RouterLink,RouterLinkActive,HasRoleDirective],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css'
})
export class NavComponent {
private route = inject(Router);
private toastr =inject(ToastrService)
model:any = {};
accountService = inject(AccountService)


login(){
  this.accountService.login(this.model).subscribe({
    next:() => {
      this.route.navigateByUrl("/members")
      this.toastr.success("login succussful")
    },
    error: error => this.toastr.error(error.error)
  })
  console.log(this.model);
}
logout(){
  this.accountService.logout();
  this.route.navigateByUrl("/")
}
}
