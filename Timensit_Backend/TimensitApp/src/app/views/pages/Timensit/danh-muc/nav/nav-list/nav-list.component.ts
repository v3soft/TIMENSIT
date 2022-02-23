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
import { navEditDialogComponent } from '../nav-edit/nav-edit.dialog.component';
import { navDataSource } from '../Model/data-sources/nav.datasource';
import { navModel } from '../Model/nav.model';
import { navService } from '../Services/nav.service';
import { CommonService } from '../../../services/common.service';
import { LayoutUtilsService, QueryParamsModel } from '../../../../../../core/_base/crud';
@Component({
    selector: 'm-nav-list',
    templateUrl: './nav-list.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush
})

export class navListComponent implements OnInit {
    // Table fields
    dataSource: navDataSource;
    displayedColumns = ['STT', 'ContractCode', 'Amount', 'RecordDate', 'Type',  'Investor_Name', 'actions'];
    @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
    @ViewChild(MatSort, { static: true }) sort: MatSort;
    selection = new SelectionModel<navModel>(true, []);
    productsResult: navModel[] = [];
    showTruyCapNhanh: boolean = true;
    filter: any = {};
    _name = "";
    list_button: boolean;
    constructor(public navService1: navService,
        private danhMucService: CommonService,
        public dialog: MatDialog,
        private route: ActivatedRoute,
        private layoutUtilsService: LayoutUtilsService,
        private translate: TranslateService) {
        this._name = this.translate.instant("NAV.NAME");
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
        this.dataSource = new navDataSource(this.navService1);
        let queryParams = new QueryParamsModel({});

        // Read from URL itemId, for restore previous state
        this.route.queryParams.subscribe(params => {
            if (params.id) {
                queryParams = this.navService1.lastFilter$.getValue();
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
    DeleteWorkplace(_item: navModel) {
        const _title = this.translate.instant('OBJECT.DELETE.TITLE', { name: this._name.toLowerCase() });
        const _description = this.translate.instant('OBJECT.DELETE.DESCRIPTION', { name: this._name.toLowerCase() });
        const _waitDesciption = this.translate.instant('OBJECT.DELETE.WAIT_DESCRIPTION', { name: this._name.toLowerCase() });
        const _deleteMessage = this.translate.instant('OBJECT.DELETE.MESSAGE', { name: this._name });

        const dialogRef = this.layoutUtilsService.deleteElement(_title, _description, _waitDesciption);
        dialogRef.afterClosed().subscribe(res => {
            if (!res) {
                return;
            }

            this.navService1.deleteItem(_item.Id_row).subscribe(res => {
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
        const navModels = new navModel();
        navModels.clear(); // Set all defaults fields
        this.Editnav(navModels);
    }
    restoreState(queryParams: QueryParamsModel, id: number) {
        if (id > 0) {
        }

        if (!queryParams.filter) {
            return;
        }
    }
    Editnav(_item: navModel,allowEdit: boolean = true) {
        let saveMessageTranslateParam = '';
        saveMessageTranslateParam += _item.Id_row > 0 ? 'OBJECT.EDIT.UPDATE_MESSAGE' : 'OBJECT.EDIT.ADD_MESSAGE';
        const _saveMessage = this.translate.instant(saveMessageTranslateParam, { name: this._name });
        const dialogRef = this.dialog.open(navEditDialogComponent, { data:  { _item,allowEdit },maxWidth: '40vw' });
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
