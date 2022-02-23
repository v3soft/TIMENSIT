import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { BehaviorSubject, ReplaySubject } from 'rxjs';
import { LayoutUtilsService } from '../../../../core/_base/crud';
import * as moment  from 'moment';
import { DatePipe } from '@angular/common';

export const data: any = {
	DonVi: [
		{
			id: 1,
			title: 'Đơn vị 1',
			data: [],
			disabled: false
		},
		{
			id: 2,
			title: 'Đơn vị 2',
			data: [{
				id: 22,
				title: 'Đơn vị 22',
				data: [],
				disabled: false
			}, {
				id: 21,
				title: 'Đơn vị 21',
				data: [],
				disabled: false
			},
			],
			disabled: false
		},
		{
			id: 3,
			title: 'Đơn vị 3',
			data: [],
			disabled: false
		}
	],
	Options: [
		{
			id: 1,
			title: 'Option 1',
		}, {
			id: 2,
			title: 'Option 2',
		}, {
			id: 3,
			title: 'Option 3',
		},
		{
			id: 4,
			title: 'Option 4'
		}, {
			id: 5,
			title: 'Option 5',
		}, {
			id: 6,
			title: 'Option 6',
		}]
};
@Component({
	selector: 'kt-my-page',
	templateUrl: './my-page.component.html',
	styleUrls: ['./my-page.component.scss'],
	providers: [DatePipe]
})
export class MyPageComponent implements OnInit {
	ItemForm: FormGroup;
	hasFormErrors: boolean = false;
	allowEdit: boolean = true;
	item: any = {};
	imagedata: any = {
		Title: 'File control - multi image'
	};
	public datatree: BehaviorSubject<any[]> = new BehaviorSubject([]);

	filtered: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
	FilterCtrl: string = '';
	listOpt: any[] = [];
	filteredListOpt: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
	constructor(
		private fb: FormBuilder,
		private layoutUtilService: LayoutUtilsService,
		private detectChange: ChangeDetectorRef) {
		this.datatree.next(data.DonVi);
		this.listOpt = data.Options;
		this.filteredListOpt.next(this.listOpt);
	}

	ngOnInit() {
		this.createForm();
	}

	/**
	 * Create form
	 */
	createForm() {
		var date = moment();
		this.ItemForm = this.fb.group({
			dropdowntreeControl: ['', Validators.required],
			imageControl: ['', Validators.required],
			datetimeControl: [date, Validators.required],
			selectfilterControl: ['', Validators.required],
		});
	}
	/**
	 * Save data
	 *
	 * @param withBack: boolean
	 */
	onSubmit(type: boolean) {
		this.hasFormErrors = false;
		const controls = this.ItemForm.controls;
		/** check form */
		if (this.ItemForm.invalid) {
			Object.keys(controls).forEach(controlName =>
				controls[controlName].markAsTouched()
			);
			let invalid = <FormControl[]>Object.keys(this.ItemForm.controls).map(key => this.ItemForm.controls[key]).filter(ctl => ctl.invalid);
			let invalidElem: any = invalid[0];
			invalidElem.nativeElement.focus();
			this.hasFormErrors = true;
			return;
		}
		this.layoutUtilService.showInfo("Open console to view result");
		let data = {
			dropdowntreeControl: controls["dropdowntreeControl"].value,
			imageControl: controls["imageControl"].value,
			datetimeControl: controls["datetimeControl"].value,
			selectfilterControl: controls["selectfilterControl"].value,
		}
		console.log("submitted result:", data);
	}

	/**
	 * Close alert
	 *
	 * @param $event
	 */
	onAlertClose($event) {
		this.hasFormErrors = false;
	}
	GetValueNode($event) {

	}

	filter() {

		if (!this.listOpt) {
			return;
		}
		let search = this.FilterCtrl;
		if (!search) {
			this.filteredListOpt.next(this.listOpt.slice());
			return;
		} else {
			search = search.toLowerCase();
		}
		this.filteredListOpt.next(
			this.listOpt.filter(ts =>
				ts.title.toLowerCase().indexOf(search) > -1)
		);
		this.detectChange.detectChanges();
	}
}
