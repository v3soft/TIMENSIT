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
import { CauHinhSMSService } from '../cau-hinh-sms-service/cau-hinh-sms.service';
import { CommonService } from '../../../services/common.service';
//Models

import { CauHinhSMSModel } from '../cau-hinh-sms-model/cau-hinh-sms.model';

import { CauHinhSMSPopupDVCComponent } from '../cau-hinh-sms-popup-donvicon/cau-hinh-sms-popup-donvicon.component';

@Component({
	selector: 'kt-cau-hinh-sms-edit',
	templateUrl: './cau-hinh-sms-edit.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
})


export class CauHinhSMSEditComponent implements OnInit, OnDestroy {
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
	datasource:any;
	public datatreeDonVi: BehaviorSubject<any[]> = new BehaviorSubject([]);
	private componentSubscriptions: Subscription;

	ListDonViCon: any[] = [];

	constructor(
		public dialogRef: MatDialogRef<CauHinhSMSEditComponent>,
		@Inject(MAT_DIALOG_DATA) public data: any,
		private FormControlFB: FormBuilder,
		public dialog: MatDialog,
		private layoutUtilsService: LayoutUtilsService,
		private changeDetectorRefs: ChangeDetectorRef,
		private CauHinhSMSsService: CauHinhSMSService,
		private commonService: CommonService) { }

	/**
	 * On init
	 */
	async ngOnInit() {
		this.commonService.fixedPoint = 0;

		this.viewLoading = true;
		this.ItemData = new CauHinhSMSModel();
		//this.ItemData.clear();
		this.ItemData.Id = 0;
		this.createForm();
		// await this.getTreeDonVi().then(res => {
		// 	if (res && res.status == 1)
		// 		this.datatreeDonVi.next(res.data);
		// 	else
		// 		this.datatreeDonVi.next([]);
		// });
		this.getTreeDonVi();
		setTimeout(() => {
			if (this.data.CauHinhSMS && this.data.CauHinhSMS.Id > 0) {
				this.CauHinhSMSsService.getCauHinhSMSById(this.data.CauHinhSMS.Id).subscribe(res => {
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

			URL: [this.ItemData.URL == null ? '' : this.ItemData.URL, [Validators.required]],
			Brandname: [this.ItemData.Brandname == null ? '' : this.ItemData.Brandname, [Validators.required]],
			UserName: [this.ItemData.UserName == null ? '' : this.ItemData.UserName, [Validators.required]],
			DonVi: [this.ItemData.DonVi == null ? '' : this.ItemData.DonVi],
			DauSo: [this.ItemData.DauSo == null ? '' : this.ItemData.DauSo, [Validators.required]],
			ServiceId: [this.ItemData.ServiceId == null ? '' : this.ItemData.ServiceId, [Validators.required]],
			Password: [this.ItemData.Password == null ? '' : this.ItemData.Password, [Validators.required]],
			IsDungChung: [this.ItemData.DonVi == 0]
		});

		this.ListDonViCon = this.ItemData.DonViCon;
		this.datasource=new MatTableDataSource(this.ListDonViCon);

		if (this.data.CauHinhSMS.View)
			this.FormControls.disable();
	}
	/**
	 * Returns page title
	 */
	getTitle(): string {


		if (this.ItemData.Id == 0) {
			return 'Thêm mới cấu hình sms';
		}

		if (this.data.CauHinhSMS.View)
			return `Xem cấu hình sms `;

		return `Chỉnh sửa cấu hình sms`;
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
		let editedCauHinhSMS = this.prepareCauHinhSMSs();

		if (this.ItemData.Id > 0) {
			this.updateCauHinhSMS(editedCauHinhSMS)
			return;
		}

		this.addCauHinhSMS(editedCauHinhSMS, type);
	}

	/**
	 * Returns object for saving
	 */
	prepareCauHinhSMSs(): any {
		const controls = this.FormControls.controls;
		const _item: any = {};
		//_CauHinhSMS.clear();


		// URL: [this.ItemData.URL == null ? '' : this.ItemData.URL, [Validators.required]],
		// Brandname: [this.ItemData.Brandname == null ? '' : this.ItemData.Brandname, [Validators.required, Validators.min(1)]],
		// UserName: [this.ItemData.UserName == null ? '' : this.ItemData.UserName, [Validators.required]],
		// DonVi: [this.ItemData.DonVi == null ? '' : this.ItemData.DonVi, [Validators.required]],
		// DauSo: [this.ItemData.DauSo == null ? '' : this.ItemData.DauSo, [Validators.required]],
		// ServiceId: [this.ItemData.ServiceId == null ? '' : this.ItemData.ServiceId, [Validators.required]],
		// Password: [this.ItemData.Password == null ? '' : this.ItemData.Password, [Validators.required]],

		_item.URL = controls['URL'].value;
		_item.Brandname = controls['Brandname'].value;
		_item.UserName = controls['UserName'].value;
		_item.DauSo = controls['DauSo'].value;
		_item.ServiceId = controls['ServiceId'].value;
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
	 * @param _CauHinhSMS: CauHinhSMSModel
	 * @param withBack: boolean
	 */
	addCauHinhSMS(_CauHinhSMS: CauHinhSMSModel, withBack: boolean = false) {

		this.CauHinhSMSsService.createCauHinhSMS(_CauHinhSMS).subscribe(res => {
			if (res.status == 1) {
				this.isChange = true;
				const message = `Thêm thành công`;
				this.layoutUtilsService.showInfo(message);
				this.FormControls.reset();
				this.ListDonViCon=[];
				this.datasource=new MatTableDataSource(this.ListDonViCon);
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
	 * @param _CauHinhSMS: CauHinhSMSsModel
	 * @param withBack: boolean
	 */
	updateCauHinhSMS(_CauHinhSMS: CauHinhSMSModel, withBack: boolean = false) {
		this.CauHinhSMSsService.updateCauHinhSMS(_CauHinhSMS).subscribe(res => {
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
		if (this.FormControls.controls['DonVi'].value == '') {
			this.layoutUtilsService.showInfo('Chưa chọn đơn vị');
			return;
		}
		let InfoDonViCon = { Id: this.FormControls.controls['DonVi'].value, LstDonViCon: this.data.CauHinhSMS && this.data.CauHinhSMS.Id > 0 ? this.ItemData.DonViCon : [] };
		const dialogRef = this.dialog.open(CauHinhSMSPopupDVCComponent, { data: { InfoDonViCon } });
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}

			this.ListDonViCon = this.ItemData.DonViCon = res;
			this.datasource=new MatTableDataSource(this.ListDonViCon);
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

		this.ListDonViCon.splice(ind);
		this.datasource=new MatTableDataSource(this.ListDonViCon);
	}
}
