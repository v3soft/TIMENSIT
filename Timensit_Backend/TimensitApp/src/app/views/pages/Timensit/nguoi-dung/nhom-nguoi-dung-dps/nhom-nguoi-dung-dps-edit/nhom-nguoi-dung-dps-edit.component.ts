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
import { SubheaderService } from 'app/core/_base/layout';
import { LayoutUtilsService, MessageType } from 'app/core/_base/crud';
import { NhomNguoiDungDPSService } from '../nhom-nguoi-dung-dps-service/nhom-nguoi-dung-dps.service';
//Models

import { NhomNguoiDungDPSModel } from '../nhom-nguoi-dung-dps-model/nhom-nguoi-dung-dps.model';

import moment from 'moment';
import { CommonService } from '../../../services/common.service';

@Component({
	selector: 'kt-nhom-nguoi-dung-dps-edit',
	templateUrl: './nhom-nguoi-dung-dps-edit.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NhomNguoiDungDPSEditComponent implements OnInit, OnDestroy {
	// Public properties
	NhomNguoiDungDPS: NhomNguoiDungDPSModel;
	NhomNguoiDungDPSForm: FormGroup;
	hasFormErrors: boolean = false;
	loadingSubject = new BehaviorSubject<boolean>(true);
	loading$: Observable<boolean>;
	viewLoading: boolean = false;
	isChange: boolean = false;

	fixedPoint = 0;

	isZoomSize: boolean = false;
	private componentSubscriptions: Subscription;
	datatree: BehaviorSubject<any[]> = new BehaviorSubject([]);
	lstChucVu: any[];
	allowEdit: boolean = true;
	disabledBtn: boolean = false;
	constructor(
		public dialogRef: MatDialogRef<NhomNguoiDungDPSEditComponent>,
		@Inject(MAT_DIALOG_DATA) public data: any,
		private NhomNguoiDungDPSFB: FormBuilder,
		public dialog: MatDialog,
		private layoutUtilsService: LayoutUtilsService,
		private changeDetectorRefs: ChangeDetectorRef,
		private nhomnguoidungdpssService: NhomNguoiDungDPSService,
		private commonService: CommonService) { }

	/**
	 * On init
	 */
	ngOnInit() {
		this.viewLoading = true;
		this.NhomNguoiDungDPS = this.data.NhomNguoiDungDPS;
		this.allowEdit = this.data.allowEdit;
		this.getTree();
		this.createForm();
		if (this.data.NhomNguoiDungDPS && this.data.NhomNguoiDungDPS.IdGroup > 0) {
			this.nhomnguoidungdpssService.getNhomNguoiDungDPSById(this.data.NhomNguoiDungDPS.IdGroup).subscribe(res => {
				this.viewLoading = false;
				if (res.status == 1 && res.data) {
					this.NhomNguoiDungDPS = res.data;
					this.loadChucVuByDonVi(this.NhomNguoiDungDPS.DonVi);
					this.createForm();
				}
				else {
					this.layoutUtilsService.showError(res.error.message);
				}
				this.changeDetectorRefs.detectChanges();
			});
		} else {
			this.loadChucVuByDonVi(this.NhomNguoiDungDPS.DonVi);
			this.viewLoading = false;
			this.changeDetectorRefs.detectChanges();
		}



	}

	loadChucVuByDonVi(Donvi) {
		this.commonService.ListChucVu(Donvi).subscribe(res => {
			if (res && res.status == 1) {
				this.lstChucVu = res.data;
			}
			else {
				this.lstChucVu = [];
				this.layoutUtilsService.showError(res.error.message);
			}
			this.changeDetectorRefs.detectChanges();
		})
	}
	getTree() {
		let Locked = false;
		if (this.NhomNguoiDungDPS.IdGroup > 0)
			Locked = true;
		this.commonService.TreeDonVi(0, 0, Locked).subscribe(res => {
			// this.commonService.TreeDonVi().subscribe(res => {
			if (res && res.status == 1) {
				this.datatree.next(res.data);
				if (this.NhomNguoiDungDPS.IdGroup == 0 && res.data.length > 0) 
					this.NhomNguoiDungDPSForm.controls["donVi"].setValue(res.data[0]["id"]);
			}
			else {
				this.datatree.next([]);
				this.layoutUtilsService.showError(res.error.message);
			}
			this.changeDetectorRefs.detectChanges();
		})
	}
	GetValueNode(item) {
		this.loadChucVuByDonVi(item.id);
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
		this.NhomNguoiDungDPSForm = this.NhomNguoiDungDPSFB.group({
			groupName: [this.NhomNguoiDungDPS.GroupName, [Validators.required, Validators.maxLength(250)]],
			ma: [this.NhomNguoiDungDPS.Ma, [Validators.required, Validators.maxLength(250)]],
			ghiChu: [this.NhomNguoiDungDPS.GhiChu, Validators.maxLength(500)],
			displayOrder: [this.NhomNguoiDungDPS.DisplayOrder, Validators.min(1)],
			locked: [this.NhomNguoiDungDPS.Locked],
			isDefault: [this.NhomNguoiDungDPS.IsDefault],
			donVi: [this.NhomNguoiDungDPS.DonVi, Validators.required],
			chucVu: [this.NhomNguoiDungDPS.ChucVu == null ? '0' : this.NhomNguoiDungDPS.ChucVu + '']
		});
		if (!this.allowEdit)
			this.NhomNguoiDungDPSForm.disable();
		this.changeDetectorRefs.detectChanges();
	}

	reset() {
		this.NhomNguoiDungDPS.clear();
		this.createForm();
	}
	/**
	 * Returns page title
	 */
	getTitle(): string {
		if (!this.allowEdit)
			return 'Chi tiết vai trò';
		if (this.NhomNguoiDungDPS.IdGroup == 0) {
			return 'Thêm mới vai trò';
		}
		return `Chỉnh sửa vai trò - ${this.NhomNguoiDungDPS.GroupName} `;
	}

	/**
	 * Check control is invalid
	 * @param controlName: string
	 */
	isControlInvalid(controlName: string): boolean {
		const control = this.NhomNguoiDungDPSForm.controls[controlName];
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
		const controls = this.NhomNguoiDungDPSForm.controls;
		/** check form */
		if (this.NhomNguoiDungDPSForm.invalid) {
			Object.keys(controls).forEach(controlName =>
				controls[controlName].markAsTouched()
			);
			let invalid = <FormControl[]>Object.keys(this.NhomNguoiDungDPSForm.controls).map(key => this.NhomNguoiDungDPSForm.controls[key]).filter(ctl => ctl.invalid);
			let invalidElem: any = invalid[0];
			invalidElem.nativeElement.focus();
			this.hasFormErrors = true;
			return;
		}

		// tslint:disable-next-line:prefer-const
		let editedNhomNguoiDungDPS = this.prepareNhomNguoiDungDPSs();

		if (this.NhomNguoiDungDPS.IdGroup > 0) {
			this.updateNhomNguoiDungDPS(editedNhomNguoiDungDPS)
			return;
		}

		this.addNhomNguoiDungDPS(editedNhomNguoiDungDPS, type);
	}

	/**
	 * Returns object for saving
	 */
	prepareNhomNguoiDungDPSs(): NhomNguoiDungDPSModel {
		const controls = this.NhomNguoiDungDPSForm.controls;
		const _NhomNguoiDungDPS = new NhomNguoiDungDPSModel();
		_NhomNguoiDungDPS.clear();
		_NhomNguoiDungDPS.GroupName = controls['groupName'].value;
		_NhomNguoiDungDPS.Ma = controls['ma'].value;
		_NhomNguoiDungDPS.DisplayOrder = controls['displayOrder'].value;
		_NhomNguoiDungDPS.Locked = controls['locked'].value;
		_NhomNguoiDungDPS.GhiChu = controls['ghiChu'].value;
		_NhomNguoiDungDPS.DonVi = controls['donVi'].value;
		_NhomNguoiDungDPS.ChucVu = controls['chucVu'].value;
		_NhomNguoiDungDPS.IsDefault = controls['isDefault'].value;
		//gán lại giá trị id 
		if (this.NhomNguoiDungDPS.IdGroup > 0) {
			_NhomNguoiDungDPS.IdGroup = this.NhomNguoiDungDPS.IdGroup;
		}

		return _NhomNguoiDungDPS;
	}
	/**
	 * Add item
	 *
	 * @param _NhomNguoiDungDPS: NhomNguoiDungDPSModel
	 * @param withBack: boolean
	 */
	addNhomNguoiDungDPS(_NhomNguoiDungDPS: NhomNguoiDungDPSModel, withBack: boolean = false) {

		this.nhomnguoidungdpssService.createNhomNguoiDungDPS(_NhomNguoiDungDPS).subscribe(res => {
			if (res.status == 1) {
				this.isChange = true;
				const message = `Thêm mới vai trò thành công`;
				this.layoutUtilsService.showInfo(message);
				//this.NhomNguoiDungDPSForm.reset();
				if (withBack)
					this.dialogRef.close(this.isChange);
				this.reset();
			}
			else {
				this.layoutUtilsService.showError(res.error.message);
			}
		});
	}

	/**
	 * Update item
	 *
	 * @param _NhomNguoiDungDPS: NhomNguoiDungDPSsModel
	 * @param withBack: boolean
	 */
	updateNhomNguoiDungDPS(_NhomNguoiDungDPS: NhomNguoiDungDPSModel, withBack: boolean = false) {

		this.nhomnguoidungdpssService.updateNhomNguoiDungDPS(_NhomNguoiDungDPS).subscribe(res => {
			if (res.status == 1) {
				this.isChange = true;
				const message = `Cập nhật vai trò thành công`;
				this.layoutUtilsService.showInfo(message);
				this.dialogRef.close(this.isChange);
			}
			else {
				this.layoutUtilsService.showError(res.error.message);
			}
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
