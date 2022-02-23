import { NgModule } from '@angular/core';
import { OrgChartService } from './Services/so-do-to-chuc.service';
import { SodotochucListComponent } from './so-do-to-chuc-new-list/so-do-to-chuc-new-list.component';
import { DPSCommonModule } from '../../dps-common.module';
import { chucvuService } from '../chucvu/Services/chucvu.service';
import { sodotochuceditComponent } from './so-do-to-chuc-edit/so-do-to-chuc-edit.component';
import { DrawListComponent } from './draw-chart/draw-chart-list.component';
import { RouterModule } from '@angular/router';
import { DndModule } from 'ngx-drag-drop';

@NgModule({
	declarations: [
		SodotochucListComponent,
		sodotochuceditComponent,
		DrawListComponent
	],
	imports: [
		RouterModule,
		DPSCommonModule,
		DndModule
	],
	entryComponents: [
		SodotochucListComponent
	],
	providers: [
		[
			OrgChartService,
			chucvuService,
		],
	],
	exports:[
		SodotochucListComponent
	]
})
// router đâu

export class OrgChartRefNewModule { }
