import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { cocautochucmoitreeComponent } from './co-cau-to-chuc-moi-tree-list/co-cau-to-chuc-moi-tree-list.component';
import { cocautochucMoiTreeService } from './Services/co-cau-to-chuc-moi-tree.service';
import { cocautochucComponent } from './co-cau-to-chuc-moi-tree.component';
import { cocautochucmoiTreeRefModule } from './co-cau-to-chuc-moi-tree-ref.module';
import { DPSCommonModule } from '../../dps-common.module';
const routes: Routes = [
	{
		path: '',
		component: cocautochucmoitreeComponent,
	}
];

@NgModule({
	imports: [
		RouterModule.forChild(routes),
		DPSCommonModule,
		cocautochucmoiTreeRefModule
	],
	providers: [
		cocautochucMoiTreeService
	],
	entryComponents: [
		cocautochucComponent,
	],
	declarations: [
		cocautochucComponent,
	],
})
export class cocautochucmoiTreeModule { }
