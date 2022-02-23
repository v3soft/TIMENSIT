import { of } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';
import { investorService } from '../../services/investor.service';
import { BaseDataSource, QueryResultsModel, QueryParamsModel } from '../../../../../../../core/_base/crud';

export class investorDataSource extends BaseDataSource {
	constructor(private investorService: investorService) {
		super();
	}

	loadList(queryParams: QueryParamsModel) {
		this.investorService.lastFilter$.next(queryParams);
		this.loadingSubject.next(true);

		this.investorService.findData(queryParams)
			.pipe(
				tap(resultFromServer => {
					this.entitySubject.next(resultFromServer.data);
				var totalCount = resultFromServer.page.TotalCount || (resultFromServer.page.AllPage * resultFromServer.page.Size);
					this.paginatorTotalSubject.next(totalCount);
				}),
				catchError(err => of(new QueryResultsModel([], err))),
				finalize(() => this.loadingSubject.next(false))
			).subscribe(
				res => {
						this.investorService.ReadOnlyControl = res.Visible;
				}
			);
	}
}
