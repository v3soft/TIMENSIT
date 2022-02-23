import { Component, OnInit, ElementRef, ViewChild, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
// Material
import { MatPaginator, MatSort, MatDialog } from '@angular/material';
import { SelectionModel } from '@angular/cdk/collections';
// RXJS
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { fromEvent, merge } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { ChucDanhEditDialogComponent } from '../chucdanh-edit/chucdanh-edit.dialog.component';
import { ChucDanhComponent } from '../chucdanh.component';
import { ChucDanhDataSource } from '../model/data-sources/chucdanh.datasource';
import { ChucDanhModel } from '../model/chucdanh.model';
import { ChucDanhService } from '../services/chucdanh.service';
import { CommonService } from '../../../services/common.service';
import { LayoutUtilsService, QueryParamsModel } from '../../../../../../core/_base/crud';
import { TokenStorage } from '../../../../../../core/auth/_services/token-storage.service';

@Component({
	selector: 'm-chucdanh-list',
	templateUrl: './chucdanh-list.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush
})

export class ChucDanhListComponent implements OnInit {
	// Table fields
	dataSource: ChucDanhDataSource;
	displayedColumns = ['STT', 'Id_CV', 'MaCV', 'TenCV', 'Cap'/*, 'IsManager'*/, 'NguoiCapNhat', 'NgayCapNhat', 'actions'];
	@ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
	@ViewChild(MatSort, { static: true }) sort: MatSort;
	// Filter fields
	listDonVi: any[] = [];
	// Selection
	selection = new SelectionModel<ChucDanhModel>(true, []);
	productsResult: ChucDanhModel[] = [];
	showTruyCapNhanh: boolean = true;
	id_menu: number = 7;
	//=================PageSize Table=====================
	pageSize: number;
	_name = "";
	list_button: boolean;
	constructor(public ChucDanhServices: ChucDanhService,
		private danhMucService: CommonService,
		public dialog: MatDialog,
		private route: ActivatedRoute,
		private layoutUtilsService: LayoutUtilsService,
		private translate: TranslateService,
		private tokenStorage: TokenStorage,) {
		this._name = this.translate.instant("CHUC_DANH.NAME");
	}

	/** LOAD DATA */
	ngOnInit() {
		this.list_button = CommonService.list_button();
		// If the user changes the sort order, reset back to the first page.
		this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));

		/* Data load will be triggered in two cases:
		- when a pagination event occurs => this.paginator.page
		- when a sort event occurs => this.sort.sortChange
		**/
		merge(this.sort.sortChange, this.paginator.page)
			.pipe(
				tap(() => {
					this.loadDataList();
				})
			)
			.subscribe();

		// Set title to page breadCrumbs
		// Init DataSource
		this.dataSource = new ChucDanhDataSource(this.ChucDanhServices);
		let queryParams = new QueryParamsModel({});
		// Read from URL itemId, for restore previous state
		this.route.queryParams.subscribe(params => {
			if (params.id) {
				queryParams = this.ChucDanhServices.lastFilter$.getValue();
				this.restoreState(queryParams, +params.id);
			}
			// First load
			this.loadDataList();
		});
		this.dataSource.entitySubject.subscribe(res => {
			this.productsResult = res;
			if (this.productsResult != null) {
				if (this.productsResult.length == 0 && this.paginator.pageIndex > 0) {
					this.loadDataList(false);
				}
			}
		});
	}

	loadDataList(holdCurrentPage: boolean = true) {
		const queryParams = new QueryParamsModel(
			this.filterConfiguration(),
			this.sort.direction,
			this.sort.active,
			this.paginator.pageIndex,
			this.paginator.pageSize
		);
		this.dataSource.loadList(queryParams);
	}

	filterConfiguration(): any {
		const filter: any = {};
		return filter;
	}
	/** Delete */
	DeletePosition(_item: ChucDanhModel) {
		const _title = this.translate.instant('OBJECT.DELETE.TITLE', { name: this._name.toLowerCase() });
		const _description = this.translate.instant('OBJECT.DELETE.DESCRIPTION', { name: this._name.toLowerCase() });
		const _waitDesciption = this.translate.instant('OBJECT.DELETE.WAIT_DESCRIPTION', { name: this._name.toLowerCase() });
		const _deleteMessage = this.translate.instant('OBJECT.DELETE.MESSAGE', { name: this._name });
		const dialogRef = this.layoutUtilsService.deleteElement(_title, _description, _waitDesciption);
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}

			this.ChucDanhServices.deleteItem(_item.Id_CV).subscribe(res => {
				if (res && res.status === 1) {
					this.layoutUtilsService.showInfo(_deleteMessage);
					this.loadDataList();
				}
				else {
					this.layoutUtilsService.showError(res.error.message);
				}
			});
		});
	}
	AddPosition() {
		const ChucDanhModels = new ChucDanhModel();
		ChucDanhModels.clear(); // Set all defaults fields
		this.EditPosition(ChucDanhModels);
	}
	restoreState(queryParams: QueryParamsModel, id: number) {
		if (id > 0) {
		}

		if (!queryParams.filter) {
			return;
		}
	}
	EditPosition(_item: ChucDanhModel, allowEdit: boolean = true) {
		let saveMessageTranslateParam = '';
		saveMessageTranslateParam += _item.Id_CV > 0 ? 'OBJECT.EDIT.UPDATE_MESSAGE' : 'OBJECT.EDIT.ADD_MESSAGE';
		const _saveMessage = this.translate.instant(saveMessageTranslateParam, { name: this._name });
		const dialogRef = this.dialog.open(ChucDanhEditDialogComponent, { data: { _item, allowEdit } });
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				this.loadDataList();
			}
			else {
				this.layoutUtilsService.showError(_saveMessage);
				this.loadDataList();
			}
		});
	}
}
