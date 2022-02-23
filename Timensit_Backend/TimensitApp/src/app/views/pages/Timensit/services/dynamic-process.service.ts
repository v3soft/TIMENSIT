import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpUtilsService, QueryParamsModel } from '../../../../core/_base/crud';
import { environment } from '../../../../../environments/environment';

const API_ROOT_URL = environment.ApiRoot + '/step';

@Injectable()
export class DynamicProcessService {
	lastFilter$: BehaviorSubject<QueryParamsModel> = new BehaviorSubject(new QueryParamsModel({}, 'asc', '', 0, 10));
	ReadOnlyControl: boolean;
	constructor(private http: HttpClient,
		private httpUtils: HttpUtilsService) { }

	detail(id: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(API_ROOT_URL + `/chitiet?Id=${id}`, { headers: httpHeaders });
	}
	FlowChart(id: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(API_ROOT_URL + `/FlowChart?Id=${id}`, { headers: httpHeaders });
	}
	deleteStep(id: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = `${API_ROOT_URL}/Xoa?Id=${id}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	saveStep(item): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post<any>(API_ROOT_URL + '/CapNhat', item, { headers: httpHeaders });
	}
	moved(id,loc): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(API_ROOT_URL + `/moved?id=${id}&loc=${loc}`, { headers: httpHeaders });
	}
	reshaped(id,points): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(environment.ApiRoot + `/step-next/reshaped?id=${id}&points=${points}`, { headers: httpHeaders });
	}
	SaveNextStep(data: any): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post<any>(environment.ApiRoot + `/step-next/CapNhat`, data , { headers: httpHeaders });
	}
	deleteNextStep(IdStep, NextStep): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = `${environment.ApiRoot}/step-next/xoa?IdStep=${IdStep}&NextStep=${NextStep}`;
		return this.http.get<any>(url, { headers: httpHeaders });
	}
	getStepLite(idLuong:any){
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(API_ROOT_URL + `/List?id=${idLuong}`, { headers: httpHeaders });
	}
	SaveFlowchartJson(url:string,item: any): Observable<any> {
        const httpHeaders = this.httpUtils.getHTTPHeaders();
        return this.http.post<any>(url, item, { headers: httpHeaders });
	}
	getAction(IdTable) {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(environment.ApiRoot + `/df/GetActionByTable?IdTable=${IdTable}`, { headers: httpHeaders });
		
	}
}
