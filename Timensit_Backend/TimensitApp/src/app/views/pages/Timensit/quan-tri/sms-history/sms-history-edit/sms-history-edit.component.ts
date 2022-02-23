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
import { SubheaderService } from '../../../../../../../app/core/_base/layout';
import { LayoutUtilsService, MessageType } from '../../../../../../../app/core/_base/crud';
import { SMSHistoryService } from '../sms-history-service/sms-history.service';
import { CommonService } from '../../../services/common.service';
//Models

import { SMSHistoryModel } from '../sms-history-model/sms-history.model';

@Component({
	selector: 'kt-sms-history-edit',
	templateUrl: './sms-history-edit.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
})

export class SMSHistoryEditComponent implements OnInit, OnDestroy {
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

	datasource: any;

	constructor(
		public dialogRef: MatDialogRef<SMSHistoryEditComponent>,
		@Inject(MAT_DIALOG_DATA) public data: any,
		private FormControlFB: FormBuilder,
		public dialog: MatDialog,
		private layoutUtilsService: LayoutUtilsService,
		private changeDetectorRefs: ChangeDetectorRef,
		private SMSHistorysService: SMSHistoryService,
		private commonService: CommonService) { }

	/**
	 * On init
	 */
	async ngOnInit() {


		this.commonService.fixedPoint = 0;

		this.viewLoading = true;
		this.ItemData = new SMSHistoryModel();
		this.ItemData.clear();
		//this.createForm();
		// await this.getTreeDonVi().then(res => {
		// 	if (res && res.status == 1)
		// 		this.datatreeDonVi.next(res.data);
		// 	else
		// 		this.datatreeDonVi.next([]);
		// });
		
		if (this.data.SMSHistory && this.data.SMSHistory.IdSMS > 0) {
			
			this.SMSHistorysService.getSMSHistoryById(this.data.SMSHistory.IdSMS).subscribe(res => {
				this.viewLoading = false;
				if (res.status == 1 && res.data) {
					this.ItemData = res.data;

					this.datasource=new MatTableDataSource(this.ItemData);
					//this.createForm();
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
		});

		if (this.data.SMSHistory.View)
			this.FormControls.disable();
	}
	/**
	 * Returns page title
	 */
	getTitle(): string {


		// if (this.ItemData.Id == 0) {
		// 	return 'Thêm mới ';
		// }

		// if (this.data.SMSHistory.View)
		// 	return `Xem ngày nghỉ `;

		return `Xem chi tiết lịch sử SMS`;
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
		this.disabledBtn = true;
		// tslint:disable-next-line:prefer-const
		let editedSMSHistory = this.prepareSMSHistorys();

		if (this.ItemData.Id > 0) {
			this.updateSMSHistory(editedSMSHistory)
			return;
		}

		this.addSMSHistory(editedSMSHistory, type);
	}

	/**
	 * Returns object for saving
	 */
	prepareSMSHistorys(): any {
		const controls = this.FormControls.controls;
		const _item: any = {};
		//_SMSHistory.clear();

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
	 * @param _SMSHistory: SMSHistoryModel
	 * @param withBack: boolean
	 */
	addSMSHistory(_SMSHistory: SMSHistoryModel, withBack: boolean = false) {

		this.SMSHistorysService.createSMSHistory(_SMSHistory).subscribe(res => {
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
	 * @param _SMSHistory: SMSHistorysModel
	 * @param withBack: boolean
	 */
	updateSMSHistory(_SMSHistory: SMSHistoryModel, withBack: boolean = false) {
		this.SMSHistorysService.updateSMSHistory(_SMSHistory).subscribe(res => {
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
















	async getTreeDonVi() {
		var res = await this.commonService.TreeDonVi().toPromise();
		return res;
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

	/* UI */
	getItemStatusString(status: number = 0): string {
		switch (status) {
			case 0:
				return 'Thất bại';
			case 1:
				return 'Thành công';
		}
		return '';
	}

	getItemCssClassByStatus(status: number = 0): string {
		switch (status) {
			case 0:
				return 'metal';
			case 1:
				return 'success';
		}
		return '';
	}
}
