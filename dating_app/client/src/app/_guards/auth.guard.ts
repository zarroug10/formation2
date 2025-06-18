import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';


import { ToastrService } from 'ngx-toastr';


import { AccountService } from '../_services/account.service';

export const authGuard: CanActivateFn = (route, state) => {
const accountservice = inject(AccountService);
const toastr =inject(ToastrService)

if (accountservice.currentUser()){
return true
}
else{
  toastr.error("you shall not pass!")
  return false;
}
};
