import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome'
import { TimeagoModule } from 'ngx-timeago';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';
import { faClock, faPaperPlane} from '@fortawesome/free-regular-svg-icons';




@Component({
  standalone: true,
  imports: [CommonModule, FontAwesomeModule,TimeagoModule],
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent {
  @Input() username?: string
  @Input() messages: Message[] = []
  faClock = faClock
  faPaperPlane= faPaperPlane

  constructor(private messageService: MessageService) { }

  loadMessages() {
    if (!this.username) return

    this.messageService.getMessagesThread(this.username).subscribe({
      next: response => this.messages = response
    })
  }

  ngOnInit(): void {
    this.loadMessages()
  }

}
