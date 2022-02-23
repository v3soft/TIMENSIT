// Angular
import { Component, OnInit, ChangeDetectionStrategy, OnDestroy, ChangeDetectorRef, Inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
// Material
import { MatPaginator, MatSort, MatDialog, MatTableDataSource, MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { SelectionModel } from '@angular/cdk/collections';
// RxJS
import { Observable, BehaviorSubject, Subscription } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
// NGRX
// Service
import { SubheaderService } from 'app/core/_base/layout';
import { LayoutUtilsService,  MessageType } from 'app/core/_base/crud';

import moment from 'moment';
import { DM_DonViService } from '../../danh-muc/dm-don-vi/dm-don-vi-service/dm-don-vi.service';

@Component({
	selector: 'kt-tree-don-vi-dialog',
	templateUrl: './tree-don-vi-dialog.component.html',
	// changeDetection: ChangeDetectionStrategy.OnPush,
})
	
export class TreeDonViDialogComponent implements OnInit, OnDestroy {
	// Public properties
	DM_YKienForm: FormGroup;
	hasFormErrors: boolean = false;
	disabledBtn:boolean=false;
	loadingSubject = new BehaviorSubject<boolean>(true);
	loading$: Observable<boolean>;
	viewLoading: boolean = false;
	isChange: boolean = false;
	fixedPoint = 0;
	isZoomSize: boolean = false;
	donvi: string = "";
	dataTreeDonVi: any[] = [];

	private componentSubscriptions: Subscription;

	constructor(
		public dialogRef: MatDialogRef<TreeDonViDialogComponent>,
		@Inject(MAT_DIALOG_DATA) public data: any,		
		public dialog: MatDialog,
		private layoutUtilsService: LayoutUtilsService,
		// private changeDetectorRefs: ChangeDetectorRef,
		private service: DM_DonViService) {}

	/**
	 * On init
	 */
	async ngOnInit() {
		this.GetTreeDonVi();
	}
	GetTreeDonVi() {
		this.viewLoading = true;

		this.service.GetTreeDonVi().subscribe(res => {
			//console.log("data tree", res);
			// res.data.anCss= {
			// 	collapse: true,
			// 	lastChild: false,
			// 	state: 0,//trạng thái luôn luôn mở node này, 0 -> open, -1 -> close
			// 	checked: false,
			// 	parentChk: ''
			// }
			// this.dataTreeDonVi=res.data;			
			this.viewLoading = false;	
			let tree = [];
			if(res.data){
				res.data.forEach(element => {
					let item = element;
					item.anCss= {
						collapse: true,
						lastChild: false,
						state: 0,//trạng thái luôn luôn mở node này, 0 -> open, -1 -> close
						checked: false,
						parentChk: ''
					}
					tree.push(item);
				});
			}
			this.dataTreeDonVi=tree;
		});
	}
	
	treeDonViChanged(item){
		// console.log("abc", item);
		if(item){
			// this.donvi=item.data.IdGroup;
			this.dialogRef.close(item);
		}		
	}
	/**
	 * On destroy
	 */
	ngOnDestroy() {
		if (this.componentSubscriptions) {
			this.componentSubscriptions.unsubscribe();
		}
	}
	
	/**
	 * Create form
	 */
	
	/**
	 * Check control is invalid
	 * @param controlName: string
	 */
	isControlInvalid(controlName: string): boolean {
		const control = this.DM_YKienForm.controls[controlName];
		const result = control.invalid && control.touched;
		return result;
	}

	/**
	 * Save data
	 *
	 * @param withBack: boolean
	 */
	
	/**
	 * Close alert
	 *
	 * @param $event
	 */
	onAlertClose($event) {
		this.hasFormErrors = false;
	}

	numberOnly(event): boolean {
		const charCode = (event.which) ? event.which : event.keyCode;
		if (charCode > 31 && (charCode < 48 || charCode > 57)) {
			return false;
		}
		return true;
	}

	closeDialog(){
		this.dialogRef.close(this.isChange);
	}
	
}
