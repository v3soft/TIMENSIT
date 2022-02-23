import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';
import { environment } from 'environments/environment';
import { HttpUtilsService } from 'app/core/_base/crud/utils/http-utils.service';
import { QueryParamsModel, QueryResultsModel } from 'app/core/_base/crud';
import { CauHinhSMSModel } from '../cau-hinh-sms-model/cau-hinh-sms.model';

const API_ROOT_URL = environment.ApiRoot + '/chsms';

@Injectable()
export class CauHinhSMSService {
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
	getCauHinhSMSById(itemId: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(API_ROOT_URL + `/detail?id=${itemId}`, { headers: httpHeaders });
	}
	deleteCauHinhSMS(itemId: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = `${API_ROOT_URL}/delete?id=${itemId}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	deleteCauHinhSMSs(ids: any[] = []): Observable<any> {
		const url = API_ROOT_URL + '';
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post<QueryResultsModel>(url, ids, { headers: httpHeaders });
	}
	createCauHinhSMS(item: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post<CauHinhSMSModel>(API_ROOT_URL + '/create', item, { headers: httpHeaders });
	}
	updateCauHinhSMS(item: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post<CauHinhSMSModel>(API_ROOT_URL + '/update', item, { headers: httpHeaders });
	}
	LockNUnLock(itemId: any, value: boolean) {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = `${API_ROOT_URL}/LockAndUnLock?id=${itemId}&Value=${value}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	//#region ***filter***
}