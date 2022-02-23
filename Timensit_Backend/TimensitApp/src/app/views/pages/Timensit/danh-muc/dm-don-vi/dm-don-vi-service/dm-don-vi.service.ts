import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';
import { environment } from 'environments/environment';
import { HttpUtilsService } from 'app/core/_base/crud/utils/http-utils.service';
import { QueryParamsModel, QueryResultsModel } from 'app/core/_base/crud';
import { DM_DonViModel } from '../dm-don-vi-model/dm-don-vi.model';

const API_ROOT_URL = environment.ApiRoot + '/dm_donvi';

@Injectable()
export class DM_DonViService {
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
		return this.http.get<any>(API_ROOT_URL + '/DM_DonVi_List', {
			headers: httpHeaders,
			params: httpParams
		});

	}
	getData_User(queryParams: QueryParamsModel): Observable<QueryResultsModel> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();

		const httpParams = this.httpUtils.getFindHTTPParams(queryParams);
		return this.http.get<any>(API_ROOT_URL + '/DM_User_DonVi', {
			headers: httpHeaders,
			params: httpParams
		});

	}
	getDM_DonViById(itemId: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(API_ROOT_URL + `/DM_DonVi_Detail?Id=${itemId}`, { headers: httpHeaders });
	}
	deleteDM_DonVi(itemId: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = `${API_ROOT_URL}/DM_DonVi_Delete?Id=${itemId}`;
		return this.http.post<any>(url, null, { headers: httpHeaders });
	}
	deleteDM_DonVis(ids: any[] = []): Observable<any> {
		const url = API_ROOT_URL + '/DM_DonVi_Multi_Delete';
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post<QueryResultsModel>(url, ids, { headers: httpHeaders });
	}
	createDM_DonVi(item: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post<DM_DonViModel>(API_ROOT_URL + '/DM_DonVi_Insert', item, { headers: httpHeaders });
	}
	updateDM_DonVi(item: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post<DM_DonViModel>(API_ROOT_URL + '/DM_DonVi_Update', item, { headers: httpHeaders });
	}
	//#region ***filter***
	getFilterGroup(column: string, url: string): Observable<any> {
		return this.http.get<any>(environment.ApiRoot + url + `${column}`);
	}
	uploadFileDM_DonVi(data: any): Observable<any> {
		const url = API_ROOT_URL + '/DM_DonVi_UploadFile';
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post<any>(url, data, { headers: httpHeaders });
	}
	importDM_DonVi(item: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post<DM_DonViModel>(API_ROOT_URL + '/DM_DonVi_Import', item, { headers: httpHeaders });
	}
	downloadTemplate(): string {
		return API_ROOT_URL + `/DownLoadFileImportMau`;
	}
	GetTreeDonVi() {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(environment.ApiRoot + `/lite/LayTreeDonVi`, { headers: httpHeaders });
	}
}
