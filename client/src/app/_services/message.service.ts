import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Message } from '../_models/message';
import { environment } from 'src/environments/environment.development';
import { getPaginationHeaders, getPaginationResult } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl

  constructor(private http: HttpClient) { }
  
  getMessagesThread(username: string) {
    const url = this.baseUrl + 'messages/thread/' + username
    return this.http.get<Message[]>(url)
  }

  getMessages(pageNumber: number, pageSize: number, label: string = "Unread") {
    let httpParams = getPaginationHeaders(pageNumber, pageSize)
    httpParams = httpParams.append('Label', label)

    const url = this.baseUrl + 'messages'

    return getPaginationResult<Message[]>(url, httpParams, this.http)
  }
}
