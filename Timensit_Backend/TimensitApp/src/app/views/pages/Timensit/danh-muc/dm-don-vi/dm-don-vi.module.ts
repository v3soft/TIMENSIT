// Angular
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import {DragDropModule} from '@angular/cdk/drag-drop';
import {ScrollingModule} from '@angular/cdk/scrolling';
import {CdkTableModule} from '@angular/cdk/table';
import {CdkTreeModule} from '@angular/cdk/tree';
// Translate Module
import { TranslateModule } from '@ngx-translate/core';

//Froala Module
// import { FroalaEditorModule, FroalaViewModule } from 'angular-froala-wysiwyg';
// UI
import { PartialsModule } from 'app/views/partials/partials.module';
// Auth
import { ModuleGuard } from 'app/core/auth';
// Core => Utils
import { HttpUtilsService,
	TypesUtilsService,
	InterceptService,
	LayoutUtilsService
} from 'app/core/_base/crud';

import { NgxPermissionsModule } from 'ngx-permissions';
import { NgbProgressbarModule, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';

//Share
import { ActionNotificationComponent } from 'app/views/partials/content/crud';
import { DeleteEntityDialogComponent } from 'app/views/partials/content/crud';
import { FetchEntityDialogComponent } from 'app/views/partials/content/crud';
import { UpdateStatusDialogComponent } from 'app/views/partials/content/crud';
import { AlertComponent } from 'app/views/partials/content/crud';



//Component
import { DM_DonViComponent } from './dm-don-vi.component';
import { DM_DonViListComponent } from './dm-don-vi-list/dm-don-vi-list.component';
import { DM_DonViEditComponent } from './dm-don-vi-edit/dm-don-vi-edit.component';
import { DM_DonViImportComponent } from './dm-don-vi-import/dm-don-vi-import.component';
//Service
import { DM_DonViService } from './dm-don-vi-service/dm-don-vi.service';

// Material
import {
	MatInputModule,
	MatPaginatorModule,
	MatProgressSpinnerModule,
	MatSortModule,
	MatTableModule,
	MatSelectModule,
	MatMenuModule,
	MatProgressBarModule,
	MatButtonModule,
	MatCheckboxModule,
	MatDialogModule,
	MatTabsModule,
	MatNativeDateModule,
	MatCardModule,
	MatRadioModule,
	MatIconModule,
	MatDatepickerModule,
	MatAutocompleteModule,
	MAT_DIALOG_DEFAULT_OPTIONS,
	MatSnackBarModule,
	MatTooltipModule,
	MatChipsModule
} from '@angular/material';
import { DPSCommonModule } from '../../dps-common.module';
import { DmNguoiDungDonViListComponent } from './dm-nguoi-dung-don-vi-list/dm-nguoi-dung-don-vi-list.component';
import { ImageControlModule } from 'dps-lib';

const routes: Routes = [
	{
		path: '',
		component: DM_DonViComponent,
		children: [
			{
				path: '',
				component: DM_DonViComponent,
			}
		]
	}
];

@NgModule({
    imports: [
        NgbModule,
		MatDialogModule,
		CommonModule,
		HttpClientModule,
		PartialsModule,
		NgxPermissionsModule.forChild(),
		NgxMatSelectSearchModule,
		RouterModule.forChild(routes),
		FormsModule,
		ReactiveFormsModule,
		TranslateModule.forChild(),
		MatButtonModule,
		MatMenuModule,
		MatSelectModule,
        MatInputModule,
		MatTableModule,
		MatAutocompleteModule,
		MatRadioModule,
		MatIconModule,
		MatNativeDateModule,
		MatProgressBarModule,
		MatDatepickerModule,
		MatCardModule,
		MatPaginatorModule,
		MatSortModule,
		MatCheckboxModule,
		MatProgressSpinnerModule,
		MatSnackBarModule,
		MatTabsModule,
		MatTooltipModule,
		NgbProgressbarModule,
		// FroalaEditorModule.forRoot(),
		// FroalaViewModule.forRoot(),
		MatChipsModule,
		DragDropModule,
		ScrollingModule,
		CdkTableModule,
		CdkTreeModule,
		DPSCommonModule,
		ImageControlModule
		
	],
	providers: [
		ModuleGuard,
		InterceptService,
      	{
        	provide: HTTP_INTERCEPTORS,
       	 	useClass: InterceptService,
        	multi: true
      	},
		{
			provide: MAT_DIALOG_DEFAULT_OPTIONS,
			useValue: {
				hasBackdrop: true,
				panelClass: 'kt-mat-dialog-container__wrapper',
				height: 'auto',
				width: '900px'
			}
		},
		TypesUtilsService,
		LayoutUtilsService,
		HttpUtilsService,
		DM_DonViService
	],
	entryComponents: [
		ActionNotificationComponent,
		DeleteEntityDialogComponent,
		UpdateStatusDialogComponent,
		FetchEntityDialogComponent,
		AlertComponent,
		DM_DonViEditComponent,
		DM_DonViImportComponent
	],
	declarations: [
		DM_DonViComponent,
		DM_DonViListComponent,
		DM_DonViEditComponent,
		DM_DonViImportComponent,
		DmNguoiDungDonViListComponent
	]
})
export class DM_DonViModule {}
