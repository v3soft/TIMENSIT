import { Component, OnInit, Inject, ChangeDetectionStrategy, HostListener, ViewChild, ElementRef, ChangeDetectorRef } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { investorModel } from '../model/investor.model';
import { LayoutUtilsService } from '../../../../../../core/_base/crud';
import { investorService } from '../services/investor.service';
import { AngularEditorConfig } from '@kolkov/angular-editor';
@Component({
	selector: 'm-investor-edit-dialog',
	templateUrl: './investor-edit.dialog.component.html',
})
export class investorEditDialogComponent implements OnInit {
	item: investorModel;
	oldItem: investorModel;
	itemForm: FormGroup;
	hasFormErrors: boolean = false;
	viewLoading: boolean = false;
	loadingAfterSubmit: boolean = false;
	listDonVi: any[] = [];
	disabledBtn: boolean = false;
	allowEdit: boolean = true;
	isZoomSize: boolean = false;
	@ViewChild("focusInput", { static: true }) focusInput: ElementRef;
	_name = "";
	constructor(public dialogRef: MatDialogRef<investorEditDialogComponent>,
		@Inject(MAT_DIALOG_DATA) public data: any,
		private fb: FormBuilder,
		private investorService: investorService,
		private changeDetectorRefs: ChangeDetectorRef,
		private layoutUtilsService: LayoutUtilsService,
		private translate: TranslateService) {
		this._name = this.translate.instant("CHUC_DANH.NAME");
	}

	/** LOAD DATA */
	ngOnInit() {
		this.item = this.data._item;
		if (this.data.allowEdit != undefined)
			this.allowEdit = this.data.allowEdit;
		this.createForm();
		if (this.item.ID > 0) {
			this.viewLoading = true;
			this.investorService.getItem(this.item.ID).subscribe(res => {
				this.viewLoading = false;
				this.changeDetectorRefs.detectChanges();
				if (res && res.status == 1) {
					this.item = res.data;
					this.createForm();
				}
				else
					this.layoutUtilsService.showError(res.error.message);
			});
		}
	}

	createForm() {
		this.itemForm = this.fb.group({
			Name: ['' + this.item.Name, Validators.required],
			Email: ['' + this.item.Email, Validators.required],
			Phone: ['' + this.item.Phone],
			Address: ['' + this.item.Address],
			CitizenID: ['' + this.item.CitizenID],

		});
		this.focusInput.nativeElement.focus();
		if (!this.allowEdit)
			this.itemForm.disable();
	}

	/** UI */
	getTitle(): string {
		if (!this.allowEdit)
			return 'Xem chi tiết';
		let result = this.translate.instant('COMMON.CREATE');
		if (!this.item || !this.item.ID) {
			return result;
		}
		result = this.translate.instant('COMMON.UPDATE') + ' - ' + this.item.Name;
		return result;
	}

	/** ACTIONS */
	PrepareCustomer(): investorModel {

		const controls = this.itemForm.controls;
		const _item = new investorModel();
		_item.ID = this.item.ID;
		_item.Name = controls['Name'].value; // lấy tên biến trong formControlName
		_item.Email = controls['Email'].value;
		_item.Phone = controls['Phone'].value;
		_item.Address = controls['Address'].value;
		_item.CitizenID = controls['CitizenID'].value;

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
		const EditPosition = this.PrepareCustomer();
		if (EditPosition.ID > 0) {
			this.UpdatePosition(EditPosition, withBack);
		} else {
			this.CreatePosition(EditPosition, withBack);
		}
	}

	UpdatePosition(_item: investorModel, withBack: boolean) {

		this.loadingAfterSubmit = true;
		this.viewLoading = true;
		this.disabledBtn = true;
		this.investorService.UpdatePosition(_item).subscribe(res => {
			/* Server loading imitation. Remove this on real code */
			this.disabledBtn = false;
			this.changeDetectorRefs.detectChanges();
			if (res && res.status === 1) {
				if (withBack == true) {
					this.dialogRef.close({
						_item
					});
				}
				else {
					this.ngOnInit();
					const _messageType = this.translate.instant('OBJECT.EDIT.UPDATE_MESSAGE', { name: this._name });
					this.layoutUtilsService.showInfo(_messageType).afterDismissed().subscribe(tt => {
					});
					this.focusInput.nativeElement.focus();
				}
			}
			else {
				this.layoutUtilsService.showError(res.error.message);
			}
		});
	}
	reset() {
		this.item = Object.assign({}, this.item);
		this.createForm();
		this.hasFormErrors = false;
		this.itemForm.markAsPristine();
		this.itemForm.markAsUntouched();
		this.itemForm.updateValueAndValidity();
	}
	CreatePosition(_item: investorModel, withBack: boolean) {
		this.loadingAfterSubmit = true;
		//	this.viewLoading = true;
		this.disabledBtn = true;
		this.investorService.CreatePosition(_item).subscribe(res => {
			this.disabledBtn = false;
			this.changeDetectorRefs.detectChanges();
			if (res && res.status === 1) {
				if (withBack == true) {
					this.dialogRef.close({
						_item
					});
				}
				else {
					const _messageType = this.translate.instant('OBJECT.EDIT.ADD_MESSAGE', { name: this._name });
					this.layoutUtilsService.showInfo(_messageType).afterDismissed().subscribe(tt => {
					});
					this.focusInput.nativeElement.focus();
					this.ngOnInit();
				}
			}
			else {
				this.viewLoading = false;
				this.layoutUtilsService.showError(res.error.message);
			}
		});
	}

	onAlertClose($event) {
		this.hasFormErrors = false;
	}

	close() {
		this.dialogRef.close();
	}

	editorConfig: AngularEditorConfig = {
		editable: true,
		spellcheck: true,
		height: '15rem',
		minHeight: '5rem',
		placeholder: 'Nhập vào nội dung',
		translate: 'no',
		//uploadUrl: SystemConstants.BASE_API + 'api/tien-ich/upload-img?token=' + JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER)).token, // if needed
		customClasses: [ // optional
			{
				name: "quote",
				class: "quote",
			},
			{
				name: 'redText',
				class: 'redText'
			},
			{
				name: "titleText",
				class: "titleText",
				tag: "h1",
			},
		]
	};
	@HostListener('document:keydown', ['$event'])
	onKeydownHandler(event: KeyboardEvent) {
		if (event.ctrlKey && event.keyCode == 13)//phím Enter
		{
			this.item = this.data._item;
			if (this.viewLoading == true) {
				this.onSubmit(true);
			}
			else {
				this.onSubmit(false);
			}
		}
	}
}
