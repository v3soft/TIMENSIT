// Angular
import { Component, OnInit, ChangeDetectionStrategy, OnDestroy, ChangeDetectorRef, Inject, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
// Material
import { MatPaginator, MatSort, MatDialog, MatTableDataSource, MAT_DIALOG_DATA, MatDialogRef, MatChipInputEvent, MatChipList } from '@angular/material';
import { SelectionModel } from '@angular/cdk/collections';
// RxJS
import { Observable, BehaviorSubject, Subscription } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
// NGRX
// Service
import { SubheaderService } from 'app/core/_base/layout';
import { LayoutUtilsService, MessageType } from 'app/core/_base/crud';
import { ConfigService } from '../config-service/config.service';
import { CommonService } from '../../../services/common.service';
//Models

import { SysConfigModel } from '../config-model/config.model';

import moment from 'moment';
import { ENTER, COMMA } from '@angular/cdk/keycodes';

@Component({
	selector: 'kt-config-edit',
	templateUrl: './config-edit.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ConfigEditComponent implements OnInit, OnDestroy {
	// Public properties
	Config: SysConfigModel;
	ConfigForm: FormGroup;
	hasFormErrors: boolean = false;
	disabledBtn: boolean = false;
	loadingSubject = new BehaviorSubject<boolean>(true);
	loading$: Observable<boolean>;
	viewLoading: boolean = false;
	isChange: boolean = false;
	isZoomSize: boolean = false;
	private componentSubscriptions: Subscription;
	allowEdit: boolean = true;

	//chip
	@ViewChild("chipList", { static: true }) chipList: MatChipList;
	readonly separatorKeysCodes: number[] = [ENTER, COMMA];
	chips: string[] = [];
	constructor(
		public dialogRef: MatDialogRef<ConfigEditComponent>,
		@Inject(MAT_DIALOG_DATA) public data: any,
		private ConfigFB: FormBuilder,
		public dialog: MatDialog,
		private layoutUtilsService: LayoutUtilsService,
		private changeDetectorRefs: ChangeDetectorRef,
		private configsService: ConfigService,
		private commonService: CommonService) { }

	/**
	 * On init
	 */
	async ngOnInit() {
		this.commonService.fixedPoint = 0;
		this.viewLoading = true;
		this.Config = this.data.Config;
		this.allowEdit = this.data.allowEdit;
		this.createForm();
		if (this.data.Config && this.data.Config.IdRow > 0) {
			this.configsService.getConfigById(this.data.Config.IdRow).subscribe(res => {
				this.viewLoading = false;
				if (res.status == 1 && res.data) {
					this.Config = res.data;
					this.createForm();
				}
				else {
					this.layoutUtilsService.showError(res.error.message);
				}
				this.changeDetectorRefs.detectChanges();
			});
		} else {
			this.viewLoading = false;
			this.changeDetectorRefs.detectChanges();
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
	createForm() {
		let value: any = this.Config.Value == null ? '' : this.Config.Value;
		if (this.Config.Type == 'BOOLEAN')
			value = this.Config.Value == '1' ? true : false;
		let temp = {
			code: [this.Config.Code == null ? '' : this.Config.Code, [Validators.required, Validators.maxLength(20)]],
			value: [value, [Validators.required, Validators.maxLength(200)]],
			priority: [this.Config.Priority, Validators.min(1)],
			description: [this.Config.Description, Validators.min(1)],
		}
		if (this.Config.Pattern && this.Config.Type != 'LIST') {
			temp.value = [value, [Validators.required, Validators.maxLength(200), Validators.pattern(this.Config.Pattern)]];
		}
		this.ConfigForm = this.ConfigFB.group(temp);
		if (this.Config.Type == 'LIST') {
			this.chips = this.Config.Value.split(",");
			this.ConfigForm.get('value').statusChanges.subscribe(
				status => {
					if (this.chipList)
						this.chipList.errorState = status === 'INVALID';
				}
			);
		}
		if (!this.allowEdit)
			this.ConfigForm.disable();
	}
	/**
	 * Returns page title
	 */
	getTitle(): string {
		if (!this.allowEdit)
			return 'Xem chi tiết cấu hình';

		return `Chỉnh sửa cấu hình - ${this.Config.Code} `;
	}

	/**
	 * Check control is invalid
	 * @param controlName: string
	 */
	isControlInvalid(controlName: string): boolean {
		const control = this.ConfigForm.controls[controlName];
		const result = control.invalid && control.touched;
		return result;
	}

	/**
	 * Save data
	 *
	 * @param withBack: boolean
	 */
	onSubmit(type: boolean) {
		this.hasFormErrors = false;
		const controls = this.ConfigForm.controls;
		/** check form */
		if (this.ConfigForm.invalid) {
			Object.keys(controls).forEach(controlName =>
				controls[controlName].markAsTouched()
			);
			let invalid = <FormControl[]>Object.keys(this.ConfigForm.controls).map(key => this.ConfigForm.controls[key]).filter(ctl => ctl.invalid);
			let invalidElem: any = invalid[0];
			invalidElem.nativeElement.focus();
			this.hasFormErrors = true;
			return;
		}
		this.disabledBtn = true;
		// tslint:disable-next-line:prefer-const
		let editedConfig = this.prepareConfigs();
		this.updateConfig(editedConfig)
	}

	/**
	 * Returns object for saving
	 */
	prepareConfigs(): SysConfigModel {
		const controls = this.ConfigForm.controls;
		const _Config = Object.assign({}, this.Config);
		if (this.Config.Type == 'LIST')
			_Config.Value = this.chips.join(",");
		else {
			if (this.Config.Type == "BOOLEAN")
				_Config.Value = controls['value'].value ? '1' : '0';
			else
				_Config.Value = controls['value'].value + '';
		}

		_Config.Priority = controls['priority'].value;
		return _Config;
	}

	/**
	 * Update item
	 *
	 * @param _Config: SysConfigsModel
	 * @param withBack: boolean
	 */
	updateConfig(_Config: SysConfigModel, withBack: boolean = false) {
		this.configsService.updateConfig(_Config).subscribe(res => {
			if (res.status == 1) {
				this.isChange = true;
				const message = `Cập nhật cấu hình thành công`;
				this.layoutUtilsService.showInfo(message);
				this.dialogRef.close(this.isChange);
			}
			else {
				this.layoutUtilsService.showError(res.error.message);
			}
			this.disabledBtn = false;
			this.changeDetectorRefs.detectChanges();
		});
	}

	/**
	 * Close alert
	 *
	 * @param $event
	 */
	onAlertClose($event) {
		this.hasFormErrors = false;
	}

	closeDialog() {
		this.dialogRef.close(this.isChange);
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
	//#region chip
	add(event: MatChipInputEvent): void {
		const input = event.input;
		const value = event.value;

		// Add our fruit
		if ((value || '').trim()) {
			if (this.Config.Pattern) {
				var regex = new RegExp(this.Config.Pattern);
				var rex = regex.test(value.trim());
				if (rex)
					this.chips.push(value.trim());
				else {
					this.layoutUtilsService.showError("Giá trị không hợp lệ");
					return;
				}
			} else
				this.chips.push(value.trim());
		}

		// Reset the input value
		if (input) {
			input.value = '';
		}
		this.ConfigForm.controls['value'].setValue(this.chips.join(","));
	}
	remove(chip: string): void {
		const index = this.chips.indexOf(chip);

		if (index >= 0) {
			this.chips.splice(index, 1);
		}
		this.ConfigForm.controls['value'].setValue(this.chips.join(","));
	}
	//#endregion
}
