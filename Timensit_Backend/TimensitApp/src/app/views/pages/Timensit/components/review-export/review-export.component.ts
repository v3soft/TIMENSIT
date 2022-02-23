import { Component, OnInit, ViewChild, ChangeDetectionStrategy, OnDestroy, ChangeDetectorRef, Inject, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DatePipe } from '@angular/common';
import { TranslateService } from '@ngx-translate/core';
// Material
import { MatPaginator, MatSort, MatDialog, MatMenuTrigger, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { SelectionModel } from '@angular/cdk/collections';
// RXJS
import { tap } from 'rxjs/operators';
import { merge, BehaviorSubject } from 'rxjs';
//Service
//Model

import { CommonService } from '../../services/common.service';
import { FormControl } from '@angular/forms';
import { LayoutUtilsService } from '../../../../../core/_base/crud';
import { DomSanitizer } from '@angular/platform-browser';


@Component({
	selector: 'kt-review-export',
	templateUrl: './review-export.component.html',
	encapsulation: ViewEncapsulation.None,
})
export class ReviewExportComponent implements OnInit {
	loadingSubject = new BehaviorSubject<boolean>(false);
	loading$ = this.loadingSubject.asObservable();
	viewLoading: boolean = false;
	isZoomSize: boolean = false;
	disabledBtn: boolean = false;
	strHtml: any;
	constructor(
		public dialogRef: MatDialogRef<ReviewExportComponent>,
		@Inject(MAT_DIALOG_DATA) public data: any,
		public dialog: MatDialog,
		private route: ActivatedRoute,
		private translate: TranslateService,
		private changeDetect: ChangeDetectorRef,
		private layoutUtilsService: LayoutUtilsService,
		private commonService: CommonService,
		private sanitized: DomSanitizer) { }
	transform(value) {
		return this.sanitized.bypassSecurityTrustHtml(value);
	}

	/** LOAD DATA */
	ngOnInit() {
		this.strHtml = this.sanitized.bypassSecurityTrustHtml(this.data)
	}
	//loai: 1 word, 2 excel, 3 pdf
	in(loai) {
		this.dialogRef.close({ loai: loai });
	}

	closeDialog() {
		this.dialogRef.close();
	}
}
