// Angular
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DPSCommonModule } from '../../dps-common.module';

//Component
import { NguoiDungDPSComponent } from './nguoi-dung-dps.component';
import { NguoiDungDPSListComponent } from './nguoi-dung-dps-list/nguoi-dung-dps-list.component';
import { NguoiDungDPSEditComponent } from './nguoi-dung-dps-edit/nguoi-dung-dps-edit.component';
//Service
import { NguoiDungDPSService } from './nguoi-dung-dps-service/nguoi-dung-dps.service';
import { NguoiDungVaiTroComponent } from './nguoi-dung-vai-tro/nguoi-dung-vai-tro.component';
import { NguoiDungDPSImportComponent } from './nguoi-dung-dps-import/nguoi-dung-dps-import.component';
import { NguoiDungDPSResetPasswordComponent } from './nguoi-dung-dps-reset-password/nguoi-dung-dps-reset-password.component';


const routes: Routes = [
	{
		path: '',
		component: NguoiDungDPSComponent,
		children: [
			{
				path: '',
				component: NguoiDungDPSListComponent,
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
		NguoiDungDPSService
	],
	entryComponents: [
		NguoiDungDPSEditComponent,
		NguoiDungDPSResetPasswordComponent,
		NguoiDungDPSImportComponent,
		NguoiDungVaiTroComponent
	],
	declarations: [
		NguoiDungDPSComponent,
		NguoiDungDPSListComponent,
		NguoiDungDPSEditComponent,
		NguoiDungDPSResetPasswordComponent,
		NguoiDungDPSImportComponent,
		NguoiDungVaiTroComponent
	]
})
export class NguoiDungDPSModule {}
