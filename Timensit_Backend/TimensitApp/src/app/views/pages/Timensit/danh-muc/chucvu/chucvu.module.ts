import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { chucvuListComponent } from './chucvu-list/chucvu-list.component';
import { chucvuComponent } from './chucvu.component';
import { chucvuService } from './Services/chucvu.service';
import { chucvuRefModule } from './chucvu-ref.module';
import { DPSCommonModule } from '../../dps-common.module';
const routes: Routes = [
	{
		path: '',
		component: chucvuComponent,
		children: [
			{
				path: '',
				component: chucvuListComponent,
			},
		]
	}
];

@NgModule({
	imports: [
		RouterModule.forChild(routes),
		DPSCommonModule,
        chucvuRefModule,
	],
	providers: [
		chucvuService
	],
	entryComponents: [
	],
	declarations: [
		chucvuComponent,
	]
})
export class chucvuModule { }
