import { Component, OnInit, Injectable } from '@angular/core';
import { EmailHistoryService } from './email-history-service/email-history.service';
import { BehaviorSubject } from 'rxjs';
import { QueryParamsModel } from 'app/core/_base/crud';

@Component({
    selector: 'kt-email-history',
    templateUrl: './email-history.component.html',
})
@Injectable()
export class EmailHistoryComponent implements OnInit {

  constructor(
		private EmailHistoryService : EmailHistoryService
	) {}

  ngOnInit() {
    if (this.EmailHistoryService != undefined)
		this.EmailHistoryService.lastFilter$ = new BehaviorSubject(new QueryParamsModel({}, 'asc', 'DanhMuc', 0, 10));
  }
}
