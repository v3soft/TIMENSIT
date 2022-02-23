// Angular
import { Component, OnInit, ChangeDetectionStrategy, OnDestroy, ChangeDetectorRef, Inject, ViewChild } from '@angular/core';
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
import { LayoutUtilsService, MessageType } from 'app/core/_base/crud';
import { DM_DonViService } from '../dm-don-vi-service/dm-don-vi.service';
//Models

import { DM_DonViModel } from '../dm-don-vi-model/dm-don-vi.model';


@Component({
	selector: 'kt-dm-don-vi-import',
	templateUrl: './dm-don-vi-import.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DM_DonViImportComponent implements OnInit, OnDestroy {
	// Public properties

	@ViewChild('fileUpload', { static: true }) fileUpload;

	
		DM_DonVi: DM_DonViModel;
	
	DM_DonViForm: FormGroup;
	hasFormErrors: boolean = false;

	loadingSubject = new BehaviorSubject<boolean>(true);
	loading$: Observable<boolean>;

	viewLoading: boolean = false;
	isChange: boolean = false;
	_soLanImport: number = 0;
	_dataImport: any[] = [];
	disabledBtn:boolean=false;

	private componentSubscriptions: Subscription;

	constructor(
		public dialogRef: MatDialogRef<DM_DonViImportComponent>,
		private DM_DonViFB: FormBuilder,
		public dialog: MatDialog,
		private layoutUtilsService: LayoutUtilsService,
		private changeDetectorRefs: ChangeDetectorRef,
		private dm_donvisService: DM_DonViService) {}
	/**
	 * On init
	 */
	ngOnInit() {
		this.viewLoading = false;
		this.dm_donvisService.data_import.subscribe(res => {
			this._dataImport = [...res];
		});
		this.createForm();
	}
	/**
	 * On destroy
	 */
	ngOnDestroy() {
		if (this.componentSubscriptions) {
			this.componentSubscriptions.unsubscribe();
		}
		this.dm_donvisService.data_import.next([]);
	}
	/**
	 * Create form
	 */
	createForm() {
		this.DM_DonViForm = this.DM_DonViFB.group({
			FileDuLieu: [''],
		});
	}

	/**
	 * Check control is invalid
	 * @param controlName: string
	 */
	isControlInvalid(controlName: string): boolean {
		const control = this.DM_DonViForm.controls[controlName];
		const result = control.invalid && control.touched;
		return result;
	}

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
	closeDialog() {
		this.dialogRef.close(this.isChange); 
	}
	selectFile() {
		let el: HTMLElement = this.fileUpload.nativeElement as HTMLElement;
		el.click();
	}
	FileSelected(evt: any) {
		if (evt.target.files && evt.target.files.length) {//Nếu có file
			let file = evt.target.files[0]; // Ví dụ chỉ lấy file đầu tiên
			let fileName = file.name;

			var res = fileName.match(/.xls$|.xlsx$/g);
			if (res) {
				if (!res["includes"]('.xlsx') && !res["includes"]('.xls')) {
					this.layoutUtilsService.showError('File không hợp lệ.');
					return;
				}
				else {
					this.DM_DonViForm.controls['FileDuLieu'].patchValue(fileName); // Set value cho control dùng để validate (trường hợp base64)
					this.DocDuLieu();
				}
			}
			else {
				this.layoutUtilsService.showError('File không hợp lệ');
				return;
			}
		}
		else {//Không có file
			this.DM_DonViForm.controls['FileDuLieu'].patchValue('');
		}
	}

	checkDataIsValid(): boolean {
		let p = document.getElementById("fileUploadExcel");
		return this.DM_DonViForm.controls['FileDuLieu'] && this.DM_DonViForm.controls['FileDuLieu'].valid && (p ? (p["type"] == 'file' ? p["files"]["length"] > 0 : false) : false);
	}

	DocDuLieu() {
		
		let t = this.checkDataIsValid();
		if (t) this.Importfile();
		else this.layoutUtilsService.showError("Mời chọn file");
	}

	Importfile() {
		let el: any = this.fileUpload.nativeElement;
		var service = this.dm_donvisService;
		var useBase64: boolean = true;
		for (var idx = 0; idx < el.files.length; idx++) {
			if (useBase64) {
				var fileName = el.files[idx].name;
				let reader = new FileReader();
				reader.readAsDataURL(el.files[idx]);
				reader.onload = function () {
					let base64Str = reader.result as String;
					var metaIdx = base64Str.indexOf(';base64,');
					base64Str = base64Str.substr(metaIdx + 8); // Cắt meta data khỏi chuỗi base64
					var data = {
						fileName: fileName,
						base64: base64Str,
					};
					service.lastFileUpload$.next(data);
					service.uploadFileDM_DonVi(data).subscribe(res => {
						if (res && res.status == 1) {
							service.data_import.next(res.data);
						}
						else {
							service.lastFilterDSExcel$.next([]);
							service.lastFilterInfoExcel$.next(undefined);
							return;
						}
					});
				};
			}
			else {
				let inputs = new FormData();
				inputs.append('file', el.files[idx]);
			}
		}
	}
	luuImport() {
		if (this._dataImport.length > 0) {
			this.dm_donvisService.importDM_DonVi(this._dataImport).subscribe(res => {
				if (res && res.status == 1) {
					this.isChange = true;
					this.DM_DonViForm.controls['FileDuLieu'].setValue('');
					this._dataImport = [];
					this.layoutUtilsService.showInfo('Import thành công!');
					this.dialogRef.close(this.isChange);
				}
				else {
					this.DM_DonViForm.controls['FileDuLieu'].setValue('');
					this._dataImport = [];
					this.layoutUtilsService.showError('Import thất bại, vui lòng kiểm tra lại file excel!');
				}
			});
		}
		else {
			this.layoutUtilsService.showError('Không có dữ liệu để import hoặc dữ liệu sai, vui lòng kiểm tra lại!');
			return;
		}
	}

	ImportFileMau() {
		let linkdownload = this.dm_donvisService.downloadTemplate();
		window.open(linkdownload);
	}
}
