// Angular
import { Injectable } from '@angular/core';
// RxJS
import { Subject } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
const API_ROOT_URL = environment.ApiRoot + "/user";

@Injectable()
export class MenuConfigService {
	// Public properties
	onConfigUpdated$: Subject<any>;
	// Private properties
	private menuConfig: any;

	/**
	 * Service Constructor
	 */
	constructor(private http: HttpClient) {
		// register on config changed event and set default config
		this.onConfigUpdated$ = new Subject();
	}

	/**
	 * Returns the menuConfig
	 */
	async getMenus() {
		let re = this.menuConfig;;
		await this.loadMenuPhanQuyen().toPromise().then(res => {
			if (res && res.status == 1) {
				re.header.items = res.data;
				re.aside.items = res.data;
			}
		});
		return re;
		//return this.menuConfig;
	}

	/**
	 * Load config
	 *
	 * @param config: any
	 */
	loadConfigs(config: any) {
		this.menuConfig = config;
		this.onConfigUpdated$.next(this.menuConfig);
	}
	loadMenuPhanQuyen() {
		const userToken = localStorage.getItem(environment.authTokenKey);
		const httpHeaders = new HttpHeaders({
			'Authorization': 'Bearer ' + userToken,
		});
		return this.http.get<any>(API_ROOT_URL + '/LayMenuChucNang', { headers: httpHeaders });
	}
}
