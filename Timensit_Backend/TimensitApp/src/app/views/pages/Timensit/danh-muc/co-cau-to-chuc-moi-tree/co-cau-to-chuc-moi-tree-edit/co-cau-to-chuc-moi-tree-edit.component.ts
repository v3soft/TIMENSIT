import { Component, OnInit, Inject, ChangeDetectionStrategy, HostListener, ViewChild, ElementRef, ChangeDetectorRef } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AngularEditorConfig } from '@kolkov/angular-editor';
import { TranslateService } from '@ngx-translate/core';
import { OrgStructureModel } from '../Model/CoCauToChuc.model';
import { cocautochucMoiTreeService } from '../Services/co-cau-to-chuc-moi-tree.service';
import { CommonService } from '../../../services/common.service';
import { LayoutUtilsService } from '../../../../../../core/_base/crud';
import { TokenStorage } from '../../../../../../core/auth/_services/token-storage.service';

// import { AssetsCategoryService } from '../Services/AssetsCategory.service';
@Component({
	selector: 'm-co-cau-to-chuc-moi-tree-edit',
	templateUrl: './co-cau-to-chuc-moi-tree-edit.component.html',
})
export class CoCauToChucEditComponent implements OnInit {
	filterprovinces: string = '';
	item: OrgStructureModel;
	oldItem: OrgStructureModel
	itemForm: FormGroup;
	hasFormErrors: boolean = false;
	viewLoading: boolean = false;
	loadingAfterSubmit: boolean = false;
	NoiDung: string;
	listorgstructure: any[] = [];
	listDV: any[] = [];
	disabledBtn: boolean = false;
	filtercalamvic: string = '';
	listCaLamViec: any[] = [];
	listCheDoLamViec: any[] = [];
	allowEdit: boolean = true;
	isZoomSize:boolean=false;
	@ViewChild("focusInput", { static: true }) focusInput: ElementRef;
	_name = "";
	ID_Goc_Pa: number = undefined;
	constructor(public dialogRef: MatDialogRef<CoCauToChucEditComponent>,
		@Inject(MAT_DIALOG_DATA) public data: any,
		private fb: FormBuilder,
		private itemFB: FormBuilder,
		private danhMucChungService: CommonService,
		private cocautochucMoiService: cocautochucMoiTreeService,
		private layoutUtilsService: LayoutUtilsService,
		private changeDetectorRefs: ChangeDetectorRef,
		private translate: TranslateService,
		private tokenStorage: TokenStorage) {
		this._name = this.translate.instant("CO_CAU_TO_CHUC.NAME");
	}

	/** LOAD DATA */
	ngOnInit() {
		this.tokenStorage.getUserInfo().subscribe(res => {
			this.filterprovinces = res.IdTinh;
		})
		this.reset();
		this.item = this.data._item;
		this.ID_Goc_Pa = this.data.ID_Goc;
		if (this.data.allowEdit != undefined)
			this.allowEdit = this.data.allowEdit;
		if (this.item.RowID > 0) {
			this.viewLoading = true;
		}
		else {
			this.viewLoading = false;
		}
		this.changeCap(this.item.Level);
		this.danhMucChungService.GetListOrganizationalChartStructure().subscribe(res => {
			this.listorgstructure = res.data;
		});
		this.danhMucChungService.GetListShift().subscribe(res => {
			this.listCheDoLamViec = res.data;
			// this.filtercalamvic = '' + res.data[0].ID_Row;
			this.changeDetectorRefs.detectChanges();
		});
		this.createForm();
		this.focusInput.nativeElement.focus();
	}
	changeCap(value) {
		this.listDV = [];
		if (value == 2)
			this.danhMucChungService.GetListDistrictByProvinces(this.filterprovinces).subscribe(res => {
				if (res && res.status == 1)
					this.listDV = res.data;
				this.changeDetectorRefs.detectChanges();
			});
		if (value == 3)
			this.danhMucChungService.GetListWardByDistrict(this.ID_Goc_Pa).subscribe(res => {
				if (res && res.status == 1)
					this.listDV = res.data;
				this.changeDetectorRefs.detectChanges();
			});
	}
	createForm() {
		this.itemForm = this.itemFB.group({
			Code: [this.item.Code, Validators.required],
			Title: [this.item.Title, Validators.required],
			id_ca: [this.item.WorkingModeID],
			Vitri: [this.item.Position, Validators.required],
			CapCoCau: [this.item.Level, Validators.required],
			idgoc: [+this.item.ID_Goc],
		});
		//this.itemForm.controls["Code"].markAsTouched();
		//this.itemForm.controls["Title"].markAsTouched();
		//this.itemForm.controls["Vitri"].markAsTouched();
		//this.itemForm.controls["CapCoCau"].markAsTouched();
		if (!this.allowEdit)
			this.itemForm.disable();
	}

	/** UI */
	getTitle(): string {
		let result = this.translate.instant('COMMON.CREATE');
		if (this.item.RowID <= 0) {
			return result;
		}
		result = this.translate.instant('COMMON.UPDATE') + `: ${this.item.Title}`;
		return result;
	}

	/** ACTIONS */
	prepareCustomer(): OrgStructureModel {
		const controls = this.itemForm.controls;
		const _item = new OrgStructureModel();
		_item.RowID = this.item.RowID;
		_item.ParentID = this.item.ParentID;
		_item.Title = controls['Title'].value;
		_item.WorkingModeID = +controls['id_ca'].value;
		_item.Code = controls['Code'].value;
		_item.Position = controls['Vitri'].value;
		_item.Level = controls['CapCoCau'].value;
		_item.ID_Goc = controls['idgoc'].value;
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
		if (+this.itemForm.controls["Vitri"].value <= 0) {
			const message = 'Vị trí phải lớn hơn 0';
			this.layoutUtilsService.showError(message);
			this.itemForm.controls["Vitri"].setValue("");
			return;
		}
		const capcocau = this.prepareCustomer();
		if (capcocau.RowID > 0) {
			this.Update(capcocau, withBack);
		} else {
			this.Create(capcocau, withBack);
		}
	}

	Update(_item: OrgStructureModel, withBack: boolean) {
		this.loadingAfterSubmit = true;
		this.viewLoading = true;
		this.disabledBtn = true;
		this.cocautochucMoiService.Updateorgstructure(_item).subscribe(res => {

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
					const _messageType = this.translate.instant('OBJECT.EDIT.UPDATE_MESSAGE', { name: this._name });
					this.layoutUtilsService.showInfo(_messageType).afterDismissed().subscribe(tt => {
					});
					this.focusInput.nativeElement.focus();
					this.ngOnInit();
				}
			}
			else {
				// this.viewLoading = false;
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
	Create(_item: OrgStructureModel, withBack: boolean) {
		this.loadingAfterSubmit = true;
		this.viewLoading = true;
		this.disabledBtn = true;
		this.cocautochucMoiService.Createorgstructure(_item).subscribe(res => {
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
					const _messageType = this.translate.instant('OBJECT.EDIT.ADD_MESSAGE', { name: this._name });
					this.layoutUtilsService.showInfo(_messageType).afterDismissed().subscribe(tt => {
					});
					// this.reset();
					this.focusInput.nativeElement.focus();
					this.changeDetectorRefs.detectChanges();
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
	text(e: any) {
		if (!((e.keyCode > 95 && e.keyCode < 106)
			|| (e.keyCode > 45 && e.keyCode < 58)
			|| e.keyCode == 8)) {
			e.preventDefault();
		}
	}
}
