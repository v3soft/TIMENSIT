import { CommonModule, } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { investorComponent } from './investor.component';
// Core
// Core => Services

import { investorListComponent } from './investor-list/investor-list.component';
import { investorEditDialogComponent } from './investor-edit/investor-edit.dialog.component';
import { investorRefModule } from './investor-ref.module';
import { DPSCommonModule } from '../../dps-common.module';
import { investorService } from './services/investor.service';
import { NgModule } from '@angular/core';

const routes: Routes = [
	{
		path: '',
		component: investorComponent,
		children: [
			{
				path: '',
				component: investorListComponent,
			},
			{
				path: 'themmoi',
				component: investorEditDialogComponent
			},
			{
				path: 'chinhsua/:id',
				component: investorEditDialogComponent
			},
		]
	}
];

@NgModule({
	imports: [
		RouterModule.forChild(routes),
		DPSCommonModule,
		investorRefModule
	],
	providers: [
		investorService,
	],
	entryComponents: [
	],
	declarations: [
		investorComponent,
	]
})
export class investorModule { }
