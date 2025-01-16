import { HttpClient } from '@angular/common/http';
import { inject, Injectable, model, signal } from '@angular/core';
import { User } from '../_models/User';
import { map } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
private http = inject(HttpClient)
basedUrl =environment.apiUrl;
currentUser = signal<User | null >(null);

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
}
logout(){
  localStorage.removeItem('user');
  this.currentUser.set(null);
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
