
import { Router, RouterStateSnapshot, ActivatedRouteSnapshot, CanActivate, CanActivateChild, CanLoad, Route } from '@angular/router';
import { Injectable } from '@angular/core';
import { TokenStorage } from './token-storage.service';
import { promise } from 'protractor';
import { UserProfileService } from './user-profile.service';
import { MatSnackBar } from '@angular/material';

@Injectable()
export class PermissionUrl implements CanActivate, CanActivateChild, CanLoad {

	constructor(private router: Router,
		private tokenStorage: TokenStorage,
		private per: UserProfileService,
		private snackBar: MatSnackBar
	) { }

	canActivateChild(childRoute: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
		throw new Error("Method not implemented.");
	}
	canLoad(route: Route): boolean {
		throw new Error("Method not implemented.");
	}

	async canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Promise<boolean> {

		if (await this.tokenStorage.getAccessToken().toPromise()) {
			let re = await this.per.isPermission(state.url).toPromise()
				.then(res => {
					if (res && res.data)
					{
						this.snackBar.dismiss();
						return true;
					}
					else {
						return false;
					}
				})
				.catch(function (e) {
					return false;
				});

			if (!re) 
			{
				this.router.navigate(['/error/403'], { queryParams: { url: state.url } });
			}
			// if (!re) this.router.navigate(['/']);
					
			return re;
		}
		// not logged in so redirect to login page with the return url
		this.router.navigate(['/auth/login'], { queryParams: { returnUrl: state.url } });
		return false;
	}
}
