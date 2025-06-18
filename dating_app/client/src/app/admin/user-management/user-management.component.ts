import { Component, inject, OnInit } from '@angular/core';


import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { initialState } from 'ngx-bootstrap/timepicker/reducer/timepicker.reducer';


import { AdminService } from '../../_services/admin.service';
import { RolesModalComponent } from '../../modals/roles-modal/roles-modal.component';
import { User } from '../../_models/User';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.css'
})
export class UserManagementComponent implements OnInit {
 
  private adminservice = inject(AdminService);
  private modalservice = inject(BsModalService);
  users: User[] =[];
  bsModalRef:BsModalRef<RolesModalComponent> = new BsModalRef<RolesModalComponent>();

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  openRolesModal(user:User)
  {
    const initialState: ModalOptions = {
      class :'model-lg',
      initialState:{
        title:'User roles',
        username: user.username,
        selectedRoles:[...user.roles],
        availableRoles:['Admin','Moderator','Member'],
        users:this.users,
        rolesUpdated:false
      }
    }
    this.bsModalRef = this.modalservice.show(RolesModalComponent, initialState);
    this.bsModalRef.onHide?.subscribe(
      {
        next: () => {
          if (this.bsModalRef.content && this.bsModalRef.content.rolesUpdated) {
            const selectedRoles = this.bsModalRef.content.selectedRoles;
            this.adminservice.updateUserRoles(user.username,selectedRoles).subscribe
            (
              {
                next: roles => user.roles = roles
              }
            )
          }
        }
      }
    )
  }

  getUsersWithRoles()
{
  this.adminservice.getUserWithRoles().subscribe(
    {
      next: users => this.users = users
    }
  )
}

}
