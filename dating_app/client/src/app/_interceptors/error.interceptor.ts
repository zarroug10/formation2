import { HttpInterceptorFn } from '@angular/common/http';
import { NavigationExtras, Router } from '@angular/router';
import { inject } from '@angular/core';


import { ToastrService } from 'ngx-toastr';
import { catchError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const route = inject(Router);
  const toastr = inject(ToastrService);
  return next(req).pipe(
    catchError(error => {
      if(error){
        switch (error.status) {
          case 400:
            if (error.error.errors){
              const modelStateErrors=[]; //storing the errors array
              for (const key in error.error.errors) { //looping what inside the errors
                if (error.error.errors[key]){
                  modelStateErrors.push(error.error.errors[key])
                }
              }
              throw modelStateErrors.flat();
            }
            else{
              toastr.error(error.error, error.status)
            }
            break;
            case 401:
              toastr.error('Unauthorized', error.status)
              break;
              case 404:
               route.navigateByUrl('/not-found')
                break; 
                case 500:
                  const navigationExtras: NavigationExtras= {state:{error: error.error}};
                  route.navigateByUrl('/server-error',navigationExtras)
                   break; 
          default:
            toastr.error("something Unexpected got caught!")
            break;
        }
      }
      throw error;
    })
  )
};
