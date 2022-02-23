// Angular
import { Component, OnInit, ChangeDetectionStrategy, OnDestroy, ChangeDetectorRef, Inject, Input, Output, EventEmitter, ViewEncapsulation, OnChanges } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
// Material
import { MatDialog, MatTableDataSource, MAT_DIALOG_DATA, MatDialogRef, } from '@angular/material';
// RxJS
import { Observable, BehaviorSubject, Subscription, ReplaySubject } from 'rxjs';
// NGRX
// Service
import { LayoutUtilsService, MessageType } from 'app/core/_base/crud';
//Models

import moment from 'moment';
import { CommonService } from '../../services/common.service';

@Component({
	selector: 'kt-choose-users',
	templateUrl: './choose-users.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
	encapsulation: ViewEncapsulation.None
})

export class ChooseUsersComponent implements OnInit, OnChanges {
	// Public properties
	@Input() options: any = {
		showSearch: true,//hiển thị search input hoặc truyền keyword
		keyword: '',
		data: []
	};
	@Output() ItemSelected = new EventEmitter<any>();
	@Output() IsSearch = new EventEmitter<any>();

	listUser: any[] = [];
	public filteredUsers: ReplaySubject<any[]> = new ReplaySubject<any[]>(1);
	public userFilterCtrl: FormControl = new FormControl();
	constructor(
		private FormControlFB: FormBuilder,
		public dialog: MatDialog,
		private layoutUtilsService: LayoutUtilsService,
		public weworkService: CommonService,
		private changeDetectorRefs: ChangeDetectorRef) { }

	/**
	 * On init
	 */
	ngOnInit() {
		this.userFilterCtrl.valueChanges
			.pipe()
			.subscribe(() => {
				this.filterUsers();
			});
	}
	ngOnChanges() {
		this.userFilterCtrl.setValue('');
		this.listUser = [];

		if (this.options.showSearch == undefined)
			this.options.showSearch = true;
		if (this.options != undefined) {
			if (this.options.data) {
				this.listUser = this.options.data;
				this.filterUsers();
				this.changeDetectorRefs.detectChanges();
			} else {
				this.weworkService.getDSNguoiDungLite().subscribe(res => {
					if (res && res.status === 1) {
						this.listUser = res.data.map(x => {
							return {
								id_nv: x.UserID,
								hoten: x.FullName,
								username: x.UserName,
								mobile: '',
								tenchucdanh: '',
								image: x.image
							}
						});
						// mảng idnv exclude
						if (this.options.excludes && this.options.excludes.length > 0) {
							var arr = this.options.excludes;
							this.listUser = this.listUser.filter(x => !arr.includes(x.id_nv));
						}
						this.filterUsers();
						this.changeDetectorRefs.detectChanges();
					};
				});
			}
		}
		if (!this.options.showSearch)
			this.filterUsers();

	}
	protected filterUsers() {
		if (!this.listUser) {
			return;
		}

		let search = !this.options.showSearch ? this.options.keyword : this.userFilterCtrl.value;
		if (!search) {
			this.filteredUsers.next(this.listUser.slice());
			return;
		} else {
			search = search.toLowerCase();
		}
		// filter the banks
		if (search[0] == '@') {
			this.filteredUsers.next(
				this.listUser.filter(bank => ("@" + bank.username.toLowerCase()).indexOf(search) > -1)
			);
		}
		else {
			this.filteredUsers.next(
				this.listUser.filter(bank => bank.hoten.toLowerCase().indexOf(search) > -1)
			);
		}
	}
	select(user) {
		this.ItemSelected.emit(user)
	}
	stopPropagation(event) {
		this.IsSearch.emit(event)
	}
}
