import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../models/User';
import { environment } from 'src/environments/environment';
import { PaginationResult } from '../models/pagination';
import { map } from 'rxjs/operators';
import { Message } from '../models/message';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  constructor(private http: HttpClient) {}

  getUsers(
    page?,
    itemsPerPage?,
    userParams?,
    likeParams?
  ): Observable<PaginationResult<User[]>> {
    const paginatedResult: PaginationResult<User[]> = new PaginationResult<
      User[]
    >();

    let params = new HttpParams();
    if (page && itemsPerPage) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    if (userParams != null) {
      params = params.append('minAge', userParams.minAge);
      params = params.append('maxAge', userParams.maxAge);
      params = params.append('gender', userParams.gender);
      params = params.append('orderBy', userParams.orderBy);
    }

    if (likeParams === 'liker') {
      params = params.append('liker', 'true');
    }

    if (likeParams === 'likee') {
      params = params.append('likee', 'true');
    }

    return this.http
      .get<any>(environment.baseUrl + 'api/users', {
        observe: 'response',
        params
      })
      .pipe(
        map(response => {
          paginatedResult.result = response.body.users;
          paginatedResult.pagination = response.body.pagination;
          // if (response.headers.get('Pagination')) {
          //   paginatedResult.pagination = JSON.parse(
          //     response.headers.get('Pagination')
          //   );
          // }
          return paginatedResult;
        })
      );
  }

  getUser(id: number): Observable<User> {
    return this.http.get<User>(environment.baseUrl + 'api/users/' + id);
  }

  updateUser(id: number, user: User) {
    return this.http.put(environment.baseUrl + 'api/users/' + id, user);
  }

  setMainPhoto(userid: number, id: number) {
    return this.http.post(
      environment.baseUrl +
        'api/users/' +
        userid +
        '/photos/' +
        id +
        '/setmain',
      {}
    );
  }

  deletePhoto(userid: number, id: number) {
    return this.http.delete(
      environment.baseUrl + 'api/Users/' + userid + '/photos/' + id
    );
  }

  sendLike(userid: number, recepientId: number) {
    return this.http.post(
      environment.baseUrl + 'api/users/' + userid + '/like/' + recepientId,
      {}
    );
  }

  getMessages(
    id: number,
    page?,
    itemsPerPage?,
    messageContainer?
  ): Observable<PaginationResult<Message[]>> {
    const paginatedResult: PaginationResult<Message[]> = new PaginationResult<
      Message[]
    >();

    let params = new HttpParams();

    params = params.append('MessageContainer', messageContainer);

    if (page && itemsPerPage) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    return this.http
      .get<any>(environment.baseUrl + 'api/users/' + id + '/messages', {
        observe: 'response',
        params
      })
      .pipe(
        map(response => {
          paginatedResult.result = response.body.messages;
          paginatedResult.pagination = response.body.pagination;
          return paginatedResult;
        })
      );
  }

  getMessageThread(userId: number, recipientId: number) {
    return this.http.get<Message[]>(
      environment.baseUrl +
        'api/users/' +
        userId +
        '/messages/thread/' +
        recipientId
    );
  }

  sendMessage(id: number, message: Message) {
    return this.http.post(
      environment.baseUrl +
        'api/users/' +
        id +
        '/messages',
        message
    );
  }

  deleteMessage(id: number, userId: number) {
    return this.http.post(
      environment.baseUrl +
        'api/users/' +
        userId +
        '/messages/' + id,
        {}
    );
  }

  markAsRead(msgId: number, userId: number) {
    return this.http.post(
      environment.baseUrl +
        'api/users/' +
        userId +
        '/messages/' + msgId + '/read',
        {}
    ).subscribe();
  }
}
