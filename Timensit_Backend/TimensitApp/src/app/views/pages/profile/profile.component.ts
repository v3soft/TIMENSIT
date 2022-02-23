// Angular
import { ChangeDetectorRef, Component, OnInit, ViewEncapsulation } from '@angular/core';
// Services
// Widgets model
import { Observable } from 'rxjs';
import moment from 'moment';
import { CommonService } from '../Timensit/services/common.service';

@Component({
	selector: 'kt-profile',
	templateUrl: './profile.component.html',
	styleUrls: ['profile.component.scss'],
	encapsulation: ViewEncapsulation.None
})
export class ProfileComponent implements OnInit {
	thoihan: string = '';
	num: number = 0;
	user$: Observable<any>;
	exp_show: number;
	constructor(private commonService: CommonService,
		private detechChange: ChangeDetectorRef) {
	}

	ngOnInit(): void {
		this.commonService.getConfig(["EXP_SHOW"]).subscribe(res => {
			if (res && res.status == 1)
				this.exp_show = +res.data.EXP_SHOW;
			this.detechChange.detectChanges();
		})
		let data = JSON.parse(localStorage.getItem("UserInfo"));
		let date2 = moment(data.exp);
		this.thoihan = date2.format("DD/MM/YYYY");
		let date1 = moment();
		this.num = date2.diff(date1, 'days');
		this.user$ = new Observable((observer) => {
			// observable execution
			observer.next(data)
			observer.complete()
		})
	}

	
	back() {
		history.back();
	}
}
