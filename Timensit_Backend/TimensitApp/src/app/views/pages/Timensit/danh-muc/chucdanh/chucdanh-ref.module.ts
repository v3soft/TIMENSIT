import { NgModule } from '@angular/core';

import { ChucDanhListComponent } from './chucdanh-list/chucdanh-list.component';
import { DPSCommonModule } from '../../dps-common.module';
import { ChucDanhService } from './services/chucdanh.service';
import { ChucDanhEditDialogComponent } from './chucdanh-edit/chucdanh-edit.dialog.component';

@NgModule({
	imports: [
		DPSCommonModule
	],
	providers: [
		ChucDanhService,
	],
	entryComponents: [
		ChucDanhListComponent,
	],
	declarations: [
		ChucDanhListComponent,
		 ChucDanhEditDialogComponent
	],
	exports:[
		ChucDanhListComponent
	]

})
export class ChucDanhRefModule { }
