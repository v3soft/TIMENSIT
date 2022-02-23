import { of } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';
import { NguoiDungDPSService } from '../nguoi-dung-dps-service/nguoi-dung-dps.service';
import { BaseDataSource, QueryParamsModel, QueryResultsModel } from '../../../../../../core/_base/crud';

export class NguoiDungDPSDataSource extends BaseDataSource {
	constructor(private productsService: NguoiDungDPSService) {
		super();
	}

	loadNguoiDungDPSs(queryParams: QueryParamsModel) {
		this.productsService.lastFilter$.next(queryParams);
        this.loadingSubject.next(true);
		this.productsService.getData(queryParams)
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
