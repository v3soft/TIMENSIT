import { of } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';
import { ChucDanhService } from '../../services/chucdanh.service';
import { BaseDataSource, QueryResultsModel, QueryParamsModel } from '../../../../../../../core/_base/crud';

export class ChucDanhDataSource extends BaseDataSource {
	constructor(private ChucDanhService: ChucDanhService) {
		super();
	}

	loadList(queryParams: QueryParamsModel) {
		this.ChucDanhService.lastFilter$.next(queryParams);
		this.loadingSubject.next(true);

		this.ChucDanhService.findData(queryParams)
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
						this.ChucDanhService.ReadOnlyControl = res.Visible;
				}
			);
	}
}
