// Angular
import { Component, OnInit, ViewEncapsulation, ChangeDetectorRef } from '@angular/core';
// Services
// Widgets model
import { Observable } from 'rxjs';
import { CommonService } from '../../Timensit/services/common.service';
import { LayoutUtilsService } from '../../../../core/_base/crud';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { ConfirmPasswordValidator } from '../../auth/register/confirm-password.validator';
import { AuthService } from '../../../../core/auth';
import { Router, ActivatedRoute } from '@angular/router';
import moment from 'moment';

@Component({
	selector: 'kt-change-password',
	templateUrl: './change-password.component.html',
	encapsulation: ViewEncapsulation.None
})
export class ChangePasswordComponent implements OnInit {
	num: number;
	thoihan: string = '';
	user$: Observable<any>;
	Form: FormGroup;
	hasFormErrors: boolean = false;
	showWarning: boolean = false;
	constructor(private activatedRoute: ActivatedRoute,
		private router: Router,
		private commonService: CommonService,
		private changeDetect: ChangeDetectorRef,
		private fb: FormBuilder,
		private auth: AuthService,
		private layoutUtilsService: LayoutUtilsService) {
	}

	ngOnInit(): void {
		let data = JSON.parse(localStorage.getItem("UserInfo"));
		let date2 = moment(data.exp);
		this.thoihan = date2.format("DD/MM/YYYY");
		let date1 = moment();
		this.num = date2.diff(date1, 'days');
		this.commonService.getConfig(["EXP_SHOW"]).subscribe(res => {
			if (res && res.status == 1) {
				let exp_show = +res.data.EXP_SHOW;
				if (data.exp !== undefined && this.num <= exp_show)
					this.showWarning = true;
			}
			this.changeDetect.detectChanges();
		})
		this.createForm();
	}
	/**
	 * Create form
	 */
	createForm() {
		this.Form = this.fb.group({
			curPassword: ['', [Validators.required]],
			password: ['', Validators.compose([
				Validators.required,
				Validators.minLength(3),
				Validators.maxLength(100)
			])
			],
			confirmPassword: ['', Validators.compose([
				Validators.required,
				Validators.minLength(3),
				Validators.maxLength(100)
			])
			],
		}, {
			validator: ConfirmPasswordValidator.MatchPassword
		});
	}

	submit() {
		this.hasFormErrors = false;
		const controls = this.Form.controls;

		// check form
		if (this.Form.invalid) {
			Object.keys(controls).forEach(controlName =>
				controls[controlName].markAsTouched()
			);
			let invalid = <FormControl[]>Object.keys(this.Form.controls).map(key => this.Form.controls[key]).filter(ctl => ctl.invalid);
			let invalidElem: any = invalid[0];
			invalidElem.nativeElement.focus();
			this.hasFormErrors = true;
			return;
		}
		let data: any = {};
		data.OldPassword = controls.curPassword.value;
		data.NewPassword = controls.password.value;
		data.RePassword = controls.confirmPassword.value;
		this.commonService.changePassword(data).subscribe(res => {
			if (res && res.status == 1) {
				this.auth.logout(true);
			} else {
				this.layoutUtilsService.showError(res.error.message);
			}
		})
	}
	back() {
		this.router.navigateByUrl("profile", { relativeTo: this.activatedRoute });
	}
	/**
	 * Close alert
	 *
	 * @param $event
	 */
	onAlertClose($event) {
		this.hasFormErrors = false;
	}
}
