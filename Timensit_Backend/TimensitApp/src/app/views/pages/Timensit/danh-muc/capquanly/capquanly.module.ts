import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { capquanlyListComponent } from './capquanly-list/capquanly-list.component';
import { capquanlyEditDialogComponent } from './capquanly-edit/capquanly-edit.dialog.component';
import { capquanlyComponent } from './capquanly.component';
import { capquanlyService } from './Services/capquanly.service';
import { capquanlyRefModule } from './capquanly-ref.module';
import { DPSCommonModule } from '../../dps-common.module';

const routes: Routes = [
	{
		path: '',
		component: capquanlyComponent,
		children: [
			{
				path: '',
				component: capquanlyListComponent,
			},
			{
				path: 'themmoi',
				component: capquanlyEditDialogComponent
			},
			{
				path: 'chinhsua/:id',
				component: capquanlyEditDialogComponent
			},
		]
	}
];

@NgModule({
	imports: [
		RouterModule.forChild(routes),
		DPSCommonModule,
		capquanlyRefModule
	],
	providers: [
		capquanlyService
	],
	entryComponents: [
	],
	declarations: [
		capquanlyComponent
	]
})
export class capquanlyModule { }
