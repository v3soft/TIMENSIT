import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin, BehaviorSubject, of } from 'rxjs';
import { map, retry } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { environment } from '../../../../../environments/environment';
import { HttpUtilsService, QueryParamsModel, QueryResultsModel } from '../../../../core/_base/crud';

const API_PRODUCTS_URL = environment.ApiRoot + '/thong-bao';

@Injectable()
export class NotifyService {
	lastFilter$: BehaviorSubject<QueryParamsModel> = new BehaviorSubject(new QueryParamsModel({}, 'asc', '', 0, 10));
	ReadOnlyControl: boolean;
	constructor(private http: HttpClient,
		private httpUtils: HttpUtilsService) { }

	findData(queryParams: QueryParamsModel): Observable<QueryResultsModel> {

		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const httpParams = this.httpUtils.getFindHTTPParams(queryParams);
		return this.http.get<QueryResultsModel>(API_PRODUCTS_URL + "/get-thong-bao-dashboard", {
			headers: httpHeaders,
			params: httpParams
		});
	}
	markAsRead(isDelete=false): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(API_PRODUCTS_URL + '/markAsRead?isDelete=' + isDelete, { headers: httpHeaders });
	}
	delete(Id): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(API_PRODUCTS_URL + '/delete?Id=' + Id, { headers: httpHeaders });
	}
	deletes(data): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post<any>(API_PRODUCTS_URL + '/deletes', data, { headers: httpHeaders });
	}
}
