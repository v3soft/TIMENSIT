// Angular
import { AfterViewInit, Component, Input, OnDestroy, OnInit, ChangeDetectorRef, OnChanges, ChangeDetectionStrategy, ViewRef, Renderer2 } from '@angular/core';
// RxJS
import { Subscription, BehaviorSubject } from 'rxjs';
// Layout
import { Breadcrumb } from '../../../../core/_base/layout/services/subheader.service';
import { filter } from 'rxjs/operators';
import { HtmlClassService } from '../../html-class.service';
import { MenuHorizontalService } from 'app/core/_base/layout';
import { Router, NavigationEnd } from '@angular/router';
import { QueryParamsModel } from 'app/core/_base/crud';
@Component({
	selector: 'kt-subheader-tab',
	templateUrl: './subheader-tab.component.html',
	styleUrls: ['./subheader-tab.component.scss'],
	// changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SubheaderTabComponent implements OnChanges, OnDestroy, AfterViewInit {
	// Public properties
	@Input() fluid: boolean;
	@Input() clear: boolean;
	@Input() data: any;
	@Input() menuId: number;
	today: number = Date.now();
	title = '';
	desc = '';
	breadcrumbs: Breadcrumb[] = [];
	currentRouteUrl: any = '';
	collapse:boolean=false;
	quantityNumber: any[];
	 
	// Private properties
	private subscriptions: Subscription[] = [];

	/**
	 * Component constructor
	 */
	constructor(
		public htmlClassService: HtmlClassService,
		public menuHorService: MenuHorizontalService,
		private router: Router,
		private cdr: ChangeDetectorRef,
		private render: Renderer2,
	) {
	}

	/**
	 * @ Lifecycle sequences => https://angular.io/guide/lifecycle-hooks
	 */

	/**
	 * On init
	 */
	ngOnChanges() {
		console.log("change");
		this.menuHorService.activeMenu$=this.menuId;
		let queryParams = new QueryParamsModel({});
		this.menuHorService.getSubMenu(queryParams);
		this.menuHorService.quantitySubmenu$.subscribe(res=>{
			this.data = res;
			this.cdr.detectChanges();
		})
		this.currentRouteUrl = this.router.url;
		this.router.events
			.pipe(filter(event => event instanceof NavigationEnd))
			.subscribe(event => {
				this.currentRouteUrl = this.router.url;
				this.cdr.markForCheck();
				
				// setTimeout(() => {
				// 	if (this.cdr && !(this.cdr as ViewRef).destroyed) {
				// 	  this.cdr.detectChanges();
				// 	}
				//   }, 5);
			});
			//this.cdr.detectChanges();

	}

	updateQuantityInMenu(){
		
	}
	/**
	 * After view init
	 */
	ngAfterViewInit(): void {
	
	}

	/**
	 * On destroy
	 */
	ngOnDestroy(): void {
		this.subscriptions.forEach(sb => sb.unsubscribe());
	}
	getItemCssClasses(item){
		if(this.isMenuItemIsActive(item)){
			return "active";
		}
		return "";
	}
	isMenuItemIsActive(item): boolean {
		// if (item.submenu) {
		// 	return this.isMenuRootItemIsActive(item);
		// }
		//console.log("item", item);
		if (!item.page) {
			return false;
		}

		return this.currentRouteUrl.indexOf(item.page) !== -1;
	}
	collapseSubmenu(){
		this.collapse=!this.collapse;
		if(this.collapse)
			this.render.addClass(document.body, 'subheader-collapse');
		else
			this.render.removeClass(document.body, 'subheader-collapse');
			
		let ele=(<HTMLInputElement>document.getElementById('kt_content'));
		ele.style.maxHeight=this.htmlClassService.getContentHeight();
	}
}
