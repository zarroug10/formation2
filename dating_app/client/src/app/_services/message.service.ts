import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Message } from '../_models/Message';
import { paginationResults } from '../_models/Pagination';
import { setpaginatedResponse, setPaginationHeaders } from './paginationHelper';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { User } from '../_models/User';
import { ToastrService } from 'ngx-toastr';
import { group } from '@angular/animations';
import { Group } from '../_models/group';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  private toastr = inject(ToastrService);
  paginatedResult = signal<paginationResults<Message[]> | null> (null);
  hubUrl= environment.hubsUrl;
  hubConnection?: HubConnection;
  messagethread = signal<Message[]> ([]);
  unreadMessageCount = signal<number>(0); 


  CreateHubConnection(user:User,otherUsername:string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl +'message?user='+ otherUsername,{
        accessTokenFactory:()=> user.token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on('ReceiveMessageThread', messages =>{
      this.messagethread.set(messages);
    })

    this.hubConnection.on('NewMessage',message => {
      this.messagethread.update(messages => [...messages, message])
    })

    this.hubConnection.on('UpdatedGroup', (group: Group) => {
      if (group.connections.some(x => x.username === otherUsername)) {
        this.messagethread.update(messages => {
          messages.forEach(message => {
            if (!message.dateRead) {
              message.dateRead = new Date(Date.now());
            }
          })
          return messages;
        })
      }
    })
  }

  stopHubConnection(){
    if(this.hubConnection?.state === HubConnectionState.Connected){
      this.hubConnection.stop().catch(error => console.log(error));
    }
  }

  getMessages(pageNumber:number,PageSize:number,container:string)
  {
    let params = setPaginationHeaders(pageNumber,PageSize);

    params= params.append("container",container)

    return this.http.get<Message[]>(this.baseUrl + 'messages',
      {observe:'response',params})
      .subscribe({
        next: response => setpaginatedResponse(response,this.paginatedResult)
      })
  }

  getMessageThread(username:string){
    return this.http.get<Message[]>(this.baseUrl +"messages/thread/"+ username);
  }

 async sendMessage(username:string , content:string)
  {
    // return this.http.post(this.baseUrl+"messages",{recipientUsername: username,content})
    return this.hubConnection?.invoke('SendMessage', {recipientUsername:username, content})
  }

  deleteMessage(id:string)
  {
    return this.http.delete(this.baseUrl +'messages/' + id);
  }
}
