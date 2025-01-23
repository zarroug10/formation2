import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Member } from '../_models/Member';
import { setpaginatedResponse, setPaginationHeaders } from './paginationHelper';
import { paginationResults } from '../_models/Pagination';

@Injectable({
  providedIn: 'root'
})
export class LikesService {
  BasedUrl = environment.apiUrl;
  paginatedResults = signal<paginationResults<Member[]> |null>(null);

  private http = inject(HttpClient);

  likesIds = signal<number[]>([]);

  toggleLike(targetid:number){
   return this.http.post(this.BasedUrl + "likes/" + targetid,{} )
  }
  getlikes(predicate:string,PageNumber:number, PageSize:number){
    let params = setPaginationHeaders(PageNumber,PageSize);
    params = params.append("Predecate",predicate);
    return this.http.get<Member[]>(`${this.BasedUrl}likes`,{observe:'response',params})
    .subscribe(
      {
        next:response => setpaginatedResponse(response,this.paginatedResults)
    });
  }
  getLikesIds(){
    return this.http.get<number[]>(`${this.BasedUrl}likes/list`).subscribe(
      {
        next: ids => {
          this.likesIds.set(ids)
        }
      }
    )
  }
}
