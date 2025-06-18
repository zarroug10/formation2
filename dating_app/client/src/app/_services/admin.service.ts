import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

import { User } from '../_models/User';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
baseUrl = environment.apiUrl;
private http = inject(HttpClient);


getUserWithRoles()
{
  return this.http.get<User[]>(this.baseUrl +'admin/users-with-admin');
}

updateUserRoles(username:string,roles:string[])
{
  return  this.http.post<string[]>(this.baseUrl +"admin/editRole/"
    + username+'?roles='+roles,{});
}
}
