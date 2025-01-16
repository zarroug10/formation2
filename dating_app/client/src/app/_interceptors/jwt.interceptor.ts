import { HttpInterceptorFn } from '@angular/common/http';
import { AccountService } from '../_services/account.service';
import { inject } from '@angular/core';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
const accountservice = inject(AccountService);


if (accountservice.currentUser()){
  req = req.clone({
    setHeaders:{
    Authorization : `Bearer ${accountservice.currentUser()?.token}`
  }})
}
  return next(req);
};
