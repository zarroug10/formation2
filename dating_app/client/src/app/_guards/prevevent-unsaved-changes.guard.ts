import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

export const preveventUnsavedChangesGuard: CanDeactivateFn<MemberEditComponent> = (component) => {
   if(component.editForm?.dirty){
    return confirm('Are you sure you wanna continue . Any unsaved changes will be lost  ')
  }
  return true;
};
