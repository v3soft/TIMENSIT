// Angular
import { Component, OnInit, ChangeDetectionStrategy, OnDestroy, ChangeDetectorRef, Inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
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

@Component({
	selector: 'kt-cau-hinh-email-popup-donvicon',
	templateUrl: './cau-hinh-email-popup-donvicon.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
})

export class CauHinhEmailPopupDVCComponent implements OnInit, OnDestroy {
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
	public datatreeDonVi: BehaviorSubject<any[]> = new BehaviorSubject([]);
	private componentSubscriptions: Subscription;

	ListDonViCon: any[] = [];

	constructor(
		public dialogRef: MatDialogRef<CauHinhEmailPopupDVCComponent>,
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
		//this.createForm();
		//this.getAllDanhMucKhac();
		// await this.getTreeDonVi().then(res => {
		// 	if (res && res.status == 1)
		// 		this.datatreeDonVi.next(res.data);
		// 	else
		// 		this.datatreeDonVi.next([]);
		// });
		//this.getTreeDonVi();
		this.commonService.getDonViTheoParent(this.data.InfoDonViCon.Id).subscribe(res => {
			this.viewLoading = false;
			if (res.status == 1 && res.data) {
				// this.ItemData = res.data;
				// this.createForm();

				let LstDVC: any[] = [];
				for (var i = 0; i < res.data.length; i++) {
					var objdetail: any = {};
					objdetail.check = this.data.InfoDonViCon?(this.data.InfoDonViCon.LstDonViCon.length>0?(this.data.InfoDonViCon.LstDonViCon.filter(x => x.Id == res.data[i].Id).length>0?true:false):false):false;
					objdetail.Id = res.data[i].Id;
					objdetail.DonVi = res.data[i].DonVi;
					LstDVC.push(objdetail)
				}
				this.ListDonViCon = LstDVC;
			}
			else {
				this.layoutUtilsService.showError(res.error.message);
			}
			this.changeDetectorRefs.detectChanges();
		});

		//this.CheckRoles();
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

	CheckedChange(p: any, e: any) {
		p.check = e;
		//console.log('list', this.ListDonViCon);
	}

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
			Port: [this.ItemData.Port == null ? '' : this.ItemData.Port, [Validators.required]],
			UserName: [this.ItemData.UserName == null ? '' : this.ItemData.UserName, [Validators.required]],
			DonVi: [this.ItemData.DonVi == null ? '' : this.ItemData.DonVi, [Validators.required]],
			EnableSSL: [this.ItemData.EnableSSL == null ? false : this.ItemData.EnableSSL, [Validators.required]],
			Password: [this.ItemData.Password == null ? '' : this.ItemData.Password, [Validators.required]],
		});

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
		// this.hasFormErrors = false;
		// const controls = this.FormControls.controls;
		// /** check form */
		// if (this.FormControls.invalid) {
		// 	Object.keys(controls).forEach(controlName =>
		// 		controls[controlName].markAsTouched()
		// 	);
		// 	this.hasFormErrors = true;
		// 	return;
		// }
		// this.disabledBtn = true;
		// // tslint:disable-next-line:prefer-const
		// let editedCauHinhEmail = this.prepareCauHinhEmails();

		// if (this.ItemData.Id > 0) {
		// 	this.updateCauHinhEmail(editedCauHinhEmail)
		// 	return;
		// }

		// this.addCauHinhEmail(editedCauHinhEmail, type);
		let ArrDVC:any[]=[];
		for (var i = 0; i < this.ListDonViCon.length; i++) {
			if (this.ListDonViCon[i].check) { 
				ArrDVC.push(this.ListDonViCon[i]);
			}
		}
		if (type) {
			this.dialogRef.close(ArrDVC);
		}
		else {
			this.dialogRef.close();
		}
	}

	/**
	 * Returns object for saving
	 */
	prepareCauHinhEmails(): any {
		const controls = this.FormControls.controls;
		const _item: any = {};
		//_CauHinhEmail.clear();

		_item.Cast_BDNghi = controls['bDNghi'].value.split('T')[0];

		_item.Cast_KTNghi = controls['kTNghi'].value.split('T')[0];
		_item.DotNghiRQ = controls['dotNghi'].value;
		_item.MoTa = controls['moTa'].value;
		//gán lại giá trị id 

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
}
