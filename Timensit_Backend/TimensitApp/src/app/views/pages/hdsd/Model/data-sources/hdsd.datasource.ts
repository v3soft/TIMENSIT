import { of } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';
import { BaseDataSource, QueryParamsModel, QueryResultsModel } from '../../../../../core/_base/crud';
import { hdsdService } from '../../Services/hdsd.service';

export class hdsdDataSource extends BaseDataSource {
	constructor(private hdsdService: hdsdService) {
		super();
	}

	loadList(queryParams: QueryParamsModel) {
		this.hdsdService.lastFilter$.next(queryParams);
		this.loadingSubject.next(true);

		this.hdsdService.findData(queryParams)
			.pipe(
				tap(resultFromServer => {
					this.entitySubject.next(resultFromServer.data);
					var totalCount = resultFromServer.page.TotalCount || (resultFromServer.page.AllPage * resultFromServer.page.Size);
					this.paginatorTotalSubject.next(totalCount);
				}),
				catchError(err => of(new QueryResultsModel([], err))),
				finalize(() => this.loadingSubject.next(false))
			).subscribe(res => {
				this.hdsdService.ReadOnlyControl = res.Visible;
			});
	}
}
