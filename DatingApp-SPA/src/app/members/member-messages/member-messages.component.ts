import { Component, OnInit, Input } from '@angular/core';
import { Message } from 'src/app/_models/message';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { error } from 'protractor';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() recipientId: number;
  messages: Message[];
  newMessage: any = {};

  constructor(private userService: UserService, private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.loadMessages();
  }

  loadMessages() {
    const id = +this.authService.decodedToken.nameid;
    this.userService.getMessageThread(id, this.recipientId)
    .pipe(
      tap(messages => {
        for (let i=0; i<messages.length; i++) {
          if (messages[i].isRead === false && messages[i].recipientId === id) {
            this.userService.markAsRead(id, messages[i].id);
          }
        }
      })
    )
    .subscribe((messages => {
      this.messages = messages;
    }), err => {
      this.alertify.error(err);
    });
  }

  sendMessage() {
    const id = this.authService.decodedToken.nameid;
    this.newMessage.recipientId = this.recipientId;
    this.userService.sendMessage(id, this.newMessage).subscribe((message: Message) => {
      this.messages.unshift(message);
      this.newMessage.content = '';
    }, err => {
      this.alertify.error(err);
    });
  }
}

