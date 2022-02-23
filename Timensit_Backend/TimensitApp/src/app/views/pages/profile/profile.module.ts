import { DPSCommonModule } from './../Timensit/dps-common.module';
// Angular
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CommonModule } from '@angular/common';
// Core Module
import { CoreModule } from '../../../core/core.module';
import { PartialsModule } from '../../partials/partials.module';
import { ProfileComponent } from './profile.component';
import { CommonService } from '../Timensit/services/common.service';
import {
	MatDatepickerModule, MatInputModule, MatTooltipModule,
} from '@angular/material';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { MY_FORMATS_EDIT } from '../../pages/Timensit/datepicker';
import { MomentDateAdapter } from '@angular/material-moment-adapter';
import { LayoutUtilsService } from 'app/core/_base/crud';
import { ActionNotificationComponent } from 'app/views/partials/content/crud';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { PersonalInformationComponent } from './personal-information/personal-information.component';
import { OverviewComponent } from './overview/overview.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NotifyService } from './Services/notify.service';
import { NotifyListComponent } from './notify-list/notify-list.component';

const routes: Routes = [
	{
		path: '',
		component: ProfileComponent,
		children: [
			{
				path: '',
				redirectTo: 'personal-information',
				pathMatch: 'full'
				// redirect to `personal-information`
				// old value component: OverviewComponent
			},
			{
				path: 'personal-information',
				component: PersonalInformationComponent
			},
			{
				path: 'change-password',
				component: ChangePasswordComponent
			},
			{
				path: 'notification',
				component: NotifyListComponent
			}]
	}
]

@NgModule({
	imports: [
		CoreModule,
		RouterModule.forChild(routes),
		DPSCommonModule,
	],
	providers: [
		CommonService,
		LayoutUtilsService,
		//{ provide: MAT_DATE_FORMATS, useValue: MY_FORMATS_EDIT },
		{ provide: MAT_DATE_LOCALE, useValue: 'vi' },
		{ provide: DateAdapter, useClass: MomentDateAdapter, deps: [MAT_DATE_LOCALE] },
		{ provide: MAT_DATE_FORMATS, useValue: MY_FORMATS_EDIT },
		NotifyService
	],
	entryComponents: [
		ActionNotificationComponent,
	],
	declarations: [
		ProfileComponent,
		OverviewComponent,
		ChangePasswordComponent,
		PersonalInformationComponent,
		NotifyListComponent
	]
})
export class ProfileModule {
}
