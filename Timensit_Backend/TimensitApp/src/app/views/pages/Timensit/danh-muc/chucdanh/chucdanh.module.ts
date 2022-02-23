import { CommonModule, } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { ChucDanhComponent } from './chucdanh.component';
// Core
// Core => Services

import { ChucDanhListComponent } from './chucdanh-list/chucdanh-list.component';
import { ChucDanhEditDialogComponent } from './chucdanh-edit/chucdanh-edit.dialog.component';
import { ChucDanhRefModule } from './chucdanh-ref.module';
import { DPSCommonModule } from '../../dps-common.module';
import { ChucDanhService } from './services/chucdanh.service';
import { NgModule } from '@angular/core';

const routes: Routes = [
	{
		path: '',
		component: ChucDanhComponent,
		children: [
			{
				path: '',
				component: ChucDanhListComponent,
			},
			{
				path: 'themmoi',
				component: ChucDanhEditDialogComponent
			},
			{
				path: 'chinhsua/:id',
				component: ChucDanhEditDialogComponent
			},
		]
	}
];

@NgModule({
	imports: [
		RouterModule.forChild(routes),
		DPSCommonModule,
		ChucDanhRefModule
	],
	providers: [
		ChucDanhService,
	],
	entryComponents: [
	],
	declarations: [
		ChucDanhComponent,
	]
})
export class ChucDanhModule { }
