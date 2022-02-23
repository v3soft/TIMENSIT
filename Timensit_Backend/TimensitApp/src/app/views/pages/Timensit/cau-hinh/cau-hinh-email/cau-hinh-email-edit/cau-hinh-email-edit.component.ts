// Angular
import { Component, OnInit, ChangeDetectionStrategy, OnDestroy, ChangeDetectorRef, Inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
// Material
import { MatPaginator, MatSort, MatDialog, MatTableDataSource, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { SelectionModel } from '@angular/cdk/collections';
// RxJS
import { Observable, BehaviorSubject, Subscription } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
// NGRX
// Service
import { LayoutUtilsService, MessageType } from 'app/core/_base/crud';
import { CauHinhEmailService } from '../cau-hinh-email-service/cau-hinh-email.service';
import { CommonService } from '../../../services/common.service';
//Models

import { CauHinhEmailModel } from '../cau-hinh-email-model/cau-hinh-email.model';

import { CauHinhEmailPopupDVCComponent } from '../cau-hinh-email-popup-donvicon/cau-hinh-email-popup-donvicon.component';

@Component({
	selector: 'kt-cau-hinh-email-edit',
	templateUrl: './cau-hinh-email-edit.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
})

export class CauHinhEmailEditComponent implements OnInit, OnDestroy {
	// Public properties
	ItemData: any;
	FormControls: FormGroup;
	hasFormErrors: boolean = false;
	disabledBtn: boolean = false;
	loadingSubject = new BehaviorSubject<boolean>(true);
	loading$: Observable<boolean>;
	viewLoading: boolean = false;
	isChange: boolean = false;
	isZoomSize: boolean = false;
	LstDanhMucKhac: any[] = [];
	datasource: any;
	public datatreeDonVi: BehaviorSubject<any[]> = new BehaviorSubject([]);
	private componentSubscriptions: Subscription;

	ListDonViCon: any[] = [];
	constructor(
		public dialogRef: MatDialogRef<CauHinhEmailEditComponent>,
		@Inject(MAT_DIALOG_DATA) public data: any,
		private FormControlFB: FormBuilder,
		public dialog: MatDialog,
		private layoutUtilsService: LayoutUtilsService,
		private changeDetectorRefs: ChangeDetectorRef,
		private CauHinhEmailsService: CauHinhEmailService,
		private commonService: CommonService) { }

	/**
	 * On init
	 */
	async ngOnInit() {
		this.commonService.fixedPoint = 0;

		this.viewLoading = true;
		this.ItemData = new CauHinhEmailModel();
		this.ItemData.clear();
		this.createForm();
		// await this.getTreeDonVi().then(res => {
		// 	if (res && res.status == 1)
		// 		this.datatreeDonVi.next(res.data);
		// 	else
		// 		this.datatreeDonVi.next([]);
		// });
		this.getTreeDonVi();
		setTimeout(() => {
			if (this.data.CauHinhEmail && this.data.CauHinhEmail.Id > 0) {
				this.CauHinhEmailsService.getCauHinhEmailById(this.data.CauHinhEmail.Id).subscribe(res => {
					this.viewLoading = false;
					if (res.status == 1 && res.data) {
						this.ItemData = res.data;
						this.createForm();
					}
					else {
						this.layoutUtilsService.showError(res.error.message);
					}
					this.changeDetectorRefs.detectChanges();
				});
			} else {
				this.viewLoading = false;
				this.changeDetectorRefs.detectChanges();
			}
		}, 200);

		//this.CheckRoles();
	}
	GetValueNode(event) {
		this.ListDonViCon = [];
	}

	// CheckRoles(){
	// 	var a=this.commonService.CheckRole(1);
	// 	if(a.length>0)
	// 	{
	// 		console.log('đúng')
	// 	}
	// 	else
	// 	{
	// 		console.log('sai')
	// 	}

	// }
	/**
	 * On destroy
	 */
	ngOnDestroy() {
		if (this.componentSubscriptions) {
			this.componentSubscriptions.unsubscribe();
		}
	}

	/**
	 * Create form
	 */
	createForm() {
		this.FormControls = this.FormControlFB.group({
			// bDNghi: [this.ItemData.BDNghi == null ? '' : this.ItemData.BDNghi, [Validators.required, Validators.maxLength(100)]],
			// kTNghi: [this.ItemData.KTNghi == null ? '' : this.ItemData.KTNghi, [Validators.required, Validators.maxLength(100)]],
			// dotNghi: [this.ItemData.DotNghi == null ? '' : this.ItemData.DotNghi + '',Validators.required],
			// moTa: [this.ItemData.MoTa == null ? '' : this.ItemData.MoTa],

			Server: [this.ItemData.Server == null ? '' : this.ItemData.Server, [Validators.required]],
			Port: [this.ItemData.Port == null ? '' : this.ItemData.Port, [Validators.required, Validators.min(1)]],
			UserName: [this.ItemData.UserName == null ? '' : this.ItemData.UserName, [Validators.required]],
			DonVi: [this.ItemData.DonVi == null ? '' : this.ItemData.DonVi + ''],
			EnableSSL: [this.ItemData.EnableSSL == null ? false : this.ItemData.EnableSSL, [Validators.required]],
			Password: [this.ItemData.Password == null ? '' : this.ItemData.Password, [Validators.required]],
			IsDungChung: [this.ItemData.DonVi == 0]
		});

		this.ListDonViCon = this.ItemData.DonViCon;
		this.datasource = new MatTableDataSource(this.ListDonViCon);

		if (this.data.CauHinhEmail.View)
			this.FormControls.disable();
	}
	/**
	 * Returns page title
	 */
	getTitle(): string {
		if (this.ItemData.Id == 0) {
			return 'Thêm mới cấu hình email';
		}

		if (this.data.CauHinhEmail.View)
			return `Xem cấu hình email `;

		return `Chỉnh sửa cấu hình email`;
	}

	/**
	 * Check control is invalid
	 * @param controlName: string
	 */
	isControlInvalid(controlName: string): boolean {
		const control = this.FormControls.controls[controlName];
		const result = control.invalid && control.touched;
		return result;
	}

	/**
	 * Save data
	 *
	 * @param withBack: boolean
	 */
	onSubmit(type: boolean) {
		this.hasFormErrors = false;
		const controls = this.FormControls.controls;
		/** check form */
		if (this.FormControls.invalid) {
			Object.keys(controls).forEach(controlName =>
				controls[controlName].markAsTouched()
			);
			let invalid = <FormControl[]>Object.keys(this.FormControls.controls).map(key => this.FormControls.controls[key]).filter(ctl => ctl.invalid);
			let invalidElem: any = invalid[0];
			invalidElem.nativeElement.focus();
			this.hasFormErrors = true;
			return;
		}
		if (!controls["IsDungChung"].value && !controls["DonVi"].value) {
			this.hasFormErrors = true;
			this.layoutUtilsService.showError("Vui lòng chọn đơn vị");
			return;
		}
		this.disabledBtn = true;
		// tslint:disable-next-line:prefer-const
		let editedCauHinhEmail = this.prepareCauHinhEmails();

		if (this.ItemData.Id > 0) {
			this.updateCauHinhEmail(editedCauHinhEmail)
			return;
		}

		this.addCauHinhEmail(editedCauHinhEmail, type);
	}

	/**
	 * Returns object for saving
	 */
	prepareCauHinhEmails(): any {
		const controls = this.FormControls.controls;
		const _item: any = {};
		//_CauHinhEmail.clear();

		// Server: [this.ItemData.Server == null ? '' : this.ItemData.Server, [Validators.required]],
		// Port: [this.ItemData.Port == null ? '' : this.ItemData.Port, [Validators.required]],
		// UserName: [this.ItemData.UserName == null ? '' : this.ItemData.UserName, [Validators.required]],
		// DonVi: [this.ItemData.DonVi == null ? '' : this.ItemData.DonVi, [Validators.required]],
		// EnableSSL: [this.ItemData.EnableSSL == null ? false : this.ItemData.EnableSSL, [Validators.required]],
		// Password: [this.ItemData.Password == null ? '' : this.ItemData.Password, [Validators.required]],

		_item.Server = controls['Server'].value;
		_item.Port = controls['Port'].value;
		_item.UserName = controls['UserName'].value;
		_item.EnableSSL = controls['EnableSSL'].value;
		_item.Password = controls['Password'].value;
		if (!controls["IsDungChung"].value) {
			_item.DonVi = controls['DonVi'].value;

			let ArrDVC: any[] = [];
			if (this.ListDonViCon && this.ListDonViCon.length > 0) {
				for (var i = 0; i < this.ListDonViCon.length; i++) {
					ArrDVC.push(+this.ListDonViCon[i].Id);
				}
			}
			//gán lại giá trị id 
			_item.DonViCon = ArrDVC;
		}
		else {
			_item.DonVi = 0;
			_item.DonViCon = [];
		}
		if (this.ItemData.Id > 0) {
			_item.Id = this.ItemData.Id;
		}

		return _item;
	}
	/**
	 * Add item
	 *
	 * @param _CauHinhEmail: CauHinhEmailModel
	 * @param withBack: boolean
	 */
	addCauHinhEmail(_CauHinhEmail: CauHinhEmailModel, withBack: boolean = false) {

		this.CauHinhEmailsService.createCauHinhEmail(_CauHinhEmail).subscribe(res => {
			if (res.status == 1) {
				this.isChange = true;
				const message = `Thêm thành công`;
				this.layoutUtilsService.showInfo(message);
				this.FormControls.reset();
				this.FormControls.controls['EnableSSL'].setValue(false);
				this.ListDonViCon = [];
				this.datasource = new MatTableDataSource(this.ListDonViCon);
				if (withBack)
					this.dialogRef.close(this.isChange);
			}
			else {
				this.layoutUtilsService.showError(res.error.message);
			}
			this.disabledBtn = false;
			this.changeDetectorRefs.detectChanges();
		});
	}

	/**
	 * Update item
	 *
	 * @param _CauHinhEmail: CauHinhEmailsModel
	 * @param withBack: boolean
	 */
	updateCauHinhEmail(_CauHinhEmail: CauHinhEmailModel, withBack: boolean = false) {
		this.CauHinhEmailsService.updateCauHinhEmail(_CauHinhEmail).subscribe(res => {
			if (res.status == 1) {
				this.isChange = true;
				const message = `Cập nhật thành công`;
				this.layoutUtilsService.showInfo(message);
				this.dialogRef.close(this.isChange);
			}
			else {
				this.layoutUtilsService.showError(res.error.message);
			}
			this.disabledBtn = false;
			this.changeDetectorRefs.detectChanges();
		});
	}

	/**
	 * Close alert
	 *
	 * @param $event
	 */
	onAlertClose($event) {
		this.hasFormErrors = false;
	}

	closeDialog() {
		this.dialogRef.close(this.isChange);
	}

	getTreeDonVi() {
		this.commonService.TreeDonVi().subscribe(res => {
			if (res && res.status == 1) {
				this.datatreeDonVi.next(res.data);
			}
			else {
				this.datatreeDonVi.next([]);
				this.layoutUtilsService.showError(res.error.message);
			}
		})
	}

	resizeDialog() {
		if (!this.isZoomSize) {
			this.dialogRef.updateSize('100vw', '100vh');
			this.isZoomSize = true;
		}
		else if (this.isZoomSize) {
			this.dialogRef.updateSize('900px', 'auto');
			this.isZoomSize = false;
		}

	}

	ChonDonViConPop() {
		if (this.FormControls.controls['DonVi'].value == '' || this.FormControls.controls['DonVi'].value == '0') {
			this.layoutUtilsService.showInfo('Chưa chọn đơn vị');
			return;
		}
		let InfoDonViCon = { Id: this.FormControls.controls['DonVi'].value, LstDonViCon: this.data.CauHinhEmail && this.data.CauHinhEmail.Id > 0 ? this.ItemData.DonViCon : [] };
		const dialogRef = this.dialog.open(CauHinhEmailPopupDVCComponent, { data: { InfoDonViCon } });
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}

			this.ListDonViCon = this.ItemData.DonViCon = res;
			this.datasource = new MatTableDataSource(this.ListDonViCon);
			this.changeDetectorRefs.detectChanges();
		});
	}

	DeleteDonViCon(ind: any) {
		// const _title: string = 'Xóa đơn vị con';
		// const _description: string = 'Bạn có chắc muốn xóa đơn vị con này không?';
		// const _waitDesciption: string = 'Đơn vị con đang được xóa...';
		// const _deleteMessage = `Xóa thành công`;

		// const dialogRef = this.layoutUtilsService.deleteElement(_title, _description, _waitDesciption);
		// dialogRef.afterClosed().subscribe(res => {
		// 	if (!res) {
		// 		return;
		// 	}

		// 	this.ListDonViCon.splice(ind);
		// });

		this.ListDonViCon.splice(ind, 1);
		this.datasource = new MatTableDataSource(this.ListDonViCon);
	}
}
