// Angular
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
// Core Module
import { CoreModule } from '../../../core/core.module';
import { PartialsModule } from '../../partials/partials.module';
import { DashboardComponent } from './dashboard.component';
import { CommonService } from '../Timensit/services/common.service';
import { ChartsModule, ThemeService } from 'ng2-charts';
import { MatPaginatorModule, MatPaginatorIntl, MatTooltipModule, MatBadgeModule } from '@angular/material';
import { CustomMatPaginatorIntl } from '../Timensit/custom-mat-pagination-int';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';

@NgModule({
	imports: [
		CommonModule,
		PartialsModule,
		MatPaginatorModule,
		CoreModule,
		MatTooltipModule,
		MatBadgeModule,
		PerfectScrollbarModule,
		RouterModule.forChild([
			{
				path: '',
				component: DashboardComponent
			},
		]),
	],
	providers: [CommonService,
		ThemeService,
		{
			provide: MatPaginatorIntl,
			useClass: CustomMatPaginatorIntl
		}],
	declarations: [
		DashboardComponent
	]
})
export class DashboardModule {
}
