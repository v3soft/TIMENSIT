import { NestedTreeControl, FlatTreeControl } from '@angular/cdk/tree';
import { Component, ChangeDetectionStrategy, OnInit, Inject, ChangeDetectorRef, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog, MatTreeFlattener, MatTreeFlatDataSource } from '@angular/material';
import { LayoutUtilsService } from '../../../../../core/_base/crud';
import { ArrayDataSource, SelectionModel } from '@angular/cdk/collections';
import { CommonService } from '../../services/common.service';

export class TodoItemNode {
	data?: TodoItemNode[];
	title: string;
	selected?: boolean;
	id?: any;
}
@Component({
	selector: 'kt-chon-don-vi',
	templateUrl: './chon-don-vi.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ChonDonViComponent implements OnInit {
	// Public properties
	hasFormErrors: boolean = false;
	loadingSubject = new BehaviorSubject<boolean>(true);
	loading$: Observable<boolean>;
	viewLoading: boolean = false;
	isZoomSize: boolean = false;
	treeControl: NestedTreeControl<TodoItemNode>;
	dataSource: ArrayDataSource<TodoItemNode>;

	hasChild = (_: number, node: TodoItemNode) => !!node.data && node.data.length > 0;

	TREE_DATA: TodoItemNode[] = [
		//{
		//	item: 'Fruit',
		//	children: [
		//		{ item: 'Apple', selected: true },
		//		{ item: 'Banana' },
		//		{ item: 'Fruit loops' },
		//	]
		//}, {
		//	item: 'Vegetables',
		//	children: [
		//		{
		//			item: 'Green',
		//			children: [
		//				{ item: 'Broccoli', selected: true },
		//				{ item: 'Brussels sprouts' },
		//			]
		//		}, {
		//			item: 'Orange',
		//			children: [
		//				{ item: 'Pumpkins', selected: true },
		//				{ item: 'Carrots', selected: true },
		//			]
		//		},
		//	]
		//},
	];
	disabledBtn: boolean = false;
	constructor(
		public dialogRef: MatDialogRef<ChonDonViComponent>,
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
		this.treeControl = new NestedTreeControl<TodoItemNode>(node => node.data);
		this.dataSource = new ArrayDataSource(this.TREE_DATA);
		this.commonService.TreeDonVi().subscribe(res => {
			if (res && res.status == 1) {
				this.TREE_DATA = res.data;
				this.treeControl = new NestedTreeControl<TodoItemNode>(node => node.data);
				this.dataSource = new ArrayDataSource(this.TREE_DATA);
			} else {
				this.layoutUtilsService.showError(res.error.message);
			}
			this.viewLoading = false;
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

	onSubmit(node) {
		this.dialogRef.close(node);
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
}
