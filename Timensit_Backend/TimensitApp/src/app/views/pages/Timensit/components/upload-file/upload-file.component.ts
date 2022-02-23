// Angular
import { Component, OnInit, ChangeDetectionStrategy, OnDestroy, ChangeDetectorRef, Inject, Input, Output, EventEmitter, ViewEncapsulation, OnChanges } from '@angular/core';
// Service
import { LayoutUtilsService, MessageType } from 'app/core/_base/crud';
import { CommonService } from '../../services/common.service';
//Models
import { MatTableDataSource } from '@angular/material';
import { FormControl } from '@angular/forms';
import { saveAs } from 'file-saver';

@Component({
	selector: 'kt-upload-file',
	templateUrl: './upload-file.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
	encapsulation: ViewEncapsulation.None
})

export class UploadFileComponent implements OnInit, OnChanges {
	/** File extension that accepted, same as 'accept' of <input type="file" />. 
          By the default, it's set to 'image/*'. */
	@Input() accept = '';
	@Input() multiple: boolean = false;
	@Input() allowEdit: boolean = true;
	@Input() value: any[] = [];
	@Output() valueChange: EventEmitter<any[]> = new EventEmitter<any[]>();

	datasource = new MatTableDataSource([]);
	displayedColumns: string[] = ["stt", "filename", "actions"];
	private files: Array<any> = [];
	private files_deleted: Array<any> = [];
	fileControl: FormControl;
	setting: any = {
		ACCEPT_DINHKEM: '',
		MAX_SIZE: 0
	};
	constructor(
		private layoutUtilsService: LayoutUtilsService,
		private changeDetectorRefs: ChangeDetectorRef,
		private commonService: CommonService) { }

	/**
	 * On init
	 */
	async ngOnInit() {
		this.fileControl = new FormControl();
		this.fileControl.valueChanges.subscribe(x => {
			this.files = x;
			this.datasource = new MatTableDataSource(this.files);
			this.valueChange.emit(this.files.concat(this.files_deleted));
		})

		await this.getConfig().then(res => {
			if (res && res.status == 1) {
				this.setting = res.data;
				this.accept = this.setting.ACCEPT_DINHKEM;
			}
			else
				this.layoutUtilsService.showError(res.error.message);
			this.changeDetectorRefs.detectChanges();
		})
	}
	/**
	 * On change
	 */
	ngOnChanges() {
		if (this.fileControl) {
			if (this.value) {
				this.files = this.value.map(x => Object.assign({}, x));
			} else
				this.files = [];
			this.fileControl.setValue(this.files);
			this.datasource = new MatTableDataSource(this.files);
		}
	}

	delete(index) {
		let temp = this.files[index];
		if (temp.IdRow > 0) {
			temp.IsDel = true;
			this.files_deleted.push(temp);
		}
		this.value = this.files;
		this.files.splice(index, 1);
		this.datasource = new MatTableDataSource(this.files);
		this.valueChange.emit(this.files.concat(this.files_deleted));
		this.changeDetectorRefs.detectChanges();
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
	async getConfig() {
		return await this.commonService.getConfig(["ACCEPT_DINHKEM", "MAX_SIZE"]).toPromise();
	}
}
