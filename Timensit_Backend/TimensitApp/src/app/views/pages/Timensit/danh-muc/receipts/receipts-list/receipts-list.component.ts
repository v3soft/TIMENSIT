import { Component, OnInit, ElementRef, ViewChild, ChangeDetectionStrategy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
// Material
import { MatPaginator, MatSort, MatDialog } from '@angular/material';
import { SelectionModel } from '@angular/cdk/collections';
// RXJS
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { fromEvent, merge } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
// Services
import { receiptsEditDialogComponent } from '../receipts-edit/receipts-edit.dialog.component';
import { receiptsDataSource } from '../Model/data-sources/receipts.datasource';
import { receiptsModel } from '../Model/receipts.model';
import { receiptsService } from '../Services/receipts.service';
import { CommonService } from '../../../services/common.service';
import { LayoutUtilsService, QueryParamsModel } from '../../../../../../core/_base/crud';
@Component({
    selector: 'm-receipts-list',
    templateUrl: './receipts-list.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush
})

export class receiptsListComponent implements OnInit {
    // Table fields
    dataSource: receiptsDataSource;
    displayedColumns = ['STT',   'ContractCode', 'Amount', 'ReceiptDate', 'EffectiveDate', 'Type',  'actions'];
    @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
    @ViewChild(MatSort, { static: true }) sort: MatSort;
    selection = new SelectionModel<receiptsModel>(true, []);
    productsResult: receiptsModel[] = [];
    showTruyCapNhanh: boolean = true;
    filter: any = {};
    _name = "";
    list_button: boolean;
    constructor(public receiptsService1: receiptsService,
        private danhMucService: CommonService,
        public dialog: MatDialog,
        private route: ActivatedRoute,
        private layoutUtilsService: LayoutUtilsService,
        private translate: TranslateService) {
        this._name = this.translate.instant("RECEIPTS.NAME");
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

        // Filtration, bind to searchInput
        //fromEvent(this.searchInput.nativeElement, 'keyup')
        //    .pipe(
        //        debounceTime(150),
        //        distinctUntilChanged(),
        //        tap(() => {
        //            this.paginator.pageIndex = 0;
        //            this.loadDataList();
        //        })
        //    )
        //    .subscribe();
        // Init DataSource
        this.dataSource = new receiptsDataSource(this.receiptsService1);
        let queryParams = new QueryParamsModel({});

        // Read from URL itemId, for restore previous state
        this.route.queryParams.subscribe(params => {
            if (params.id) {
                queryParams = this.receiptsService1.lastFilter$.getValue();
                this.restoreState(queryParams, +params.id);
            }
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
            this.paginator.pageSize
        );
        this.dataSource.loadList(queryParams);
    }
    filterConfiguration(): any {

        //const filter: any = {};
        //if (this.filterchucdanh && this.filterchucdanh.length > 0) {
        //    filter.Id_CV = +this.filterchucdanh;
        //}
        //const searchText: string = this.searchInput.nativeElement.value;
        //filter.keyword = searchText;
        //return filter;
        return this.filter;
    }

    /** Delete */
    DeleteWorkplace(_item: receiptsModel) {
        const _title = this.translate.instant('OBJECT.DELETE.TITLE', { name: this._name.toLowerCase() });
        const _description = this.translate.instant('OBJECT.DELETE.DESCRIPTION', { name: this._name.toLowerCase() });
        const _waitDesciption = this.translate.instant('OBJECT.DELETE.WAIT_DESCRIPTION', { name: this._name.toLowerCase() });
        const _deleteMessage = this.translate.instant('OBJECT.DELETE.MESSAGE', { name: this._name });

        const dialogRef = this.layoutUtilsService.deleteElement(_title, _description, _waitDesciption);
        dialogRef.afterClosed().subscribe(res => {
            if (!res) {
                return;
            }

            this.receiptsService1.deleteItem(_item.Id_row).subscribe(res => {
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
    AddWorkplace() {
        const receiptsModels = new receiptsModel();
        receiptsModels.clear(); // Set all defaults fields
        this.Editreceipts(receiptsModels);
    }
    restoreState(queryParams: QueryParamsModel, id: number) {
        if (id > 0) {
        }

        if (!queryParams.filter) {
            return;
        }
    }
    Editreceipts(_item: receiptsModel,allowEdit: boolean = true) {
        let saveMessageTranslateParam = '';
        saveMessageTranslateParam += _item.Id_row > 0 ? 'OBJECT.EDIT.UPDATE_MESSAGE' : 'OBJECT.EDIT.ADD_MESSAGE';
        const _saveMessage = this.translate.instant(saveMessageTranslateParam, { name: this._name });
        const dialogRef = this.dialog.open(receiptsEditDialogComponent, { data: { _item,allowEdit },maxWidth: '40vw' });
        dialogRef.afterClosed().subscribe(res => {
            if (!res) {
                this.loadDataList();
            }
            else {
                this.layoutUtilsService.showInfo(_saveMessage);
                this.loadDataList();
            }

        });
    }
}
