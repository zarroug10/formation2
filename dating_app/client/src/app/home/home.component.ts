import { Component } from '@angular/core';


import { RegisterComponent } from "../register/register.component";

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RegisterComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent  {

RegisterMode=false;


registerToggle(){
  this.RegisterMode=!this.RegisterMode
}
cancelRegisterMode(event:boolean){
this.RegisterMode = event;
}

}
