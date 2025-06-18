import { Router } from '@angular/router';
import { inject, Injectable, signal } from '@angular/core';


import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';


import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';


import { User } from '../_models/User';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class PresenceService {
  private hubUrl = environment.hubsUrl;
  private hubConnection?: HubConnection;
  private toastr = inject(ToastrService);
  private route = inject(Router);
  onlineUsers = signal<string[]>([]);

  createHubConnection(user: User): void {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${this.hubUrl}presence`, {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .catch(error => console.error('Error starting SignalR connection:', error));

    this.hubConnection.on('UserIsOnline', username => {
      this.onlineUsers.update(users => [...users,username])
    });

    this.hubConnection.on('UserIsDisconnected', username => {
      this.onlineUsers.update(users => users.filter(x => x !== username));
    });

    this.hubConnection.on('GetOnlineUsers',usernames => {
      this.onlineUsers.set(usernames);
    })

    this.hubConnection.on('NewMessageReceived',({username,knownAs}) => {
      this.toastr.info( knownAs + 'Has sent u a new  message')
      .onTap
      .pipe(take(1))
      .subscribe(()=> this.route.navigateByUrl('/members/' + username + '?tab=Messages'))
    })
    
  }

  stopHubConnection(): void {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection
        .stop()
        .catch(error => console.error('Error stopping SignalR connection:', error));
    }
  }
}
