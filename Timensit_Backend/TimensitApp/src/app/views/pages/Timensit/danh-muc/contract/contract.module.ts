import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { contractListComponent } from './contract-list/contract-list.component';
import { contractComponent } from './contract.component';
import { contractService } from './Services/contract.service';
import { contractRefModule } from './contract-ref.module';
import { DPSCommonModule } from '../../dps-common.module';
import { investorModule } from '../investor/investor.module';
import { investorRefModule } from '../investor/investor-ref.module';
import { investorService } from '../investor/services/investor.service';
const routes: Routes = [
	{
		path: '',
		component: contractComponent,
		children: [
			{
				path: '',
				component: contractListComponent,
			},
		]
	}
];

@NgModule({
	imports: [
		RouterModule.forChild(routes),
		DPSCommonModule,
		contractRefModule,
		investorRefModule
	],
	providers: [
		contractService,
	],
	entryComponents: [
	],
	declarations: [
		contractComponent,
	]
})
export class contractModule { }
