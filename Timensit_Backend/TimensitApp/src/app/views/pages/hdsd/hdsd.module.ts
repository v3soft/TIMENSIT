import { hdsdService } from './Services/hdsd.service';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { hdsdListComponent } from './hdsd-list/hdsd-list.component';
import { hdsdComponent } from './hdsd.component';
import { DPSCommonModule } from '../Timensit/dps-common.module';
import { HDSDEditDialogComponent } from './hdsd-edit/hdsd-edit.dialog.component';


const routes: Routes = [
	{
		path: '',
		component: hdsdComponent,
		children: [
			{
				path: '',
				component: hdsdListComponent,
			}
		]
	}
];

@NgModule({
	imports: [
		RouterModule.forChild(routes),
		DPSCommonModule
	],
	providers: [
		hdsdService
	],
	entryComponents: [
		HDSDEditDialogComponent
	],
	declarations: [
		hdsdComponent,
		hdsdListComponent,
		HDSDEditDialogComponent
	]
})
export class hdsdModule { }
