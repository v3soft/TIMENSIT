import { NgModule } from '@angular/core';

import { chucvuListComponent } from './chucvu-list/chucvu-list.component';
import { chucvuService } from './Services/chucvu.service';
import { DPSCommonModule } from '../../dps-common.module';
import { chucvuEditDialogComponent } from './chucvu-edit/chucvu-edit.dialog.component';

@NgModule({
	imports: [
		DPSCommonModule
	],
	providers: [
		chucvuService
	],
	entryComponents: [
		chucvuListComponent,
		chucvuEditDialogComponent
	],
	declarations: [
		chucvuListComponent,
		chucvuEditDialogComponent
	],
	exports:[
		chucvuListComponent,
	]
})
export class chucvuRefModule { }
