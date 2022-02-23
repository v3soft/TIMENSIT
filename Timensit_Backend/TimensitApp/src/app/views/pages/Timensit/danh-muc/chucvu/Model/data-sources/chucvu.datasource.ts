import { of } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';
import { chucvuService } from '../../Services/chucvu.service';
import { BaseDataSource, QueryParamsModel, QueryResultsModel } from '../../../../../../../core/_base/crud';

export class chucvuDataSource extends BaseDataSource {
	constructor(private chucvuService: chucvuService) {
		super();
	}

	loadList(queryParams: QueryParamsModel) {
		this.chucvuService.lastFilter$.next(queryParams);
		this.loadingSubject.next(true);
		this.chucvuService.findData(queryParams)
			.pipe(
				tap(resultFromServer => {
					this.entitySubject.next(resultFromServer.data);
					var totalCount = resultFromServer.page.TotalCount || (resultFromServer.page.AllPage * resultFromServer.page.Size);
					this.paginatorTotalSubject.next(totalCount);
				}),
				catchError(err => of(new QueryResultsModel([], err))),
				finalize(() => this.loadingSubject.next(false))
			).subscribe(res => {
				this.chucvuService.ReadOnlyControl = res.Visible;
			});
	}
}
