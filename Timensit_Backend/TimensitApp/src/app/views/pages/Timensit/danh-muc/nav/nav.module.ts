import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { navListComponent } from './nav-list/nav-list.component';
import { navComponent } from './nav.component';
import { navService } from './Services/nav.service';
import { navRefModule } from './nav-ref.module';
import { DPSCommonModule } from '../../dps-common.module';
import { investorModule } from '../investor/investor.module';
import { investorRefModule } from '../investor/investor-ref.module';
import { investorService } from '../investor/services/investor.service';
import { contractRefModule } from '../contract/contract-ref.module';
const routes: Routes = [
	{
		path: '',
		component: navComponent,
		children: [
			{
				path: '',
				component: navListComponent,
			},
		]
	}
];

@NgModule({
	imports: [
		RouterModule.forChild(routes),
		DPSCommonModule,
		navRefModule,
		contractRefModule
	],
	providers: [
		navService,
	],
	entryComponents: [
	],
	declarations: [
		navComponent,
	]
})
export class navModule { }
