// Angular
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
//Share
import { DPSCommonModule } from '../../dps-common.module';
//Component
import { EmailHistoryComponent } from './email-history.component';
import { EmailHistoryListComponent } from './email-history-list/email-history-list.component';
//Service
import { EmailHistoryService } from './email-history-service/email-history.service';

const routes: Routes = [
	{
		path: '',
		component: EmailHistoryComponent,
		children: [
			{
				path: '',
				component: EmailHistoryListComponent,
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
		EmailHistoryService
	],
	entryComponents: [
	],
	declarations: [
		EmailHistoryComponent,
		EmailHistoryListComponent,
	]
})
export class EmailHistoryModule {}
