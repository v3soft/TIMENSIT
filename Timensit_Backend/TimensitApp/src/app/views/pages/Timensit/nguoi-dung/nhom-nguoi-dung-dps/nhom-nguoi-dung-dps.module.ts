// Angular
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

//Component
import { NhomNguoiDungDPSComponent } from './nhom-nguoi-dung-dps.component';
import { NhomNguoiDungDPSListComponent } from './nhom-nguoi-dung-dps-list/nhom-nguoi-dung-dps-list.component';
import { NhomNguoiDungDPSEditComponent } from './nhom-nguoi-dung-dps-edit/nhom-nguoi-dung-dps-edit.component';
//Service
import { NhomNguoiDungDPSService } from './nhom-nguoi-dung-dps-service/nhom-nguoi-dung-dps.service';
import { DPSCommonModule } from '../../dps-common.module';
import { PhanQuyenComponent } from './phan-quyen/phan-quyen.component';
import { CdkTreeModule } from '@angular/cdk/tree';

// Material
const routes: Routes = [
	{
		path: '',
		component: NhomNguoiDungDPSComponent,
		children: [
			{
				path: '',
				component: NhomNguoiDungDPSListComponent,
			}
		]
	}
];

@NgModule({
    imports: [
		RouterModule.forChild(routes),
		DPSCommonModule,
		CdkTreeModule
	],
	providers: [
		NhomNguoiDungDPSService
	],
	entryComponents: [
		NhomNguoiDungDPSEditComponent,
		PhanQuyenComponent
	],
	declarations: [
		NhomNguoiDungDPSComponent,
		NhomNguoiDungDPSListComponent,
		NhomNguoiDungDPSEditComponent,
		PhanQuyenComponent
	]
})
export class NhomNguoiDungDPSModule {}
