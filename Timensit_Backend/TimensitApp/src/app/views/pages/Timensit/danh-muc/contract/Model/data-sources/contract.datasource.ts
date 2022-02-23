import { of } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';
import { contractService } from '../../Services/contract.service';
import { BaseDataSource, QueryParamsModel, QueryResultsModel } from '../../../../../../../core/_base/crud';

export class contractDataSource extends BaseDataSource {
	constructor(private contractService: contractService) {
		super();
	}

	loadList(queryParams: QueryParamsModel) {
		this.contractService.lastFilter$.next(queryParams);
		this.loadingSubject.next(true);
		this.contractService.findData(queryParams)
			.pipe(
				tap(resultFromServer => {
					this.entitySubject.next(resultFromServer.data);
					var totalCount = resultFromServer.page.TotalCount || (resultFromServer.page.AllPage * resultFromServer.page.Size);
					this.paginatorTotalSubject.next(totalCount);
				}),
				catchError(err => of(new QueryResultsModel([], err))),
				finalize(() => this.loadingSubject.next(false))
			).subscribe(res => {
				this.contractService.ReadOnlyControl = res.Visible;
			});
	}
}
