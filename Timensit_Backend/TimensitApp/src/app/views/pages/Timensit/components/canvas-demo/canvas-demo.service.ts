import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpUtilsService } from '../../../../../core/_base/crud';
import { environment } from '../../../../../../environments/environment';

@Injectable()
export class CanvasDemoService {

	constructor(private http: HttpClient,
		private httpUtils: HttpUtilsService) {
	}

	public getData(url: string): Observable<any> {
		const httpHeaders = this.httpUtils.getHTTPHeaders();
		return this.http.get<any>(url, { headers: httpHeaders });
	}
}
