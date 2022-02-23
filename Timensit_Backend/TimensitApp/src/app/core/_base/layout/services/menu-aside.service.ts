// Angular
import { Injectable } from '@angular/core';
// RxJS
import { BehaviorSubject } from 'rxjs';
// Object path
import * as objectPath from 'object-path';
// Services
import { MenuConfigService } from './menu-config.service';

@Injectable()
export class MenuAsideService {
	// Public properties
	menuList$: BehaviorSubject<any[]> = new BehaviorSubject<any[]>([]);

	/**
	 * Service constructor
	 *
	 * @param menuConfigService: MenuConfigService
	 */
	constructor(private menuConfigService: MenuConfigService) {
		this.loadMenu();
	}

	/**
	 * Load menu list
	 */
	async loadMenu() {
		// get menu list
		await this.menuConfigService.getMenus().then(
			res => {
				setTimeout(() => {
					const menuItems: any[] = objectPath.get(res, 'aside.items');
					this.menuList$.next(menuItems);
				}
				);
			});
	}
}
