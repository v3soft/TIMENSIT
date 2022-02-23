import { NgModule } from '@angular/core';

import { investorListComponent } from './investor-list/investor-list.component';
import { DPSCommonModule } from '../../dps-common.module';
import { investorService } from './services/investor.service';
import { investorEditDialogComponent } from './investor-edit/investor-edit.dialog.component';

@NgModule({
	imports: [
		DPSCommonModule
	],
	providers: [
		investorService,
	],
	entryComponents: [
		investorListComponent,
	],
	declarations: [
		investorListComponent,
		//  investorEditDialogComponent
	],
	exports:[
		investorListComponent
	]

})
export class investorRefModule { }
