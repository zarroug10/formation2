import { HttpClient } from '@angular/common/http';
import { Component, inject, Input, OnInit, Output } from '@angular/core';
import { NavComponent } from "./nav/nav.component";
import { AccountService } from './_services/account.service';
import { HomeComponent } from "./home/home.component";
import { RouterOutlet } from '@angular/router';
import { NgxSpinnerComponent } from 'ngx-spinner';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavComponent, HomeComponent,NgxSpinnerComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  private accountService = inject(AccountService)

  
// the other way of using dependency injection in angular
// constructor(private httpclient:HttpClient){}
ngOnInit(): void {
 this.SetCurrentUser();

}

SetCurrentUser(){
  const userString = localStorage.getItem('user');
  if (!userString)  return;
  const user = JSON.parse(userString);
  this.accountService.currentUser.set(user);
}



}
