import { Component, OnInit, Inject, ChangeDetectionStrategy, HostListener, ViewChild, ElementRef, ChangeDetectorRef } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule, MatDialog } from '@angular/material';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { contractModel } from '../Model/contract.model';
import { AngularEditorConfig } from '@kolkov/angular-editor';
import { TranslateService } from '@ngx-translate/core';
import { contractService } from '../Services/contract.service';
import { CommonService } from '../../../services/common.service';
import { LayoutUtilsService } from '../../../../../../core/_base/crud';
import { investorEditDialogComponent } from '../../investor/investor-edit/investor-edit.dialog.component';
import { investorModel } from '../../investor/model/investor.model';
@Component({
	selector: 'm-contract-edit-dialog',
	templateUrl: './contract-edit.dialog.component.html',
})
export class contractEditDialogComponent implements OnInit {
	item: contractModel;
	oldItem: contractModel
	itemForm: FormGroup;
	hasFormErrors: boolean = false;
	viewLoading: boolean = false;
	filterDonVi: string = '';
	loadingAfterSubmit: boolean = false;
	list_Investor: any[] = [];
	disabledBtn: boolean = false;
	allowEdit: boolean = true;
	isZoomSize: boolean = false;
	productsResult: investorModel[] = [];

	@ViewChild("focusInput", { static: true }) focusInput: ElementRef;
	_name = "";
	constructor(public dialogRef: MatDialogRef<contractEditDialogComponent>,
		@Inject(MAT_DIALOG_DATA) public data: any,
		private fb: FormBuilder,
		private danhMucService: CommonService,
		public contractService: contractService,
		private changeDetectorRefs: ChangeDetectorRef,
		private layoutUtilsService: LayoutUtilsService,
		public dialog: MatDialog,
		private translate: TranslateService) {
		this._name = this.translate.instant("CONTRACT.NAME");

	}

	/** LOAD DATA */
	ngOnInit() {
		this.reset();
		this.item = this.data._item;
		if (this.data.allowEdit != undefined)
			this.allowEdit = this.data.allowEdit;
		this.createForm();
		if (this.item.Id_row > 0) {
			this.viewLoading = true;
			this.contractService.getItem(this.item.Id_row).subscribe(res => {
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
		this.danhMucService.getAllInvestor().subscribe(res => {
			this.list_Investor = res.data;
		});
	}

	createForm() {

		this.itemForm = this.fb.group({
			ContractCode: [this.item.ContractCode, Validators.required],
			InvestorID: ['' + this.item.InvestorID, Validators.required],
			DepositPeriod: [this.item.DepositPeriod],
			Fund: [this.item.Fund],
			Amount: [this.item.Amount],
			ProfitShare: [this.item.ProfitShare],
			StartDate: [this.item.StartDate],
			EndDate: [this.item.EndDate],
		});
		this.itemForm.controls["ContractCode"].markAsTouched();
		this.itemForm.controls["InvestorID"].markAsTouched();
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
	prepareCustomer(): contractModel {
		const controls = this.itemForm.controls;
		const _item = new contractModel();
		_item.Id_row = this.item.Id_row;
		_item.ContractCode = controls['ContractCode'].value; // lấy tên biến trong formControlName
		_item.InvestorID = controls['InvestorID'].value;
		_item.DepositPeriod = controls['DepositPeriod'].value;
		_item.Fund = controls['Fund'].value;
		_item.Amount = +controls['Amount'].value;
		_item.ProfitShare = +controls['ProfitShare'].value;
		if (controls.StartDate.value !== '')
			_item.StartDate = this.danhMucService.f_convertDate(controls.StartDate.value);
		if (controls.EndDate.value !== '')
			_item.EndDate = this.danhMucService.f_convertDate(controls.EndDate.value);
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
		const Editcontract = this.prepareCustomer();
		if (Editcontract.Id_row > 0) {
			this.Updatecontract(Editcontract, withBack);
		} else {
			this.Createcontract(Editcontract, withBack);
		}
	}

	Updatecontract(_item: contractModel, withBack: boolean) {

		this.loadingAfterSubmit = true;
		this.viewLoading = true;

		this.disabledBtn = true;
		this.contractService.Updatecontract(_item).subscribe(res => {
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
	Add_Investor() {
		let _item = new investorModel;
		_item.ID = 0;
		_item.Address = '';
		_item.CitizenID = '';
		_item.Name = '';
		_item.Email = '';
		_item.Phone = '';
		let saveMessageTranslateParam = '';
		saveMessageTranslateParam += _item.ID > 0 ? 'OBJECT.EDIT.UPDATE_MESSAGE' : 'OBJECT.EDIT.ADD_MESSAGE';
		const _saveMessage = this.translate.instant(saveMessageTranslateParam, { name: this._name });
		const dialogRef = this.dialog.open(investorEditDialogComponent, { data: { _item } });
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
	Createcontract(_item: contractModel, withBack: boolean) {
		this.loadingAfterSubmit = true;
		this.viewLoading = true;
		this.disabledBtn = true;
		this.contractService.Createcontract(_item).subscribe(res => {
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
