import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, model, signal } from '@angular/core';


import { map } from 'rxjs';


import { LikesService } from './likes.service';
import { PresenceService } from './presence.service';
import { User } from '../_models/User';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
private presenceservice = inject(PresenceService)
private http = inject(HttpClient)
private likeservice = inject(LikesService)
basedUrl =environment.apiUrl;
currentUser = signal<User | null >(null);
roles = computed(() => 
  {
    const user = this.currentUser();
    if(user && user.token)
    {
      const role = JSON.parse(atob(user.token.split(".")[1])).role
      return Array.isArray(role) ? role : [role];
    }
    return [
      
    ];
  })

login(model:any){
  return this.http.post<User>(this.basedUrl + 'account/login',model).pipe(
    map(user => {
      if (user){
        this.setCurrentUser(user);
      }
    })
  );
}

setCurrentUser(user:User)
{
  localStorage.setItem('user',JSON.stringify(user));
        this.currentUser.set(user);
        this.likeservice.getLikesIds();
        this.presenceservice.createHubConnection(user);
}
logout(){
  localStorage.removeItem('user');
  this.currentUser.set(null);
  this.presenceservice.stopHubConnection();
}


Register(model:any){
  return this.http.post<User>(this.basedUrl + 'account/register',model).pipe(
    map(user => {
      if (user){
        this.setCurrentUser(user);
      }
      return user ;
    })
  );
}
}
