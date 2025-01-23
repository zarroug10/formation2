import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/Member';
import { Photo } from '../_models/Photo';
import { paginationResults } from '../_models/Pagination';
import { UserParams } from '../_models/UserParams';
import { of } from 'rxjs';
import { AccountService } from './account.service';
import { setpaginatedResponse, setPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  private http = inject(HttpClient);
  private accounrservice = inject(AccountService);
  baseUrl = environment.apiUrl;
  // members = signal<Member[]>([]);
  paginatedResults = signal<paginationResults<Member[]>| null >(null);
  memberCache = new Map() ;
  user = this.accounrservice.currentUser();
  userParams = signal<UserParams>(new UserParams(this.user));


  getMembers() {
    const response = this.memberCache.get(Object.values(this.userParams()).join('-'));
   
 console.log(response)
    if(response) return setpaginatedResponse(response,this.paginatedResults);
 
     let params =  setPaginationHeaders(this.userParams().PageNumber,this.userParams().PageSize);
 
     params= params.append('minAge',this.userParams().minAge);
     params= params.append('maxAge',this.userParams().maxAge);
     params= params.append('gender',this.userParams().gender);
     params= params.append('orderBy',this.userParams().orderBy);
     
     return this.http.get<Member[]>(this.baseUrl + "users",{observe:'response',params}).subscribe(
       {
         next: response =>  {
           setpaginatedResponse(response,this.paginatedResults);
           this.memberCache.set(Object.values(this.userParams()).join('-'),response)
         }
       }
     )
   }
 
 



  getMember(username: string) {
    const member:Member = [...this.memberCache.values()]
    .reduce((arr,elem)=> arr.concat(elem.body),[])
    .find((m:Member)=> m.username === username);
    if (member) return of(member);

    return this.http.get<Member>(this.baseUrl + "users/" + username,);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(

    )
  }

  UpdateMainPhoto(photo: Photo) {
    return this.http.put(this.baseUrl + "users/set-Photo/" + photo.id, {}).pipe(
    )
  }

  deletePhoto(photo: Photo) {
    return this.http.delete(this.baseUrl + "users/Delete-photo/" + photo.id).pipe(
    )
  }
  resetUSerParamms(){
    this.userParams.set(new UserParams(this.user));
  }

}
