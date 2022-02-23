import { of } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';
import { BaseDataSource, QueryParamsModel, QueryResultsModel } from 'app/core/_base/crud';
import { NhomNguoiDungDPSService } from '../nhom-nguoi-dung-dps-service/nhom-nguoi-dung-dps.service';

export class NhomNguoiDungDPSDataSource extends BaseDataSource {
	constructor(private productsService: NhomNguoiDungDPSService) {
		super();
	}

	loadNhomNguoiDungDPSs(queryParams: QueryParamsModel) {
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
