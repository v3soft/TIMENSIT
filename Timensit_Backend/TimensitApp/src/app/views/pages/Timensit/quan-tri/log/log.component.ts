import { Component, OnInit, Injectable } from '@angular/core';
import { LogService } from './log-service/log.service';
import { BehaviorSubject } from 'rxjs';
import { QueryParamsModel } from '../../../../../core/_base/crud';
// import { QueryParamsModel } from 'app/core/_base/crud';

@Component({
    selector: 'kt-log',
    templateUrl: './log.component.html',
})
@Injectable()
export class LogComponent implements OnInit {

  constructor(
		private LogService : LogService
	) {}

  ngOnInit() {
    if (this.LogService != undefined)
		this.LogService.lastFilter$ = new BehaviorSubject(new QueryParamsModel({}, 'desc', 'CreatedDate', 0, 10));
  }
}
