import { NgModule } from '@angular/core';

import { navListComponent } from './nav-list/nav-list.component';
import { navService } from './Services/nav.service';
import { DPSCommonModule } from '../../dps-common.module';
import { navEditDialogComponent } from './nav-edit/nav-edit.dialog.component';

@NgModule({
	imports: [
		DPSCommonModule
	],
	providers: [
		navService
	],
	entryComponents: [
		navListComponent,
		navEditDialogComponent
	],
	declarations: [
		navListComponent,
		navEditDialogComponent
	],
	exports:[
		navListComponent,
	]
})
export class navRefModule { }
