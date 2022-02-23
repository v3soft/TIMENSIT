import { Component, OnInit, Inject, ChangeDetectionStrategy, HostListener, ViewChild, ElementRef, ChangeDetectorRef } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule, MatDialog } from '@angular/material';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { receiptsModel } from '../Model/receipts.model';
import { AngularEditorConfig } from '@kolkov/angular-editor';
import { TranslateService } from '@ngx-translate/core';
import { receiptsService } from '../Services/receipts.service';
import { CommonService } from '../../../services/common.service';
import { LayoutUtilsService } from '../../../../../../core/_base/crud';
import { investorModel } from '../../investor/model/investor.model';
import { contractEditDialogComponent } from '../../contract/contract-edit/contract-edit.dialog.component';
import { contractModel } from '../../contract/Model/contract.model';
@Component({
	selector: 'm-receipts-edit-dialog',
	templateUrl: './receipts-edit.dialog.component.html',
})
export class receiptsEditDialogComponent implements OnInit {
	item: receiptsModel;
	oldItem: receiptsModel;
	itemForm: FormGroup;
	hasFormErrors: boolean = false;
	viewLoading: boolean = false;
	filterDonVi: string = '';
	loadingAfterSubmit: boolean = false;
	list_contract: any[] = [];
	disabledBtn: boolean = false;
	allowEdit: boolean = true;
	isZoomSize: boolean = false;
	productsResult: investorModel[] = [];

	@ViewChild("focusInput", { static: true }) focusInput: ElementRef;
	_name = "";
	constructor(public dialogRef: MatDialogRef<receiptsEditDialogComponent>,
		@Inject(MAT_DIALOG_DATA) public data: any,
		private fb: FormBuilder,
		private danhMucService: CommonService,
		public receiptsService: receiptsService,
		private changeDetectorRefs: ChangeDetectorRef,
		private layoutUtilsService: LayoutUtilsService,
		public dialog: MatDialog,
		private translate: TranslateService) {
		this._name = this.translate.instant("RECEIPTS.NAME");

	}

	/** LOAD DATA */
	ngOnInit() {
		this.reset();
		this.item = this.data._item;
		if (this.data.allowEdit != undefined)
			this.allowEdit = this.data.allowEdit;
		this.createForm();
		//debugger
		if (this.item.Id_row > 0) {
			this.viewLoading = true;
			this.receiptsService.getItem(this.item.Id_row).subscribe(res => {
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

		//Load unit list
		this.danhMucService.getAllContract().subscribe(res => {
			this.list_contract = res.data;
		});
	}

	createForm() {

		this.itemForm = this.fb.group({
			ContractID: ['' + this.item.ContractID, Validators.required],
			Type: [this.item.Type],
			Amount: [this.item.Amount],
			ReceiptDate: [this.item.ReceiptDate],
			EffectiveDate: [this.item.EffectiveDate],
		});
		this.itemForm.controls["Amount"].markAsTouched();
		this.itemForm.controls["ContractID"].markAsTouched();
		this.focusInput.nativeElement.focus();
		if (!this.allowEdit)
			this.itemForm.disable();
	}

	/** UI */
	getTitle(): string {
		if (!this.allowEdit)
			return 'Xem chi tiết';
		let result = this.translate.instant('COMMON.CREATE');
		if (!this.item || !this.item.Id_row) {
			return result;
		}
		result = this.translate.instant('COMMON.UPDATE') + `- ${this.item.ContractCode}`;
		return result;
	}

	/** ACTIONS */
	prepareCustomer(): receiptsModel {
		const controls = this.itemForm.controls;
		const _item = new receiptsModel();
		_item.Id_row = this.item.Id_row;
		_item.ContractID = controls['ContractID'].value;
		_item.Type = controls['Type'].value;
		_item.Amount = +controls['Amount'].value;
		if (controls.ReceiptDate.value !== '')
			_item.ReceiptDate = this.danhMucService.f_convertDate(controls.ReceiptDate.value);
		if (controls.EffectiveDate.value !== '')
			_item.EffectiveDate = this.danhMucService.f_convertDate(controls.EffectiveDate.value);
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
		const Editreceipts = this.prepareCustomer();
		if (Editreceipts.Id_row > 0) {
			this.Updatereceipts(Editreceipts, withBack);
		} else {
			this.Createreceipts(Editreceipts, withBack);
		}
	}

	Updatereceipts(_item: receiptsModel, withBack: boolean) {

		this.loadingAfterSubmit = true;
		this.viewLoading = true;

		this.disabledBtn = true;
		this.receiptsService.Updatereceipts(_item).subscribe(res => {
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
	Add_ContractCode() {
		let _item = new contractModel;
		_item.Id_row = 0;
		_item.Amount = 0;
		_item.ContractCode = '';
		_item.StartDate = '';
		_item.EndDate = '';
		_item.Fund = '';
		_item.DepositPeriod = '';
		_item.ProfitShare = 0;
		let saveMessageTranslateParam = '';
		saveMessageTranslateParam += _item.Id_row > 0 ? 'OBJECT.EDIT.UPDATE_MESSAGE' : 'OBJECT.EDIT.ADD_MESSAGE';
		const _saveMessage = this.translate.instant(saveMessageTranslateParam, { name: this._name });
		const dialogRef = this.dialog.open(contractEditDialogComponent, { data: { _item } });
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				this.ngOnInit();
			}
			else {
				this.layoutUtilsService.showError(_saveMessage);
				this.ngOnInit();
			}
		});
	}
	Createreceipts(_item: receiptsModel, withBack: boolean) {
		this.loadingAfterSubmit = true;
		this.viewLoading = true;
		this.disabledBtn = true;
		this.receiptsService.Createreceipts(_item).subscribe(res => {
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
	reset() {
		this.item = Object.assign({}, this.item);
		this.createForm();
		this.hasFormErrors = false;
		this.itemForm.markAsPristine();
		this.itemForm.markAsUntouched();
		this.itemForm.updateValueAndValidity();
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
