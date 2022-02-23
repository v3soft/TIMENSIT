// Angular
import { AfterViewInit, Component, ElementRef, OnInit, ViewChild, ChangeDetectorRef, ChangeDetectionStrategy } from '@angular/core';
import {
	NavigationCancel,
	NavigationEnd,
	NavigationStart,
	RouteConfigLoadEnd,
	RouteConfigLoadStart,
	Router
} from '@angular/router';
// Object-Path
import * as objectPath from 'object-path';
// Loading bar
import { LoadingBarService } from '@ngx-loading-bar/core';
// Layout
import { LayoutConfigService, LayoutRefService } from '../../../core/_base/layout';
// HTML Class Service
import { HtmlClassService } from '../html-class.service';
import { BehaviorSubject } from 'rxjs';


@Component({
	selector: 'kt-header',
	templateUrl: './header.component.html',
	styleUrls: ['./header.component.scss'],
	// changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HeaderComponent implements OnInit, AfterViewInit {
	// Public properties
	menuHeaderDisplay: boolean;
	subheaderDisplay: boolean;
	dataSubmenu:any;
	fluid: boolean;
	fluid_sub: boolean;
	constants:any;
	clear: boolean;
	currentRouteUrl: any = '';
	activeMenu: number;
	@ViewChild('ktHeader', {static: true}) ktHeader: ElementRef;

	/**
	 * Component constructor
	 *
	 * @param router: Router
	 * @param layoutRefService: LayoutRefService
	 * @param layoutConfigService: LayoutConfigService
	 * @param loader: LoadingBarService
	 * @param htmlClassService: HtmlClassService
	 */
	constructor(
		private router: Router,
		private layoutRefService: LayoutRefService,
		private layoutConfigService: LayoutConfigService,
		public loader: LoadingBarService,
		public htmlClassService: HtmlClassService,
		// private cdr: ChangeDetectorRef
	) {
		// page progress bar percentage
		this.router.events.subscribe(event => {
			if (event instanceof NavigationStart) {
				// set page progress bar loading to start on NavigationStart event router
				this.loader.start();
			}
			if (event instanceof RouteConfigLoadStart) {
				this.loader.increment(35);
			}
			if (event instanceof RouteConfigLoadEnd) {
				this.loader.increment(75);
			}
			if (event instanceof NavigationEnd || event instanceof NavigationCancel) {
				// set page progress bar loading to end on NavigationEnd event router
				
				this.loader.complete();
			}
		});
	}

	/**
	 * @ Lifecycle sequences => https://angular.io/guide/lifecycle-hooks
	 */

	/**
	 * On init
	 */
	ngOnInit(): void {
		this.currentRouteUrl = this.router.url;
		const config = this.layoutConfigService.getConfig();
		this.constants = this.layoutConfigService.getConfig('constants');;
		// get menu header display option
		this.menuHeaderDisplay = objectPath.get(config, 'header.menu.self.display');
		this.subheaderDisplay = objectPath.get(config, 'subheader.display');
		// header width fluid
		this.fluid = objectPath.get(config, 'header.self.width') === 'fluid';
		this.fluid_sub = objectPath.get(config, 'footer.self.width') === 'fluid';
		this.clear = objectPath.get(config, 'subheader.clear');
		// animate the header minimize the height on scroll down
		if (objectPath.get(config, 'header.self.fixed.desktop.enabled') || objectPath.get(config, 'header.self.fixed.desktop')) {
			// header minimize on scroll down
			this.ktHeader.nativeElement.setAttribute('data-ktheader-minimize', '1');
		}
		
	}

	ngAfterViewInit(): void {
		// keep header element in the service
		this.layoutRefService.addElement('header', this.ktHeader.nativeElement);
	}
	/**********Load Submenu Header********/
	getActiveHeader(event){
		// console.log("event", event);
		this.dataSubmenu=event.submenu;
		this.activeMenu= event.id?event.id:0;
		if(this.dataSubmenu&&this.dataSubmenu.length>0 &&!event.init){//init true là lấy link hiện tại của trang khi reload trang, false: chuyển tab menu
			this.router.navigateByUrl(this.dataSubmenu[0].page);
		}		
	}	
}
