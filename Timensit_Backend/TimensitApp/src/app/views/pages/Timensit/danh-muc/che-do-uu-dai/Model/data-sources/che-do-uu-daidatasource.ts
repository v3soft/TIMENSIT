import { of } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';
import { chedouudaiService } from '../../Services/che-do-uu-dai.service';
import { BaseDataSource, QueryParamsModel, QueryResultsModel } from '../../../../../../../core/_base/crud';

export class chedouudaiDataSource extends BaseDataSource {
	constructor(private chedouudaiService: chedouudaiService) {
		super();
	}

	loadList(queryParams: QueryParamsModel) {
		this.chedouudaiService.lastFilter$.next(queryParams);
		this.loadingSubject.next(true);
		this.chedouudaiService.findData(queryParams)
			.pipe(
				tap(resultFromServer => {
					this.entitySubject.next(resultFromServer.data);
					var totalCount = resultFromServer.page.TotalCount || (resultFromServer.page.AllPage * resultFromServer.page.Size);
					this.paginatorTotalSubject.next(totalCount);
				}),
				catchError(err => of(new QueryResultsModel([], err))),
				finalize(() => this.loadingSubject.next(false))
			).subscribe(res => {
				this.chedouudaiService.ReadOnlyControl = res.Visible;
			});
	}
}
