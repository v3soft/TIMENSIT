// Angular
import { Injectable } from '@angular/core';
// RxJS
import { BehaviorSubject } from 'rxjs';
// Object path
import * as objectPath from 'object-path';
// Services
import { MenuConfigService } from './menu-config.service';
import { CommonService } from 'app/views/pages/Timensit/services/common.service';
import { AuthService } from 'app/core/auth';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'environments/environment';
import { HttpUtilsService, QueryParamsModel } from '../../crud';

@Injectable()
export class MenuHorizontalService {
	// Public properties
	menuList$: BehaviorSubject<any[]> = new BehaviorSubject<any[]>([]);
	quantitySubmenu$: BehaviorSubject<any[]> = new BehaviorSubject<any[]>([]);
	activeMenu$: number;
	/**
	 * Service constructor
	 *
	 * @param menuConfigService: MenuConfigService
	 */
	constructor(private menuConfigService: MenuConfigService,
		private commonService: CommonService,
		private http: HttpClient,
		private httpUtils: HttpUtilsService,
		private auth: AuthService,) {
		this.loadMenu();
		this.GetConfigTimeOut();
		this.commonService.TimesOutExpire();
		// setInterval(() => {
		// 	if (localStorage.getItem('UserInfo') != null) { 
		// 		this.CheckExpireLogin(); 
		// 	}
		// }, 1000);
	}

	/**
	 * Load menu list
	 */
	async loadMenu() {
		// get menu list
		await this.menuConfigService.getMenus().then(
			res => {
				setTimeout(() => {
					const menuItems: any[] = objectPath.get(res, 'header.items');
					this.menuList$.next(menuItems);
				}
				);
			});
	}

	//CheckExpireLogin() {
	//	// get menu list
	//	this.commonService.CheckExpireLogin().subscribe(
	//		res => {
	//			if (res.status == 0) {
	//				this.auth.logout(true);
	//			}
	//		});
	//}

	async GetConfigTimeOut() {
		await this.commonService.getConfig(['TIME_LOGOUT','DROP_BUTTON']).toPromise().then(res => {
			if (res && res.status == 1) {
				if (res.data.DROP_BUTTON != undefined) 
					localStorage.setItem('DROP_BUTTON',res.data.DROP_BUTTON );
				if (res.data.TIME_LOGOUT == undefined || res.data.TIME_LOGOUT == '') {
					localStorage.setItem('TIME_LOGOUT', '0');//gán mặc định nếu result trả về lỗi
				}
				else { 
					localStorage.setItem('TIME_LOGOUT',res.data.TIME_LOGOUT);//res.data.TIME_LOGOUT
				}
			}
			else
			{
				localStorage.setItem('TIME_LOGOUT', '0');//gán mặc định nếu result trả về lỗi
				localStorage.setItem('DROP_BUTTON','1');
			}
			console.log('TIME_LOGOUT', localStorage.getItem('TIME_LOGOUT'))
		})
	}
	getSubMenu(queryParams){
		this.loadSubmenu(queryParams).subscribe(res=>{
			if(res.status==1){
				this.quantitySubmenu$.next(res.data);
			}			
		})
	}
	loadSubmenu(queryParams: QueryParamsModel){
		const userToken = localStorage.getItem(environment.authTokenKey);
		queryParams.filter.active = this.activeMenu$?this.activeMenu$:0;
		const httpParms = this.httpUtils.getFindHTTPParams(queryParams)

		const httpHeaders = new HttpHeaders({
			'Authorization': 'Bearer ' + userToken,
		});
		return this.http.get<any>(environment.ApiRoot+`/user/getSubMenu`, { headers: httpHeaders, params:httpParms });	
	}
}
