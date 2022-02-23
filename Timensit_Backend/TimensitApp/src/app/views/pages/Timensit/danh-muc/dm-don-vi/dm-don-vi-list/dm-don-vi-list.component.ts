import { Component, OnInit, ElementRef, ViewChild, ChangeDetectionStrategy, ChangeDetectorRef, HostListener, Input, OnChanges, Output, EventEmitter, ApplicationRef } from '@angular/core';
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
import { DM_DonViDataSource } from '../dm-don-vi-datasource/dm-don-vi.datasource';
//Service
import { DM_DonViService } from '../dm-don-vi-service/dm-don-vi.service';
import { SubheaderService } from 'app/core/_base/layout';
import { LayoutUtilsService, QueryParamsModel, MessageType } from 'app/core/_base/crud';
import { DM_DonViEditComponent } from '../dm-don-vi-edit/dm-don-vi-edit.component';
import { DM_DonViImportComponent } from '../dm-don-vi-import/dm-don-vi-import.component';
import { environment } from 'environments/environment';
//Model

import { DM_DonViModel } from '../dm-don-vi-model/dm-don-vi.model';
import { TokenStorage } from 'app/core/auth/_services/token-storage.service';
import { CommonService } from '../../../services/common.service';
import { TableService } from '../../../../../partials/table/table.service';
import { TableModel } from '../../../../../partials/table';


@Component({
	selector: 'm-dm-don-vi-list',
	templateUrl: './dm-don-vi-list.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
	providers: [DatePipe]
})


export class DM_DonViListComponent implements OnChanges {
	
	@Input() donvi: string = "";
	//TH1: #filter
	filterGroupDataChecked: any = {

	};
	//
	filterText: any = {};
	tmpfilterText: any = {};
	filterGroupDataCheckedFake: any = {};
	filterGroupData: any = {};
	filterGroupArray: any = {};

	haveFilter: boolean = false;

	// Table fields
	dataSourceDV: DM_DonViDataSource;

	displayedColumns = [];


	@ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
	@ViewChild('sort1', { static: true }) sort: MatSort;
	@ViewChild('trigger', { static: true }) _trigger: MatMenuTrigger;

	// Filter fields

	// @ViewChild('searchDonVi', { static: true }) searchDonVi: ElementRef;


	// Selection
	// selectedColumns = new SelectionModel<any>(true, this.availableColumns);
	selection = new SelectionModel<DM_DonViModel>(true, []);
	dm_donvisResult: DM_DonViModel[] = [];
	tmpdm_donvisResult: DM_DonViModel[] = [];

	loadingSubject = new BehaviorSubject<boolean>(false);
	loading$ = this.loadingSubject.asObservable();

	previousIndex: number;
	gridService: TableService;
	girdModel: TableModel = new TableModel();
	fixedPoint = 0;
	// donvi: string = "";
	dataTreeDonVi: any[] = [];
	@Output() ChangeTreDonVi: EventEmitter<any> = new EventEmitter<any>();
	@Output() ChangeListUser: EventEmitter<any> = new EventEmitter<any>();
	rR = {};
	constructor(
		private dm_donvisService: DM_DonViService,
		public dialog: MatDialog,
		private route: ActivatedRoute,
		private router: Router,
		private datePipe: DatePipe,
		private translate: TranslateService,
		private subheaderService: SubheaderService,
		private ref: ApplicationRef,
		private changeDetect: ChangeDetectorRef,
		private tokenStorage: TokenStorage,
		private commonService: CommonService,

		private layoutUtilsService: LayoutUtilsService) { }

	/** LOAD DATA */
	ngOnChanges() {
		this.tokenStorage.getUserRolesObject().subscribe(t => {
			this.rR = t;
		});
		//#region ***Filter***
		this.girdModel = new TableModel();
		this.girdModel.clear();
		this.girdModel.haveFilter = true;
		this.girdModel.tmpfilterText = Object.assign({}, this.girdModel.filterText);
		this.girdModel.filterText['DonVi'] = "";
		this.girdModel.filterText['MaDonvi'] = "";
		this.girdModel.filterText['MaDinhDanh'] = "";
		this.girdModel.filterText['Email'] = "";
		this.girdModel.filterText['SDT'] = "";
		this.girdModel.filterText['DiaChi'] = "";
		
		let optionsTinhTrang = [
			{
				name: 'Khóa',
				value: '1',
			},
			{
				name: 'Hoạt động',
				value: '0',
			}
		];
		this.girdModel.filterGroupDataChecked['Locked'] = optionsTinhTrang.map(x => {
			return {
				name: x.name,
				value: x.value,
				checked: false
			}
		});
		this.girdModel.filterGroupDataCheckedFake = Object.assign({}, this.girdModel.filterGroupDataChecked);

		//#endregion ***Filter***
		let availableColumns = [
			// {
			// 	stt: 1,
			// 	name: 'select',
			// 	displayName: 'Check chọn',
			// 	alwaysChecked: true,
			// 	isShow: true
			// },
			{
				stt: 2,
				name: 'STT',
				displayName: 'STT',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 4,
				name: 'DonVi',
				displayName: 'Tên đơn vị',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 5,
				name: 'MaDonvi',
				displayName: 'Mã đơn vị',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 6,
				name: 'MaDinhDanh',
				displayName: 'Mã định danh',
				alwaysChecked: false,
				isShow: false
			},

			{

				stt: 7,
				name: 'SDT',
				displayName: 'Số điện thoại',
				alwaysChecked: false,
				isShow: false
			},

			{

				stt: 8,
				name: 'Email',
				displayName: 'Email',
				alwaysChecked: false,
				isShow: false
			},
			{

				stt: 9,
				name: 'DiaChi',
				displayName: 'Địa chỉ',
				alwaysChecked: false,
				isShow: false
			},
			// {

			// 	stt: 10,
			// 	name: 'Logo',
			// 	displayName: 'Logo',
			// 	alwaysChecked: false,
			// 	isShow: false
			// },
			{

				stt: 11,
				name: 'Locked',
				displayName: 'Trạng thái',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 12,
				name: 'Priority',
				displayName: 'Thứ tự ưu tiên',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 15,
				name: 'CreatedDate',
				displayName: 'Ngày tạo',
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

		// console.log("availabel column dv", availableColumns);

		this.gridService.showColumnsInTable();
		this.gridService.applySelectedColumns();
		// // If the DM_DonVi changes the sort order, reset back to the first page.
		this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));

		/* Data load will be triggered in two cases:
		- when a pagination event occurs => this.paginator.page
		- when a sort event occurs => this.sort.sortChange
		**/
		merge(this.sort.sortChange, this.paginator.page, this.gridService.result)
			.pipe(
				tap(() => {
					this.loadDM_DonVisList(true);
				})
			)
			.subscribe();
		// Filtration, bind to searchInput
		// fromEvent(this.searchDonVi.nativeElement, 'keyup')
		// 	.pipe(
		// 		debounceTime(150),
		// 		distinctUntilChanged(),
		// 		tap(() => {
		// 			this.paginator.pageIndex = 0;
		// 			this.loadDM_DonVisList();
		// 		})
		// 	)
		// 	.subscribe();

		// // Set title to page breadCrumbs
		this.subheaderService.setTitle('');
		// Init DataSource
		this.dataSourceDV = new DM_DonViDataSource(this.dm_donvisService);
		let queryParams = new QueryParamsModel({});
		// // Read from URL itemId, for restore previous state
		if (this.donvi) {
			this.loadDM_DonVisList();
		} else {
			this.route.queryParams.subscribe(params => {
				if (params.id) {
					queryParams = this.dm_donvisService.lastFilter$.getValue();
				}
				// First load
				this.dataSourceDV.loadDM_DonVis(queryParams);
				setTimeout(x => {
					this.loadPage();
				}, 500)
			});
			
		}
		this.dataSourceDV.entitySubject.subscribe(res => {
			this.dm_donvisResult = res
			this.tmpdm_donvisResult = []
			if (this.dm_donvisResult != null) {
				for (let i = 0; i < this.dm_donvisResult.length; i++) {
					let tmpElement = new DM_DonViModel();
					tmpElement.copy(this.dm_donvisResult[i])
					this.tmpdm_donvisResult.push(tmpElement);
				}
			}
		});
	}
	getListUser(event){
		// console.log("đơn vị", event);
		this.ChangeListUser.emit(event)
	}

	loadDM_DonVisList(holdCurrentPage: boolean = false) {
		this.selection.clear();
		const queryParams = new QueryParamsModel(
			this.filterConfiguration(),
			this.sort.direction,
			this.sort.active,
			holdCurrentPage ? this.paginator.pageIndex : this.paginator.pageIndex = 0,
			this.paginator.pageSize,
			this.gridService.model.filterGroupData
		);
		this.dataSourceDV.loadDM_DonVis(queryParams);
		setTimeout(x => {
			this.loadPage();
		}, 500)
	}
	loadPage() {
		var arrayData = [];
		this.dataSourceDV.entitySubject.subscribe(res => arrayData = res);
		if (arrayData && arrayData.length == 0) {
			var totalRecord = 0;
			this.dataSourceDV.paginatorTotal$.subscribe(tt => totalRecord = tt)
			if (totalRecord > 0) {
				const queryParams = new QueryParamsModel(
					this.filterConfiguration(),
					this.sort.direction,
					this.sort.active,
					this.paginator.pageIndex = this.paginator.pageIndex - 1,
					this.paginator.pageSize,
					this.gridService.model.filterGroupData

				);
				this.dataSourceDV.loadDM_DonVis(queryParams);
			}
			else {
				const queryParams = new QueryParamsModel(
					this.filterConfiguration(),
					this.sort.direction,
					this.sort.active,
					this.paginator.pageIndex = 0,
					this.paginator.pageSize,
					this.gridService.model.filterGroupData

				);
				this.dataSourceDV.loadDM_DonVis(queryParams);
			}
		}
	}

	/** FILTRATION */
	filterConfiguration(): any {
		const filter: any = {};
		if (this.donvi) {
			filter.IdDV = this.donvi
		}
		if (this.gridService.model.filterText) {
			filter.DonVi = this.gridService.model.filterText['DonVi'];
			filter.MaDonvi = this.gridService.model.filterText['MaDonvi'];
			filter.MaDinhDanh = this.gridService.model.filterText['MaDinhDanh'];
			filter.SDT = this.gridService.model.filterText['SDT'];
			filter.Email = this.gridService.model.filterText['Email'];
			filter.DiaChi = this.gridService.model.filterText['DiaChi'];
			// filter.Locked = this.gridService.model.filterText['Locked'];
		}
		// filter.DonVi = this.searchDonVi.nativeElement.value;
		return filter;
	}
	/** ACTIONS */
	/** Delete */
	deleteDM_DonVi(_item: DM_DonViModel) {
		const _title: string = 'Xóa đơn vị';
		const _description: string = `Bạn có chắc muốn xóa ${_item.DonVi} không?`;
		const _waitDesciption: string = 'Đơn vị đang được xóa...';
		const _deleteMessage = `Xóa thành công`;

		const dialogRef = this.layoutUtilsService.deleteElement(_title, _description, _waitDesciption);
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}
			this.dm_donvisService.deleteDM_DonVi(_item.Id).subscribe(res => {
				if (res && res.status === 1) {
					this.layoutUtilsService.showInfo(_deleteMessage);
				}
				else {
					this.layoutUtilsService.showError(res.error.message);
				}
				this.ChangeTreDonVi.emit(res);
				this.loadDM_DonVisList(true);
			});
		});
	}

	deleteDM_DonVis() {
		const _title: string = 'Xóa đơn vị';
		const _description: string = 'Bạn có chắc muốn xóa những đơn vị này không?';
		const _waitDesciption: string = 'Đơn vị đang được xóa...';
		const _deleteMessage = `Xóa thành công`;

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
			this.dm_donvisService.deleteDM_DonVis(idsForDeletion).subscribe(() => {
				this.layoutUtilsService.showInfo(_deleteMessage);
				this.ChangeTreDonVi.emit(res);
				this.loadDM_DonVisList(true);
				this.selection.clear();
			});
		});
	}

	/** SELECTION */
	isAllSelected() {
		const numSelected = this.selection.selected.length;
		const numRows = this.dm_donvisResult.length;
		return numSelected === numRows;
	}

	/** Selects all rows if they are not all selected; otherwise clear selection. */
	masterToggle() {
		if (this.isAllSelected()) {
			this.selection.clear();
		} else {
			this.dm_donvisResult.forEach(row => this.selection.select(row));
		}
	}



	menuChange(e: any, type: 0 | 1 = 0) {
		this.layoutUtilsService.menuSelectColumns_On_Off();
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
				return 'metal';
			case false:
				return 'success';
		}
		return '';
	}

	/**
	 * Show add DM_DonVi dialog
	 */
	addDM_DonVi() {
		const newDM_DonVi = new DM_DonViModel();
		newDM_DonVi.clear(); // Set all defaults fields
		this.editDM_DonVi(newDM_DonVi);
	}
	/**
	 * Show Edit DM_DonVi dialog and save after success close result
	 * @param DM_DonVi: DM_DonViModel
	 */
	editDM_DonVi(DM_DonVi: DM_DonViModel, IsShow:boolean=false) {
		DM_DonVi.IsShow = IsShow;
		const dialogRef = this.dialog.open(DM_DonViEditComponent, { data: { DM_DonVi } });
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}
			this.ChangeTreDonVi.emit(res);
			this.loadDM_DonVisList(true);
		});
	}

	ViewDM_DonVi(item) {
		this.editDM_DonVi(item, true);
	}

	ValidateChangeNumberEvent(columnName: string, item: any, event: any) {
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


	ImportExcel() {
		const dialogRef = this.dialog.open(DM_DonViImportComponent);
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}
			this.loadDM_DonVisList();
		});
	}

	xuatExcel() {
		let filter = this.filterConfiguration();

		var linkdownload = environment.ApiRoot + `/don-viundefined?LoaiDonVi=${filter.LoaiDonVi}&DonVi=${filter.DonVi}&MaDonvi=${filter.MaDonvi}&MaDinhDanh=${filter.MaDinhDanh}&Parent=${filter.Parent}&SDT=${filter.SDT}&Email=${filter.Email}&DiaChi=${filter.DiaChi}&Logo=${filter.Logo}&Locked=${filter.Locked}&Priority=${filter.Priority}&DangKyLichLanhDao=${filter.DangKyLichLanhDao}&KhongCoVanThu=${filter.KhongCoVanThu}&CreatedDate=${filter.CreatedDate}&UpdatedBy=${filter.UpdatedBy}&UpdatedDate=${filter.UpdatedDate}&Disabled=${filter.Disabled}`;
		window.open(linkdownload);
	}

	In() {
		this.layoutUtilsService.showError('Tạm thời chưa in được đâu nhé!');
		return;
	}

	getFormatDate(v: string = '') {
		if (v != '') {
			return v.includes('T') ? v.replace(/(\d{4})(-)(\d{2})(-)(\d{2})(T)(\d{2})(:)(\d{2})(:)(\d{2}).*$/g, "$5/$3/$1") : v.replace(/(\d{4})(-)(\d{2})(-)(\d{2})/g, "$5/$3/$1");
		}
		return '';
	}


	//#filter Filter function
	SelectAll(event, col) {
		if (event.checked) {
			this.filterGroupDataCheckedFake[col].forEach(element => {
				element.checked = true;
			});
		}
		else {
			this.filterGroupDataCheckedFake[col].forEach(element => {
				element.checked = false;
			});
		}
	}

	isColumnSelectAll(col) {
		if (this.filterGroupDataCheckedFake[col]) {
			let lst_checked = this.filterGroupDataCheckedFake[col].filter(opt => opt.checked).map(opt => opt.value);
			if (lst_checked) {
				if (lst_checked.length != this.filterGroupDataCheckedFake[col].length) {
					return false;
				}
				else {
					return true;
				}
			}
			else {
				return false;
			}
		}
		return false;
	}

	LoadDataDropFilter(col) {
		this.tmpfilterText = Object.assign({}, this.filterText);
		if (this.filterGroupDataChecked && this.filterGroupDataCheckedFake && this.filterGroupDataCheckedFake[col] && this.filterGroupDataChecked[col]) {
			this.filterGroupDataCheckedFake[col] = this.filterGroupDataChecked[col].map(x => Object.assign({}, x));
		}
	}
	filterHead(col) {
		if (this.filterGroupDataCheckedFake && this.filterGroupDataCheckedFake[col]) {
			this.filterGroupDataChecked[col] = this.filterGroupDataCheckedFake[col].map(x => Object.assign({}, x));
			this.filterGroupData[col] = [];
			this.filterGroupArray[col] = [];
			this.filterGroupArray[col] = this.filterGroupDataCheckedFake[col]
				.filter(opt => opt.checked);
			this.filterGroupData[col] = this.filterGroupDataCheckedFake[col]
				.filter(opt => opt.checked).map(op => op.value);
		}
		this.filterText = Object.assign({}, this.tmpfilterText);
		if (this.filterText) {
			this.filterConfiguration();
		}
		this.loadDM_DonVisList();
	}
	remove(item, col) {
		let _index = this.filterGroupDataChecked[col].findIndex(x => x.value == item.value);
		if (_index > -1) {
			this.filterGroupDataChecked[col][_index].checked = false;
			this.filterGroupData[col] = [];
			this.filterGroupArray[col] = [];
			this.filterGroupArray[col] = this.filterGroupDataChecked[col]
				.filter(opt => opt.checked);
			this.filterGroupData[col] = this.filterGroupDataChecked[col]
				.filter(opt => opt.checked).map(op => op.value);
			this.loadDM_DonVisList();
		}
	}
	removeText(col) {
		this.filterText[col] = "";
		this.loadDM_DonVisList();
	}
	Clear() {
		this.filterGroupData = [];
		this.filterGroupArray = [];
		for (var key in this.filterGroupDataChecked) {
			if (this.filterGroupDataChecked.hasOwnProperty(key)) {
				if (this.filterGroupDataChecked[key]) {
					this.filterGroupDataChecked[key].forEach(element => {
						element.checked = false;
					});
				}
			}
		}
		for (var key in this.filterText) {
			if (this.filterText.hasOwnProperty(key)) {
				this.filterText[key] = "";
			}
		}
		this.loadDM_DonVisList();
		this.changeDetect.detectChanges();
	}
	checkFilterHasValue(data, col) {
		let _count = 0;
		if (col == 'ALL' && data) {//check all column
			for (var key in data) {
				if (data.hasOwnProperty(key)) {
					if (data[key]) {
						let _arrflt = data[key].filter(opt => opt.checked);
						_count += _arrflt ? _arrflt.length : 0;
					}
				}
			}
			let _text = "";
			for (var key in this.filterText) {
				if (this.filterText.hasOwnProperty(key)) {
					_text += this.filterText[key];
				}
			}
			if (_count > 0 || _text) {
				return true;
			}
		}
		else {//check in 1 column
			if (data && data[col]) {
				let _arrflt = data[col].filter(opt => opt.checked);
				_count += _arrflt ? _arrflt.length : 0;
			}
			if (_count > 0) {
				return true;
			}
		}
		return false;
	}
	
	
}
