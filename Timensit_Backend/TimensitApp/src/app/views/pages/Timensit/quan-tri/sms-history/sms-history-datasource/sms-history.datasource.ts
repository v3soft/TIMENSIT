import { of } from 'rxjs';
import { catchError, finalize, tap } from 'rxjs/operators';
import { BaseDataSource, QueryParamsModel, QueryResultsModel } from 'app/core/_base/crud';
import { SMSHistoryService } from '../sms-history-service/sms-history.service';

export class SMSHistoryDataSource extends BaseDataSource {
	constructor(private productsService: SMSHistoryService) {
		super();
	}

	loadSMSHistorys(queryParams: QueryParamsModel) {
		this.productsService.lastFilter$.next(queryParams);
        this.loadingSubject.next(true);
		this.productsService.getData(queryParams)
			.pipe(
				tap(resultFromServer => {
					if(resultFromServer && resultFromServer.status ==1){
						this.entitySubject.next(resultFromServer.data);
						this.paginatorTotalSubject.next(resultFromServer.page.TotalCount);
					}else{
						this.entitySubject.next([]);
						this.paginatorTotalSubject.next(0);
					}	
				}),
				catchError(err => of(new QueryResultsModel([], err))),
				finalize(() => this.loadingSubject.next(false))
			).subscribe();
	}
}
