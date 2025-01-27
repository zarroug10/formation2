import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { inject } from '@angular/core';
import { ConfirmService } from '../_services/confirm.service';

export const preveventUnsavedChangesGuard: CanDeactivateFn<MemberEditComponent> = (component) => {
const confirmservice = inject(ConfirmService);

   if(component.editForm?.dirty){
    return confirmservice.confirm() ?? false ;
  }
  return true;
};
