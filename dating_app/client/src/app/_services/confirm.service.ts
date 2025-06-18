import { Injectable } from '@angular/core';


import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { map } from 'rxjs';


import { ConfirmDialogComponent } from '../modals/confirm-dialog/confirm-dialog.component';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {

  BsModelRef?: BsModalRef;

  constructor( private _modalService:BsModalService) {}

  confirm(
    title ='Confirmation',
    message='Are you sure you want to do this',
    btnOkText = 'Ok',
    btnCancelText = 'Cancel'
  ){
    const config: ModalOptions = {
      initialState : {
        title,
        message,
        btnOkText,
        btnCancelText
      }
    };
    this.BsModelRef = this._modalService.show(ConfirmDialogComponent,config);
    return this.BsModelRef.onHidden?.pipe(
      map(()=>{
        if (this.BsModelRef?.content){
          return this.BsModelRef.content.result;
        }else return false ;
      })
    )
  }
}
