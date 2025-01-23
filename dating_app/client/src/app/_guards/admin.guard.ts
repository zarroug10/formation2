import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

export const adminGuard: CanActivateFn = (route, state) => {
  const accountservice = inject(AccountService);
  const toastr = inject(ToastrService);
  if(accountservice.roles().includes("Admin") || accountservice.roles().includes("Moderator"))
  {
    return true;
  } else
  {
    toastr.error('You cannot enter this area')
    return false
  }
};
