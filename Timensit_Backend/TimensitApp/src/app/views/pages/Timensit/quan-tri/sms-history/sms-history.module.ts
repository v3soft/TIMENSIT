// Angular
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
//Share
import { DPSCommonModule } from '../../dps-common.module';
//Component
import { SMSHistoryComponent } from './sms-history.component';
import { SMSHistoryListComponent } from './sms-history-list/sms-history-list.component';
import { SMSHistoryEditComponent } from './sms-history-edit/sms-history-edit.component';
//Service
import { SMSHistoryService } from './sms-history-service/sms-history.service';
//import { SMSHistoryPopupDVCComponent } from './sms-history-popup-donvicon/sms-history-popup-donvicon.component';

const routes: Routes = [
	{
		path: '',
		component: SMSHistoryComponent,
		children: [
			{
				path: '',
				component: SMSHistoryListComponent,
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
		SMSHistoryService
	],
	entryComponents: [
		SMSHistoryEditComponent
		//SMSHistoryEditComponent,
		//SMSHistoryPopupDVCComponent
	],
	declarations: [
		SMSHistoryComponent,
		SMSHistoryListComponent,
		SMSHistoryEditComponent
		//SMSHistoryEditComponent,
		//SMSHistoryPopupDVCComponent
	]
})
export class SMSHistoryModule {}
