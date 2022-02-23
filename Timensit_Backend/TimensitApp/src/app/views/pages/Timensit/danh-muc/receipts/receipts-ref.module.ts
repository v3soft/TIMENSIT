import { NgModule } from '@angular/core';

import { receiptsListComponent } from './receipts-list/receipts-list.component';
import { receiptsService } from './Services/receipts.service';
import { DPSCommonModule } from '../../dps-common.module';
import { receiptsEditDialogComponent } from './receipts-edit/receipts-edit.dialog.component';

@NgModule({
	imports: [
		DPSCommonModule
	],
	providers: [
		receiptsService
	],
	entryComponents: [
		receiptsListComponent,
		receiptsEditDialogComponent
	],
	declarations: [
		receiptsListComponent,
		receiptsEditDialogComponent
	],
	exports:[
		receiptsListComponent,
	]
})
export class receiptsRefModule { }
