import { of } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';
import { capquanlyService } from '../../Services/capquanly.service';
import { BaseDataSource, QueryParamsModel, QueryResultsModel } from '../../../../../../../core/_base/crud';

export class capquanlyDataSource extends BaseDataSource {
	constructor(private capquanlyService: capquanlyService) {
		super();
	}

	loadList(queryParams: QueryParamsModel) {
		this.capquanlyService.lastFilter$.next(queryParams);
		this.loadingSubject.next(true);

		this.capquanlyService.findData(queryParams)
			.pipe(
				tap(resultFromServer => {
					this.entitySubject.next(resultFromServer.data);
				var totalCount = resultFromServer.page.TotalCount || (resultFromServer.page.AllPage * resultFromServer.page.Size);
					this.paginatorTotalSubject.next(totalCount);
				}),
				catchError(err => of(new QueryResultsModel([], err))),
				finalize(() => this.loadingSubject.next(false))
			).subscribe(res => {
				this.capquanlyService.ReadOnlyControl = res.Visible;
			});
	}
}
