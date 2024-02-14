import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css']
})
export class RolesModalComponent {

  title = ''
  list: any
  closeBtnName = ''
  user:User = {} as User
  availableRoles: any[] = []
  selectedRoles: any[] = []
  isConfirmUpdate = false

  constructor(public bsModalRef: BsModalRef) { }

  onRolesChecked(role: string) {
    const index = this.selectedRoles.indexOf(role)
    index !== -1 ? this.selectedRoles.splice(index, 1) : this.selectedRoles.push(role)
  }
  updateRole() {
    const selectedRoles = this.bsModalRef.content?.selectedRoles
    if (this._isRolesChanged(selectedRoles))
      this.isConfirmUpdate = true
    //console.log(this.isConfirmUpdate)
  }
  
  private _isRolesChanged(selectedRoles: any[]) {
    return JSON.stringify(selectedRoles.sort()) !== JSON.stringify(this.user.roles.sort())
  }
}
