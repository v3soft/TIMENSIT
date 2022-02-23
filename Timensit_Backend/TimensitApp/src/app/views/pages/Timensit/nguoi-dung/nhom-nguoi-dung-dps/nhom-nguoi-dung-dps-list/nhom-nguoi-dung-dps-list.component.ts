import { Component, OnInit, ElementRef, ViewChild, ChangeDetectionStrategy, ChangeDetectorRef, HostListener, OnDestroy, ApplicationRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DatePipe } from '@angular/common';
import { TranslateService } from '@ngx-translate/core';
// Material
import { MatPaginator, MatSort, MatDialog, MatTableDataSource, MatMenuTrigger } from '@angular/material';
import { SelectionModel } from '@angular/cdk/collections';
import { CdkDragStart, CdkDropList, moveItemInArray } from '@angular/cdk/drag-drop';
// RXJS
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { ReplaySubject, fromEvent, merge, BehaviorSubject } from 'rxjs';
//Datasource
import { NhomNguoiDungDPSDataSource } from '../nhom-nguoi-dung-dps-datasource/nhom-nguoi-dung-dps.datasource';
//Service
import { NhomNguoiDungDPSService } from '../nhom-nguoi-dung-dps-service/nhom-nguoi-dung-dps.service';
import { SubheaderService } from 'app/core/_base/layout';
import { LayoutUtilsService, QueryParamsModel, MessageType } from 'app/core/_base/crud';
import { NhomNguoiDungDPSEditComponent } from '../nhom-nguoi-dung-dps-edit/nhom-nguoi-dung-dps-edit.component';
import { environment } from 'environments/environment';
//Model

import { NhomNguoiDungDPSModel } from '../nhom-nguoi-dung-dps-model/nhom-nguoi-dung-dps.model';
import { TokenStorage } from '../../../../../../core/auth/_services/token-storage.service';
import { CommonService } from '../../../services/common.service';
import { PhanQuyenComponent } from '../phan-quyen/phan-quyen.component';
import { TableModel } from '../../../../../partials/table';
import { TableService } from '../../../../../partials/table/table.service';


@Component({
	selector: 'm-nhom-nguoi-dung-dps-list',
	templateUrl: './nhom-nguoi-dung-dps-list.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
	providers: [DatePipe]
})
export class NhomNguoiDungDPSListComponent implements OnInit, OnDestroy {
	// Table fields
	dataSource: NhomNguoiDungDPSDataSource;

	@ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
	@ViewChild('sort1', { static: true }) sort: MatSort;
	@ViewChild('trigger', { static: true }) _trigger: MatMenuTrigger;

	// Selection
	selection = new SelectionModel<NhomNguoiDungDPSModel>(true, []);
	nhomnguoidungdpssResult: NhomNguoiDungDPSModel[] = [];
	tmpnhomnguoidungdpssResult: NhomNguoiDungDPSModel[] = [];

	loadingSubject = new BehaviorSubject<boolean>(false);
	loading$ = this.loadingSubject.asObservable();

	previousIndex: number;

	fixedPoint = 0;
	name = "Vai trò";
	rR = {};
	DonVi: number = 0;
	datatree: BehaviorSubject<any[]> = new BehaviorSubject([]);
	gridService: TableService;
	girdModel: TableModel = new TableModel();
	disabledBtn: boolean = false;
	list_button: boolean;
	constructor(
		private nhomnguoidungdpssService: NhomNguoiDungDPSService,
		public dialog: MatDialog,
		private route: ActivatedRoute,
		private router: Router,
		private datePipe: DatePipe,
		private translate: TranslateService,
		private subheaderService: SubheaderService,
		private changeDetect: ChangeDetectorRef,
		private layoutUtilsService: LayoutUtilsService,
		private ref: ApplicationRef,
		private tokenStorage: TokenStorage,
		private commonService: CommonService, ) { }

	/** LOAD DATA */
	ngOnInit() {
		this.list_button = CommonService.list_button();
		this.tokenStorage.getUserRolesObject().subscribe(t => {
			this.rR = t;
		});
		//#region ***Filter***
		this.girdModel.haveFilter = true;
		this.girdModel.tmpfilterText = Object.assign({}, this.girdModel.filterText);
		this.girdModel.filterText['GroupName'] = "";
		this.girdModel.filterText['Ma'] = "";
		this.girdModel.filterText['GhiChu'] = "";
		this.girdModel.disableButtonFilter['Locked'] = true;
		this.girdModel.filterGroupDataChecked['Locked'] = [
			{
				name: 'Khóa',
				value: 'True',
				checked: false
			},
			{
				name: 'Hoạt động',
				value: 'False',
				checked: false
			}
		]
		this.girdModel.filterGroupDataCheckedFake = Object.assign({}, this.girdModel.filterGroupDataChecked);

		this.commonService.TreeDonVi().subscribe(res => {
			if (res && res.status == 1) {
				this.datatree.next(res.data);
			}
			else {
				this.datatree.next([]);
				this.layoutUtilsService.showError(res.error.message);
			}
		})
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
				stt: 2,
				name: 'STT',
				displayName: 'STT',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 3,
				name: 'GroupName',
				displayName: 'Tên vai trò',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 4,
				name: 'Ma',
				displayName: 'Mã vai trò',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 5,
				name: 'GhiChu',
				displayName: 'Mô tả',
				alwaysChecked: false,
				isShow: true
			},
			//{

			//	stt: 6,
			//	name: 'DonVi',
			//	displayName: 'Đơn vị',
			//	alwaysChecked: false,
			//	isShow: true
			//},
			//{

			//	stt: 7,
			//	name: 'ChucVu',
			//	displayName: 'Chức vụ',
			//	alwaysChecked: false,
			//	isShow: false
			//},
			//{

			//	stt: 8,
			//	name: 'IsDefault',
			//	displayName: 'Vai trò mặc định',
			//	alwaysChecked: false,
			//	isShow: false
			//},
			{

				stt: 9,
				name: 'DisplayOrder',
				displayName: 'Thứ tự',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 10,
				name: 'Locked',
				displayName: 'Trạng thái',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 11,
				name: 'ModifiedBy',
				displayName: 'Người cập nhật cuối',
				alwaysChecked: false,
				isShow: false
			},
			{

				stt: 12,
				name: 'ModifiedDate',
				displayName: 'Lần cập nhật cuối',
				alwaysChecked: false,
				isShow: false
			},
			{
				stt: 99,
				name: 'actions',
				displayName: 'Thao tác',
				alwaysChecked: true,
				isShow: true
			}
		];
		this.girdModel.availableColumns = availableColumns.sort((a, b) => a.stt - b.stt);
		this.girdModel.selectedColumns = new SelectionModel<any>(true, this.girdModel.availableColumns);

		this.gridService = new TableService(this.layoutUtilsService, this.ref, this.girdModel);
		this.gridService.showColumnsInTable();
		this.gridService.applySelectedColumns();
		//#endregion

		// // If the NhomNguoiDungDPS changes the sort order, reset back to the first page.
		this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));

		/* Data load will be triggered in two cases:
		- when a pagination event occurs => this.paginator.page
		- when a sort event occurs => this.sort.sortChange
		**/
		merge(this.sort.sortChange, this.paginator.page, this.gridService.result)
			.pipe(
				tap(() => {
					this.loadNhomNguoiDungDPSsList(true);
				})
			).subscribe();
		// // Set title to page breadCrumbs
		this.subheaderService.setTitle('');
		// Init DataSource
		this.dataSource = new NhomNguoiDungDPSDataSource(this.nhomnguoidungdpssService);
		let queryParams = new QueryParamsModel({});
		// // Read from URL itemId, for restore previous state
		this.route.queryParams.subscribe(params => {
			queryParams = this.nhomnguoidungdpssService.lastFilter$.getValue();
			// First load
			this.dataSource.loadNhomNguoiDungDPSs(queryParams);
		});
		this.dataSource.entitySubject.subscribe(res => {
			this.nhomnguoidungdpssResult = res
			this.tmpnhomnguoidungdpssResult = []
			if (this.nhomnguoidungdpssResult != null) {
				if (this.nhomnguoidungdpssResult.length == 0 && this.paginator.pageIndex > 0) {
					this.loadNhomNguoiDungDPSsList(true);
				} else {
					for (let i = 0; i < this.nhomnguoidungdpssResult.length; i++) {
						let tmpElement = new NhomNguoiDungDPSModel();
						tmpElement.copy(this.nhomnguoidungdpssResult[i])
						this.tmpnhomnguoidungdpssResult.push(tmpElement);
					}
				}
			}
		});
	}

	ngOnDestroy() {
		this.gridService.Clear();
	}

	GetValueNode(item) {
		this.DonVi = item.id;
		this.loadNhomNguoiDungDPSsList(true);
	}

	loadNhomNguoiDungDPSsList(holdCurrentPage: boolean = false) {
		this.selection.clear();
		const queryParams = new QueryParamsModel(
			this.filterConfiguration(),
			this.sort.direction,
			this.sort.active,
			holdCurrentPage ? this.paginator.pageIndex : this.paginator.pageIndex = 0,
			this.paginator.pageSize,
			this.gridService.model.filterGroupData

		);
		this.dataSource.loadNhomNguoiDungDPSs(queryParams);
	}

	/** FILTRATION */
	filterConfiguration(): any {
		const filter: any = {};
		if (this.DonVi > 0)
			filter.DonVi = this.DonVi;
		if (this.gridService.model.filterText) {
			filter.GroupName = this.gridService.model.filterText['GroupName'];
			filter.GhiChu = this.gridService.model.filterText['GhiChu'];
			filter.Ma = this.gridService.model.filterText['Ma'];
		}
		return filter;
	}
	/** ACTIONS */
	/** Delete */
	deleteNhomNguoiDungDPS(_item: NhomNguoiDungDPSModel) {
		const _title: string = 'Xác nhận';
		const _description: string = 'Bạn chắc chắn xóa vai trò?';
		const _waitDesciption: string = 'Vai trò đang được xóa...';
		const _deleteMessage = `Xóa vai trò thành công`;

		const dialogRef = this.layoutUtilsService.deleteElement(_title, _description, _waitDesciption);
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}
			this.nhomnguoidungdpssService.deleteNhomNguoiDungDPS(_item.IdGroup).subscribe(res => {
				if (res && res.status === 1) {
					this.layoutUtilsService.showInfo(_deleteMessage);
				}
				else {
					this.layoutUtilsService.showError(res.error.message);
				}
				this.loadNhomNguoiDungDPSsList(true);
			});
		});
	}

	lock(_item: NhomNguoiDungDPSModel, islock = true) {
		let _message = (islock ? "Khóa" : "Mở khóa") + " vai trò thành công";
		this.nhomnguoidungdpssService.lock(_item.IdGroup, islock).subscribe(res => {
			if (res && res.status === 1) {
				this.layoutUtilsService.showInfo(_message);
			}
			else {
				this.layoutUtilsService.showError(res.error.message);
			}
			this.loadNhomNguoiDungDPSsList(true);
		});
	}

	/** SELECTION */
	isAllSelected() {
		const numSelected = this.selection.selected.length;
		const numRows = this.nhomnguoidungdpssResult.length;
		return numSelected === numRows;
	}

	/** Selects all rows if they are not all selected; otherwise clear selection. */
	masterToggle() {
		if (this.isAllSelected()) {
			this.selection.clear();
		} else {
			this.nhomnguoidungdpssResult.forEach(row => this.selection.select(row));
		}
	}
	/**
	 * Show add NhomNguoiDungDPS dialog
	 */
	addNhomNguoiDungDPS() {
		const newNhomNguoiDungDPS = new NhomNguoiDungDPSModel();
		newNhomNguoiDungDPS.clear(); // Set all defaults fields
		this.editNhomNguoiDungDPS(newNhomNguoiDungDPS);
	}
	/**
	 * Show Edit NhomNguoiDungDPS dialog and save after success close result
	 * @param NhomNguoiDungDPS: NhomNguoiDungDPSModel
	 */
	editNhomNguoiDungDPS(NhomNguoiDungDPS: NhomNguoiDungDPSModel, allowEdit: boolean = true) {
		const dialogRef = this.dialog.open(NhomNguoiDungDPSEditComponent, { data: { NhomNguoiDungDPS: NhomNguoiDungDPS, allowEdit: allowEdit } });
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}

			this.loadNhomNguoiDungDPSsList(true);
		});
	}

	phanquyen(NhomNguoiDungDPS: NhomNguoiDungDPSModel) {
		const dialogRef = this.dialog.open(PhanQuyenComponent, { data: { NhomNguoiDungDPS } });
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}

			this.loadNhomNguoiDungDPSsList(true);
		});
	}
	/* UI */
	getItemStatusString(status: boolean = false): string {
		switch (status) {
			case true:
				return 'Khóa';
			case false:
				return 'Hoạt động';
		}
		return '';
	}

	getItemCssClassByStatus(status: boolean = false): string {
		switch (status) {
			case true:
				return 'kt-badge--metal';
			case false:
				return 'kt-badge--success';
		}
		return '';
	}
}
