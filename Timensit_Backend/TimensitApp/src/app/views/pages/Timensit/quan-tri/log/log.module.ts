// Angular
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
//Share
import { DPSCommonModule } from '../../dps-common.module';
//Component
import { LogComponent } from './log.component';
import { LogListComponent } from './log-list/log-list.component';
//Service
import { LogService } from './log-service/log.service';
import { FileListComponent } from './file-list/file-list.component';

const routes: Routes = [
	{
		path: '',
		component: LogComponent,
		children: [
			{
				path: '',
				component: LogListComponent,
			},
			{
				path: 'doi-tuong/:loai/:id',
				component: LogListComponent,
			},
		]
	}
];

@NgModule({
    imports: [
		RouterModule.forChild(routes),
		DPSCommonModule
	],
	providers: [
		LogService
	],
	entryComponents: [
		// LogEditComponent
	],
	declarations: [
		LogComponent,
		LogListComponent,
		FileListComponent
		// LogEditComponent
	]
})
export class LogModule {}
