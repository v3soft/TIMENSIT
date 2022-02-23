// Angular
import { Component, OnInit, ChangeDetectionStrategy, OnDestroy, ChangeDetectorRef, Inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
// Material
import { MatDialog, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
// RxJS
import { Observable, BehaviorSubject, Subscription } from 'rxjs';
// NGRX
// Service
import { LayoutUtilsService, MessageType } from 'app/core/_base/crud';
//Models

import { NguoiDungDPSService } from '../nguoi-dung-dps-service/nguoi-dung-dps.service';
import { TokenStorage } from '../../../../../../core/auth/_services/token-storage.service';
import { ChonDonViComponent } from '../../../components/chon-don-vi/chon-don-vi.component';
import { ChonVaiTroComponent } from '../../../components/chon-vai-tro/chon-vai-tro.component';

@Component({
	selector: 'kt-nguoi-dung-vai-tro',
	templateUrl: './nguoi-dung-vai-tro.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
})

export class NguoiDungVaiTroComponent implements OnInit, OnDestroy {
	// Public properties
	disabledBtn: boolean = false;
	loadingSubject = new BehaviorSubject<boolean>(true);
	loading$: Observable<boolean>;
	viewLoading: boolean = false;
	isChange: boolean = false;

	fixedPoint = 0;

	isZoomSize: boolean = false;
	rR = {};
	User: any;
	ListVaiTro: any[];
	private componentSubscriptions: Subscription;
	displayedColumns: string[] = ['STT'/*, 'DonVi'*/, 'VaiTro',/* 'NguoiKy', 'XuLyViec', 'LanhDao', 'NhanVanBan',*/ 'actions'];

	newrow: any = {
		IdRow: 0,
		UserID: 0,
		IdGroup: 0,
		GroupName: "",
		DonVi: 0,
		TenDonVi: "",
		NguoiKy: false,
		XuLyViec: false,
		LanhDao: false,
		NhanVanBan: false,
		Locked: false,
		Priority: 1
	}
	constructor(
		public dialogRef: MatDialogRef<NguoiDungVaiTroComponent>,
		@Inject(MAT_DIALOG_DATA) public data: any,
		public dialog: MatDialog,
		private layoutUtilsService: LayoutUtilsService,
		private changeDetectorRefs: ChangeDetectorRef,
		private service: NguoiDungDPSService,
		private tokenStorage: TokenStorage) { }

	/**
	 * On init
	 */
	async ngOnInit() {
		this.tokenStorage.getUserRolesObject().subscribe(t => {
			this.rR = t;
		});
		this.User = this.data._item;
		this.newrow.UserID = this.User.UserID;
		if (this.User && this.User.UserID > 0) {
			this.loadVaiTro();
		} else {
			this.changeDetectorRefs.detectChanges();
		}
	}
	loadVaiTro() {
		this.viewLoading = true;
		this.service.getVaiTro(this.User.UserID).subscribe(res => {
			this.viewLoading = false;
			if (res && res.status == 1) {
				this.ListVaiTro = res.data;
				if (this.ListVaiTro.length > 0)
					this.newrow.Priority = this.ListVaiTro[this.ListVaiTro.length - 1].Priority + 1;
			}
			else {
				this.ListVaiTro = [];
				this.layoutUtilsService.showError(res.error.message);
			}
			let _new = Object.assign({}, this.newrow);
			this.ListVaiTro.push(_new);
			this.changeDetectorRefs.detectChanges();
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
	 * Update item
	 *
	 * @param item: any
	 */
	update(item: any) {
		item.IdUser = item.UserID;
		item.IdGroupUser = item.IdGroup;
		this.disabledBtn = true;
		let message = `Cập nhật vai trò người dùng thành công`;
		if (item.IdRow == 0)
			message = `Thêm vai trò người dùng thành công`;
		this.service.updateVaiTro(item).subscribe(res => {
			if (res.status == 1) {
				this.layoutUtilsService.showInfo(message);
				this.loadVaiTro();
			}
			else {
				this.layoutUtilsService.showError(res.error.message);
			}
			this.disabledBtn = false;
			this.changeDetectorRefs.detectChanges();
		});
	}


	/** Delete */
	delete(_item: any) {
		const _title: string = 'Xác nhận';
		const _description: string = 'Bạn chắc chắn xóa vai trò người dùng không?';
		const _waitDesciption: string = 'Vai trò người dùng đang được xóa...';
		const _deleteMessage = `Xóa vai trò người dùng thành công`;

		const dialogRef = this.layoutUtilsService.deleteElement(_title, _description, _waitDesciption);
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}
			this.disabledBtn = true;
			this.service.deleteVaiTro(_item.IdRow).subscribe(res => {
				if (res && res.status === 1) {
					this.layoutUtilsService.showInfo(_deleteMessage);
					this.loadVaiTro();
				}
				else {
					this.layoutUtilsService.showError(res.error.message);
				}
				this.disabledBtn = false;
				this.changeDetectorRefs.detectChanges();
			});
		});
	}
	lock(_item: any, islock = true) {
		this.disabledBtn = true;
		let _message = (islock ? "Khóa" : "Mở khóa") + " vai trò người dùng thành công";
		this.service.lockVaiTro(_item.IdRow, islock).subscribe(res => {
			if (res && res.status === 1) {
				this.layoutUtilsService.showInfo(_message);
				this.loadVaiTro();
			}
			else {
				this.layoutUtilsService.showError(res.error.message);
			}
			this.disabledBtn = false;
			this.changeDetectorRefs.detectChanges();
		});
	}
	chonDV(element) {
		const dialogRef = this.dialog.open(ChonDonViComponent, { data: {} });
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}
			if (element.DonVi != res.id) {
				element.DonVi = res.id;
				element.TenDonVi = res.title;
				element.IdGroup = 0;
				element.GroupName = "";
			}
			this.changeDetectorRefs.detectChanges();
		});
	}
	chonVT(element) {
		const dialogRef = this.dialog.open(ChonVaiTroComponent, { data: { DonVi: element.DonVi } });
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}
			element.IdGroup = res.IdGroup;
			element.GroupName = res.GroupName;
			element.DonVi = res.IdDonVi;
			element.TenDonVi = res.DonVi;
			this.changeDetectorRefs.detectChanges();
		});
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
