import { Component, OnInit, Injectable } from '@angular/core';
import { NhomNguoiDungDPSService } from './nhom-nguoi-dung-dps-service/nhom-nguoi-dung-dps.service';
import { BehaviorSubject } from 'rxjs';
import { QueryParamsModel } from 'app/core/_base/crud';

@Component({
    selector: 'kt-nhom-nguoi-dung-dps',
    templateUrl: './nhom-nguoi-dung-dps.component.html',
    styleUrls: ['./nhom-nguoi-dung-dps.component.scss']
})
@Injectable()
export class NhomNguoiDungDPSComponent implements OnInit {

  constructor(
		private nhomNguoiDungDPSService : NhomNguoiDungDPSService
	) {}

  ngOnInit() {
    if (this.nhomNguoiDungDPSService != undefined)
			this.nhomNguoiDungDPSService.lastFilter$ = new BehaviorSubject(new QueryParamsModel({}, 'asc', 'DisplayOrder', 0, 10));
  }

}
