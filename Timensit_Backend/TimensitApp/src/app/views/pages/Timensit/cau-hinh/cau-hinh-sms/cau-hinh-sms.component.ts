import { Component, OnInit, Injectable } from '@angular/core';
import { CauHinhSMSService } from './cau-hinh-sms-service/cau-hinh-sms.service';
import { BehaviorSubject } from 'rxjs';
import { QueryParamsModel } from 'app/core/_base/crud';

@Component({
    selector: 'kt-cau-hinh-sms',
    templateUrl: './cau-hinh-sms.component.html',
})
@Injectable()
export class CauHinhSMSComponent implements OnInit {

  constructor(
		private CauHinhSMSService : CauHinhSMSService
	) {}

  ngOnInit() {
    if (this.CauHinhSMSService != undefined)
		this.CauHinhSMSService.lastFilter$ = new BehaviorSubject(new QueryParamsModel({}, 'asc', 'DanhMuc', 0, 10));
  }
}
