import { NgModule } from '@angular/core';
import { capquanlyListComponent } from './capquanly-list/capquanly-list.component';
import { capquanlyService } from './Services/capquanly.service';
import { DPSCommonModule } from '../../dps-common.module';
import { capquanlyEditDialogComponent } from './capquanly-edit/capquanly-edit.dialog.component';

@NgModule({
	imports: [
		DPSCommonModule,
	],
	providers: [
		capquanlyService
	],
	entryComponents: [
		capquanlyListComponent
	],
	declarations: [
		capquanlyListComponent,
		capquanlyEditDialogComponent 
	],
	exports:[
		capquanlyListComponent
	]
})
export class capquanlyRefModule { }
