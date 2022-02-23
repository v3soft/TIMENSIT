import { NestedTreeControl, FlatTreeControl } from '@angular/cdk/tree';
import { Component, ChangeDetectionStrategy, OnInit, Inject, ChangeDetectorRef, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog, MatTreeFlattener, MatTreeFlatDataSource, MatTableDataSource } from '@angular/material';
import { LayoutUtilsService } from '../../../../../core/_base/crud';
import { CommonService } from '../../services/common.service';
import { saveAs } from 'file-saver';

export class TodoItemNode {
	data?: TodoItemNode[];
	title: string;
	selected?: boolean;
	id?: any;
}
@Component({
	selector: 'kt-list-file-dinh-kem',
	templateUrl: './list-file-dinh-kem.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ListFileDinhKemComponent implements OnInit {
	// Public properties
	loadingSubject = new BehaviorSubject<boolean>(true);
	loading$: Observable<boolean>;
	viewLoading: boolean = false;
	isZoomSize: boolean = false;

	disabledBtn: boolean = false;
	datasource = new MatTableDataSource([]);
	displayedColumns: string[] = ["stt", "filename", "Version", "CreatedDate", "CreatedBy", "UpdatedDate", "UpdatedBy", "actions"];
	private files: Array<any> = [];
	constructor(
		public dialogRef: MatDialogRef<ListFileDinhKemComponent>,
		@Inject(MAT_DIALOG_DATA) public data: any,
		public dialog: MatDialog,
		private layoutUtilsService: LayoutUtilsService,
		private changeDetectorRefs: ChangeDetectorRef,
		private commonService: CommonService) { }

	/**
	 * On init
	 */
	async ngOnInit() {
		this.viewLoading = true;
		if (this.data) {
			this.commonService.get_dinhkem(this.data.Loai, this.data.Id).subscribe(res => {
				if (res && res.status == 1) {
					this.files = res.data;
					this.datasource = new MatTableDataSource(this.files);
				}
				else
					this.layoutUtilsService.showError(res.error.message);
				this.viewLoading = false;
				this.changeDetectorRefs.detectChanges();
			})
		}
	}

	closeDialog() {
		this.dialogRef.close();
	}

	resizeDialog() {
		if (!this.isZoomSize) {
			this.dialogRef.updateSize('100vw', '100vh');
			this.isZoomSize = true;
		}
		else if (this.isZoomSize) {
			this.dialogRef.updateSize('900px', 'auto');
			this.isZoomSize = false;
		}

	}
	download(index) {
		this.commonService.download_dinhkem(this.files[index].IdRow).subscribe(res => {
			if (res && res.status == 1) {
				let blob = new Blob([res.data.FileContents], { type: res.data.ContentType });
				saveAs(blob, res.data.FileDownloadName);
			}
			else
				this.layoutUtilsService.showInfo(res.error.message);
		});
	}
}
