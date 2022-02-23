import { NgModule } from '@angular/core';

import { contractListComponent } from './contract-list/contract-list.component';
import { contractService } from './Services/contract.service';
import { DPSCommonModule } from '../../dps-common.module';

@NgModule({
	imports: [
		DPSCommonModule
	],
	providers: [
		contractService
	],
	entryComponents: [
		contractListComponent,
	],
	declarations: [
		contractListComponent,
	],
	exports:[
		contractListComponent,
	]
})
export class contractRefModule { }
