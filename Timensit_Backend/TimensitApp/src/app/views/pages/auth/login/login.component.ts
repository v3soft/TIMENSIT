// Angular
import { ChangeDetectorRef, Component, OnDestroy, OnInit, ViewEncapsulation, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
// RxJS
import { Observable, Subject } from 'rxjs';
import { finalize, takeUntil, tap } from 'rxjs/operators';
// Translate
import { TranslateService } from '@ngx-translate/core';
// Store
import { Store } from '@ngrx/store';
import { AppState } from '../../../../core/reducers';
// Auth
import { AuthNoticeService, AuthService, Login } from '../../../../core/auth';
import { LayoutConfigService } from '../../../../core/_base/layout';
import * as objectPath from 'object-path';
import { environment } from 'environments/environment';
import { ReCaptchaComponent } from 'angular2-recaptcha';
import { MatDialog } from '@angular/material';
import { CommonService } from '../../Timensit/services/common.service';
/**
 * ! Just example => Should be removed in development
 */
const DEMO_PARAMS = {
	EMAIL: '',
	PASSWORD: ''
};

@Component({
	selector: 'kt-login',
	templateUrl: './login.component.html',
	styleUrls: ['./login.component.scss'],
	encapsulation: ViewEncapsulation.None
})
export class LoginComponent implements OnInit, OnDestroy {
	@ViewChild(ReCaptchaComponent, { static: false }) recaptcha: ReCaptchaComponent;
	// Public params
	loginForm: FormGroup;
	loading = false;
	isLoggedIn$: Observable<boolean>;
	errors: any = [];
	logo: string = "";
	constants: any;
	expression: boolean = false;
	private unsubscribe: Subject<any>;
	private returnUrl: any;
	showCaptcha: boolean = false;
	numShowRecaptcha: number = 0;
	captchaResponse: string = "";
	YOUR_SITE_KEY: string = "";
	error_txt: any = {
		username: "",
		password: ""
	}
	// Read more: => https://brianflove.com/2016/12/11/anguar-2-unsubscribe-observables/

	/**
	 * Component constructor
	 *
	 * @param router: Router
	 * @param auth: AuthService
	 * @param authNoticeService: AuthNoticeService
	 * @param translate: TranslateService
	 * @param store: Store<AppState>
	 * @param fb: FormBuilder
	 * @param cdr
	 * @param route
	 */
	constructor(
		private router: Router,
		private auth: AuthService,
		public dialog: MatDialog,
		private authNoticeService: AuthNoticeService,
		private translate: TranslateService,
		private store: Store<AppState>,
		private fb: FormBuilder,
		private cdr: ChangeDetectorRef,
		private route: ActivatedRoute,
		private layoutConfigService: LayoutConfigService,
		private commonService: CommonService
	) {
		this.unsubscribe = new Subject();
	}

	/**
	 * @ Lifecycle sequences => https://angular.io/guide/lifecycle-hooks
	 */

	/**
	 * On init
	 */
	async ngOnInit() {
		this.dialog.closeAll();
		this.initLoginForm();
		this.constants = this.layoutConfigService.getConfig('constants');
		await this.commonService.getConfig(["NUM_CAPCHA"]).toPromise().then(res => {
			if (res && res.status == 1)
				this.numShowRecaptcha = +res.data.NUM_CAPCHA;
		})
		//this.numShowRecaptcha = environment.numShowCaptcha;
		this.YOUR_SITE_KEY = environment.YOUR_SITE_KEY;
		// console.log("site key", this.YOUR_SITE_KEY);
		let crr_num = parseInt(localStorage.getItem("NumLogin"));
		if (!crr_num) crr_num = 0;
		if (this.numShowRecaptcha > 0 && crr_num > this.numShowRecaptcha) {
			this.showCaptcha = true;
		}
		// redirect back to the returnUrl before login
		this.route.queryParams.subscribe(params => {
			this.returnUrl = params.returnUrl || '/';
			if (this.returnUrl == '/error/404')
				this.returnUrl = '';
		});
	}

	/**
	 * On destroy
	 */
	ngOnDestroy(): void {
		this.authNoticeService.setNotice(null);
		this.unsubscribe.next();
		this.unsubscribe.complete();
		this.loading = false;
	}

	/**
	 * Form initalization
	 * Default params, validators
	 */
	initLoginForm() {
		// demo message to show
		//if (!this.authNoticeService.onNoticeChanged$.getValue()) {
		//	const initialNotice = `Use account
		//	<strong>${DEMO_PARAMS.EMAIL}</strong> and password
		//	<strong>${DEMO_PARAMS.PASSWORD}</strong> to continue.`;
		//	this.authNoticeService.setNotice(initialNotice, 'info');
		//}

		this.loginForm = this.fb.group({
			username: [DEMO_PARAMS.EMAIL, Validators.compose([
				Validators.required,
				Validators.minLength(3),
				Validators.maxLength(320) // https://stackoverflow.com/questions/386294/what-is-the-maximum-length-of-a-valid-email-address
			])
			],
			password: [DEMO_PARAMS.PASSWORD, Validators.compose([
				Validators.required,
				Validators.minLength(3),
				Validators.maxLength(100)
			])
			]
		});
	}

	/**
	 * Form Submit
	 */
	submit() {
		this.authNoticeService.setNotice(null);
		const controls = this.loginForm.controls;
		this.error_txt = {
			username: '',
			password: ''
		}
		if (objectPath.get(controls, 'username.errors.required')) {
			this.error_txt.username = this.translate.instant('AUTH.VALIDATION.REQUIRED', { name: this.translate.instant('AUTH.INPUT.USERNAME') });
		}
		if (objectPath.get(controls, 'username.errors.minlength.requiredLength')) {
			this.error_txt.username = this.translate.instant('AUTH.VALIDATION.MIN_LENGTH_FIELD', { name: this.translate.instant('AUTH.INPUT.USERNAME') }) + " 3 ký tự";
		}
		if (objectPath.get(controls, 'username.errors.maxlength.requiredLength')) {
			this.error_txt.username = this.translate.instant('AUTH.VALIDATION.MAX_LENGTH_FIELD', { name: this.translate.instant('AUTH.INPUT.USERNAME') }) + " 320 ký tự";
		}
		if (objectPath.get(controls, 'password.errors.required')) {
			this.error_txt.password = this.translate.instant('AUTH.VALIDATION.REQUIRED', { name: this.translate.instant('AUTH.INPUT.PASSWORD') });
		}

		if (objectPath.get(controls, 'password.errors.minlength.requiredLength')) {
			this.error_txt.password = this.translate.instant('AUTH.VALIDATION.MIN_LENGTH_FIELD', { name: this.translate.instant('AUTH.INPUT.PASSWORD') }) + " 3 ký tự";
		}
		if (objectPath.get(controls, 'password.errors.maxlength.requiredLength')) {
			this.error_txt.password = this.translate.instant('AUTH.VALIDATION.MAX_LENGTH_FIELD', { name: this.translate.instant('AUTH.INPUT.PASSWORD') }) + " 320 ký tự";
		}
		/** check form */
		if (this.loginForm.invalid) {
			Object.keys(controls).forEach(controlName =>
				controls[controlName].markAsTouched()
			);
			return;
		}

		this.loading = true;

		const authData = {
			username: controls.username.value,
			password: controls.password.value,
			checkReCaptCha: this.showCaptcha,
			GReCaptCha: this.captchaResponse
		};
		this.auth
			.login(authData.username, authData.password, authData.checkReCaptCha, authData.GReCaptCha)
			.subscribe(response => {
				let numlogin = parseInt(localStorage.getItem("NumLogin"));
				if (!numlogin) numlogin = 0;
				localStorage.setItem("NumLogin", (numlogin + 1) + '');
				if (response && response.status === 1) {
					this.error_txt = {
						username: '',
						password: ''
					}
					localStorage.setItem("NumLogin", "0");
					this.router.navigate([this.returnUrl]);
				}
				else {
					this.authNoticeService.setNotice(response.error.message, 'danger');
				}
				let crr_num = parseInt(localStorage.getItem("NumLogin"));
				if (this.captchaResponse) {
					this.recaptcha.reset();
				}
				//console.log("num", crr_num);
				//console.log("numShowRecaptcha", this.numShowRecaptcha);
				if (this.numShowRecaptcha > 0 && crr_num > this.numShowRecaptcha) {
					this.showCaptcha = true;
				}
				this.loading = false;
				this.cdr.detectChanges();
			});
	}

	/**
	 * Checking control validation
	 *
	 * @param controlName: string => Equals to formControlName
	 * @param validationType: string => Equals to valitors name
	 */
	isControlHasError(controlName: string, validationType: string): boolean {
		const control = this.loginForm.controls[controlName];
		if (!control) {
			return false;
		}

		const result = control.hasError(validationType) && (control.dirty || control.touched);
		return result;
	}
	public handleCorrectCaptcha(captchaResponse: string): void {
		this.captchaResponse = captchaResponse;

	}
}
