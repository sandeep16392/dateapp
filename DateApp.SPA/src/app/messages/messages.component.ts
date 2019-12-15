import { Component, OnInit } from '@angular/core';
import { Message } from '../models/message';
import { Pagination, PaginationResult } from '../models/pagination';
import { UserService } from '../services/user.service';
import { AlertifyService } from '../services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages: Message[];
  pagination: Pagination;
  messageContainer: string = 'Unread';
  constructor(
    private userService: UserService,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.messages = data['messages'].result;
      this.pagination = data['messages'].pagination;
    });
  }

  loadMessages() {
    this.userService
      .getMessages(
        this.authService.decodedToken.nameid,
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        this.messageContainer
      )
      .subscribe(
        (res: PaginationResult<Message[]>) => {
          this.messages = res.result;
          this.pagination = res.pagination;
        },
        error => {
          this.alertify.error(error);
        }
      );
  }

  deleteMessage(id: number) {
    this.alertify.confirm(
      'Are you sure you want to delete this message?',
      () => {
        this.userService
          .deleteMessage(id, this.authService.decodedToken.nameid)
          .subscribe(
            () => {
              this.messages = this.messages.filter(m=>m.id !== id);
              this.messages.slice(this.messages.findIndex(m => m.id === id), 1);
              this.alertify.success('Message has been deleted.');
            },
            error => {
              this.alertify.error('Failed to delete message.');
            }
          );
      }
    );
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadMessages();
  }
}
