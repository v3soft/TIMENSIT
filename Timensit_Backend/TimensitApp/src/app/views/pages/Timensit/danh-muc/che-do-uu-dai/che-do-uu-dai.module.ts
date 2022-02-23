import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { chedouudaiListComponent } from './che-do-uu-dai-list/che-do-uu-dai-list.component';
import { chedouudaiComponent } from './che-do-uu-dai.component';
import { chedouudaiService } from './Services/che-do-uu-dai.service';
import { chedouudaiRefModule } from './che-do-uu-dai-ref.module';
import { DPSCommonModule } from '../../dps-common.module'; 
const routes: Routes = [
	{
		path: '',
		component: chedouudaiComponent,
		children: [
			{
				path: '',
				component: chedouudaiListComponent,
			},
		]
	}
];

@NgModule({
	imports: [
		RouterModule.forChild(routes),
		DPSCommonModule,
        chedouudaiRefModule,
	],
	providers: [
		chedouudaiService
	],
	entryComponents: [
	],
	declarations: [
		chedouudaiComponent,
	]
})
export class CheDoUuDaiModule { }
