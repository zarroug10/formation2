import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';


import { ModalModule} from 'ngx-bootstrap/modal';
import { NgxSpinnerModule } from 'ngx-spinner';
import { TimeagoModule } from 'ngx-timeago';
import { provideToastr } from 'ngx-toastr';



import { routes } from './app.routes';
import { errorInterceptor } from './_interceptors/error.interceptor';
import { jwtInterceptor } from './_interceptors/jwt.interceptor';
import { loadingInterceptor } from './_interceptors/loading.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptors
      ([errorInterceptor,
        jwtInterceptor,
      loadingInterceptor]

      )),//adding our  interceptor through adding it to the http client
    provideAnimations(),
    provideToastr({
      positionClass:"toast-bottom-right"
    }),
    importProvidersFrom(NgxSpinnerModule,TimeagoModule.forRoot(),ModalModule.forRoot()),
  ]
};
