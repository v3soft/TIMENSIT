// Angular
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
//Share
import { DPSCommonModule } from '../../dps-common.module';
//Component
import { CauHinhEmailComponent } from './cau-hinh-email.component';
import { CauHinhEmailListComponent } from './cau-hinh-email-list/cau-hinh-email-list.component';
import { CauHinhEmailEditComponent } from './cau-hinh-email-edit/cau-hinh-email-edit.component';
//Service
import { CauHinhEmailService } from './cau-hinh-email-service/cau-hinh-email.service';
import { CauHinhEmailPopupDVCComponent } from './cau-hinh-email-popup-donvicon/cau-hinh-email-popup-donvicon.component';

const routes: Routes = [
	{
		path: '',
		component: CauHinhEmailComponent,
		children: [
			{
				path: '',
				component: CauHinhEmailListComponent,
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
		CauHinhEmailService
	],
	entryComponents: [
		CauHinhEmailEditComponent,
		CauHinhEmailPopupDVCComponent
	],
	declarations: [
		CauHinhEmailComponent,
		CauHinhEmailListComponent,
		CauHinhEmailEditComponent,
		CauHinhEmailPopupDVCComponent
	]
})
export class CauHinhEmailModule {}
