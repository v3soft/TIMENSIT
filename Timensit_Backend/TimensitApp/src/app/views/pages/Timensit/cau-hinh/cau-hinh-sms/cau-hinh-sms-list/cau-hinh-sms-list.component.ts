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
import { CauHinhSMSDataSource } from '../cau-hinh-sms-datasource/cau-hinh-sms.datasource';
//Service
import { CauHinhSMSService } from '../cau-hinh-sms-service/cau-hinh-sms.service';
import { CommonService } from '../../../services/common.service';
import { LayoutUtilsService, QueryParamsModel, MessageType } from '../../../../../../core/_base/crud';
import { CauHinhSMSEditComponent } from '../cau-hinh-sms-edit/cau-hinh-sms-edit.component';
//Model

import { CauHinhSMSModel } from '../cau-hinh-sms-model/cau-hinh-sms.model';
import { TableService } from '../../../../../partials/table/table.service';
import { TableModel } from '../../../../../partials/table';
import { SubheaderService } from '../../../../../../core/_base/layout';


@Component({
	selector: 'm-cau-hinh-sms-list',
	templateUrl: './cau-hinh-sms-list.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
	providers: [DatePipe]
})

export class CauHinhSMSListComponent implements OnInit, OnDestroy {

	haveFilter: boolean = false;

	// Table fields
	dataSource: CauHinhSMSDataSource;
	@ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
	@ViewChild('sort1', { static: true }) sort: MatSort;
	@ViewChild('trigger', { static: true }) _trigger: MatMenuTrigger;

	// Filter fields
	// Selection
	selection = new SelectionModel<CauHinhSMSModel>(true, []);
	CauHinhSMSsResult: CauHinhSMSModel[] = [];
	tmpCauHinhSMSsResult: CauHinhSMSModel[] = [];

	loadingSubject = new BehaviorSubject<boolean>(false);
	loading$ = this.loadingSubject.asObservable();

	BatDau_tungay: string = '';
	BatDau_denngay: string = '';
	KetThuc_tungay: string = '';
	KetThuc_denngay: string = '';

	IdDonVi:string='';
	public datatreeDonVi: BehaviorSubject<any[]> = new BehaviorSubject([]);

	gridService: TableService;
	girdModel: TableModel = new TableModel();
	list_button: boolean;
	constructor(
		private CauHinhSMSsService: CauHinhSMSService,
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
		//#region ***Filter***
		this.getTreeDonVi();

		this.girdModel.haveFilter = true;
		this.girdModel.tmpfilterText = Object.assign({}, this.girdModel.filterText);
		this.girdModel.filterText['Brandname'] = "";
		this.girdModel.filterText['URL'] = "";
		this.girdModel.filterText['UserName'] = "";
		this.girdModel.disableButtonFilter['Locked'] = true;
		//TH1: #filter

		this.girdModel.filterGroupDataChecked = {
			"Locked": [

				{
					name: "Ho???t ?????ng",
					value: false,
					checked: false
				},

				{
					name: "Kh??a",
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
				stt: 2,
				name: 'STT',
				displayName: 'STT',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 3,
				name: 'TenDonVi',
				displayName: 'T??n ????n v???',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 4,
				name: 'URL',
				displayName: 'URL',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 5,
				name: 'Brandname',
				displayName: 'Brandname',
				alwaysChecked: false,
				isShow: true
			},

			{

				stt: 6,
				name: 'UserName',
				displayName: 'UserName',
				alwaysChecked: false,
				isShow: true
			},

			{

				stt: 7,
				name: 'Locked',
				displayName: 'T??nh tr???ng',
				alwaysChecked: false,
				isShow: true
			},

			{
				stt: 99,
				name: 'actions',
				displayName: 'Thao t??c',
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

		// // If the CauHinhSMS changes the sort order, reset back to the first page.
		this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));

		/* Data load will be triggered in two cases:
		- when a pagination event occurs => this.paginator.page
		- when a sort event occurs => this.sort.sortChange
		**/
		merge(this.sort.sortChange, this.paginator.page, this.gridService.result)
			.pipe(
				tap(() => {
					this.loadCauHinhSMSsList(true);
				})
			)
			.subscribe();

		// // Set title to page breadCrumbs
		this.subheaderService.setTitle('');
		// Init DataSource
		this.dataSource = new CauHinhSMSDataSource(this.CauHinhSMSsService);
		let queryParams = new QueryParamsModel({});
		// // Read from URL itemId, for restore previous state
		this.route.queryParams.subscribe(params => {
			queryParams = this.CauHinhSMSsService.lastFilter$.getValue();
			// First load
			this.dataSource.loadCauHinhSMSs(queryParams);
		});
		this.dataSource.entitySubject.subscribe(res => {
			this.CauHinhSMSsResult = res
			this.tmpCauHinhSMSsResult = []
			if (this.CauHinhSMSsResult != null) {
				if (this.CauHinhSMSsResult.length == 0 && this.paginator.pageIndex > 0) {
					this.loadCauHinhSMSsList();
				} else {
					for (let i = 0; i < this.CauHinhSMSsResult.length; i++) {
						let tmpElement = new CauHinhSMSModel();
						tmpElement.copy(this.CauHinhSMSsResult[i])
						this.tmpCauHinhSMSsResult.push(tmpElement);
					}
				}
			}
		});
	}


	ngOnDestroy() {
		this.gridService.Clear();
	}
	getTreeDonVi() {
		this.commonService.TreeDonVi().subscribe(res => {
			if (res && res.status == 1) {
				this.datatreeDonVi.next(res.data);
			}
			else {
				this.datatreeDonVi.next([]);
				this.layoutUtilsService.showError(res.error.message);
			}
		})
	}

	loadCauHinhSMSsList(holdCurrentPage: boolean = false) {
		this.selection.clear();
		const queryParams = new QueryParamsModel(
			this.filterConfiguration(),
			this.sort.direction,
			this.sort.active,
			holdCurrentPage ? this.paginator.pageIndex : this.paginator.pageIndex = 0,
			this.paginator.pageSize,

			this.gridService.model.filterGroupData

		);
		this.dataSource.loadCauHinhSMSs(queryParams);
	}

	DateChanged(value: any, ind: number) {
		let date = value.targetElement.value.replace(/-/g, '/').split('T')[0].split('/');
		if (+date[0] < 10 && date[0].length < 2)
			date[0] = '0' + date[0];
		if (+date[1] < 10 && date[1].length < 2)
			date[1] = '0' + date[1];

		if (ind == 1) {
			this.BatDau_tungay = date[2] + '-' + date[1] + '-' + date[0];
		}
		if (ind == 2) {
			this.BatDau_denngay = date[2] + '-' + date[1] + '-' + date[0];
		}
		if (ind == 3) {
			this.KetThuc_tungay = date[2] + '-' + date[1] + '-' + date[0];
		}
		if (ind == 4) {
			this.KetThuc_denngay = date[2] + '-' + date[1] + '-' + date[0];
		}

		this.loadCauHinhSMSsList();
	}

	/** FILTRATION */
	filterConfiguration(): any {
		const filter: any = {};

		//#filter
		if (this.gridService.model.filterText) {
			filter.Brandname = this.gridService.model.filterText['Brandname'];
			filter.URL = this.gridService.model.filterText['URL'];
			filter.UserName = this.gridService.model.filterText['UserName'];
		}

		filter.IdDonVi=this.IdDonVi;

		return filter;
	}
	/** ACTIONS */
	/** Delete */
	delete(_item: CauHinhSMSModel) {
		const _title: string = 'X??c nh???n';
		const _description: string = 'B???n ch???c ch???n x??a c???u h??nh sms?';
		const _waitDesciption: string = 'C???u h??nh sms ??ang ???????c x??a...';
		const _deleteMessage = `X??a th??nh c??ng`;

		const dialogRef = this.layoutUtilsService.deleteElement(_title, _description, _waitDesciption);
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}
			this.CauHinhSMSsService.deleteCauHinhSMS(_item.Id).subscribe(res => {
				if (res && res.status === 1) {
					this.layoutUtilsService.showInfo(_deleteMessage);
				}
				else {
					this.layoutUtilsService.showError(res.error.message);
				}
				this.loadCauHinhSMSsList(true);
			});
		});
	}

	deleteCauHinhSMSs() {
		const _title: string = 'X??a danh m???c kh??c';
		const _description: string = 'B???n c?? ch???c mu???n x??a nh???ng danh m???c kh??c n??y kh??ng?';
		const _waitDesciption: string = 'Danh m???c kh??c ??ang ???????c x??a...';
		const _deleteMessage = `Danh m???c kh??c ???? ???????c x??a`;

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
			this.CauHinhSMSsService.deleteCauHinhSMSs(idsForDeletion).subscribe(() => {
				this.layoutUtilsService.showInfo(_deleteMessage);
				this.loadCauHinhSMSsList(true);
				this.selection.clear();
			});
		});
	}




	/** SELECTION */
	isAllSelected() {
		const numSelected = this.selection.selected.length;
		const numRows = this.CauHinhSMSsResult.length;
		return numSelected === numRows;
	}

	/** Selects all rows if they are not all selected; otherwise clear selection. */
	masterToggle() {
		if (this.isAllSelected()) {
			this.selection.clear();
		} else {
			this.CauHinhSMSsResult.forEach(row => this.selection.select(row));
		}
	}
	/* UI */
	getItemStatusString(status: boolean = false): string {
		switch (status) {
			case true:
				return 'Kh??a';
			case false:
				return 'Ho???t ?????ng';
		}
	}

	getItemCssClassByStatus(status: boolean = false): string {
		switch (status) {
			case true:
				return 'metal';
			case false:
				return 'success';
		}
	}

	/**
	 * Show add CauHinhSMS dialog
	 */
	add() {
		const newCauHinhSMS = new CauHinhSMSModel();
		newCauHinhSMS.clear(); // Set all defaults fields
		this.edit(newCauHinhSMS);
	}
	/**
	 * Show Edit CauHinhSMS dialog and save after success close result
	 * @param CauHinhSMS: CauHinhSMSModel
	 */
	edit(CauHinhSMS: any,View:boolean=false) {
		CauHinhSMS.View=View;
		const dialogRef = this.dialog.open(CauHinhSMSEditComponent, { data: { CauHinhSMS } });
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}

			this.loadCauHinhSMSsList(true);
		});
	}

	DonViChanged(e:any){
		//console.log('e',e)
		this.IdDonVi=e.id;
		this.loadCauHinhSMSsList();
	}

	LockAndUnLock(item:any) {
		this.CauHinhSMSsService.LockNUnLock(item.Id,item.Locked).subscribe(res=>{
			if(res && res.status==1){
				this.layoutUtilsService.showInfo(item.Locked?'M??? kh??a th??nh c??ng':'Kh??a th??nh c??ng');
			}
			else{
				this.layoutUtilsService.showError(res.error.message);
			}
			this.loadCauHinhSMSsList(true);

		})
	}

}
