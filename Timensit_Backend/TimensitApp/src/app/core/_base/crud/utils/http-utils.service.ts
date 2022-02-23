// Angular
import { Injectable } from '@angular/core';
import { HttpParams, HttpHeaders } from '@angular/common/http';
// CRUD
import { QueryResultsModel } from '../models/query-models/query-results.model';
import { QueryParamsModel } from '../models/query-models/query-params.model';
import { HttpExtenstionsModel } from '../../crud/models/http-extentsions-model';
import { TokenStorage } from '../../../auth/_services/token-storage.service';

@Injectable()
export class HttpUtilsService {
	constructor(private tokenStorage: TokenStorage) {
	}
	/**
	 * Prepare query http params
	 * @param queryParams: QueryParamsModel
	 */
	getFindHTTPParams(queryParams): HttpParams {
		let params = new HttpParams()
			.set('sortOrder', queryParams.sortOrder)
			.set('sortField', queryParams.sortField)
			.set('page', (queryParams.pageNumber + 1).toString())
			.set('record', queryParams.pageSize.toString());
		let keys = [], values = [];
		if (queryParams.more) {
			params = params.append('more', 'true');
		}
		Object.keys(queryParams.filter).forEach(function (key) {
			if (typeof queryParams.filter[key] !== 'string' || queryParams.filter[key] !== '') {
				keys.push(key);
				values.push(queryParams.filter[key]);
			}
		});
		if (keys.length > 0) {
			params = params.append('filter.keys', keys.join('|'))
				.append('filter.vals', values.join('|'));
		}
		let grp_keys = [], grp_values = [];
		Object.keys(queryParams.filterGroup).forEach(function (key) {
			if (typeof queryParams.filterGroup[key] !== 'string' || queryParams.filterGroup[key] !== '') {
				grp_keys.push(key);
				let value_str = "";
				if (queryParams.filterGroup[key] && queryParams.filterGroup[key].length > 0) {
					value_str = queryParams.filterGroup[key].join(',')
				}
				grp_values.push(value_str);
			}
		});
		if (grp_keys.length > 0) {
			params = params.append('filterGroup.keys', grp_keys.join('|'))
				.append('filterGroup.vals', grp_values.join('|'));
		}
		return params;
	}

	/**
	 * get standard content-type
	 */
	getHTTPHeaders(isFormData?: boolean): HttpHeaders {
		var _token = '';
		this.tokenStorage.getAccessToken().subscribe(t => { _token = t; });
		let result = new HttpHeaders({
			'Content-Type': 'application/json',
			'Authorization': 'Bearer ' + _token,
			'Access-Control-Allow-Origin': '*',
			'Access-Control-Allow-Headers': 'Content-Type'
		});
		return result;
	}

	parseFilter(filter): HttpParams{
		let params = new HttpParams()
		let keys = [], values = [];
		Object.keys(filter).forEach(function (key) {
			if (typeof filter[key] !== 'string' || filter[key] !== '') {
				keys.push(key);
				values.push(filter[key]);
			}
		});
		if (keys.length > 0) {
			params = params.append('keys', keys.join('|'))
				.append('vals', values.join('|'));
		}
		return params;
	}

	baseFilter(_entities: any[], _queryParams: QueryParamsModel, _filtrationFields: string[] = []): QueryResultsModel {
		const httpExtention = new HttpExtenstionsModel();
		return httpExtention.baseFilter(_entities, _queryParams, _filtrationFields);
	}

	sortArray(_incomingArray: any[], _sortField: string = '', _sortOrder: string = 'asc'): any[] {
		const httpExtention = new HttpExtenstionsModel();
		return httpExtention.sortArray(_incomingArray, _sortField, _sortOrder);
	}

	searchInArray(_incomingArray: any[], _queryObj: any, _filtrationFields: string[] = []): any[] {
		const httpExtention = new HttpExtenstionsModel();
		return httpExtention.searchInArray(_incomingArray, _queryObj, _filtrationFields);
	}
}
