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
import { LayoutUtilsService, MessageType, QueryParamsModel } from 'app/core/_base/crud';
import { DM_DonViService } from '../dm-don-vi-service/dm-don-vi.service';
//Models

import { DM_DonViModel, ListImageModel } from '../dm-don-vi-model/dm-don-vi.model';

import moment from 'moment';
import { TreeDonViDialogComponent } from '../../../components/tree-don-vi-dialog/tree-don-vi-dialog.component';
import { threadId } from 'worker_threads';
import { environment } from 'environments/environment';

@Component({
	selector: 'kt-dm-don-vi-edit',
	templateUrl: './dm-don-vi-edit.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
})

export class DM_DonViEditComponent implements OnInit, OnDestroy {
	// Public properties
	DM_DonVi: DM_DonViModel;
	DM_DonViForm: FormGroup;
	hasFormErrors: boolean = false;
	disabledBtn: boolean = false;
	loadingSubject = new BehaviorSubject<boolean>(true);
	loading$: Observable<boolean>;
	viewLoading: boolean = false;
	isChange: boolean = false;
	fixedPoint = 0;
	isZoomSize: boolean = false;
	lst_DanhMucDV: any;
	parentDV: number = 0;
	parentName: string = "";
	isShowImage: boolean = false;
	private componentSubscriptions: Subscription;
	// imageLogo:any;
	picLogo: ListImageModel[] = [];
	imgvlLogo: any;
	constructor(
		public dialogRef: MatDialogRef<DM_DonViEditComponent>,
		@Inject(MAT_DIALOG_DATA) public data: any,
		private DM_DonViFB: FormBuilder,
		public dialog: MatDialog,
		private layoutUtilsService: LayoutUtilsService,
		private changeDetectorRefs: ChangeDetectorRef,
		private dm_donvisService: DM_DonViService) { }

	/**
	 * On init
	 */
	async ngOnInit() {
		// this.imageLogo = {
		// 	RowID: "",
		// 	Title: "",
		// 	Description: "",
		// 	Required: false,
		// 	Files: []
		// }
		this.viewLoading = true;
		this.DM_DonVi = new DM_DonViModel();
		this.DM_DonVi.clear();
		this.createForm();
		if (this.data.DM_DonVi && this.data.DM_DonVi.Id > 0) {
			this.dm_donvisService.getDM_DonViById(this.data.DM_DonVi.Id).subscribe(res => {
				this.viewLoading = false;
				if (res.status == 1 && res.data) {
					this.DM_DonVi = res.data;
					this.parentDV = this.DM_DonVi.Parent ? this.DM_DonVi.Parent : 0;
					//this.parentName=this.DM_DonVi.ParentName?this.DM_DonVi.ParentName:"";
					//console.log("don vi edit", this.DM_DonVi);
					if (this.DM_DonVi.Logo) {		
						let tmpElement = new ListImageModel();
						tmpElement.clear();
						tmpElement.src = this.DM_DonVi.Logo,
						tmpElement.type="image/png",
						this.picLogo.push(tmpElement);
					}
					this.createForm();
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
		this.DM_DonViForm = this.DM_DonViFB.group({
			donVi: [this.DM_DonVi.DonVi == null ? '' : this.DM_DonVi.DonVi, Validators.required],
			maDonvi: [this.DM_DonVi.MaDonvi == null ? '' : this.DM_DonVi.MaDonvi, Validators.required],
			maDinhDanh: [this.DM_DonVi.MaDinhDanh ?  this.DM_DonVi.MaDinhDanh: ''],
			parentName: [this.DM_DonVi.ParentName == null ? '' : this.DM_DonVi.ParentName],
			loaiDonVi: [this.DM_DonVi.LoaiDonVi == 0 ? '' : this.DM_DonVi.LoaiDonVi, Validators.required],
			sDT: [this.DM_DonVi.SDT, [Validators.pattern(/^([0-9|,]*)$/), Validators.minLength(9), Validators.maxLength(12)]],
			email: [this.DM_DonVi.Email == null ? '' : this.DM_DonVi.Email, [Validators.pattern(/^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/)]],
			diaChi: [this.DM_DonVi.DiaChi == null ? '' : this.DM_DonVi.DiaChi],
			logo: [this.DM_DonVi.Logo == null ? '' : this.DM_DonVi.Logo],
			priority: [this.DM_DonVi.Priority ? this.DM_DonVi.Priority : 1, [Validators.required, Validators.min(1)]],
			locked: [this.DM_DonVi.Locked?[this.DM_DonVi.Locked]: false],
			dangKyLichLanhDao: [this.DM_DonVi.DangKyLichLanhDao ? true : false],
			khongCoVanThu: [this.DM_DonVi.KhongCoVanThu ? true : false],
			imageLogo: [this.picLogo],
		});
	}
	/**
	 * Returns page title
	 */
	getTitle(): string {


		if (this.DM_DonVi.Id == 0) {
			return 'Thêm mới đơn vị';
		}
		if(this.data.DM_DonVi.IsShow){
			return `Xem đơn vị - ${this.DM_DonVi.DonVi} `;
		}

		return `Chỉnh sửa đơn vị - ${this.DM_DonVi.DonVi}(${this.DM_DonVi.MaDonvi}) `;
	}

	/**
	 * Check control is invalid
	 * @param controlName: string
	 */
	isControlInvalid(controlName: string): boolean {
		const control = this.DM_DonViForm.controls[controlName];
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
		const controls = this.DM_DonViForm.controls;
		/** check form */
		if (this.DM_DonViForm.invalid) {
			Object.keys(controls).forEach(controlName =>
				controls[controlName].markAsTouched()
			);
			let invalid = <FormControl[]>Object.keys(this.DM_DonViForm.controls).map(key => this.DM_DonViForm.controls[key]).filter(ctl => ctl.invalid);
			let invalidElem: any = invalid[0];
			invalidElem.nativeElement.focus();
			this.hasFormErrors = true;
			return;
		}
		//if(this.S)
		this.disabledBtn = true;
		// tslint:disable-next-line:prefer-const
		let editedDM_DonVi = this.prepareDM_DonVis();
		//console.log("don vi", editedDM_DonVi);
		if (this.DM_DonVi.Id > 0) {
			this.updateDM_DonVi(editedDM_DonVi)
			return;
		}

		this.addDM_DonVi(editedDM_DonVi, type);
	}

	/**
	 * Returns object for saving
	 */
	prepareDM_DonVis(): DM_DonViModel {
		const controls = this.DM_DonViForm.controls;
		const _DM_DonVi = new DM_DonViModel();
		_DM_DonVi.clear();
		_DM_DonVi.DonVi = controls['donVi'].value;

		_DM_DonVi.MaDonvi = controls['maDonvi'].value;

		_DM_DonVi.MaDinhDanh = controls['maDinhDanh'].value;
		_DM_DonVi.LoaiDonVi = controls['loaiDonVi'].value ? Number(controls['loaiDonVi'].value) : 0;
		_DM_DonVi.Parent = this.parentDV;

		_DM_DonVi.SDT = controls['sDT'].value;

		_DM_DonVi.Email = controls['email'].value;

		_DM_DonVi.DiaChi = controls['diaChi'].value;

		_DM_DonVi.Logo = controls['logo'].value;

		_DM_DonVi.Priority = controls['priority'].value ? Number(controls['priority'].value) :1;

		// _DM_DonVi.Locked = controls['locked'].value ;

		_DM_DonVi.DangKyLichLanhDao = controls['dangKyLichLanhDao'].value?controls['dangKyLichLanhDao'].value:false;

		_DM_DonVi.KhongCoVanThu = controls['khongCoVanThu'].value?controls['khongCoVanThu'].value:false;
		_DM_DonVi.listLinkImage = [];	
		this.imgvlLogo = controls['imageLogo'].value;
		if (this.imgvlLogo.length > 0) {
			for (let i = 0; i < this.imgvlLogo.length; i++) {
				const md = new ListImageModel();
				md.strBase64 = this.imgvlLogo[i].strBase64;
				md.filename = this.imgvlLogo[i].filename;
				md.src = this.imgvlLogo[i].src;
				md.IsAdd = this.imgvlLogo[i].IsAdd;
				md.IsDel = this.imgvlLogo[i].IsDel;
				md.IsImagePresent = true;
				_DM_DonVi.listLinkImage.push(md);
			}
		}
			//gán lại giá trị id 

			if (this.DM_DonVi.Id > 0) {
				_DM_DonVi.Id = this.DM_DonVi.Id;
				_DM_DonVi.Locked = controls['locked'].value ;
			}

			return _DM_DonVi;

	}
	/**
	 * Add item
	 *
	 * @param _DM_DonVi: DM_DonViModel
	 * @param withBack: boolean
	 */
	addDM_DonVi(_DM_DonVi: DM_DonViModel, withBack: boolean = false) {
		this.dm_donvisService.createDM_DonVi(_DM_DonVi).subscribe(res => {
			if (res.status == 1) {
				this.isChange = true;
				const message = `Thêm thành công`;
				this.layoutUtilsService.showInfo(message);
				this.DM_DonViForm.reset();
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
	openTreeDonVi() {
		const dialogRef = this.dialog.open(TreeDonViDialogComponent, { data: {} });
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}
			// console.log("res dv", res);
			this.parentDV = res.data.IdGroup;
			this.DM_DonViForm.controls["parentName"].setValue(res.text);
			// this.parentName = res.text;
			this.changeDetectorRefs.detectChanges();
		});
	}
	clearCapTren() {
		this.parentDV = 0;
		this.DM_DonViForm.controls["parentName"].setValue("");
		this.changeDetectorRefs.detectChanges();
	}
	/**
	 * Update item
	 *
	 * @param _DM_DonVi: DM_DonVisModel
	 * @param withBack: boolean
	 */
	updateDM_DonVi(_DM_DonVi: DM_DonViModel, withBack: boolean = false) {

		this.dm_donvisService.updateDM_DonVi(_DM_DonVi).subscribe(res => {
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

	numberOnly(event): boolean {
		const charCode = (event.which) ? event.which : event.keyCode;
		if (charCode > 31 && (charCode < 48 || charCode > 57)) {
			return false;
		}
		return true;
	}

	closeDialog() {
		this.dialogRef.close(this.isChange);
	}

	ValidateChangeNumberEvent(event: any) {
		
		if (event.target.value == null || event.target.value == '') {
			const message = 'Không thể để trống dữ liệu';
			this.layoutUtilsService.showError(message);
			return false;
		}
		var count = 0;
		for (let i = 0; i < event.target.value.length; i++) {
			if (event.target.value[i] == '.') {
				count += 1;
			}
		}
		var regex = /[a-zA-Z -!$%^&*()_+|~=`{}[:;<>?@#\]]/g;
		var found = event.target.value.match(regex);
		if (found != null) {
			const message = 'Dữ liệu không gồm chữ hoặc kí tự đặc biệt';
			this.layoutUtilsService.showError(message);
			return false;;
		}
		if (count >= 2) {
			const message = 'Dữ liệu không thể có nhiều hơn 2 dấu .';
			this.layoutUtilsService.showError(message);
			return false;;
		}
		return true;
	}

	formatNumber(item: string) {
		if (item == '' || item == null || item == undefined) return '';
		return Number(Math.round(parseFloat(item + 'e' + this.fixedPoint)) + 'e-' + this.fixedPoint).toFixed(this.fixedPoint)
	}

	f_currency(value: string): any {
		if (value == null || value == undefined || value == '') value = '0.00';
		let nbr = Number((value.substring(0, value.length - (this.fixedPoint + 1)) + '').replace(/,/g, ""));
		return (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,") + value.substr(value.length - (this.fixedPoint + 1), (this.fixedPoint + 1));
	}

	f_currency_V2(value: string): any {
		if (value == '-1') return '';
		if (value == null || value == undefined || value == '') value = '0';
		let nbr = Number((value + '').replace(/,/g, ""));
		return (nbr + '').replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
	}

	changeValueOfForm(controlName: string, event: any) {
		if (this.ValidateChangeNumberEvent(event)) {
			let tmpValue = this.DM_DonViForm.controls[controlName].value.replace(/,/g, "");
			this.DM_DonViForm.controls[controlName].setValue(

				this.f_currency_V2(tmpValue)

			);
		}
		else {
			this.DM_DonViForm.controls[controlName].setValue(event.target.value);
		}
	}


	f_convertDateTime(date: string) {
		var componentsDateTime = date.split("/");
		var date = componentsDateTime[0];
		var month = componentsDateTime[1];
		var year = componentsDateTime[2];
		var formatConvert = year + "-" + month + "-" + date + "T00:00:00.0000000";
		return new Date(formatConvert);
	}
	f_convertDate(p_Val: any) {
		let a = p_Val === "" ? new Date() : new Date(p_Val);
		return a.getFullYear() + "/" + ("0" + (a.getMonth() + 1)).slice(-2) + "/" + ("0" + (a.getDate())).slice(-2);
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
	selectFile() {
		let f = document.getElementById("FileUpLoad");
		f.click();
	}
	DeleteFile() {
		let f = document.getElementById("img_icon");
		f.setAttribute("src", "");
		let a = document.getElementById("img_icon");

		a["type"] = "text";
		a["type"] = "file";

		this.DM_DonViForm.controls['strBase64'].setValue('');
		this.DM_DonViForm.controls['isnew'].setValue(true);

		if (this.DM_DonViForm.controls['strBase64'].value != '') {
			this.isShowImage = true;
		}
		else {
			this.isShowImage = false;
		}
		this.changeDetectorRefs.detectChanges();
	}

	FileChoose(evt: any) {
		if (evt.target.files && evt.target.files.length) {//Nếu có file
			let file = evt.target.files[0]; // Ví dụ chỉ lấy file đầu tiên
			let size = file.size;
			if (size >= environment.DungLuong) {
				const message = `Không thể upload hình dung lượng cao hơn 3MB.`;
				this.layoutUtilsService.showError(message);
				return;
			}
			let reader = new FileReader();
			reader.readAsDataURL(evt.target.files[0]);
			let base64Str;
			let extension
			reader.onload = function () {
				base64Str = reader.result as String;
				extension = base64Str.match(/[^:/]\w+(?=;|,)/)[0];
				var metaIdx = base64Str.indexOf(';base64,');
				base64Str = base64Str.substr(metaIdx + 8); // Cắt meta data khỏi chuỗi base64
			};
			setTimeout(res => {
				this.DM_DonViForm.controls['strBase64'].setValue(base64Str);
				this.DM_DonViForm.controls['filename'].setValue(`${evt.target.files[0].name}`);
				this.DM_DonViForm.controls['extension'].setValue(extension);
				this.DM_DonViForm.controls['isnew'].setValue(true);
				if (this.DM_DonViForm.controls['strBase64'].value != '') {
					this.isShowImage = true;
				}
				else {
					this.isShowImage = false;
				}
				this.changeDetectorRefs.detectChanges();

				let f = document.getElementById("img_icon");
				f.setAttribute("src", `data:image/${extension};base64,` + base64Str);
			}, 1000);
		}
	}
}
