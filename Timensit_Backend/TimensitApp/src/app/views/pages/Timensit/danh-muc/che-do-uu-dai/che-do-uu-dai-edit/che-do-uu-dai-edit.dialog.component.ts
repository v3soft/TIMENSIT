import { Component, OnInit, Inject, ChangeDetectionStrategy, HostListener, ViewChild, ElementRef, ChangeDetectorRef } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { chedouudaiModel } from '../../che-do-uu-dai/Model/che-do-uu-dai.model';
import { AngularEditorConfig } from '@kolkov/angular-editor';
import { TranslateService } from '@ngx-translate/core';
import { chedouudaiService } from '../Services/che-do-uu-dai.service';
import { CommonService } from '../../../services/common.service';
import { LayoutUtilsService } from '../../../../../../core/_base/crud';
@Component({
	selector: 'm-che-do-uu-dai-edit-dialog',
	templateUrl: './che-do-uu-dai-edit.dialog.component.html',
})
export class chedouudaiEditDialogComponent implements OnInit {
	item: chedouudaiModel;
	oldItem: chedouudaiModel;
	itemForm: FormGroup;
	hasFormErrors: boolean = false;
	viewLoading: boolean = false;
	filterDonVi: string = '';
	loadingAfterSubmit: boolean = false;
	listchucdanh: any[] = [];
	disabledBtn = false;
	allowEdit = false;
	isZoomSize: boolean = false;
	change: boolean = false;

	@ViewChild("focusInput", { static: true }) focusInput: ElementRef;
	_name = "";
	constructor(public dialogRef: MatDialogRef<chedouudaiEditDialogComponent>,
		@Inject(MAT_DIALOG_DATA) public data: any,
		private fb: FormBuilder,
		private danhMucService: CommonService,
		public chedouudaiService: chedouudaiService,
		private changeDetectorRefs: ChangeDetectorRef,
		private layoutUtilsService: LayoutUtilsService,
		private translate: TranslateService) {
		this._name = this.translate.instant("CHE_DO_UU_DAI.NAME");

	}

	/** LOAD DATA */
	ngOnInit() {
		this.item = this.data._item; //lấy data _item từ list

		this.allowEdit = this.data.allowEdit; //false là xem chi tiết
		
		this.createForm();
        if (this.item.Id > 0) { //đang sửa hoặc xem
			this.viewLoading = true;
			
			this.chedouudaiService.getItem(this.item.Id).subscribe(res => {
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
			CheDoUuDai: [this.item.CheDoUuDai, Validators.required],
			MoTa: [this.item.MoTa],
            Locked: ['' + this.item.Locked],
            Priority: ['' + this.item.Priority, Validators.min(1)]
        });
        
		this.itemForm.controls["CheDoUuDai"].markAsTouched();
        this.itemForm.controls["MoTa"];
        this.itemForm.controls["Locked"];
        this.itemForm.controls["Priority"];
        this.focusInput.nativeElement.focus();

		if (!this.allowEdit) //false thì không cho sửa
			this.itemForm.disable();
	}

	/** UI */
	getTitle(): string {
		let result = this.translate.instant('CHE_DO_UU_DAI.ADD');
		if (!this.item || !this.item.Id) {
			return result;
		}
		if(!this.allowEdit) { 
			result = this.translate.instant('CHE_DO_UU_DAI.DETAIL') + ` - ${this.item.CheDoUuDai}`;
			return result;
		}
		result = this.translate.instant('CHE_DO_UU_DAI.UPDATE') + ` - ${this.item.CheDoUuDai}`;
		return result;
	}

	/** ACTIONS */
	prepareCustomer(): chedouudaiModel {
		const controls = this.itemForm.controls;
		const _item = new chedouudaiModel();
		_item.Id = this.item.Id;
		_item.CheDoUuDai = controls['CheDoUuDai'].value; 
		_item.MoTa = controls['MoTa'].value;
        _item.Locked = controls['Locked'].value;
		_item.Priority = controls['Priority'].value;

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
		const EditNhomLeTet = this.prepareCustomer();
		if (EditNhomLeTet.Id > 0) {
			this.UpdateNhom(EditNhomLeTet, withBack);
		} else {
			this.CreateNhom(EditNhomLeTet, withBack);
		}
	}

	UpdateNhom(_item: chedouudaiModel, withBack: boolean) {

		this.loadingAfterSubmit = true;
		this.viewLoading = true;

		this.disabledBtn = true;
		this.chedouudaiService.updateCheDo(_item).subscribe(res => {
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

	CreateNhom(_item: chedouudaiModel, withBack: boolean) {
		this.loadingAfterSubmit = true;
		this.viewLoading = true;
		this.disabledBtn = true;
		this.chedouudaiService.createCheDo(_item).subscribe(res => {
			this.disabledBtn = false;
			this.changeDetectorRefs.detectChanges();
			if (res && res.status === 1) {
				if (withBack == true) {
					this.dialogRef.close({
						_item
					});
				}
				else {
					this.change=true;
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
		this.dialogRef.close(this.change);
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
