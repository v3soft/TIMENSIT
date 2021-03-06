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
import { SMSHistoryDataSource } from '../sms-history-datasource/sms-history.datasource';
//Service
import { SMSHistoryService } from '../sms-history-service/sms-history.service';
import { CommonService } from '../../../services/common.service';
import { SubheaderService } from 'app/core/_base/layout';
import { LayoutUtilsService, QueryParamsModel, MessageType } from 'app/core/_base/crud';
import { SMSHistoryEditComponent } from '../sms-history-edit/sms-history-edit.component';
import { environment } from 'environments/environment';
//Model

import { SMSHistoryModel } from '../sms-history-model/sms-history.model';
import { TableService } from '../../../../../partials/table/table.service';
import { TableModel } from '../../../../../partials/table';


@Component({
	selector: 'm-sms-history-list',
	templateUrl: './sms-history-list.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
	providers: [DatePipe]
})

export class SMSHistoryListComponent implements OnInit, OnDestroy {

	haveFilter: boolean = false;

	// Table fields
	dataSource: SMSHistoryDataSource;
	@ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
	@ViewChild('sort1', { static: true }) sort: MatSort;
	@ViewChild('trigger', { static: true }) _trigger: MatMenuTrigger;

	// Filter fields
	// Selection
	selection = new SelectionModel<any>(true, []);
	SMSHistorysResult: any[] = [];
	tmpSMSHistorysResult: any[] = [];

	loadingSubject = new BehaviorSubject<boolean>(false);
	loading$ = this.loadingSubject.asObservable();

	BatDau_tungay: string = '';
	BatDau_denngay: string = '';
	KetThuc_tungay: string = '';
	KetThuc_denngay: string = '';

	IdDonVi: string = '';
	Loai: string = '0';
	public datatreeDonVi: BehaviorSubject<any[]> = new BehaviorSubject([]);

	gridService: TableService;
	girdModel: TableModel = new TableModel();
	constructor(
		private SMSHistorysService: SMSHistoryService,
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
		//#region ***Filter***
		this.getTreeDonVi();

		this.girdModel.haveFilter = true;
		this.girdModel.tmpfilterText = Object.assign({}, this.girdModel.filterText);
		this.girdModel.filterText['Brandname'] = "";
		this.girdModel.filterText['SDT'] = "";
		this.girdModel.filterText['Username'] = "";
		this.girdModel.filterText['Message'] = "";
		this.girdModel.disableButtonFilter['Locked'] = true;
		//TH1: #filter

		this.girdModel.filterGroupDataChecked = {
			"TrangThai": [
				{
					name: "Th???t b???i",
					value: 0,
					checked: false
				},

				{
					name: "Th??nh c??ng",
					value: 1,
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
				displayName: 'Ch???n',
				alwaysChecked: false,
				isShow: true
			},
			{
				stt: 2,
				name: 'STT',
				displayName: 'STT',
				alwaysChecked: false,
				isShow: true
			},
			// {

			// 	stt: 3,
			// 	name: 'SDT',
			// 	displayName: 'S??? ??i???n tho???i',
			// 	alwaysChecked: false,
			// 	isShow: true
			// },
			{

				stt: 4,
				name: 'Message',
				displayName: 'Tin nh???n',
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
				name: 'Username',
				displayName: 'Username',
				alwaysChecked: false,
				isShow: true
			},

			// {

			// 	stt: 7,
			// 	name: 'TrangThai',
			// 	displayName: 'Tr???ng th??i',
			// 	alwaysChecked: false,
			// 	isShow: true
			// },

			{

				stt: 8,
				name: 'Loai',
				displayName: 'Lo???i tin nh???n',
				alwaysChecked: false,
				isShow: true
			},

			{

				stt: 9,
				name: 'CreatedDate',
				displayName: 'Th???i gian',
				alwaysChecked: false,
				isShow: true
			},

			{

				stt: 10,
				name: 'ThanhCong',
				displayName: 'Th??nh c??ng',
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

		// // If the SMSHistory changes the sort order, reset back to the first page.
		this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));

		/* Data load will be triggered in two cases:
		- when a pagination event occurs => this.paginator.page
		- when a sort event occurs => this.sort.sortChange
		**/
		merge(this.sort.sortChange, this.paginator.page, this.gridService.result)
			.pipe(
				tap(() => {
					this.loadSMSHistorysList(true);
				})
			)
			.subscribe();

		// // Set title to page breadCrumbs
		this.subheaderService.setTitle('');
		// Init DataSource
		this.dataSource = new SMSHistoryDataSource(this.SMSHistorysService);
		let queryParams = new QueryParamsModel({});
		// // Read from URL itemId, for restore previous state
		this.route.queryParams.subscribe(params => {
			queryParams = this.SMSHistorysService.lastFilter$.getValue();
			// First load
			this.dataSource.loadSMSHistorys(queryParams);
		});
		this.dataSource.entitySubject.subscribe(res => {
			this.SMSHistorysResult = res
			this.tmpSMSHistorysResult = []
			if (this.SMSHistorysResult != null) {
				if (this.SMSHistorysResult.length == 0 && this.paginator.pageIndex > 0) {
					this.loadSMSHistorysList();
				} else {
					for (let i = 0; i < this.SMSHistorysResult.length; i++) {
						let tmpElement = new SMSHistoryModel();
						tmpElement.copy(this.SMSHistorysResult[i])
						this.tmpSMSHistorysResult.push(tmpElement);
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

	loadSMSHistorysList(holdCurrentPage: boolean = false) {
		this.selection.clear();
		const queryParams = new QueryParamsModel(
			this.filterConfiguration(),
			this.sort.direction,
			this.sort.active,
			holdCurrentPage ? this.paginator.pageIndex : this.paginator.pageIndex = 0,
			this.paginator.pageSize,

			this.gridService.model.filterGroupData

		);
		this.dataSource.loadSMSHistorys(queryParams);
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
		if (value.targetElement.value == '') {
			if (ind == 1) {
				this.BatDau_tungay = '';
			}
			if (ind == 2) {
				this.BatDau_denngay = '';
			}
		}
		else {

			if (ind == 1) {
				var re = /^(\d{4})(\/|\-)(\d{2})(\/|\-)(\d{2})$/;
				if (!this.BatDau_tungay.match(re)) {
					this.layoutUtilsService.showInfo("Th???i gian t??? ng??y kh??ng ????ng");
					return;
				}
			}
			if (ind == 2) {
				var re = /^(\d{4})(\/|\-)(\d{2})(\/|\-)(\d{2})$/;
				if (!this.BatDau_denngay.match(re)) {
					this.layoutUtilsService.showInfo("Th???i gian ?????n ng??y kh??ng ????ng");
					return;
				}
			}
		}

		this.loadSMSHistorysList();
	}

	/** FILTRATION */
	filterConfiguration(): any {
		const filter: any = {};

		//#filter
		if (this.gridService.model.filterText) {
			filter.Brandname = this.gridService.model.filterText['Brandname'];
			filter.SDT = this.gridService.model.filterText['SDT'];
			filter.Username = this.gridService.model.filterText['Username'];
			filter.Message = this.gridService.model.filterText['Message'];
		}

		if (this.BatDau_tungay != '') {
			filter.tungay = this.BatDau_tungay;
		}
		if (this.BatDau_denngay != '') {
			filter.denngay = this.BatDau_denngay;
		}

		filter.IdDonVi = this.IdDonVi;
		filter.Loai = this.Loai;

		return filter;
	}
	/** ACTIONS */
	/** Delete */
	delete(_item: any) {
		const _title: string = 'X??c nh???n';
		const _description: string = 'B???n ch???c ch???n x??a l???ch s??? sms?';
		const _waitDesciption: string = 'L???ch s??? sms ??ang ???????c x??a...';
		const _deleteMessage = `X??a th??nh c??ng`;

		const dialogRef = this.layoutUtilsService.deleteElement(_title, _description, _waitDesciption);
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}
			this.SMSHistorysService.deleteSMSHistory(_item.IdSMS).subscribe(res => {
				if (res && res.status === 1) {
					this.layoutUtilsService.showInfo(_deleteMessage);
				}
				else {
					this.layoutUtilsService.showError(res.error.message);
				}
				this.loadSMSHistorysList(true);
			});
		});
	}

	deleteSMSHistorys() {
		const _title: string = 'X??c nh???n';
		const _description: string = 'B???n ch???c ch???n x??a l???ch s??? sms?';
		const _waitDesciption: string = 'L???ch s??? sms ??ang ???????c x??a...';
		const _deleteMessage = `X??a th??nh c??ng`;

		const dialogRef = this.layoutUtilsService.deleteElement(_title, _description, _waitDesciption);
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}

			const idsForDeletion: number[] = [];
			for (let i = 0; i < this.selection.selected.length; i++) {
				idsForDeletion.push(

					this.selection.selected[i].IdSMS

				);
			}
			this.SMSHistorysService.deleteSMSHistorys(idsForDeletion).subscribe(() => {
				
				this.layoutUtilsService.showInfo(_deleteMessage);
				this.loadSMSHistorysList(true);
				this.selection.clear();
			});
		});
	}




	/** SELECTION */
	isAllSelected() {
		const numSelected = this.selection.selected.length;
		const numRows = this.SMSHistorysResult.length;
		return numSelected === numRows;
	}

	/** Selects all rows if they are not all selected; otherwise clear selection. */
	masterToggle() {
		if (this.isAllSelected()) {
			this.selection.clear();
		} else {
			this.SMSHistorysResult.forEach(row => this.selection.select(row));
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
	 * Show add SMSHistory dialog
	 */
	add() {
		const newSMSHistory = new SMSHistoryModel();
		newSMSHistory.clear(); // Set all defaults fields
		this.edit(newSMSHistory);
	}
	/**
	 * Show Edit SMSHistory dialog and save after success close result
	 * @param SMSHistory: SMSHistoryModel
	 */
	edit(SMSHistory: any, View: boolean = false) {
		SMSHistory.View=View;
		const dialogRef = this.dialog.open(SMSHistoryEditComponent, { data: { SMSHistory } });
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}

			this.loadSMSHistorysList(true);
		});
	}

	DonViChanged(e: any) {
		console.log('e', e)
		this.IdDonVi = e.id;
		this.loadSMSHistorysList();
	}

	LockAndUnLock(item: any) {
		this.SMSHistorysService.LockNUnLock(item.Id, item.Locked).subscribe(res => {
			if (res && res.status == 1) {
				this.layoutUtilsService.showInfo(item.Locked ? 'M??? kh??a th??nh c??ng' : 'Kh??a th??nh c??ng');
			}
			else {
				this.layoutUtilsService.showError(res.error.message);
			}
			this.loadSMSHistorysList(true);

		})
	}

}
