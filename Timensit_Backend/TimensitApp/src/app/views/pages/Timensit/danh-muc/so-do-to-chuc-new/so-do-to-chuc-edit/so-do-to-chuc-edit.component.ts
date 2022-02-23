import { Component, OnInit, Inject, ChangeDetectionStrategy, HostListener, ViewChild, ChangeDetectorRef, ElementRef } from '@angular/core';
import { MatDialog, MatSelect } from '@angular/material';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Observable, forkJoin, from, of, BehaviorSubject } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { AngularEditorConfig } from '@kolkov/angular-editor';
import { chucvuEditDialogComponent } from '../../chucvu/chucvu-edit/chucvu-edit.dialog.component';
import { chucvuModel } from '../../chucvu/Model/chucvu.model';
import { UpdateThongTinChucVuModel } from '../Model/so-do-to-chuc.model';
import { OrgChartService } from '../Services/so-do-to-chuc.service';
import { environment } from '../../../../../../../environments/environment';
import { CommonService } from '../../../services/common.service';
import { LayoutUtilsService } from '../../../../../../core/_base/crud';
import { TokenStorage } from '../../../../../../core/auth/_services/token-storage.service';
@Component({
	selector: 'm-so-do-to-chuc-edit',
	templateUrl: './so-do-to-chuc-edit.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush
})
export class sodotochuceditComponent implements OnInit {
	item: UpdateThongTinChucVuModel;

	oldItem: UpdateThongTinChucVuModel;
	selectedTab: number = 0;
	loadingSubject = new BehaviorSubject<boolean>(false);
	loadingControl = new BehaviorSubject<boolean>(false);
	loading$ = this.loadingSubject.asObservable();
	itemForm: FormGroup;
	viewLoading: boolean = false;
	hasFormErrors: boolean = false;
	loadingAfterSubmit: boolean = false;
	listDonVi: any[] = [];
	listPhongBan: any[] = [];
	listchucdanh: any[] = [];
	listchucvu: any[] = [];
	listcapquanly: any[] = [];
	// Filter fields
	filterDonVi: string = '';
	filterPhongBan: string = '';
	filterChucDanh: string = '';
	filterChucVu: string = '';
	ID_NV: string = '';
	id_hd: number = 0;
	vitriMax: number = 0;
	ShowButton: boolean = false;
	ShowKhiEdit: boolean = false;
	Show_TenChucVu: boolean = false;
	isshowtextbox: boolean = false;
	tenchucdanh: string;
	tentienganh: string;
	id_dv: string = '';
	id_pb: string = '';
	id_cd: number = 0;
	// _id_nv: number;
	editorConfig: AngularEditorConfig = {
		editable: true,
		spellcheck: true,
		height: '15rem',
		minHeight: '5rem',
		placeholder: 'Nhập vào nội dung',
		translate: 'no',
		uploadUrl: environment.ApiRoot + 'api/tien-ich/upload-img?token=', // if needed //+ JSON.parse(localStorage.getItem(SystemConstants.CURRENT_USER)).token
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

	@ViewChild("focusInput", { static: true }) focusInput: ElementRef;
	disabledBtn: boolean = false;
	public datatree: BehaviorSubject<any[]> = new BehaviorSubject([]);
	title: string = '';
	selectedNode: BehaviorSubject<any> = new BehaviorSubject([]);
	ID_Struct: string = '';
	Id_parent: string = '';
	allowEdit: boolean = true;
	isZoomSize: boolean = false;
	_name = "";
	constructor(public dialogRef: MatDialogRef<sodotochuceditComponent>,
		@Inject(MAT_DIALOG_DATA) public data: any,
		private activatedRoute: ActivatedRoute,
		private danhMucService: CommonService,
		private fb: FormBuilder,
		private router: Router,
		private NewInfoJobTitleService: OrgChartService,
		private itemFB: FormBuilder,
		public dialog: MatDialog,
		private layoutUtilsService: LayoutUtilsService,
		private changeDetectorRefs: ChangeDetectorRef,
		private translate: TranslateService,
		private tokenStorage: TokenStorage) {
		this._name = this.translate.instant("SO_DO_TO_CHUC.NAME");
	}
	ngOnInit() {
		this.oldItem = new UpdateThongTinChucVuModel();
		this.oldItem.clear();
		this.reset();
		this.title = this.translate.instant("CO_CAU_TO_CHUC.NAME") + ' *';
		this.id_cd = this.data.id_cd;
		this.Id_parent = this.data.Id_parent;
		this.vitriMax = this.data.vitriMax;
		if (this.data.allowEdit != undefined)
			this.allowEdit = this.data.allowEdit;
		if (this.id_cd > 0) {
			var _newmodel = new UpdateThongTinChucVuModel();
			_newmodel.clear();
			this.item = _newmodel;
			this.NewInfoJobTitleService.SelectedNodeChanged(+this.id_cd).subscribe(res => {
				this.item = res.data;
				this.oldItem = Object.assign({}, res);
				this.loadthongtin();
				this.getTreeValue();
				this.initLoad();
				this.itemForm;

				this.GetValueNode({ id: this.item.StructureID });

			});
			this.viewLoading = true;
		}
		else {
			this.viewLoading = false;
			this.danhMucService.GetListPosition().subscribe(res => {
				this.listchucdanh = res.data;
			});
			this.danhMucService.getCapQuanLy().subscribe(res => {
				this.listcapquanly = res.data;
			});
			this.itemForm.controls["ViTri"].setValue(this.data.vitriMax);
		}
		// setTimeout(function () { document.getElementById('id').focus(); }, 100);
		this.getTreeValue();
		this.initLoad();
		// this.loadthongtin();
		this.changeDetectorRefs.detectChanges();
	}
	createForm() {
		this.itemForm = this.itemFB.group({
			SoNhanVien: [this.item.SoNhanVien, [Validators.required]],
			ViTri: [this.item.ViTri, [Validators.required]],
			TenChucVu: [this.item.TenChucVu, [Validators.required]],
			ID_ChucVu: [this.item.ID_ChucVu, [Validators.required]],
			ID_ChucDanh: [this.item.ID_ChucDanh, [Validators.required]],
			ID_Cap: [this.item.ID_Cap],
			HienThiDonVi: [this.item.HienThiDonVi],
			DungChuyenCap: [this.item.DungChuyenCap],
			HienThiPhongBan: [this.item.HienThiPhongBan],
			StuctItem: [this.item.StructureID, Validators.required],
			HienThiID: [this.item.HienThiID],
		});
		this.itemForm.controls["SoNhanVien"].markAsTouched();
		this.itemForm.controls["ViTri"].markAsTouched();
		this.itemForm.controls["ID_ChucDanh"].markAsTouched();
		this.itemForm.controls["ID_ChucVu"].markAsTouched();
		this.itemForm.controls["TenChucVu"].markAsTouched();
		this.itemForm.controls["StuctItem"].markAsTouched();
		this.changeDetectorRefs.detectChanges();
		if (!this.allowEdit)
			this.itemForm.disable();

	}
	getTreeValue() {
		this.danhMucService.Get_CoCauToChuc().subscribe(res => {
			if (res.data && res.data.length > 0) {
				this.datatree.next(res.data);
				this.selectedNode.next({
					RowID: "" + this.item.StructureID,
				});
				if ("" + this.item.StructureID != undefined)
					this.ID_Struct = '' + this.item.StructureID;
				else
					this.ID_Struct = '';
				// this.loadListChucVu();
			}
		});
	}
	GetValueNode(val: any) {
		this.ID_Struct = val.id;
		this.danhMucService.GetListPositionbyStructure(this.ID_Struct).subscribe(res => {
			//this.listChucDanh = res.data;
			if (res.data.length > 0) {
				// this.id_cd = '' + res.data[0].ID;
				// this.itemForm.controls['chucDanh'].setValue(this.id_cd);
				this.danhMucService.GetListJobtitleByStructure(this.id_cd, this.ID_Struct).subscribe(res => {
					//this.listChucVu = res.data;
					if (res.data.length > 0) {
						this.itemForm.controls['chucVu'].setValue('' + res.data[0].ID);
					}
					this.changeDetectorRefs.detectChanges();
				});
			} else {
				// this.itemForm.controls['chucDanh'].setValue('');
				// this.itemForm.controls['chucVu'].setValue('');
			}
		});
	}
	loadthongtin() {
		this.danhMucService.GetListPosition().subscribe(res => {
			this.listchucdanh = res.data;
			this.danhMucService.GetListNhomChucDanhTheoChucDanh(this.item.ID_ChucDanh).subscribe(res => {
				this.listchucvu = res.data;
				if (this.listchucvu && this.listchucvu.length > 0) {
					// this.filterChucVu = this.listchucvu[0].id_row;
					// this.itemForm.controls["ID_ChucVu"].setValue('' + this.listchucvu[0].id_row);
					// this.loadTextJobTitle();
					this.changeDetectorRefs.detectChanges();
				}
			});
		});
		this.danhMucService.getCapQuanLy().subscribe(res => {
			this.listcapquanly = res.data;
		});
	}

	reset() {
		this.item = Object.assign({}, this.oldItem);
		this.createForm();
		this.hasFormErrors = false;
		this.itemForm.markAsPristine();
		this.itemForm.markAsUntouched();
		this.itemForm.updateValueAndValidity();
	}
	checkValue(e: any, vi: any) {
		if (!((e.keyCode > 95 && e.keyCode < 106)
			|| (e.keyCode > 45 && e.keyCode < 58)
			|| e.keyCode == 8)) {
			e.preventDefault();
		}
	};
	onSubmit(withBack: boolean = false) {

		this.itemForm.controls["StuctItem"].setValue(this.ID_Struct);
		this.hasFormErrors = false;
		const controls = this.itemForm.controls;
		/** check form */
		if (this.itemForm.invalid) {
			Object.keys(controls).forEach(controlName =>
				controls[controlName].markAsTouched()
			);
			this.hasFormErrors = true;
			this.selectedTab = 0;
			return;
		}
		let _update = this.PrepareUpdateInfoJobtitle();

		if (this.id_cd > 0) {
			this.Update(_update, withBack);
		}
		else
			this.AddItem(_update, withBack);

	}
	ThemMoiChucVu() {
		let _item = new chucvuModel;
		_item.Id_row = 0;
		_item.Id_CV = '';
		_item.Tenchucdanh = '';
		_item.Tentienganh = '';
		let saveMessageTranslateParam = '';
		saveMessageTranslateParam += _item.Id_row > 0 ? 'OBJECT.EDIT.UPDATE_MESSAGE' : 'OBJECT.EDIT.ADD_MESSAGE';
		const _saveMessage = this.translate.instant(saveMessageTranslateParam, { name: this._name });
		const dialogRef = this.dialog.open(chucvuEditDialogComponent, { data: { _item } });
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				this.ngOnInit();
				return;
			}

			this.layoutUtilsService.showInfo(_saveMessage);
			if (this.item.ID_ChucDanh != undefined || this.item.ID_ChucDanh == '')
				this.filterChucDanh = this.item.ID_ChucDanh;
			// this.danhMucService.GetListNhomChucDanhTheoChucDanh(this.filterChucDanh).subscribe(res => {
			// 	this.listchucvu = res.data;
			// });
			this.ngOnInit();
			this.changeDetectorRefs.detectChanges();
		});
	}
	getTitle(): string {

		let result = this.translate.instant('COMMON.CREATE') + ' <' + this.data.chucdanhParent + '>';
		if (this.data.id_cd == 0) {
			this.ShowButton = true;
			return result;
		}
		this.ShowButton = false;

		result = this.translate.instant('COMMON.UPDATE') + ': ' + this.item.TenChucVu + '';
		return result;
	}
	loadPhongBan() {
		//this.danhMucService.GetListDepartmentbyBranch(this.filterDonVi).subscribe(res => {

		//	this.listPhongBan = res.data;
		//	// this.filterPhongBan = this.listPhongBan[0].IDPhongBan;
		//	this.itemForm.controls["ID_PhongBan"].setValue('' + this.listPhongBan[0].IDPhongBan);
		//	this.changeDetectorRefs.detectChanges();
		//	// this.filterPhongBan = '' + this.listPhongBan[0].ID_PhongBan;
		//});
	}
	loadChucVuTheoNhomChucDanh() {
		this.danhMucService.GetListNhomChucDanhTheoChucDanh(this.filterChucDanh).subscribe(res => {
			this.listchucvu = res.data;
			if (this.listchucvu && this.listchucvu.length > 0) {
				this.filterChucVu = this.listchucvu[0].id_row;
				this.itemForm.controls["ID_ChucVu"].setValue('' + this.listchucvu[0].id_row);
				// this.loadTextJobTitle();
				this.loadTextJobTitle('' + this.listchucvu[0].id_row);
				this.changeDetectorRefs.detectChanges();
			}
			else {
				this.filterChucVu = '';
				// this.loadTextJobTitle();
				this.itemForm.controls["ID_ChucVu"].setValue('');
				this.itemForm.controls['TenChucVu'].setValue('');
				// this.itemForm.controls['TenTiengAnh'].setValue('');
				this.changeDetectorRefs.detectChanges();
			}
		});
	}
	loadTextJobTitle(id: string) {
		this.danhMucService.GetListOnlyNhomChucDanh(id).subscribe(res => {
			let tenchucdanh = [];
			let tentienganh = [];
			tenchucdanh = res.data[0].tenchucdanh;
			tentienganh = res.data[0].tentienganh;
			this.itemForm.controls['TenChucVu'].setValue(tenchucdanh);
		});
	}
	PrepareUpdateInfoJobtitle(): UpdateThongTinChucVuModel {
		const controls = this.itemForm.controls;
		const update = new UpdateThongTinChucVuModel();
		update.TenChucVu = controls['TenChucVu'].value;
		update.ID_ChucDanh = controls['ID_ChucDanh'].value;
		update.ID_ChucVu = controls['ID_ChucVu'].value;
		update.ID_Cap = +controls['ID_Cap'].value;
		update.HienThiDonVi = controls['HienThiDonVi'].value;
		update.DungChuyenCap = controls['DungChuyenCap'].value;
		update.ViTri = controls['ViTri'].value;
		update.SoNhanVien = controls['SoNhanVien'].value;
		update.StructureID = controls['StuctItem'].value;
		if (this.data.id_cd == 0)
			update.ID_Parent = this.data.Id_parent;
		else
			update.ID = this.data.id_cd;
		update.HienThiID = controls['HienThiID'].value;
		return update;
	}
	AddItem(item: UpdateThongTinChucVuModel, withBack: boolean) {

		this.loadingAfterSubmit = true;
		this.viewLoading = true;
		this.NewInfoJobTitleService.UpdateThongTinChucVu(item).subscribe(res => {
			if (res && res.status === 1) {
				if (withBack == true) {
					this.dialogRef.close({
						item
					});
				}
				else {

					this.ngOnInit();
					// document.getElementById('tieude').focus();
					this.itemForm.controls["StuctItem"].setValue(null);
					this.getTreeValue();
					this.itemForm.controls["ViTri"].setValue(item.ViTri + 1);
					const _messageType = this.translate.instant('OBJECT.EDIT.ADD_MESSAGE', { name: this._name });
					this.layoutUtilsService.showInfo(_messageType);
					this.changeDetectorRefs.detectChanges();

				}
			}
			else {
				this.viewLoading = false;
				this.layoutUtilsService.showError(res.error.message);
			}
		});
	}
	Update(_item: UpdateThongTinChucVuModel, withBack: boolean) {
		this.loadingAfterSubmit = true;
		this.viewLoading = true;
		this.disabledBtn = true;
		this.NewInfoJobTitleService.UpdateThongTinChucVu(_item).subscribe(res => {
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
					this.layoutUtilsService.showInfo(_messageType,).afterDismissed().subscribe(tt => {
					});
					// this.focusInput.nativeElement.focus();
				}
			}
			else {
				this.viewLoading = false;
				this.layoutUtilsService.showError(res.error.message);
			}
		});
	}
	close() {
		this.dialogRef.close();
	}
	goBack() {
		this.dialogRef.close();
	}
	initLoad() {
		this.createForm();
		this.loadingSubject.next(false);
		this.loadingControl.next(true);
	}
	onAlertClose($event) {
		this.hasFormErrors = false;
	}
	TestInputNumber(e: any, vi: any) {
		if (!((e.keyCode > 95 && e.keyCode < 106)
			|| (e.keyCode > 47 && e.keyCode < 58)
			|| e.keyCode == 8)) {
			e.preventDefault();
		}
	}
	f_number(value: any) {
		return Number((value + '').replace(/,/g, ""));
	}

	f_currency(value: any, args?: any): any {
		let nbr = Number((value + '').replace(/,|-/g, ""));
		return (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
	}
	f_convertDate(v: any) {
		if (v != "") {
			let a = new Date(v);
			return a.getFullYear() + "-" + ("0" + (a.getMonth() + 1)).slice(-2) + "-" + ("0" + (a.getDate())).slice(-2) + "T00:00:00.0000000";
		}
	}
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
	text(e: any, vi: any) {
		if (!((e.keyCode > 95 && e.keyCode < 106)
			|| (e.keyCode > 45 && e.keyCode < 58)
			|| e.keyCode == 8)) {
			e.preventDefault();
		}
	}
}
