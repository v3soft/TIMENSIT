// Angular
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
//Share
import { DPSCommonModule } from '../../dps-common.module';
//Component
import { CauHinhSMSComponent } from './cau-hinh-sms.component';
import { CauHinhSMSListComponent } from './cau-hinh-sms-list/cau-hinh-sms-list.component';
import { CauHinhSMSEditComponent } from './cau-hinh-sms-edit/cau-hinh-sms-edit.component';
//Service
import { CauHinhSMSService } from './cau-hinh-sms-service/cau-hinh-sms.service';
import { CauHinhSMSPopupDVCComponent } from './cau-hinh-sms-popup-donvicon/cau-hinh-sms-popup-donvicon.component';

const routes: Routes = [
	{
		path: '',
		component: CauHinhSMSComponent,
		children: [
			{
				path: '',
				component: CauHinhSMSListComponent,
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
		CauHinhSMSService
	],
	entryComponents: [
		CauHinhSMSEditComponent,
		CauHinhSMSPopupDVCComponent
	],
	declarations: [
		CauHinhSMSComponent,
		CauHinhSMSListComponent,
		CauHinhSMSEditComponent,
		CauHinhSMSPopupDVCComponent
	]
})
export class CauHinhSMSModule {}
