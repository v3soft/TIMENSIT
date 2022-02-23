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
import { LogDataSource } from '../log-datasource/log.datasource';
//Service
import { LogService } from '../log-service/log.service';
import { CommonService } from '../../../services/common.service';
import { SubheaderService } from '../../../../../../core/_base/layout';
import { LayoutUtilsService, QueryParamsModel, MessageType } from '../../../../../../core/_base/crud';
//Model

import { LogModel } from '../log-model/log.model';
import { TableService } from '../../../../../partials/table/table.service';
import { TableModel } from '../../../../../partials/table';
import * as moment from 'moment';


@Component({
	selector: 'm-log-list',
	templateUrl: './log-list.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
	providers: [DatePipe]
})

export class LogListComponent implements OnInit, OnDestroy {

	haveFilter: boolean = false;

	// Table fields
	dataSource: LogDataSource;
	@ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
	@ViewChild('sort1', { static: true }) sort: MatSort;
	@ViewChild('trigger', { static: true }) _trigger: MatMenuTrigger;

	// Filter fields
	// Selection
	selection = new SelectionModel<LogModel>(true, []);
	LogsResult: LogModel[] = [];
	tmpLogsResult: LogModel[] = [];

	loadingSubject = new BehaviorSubject<boolean>(false);
	loading$ = this.loadingSubject.asObservable();

	BatDau_tungay: any;
	BatDau_denngay: any;

	ListLoaiDoiTuong: any[] = [];
	ListLoaiHanhDong: any[] = [];
	LoaiDoiTuong: number = 0;
	LoaiHanhDong: string = '0';
	IdDoiTuong: number = 0;

	gridService: TableService;
	girdModel: TableModel = new TableModel();
	displayedColumns1 = ['STT', 'Name', 'actions'];
	listFile: any[] = [];

	list_button: boolean;

	constructor(
		private LogsService: LogService,
		public dialog: MatDialog,
		private route: ActivatedRoute,
		private router: Router,
		private datePipe: DatePipe,
		private translate: TranslateService,
		private subheaderService: SubheaderService,
		private changeDetect: ChangeDetectorRef,
		private layoutUtilsService: LayoutUtilsService,
		private ref: ApplicationRef,
		private commonService: CommonService) { }

	/** LOAD DATA */
	ngOnInit() {
		this.list_button = CommonService.list_button();

		this.route.paramMap.subscribe(params => {
			this.LoaiDoiTuong = +params.get('loai');
			this.IdDoiTuong = +params.get('id');
		});
		if (this.IdDoiTuong == 0) {
			let now = moment();
			let from = now.set("date", 1)
			this.BatDau_tungay = from;
			this.BatDau_denngay = moment();
		}

		//#region ***Filter***
		this.GetAllLoaiDoiTuong();
		this.GetAllLoaiHanhDong();
		this.girdModel.haveFilter = true;
		this.girdModel.tmpfilterText = Object.assign({}, this.girdModel.filterText);
		this.girdModel.filterText['Username'] = "";
		this.girdModel.filterText['IP'] = "";
		this.girdModel.filterText['HanhDong'] = "";
		this.girdModel.filterText['NoiDung'] = "";
		this.girdModel.disableButtonFilter['Locked'] = true;
		//TH1: #filter

		this.girdModel.filterGroupDataChecked = {
			"Locked": [
				{
					name: "Hoạt động",
					value: false,
					checked: false
				},

				{
					name: "Khóa",
					value: true,
					checked: false
				},

			],
		};

		this.girdModel.filterGroupDataCheckedFake = Object.assign({}, this.girdModel.filterGroupDataChecked);
		//#endregion ***Filter***

		//#region ***Drag Drop***
		let availableColumns = [
			{
				stt: 1,
				name: 'select',
				displayName: 'Chọn',
				alwaysChecked: false,
				isShow: true
			},
			{
				stt: 2,
				name: 'STT',
				displayName: 'STT',
				alwaysChecked: false,
				isShow: false
			},
			{
				stt: 2,
				name: 'Id',
				displayName: 'ID',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 3,
				name: 'Username',
				displayName: 'Tên đăng nhập',
				alwaysChecked: false,
				isShow: false
			},
			{

				stt: 3,
				name: 'Fullname',
				displayName: 'Họ tên',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 4,
				name: 'IP',
				displayName: 'IP',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 5,
				name: 'HanhDong',
				displayName: 'Hành động',
				alwaysChecked: false,
				isShow: true
			},

			{

				stt: 6,
				name: 'LoaiLog',
				displayName: 'Loại đối tượng',
				alwaysChecked: false,
				isShow: true
			},

			{

				stt: 7,
				name: 'NoiDung',
				displayName: 'Nội dung',
				alwaysChecked: false,
				isShow: true
			},

			{

				stt: 8,
				name: 'CreatedDate',
				displayName: 'Thời gian',
				alwaysChecked: false,
				isShow: true
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

		this.commonService.fixedPoint = 0;

		// // If the Log changes the sort order, reset back to the first page.
		this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));

		/* Data load will be triggered in two cases:
		- when a pagination event occurs => this.paginator.page
		- when a sort event occurs => this.sort.sortChange
		**/
		merge(this.sort.sortChange, this.paginator.page, this.gridService.result)
			.pipe(
				tap(() => {
					this.loadLogsList(true);
				})
			)
			.subscribe();

		// // Set title to page breadCrumbs
		this.subheaderService.setTitle('');
		// Init DataSource
		this.dataSource = new LogDataSource(this.LogsService);
		let queryParams = new QueryParamsModel({});
		// // Read from URL itemId, for restore previous state
		this.route.queryParams.subscribe(params => {
			queryParams = this.LogsService.lastFilter$.getValue();
			queryParams.filter.LoaiLog = this.LoaiDoiTuong;
			if (this.IdDoiTuong > 0)
				queryParams.filter.IdDoiTuong = this.IdDoiTuong;
			// First load
			this.dataSource.loadLogs(queryParams);
		});
		this.dataSource.entitySubject.subscribe(res => {
			this.LogsResult = res
			this.tmpLogsResult = []
			if (this.LogsResult != null) {
				if (this.LogsResult.length == 0 && this.paginator.pageIndex > 0) {
					this.loadLogsList();
				} else {
					for (let i = 0; i < this.LogsResult.length; i++) {
						let tmpElement = new LogModel();
						tmpElement.copy(this.LogsResult[i])
						this.tmpLogsResult.push(tmpElement);
					}
				}
			}
		});
		this.LogsService.getFileLogs(new QueryParamsModel({})).subscribe(res => {
			this.listFile = res.data;
		})
	}

	ngOnDestroy() {
		this.gridService.Clear();
	}

	GetAllLoaiDoiTuong() {
		this.commonService.Log_LoaiLog().subscribe(res => {
			this.ListLoaiDoiTuong = res.data;
		})
	}
	GetAllLoaiHanhDong() {
		this.commonService.Log_HanhDong().subscribe(res => {
			this.ListLoaiHanhDong = res.data;
		})
	}

	loadLogsList(holdCurrentPage: boolean = false) {
		this.selection.clear();
		const queryParams = new QueryParamsModel(
			this.filterConfiguration(),
			this.sort.direction,
			this.sort.active,
			holdCurrentPage ? this.paginator.pageIndex : this.paginator.pageIndex = 0,
			this.paginator.pageSize,

			this.gridService.model.filterGroupData

		);
		this.dataSource.loadLogs(queryParams);
	}

	/** FILTRATION */
	filterConfiguration(): any {
		const filter: any = {};

		//#filter
		if (this.gridService.model.filterText) {
			// filter.DanhMuc = this.gridService.model.filterText['DanhMuc'];
			// filter.MaDanhMuc = this.gridService.model.filterText['MaDanhMuc'];
			filter.Username = this.gridService.model.filterText['Username'];
			filter.IP = this.gridService.model.filterText['IP'];
			filter.HanhDong = this.gridService.model.filterText['HanhDong'];
			filter.NoiDung = this.gridService.model.filterText['NoiDung'];
		}


		filter.LoaiLog = this.LoaiDoiTuong;
		if (this.IdDoiTuong > 0)
			filter.IdDoiTuong = this.IdDoiTuong;
		filter.LoaiHanhDong = this.LoaiHanhDong;

		if (this.BatDau_tungay != undefined) {
			filter.tungay = moment(this.BatDau_tungay).format("YYYY-MM-DD");
		}
		if (this.BatDau_denngay != undefined) {
			filter.denngay = moment(this.BatDau_denngay).format("YYYY-MM-DD");
		}

		return filter;
	}
	/** ACTIONS */
	/** Delete */
	delete(_item: any) {
		const _title: string = 'Xác nhận';
		const _description: string = 'Bạn có chắc muốn xóa lịch sử?';
		const _waitDesciption: string = 'Log đang được xóa...';
		const _deleteMessage = `Xóa thành công`;

		const dialogRef = this.layoutUtilsService.deleteElement(_title, _description, _waitDesciption);
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}
			this.LogsService.deleteLog(_item.IdRow).subscribe(res => {
				if (res && res.status === 1) {
					this.layoutUtilsService.showInfo(_deleteMessage);
				}
				else {
					this.layoutUtilsService.showError(res.error.message);
				}
				this.loadLogsList(true);
			});
		});
	}

	deleteLogs() {
		const _title: string = 'Xóa lịch sử';
		const _description: string = 'Bạn có chắc muốn xóa những lịch sử này không?';
		const _waitDesciption: string = 'Lịch sử đang được xóa...';
		const _deleteMessage = `Lịch sử đã được xóa`;

		const dialogRef = this.layoutUtilsService.deleteElement(_title, _description, _waitDesciption);
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}

			const idsForDeletion: number[] = [];
			for (let i = 0; i < this.selection.selected.length; i++) {
				idsForDeletion.push(

					this.selection.selected[i].Id

				);
			}
			this.LogsService.deleteLogs(idsForDeletion).subscribe(() => {
				this.layoutUtilsService.showInfo(_deleteMessage);
				this.loadLogsList(true);
				this.selection.clear();
			});
		});
	}




	/** SELECTION */
	isAllSelected() {
		const numSelected = this.selection.selected.length;
		const numRows = this.LogsResult.length;
		return numSelected === numRows;
	}

	/** Selects all rows if they are not all selected; otherwise clear selection. */
	masterToggle() {
		if (this.isAllSelected()) {
			this.selection.clear();
		} else {
			this.LogsResult.forEach(row => this.selection.select(row));
		}
	}
}
