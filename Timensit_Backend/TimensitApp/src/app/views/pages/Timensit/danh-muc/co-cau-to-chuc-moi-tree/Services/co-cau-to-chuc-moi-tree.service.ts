import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin, BehaviorSubject, of } from 'rxjs';
import { Injectable } from '@angular/core';
import { _ParseAST } from '@angular/compiler';
import { environment } from '../../../../../../../environments/environment';
import { QueryParamsModel, HttpUtilsService, QueryResultsModel } from '../../../../../../core/_base/crud';
import { OrgStructureModel } from '../Model/CoCauToChuc.model';
const API_PRODUCTS_URL = environment.ApiRoot + '/co-cau-to-chuc';
const API_URL = environment.ApiRoot;

@Injectable()
export class cocautochucMoiTreeService {
	lastFilter$: BehaviorSubject<QueryParamsModel> = new BehaviorSubject(new QueryParamsModel({}, 'asc', '', 0, 10));

	constructor(private http: HttpClient,
		private httpUtils: HttpUtilsService) { }
	Get_CoCauToChuc(): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		// const url = API_PRODUCTS_URL + '/Get_SoDoToChuc?id=${itemId};
		const url = `${API_PRODUCTS_URL}`;
		return this.http.get<QueryResultsModel>(url, {
			headers: httpHeaders,
		});
	}
	Createorgstructure(item: OrgStructureModel): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post<any>(API_PRODUCTS_URL, item, { headers: httpHeaders });
	}
	// UPDATE => PUT: update the product on the server
	Updateorgstructure(item: OrgStructureModel): Observable<any> {
		// Note: Add headers if needed (tokens/bearer)
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.put(API_PRODUCTS_URL + `/${item.RowID}`, item, { headers: httpHeaders });
	}
	// DELETE => delete the product from the server
	Deleteorgstructure(itemId: number, title: string): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = `${API_PRODUCTS_URL}/${itemId}`;
		return this.http.delete<any>(url, { headers: httpHeaders });
	}
	handleDropLevel(item: OrgStructureModel): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post(API_PRODUCTS_URL + '/handleDropOrgChart?idfrom=' + item.drop_idfrom + '&namefrom=' + item.drop_namefrom + '&idto=' + item.drop_idto + '&nameto=' + item.drop_nameto + '&IsAbove=' + item.IsAbove, item, { headers: httpHeaders });
	}
	handleDropParent(item: OrgStructureModel): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.post(API_PRODUCTS_URL + '/handleDropOrgChart?idfrom=' + item.drop_idfrom + '&namefrom=' + item.drop_namefrom + '&idto=' + item.drop_idto + '&level=' + item.level + '&nameto=' + item.drop_nameto, item, { headers: httpHeaders });
	}
	getCoCauMap() {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = `${API_PRODUCTS_URL}/co-cau-map`;
		return this.http.get<QueryResultsModel>(url, { headers: httpHeaders });
	}
	map(data: any) {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		const url = `${API_PRODUCTS_URL}/map`;
		return this.http.post<QueryResultsModel>(url, data, { headers: httpHeaders });
	}
}
