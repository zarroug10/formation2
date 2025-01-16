import { inject, Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {
  busyReaquestCount = 0;
  private spinnerService = inject(NgxSpinnerService);

  busy() {
    this.busyReaquestCount++;
    this.spinnerService.show(undefined, {
      type: 'pacman',
      bdColor: "rgba(255,255,255,0)",
      color: "#333333",
      fullScreen: true
    })
  }

  Idle() {
    this.busyReaquestCount--;
    if(this.busyReaquestCount <= 0){
      this.busyReaquestCount =0
      this.spinnerService.hide();
    }
  }
}
