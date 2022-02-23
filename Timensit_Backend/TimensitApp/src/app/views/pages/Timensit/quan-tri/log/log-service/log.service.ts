import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';
// import { environment } from 'environments/environment';
import { HttpUtilsService } from '../../../../../../core/_base/crud/utils/http-utils.service';
import { QueryParamsModel, QueryResultsModel } from '../../../../../../core/_base/crud';
import { LogModel } from '../log-model/log.model';
import { environment } from '../../../../../../../environments/environment';

const API_ROOT_URL = environment.ApiRoot + '/log';

@Injectable()
export class LogService {
	lastFilter$: BehaviorSubject<QueryParamsModel> = new BehaviorSubject(new QueryParamsModel({}, 'asc', '', 0, 10));
	lastFilterDSExcel$: BehaviorSubject<any[]> = new BehaviorSubject([]);
	lastFilterInfoExcel$: BehaviorSubject<any> = new BehaviorSubject(undefined);
	lastFileUpload$: BehaviorSubject<{}> = new BehaviorSubject({});
	data_import: BehaviorSubject<any[]> = new BehaviorSubject([]);
	ReadOnlyControl: boolean;
	constructor(private http: HttpClient,
		private httpUtils: HttpUtilsService) { }

	getData(queryParams: QueryParamsModel): Observable<QueryResultsModel> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const httpParms = this.httpUtils.getFindHTTPParams(queryParams)

		return this.http.get<any>(API_ROOT_URL + '/list', { headers: httpHeaders, params: httpParms });

	}
	getFileLogs(queryParams: QueryParamsModel): Observable<QueryResultsModel> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const httpParms = this.httpUtils.getFindHTTPParams(queryParams)

		return this.http.get<any>(API_ROOT_URL, { headers: httpHeaders, params: httpParms });

	}
	getLogById(itemId: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(API_ROOT_URL + `/detail?id=${itemId}`, { headers: httpHeaders });
	}
	deleteLog(itemId: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = `${API_ROOT_URL}/delete?id=${itemId}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	deleteLogs(ids: any[] = []): Observable<any> {
		const url = API_ROOT_URL + '/deletes';
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post<QueryResultsModel>(url, ids, { headers: httpHeaders });
	}
	createLog(item: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post<LogModel>(API_ROOT_URL + '/create', item, { headers: httpHeaders });
	}
	updateLog(item: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post<LogModel>(API_ROOT_URL + '/update', item, { headers: httpHeaders });
	}
	LockNUnLock(itemId: any, value: boolean) {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = `${API_ROOT_URL}/LockAndUnLock?id=${itemId}&Value=${value}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	//#region ***filter***
}
