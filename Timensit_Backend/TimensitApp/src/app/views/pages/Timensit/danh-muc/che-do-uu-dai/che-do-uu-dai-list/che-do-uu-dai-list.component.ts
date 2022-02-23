import { Component, OnInit, ElementRef, ViewChild, ChangeDetectionStrategy, ApplicationRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
// Material
import { MatPaginator, MatSort, MatDialog } from '@angular/material';
import { SelectionModel } from '@angular/cdk/collections';
// RXJS
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { BehaviorSubject, fromEvent, merge } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
// Services
import { chedouudaiEditDialogComponent } from '../che-do-uu-dai-edit/che-do-uu-dai-edit.dialog.component';
import { chedouudaiDataSource } from '../Model/data-sources/che-do-uu-daidatasource';
import { chedouudaiModel } from '../Model/che-do-uu-dai.model';
import { chedouudaiService } from '../Services/che-do-uu-dai.service';
import { CommonService } from '../../../services/common.service';
import { LayoutUtilsService, QueryParamsModel } from '../../../../../../core/_base/crud';
import { TableModel} from '../../../../../partials/table';
import { TableService } from '../../../../../partials/table/table.service';
import { ValueConverter } from '@angular/compiler/src/render3/view/template';
// import { TableModel, TableService } from '../../../partials/table';

@Component({
    selector: 'm-che-do-uu-dai-list',
    templateUrl: './che-do-uu-dai-list.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush
})

export class chedouudaiListComponent implements OnInit {
    // Table fields
    dataSource: chedouudaiDataSource;
    displayedColumns = ['Id', 'CheDoUuDai', 'MoTa', 'Locked', 'Priority', 'CreatedBy', 'CreatedDate', 'UpdatedBy', 'UpdatedDate', 'actions'];
	@ViewChild(MatPaginator, {static:true}) paginator: MatPaginator;
	@ViewChild('sort1', { static: true }) sort: MatSort;
    //@ViewChild('searchInput') searchInput: ElementRef;
    //filterchucdanh: string = '';
    // Filter fields
    //listchucdanh: any[] = [];
    filterStatus = '';
	filterCondition = '';
    // Selection
    selection = new SelectionModel<chedouudaiModel>(true, []);
    productsResult: chedouudaiModel[] = [];
    showTruyCapNhanh: boolean = true;
	// filter: any = {};
    _name = "";
    
    gridModel: TableModel;
    gridService: TableService;
    
	constructor(public chedouudaiService: chedouudaiService,
		private danhMucService: CommonService,
        public dialog: MatDialog,
        private route: ActivatedRoute,
        private ref: ApplicationRef,
        private layoutUtilsService: LayoutUtilsService,
        private translate: TranslateService) 
    {
        this._name = this.translate.instant("CHE_DO_UU_DAI.NAME");
    }

    /** LOAD DATA */
    ngOnInit() {
        if (this.chedouudaiService !== undefined) {
			this.chedouudaiService.lastFilter$ = new BehaviorSubject(new QueryParamsModel({}, 'asc', 'Priority', 0, 10));
        } //mặc định theo priority
        
        this.gridModel = new TableModel();
		this.gridModel.clear();
		this.gridModel.haveFilter = true;
		this.gridModel.tmpfilterText = Object.assign({}, this.gridModel.filterText);
        this.gridModel.filterText['CheDoUuDai'] = "";
        this.gridModel.filterText['MoTa'] = "";

        this.gridModel.disableButtonFilter['Locked'] = true;

        let optionsTinhTrang = [
			{
				name: 'Đã khóa',
				value: 'True', //ko in hoa ko nhận
			},
			{
				name: 'Hoạt động',
				value: 'False',
			}
		];

        this.gridModel.filterGroupDataChecked['Locked'] = optionsTinhTrang.map(x => {
			return {
				name: x.name,
				value: x.value,
				checked: false
			}
        });
        
        this.gridModel.filterGroupDataCheckedFake = Object.assign({}, this.gridModel.filterGroupDataChecked);

        let availableColumns = [
			{

				stt: 1,
				name: 'STT',
				displayName: 'STT',
				alwaysChecked: false,
				isShow: true
            },
            {

				stt: 2,
				name: 'CheDoUuDai',
				displayName: 'Chế độ ưu đãi',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 3,
				name: 'MoTa',
				displayName: 'Mô tả',
				alwaysChecked: false,
				isShow: true
			},
			{

				stt: 4,
				name: 'Locked',
				displayName: 'Locked',
				alwaysChecked: false,
				isShow: true
            },
            {

				stt: 5,
				name: 'Priority',
				displayName: 'Priority',
				alwaysChecked: false,
				isShow: true
            },
            {

				stt: 6,
				name: 'CreatedBy',
				displayName: 'Người tạo',
				alwaysChecked: false,
				isShow: false
            },
            {

				stt: 7,
				name: 'CreatedDate',
				displayName: 'Ngày tạo',
				alwaysChecked: false,
				isShow: true
            },
            {

				stt: 8,
				name: 'UpdatedBy',
				displayName: 'Người sửa',
				alwaysChecked: false,
				isShow: false
            },
            {

				stt: 9,
				name: 'UpdatedDate',
				displayName: 'Ngày sửa',
				alwaysChecked: false,
				isShow: false
            },
			{
				stt: 99,
				name: 'actions',
				displayName: 'actions',
				alwaysChecked: true,
				isShow: true
			}
		];
		this.gridModel.availableColumns = availableColumns.sort((a, b) => a.stt - b.stt);
		this.gridModel.selectedColumns = new SelectionModel<any>(true, this.gridModel.availableColumns)


		this.gridService = new TableService(this.layoutUtilsService, this.ref, this.gridModel);

		this.gridService.showColumnsInTable();
		this.gridService.applySelectedColumns();

        // If the user changes the sort order, reset back to the first page.
        this.sort.sortChange.subscribe(() => (this.paginator.pageIndex = 0));

        merge(this.sort.sortChange, this.paginator.page, this.gridService.result)
            .pipe(
                tap(() => {
                    this.loadDataList();
                })
            )
            .subscribe();

        // Init DataSource
        this.dataSource = new chedouudaiDataSource(this.chedouudaiService);
        let queryParams = new QueryParamsModel({});

        // Read from URL itemId, for restore previous state
        this.route.queryParams.subscribe(params => {
            queryParams = this.chedouudaiService.lastFilter$.getValue();
            // First load
            this.dataSource.loadList(queryParams);
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
			holdCurrentPage ? this.paginator.pageIndex : this.paginator.pageIndex = 0,
            this.paginator.pageSize,
            this.gridService.model.filterGroupData  //phải có mới filter theo group
        );
        this.dataSource.loadList(queryParams);
	}

    filterConfiguration(): any {

        const filter: any = {};
        if (this.filterStatus && this.filterStatus.length > 0) {
			filter.status = +this.filterStatus;
		}

		if (this.filterCondition && this.filterCondition.length > 0) {
            filter.type = +this.filterCondition;
        }

        if (this.gridService.model.filterText) 
            filter.CheDoUuDai = this.gridService.model.filterText['CheDoUuDai'];
            filter.MoTa = this.gridService.model.filterText['MoTa'];

        return filter; //trả về đúng biến filter
    }

    /** Delete */
    DeleteWorkplace(_item: chedouudaiModel) {
		const _title = this.translate.instant('OBJECT.DELETE.TITLE', { name: this._name.toLowerCase() });
		const _description = this.translate.instant('OBJECT.DELETE.DESCRIPTION', { name: this._name.toLowerCase() });
		const _waitDesciption = this.translate.instant('OBJECT.DELETE.WAIT_DESCRIPTION', { name: this._name.toLowerCase() });
		const _deleteMessage = this.translate.instant('OBJECT.DELETE.MESSAGE', { name: this._name });

        const dialogRef = this.layoutUtilsService.deleteElement(_title, _description, _waitDesciption);
        dialogRef.afterClosed().subscribe(res => {
            if (!res) {
                return;
            }

            this.chedouudaiService.deleteItem(_item.Id).subscribe(res => {
                if (res && res.status === 1) {
					this.layoutUtilsService.showInfo(_deleteMessage);
                }
				else {
					this.layoutUtilsService.showError(res.error.message);
                }
                this.loadDataList();
            });
        });
    }

    //Khóa
    BlockWorkplace(_item: chedouudaiModel) {
        let _description = '';
        let _waitDesciption = '';
        let _title = '';

        if(_item.Locked == false) { 
            _description = 'Bạn có chắc chắn muốn khóa chế độ ưu đãi này không ??';
            _waitDesciption = 'Đang cập nhật ...';
            _title = 'Khóa chế độ ưu đãi';
            _item.Locked = true;
        }
        else {
            _description = 'Bạn có chắc chắn muốn mở khóa chế độ ưu đãi này không ??';
            _waitDesciption = 'Đang cập nhật ...';
            _title = 'Mở khóa chế độ ưu đãi';
            _item.Locked = false;
        }

        const dialogRef = this.layoutUtilsService.deleteElement(_title, _description, _waitDesciption); //thay đổi titile là ra confirm khác
        dialogRef.afterClosed().subscribe(res => {
            if (!res) {
                this.loadDataList(); //để không biến mất ổ khóa
                return;
            }
		    this.chedouudaiService.updateCheDo(_item).subscribe(res => {
                if (res && res.status === 1) {
                    const _messageType = this.translate.instant('OBJECT.EDIT.UPDATE_MESSAGE', { name: this._name });
					this.layoutUtilsService.showInfo(_messageType);
                }
				else {
					this.layoutUtilsService.showError(res.error.message);
                }
                this.loadDataList();
            });
        });

    }

    // lock(_item: any, islock = true) {
	// 	let _message = (islock ? "Khóa" : "Mở khóa") + " người dùng thành công";
	// 	this.nguoidungdpssService.lock(_item.UserID, islock).subscribe(res => {
	// 		if (res && res.status === 1) {
	// 			this.layoutUtilsService.showInfo(_message);
	// 		}
	// 		else {
	// 			this.layoutUtilsService.showError(res.error.message);
	// 		}
	// 		this.loadNguoiDungDPSsList(true);
	// 	});
	// }

    AddWorkplace() {
        const dungcuchinhhinhModels = new chedouudaiModel();
        dungcuchinhhinhModels.clear(); // Set all defaults fields
        this.EditCheDo(dungcuchinhhinhModels);
    }

    restoreState(queryParams: QueryParamsModel, id: number) {
        if (id > 0) {
        }

        if (!queryParams.filter) {
            return;
        }
    }

    EditCheDo(_item: chedouudaiModel, allowEdit: boolean = true) {
        let saveMessageTranslateParam = '';
        saveMessageTranslateParam += _item.Id > 0 ?  'OBJECT.EDIT.UPDATE_MESSAGE' : 'OBJECT.EDIT.ADD_MESSAGE'; 
        //thông báo khi thực hiện trong tác vụ
        const _saveMessage = this.translate.instant(saveMessageTranslateParam, {name:this._name});
        const dialogRef = this.dialog.open(chedouudaiEditDialogComponent, { data: { _item, allowEdit} });
        dialogRef.afterClosed().subscribe(res => {
            if (!res) {
            }
			else {
				this.layoutUtilsService.showInfo(_saveMessage);
                this.loadDataList();
            }

        });
    }
    getHeight(): any {
		let obj = window.location.href.split("/").find(x => x == "tabs-references");
		if (obj) {
			let tmp_height = 0;
			tmp_height = window.innerHeight - 354;
			return tmp_height + 'px';
		} else {
			let tmp_height = 0;
			tmp_height = window.innerHeight - 236;
			return tmp_height + 'px';
		}
    }


    //phục vụ CSS
    covertLockButton(lock:boolean): string {
        switch(lock){
            case true: 
                return 'lock_open';
            case false:
                return 'lock'
        }
    }

    covertToolTip(lock:boolean): string {
        switch(lock){
            case true: 
                return 'COMMON.UNBLOCK';
            case false:
                return 'COMMON.BLOCK';
        }
    }

    covertLockToString(lock:boolean): string {
        switch(lock){
            case true: 
                return 'Đã khóa';
            case false:
                return 'Hoạt động';
        }
    }

    covertLockToColor(lock:boolean): string {
		switch (lock) {
            case false:
				return 'kt-badge--success';
			case true:
				return 'kt-badge--metal';
		}
		return '';
	}
}
