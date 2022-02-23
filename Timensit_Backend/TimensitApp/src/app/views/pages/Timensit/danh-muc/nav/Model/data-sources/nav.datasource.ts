import { of } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';
import { navService } from '../../Services/nav.service';
import { BaseDataSource, QueryParamsModel, QueryResultsModel } from '../../../../../../../core/_base/crud';

export class navDataSource extends BaseDataSource {
	constructor(private navService: navService) {
		super();
	}

	loadList(queryParams: QueryParamsModel) {
		this.navService.lastFilter$.next(queryParams);
		this.loadingSubject.next(true);
		this.navService.findData(queryParams)
			.pipe(
				tap(resultFromServer => {
					this.entitySubject.next(resultFromServer.data);
					var totalCount = resultFromServer.page.TotalCount || (resultFromServer.page.AllPage * resultFromServer.page.Size);
					this.paginatorTotalSubject.next(totalCount);
				}),
				catchError(err => of(new QueryResultsModel([], err))),
				finalize(() => this.loadingSubject.next(false))
			).subscribe(res => {
				this.navService.ReadOnlyControl = res.Visible;
			});
	}
}
