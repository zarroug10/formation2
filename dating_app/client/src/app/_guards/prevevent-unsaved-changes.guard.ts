import { CanDeactivateFn } from '@angular/router';
import { inject } from '@angular/core';


import { ConfirmService } from '../_services/confirm.service';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

export const preveventUnsavedChangesGuard: CanDeactivateFn<MemberEditComponent> = (component) => {
const confirmservice = inject(ConfirmService);

   if(component.editForm?.dirty){
    return confirmservice.confirm() ?? false ;
  }
  return true;
};
