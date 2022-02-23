import { of } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';
import { BaseDataSource, QueryParamsModel, QueryResultsModel } from 'app/core/_base/crud';
import { CommonService } from '../../services/common.service';

export class ChonVaiTroDataSource extends BaseDataSource {
	constructor(private service: CommonService) {
		super();
	}

	LoadData(queryParams: QueryParamsModel) {
		this.loadingSubject.next(true);
		this.service.ListVaiTroPhanTrang(queryParams)
			.pipe(
				tap(resultFromServer => {
					if (resultFromServer.data != null && resultFromServer.data != undefined) {
						this.entitySubject.next(resultFromServer.data);
						this.paginatorTotalSubject.next(resultFromServer.page.TotalCount);
					}
					else {
						this.entitySubject.next(null);
						this.paginatorTotalSubject.next(null);
					}
				}),
				catchError(err => of(new QueryResultsModel([], err))),
				finalize(() => this.loadingSubject.next(false))
			).subscribe();
	}
}
