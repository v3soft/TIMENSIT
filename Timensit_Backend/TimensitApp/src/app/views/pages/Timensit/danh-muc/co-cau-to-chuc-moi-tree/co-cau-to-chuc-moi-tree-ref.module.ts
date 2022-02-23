import { NgModule } from '@angular/core';
import { cocautochucmoitreeComponent } from './co-cau-to-chuc-moi-tree-list/co-cau-to-chuc-moi-tree-list.component';
import { cocautochucMoiTreeService } from './Services/co-cau-to-chuc-moi-tree.service';
import { DPSCommonModule } from '../../dps-common.module';
import { CoCauToChucEditComponent } from './co-cau-to-chuc-moi-tree-edit/co-cau-to-chuc-moi-tree-edit.component';
import { CoCauMapDialogComponent } from './co-cau-map/co-cau-map-dialog.component';
import { RouterModule } from '@angular/router';

@NgModule({
	imports: [
		RouterModule,
		DPSCommonModule
	],
	providers: [
		cocautochucMoiTreeService,
	],
	entryComponents: [
		cocautochucmoitreeComponent,
		CoCauToChucEditComponent,
		CoCauMapDialogComponent
	],
	declarations: [
		cocautochucmoitreeComponent,
		CoCauToChucEditComponent,
		CoCauMapDialogComponent
	],
	exports: [
		cocautochucmoitreeComponent,
		CoCauToChucEditComponent,
		CoCauMapDialogComponent
	]
})
export class cocautochucmoiTreeRefModule { }
