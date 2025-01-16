import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/Member';
import { Photo } from '../_models/Photo';
import { paginationResults } from '../_models/Pagination';
import { UserParams } from '../_models/UserParams';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  // members = signal<Member[]>([]);
  paginatedResults = signal<paginationResults<Member[]>| null >(null);
  

  getMembers(UserParams:UserParams) {
    let params = this.setPaginationHeaders(UserParams.PageNumber,UserParams.PageSize);
    params= params.append('minAge',UserParams.minAge);
    params= params.append('maxAge',UserParams.maxAge);
    params= params.append('gender',UserParams.gender);
    params= params.append('orderBy',UserParams.orderBy);
    
    return this.http.get<Member[]>(this.baseUrl + "users",{observe:'response',params}).subscribe(
      {
        next: response => this.paginatedResults.set({
          items:response.body as Member[],
          pagination: JSON.parse(response.headers.get('Pagination')!)
        })
      }
    )
  }


  private setPaginationHeaders(pageNumber:number,pageSize:number){
    let params = new HttpParams;
    if(pageNumber && pageSize) {
      params= params.append('pageNumber',pageNumber);
      params= params.append('pageSize',pageSize);
    }
    return params
  }

  getMember(username: string) {

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

}
