import { Component, OnInit, ViewChild, ChangeDetectionStrategy, OnDestroy, ApplicationRef, ElementRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DatePipe } from '@angular/common';
// Material
import { MatPaginator, MatSort, MatDialog, MatMenuTrigger } from '@angular/material';
import { SelectionModel } from '@angular/cdk/collections';
// RXJS
import { tap } from 'rxjs/operators';
import { merge, BehaviorSubject } from 'rxjs';
//Datasource
import { NguoiDungDPSDataSource } from '../nguoi-dung-dps-datasource/nguoi-dung-dps.datasource';
//Service
import { NguoiDungDPSService } from '../nguoi-dung-dps-service/nguoi-dung-dps.service';
import { NguoiDungDPSEditComponent } from '../nguoi-dung-dps-edit/nguoi-dung-dps-edit.component';
//Model

import { NguoiDungDPSModel } from '../nguoi-dung-dps-model/nguoi-dung-dps.model';
import { CommonService } from '../../../services/common.service';
import { TranslateService } from '@ngx-translate/core';
import { TokenStorage } from '../../../../../../core/auth/_services/token-storage.service';
import { NguoiDungVaiTroComponent } from '../nguoi-dung-vai-tro/nguoi-dung-vai-tro.component';
import { NguoiDungDPSImportComponent } from '../nguoi-dung-dps-import/nguoi-dung-dps-import.component';
import { NguoiDungDPSResetPasswordComponent } from '../nguoi-dung-dps-reset-password/nguoi-dung-dps-reset-password.component';
import { TableModel } from '../../../../../partials/table';
import { TableService } from '../../../../../partials/table/table.service';
import { LayoutUtilsService, QueryParamsModel } from '../../../../../../core/_base/crud';
// import { element } from 'protractor';
// import * as jspdf from 'jspdf';
// import html2canvas from 'html2canvas';
// import { ExportAsService, ExportAsConfig } from 'ngx-export-as';

@Component({
	selector: 'm-nguoi-dung-dps-list',
	templateUrl: './nguoi-dung-dps-list.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
	providers: [DatePipe]
})
export class NguoiDungDPSListComponent implements OnInit, OnDestroy {
	haveFilter: boolean = false;

	// Table fields
	dataSource: NguoiDungDPSDataSource;

	@ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
	@ViewChild('sort1', { static: true }) sort: MatSort;
	@ViewChild('trigger', { static: true }) _trigger: MatMenuTrigger;
	@ViewChild('printPage', { static: true }) printPage: ElementRef;
	// Filter fields
	listIdGroup: any[] = [];

	// Selection
	selection = new SelectionModel<NguoiDungDPSModel>(true, []);
	nguoidungdpssResult: NguoiDungDPSModel[] = [];
	tmpnguoidungdpssResult: NguoiDungDPSModel[] = [];

	loadingSubject = new BehaviorSubject<boolean>(false);
	loading$ = this.loadingSubject.asObservable();

	name = 'Người dùng';
	rR = {};
	curUser: any = {};
	DonVi: number = 0;
	datatree: BehaviorSubject<any[]> = new BehaviorSubject([]);
	VaiTro: number = 0;
	lstVaiTro: any[] = [];
	ChuaCoVaiTro: boolean = false;
	gridService: TableService;
	girdModel: TableModel = new TableModel();
	list_button: boolean;
	// exportAsConfig: ExportAsConfig = {
	// 	type: 'xlsx', // the type you want to download
	// 	elementIdOrContent: 'printpdf', // the id of html/table element
	// 	download:true
	// }

	constructor(
		private nguoidungdpssService: NguoiDungDPSService,
		public dialog: MatDialog,
		private route: ActivatedRoute,
		private layoutUtilsService: LayoutUtilsService,
		private ref: ApplicationRef,
		private tokenStorage: TokenStorage,
		private commonService: CommonService,
		private translate: TranslateService,
		// private exportAsService: ExportAsService	
	) { }

	/** LOAD DATA */
	ngOnInit() {
		this.list_button = CommonService.list_button();
		this.tokenStorage.getUserRolesObject().subscribe(t => {
			this.rR = t;
		});
		this.tokenStorage.getUserInfo().subscribe(res => {
			this.curUser = res;
		})
		//#region ***Filter***
		this.girdModel.haveFilter = true;
		this.girdModel.tmpfilterText = Object.assign({}, this.girdModel.filterText);
		this.girdModel.filterText['FullName'] = "";
		this.girdModel.filterText['UserName'] = "";
		this.girdModel.filterText['Email'] = "";
		this.girdModel.filterText['PhoneNumber'] = "";
		this.girdModel.filterText['MaNV'] = "";
		this.girdModel.filterText['ViettelStudy'] = "";
		this.girdModel.disableButtonFilter['Active'] = true;
		this.girdModel.disableButtonFilter['NhanLichDonVi'] = true;
		this.girdModel.disableButtonFilter['ChucVu'] = true;

		let optionsTinhTrang = [
			{
				name: 'Khóa',
				value: '0',
			},
			{
				name: 'Kích hoạt',
				value: '1',
			}
		];

		this.girdModel.filterGroupDataChecked['Active'] = optionsTinhTrang.map(x => {
			return {
				name: x.name,
				value: x.value,
				checked: false
			}
		});
		this.girdModel.filterGroupDataChecked['NhanLichDonVi'] = [
			{
				name: 'Có',
				value: 'True',
				checked: false
			},
			{
				name: 'Không',
				value: 'False',
				checked: false
			}
		]
		this.girdModel.filterGroupDataCheckedFake = Object.assign({}, this.girdModel.filterGroupDataChecked);
		//this.commonService.getListNhomNguoiDung().subscribe(res => {
		//	if (res && res.status === 1) {
		//		this.listIdGroup = res.data;
		//	};
		//	this.gridService.filterGroupDataChecked['IdGroup'] = this.listIdGroup.map(x => {
		//		return {
		//			id: x.IdGroup,
		//			name: x.GroupName,
		//			value: x.IdGroup,
		//			checked: false
		//		}
		//	});
		//	this.gridService.filterGroupDataCheckedFake = Object.assign({}, this.gridService.filterGroupDataChecked);
		//});
		//#endregion ***Filter***

		//#region ***Drag Drop***
		let availableColumns = [
			//{
			//	stt: 1,
			//	name: 'select',
			//	displayName: 'Check chọn',
			//	alwaysChecked: true,
			//	isShow: true
			//},
			{
				stt: 1,
				name: 'STT',
				displayName: 'STT',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 2,
				name: 'Avatar',
				displayName: 'Avatar',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 3,
				name: 'MaNV',
				displayName: 'Mã NV',
				alwaysChecked: false,
				isShow: false
			},
			{

				stt: 4,
				name: 'UserName',
				displayName: 'Tên đăng nhập',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 5,
				name: 'FullName',
				displayName: 'Họ tên',
				alwaysChecked: false,
				isShow: true
			},
			//{

			//	stt: 6,
			//	name: 'ViettelStudy',
			//	displayName: 'Tài khoản Viettel Study',
			//	alwaysChecked: false,
			//	isShow: true
			//},
			{

				stt: 7,
				name: 'ChucVu',
				displayName: 'Chức vụ',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 8,
				name: 'DonVi',
				displayName: 'Đơn vị',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 9,
				name: 'Email',
				displayName: 'Email',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 10,
				name: 'PhoneNumber',
				displayName: 'Số điện thoại',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 11,
				name: 'ExpDate',
				displayName: 'Ngày hết hạn',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 12,
				name: 'Active',
				displayName: 'Tình trạng',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 13,
				name: 'GioiTinh',
				displayName: 'Giới tính',
				alwaysChecked: false,
				isShow: false
			},
			//{

			//	stt: 14,
			//	name: 'NhanLichDonVi',
			//	displayName: 'Nhận lịch đơn vị',
			//	alwaysChecked: false,
			//	isShow: false
			//},
			{

				stt: 15,
				name: 'LastLogin',
				displayName: 'Đăng nhập lần cuối',
				alwaysChecked: false,
				isShow: false
			},
			{
				stt: 99,
				name: 'actions',
				displayName: 'Thao tác',
				alwaysChecked: false,
				isShow: true
			}
		];
		this.girdModel.availableColumns = availableColumns.sort((a, b) => a.stt - b.stt);
		this.girdModel.selectedColumns = new SelectionModel<any>(true, this.girdModel.availableColumns);

		this.gridService = new TableService(this.layoutUtilsService, this.ref, this.girdModel);
		this.gridService.showColumnsInTable();
		this.gridService.applySelectedColumns();
		//#endregion

		this.loadData();
		this.commonService.TreeDonVi().subscribe(res => {
			if (res && res.status == 1) {
				this.datatree.next(res.data);
			}
			else {
				this.datatree.next([]);
				this.layoutUtilsService.showError(res.error.message);
			}
		});
		// // If the NguoiDungDPS changes the sort order, reset back to the first page.
		this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));

		/* Data load will be triggered in two cases:
		- when a pagination event occurs => this.paginator.page
		- when a sort event occurs => this.sort.sortChange
		- when a filter event occurs => this.gridService.result
		**/
		merge(this.sort.sortChange, this.paginator.page, this.gridService.result)
			.pipe(
				tap(() => {
					this.loadNguoiDungDPSsList(true);
				})
			)
			.subscribe();
		// Init DataSource
		this.dataSource = new NguoiDungDPSDataSource(this.nguoidungdpssService);
		let queryParams = new QueryParamsModel({});
		// // Read from URL itemId, for restore previous state
		this.route.queryParams.subscribe(params => {
			queryParams = this.nguoidungdpssService.lastFilter$.getValue();
			// First load
			this.dataSource.loadNguoiDungDPSs(queryParams);
		});
		this.dataSource.entitySubject.subscribe(res => {
			this.nguoidungdpssResult = res
			this.tmpnguoidungdpssResult = []
			if (this.nguoidungdpssResult != null) {
				if (this.nguoidungdpssResult.length == 0 && this.paginator.pageIndex > 0) {
					this.loadNguoiDungDPSsList();
				} else {
					for (let i = 0; i < this.nguoidungdpssResult.length; i++) {
						let tmpElement = new NguoiDungDPSModel();
						tmpElement.copy(this.nguoidungdpssResult[i])
						this.tmpnguoidungdpssResult.push(tmpElement);
					}
				}
			}
		});
	}

	ngOnDestroy() {
		this.gridService.Clear();
	}

	loadData() {
		this.commonService.ListVaiTroByDonVi(this.DonVi).subscribe(res => {
			if (res && res.status == 1)
				this.lstVaiTro = res.data;
			else {
				this.lstVaiTro = [];
				this.layoutUtilsService.showError(res.error.message);
			}
		});

		this.commonService.ListChucVu(this.DonVi).subscribe(res => {
			if (res && res.status == 1) {
				this.gridService.model.filterGroupDataChecked['ChucVu'] = res.data.map(x => {
					return {
						id: x.id,
						name: x.title,
						value: x.id,
						checked: false
					}
				});
			} else
				this.gridService.model.filterGroupDataChecked['ChucVu'] = [];
			this.gridService.model.filterGroupDataCheckedFake = Object.assign({}, this.gridService.model.filterGroupDataChecked);
		})
	}

	LoadPagePrint() {
		// this.girdModel.availableColumns[this.girdModel.availableColumns.length -1].isShow = false;
		const printPage = this.printPage.nativeElement as HTMLElement;
		printPage.click()
	}

	GetValueNode(item) {
		this.DonVi = item.id;
		this.VaiTro = 0;
		this.loadData();
		this.loadNguoiDungDPSsList(true);
	}
	loadNguoiDungDPSsList(holdCurrentPage: boolean = false) {
		this.selection.clear();
		const queryParams = new QueryParamsModel(
			this.filterConfiguration(),
			this.sort.direction,
			this.sort.active,
			holdCurrentPage ? this.paginator.pageIndex : this.paginator.pageIndex = 0,
			this.paginator.pageSize,
			this.gridService.model.filterGroupData
		);
		this.dataSource.loadNguoiDungDPSs(queryParams);
	}

	/** FILTRATION */
	filterConfiguration(): any {
		const filter: any = {};
		filter.ChuaCoVaiTro = this.ChuaCoVaiTro;
		if (this.DonVi > 0)
			filter.Donvi = this.DonVi;
		if (this.VaiTro > 0)
			filter.VaiTro = this.VaiTro;
		if (this.gridService.model.filterText) {
			filter.FullName = this.gridService.model.filterText['FullName'];
			filter.UserName = this.gridService.model.filterText['UserName'];
			filter.Email = this.gridService.model.filterText['Email'];
			filter.PhoneNumber = this.gridService.model.filterText['PhoneNumber'];
			filter.MaNV = this.gridService.model.filterText['MaNV'];
			filter.ViettelStudy = this.gridService.model.filterText['ViettelStudy'];
		}
		return filter;
	}
	/** ACTIONS */
	/** Delete */
	deleteNguoiDungDPS(_item: NguoiDungDPSModel) {
		const _title: string = 'Xác nhận';
		const _description: string = 'Bạn chắc chắn xóa người dùng?';
		const _waitDesciption: string = 'Người dùng đang được xóa...';
		const _deleteMessage = `Xóa người dùng thành công`;

		const dialogRef = this.layoutUtilsService.deleteElement(_title, _description, _waitDesciption);
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}
			this.nguoidungdpssService.deleteNguoiDungDPS(_item.UserID).subscribe(res => {
				if (res && res.status === 1) {
					this.layoutUtilsService.showInfo(_deleteMessage);
				}
				else {
					this.layoutUtilsService.showError(res.error.message);
				}
				this.loadNguoiDungDPSsList(true);
			});
		});
	}
	lock(_item: any, islock = true) {
		let _message = (islock ? "Khóa" : "Mở khóa") + " người dùng thành công";
		this.nguoidungdpssService.lock(_item.UserID, islock).subscribe(res => {
			if (res && res.status === 1) {
				this.layoutUtilsService.showInfo(_message);
			}
			else {
				this.layoutUtilsService.showError(res.error.message);
			}
			this.loadNguoiDungDPSsList(true);
		});
	}

	/** SELECTION */
	isAllSelected() {
		const numSelected = this.selection.selected.length;
		const numRows = this.nguoidungdpssResult.length;
		return numSelected === numRows;
	}

	/** Selects all rows if they are not all selected; otherwise clear selection. */
	masterToggle() {
		if (this.isAllSelected()) {
			this.selection.clear();
		} else {
			this.nguoidungdpssResult.forEach(row => this.selection.select(row));
		}
	}

	/**
	 * Show add NguoiDungDPS dialog
	 */
	addNguoiDungDPS() {
		const newNguoiDungDPS = new NguoiDungDPSModel();
		newNguoiDungDPS.clear(); // Set all defaults fields
		this.editNguoiDungDPS(newNguoiDungDPS);
	}
	/**
	 * Show Edit NguoiDungDPS dialog and save after success close result
	 * @param NguoiDungDPS: NguoiDungDPSModel
	 */
	editNguoiDungDPS(NguoiDungDPS: NguoiDungDPSModel, allowEdit: boolean = true) {
		const dialogRef = this.dialog.open(NguoiDungDPSEditComponent, { data: { NguoiDungDPS: NguoiDungDPS, allowEdit: allowEdit } });
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}
			if (NguoiDungDPS.Id)
				this.loadNguoiDungDPSsList();
			else
				this.loadNguoiDungDPSsList(true);
		});
	}

	ResetPassNguoiDungDPS(NguoiDungDPS: NguoiDungDPSModel, allowEdit: boolean = true) {
		const dialogRef = this.dialog.open(NguoiDungDPSResetPasswordComponent, { data: { NguoiDungDPS: NguoiDungDPS, allowEdit: allowEdit }, width: '650px' });
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}
			if (NguoiDungDPS.Id)
				this.loadNguoiDungDPSsList();
			else
				this.loadNguoiDungDPSsList(true);
		});
	}

	vaitro(_item: NguoiDungDPSModel) {
		const dialogRef = this.dialog.open(NguoiDungVaiTroComponent, { data: { _item }/*, width: '80%'*/ });
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}
		});
	}

	giahan(_item: NguoiDungDPSModel) {
		const _title: string = 'Xác nhận';
		const _description: string = 'Bạn chắc chắn gia hạn mật khẩu cho người dùng?';
		const _waitDesciption: string = 'Người dùng đang được gia hạn...';
		const _deleteMessage = `Gian hạn mật người cho dùng thành công`;

		const dialogRef = this.layoutUtilsService.deleteElement(_title, _description, _waitDesciption);
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}
			this.nguoidungdpssService.GiaHan(_item.UserID).subscribe(res => {
				if (res && res.status === 1) {
					this.layoutUtilsService.showInfo(_deleteMessage);
				}
				else {
					this.layoutUtilsService.showError(res.error.message);
				}
				this.loadNguoiDungDPSsList(true);
			});
		});
	}

	/* UI */
	getItemStatusString(status: number = 0): string {
		switch (status) {
			case 0:
				return 'Khóa';
			case 1:
				return 'Hoạt động';
		}
		return '';
	}

	getItemCssClassByStatus(status: number = 0): string {
		switch (status) {
			case 0:
				return 'kt-badge--metal';
			case 1:
				return 'kt-badge--success';
		}
		return '';
	}
	import() {
		const dialogRef = this.dialog.open(NguoiDungDPSImportComponent);
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}
			this.loadNguoiDungDPSsList();
		});
	}
	ExportFile() {
		this.nguoidungdpssService.ExportFile().subscribe(response => {
			const headers = response.headers;
			const filename = headers.get('x-filename');
			const type = headers.get('content-type');
			const blob = new Blob([response.body], { type });
			const fileURL = URL.createObjectURL(blob);
			const link = document.createElement('a');
			link.href = fileURL;
			link.download = filename;
			link.click();
		});
	}

	public convetToPDF() {
		// window.scrollTo(0, 0);
		// var data = document.getElementById('printpdf');
		// var hiden = document.getElementsByClassName('hiden-print') as HTMLCollectionOf<HTMLElement> ;
		// for(var i=0; i< hiden.length; i++){
		// 	hiden[i].style.display = "none"
		 
		// }
		// html2canvas(data).then(canvas => {
		// 	debugger
		// 	// Few necessary setting options
		// 	var imgWidth = 219;
		// 	var pageHeight = 295;
		// 	var imgHeight = 512*canvas.height/canvas.width;
		// 	var heightLeft = imgHeight;
		// 	const contentDataURL = canvas.toDataURL('image/png')
		// 	let pdf;
		// 	pdf = new jspdf.jsPDF('p', 'mm', 'a4');
		// 	var x = 0;
		// 	var y = 0;
		// 	for(var i=0; i< hiden.length; i++){
		// 		hiden[i].style.display = ""
		// 	}
		// 	pdf.addImage(contentDataURL, 'PNG', x, y, imgWidth, imgHeight)
		// 	pdf.save('danh-sach-nguoi-dung.pdf'); // Generated PDF
		// });
		// this.viewLoading = false;
	}
	
	DownloadFile() {
		// download the file using old school javascript method
		// var hiden = document.getElementsByClassName('hiden-print') as HTMLCollectionOf<HTMLElement> ;
		// for(var i=0; i< hiden.length; i++){
		// 	hiden[i].style.visibility = "hidden"
		// }
		// this.exportAsService.save(this.exportAsConfig, 'hinhanh').subscribe(() => {
		// });
		
		// setTimeout(() => {
		// 	for(var i=0; i< hiden.length; i++){
		// 		hiden[i].style.visibility = "visible"
		// 	}
		// }, 10000);
	}

}
