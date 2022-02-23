import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { access } from 'fs';

@Injectable()
export class TokenStorage {
	/**
	 * Get User infor
	 * @returns {Observable<string>}
	 */
	public getUserInfo(): Observable<any> {
		var temp = localStorage.getItem('UserInfo');
		const user: any = JSON.parse(temp);
		return of(user);
	}
	/**
	 * Set user info
	 * @returns {UserInfo}
	 */
	public setUserInfo(info: any): TokenStorage {
		localStorage.setItem('UserInfo', info);
		return this;
	}

	/**
	 * Get access token
	 * @returns {Observable<string>}
	 */
	public getAccessToken(): Observable<string> {

		const token: string = <string>localStorage.getItem(environment.authTokenKey);
		return of(token);
	}
	public getAccessTokenString(): string {
		const token: string = <string>localStorage.getItem(environment.authTokenKey);
		return token;
	}
	/**
	 * Get user roles in JSON string
	 * @returns {Observable<any>}
	 */
	public getUserRoles(): Observable<any> {
		const roles: any = localStorage.getItem('userRoles');
		try {
			return of(JSON.parse(roles));
		} catch (e) { }
	}

	/**
	 * Get object user roles in JSON string
	 * @returns {Observable<any>}
	 */
	public getUserRolesObject(): Observable<any> {
		const roles: any = localStorage.getItem('userRoles');
		try {
			var temp = JSON.parse(roles);
			var re = {};
			for (var i = 0; i < temp.length; i++) {
				re['r' + temp[i]] = true;
			}
			return of(re);
		} catch (e) { }
	}

	/**
	 * Set access token
	 * @returns {TokenStorage}
	 */
	public setAccessToken(token: string): TokenStorage {
		localStorage.setItem(environment.authTokenKey, token);
		return this;
	}

	/**
	 * Set user roles
	 * @param roles
	 * @returns {TokenStorage}
	 */
	public setUserRoles(roles: any): any {
		if (roles != null) {
			localStorage.setItem('userRoles', JSON.stringify(roles));
		}

		return this;
	}

	/**
	 * Remove tokens
	 */
	public clear() {
		localStorage.removeItem('UserInfo');
		localStorage.removeItem(environment.authTokenKey);
	}

	public clearItem(item) {
		localStorage.removeItem(item);
	}

	public updateStorage(data) {
		this.clearItem("UserInfo");
		let accessData = {
			accessToken: data.Token,
			username: data.UserName,
			fullname: data.FullName,
			roles: data.Rules,
			id: data.Id,
			avata: data.Avata,
			IdDonVi: data.IdDonVi,
			ID_Goc: data.ID_Goc,
			ID_Goc_Cha: data.ID_Goc_Cha,
			Capcocau: data.Capcocau,
			DonVi: data.DonVi,
			MaDinhDanh: data.MaDinhDanh,
			vaitro: data.VaiTro,
			tenvaitro: data.TenVaiTro,
			email: data.Email,
			sdt: data.SDT,
			exp: data.ExpDate,
			IdTinh: data.IdTinh,
		};
		let user = {
			id: accessData.id,
			username: accessData.username,
			fullname: accessData.fullname,
			avata: accessData.avata,
			IdDonVi: accessData.IdDonVi,
			ID_Goc: accessData.ID_Goc,
			ID_Goc_Cha: accessData.ID_Goc_Cha,
			Capcocau: accessData.Capcocau,
			DonVi: accessData.DonVi,
			MaDinhDanh: accessData.MaDinhDanh,
			vaitro: accessData.vaitro,
			tenvaitro: accessData.tenvaitro,
			sdt: accessData.sdt,
			email: accessData.email,
			exp: accessData.exp,
			IdTinh: accessData.IdTinh,
		}
		this.setUserInfo(JSON.stringify(user))
			.setAccessToken(accessData.accessToken)
			.setUserRoles(accessData.roles);
		localStorage.setItem('cur_vaitro', accessData.vaitro);
	}
}
