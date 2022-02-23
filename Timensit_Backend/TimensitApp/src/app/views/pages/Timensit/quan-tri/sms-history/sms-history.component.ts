import { Component, OnInit, Injectable } from '@angular/core';
import { SMSHistoryService } from './sms-history-service/sms-history.service';
import { BehaviorSubject } from 'rxjs';
import { QueryParamsModel } from 'app/core/_base/crud';

@Component({
    selector: 'kt-sms-history',
    templateUrl: './sms-history.component.html',
})
@Injectable()
export class SMSHistoryComponent implements OnInit {

  constructor(
		private SMSHistoryService : SMSHistoryService
	) {}

  ngOnInit() {
    if (this.SMSHistoryService != undefined)
		this.SMSHistoryService.lastFilter$ = new BehaviorSubject(new QueryParamsModel({}, 'asc', 'DanhMuc', 0, 10));
  }
}
