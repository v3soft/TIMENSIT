
import { TranslateService } from '@ngx-translate/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Component, OnInit, ElementRef, Inject, ChangeDetectorRef, ViewChild, HostListener } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { CommonService } from '../../Timensit/services/common.service';
import { LayoutUtilsService } from 'app/core/_base/crud/utils/layout-utils.service';
import { hdsdService } from '../Services/hdsd.service';

@Component({
	selector: 'kt-hdsd-edit',
	templateUrl: './hdsd-edit.dialog.component.html',
})
export class HDSDEditDialogComponent implements OnInit {
	item: any;
	itemForm: FormGroup;
	hasFormErrors: boolean = false;
	viewLoading: boolean = false;
	loadingAfterSubmit: boolean = false;
	disabledBtn: boolean = false;
	allowEdit: boolean = true;
	isZoomSize: boolean = false;
	lstCanCu: any[];
	change: boolean = false;
	@ViewChild("focusInput", { static: true }) focusInput: ElementRef;
	arr: string[] = [];
	_name = "";
	strHtml: any;
	keys: any[] = [];
	lstLoai: any[] = [];
	lstLoaiQD: any[] = [];
	Id: number = 0;

	constructor(public dialogRef: MatDialogRef<HDSDEditDialogComponent>,
		@Inject(MAT_DIALOG_DATA) public data: any,
		private fb: FormBuilder,
		public dialog: MatDialog,
		public commonService: CommonService,
		private hdsdService1: hdsdService,
		private layoutUtilsService: LayoutUtilsService,
		private changeDetectorRefs: ChangeDetectorRef,
		private translate: TranslateService,
		private sanitized: DomSanitizer) {
		this._name = this.translate.instant("LOAI_DD.NAME");
	}
	transform(value) {
		return this.sanitized.bypassSecurityTrustHtml(value);
	}
	ngOnInit() {
		this.item = this.data._item;
		if (this.data.allowEdit != undefined)
			this.allowEdit = this.data.allowEdit;
		this.createForm();
		if (+this.item.Id > 0) {
			this.hdsdService1.getItem(this.item.Id).subscribe(res => {
				if (res && res.status == 1) {
					this.item = res.data;
					this.Id = this.item.Id;
					this.createForm();
				} else
					this.layoutUtilsService.showError(res.error.message);
			})
		}
	}

	createForm() {
		let temp = {
			TenHuongDan: [this.item.HDSD, Validators.required],
			IsUp: [this.item.Id == 0?true:false],
			fileDinhKem: [null]
		};
		this.itemForm = this.fb.group(temp);

		this.focusInput.nativeElement.focus();
		if (!this.allowEdit)
			this.itemForm.disable();
		this.changeDetectorRefs.detectChanges();
	}

	getTitle(): string {
		if (this.item.Id > 0) {
			if (this.allowEdit)
				return 'Cập nhật hướng dẫn';
			else
				return 'Chi tiết hướng dẫn';
		}
		else
			return 'Thêm mới hướng dẫn';
	}
	/** ACTIONS */
	prepareCustomer(): any {
		//debugger
		const controls = this.itemForm.controls;
		const _item: any = {};
		_item.Id = this.item.Id;
		_item.IsUp = controls['IsUp'].value;
		_item.TenHuongDan = controls['TenHuongDan'].value;
		_item.FileDinhKem = null;
		let f = controls['fileDinhKem'].value;
		if (f && f.length > 0)
			_item.FileDinhKem = f[0];

		return _item;
	}
	onSubmit(withBack: boolean = false) {

		this.hasFormErrors = false;
		this.loadingAfterSubmit = false;
		const controls = this.itemForm.controls;
		/* check form */
		if (this.itemForm.invalid) {
			Object.keys(controls).forEach(controlName =>
				controls[controlName].markAsTouched()
			);

			this.hasFormErrors = true;
			return;
		}
		const EditDanhmucKhac = this.prepareCustomer();
		if (EditDanhmucKhac.Id > 0) {
			this.UpdateDanhmuc(EditDanhmucKhac);
		} else {
			this.CreateDanhmuc(EditDanhmucKhac, withBack);
		}
	}
	UpdateDanhmuc(_item: any) {
		this.loadingAfterSubmit = true;
		this.viewLoading = true;
		this.disabledBtn = true;
		this.hdsdService1.UpdateItem(_item).subscribe(res => {
			this.disabledBtn = false;
			this.changeDetectorRefs.detectChanges();
			if (res && res.status === 1) {
				this.dialogRef.close({
					_item
				});
			}
			else {
				this.layoutUtilsService.showError(res.error.message);
			}
		});
	}

	CreateDanhmuc(_item: any, withBack: boolean) {
		this.loadingAfterSubmit = true;
		//	this.viewLoading = true;
		this.disabledBtn = true;
		this.hdsdService1.CreateItem(_item).subscribe(res => {
			this.disabledBtn = false;
			this.changeDetectorRefs.detectChanges();
			if (res && res.status === 1) {
				if (withBack == true) {
					this.dialogRef.close({
						_item
					});
				}
				else {
					this.change = true;
					this.reset();
				}
			}
			else {
				this.viewLoading = false;
				this.layoutUtilsService.showError(res.error.message);
			}
		});
	}
	reset() {
		this.item = { Id: 0, Version: '1.0.0', content: '$' };
		this.createForm();
	}
	onAlertClose($event) {
		this.hasFormErrors = false;
	}
	close() {
		this.dialogRef.close(this.change);
	}
	// download() {
	// 	this.hdsdService1.download(this.item.Id).subscribe(response => {
	// 		const headers = response.headers;
	// 		const filename = headers.get('x-filename');
	// 		const type = headers.get('content-type');
	// 		const blob = new Blob([response.body], { type });
	// 		const fileURL = URL.createObjectURL(blob);
	// 		const link = document.createElement('a');
	// 		link.href = fileURL;
	// 		link.download = filename;
	// 		link.click();
	// 	});
	// }
}
