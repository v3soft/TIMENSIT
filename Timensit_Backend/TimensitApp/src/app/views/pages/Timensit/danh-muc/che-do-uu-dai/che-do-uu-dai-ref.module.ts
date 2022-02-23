import { NgModule } from '@angular/core';

import { chedouudaiListComponent } from './che-do-uu-dai-list/che-do-uu-dai-list.component';
import { chedouudaiService } from './Services/che-do-uu-dai.service';
import { DPSCommonModule } from '../../dps-common.module';
import { chedouudaiEditDialogComponent } from './che-do-uu-dai-edit/che-do-uu-dai-edit.dialog.component';

@NgModule({
	imports: [
		DPSCommonModule
	],
	providers: [
		chedouudaiService
	],
	entryComponents: [
		chedouudaiListComponent,
		chedouudaiEditDialogComponent
	],
	declarations: [
		chedouudaiListComponent,
		chedouudaiEditDialogComponent
	],
	exports:[
		chedouudaiListComponent,
	]
})
export class chedouudaiRefModule { }
