import { of } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';
import { BaseDataSource, QueryParamsModel, QueryResultsModel, MessageType, LayoutUtilsService } from 'app/core/_base/crud';
import { CommonService } from '../../services/common.service';

export class ChonNhieuNhanVienListDataSource extends BaseDataSource {
	constructor(private service: CommonService,
		private layoutUtilsService: LayoutUtilsService) {
		super();
	}

	loadList_Emp(queryParams: QueryParamsModel) {
		this.service.lastFilter$.next(queryParams);
		this.loadingSubject.next(true);
		this.service.findData_Emp(queryParams)
			.pipe(
				tap(resultFromServer => {
					if (resultFromServer && resultFromServer.status == 1) {
						this.entitySubject.next(resultFromServer.data);
						var totalCount = resultFromServer.page.TotalCount || (resultFromServer.page.AllPage * resultFromServer.page.Size);
						this.paginatorTotalSubject.next(totalCount);
					} else {
						this.entitySubject.next([]);
						this.paginatorTotalSubject.next(0);
						this.layoutUtilsService.showError(resultFromServer.error.message);
					}
				}),
				catchError(err => of(new QueryResultsModel([], err))),
				finalize(() => this.loadingSubject.next(false))
			).subscribe();
	}

}
