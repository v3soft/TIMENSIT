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
import { NguoiDungDPSService } from '../nguoi-dung-dps-service/nguoi-dung-dps.service';
//Models

import { NguoiDungDPSModel } from '../nguoi-dung-dps-model/nguoi-dung-dps.model';

import moment from 'moment';
import { ConfirmPasswordValidator } from '../../../../auth/register/confirm-password.validator';
import { CommonService } from '../../../services/common.service';
import { ChonNhieuDonViComponent } from '../../../components/chon-nhieu-don-vi/chon-nhieu-don-vi.component';

@Component({
	selector: 'kt-nguoi-dung-dps-reset-password',
	templateUrl: './nguoi-dung-dps-reset-password.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
})

export class NguoiDungDPSResetPasswordComponent implements OnInit, OnDestroy {
	// Public properties
	NguoiDungDPS: NguoiDungDPSModel;
	NguoiDungDPSForm: FormGroup;
	hasFormErrors: boolean = false;
	loadingSubject = new BehaviorSubject<boolean>(true);
	loading$: Observable<boolean>;
	viewLoading: boolean = false;
	isChange: boolean = false;

	fixedPoint = 0;

	isZoomSize: boolean = false;
	listIdGroup: any[] = [];
	listNV: any[] = [];

	selectIdGroup: string = '0';
	private componentSubscriptions: Subscription;

	datatree: BehaviorSubject<any[]> = new BehaviorSubject([]);
	lstChucVu: any[];
	lstLoaiChungThu: any[];
	lstGioiTinh: any[];
	maxNS = moment(new Date()).add(-16, 'year').toDate();

	selectable: boolean = true;
	removable: boolean = true;
	allowEdit: boolean = true;

	avatar: any = { strBase64: '', filename: '' };
	sign: any = { strBase64: '', filename: '' };

	Strong_Pass: string = '';
	disabledBtn:boolean=false;
	constructor(
		public dialogRef: MatDialogRef<NguoiDungDPSResetPasswordComponent>,
		@Inject(MAT_DIALOG_DATA) public data: any,
		private NguoiDungDPSFB: FormBuilder,
		public dialog: MatDialog,
		private layoutUtilsService: LayoutUtilsService,
		private changeDetectorRefs: ChangeDetectorRef,
		private nguoidungdpssService: NguoiDungDPSService,
		private commonService: CommonService) { }

	/**
	 * On init
	 */
	async ngOnInit() {
		this.viewLoading = true;
		this.NguoiDungDPS = this.data.NguoiDungDPS;
		this.allowEdit = this.data.allowEdit;
		this.GetConfig();
		this.createForm();
		// this.ListIdGroup();
		if (this.data.NguoiDungDPS) {
			this.NguoiDungDPS = this.data.NguoiDungDPS;
		}
		if (this.data.NguoiDungDPS && this.data.NguoiDungDPS.UserID > 0) {
			this.nguoidungdpssService.getNguoiDungDPSById(this.data.NguoiDungDPS.UserID).subscribe(res => {
				this.viewLoading = false;
				if (res.status == 1 && res.data) {
					this.NguoiDungDPS = res.data;
					// this.GetValueNode({ id: res.data.IdDonvi });
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
	GetValueNode(item) {
		this.commonService.ListChucVu(item.id).subscribe(res => {
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

	GetConfig() {
		this.commonService.getConfig(['STRONG_PASS']).subscribe(res => {
			this.Strong_Pass = res.data.STRONG_PASS;
		});
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
		// let temp: any = {
		// 	Password: ['', [Validators.required, Validators.maxLength(100)]],
		// 	RePassword: ['', [Validators.required, Validators.maxLength(50)]],
		// 	// viettelStudy: [this.NguoiDungDPS.ViettelStudy, Validators.maxLength(100)],
		// 	// email: [this.NguoiDungDPS.Email, [Validators.email, Validators.maxLength(100)]],
		// 	// phoneNumber: [this.NguoiDungDPS.PhoneNumber, [Validators.pattern("[0][0-9]{9}"), Validators.maxLength(20)]],
		// 	// simCA: [this.NguoiDungDPS.SimCA, Validators.maxLength(100)],
		// 	// loaiChungThu: [this.NguoiDungDPS.LoaiChungThu == null ? '0' : this.NguoiDungDPS.LoaiChungThu + ''],
		// 	// serialToken: [this.NguoiDungDPS.SerialToken, Validators.maxLength(100)],
		// 	// donVi: [this.NguoiDungDPS.IdDonVi, Validators.required],
		// 	// chucVu: [this.NguoiDungDPS.IdChucVu == null ? '0' : this.NguoiDungDPS.IdChucVu + ''],
		// 	// maNV: [this.NguoiDungDPS.MaNV, Validators.maxLength(50)],
		// 	// gioiTinh: [this.NguoiDungDPS.GioiTinh == null ? "0" : this.NguoiDungDPS.GioiTinh + ''],
		// 	// nhanLichDonVi: [this.NguoiDungDPS.NhanLichDonVi],
		// 	// cmtnd: [this.NguoiDungDPS.CMTND, Validators.maxLength(50)],
		// 	// ngaySinh: [this.NguoiDungDPS.NgaySinh]
		// };

		let temp: any = {};
		if (this.Strong_Pass == '1') {
			temp.password = ['', [Validators.required, Validators.pattern(this.commonService.ValidateFormatRegex('password'))]];
		}
		else { 
			temp.password = ['', Validators.required];
		}
		temp.confirmPassword = ['', Validators.required];
		this.NguoiDungDPSForm = this.NguoiDungDPSFB.group(temp, {
			validator: ConfirmPasswordValidator.MatchPassword
		});
		if (!this.allowEdit)
			this.NguoiDungDPSForm.disable();
	}
	/**
	 * Returns page title
	 */
	getTitle(): string {
		// if (!this.allowEdit)
		// 	return 'Chi tiết người dùng';
		// if (!this.NguoiDungDPS.UserID) {
		// 	return 'Thêm mới người dùng';
		// }
		return `Đặt lại mật khẩu người dùng - ${this.NguoiDungDPS.UserName} `;
	}

	/**
	 * Check control is invalid
	 * @param controlName: string
	 */
	isControlInvalid(controlName: string): boolean {
		const control = this.NguoiDungDPSForm.controls[controlName];
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
		const controls = this.NguoiDungDPSForm.controls;
		/** check form */
		if (this.NguoiDungDPSForm.invalid) {
			Object.keys(controls).forEach(controlName =>
				controls[controlName].markAsTouched()
			);
			let invalid = <FormControl[]>Object.keys(this.NguoiDungDPSForm.controls).map(key => this.NguoiDungDPSForm.controls[key]).filter(ctl => ctl.invalid);
			let invalidElem: any = invalid[0];
			invalidElem.nativeElement.focus();
			this.hasFormErrors = true;
			return;
		}

		// tslint:disable-next-line:prefer-const
		let editedNguoiDungDPS = this.prepareNguoiDungDPSs();
		this.updateNguoiDungDPS(editedNguoiDungDPS)
		// if (editedNguoiDungDPS != null) {
		// 	if (this.NguoiDungDPS.UserID) {
		// 		this.updateNguoiDungDPS(editedNguoiDungDPS)
		// 		return;
		// 	}
		// 	this.addNguoiDungDPS(editedNguoiDungDPS, type);
		// }
	}

	/**
	 * Returns object for saving
	 */
	prepareNguoiDungDPSs(): any {
		
		const controls = this.NguoiDungDPSForm.controls;
		const _NguoiDungDPS: any = {};
		// _NguoiDungDPS.FullName = controls['fullname'].value;
		// _NguoiDungDPS.UserName = controls['userName'].value;
		// _NguoiDungDPS.ViettelStudy = controls['viettelStudy'].value;
		// _NguoiDungDPS.Email = controls['email'].value;
		// _NguoiDungDPS.PhoneNumber = controls['phoneNumber'].value;
		// _NguoiDungDPS.SimCA = controls['simCA'].value;
		// _NguoiDungDPS.LoaiChungThu = controls['loaiChungThu'].value;
		// _NguoiDungDPS.SerialToken = controls['serialToken'].value;
		// _NguoiDungDPS.IdDonVi = controls['donVi'].value;
		// _NguoiDungDPS.IdChucVu = controls['chucVu'].value;
		// _NguoiDungDPS.MaNV = controls['maNV'].value;
		// _NguoiDungDPS.GioiTinh = controls['gioiTinh'].value;
		// _NguoiDungDPS.NhanLichDonVi = controls['nhanLichDonVi'].value;
		// _NguoiDungDPS.CMTND = controls['cmtnd'].value;
		// _NguoiDungDPS.NgaySinh = this.commonService.f_convertDate(controls['ngaySinh'].value);
		// _NguoiDungDPS.DonViQuanTam = this.NguoiDungDPS.DonViQuanTam;
		// _NguoiDungDPS.DonViLayHanXuLy = this.NguoiDungDPS.DonViLayHanXuLy;
		// _NguoiDungDPS.avatar = this.avatar;
		// _NguoiDungDPS.Sign = this.sign;

		_NguoiDungDPS.NewPassword = controls['password'].value;
		_NguoiDungDPS.RePassword = controls['confirmPassword'].value;
		if (_NguoiDungDPS.NewPassword != _NguoiDungDPS.RePassword) {
			this.layoutUtilsService.showError("Xác nhận mật khẩu không đúng");
			return null;
		}

		//gán lại giá trị id 
		if (this.NguoiDungDPS.UserID) {
			_NguoiDungDPS.Id = this.NguoiDungDPS.UserID;
		}

		return _NguoiDungDPS;
	}
	/**
	 * Add item
	 *
	 * @param _NguoiDungDPS: NguoiDungDPSModel
	 * @param withBack: boolean
	 */
	addNguoiDungDPS(_NguoiDungDPS: NguoiDungDPSModel, withBack: boolean = false) {
		this.nguoidungdpssService.createNguoiDungDPS(_NguoiDungDPS).subscribe(res => {
			if (res.status == 1) {
				this.isChange = true;
				const message = `Thêm mới người dùng thành công`;
				this.layoutUtilsService.showInfo(message);
				this.NguoiDungDPSForm.reset();
				if (withBack)
					this.dialogRef.close(this.isChange);
			}
			else {
				this.layoutUtilsService.showError(res.error.message);
			}
		});
	}

	/**
	 * Update item
	 *
	 * @param _NguoiDungDPS: NguoiDungDPSsModel
	 * @param withBack: boolean
	 */
	updateNguoiDungDPS(_NguoiDungDPS: any, withBack: boolean = false) {

		this.nguoidungdpssService.ResetPassNguoiDungDPS(_NguoiDungDPS).subscribe(res => {
			if (res.status == 1) {
				this.isChange = true;
				const message = `Đặt lại mật khẩu người dùng thành công`;
				this.layoutUtilsService.showInfo(message);
				this.dialogRef.close(this.isChange);
			}
			else {
				this.layoutUtilsService.showError(res.error.message);
			}
		});
	}

	ListIdGroup() {
		this.commonService.getListNhomNguoiDung().subscribe(res => {
			this.loadingSubject.next(false);
			if (res && res.status === 1) {
				this.listIdGroup = res.data;
				this.selectIdGroup = '' + this.listIdGroup[0].IdGroup;
				this.changeDetectorRefs.detectChanges();
			};
		});
		this.commonService.TreeDonVi().subscribe(res => {
			if (res && res.status == 1) {
				this.datatree.next(res.data);
			}
			else {
				this.datatree.next([]);
				this.layoutUtilsService.showError(res.error.message);
			}
		})
		this.commonService.ListLoaiChungThu().subscribe(res => {
			if (res && res.status == 1) {
				this.lstLoaiChungThu = res.data;
			}
			else {
				this.lstLoaiChungThu = [];
				this.layoutUtilsService.showError(res.error.message);
			}
			this.changeDetectorRefs.detectChanges();
		})
		this.commonService.ListGioiTinh().subscribe(res => {
			if (res && res.status == 1) {
				this.lstGioiTinh = res.data;
			}
			else {
				this.lstGioiTinh = [];
				this.layoutUtilsService.showError(res.error.message);
			}
			this.changeDetectorRefs.detectChanges();
		})
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

	remove(fruit: any, loai): void {
		if (loai == 1) {
			const index = this.NguoiDungDPS.DonViQuanTam.indexOf(fruit);

			if (index >= 0) {
				this.NguoiDungDPS.DonViQuanTam.splice(index, 1);
			}
		} else {

			const index = this.NguoiDungDPS.DonViLayHanXuLy.indexOf(fruit);

			if (index >= 0) {
				this.NguoiDungDPS.DonViLayHanXuLy.splice(index, 1);
			}
		}
		this.changeDetectorRefs.detectChanges();
	}
	dv(loai) {
		const dialogRef = this.dialog.open(ChonNhieuDonViComponent, { data: { selected: loai == 1 ? this.NguoiDungDPS.DonViQuanTam : this.NguoiDungDPS.DonViLayHanXuLy } });
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}
			if (loai == 1)
				this.NguoiDungDPS.DonViQuanTam = res;
			else
				this.NguoiDungDPS.DonViLayHanXuLy = res;
			this.changeDetectorRefs.detectChanges();
		});
	}
	removeall(loai) {
		if (loai == 1)
			this.NguoiDungDPS.DonViQuanTam = [];
		else
			this.NguoiDungDPS.DonViLayHanXuLy = [];
		this.changeDetectorRefs.detectChanges();
	}

	FileSelectedPrivate(evt: any, ind: any) {
		if (evt.target.files && evt.target.files.length) {//Nếu có file
			let file = evt.target.files[0]; // Ví dụ chỉ lấy file đầu tiên
			let size = file.size;
			let filename = file.name;
			// if (size >= 9999999) {
			// 	const message = `Không thể upload hình dung lượng quá cao.`;
			// 	this.layoutUtilsService.showActionNotification(message, MessageType.Update, 10000, true, false);
			// 	return;
			// }
			let reader = new FileReader();
			reader.readAsDataURL(evt.target.files[0]);
			let base64Str;
			reader.onload = function () {
				base64Str = reader.result as String;
				var metaIdx = base64Str.indexOf(';base64,');
				base64Str = base64Str.substr(metaIdx + 8); // Cắt meta data khỏi chuỗi base64
				//item.Image = "";
				let f = document.getElementById("imgIcondd" + ind);
				f.setAttribute("src", "data:image/png;base64," + base64Str);

			};
			setTimeout(res => {
				if (ind == 1) {
					this.avatar.strBase64 = base64Str;
					this.avatar.filename = filename;
					this.changeDetectorRefs.detectChanges();
				}
				else if (ind == 2) {
					this.sign.strBase64 = base64Str;
					this.sign.filename = filename;
					this.changeDetectorRefs.detectChanges();
				}
			}, 1000);
		}
	}

	selectFile(ind) {
		let f = document.getElementById("imgInpdd" + ind);
		f["type"] = "text";
		f["type"] = "file";
		f.click();
	}
}
