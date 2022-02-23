import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { receiptsListComponent } from './receipts-list/receipts-list.component';
import { receiptsComponent } from './receipts.component';
import { receiptsService } from './Services/receipts.service';
import { receiptsRefModule } from './receipts-ref.module';
import { DPSCommonModule } from '../../dps-common.module';
import { investorModule } from '../investor/investor.module';
import { investorRefModule } from '../investor/investor-ref.module';
import { investorService } from '../investor/services/investor.service';
import { contractRefModule } from '../contract/contract-ref.module';
const routes: Routes = [
	{
		path: '',
		component: receiptsComponent,
		children: [
			{
				path: '',
				component: receiptsListComponent,
			},
		]
	}
];

@NgModule({
	imports: [
		RouterModule.forChild(routes),
		DPSCommonModule,
		receiptsRefModule,
		contractRefModule
	],
	providers: [
		receiptsService,
	],
	entryComponents: [
	],
	declarations: [
		receiptsComponent,
	]
})
export class receiptsModule { }
