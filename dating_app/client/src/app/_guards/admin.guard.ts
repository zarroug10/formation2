import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';


import { ToastrService } from 'ngx-toastr';


import { AccountService } from '../_services/account.service';

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
