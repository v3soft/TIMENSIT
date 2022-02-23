import { Component, OnInit, Injectable } from '@angular/core';
import { NguoiDungDPSService } from './nguoi-dung-dps-service/nguoi-dung-dps.service';
import { BehaviorSubject } from 'rxjs';
import { FormControlName } from '@angular/forms';
import { QueryParamsModel } from '../../../../../core/_base/crud';


@Component({
    selector: 'kt-nguoi-dung-dps',
    templateUrl: './nguoi-dung-dps.component.html',
    styleUrls: ['./nguoi-dung-dps.component.scss']
})
@Injectable()
export class NguoiDungDPSComponent implements OnInit {

  constructor(
		private nguoiDungDPSService : NguoiDungDPSService
	) {}

  ngOnInit() {
    if (this.nguoiDungDPSService != undefined)
		this.nguoiDungDPSService.lastFilter$ = new BehaviorSubject(new QueryParamsModel({}, 'asc', 'FullName', 0, 10));
  }

}
