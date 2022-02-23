// Angular
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
// RxJS
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
// NGRX
import { select, Store } from '@ngrx/store';
// Auth reducers and selectors
import { AppState } from '../../../core/reducers/';
import { isLoggedIn } from '../_selectors/auth.selectors';
import * as jwt_decode from 'jwt-decode';
import { TokenStorage } from '../_services/token-storage.service';
import { environment } from '../../../../environments/environment';

@Injectable()
export class AuthGuard implements CanActivate {
	constructor(private store: Store<AppState>,
		private router: Router,
		private tokenStorage: TokenStorage) { }


	async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {
		let token = await this.tokenStorage.getAccessToken().toPromise()
		if (token && this.isTokenExpired()) {
			// logged in so return true
			if (state.url.startsWith('/auth'))
				this.router.navigateByUrl('/');
			return true;
		}
		// not logged in so redirect to login page with the return url
		if (!state.url.startsWith('/auth'))
			this.router.navigate(['/auth/login'], { queryParams: { returnUrl: state.url } });
		else
			return true;
		return false;
	}

	getToken(): string {
		return localStorage.getItem(environment.authTokenKey);
	}

	getTokenExpirationDate(token: string): Date {
		// token = atob(token);
		const decoded = jwt_decode(token);

		if (decoded.exp === undefined) return null;

		const date = new Date(0);
		date.setUTCSeconds(decoded.exp);
		return date;
	}

	isTokenExpired(token?: string): boolean {
		if (!token) token = this.getToken();
		if (!token) return false;

		const date = this.getTokenExpirationDate(token);
		if (date === undefined) return false;
		return (date.valueOf() > new Date().valueOf());
	}
	//canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean>  {
	//       return this.store
	//           .pipe(
	//               select(isLoggedIn),
	//               tap(loggedIn => {
	//                   if (!loggedIn) {
	//                       this.router.navigateByUrl('/auth/login');
	//                   }
	//               })
	//           );
	//   }
}
