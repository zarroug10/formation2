import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-test-errors',
  standalone: true,
  imports: [],
  templateUrl: './test-errors.component.html',
  styleUrl: './test-errors.component.css'
})
export class TestErrorsComponent {
basedUrl=environment
private http = inject(HttpClient);
validationErrors:string[] =[];

get400Error(){
this.http.get(this.basedUrl +'/buggy/bad-request ').subscribe({
  next:response => console.log(response),
  error : error => console.log(error)
})
}

get401Error(){
  this.http.get(this.basedUrl +'/buggy/auth').subscribe({
    next:response => console.log(response),
    error : error => console.log(error)
  })
  }

  get404Error(){
    this.http.get(this.basedUrl +'/buggy/Not-Found').subscribe({
      next:response => console.log(response),
      error : error => console.log(error)
    })
    }


    get500Error(){
      this.http.get(this.basedUrl +'/buggy/Server-error').subscribe({
        next:response => console.log(response),
        error : error => console.log(error)
      })
      }

      get400ValidationError(){
        this.http.post(this.basedUrl +'/account/register',{}).subscribe({
          next:response => console.log(response),
          error : error => {
          console.log(error);
          this.validationErrors = error ;
        }
        })
        }
}
