// Angular
import { NgModule, ModuleWithProviders } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
//Share
import { DPSCommonModule } from '../dps-common.module';
//Component
import { FileViewerComponent } from './file-viewer.component';
import { FileDinhKemViewerComponent } from './file-dinh-kem/file-dinh-kem.component';
import { AuthGuard } from '../../../../core/auth';
//Service

const routes: Routes = [
	{
		path: '',
		component: FileViewerComponent,
		canActivate: [AuthGuard],
		children: [
			{
				path: '',
				redirectTo: '/',
				pathMatch: 'full'
			},
			{
				path: 'file-dinh-kem/:id',
				component: FileDinhKemViewerComponent
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
	],
	entryComponents: [
	],
	declarations: [
		FileViewerComponent,
		FileDinhKemViewerComponent
	]
})
export class FileViewerModule {
	static forRoot(): ModuleWithProviders {
		return {
			ngModule: FileViewerModule,
			providers: []
		};
	}
}
