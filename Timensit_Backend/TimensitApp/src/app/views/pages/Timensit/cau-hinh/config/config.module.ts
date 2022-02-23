// Angular
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
//Share
import { DPSCommonModule } from '../../dps-common.module';
//Component
import { ConfigComponent } from './config.component';
import { ConfigListComponent } from './config-list/config-list.component';
import { ConfigEditComponent } from './config-edit/config-edit.component';
//Service
import { ConfigService } from './config-service/config.service';

const routes: Routes = [
	{
		path: '',
		component: ConfigComponent,
		children: [
			{
				path: '',
				component: ConfigListComponent,
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
		ConfigService
	],
	entryComponents: [
		ConfigEditComponent
	],
	declarations: [
		ConfigComponent,
		ConfigListComponent,
		ConfigEditComponent
	]
})
export class ConfigModule {}
