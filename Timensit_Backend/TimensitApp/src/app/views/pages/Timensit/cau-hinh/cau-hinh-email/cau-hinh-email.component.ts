import { Component, OnInit, Injectable } from '@angular/core';
import { CauHinhEmailService } from './cau-hinh-email-service/cau-hinh-email.service';
import { BehaviorSubject } from 'rxjs';
import { QueryParamsModel } from 'app/core/_base/crud';

@Component({
    selector: 'kt-cau-hinh-email',
    templateUrl: './cau-hinh-email.component.html',
})
@Injectable()
export class CauHinhEmailComponent implements OnInit {

  constructor(
		private CauHinhEmailService : CauHinhEmailService
	) {}

  ngOnInit() {
    if (this.CauHinhEmailService != undefined)
		this.CauHinhEmailService.lastFilter$ = new BehaviorSubject(new QueryParamsModel({}, 'asc', 'DanhMuc', 0, 10));
  }
}
