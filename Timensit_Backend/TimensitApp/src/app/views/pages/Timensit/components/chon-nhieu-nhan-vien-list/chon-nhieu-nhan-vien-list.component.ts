//Cores
import { Component, OnInit, ViewChild, ElementRef, ChangeDetectorRef, Inject } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
// Materials
import { MatDialogRef, MatPaginator, MatSort, MAT_DIALOG_DATA } from '@angular/material';
// Services
// Models
//Datasources
// RXJS
import { merge, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { QueryParamsModel, LayoutUtilsService } from 'app/core/_base/crud';
import { SelectionModel } from '@angular/cdk/collections';
import { ChonNhieuNhanVienListModel } from './chon-nhieu-nhan-vien-list.model';
import { ChonNhieuNhanVienListDataSource } from './chon-nhieu-nhan-vien-list.datasource';
import { NguoiDungDPSService } from '../../nguoi-dung/nguoi-dung-dps/nguoi-dung-dps-service/nguoi-dung-dps.service';
import { CommonService } from '../../services/common.service';


@Component({
	selector: 'm-chon-nhieu-nhan-vien-list',
	templateUrl: './chon-nhieu-nhan-vien-list.component.html',
})
export class ChonNhieuNhanVienListComponent implements OnInit {
	item: ChonNhieuNhanVienListModel;
	dataSource: ChonNhieuNhanVienListDataSource;
	displayedColumns = [];
	availableColumns = [
		{
			stt: 1,
			name: 'select',
			alwaysChecked: true,
		},
		{
			stt: 2,
			name: 'MaNV',
			alwaysChecked: false,
		},
		{
			stt: 3,
			name: 'HoTen',
			alwaysChecked: false,
		},
		{
			stt: 6,
			name: 'NgaySinh',
			alwaysChecked: false,
		},
		{
			stt: 7,
			name: 'Structure',
			alwaysChecked: false,
		},
		{
			stt: 9,
			name: 'TenChucVu',
			alwaysChecked: false,
		},

	];
	selectedColumns = new SelectionModel<any>(true, this.availableColumns);
	hasFormErrors: boolean = false;
	viewLoading: boolean = false;
	loadingAfterSubmit: boolean = false;
	//===================================Khai báo dữ liệu===================
	listDonVi: any[] = [];
	listPhongBan: any[] = [];
	listBoPhan: any[] = [];
	listChucDanh: any[] = [];
	listChucVu: any[] = [];
	// listDiaDiemLamViec: any[] = [];
	id_dv: string = '';
	id_pb: string = '';
	id_cd: string = '';
	id_to: string = '';
	id_cv: string = '';
	// id_dd: string = '';

	id_nv: string = '';
	//=======================================================================
	// Filter fields
	@ViewChild('searchInput', { static: true }) searchInput: ElementRef;
	@ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
	@ViewChild(MatSort, { static: true }) sort: MatSort;

	selection = new SelectionModel<ChonNhieuNhanVienListModel>(true, []);
	productsResult: ChonNhieuNhanVienListModel[] = [];

	public datatree: BehaviorSubject<any[]> = new BehaviorSubject([]);
	title: string = '';
	ID_Struct: string = '';
	disabledBtn: boolean = false;
	selected: any[] = [];
	constructor(public dialogRef: MatDialogRef<ChonNhieuNhanVienListComponent>,
		private service: CommonService,
		private changeDetectorRefs: ChangeDetectorRef,
		@Inject(MAT_DIALOG_DATA) public data: any,
		private layoutUtilsService: LayoutUtilsService) { }

	/** LOAD DATA */
	ngOnInit() {
		if (this.data.selected)
			this.selected = this.data.selected;
		this.title = "Chọn cơ cấu tổ chức";

		this.getTreeValue();

		this.item = new ChonNhieuNhanVienListModel();

		this.applySelectedColumns();
		// If the user changes the sort order, reset back to the first page.
		this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));

		/* Data load will be triggered in two cases:
		- when a pagination event occurs => this.paginator.page
		- when a sort event occurs => this.sort.sortChange
		**/
		merge(this.sort.sortChange, this.paginator.page)
			.pipe(
				tap(() => {
					this.selection.clear();
					this.LoadDataList(true);
				})
			)
			.subscribe();

		// Init DataSource
		this.dataSource = new ChonNhieuNhanVienListDataSource(this.service, this.layoutUtilsService);

		//Load unit list

		this.id_nv = this.data.idnv;
		this.LoadDataList();
		this.dataSource.entitySubject.subscribe(res => {
			this.productsResult = res;
			this.productsResult.forEach(x => {
				if (this.selected.findIndex(nv => nv.ID_NV == x.ID_NV) >= 0)
					this.selection.select(x);
			})
		});
	}

	getTreeValue() {
		this.service.Get_CoCauToChuc().subscribe(res => {
			if (res.data && res.data.length > 0) {
				this.datatree.next(res.data);
			}
		});
		// this.danhMucService.GetListWorkplaceByBranch("0").subscribe(res => {
		// 	this.listDiaDiemLamViec = res.data;
		// 	this.changeDetectorRefs.detectChanges();
		// })
	}

	GetValueNode(val: any) {
		this.ID_Struct = val.id;
		this.service.GetListPositionbyStructure(this.ID_Struct).subscribe(res => {
			this.listChucDanh = res.data;
			if (this.listChucDanh.length > 0) {
				this.id_cd = '';
				this.service.GetListJobtitleByStructure(this.id_cd, this.ID_Struct).subscribe(res => {
					this.listChucVu = res.data;
					this.id_cv = '';
					if (this.listChucVu.length > 0) {
						this.changeDetectorRefs.detectChanges();
					}
					else {
						this.changeDetectorRefs.detectChanges();
					}

				});
			}
			else {
				this.id_cd = '';
				this.id_cv = '';
				this.listChucVu = [];
				this.changeDetectorRefs.detectChanges();
			}
		});
	}
	//=============================================================================
	loadListChucVu() {
		this.service.GetListBranchbyCustomerID().subscribe(res => {
			this.listDonVi = res.data;
			this.id_dv = "";
			this.changeDetectorRefs.detectChanges();
			this.service.GetListDepartmentbyBranch(this.id_dv).subscribe(res => {
				this.listPhongBan = res.data;
				this.id_pb = "";
				this.changeDetectorRefs.detectChanges();
				this.service.GetListTeamByDepartment(this.id_pb).subscribe(res => {
					this.listBoPhan = res.data;
					this.changeDetectorRefs.detectChanges();
				});
				this.service.GetListPositionbyDepartment(this.id_pb).subscribe(res => {
					this.listChucDanh = res.data;
					this.service.GetListJobtitleByPosition(this.id_cd, this.id_pb).subscribe(res => {
						this.listChucVu = res.data;
						this.changeDetectorRefs.detectChanges();
					});
				});
			});

		});
	}

	loadDonViChange(iddonvi: any) {
		this.service.GetListDepartmentbyBranch(iddonvi).subscribe(res => {
			this.listPhongBan = res.data;
			if (this.listPhongBan.length > 0) {
				this.id_pb = '';
				this.changeDetectorRefs.detectChanges();
				this.service.GetListTeamByDepartment(this.id_pb).subscribe(res => {
					this.listBoPhan = res.data;
					if (this.listBoPhan.length > 0) {
					}
					this.changeDetectorRefs.detectChanges();
				});
				this.service.GetListPositionbyDepartment(this.id_pb).subscribe(res => {
					this.listChucDanh = res.data;
					if (this.listChucDanh.length > 0) {
						this.id_cd = '';
						this.service.GetListJobtitleByPosition(this.id_cd, this.id_pb).subscribe(res => {
							this.listChucVu = res.data;
							if (this.listChucVu.length > 0) {
								this.changeDetectorRefs.detectChanges();
							}
							else {
								this.changeDetectorRefs.detectChanges();
							}

						});
					}
					else {
						this.listChucVu = [];
						this.changeDetectorRefs.detectChanges();
					}
				});
			}
			else {
				this.listBoPhan = [];
				this.listChucDanh = [];
				this.listChucVu = [];
			}
		});
		// this.danhMucService.GetListWorkplaceByBranch("0").subscribe(res => {
		// 	this.listDiaDiemLamViec = res.data;
		// 	if (this.listDiaDiemLamViec.length > 0) {
		// 		this.changeDetectorRefs.detectChanges();
		// 	}
		// 	else {
		// 		this.changeDetectorRefs.detectChanges();
		// 	}

		// })
	}

	loadPhongBanChange(idpb: any) {
		this.service.GetListTeamByDepartment(idpb).subscribe(res => {
			this.listBoPhan = res.data;
			if (this.listBoPhan.length > 0) {
				this.changeDetectorRefs.detectChanges();
			}
			else {
				this.changeDetectorRefs.detectChanges();
			}

		});
		this.service.GetListPositionbyDepartment(idpb).subscribe(res => {
			this.listChucDanh = res.data;
			if (this.listChucDanh.length > 0) {
				this.id_cd = '';
				this.service.GetListJobtitleByPosition(this.id_cd, this.id_pb).subscribe(res => {
					this.listChucVu = res.data;
					if (this.listChucVu.length > 0) {
						this.changeDetectorRefs.detectChanges();
					}
					else {
						this.changeDetectorRefs.detectChanges();
					}

				});
			}
			else {
				this.listChucVu = [];
				this.changeDetectorRefs.detectChanges();
			}
		});
	}

	loadChucDanhChange(idcd: any) {
		let id_st = this.ID_Struct;
		this.service.GetListJobtitleByStructure(idcd, id_st).subscribe(res => {
			this.listChucVu = res.data;
			this.id_cv = "";
			if (this.listChucVu.length > 0) {
				this.changeDetectorRefs.detectChanges();
			}
			else {
				this.changeDetectorRefs.detectChanges();
			}
		});
	}
	//=============================================================================
	/** FILTRATION */
	filterConfiguration(): any {
		const filter: any = {};
		const searchText: string = this.searchInput.nativeElement.value;
		filter.StructureID = '' + this.ID_Struct;

		if (this.id_cd && this.id_cd.length > 0) {
			filter.IDChucDanh = +this.id_cd;
		}

		// if (this.id_dd && this.id_dd.length > 0) {
		// 	filter.IDDiaDiemLamViec = +this.id_dd;
		// }

		if (this.id_cv && this.id_cv.length > 0) {
			filter.IDChucVu = +this.id_cv;
		}

		if (this.id_nv != "" && this.id_nv != undefined) {
			filter.ID_NhanVien = this.id_nv;
		}

		filter.keyword = searchText;
		return filter;
	}

	/** ACTIONS */
	LoadDataList(page: boolean = false) {
		this.selection.clear();
		const queryParams = new QueryParamsModel(
			this.filterConfiguration(),
			this.sort.direction,
			this.sort.active,
			page ? this.paginator.pageIndex : this.paginator.pageIndex = 0,
			this.paginator.pageSize
		);
		this.dataSource.loadList_Emp(queryParams);
	}

	//chọn cán bộ
	selectItems(_item: ChonNhieuNhanVienListModel) {
		this.dialogRef.close({
			_item
		});
	}

	onAlertClose($event) {
		this.hasFormErrors = false;
	}

	close() {
		this.dialogRef.close();
	}
	selectRow($event, row) {
		if ($event) {
			this.selection.toggle(row);
			let index = this.selected.findIndex(x => x.ID_NV == row.ID_NV);
			if ($event.checked && index == -1)
				this.selected.push(row);
			if (!$event.checked && index >= 0)
				this.selected.splice(index, 1);
		}
	}
	/** SELECTION */
	isAllSelected() {
		const numSelected = this.selection.selected.length;
		const numRows = this.productsResult.length;
		return numSelected === numRows;
	}

	/** Selects all rows if they are not all selected; otherwise clear selection. */
	masterToggle() {
		if (this.isAllSelected()) {
			this.selection.clear();
			this.productsResult.forEach(row => {
				let index = this.selected.findIndex(x => x.ID_NV == row.ID_NV);
				if (index >= 0)
					this.selected.splice(index, 1);
			});
		} else {
			this.productsResult.forEach(row => {
				this.selection.select(row);
				let index = this.selected.findIndex(x => x.ID_NV == row.ID_NV);
				if (index == -1)
					this.selected.push(row);
			});
		}
	}

	IsAllColumnsChecked() {
		const numSelected = this.selectedColumns.selected.length;
		const numRows = this.availableColumns.length;
		return numSelected === numRows;
	}
	CheckAllColumns() {
		if (this.IsAllColumnsChecked()) {
			this.availableColumns.forEach(row => { if (!row.alwaysChecked) this.selectedColumns.deselect(row); });
		} else {
			this.availableColumns.forEach(row => this.selectedColumns.select(row));
		}
	}

	applySelectedColumns() {
		const _selectedColumns: string[] = [];
		this.selectedColumns.selected.sort((a, b) => { return a.stt > b.stt ? 1 : 0; }).forEach(col => { _selectedColumns.push(col.name) });
		this.displayedColumns = _selectedColumns;
	}

	goBack(id = 0) {

		if (this.selection.selected.length > 0 && id == 1)
			this.dialogRef.close({ done: true, nhanVienSelected: this.selected });
		else
			this.dialogRef.close({ done: false, nhanVienSelected: [] });
	}

	luuNhanVien() {
		this.goBack(1);
	}
}
