import { of } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';
import { receiptsService } from '../../Services/receipts.service';
import { BaseDataSource, QueryParamsModel, QueryResultsModel } from '../../../../../../../core/_base/crud';

export class receiptsDataSource extends BaseDataSource {
	constructor(private receiptsService: receiptsService) {
		super();
	}

	loadList(queryParams: QueryParamsModel) {
		this.receiptsService.lastFilter$.next(queryParams);
		this.loadingSubject.next(true);
		this.receiptsService.findData(queryParams)
			.pipe(
				tap(resultFromServer => {
					this.entitySubject.next(resultFromServer.data);
					var totalCount = resultFromServer.page.TotalCount || (resultFromServer.page.AllPage * resultFromServer.page.Size);
					this.paginatorTotalSubject.next(totalCount);
				}),
				catchError(err => of(new QueryResultsModel([], err))),
				finalize(() => this.loadingSubject.next(false))
			).subscribe(res => {
				this.receiptsService.ReadOnlyControl = res.Visible;
			});
	}
}
