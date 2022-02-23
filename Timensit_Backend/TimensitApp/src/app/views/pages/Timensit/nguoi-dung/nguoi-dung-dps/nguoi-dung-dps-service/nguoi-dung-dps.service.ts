import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';
import { environment } from 'environments/environment';
import { HttpUtilsService } from 'app/core/_base/crud/utils/http-utils.service';
import { QueryParamsModel, QueryResultsModel } from 'app/core/_base/crud';
import { NguoiDungDPSModel } from '../nguoi-dung-dps-model/nguoi-dung-dps.model';

const API_ROOT_URL = environment.ApiRoot + '/nguoi-dung';
const API_ROOT_URL1 = environment.ApiRoot + '/vai-tro-nguoi-dung';

@Injectable()
export class NguoiDungDPSService {
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
	getNguoiDungDPSById(itemId: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(API_ROOT_URL + `/detail?id=${itemId}`, { headers: httpHeaders });
	}
	deleteNguoiDungDPS(itemId: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = `${API_ROOT_URL}/delete?id=${itemId}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	lock(itemId: any, islock: boolean): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = `${API_ROOT_URL}/lock?id=${itemId}&islock=${islock}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	GiaHan(itemId: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = `${API_ROOT_URL}/gia-han?id=${itemId}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	createNguoiDungDPS(item: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post<NguoiDungDPSModel>(API_ROOT_URL + '/create', item, { headers: httpHeaders });
	}
	updateNguoiDungDPS(item: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post<NguoiDungDPSModel>(API_ROOT_URL + '/update', item, { headers: httpHeaders });
	}
	ResetPassNguoiDungDPS(item: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post<NguoiDungDPSModel>(API_ROOT_URL + '/reset-password', item, { headers: httpHeaders });
	}
	//#region vai trò
	getVaiTro(itemId: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = `${API_ROOT_URL1}/list?id=${itemId}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	deleteVaiTro(itemId: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = `${API_ROOT_URL1}/delete?id=${itemId}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	lockVaiTro(itemId: any, islock: boolean): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = `${API_ROOT_URL1}/lock?id=${itemId}&islock=${islock}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	updateVaiTro(item: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post<NguoiDungDPSModel>(API_ROOT_URL1 + '/update', item, { headers: httpHeaders });
	}
	uploadFile(data: any): Observable<any> {
		const url = API_ROOT_URL + '/UploadFile';
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post<any>(url, data, { headers: httpHeaders });
	}
	importFile(item: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post<any>(API_ROOT_URL + '/ImportFile', item, { headers: httpHeaders });
	}
	downloadTemplate(): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get(API_ROOT_URL + `/DownloadFileMauImportNguoiDung`, {
			headers: httpHeaders,
			responseType: 'blob',
			observe: 'response'
		});
	}
	ExportFile(): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get(API_ROOT_URL + `/ExportExcelNguoiDung`, {
			headers: httpHeaders,
			responseType: 'blob',
			observe: 'response'
		});
	}
	//#endregion
}
