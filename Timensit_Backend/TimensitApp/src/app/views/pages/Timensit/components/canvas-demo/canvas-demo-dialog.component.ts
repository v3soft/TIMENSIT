import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Inject, Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormGroup, Validators, FormBuilder, FormControl } from '@angular/forms';
import { LayoutUtilsService } from '../../../../../core/_base/crud';
import { CommonService } from '../../services/common.service';
import { DynamicProcessService } from '../../services/dynamic-process.service';

@Component({
	selector: 'canvas-demo-dialog',
	templateUrl: 'canvas-demo-dialog.component.html',
})
export class CanvasDemoDialogComponent implements OnInit {
	itemForm: FormGroup;
	selectedNodeEnd: any;
	buttonText: string = "";
	dataNode: any[] = [];
	viewLoading: boolean = false;
	listQuyen: any[] = [];
	listType: any[] = [];
	hasFormErrors: boolean = false;
	disabledBtn: boolean = false;
	constructor(
		public dialogRef: MatDialogRef<CanvasDemoDialogComponent>,
		private service: DynamicProcessService,
		private commonService: CommonService,
		private layoutUtilsService: LayoutUtilsService,
		private fb: FormBuilder,
		private changeDetectorRef: ChangeDetectorRef,
		@Inject(MAT_DIALOG_DATA) public data: any
	) { }
	ngOnInit() {
		this.listQuyen = [];

		this.itemForm = this.fb.group({
			buttonText: ["", [Validators.required, Validators.maxLength(50)]],
			NodeEnd: ["", Validators.required],
		});

		if (this.data.item && this.data.item.end_node) {
			this.selectedNodeEnd = this.data.item.end_node.ID;
			this.buttonText = this.data.item.arrow.ButtonText;

		}
		this.dataNode = this.data.dataNode.map(x => Object.assign({}, x));
		for (var i = this.dataNode.length - 1; i >= 0; i--) {
			if (this.dataNode[i].data.IdRow == this.data.selectedNode.data.IdRow) {
				this.dataNode.splice(i, 1);
				continue;
			}
			for (var j = 0; j < this.data.selectedNode.arrows.length; j++) {
				if (this.dataNode[i].data.IdRow == this.data.selectedNode.arrows[j].end_node.ID) {
					if (this.selectedNodeEnd != undefined && this.selectedNodeEnd == this.data.selectedNode.arrows[j].end_node.ID)
						continue;
					this.dataNode.splice(i, 1);
					continue;
				}
			}
		}
		this.changeDetectorRef.detectChanges();
	}
	onNoClick(): void {
		this.dialogRef.close();
	}
	onSubmit() {
		this.hasFormErrors = false;
		const controls = this.itemForm.controls;
		/** check form */
		if (this.itemForm.invalid) {
			this.hasFormErrors = true;
			Object.keys(controls).forEach(controlName =>
				controls[controlName].markAsTouched()
			);
			let invalid = <FormControl[]>Object.keys(this.itemForm.controls).map(key => this.itemForm.controls[key]).filter(ctl => ctl.invalid);
			let invalidElem: any = invalid[0];
			invalidElem.nativeElement.focus();
			return;
		}
		var node = {};
		for (var i = this.dataNode.length - 1; i >= 0; i--) {
			if (this.dataNode[i].data.IdRow == this.selectedNodeEnd) {
				node = this.dataNode[i].data;
				break;
			}
		}
		let data = {
			Step: this.data.selectedNode.data,
			Next: node,
			ButtonText: this.buttonText,
		}
		this.disabledBtn = true;
		this.service.SaveNextStep(data).subscribe(res => {
			if (res.status == 1) {
				const message = `Cập nhật bước thành công`;
				this.layoutUtilsService.showInfo(message);
				this.dialogRef.close(res);
			}
			else {
				this.layoutUtilsService.showError(res.error.message);
			}
			this.disabledBtn = false;
			this.changeDetectorRef.detectChanges();
		});
	}
	onAlertClose() {
		this.hasFormErrors = false;
	}
}
