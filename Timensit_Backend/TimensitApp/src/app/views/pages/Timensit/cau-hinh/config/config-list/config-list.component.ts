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
import { ConfigDataSource } from '../config-datasource/config.datasource';
//Service
import { ConfigService } from '../config-service/config.service';
import { CommonService } from '../../../services/common.service';
import { ConfigEditComponent } from '../config-edit/config-edit.component';
import { TokenStorage } from '../../../../../../core/auth/_services/token-storage.service';
//Model

import { SysConfigModel } from '../config-model/config.model';
import { TableService } from '../../../../../partials/table/table.service';
import { TableModel } from '../../../../../partials/table';
import { LayoutUtilsService, QueryParamsModel } from '../../../../../../core/_base/crud';


@Component({
	selector: 'm-config-list',
	templateUrl: './config-list.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
	providers: [DatePipe]
})
export class ConfigListComponent implements OnInit, OnDestroy {

	haveFilter: boolean = false;

	// Table fields
	dataSource: ConfigDataSource;
	@ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
	@ViewChild('sort1', { static: true }) sort: MatSort;
	@ViewChild('trigger', { static: true }) _trigger: MatMenuTrigger;

	// Filter fields
	IdGroup: number = 0;
	// Selection
	selection = new SelectionModel<SysConfigModel>(true, []);
	configsResult: SysConfigModel[] = [];
	tmpconfigsResult: SysConfigModel[] = [];

	loadingSubject = new BehaviorSubject<boolean>(false);
	loading$ = this.loadingSubject.asObservable();
	rR = {};
	gridService: TableService;
	girdModel: TableModel = new TableModel();
	list_button: boolean;
	constructor(
		private configsService: ConfigService,
		public dialog: MatDialog,
		private route: ActivatedRoute,
		private router: Router,
		private datePipe: DatePipe,
		private translate: TranslateService,
		private changeDetect: ChangeDetectorRef,
		private layoutUtilsService: LayoutUtilsService,
		private tokenStorage: TokenStorage,
		private ref: ApplicationRef,
		private commonService: CommonService) { }

	/** LOAD DATA */
	ngOnInit() {
		this.list_button = CommonService.list_button();
		this.tokenStorage.getUserRolesObject().subscribe(t => {
			this.rR = t;
		});
		//#region ***Filter***
		this.girdModel.haveFilter = true;
		this.girdModel.tmpfilterText = Object.assign({}, this.girdModel.filterText);
		this.girdModel.filterText['Code'] = "";
		this.girdModel.filterText['Value'] = "";
		this.girdModel.filterText['Description'] = "";
		//TH1: #filter

		this.girdModel.filterGroupDataChecked = {};

		this.girdModel.filterGroupDataCheckedFake = Object.assign({}, this.girdModel.filterGroupDataChecked);

		this.configsService.configGroup().subscribe(res => {
			if (res && res.status == 1) {
				this.gridService.model.filterGroupDataChecked['IdGroup'] = res.data.map(x => {
					return {
						name: x.title,
						value: x.id,
						checked: false
					}
				});
				this.gridService.model.filterGroupDataCheckedFake = Object.assign({}, this.gridService.model.filterGroupDataChecked);
			}
			else
				this.layoutUtilsService.showError(res.error.message);
		});
		//#endregion ***Filter***

		//#region ***Drag Drop***
		let availableColumns = [
			{
				stt: 2,
				name: 'STT',
				displayName: 'STT',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 3,
				name: 'Code',
				displayName: 'Mã',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 4,
				name: 'Value',
				displayName: 'Giá trị',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 5,
				name: 'IdGroup',
				displayName: 'Nhóm cấu hình',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 6,
				name: 'Priority',
				displayName: 'Thứ tự',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 6,
				name: 'Description',
				displayName: 'Mô tả',
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

		// // If the Config changes the sort order, reset back to the first page.
		this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));

		/* Data load will be triggered in two cases:
		- when a pagination event occurs => this.paginator.page
		- when a sort event occurs => this.sort.sortChange
		**/
		merge(this.sort.sortChange, this.paginator.page, this.gridService.result)
			.pipe(
				tap(() => {
					this.loadConfigsList(true);
				})
			)
			.subscribe();
		// Init DataSource
		this.dataSource = new ConfigDataSource(this.configsService);
		let queryParams = new QueryParamsModel({});
		// // Read from URL itemId, for restore previous state
		this.route.queryParams.subscribe(params => {
			queryParams = this.configsService.lastFilter$.getValue();
			// First load
			this.dataSource.loadConfigs(queryParams);
		});
		this.dataSource.entitySubject.subscribe(res => {
			this.configsResult = res
			this.tmpconfigsResult = []
			if (this.configsResult != null) {
				if (this.configsResult.length == 0 && this.paginator.pageIndex > 0) {
					this.loadConfigsList();
				} else {
					for (let i = 0; i < this.configsResult.length; i++) {
						let tmpElement = new SysConfigModel();
						tmpElement.copy(this.configsResult[i])
						this.tmpconfigsResult.push(tmpElement);
					}
				}
			}
		});
	}

	ngOnDestroy() {
		this.gridService.Clear();
	}

	loadConfigsList(holdCurrentPage: boolean = false) {
		this.selection.clear();
		const queryParams = new QueryParamsModel(
			this.filterConfiguration(),
			this.sort.direction,
			this.sort.active,
			holdCurrentPage ? this.paginator.pageIndex : this.paginator.pageIndex = 0,
			this.paginator.pageSize,

			this.gridService.model.filterGroupData

		);
		this.dataSource.loadConfigs(queryParams);
	}


	/** FILTRATION */
	filterConfiguration(): any {
		const filter: any = {};
		//#filter
		if (this.gridService.model.filterText) {
			filter.Code = this.gridService.model.filterText['Code'];
			filter.Value = this.gridService.model.filterText['Value'];
			filter.Description = this.gridService.model.filterText['Description'];
		}

		return filter;
	}
	/** SELECTION */
	isAllSelected() {
		const numSelected = this.selection.selected.length;
		const numRows = this.configsResult.length;
		return numSelected === numRows;
	}

	/** Selects all rows if they are not all selected; otherwise clear selection. */
	masterToggle() {
		if (this.isAllSelected()) {
			this.selection.clear();
		} else {
			this.configsResult.forEach(row => this.selection.select(row));
		}
	}
	/**
	 * Show Edit Config dialog and save after success close result
	 * @param Config: SysConfigModel
	 */
	editConfig(Config: SysConfigModel, allowEdit: boolean = true) {
		const dialogRef = this.dialog.open(ConfigEditComponent, { data: { Config: Config, allowEdit: allowEdit } });
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}

			this.loadConfigsList(true);
		});
	}
}
