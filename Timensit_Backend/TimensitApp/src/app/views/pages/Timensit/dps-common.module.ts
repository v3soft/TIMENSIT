// Angular
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { CdkTableModule } from '@angular/cdk/table';
import { CdkTreeModule } from '@angular/cdk/tree';
// Translate Module
import { TranslateModule } from '@ngx-translate/core';

// UI
import { PartialsModule } from '../../../views/partials/partials.module';
// Auth
import { ModuleGuard } from '../../../core/auth';
// Core => Utils
import {
	HttpUtilsService,
	TypesUtilsService,
	InterceptService,
	LayoutUtilsService
} from '../../../core/_base/crud';

import { NgxPermissionsModule } from 'ngx-permissions';
import { NgbProgressbarModule, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';

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
	MatChipsModule,

	MatPaginatorIntl,

	MatToolbarModule
} from '@angular/material';

//Datetime format
import { MomentDateAdapter } from '@angular/material-moment-adapter';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { MY_FORMATS_EDIT } from './datepicker';

import { DropdownTreeModule, DatetimePickerModule, ImageControlModule, DynamicComponentModule } from 'dps-lib';//,

//Share
import { ActionNotificationComponent, DeleteEntityDialogComponent, FetchEntityDialogComponent, UpdateStatusDialogComponent, AlertComponent } from '../../partials/content/crud';
import { CommonService } from './services/common.service';
import { CustomMatPaginatorIntl } from './custom-mat-pagination-int';
import { MatTreeModule } from '@angular/material/tree';
import { ColorPickerModule } from 'ngx-color-picker';
import { MatExpansionModule } from '@angular/material/expansion';
import { NgxPrintModule } from 'ngx-print';
import { NgIdleKeepaliveModule } from '@ng-idle/keepalive';
import { TableService } from '../../partials/table/table.service';
import { ColumnFilterComponent, ColumnOptionComponent } from '../../partials/table';
import {
	ChonDonViComponent,
	ChonNhieuDonViComponent,
	ChonVaiTroComponent,
	TreeDonViNodeComponent,
	TreeDonViComponent,
	TreeDonViDialogComponent,
	ListFileDinhKemComponent,
	ChonNhieuNhanVienListComponent,
	CommentComponent,
	CommentEditDialogComponent,
	EmotionDialogComponent,
	ChooseUsersComponent,
	ReviewExportComponent,
	DisplayHtmlContentComponent,
	LoadingComponent,
	// TroCapEditComponent,
	// QuyetDinhEditComponent,
	// QuaTrinhHoatDongEditComponent,
	NguoiDungDonViComponent,

} from './components';
import { PopoverModule } from 'ngx-smart-popover';
import { CommentService } from './components/comment/comment.service';
import { CanvasDemoModule } from './components/canvas-demo/canvas-demo.module';
import { DynamicProcessService } from './services/dynamic-process.service';
import { RouterModule } from '@angular/router';
import { NgxMaskModule, IConfig } from 'ngx-mask'
import { LoadingService } from './services/loading.service';
import { LoadingInterceptor } from './services/loading.interceptor';
import { AngularEditorModule } from '@kolkov/angular-editor';
import { EditorModule } from '@tinymce/tinymce-angular';
import { investorEditDialogComponent } from './danh-muc/investor/investor-edit/investor-edit.dialog.component';
import { contractEditDialogComponent } from './danh-muc/contract/contract-edit/contract-edit.dialog.component';

export const options: Partial<IConfig> | (() => Partial<IConfig>) = null;

@NgModule({
	imports: [
		RouterModule,
		NgbModule,
		MatDialogModule,
		CommonModule,
		HttpClientModule,
		PartialsModule,
		NgxPermissionsModule.forChild(),
		NgxMatSelectSearchModule,
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
		MatChipsModule,
		DragDropModule,
		ScrollingModule,
		CdkTableModule,
		CdkTreeModule,
		DropdownTreeModule,
		DatetimePickerModule,
		DynamicComponentModule,
		MatTreeModule,
		ImageControlModule,
		ColorPickerModule,
		MatExpansionModule,
		NgxPrintModule,
		MatToolbarModule,
		DynamicComponentModule,
		MatCardModule,
		PopoverModule,
		CanvasDemoModule,
		NgxMaskModule.forRoot(),
		//NgIdleKeepaliveModule.forRoot(),
		AngularEditorModule,
		EditorModule
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
				width: '70%'
			}
		},
		TypesUtilsService,
		LayoutUtilsService,
		HttpUtilsService,
		{ provide: MAT_DATE_LOCALE, useValue: 'vi' },
		{ provide: DateAdapter, useClass: MomentDateAdapter, deps: [MAT_DATE_LOCALE] },
		{ provide: MAT_DATE_FORMATS, useValue: MY_FORMATS_EDIT },
		{
			provide: MatPaginatorIntl,
			useClass: CustomMatPaginatorIntl
		},
		CommonService,
		TableService,
		CommentService,
		DynamicProcessService,
		LoadingService,
		{
			provide: HTTP_INTERCEPTORS,
			useClass: LoadingInterceptor,
			multi: true
		}
	],
	declarations: [
		ColumnFilterComponent,
		ColumnOptionComponent,
		ChonDonViComponent,
		ChonNhieuDonViComponent,
		ChonVaiTroComponent,
		TreeDonViNodeComponent,
		TreeDonViComponent,
		TreeDonViDialogComponent,
		ListFileDinhKemComponent,
		ChonNhieuNhanVienListComponent,
		CommentComponent,
		CommentEditDialogComponent,
		EmotionDialogComponent,
		ChooseUsersComponent,
		ReviewExportComponent,
		DisplayHtmlContentComponent,
		LoadingComponent,
		// TroCapEditComponent,
		// QuyetDinhEditComponent,
		// QuaTrinhHoatDongEditComponent,
		NguoiDungDonViComponent,
		investorEditDialogComponent,
		contractEditDialogComponent
	],
	entryComponents: [
		ActionNotificationComponent,
		DeleteEntityDialogComponent,
		UpdateStatusDialogComponent,
		FetchEntityDialogComponent,
		AlertComponent,
		ColumnFilterComponent,
		ColumnOptionComponent,
		ChonDonViComponent,
		ChonNhieuDonViComponent,
		ChonVaiTroComponent,
		TreeDonViNodeComponent,
		TreeDonViComponent,
		TreeDonViDialogComponent,
		ListFileDinhKemComponent,
		ChonNhieuNhanVienListComponent,
		CommentComponent,
		CommentEditDialogComponent,
		EmotionDialogComponent,
		ChooseUsersComponent,
		ReviewExportComponent,
		DisplayHtmlContentComponent,
		LoadingComponent,
		// TroCapEditComponent,
		// QuyetDinhEditComponent,
		// QuaTrinhHoatDongEditComponent,
		NguoiDungDonViComponent,
		investorEditDialogComponent,
		contractEditDialogComponent

	],
	exports: [
		RouterModule,
		NgbModule,
		MatDialogModule,
		CommonModule,
		HttpClientModule,
		PartialsModule,
		NgxPermissionsModule,
		NgxMatSelectSearchModule,
		FormsModule,
		ReactiveFormsModule,
		TranslateModule,
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
		MatChipsModule,
		DragDropModule,
		ScrollingModule,
		CdkTableModule,
		CdkTreeModule,
		DropdownTreeModule,
		DatetimePickerModule,
		DynamicComponentModule,
		ImageControlModule,
		MatTreeModule,
		MatExpansionModule,
		NgxPrintModule,
		PopoverModule,
		CanvasDemoModule,
		NgxMaskModule,
		AngularEditorModule,
		ActionNotificationComponent,
		DeleteEntityDialogComponent,
		UpdateStatusDialogComponent,
		FetchEntityDialogComponent,
		AlertComponent,
		ColumnFilterComponent,
		ColumnOptionComponent,
		ChonDonViComponent,
		ChonNhieuDonViComponent,
		ChonVaiTroComponent,
		TreeDonViNodeComponent,
		TreeDonViComponent,
		TreeDonViDialogComponent,
		ListFileDinhKemComponent,
		ChonNhieuNhanVienListComponent,
		CommentComponent,
		CommentEditDialogComponent,
		EmotionDialogComponent,
		ChooseUsersComponent,
		ReviewExportComponent,
		LoadingComponent,
		// TroCapEditComponent,
		// QuyetDinhEditComponent,
		// QuaTrinhHoatDongEditComponent,
		NguoiDungDonViComponent,
		investorEditDialogComponent,
		contractEditDialogComponent
	]
})
export class DPSCommonModule { }
