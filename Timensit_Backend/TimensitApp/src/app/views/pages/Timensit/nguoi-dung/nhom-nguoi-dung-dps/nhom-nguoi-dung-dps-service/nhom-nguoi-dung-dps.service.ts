import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';
import { environment } from 'environments/environment';
import { HttpUtilsService } from 'app/core/_base/crud/utils/http-utils.service';
import { QueryParamsModel, QueryResultsModel } from 'app/core/_base/crud';
import { NhomNguoiDungDPSModel } from '../nhom-nguoi-dung-dps-model/nhom-nguoi-dung-dps.model';

const API_ROOT_URL = environment.ApiRoot + '/nhom-nguoi-dung';

@Injectable()
export class NhomNguoiDungDPSService {
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
		const httpParams = this.httpUtils.getFindHTTPParams(queryParams);
		return this.http.get<any>(API_ROOT_URL + '/list', { headers: httpHeaders, params: httpParams });

	}
	getNhomNguoiDungDPSById(itemId: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(API_ROOT_URL + `/detail?id=${itemId}`, { headers: httpHeaders });
	}
	deleteNhomNguoiDungDPS(itemId: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = `${API_ROOT_URL}/delete?id=${itemId}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	lock(itemId: any, islock: boolean): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = `${API_ROOT_URL}/lock?id=${itemId}&islock=${islock}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	createNhomNguoiDungDPS(item: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post<NhomNguoiDungDPSModel>(API_ROOT_URL + '/create', item, { headers: httpHeaders });
	}
	updateNhomNguoiDungDPS(item: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post<NhomNguoiDungDPSModel>(API_ROOT_URL + '/update', item, { headers: httpHeaders });
	}
	uploadFileNhomNguoiDungDPS(data: any): Observable<any> {
		const url = API_ROOT_URL + '';
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post<any>(url, data, { headers: httpHeaders });
	}
	updateQuyen(item: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post<NhomNguoiDungDPSModel>(API_ROOT_URL + '/update-quyen', item, { headers: httpHeaders });
	}
}
