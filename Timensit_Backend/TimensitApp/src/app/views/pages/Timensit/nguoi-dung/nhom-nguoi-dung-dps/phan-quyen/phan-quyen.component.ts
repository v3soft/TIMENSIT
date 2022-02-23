import { NestedTreeControl, FlatTreeControl } from '@angular/cdk/tree';
import { Component, ChangeDetectionStrategy, OnInit, Inject, ChangeDetectorRef, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog, MatTreeFlattener, MatTreeFlatDataSource } from '@angular/material';
import { LayoutUtilsService } from '../../../../../../core/_base/crud';
import { NhomNguoiDungDPSService } from '../nhom-nguoi-dung-dps-service/nhom-nguoi-dung-dps.service';
import { NhomNguoiDungDPSModel } from '../nhom-nguoi-dung-dps-model/nhom-nguoi-dung-dps.model';
import { ArrayDataSource, SelectionModel } from '@angular/cdk/collections';
import { CommonService } from '../../../services/common.service';

export class TodoItemNode {
	children?: TodoItemNode[];
	item: string;
	selected?: boolean;
	value?: any;
}
@Component({
	selector: 'kt-phan-quyen',
	templateUrl: './phan-quyen.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PhanQuyenComponent implements OnInit {
	// Public properties
	hasFormErrors: boolean = false;
	loadingSubject = new BehaviorSubject<boolean>(true);
	loading$: Observable<boolean>;
	viewLoading: boolean = false;
	isZoomSize: boolean = false;
	NhomNguoiDungDPS: NhomNguoiDungDPSModel;
	treeControl: NestedTreeControl<TodoItemNode>;
	dataSource: ArrayDataSource<TodoItemNode>;

	hasChild = (_: number, node: TodoItemNode) => !!node.children && node.children.length > 0;

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
	/** The selection for checklist */
	checklistSelection = new SelectionModel<TodoItemNode>(true /* multiple */);
	selected: number[] = [];
	disabledBtn: boolean = false;
	constructor(
		public dialogRef: MatDialogRef<PhanQuyenComponent>,
		@Inject(MAT_DIALOG_DATA) public data: any,
		public dialog: MatDialog,
		private layoutUtilsService: LayoutUtilsService,
		private changeDetectorRefs: ChangeDetectorRef,
		private nhomnguoidungdpssService: NhomNguoiDungDPSService,
		private commonService: CommonService) { }

	/**
	 * On init
	 */
	async ngOnInit() {
		this.viewLoading = true;
		this.treeControl = new NestedTreeControl<TodoItemNode>(node => node.children);
		this.dataSource = new ArrayDataSource(this.TREE_DATA);
		this.NhomNguoiDungDPS = this.data.NhomNguoiDungDPS;
		this.commonService.getTreeQuyen(this.NhomNguoiDungDPS.IdGroup).subscribe(res => {
			if (res && res.status == 1) {
				this.TREE_DATA = res.data;
				this.treeControl = new NestedTreeControl<TodoItemNode>(node => node.children);
				this.dataSource = new ArrayDataSource(this.TREE_DATA);
				this.bindSelection(null, this.TREE_DATA);
			} else {
				this.layoutUtilsService.showError(res.error.message);
			}
			this.viewLoading = false;
			this.changeDetectorRefs.detectChanges();
		});

	}
	bindSelection(parent: TodoItemNode, nodes: TodoItemNode[]) {
		for (var i = 0; i < nodes.length; i++) {
			if (nodes[i].selected) {
				this.todoLeafItemSelectionToggle(nodes[i]);
				while (parent) {
					this.treeControl.expand(parent);
					parent = this.getParentNode(parent);
				}
			}
			if (nodes[i].children) {
				this.bindSelection(nodes[i], nodes[i].children);
			}
		}
	}
	/**
	 * Returns page title
	 */
	getTitle(): string {
		return `Phân quyền`;
	}
	/**
	 * Close alert
	 *
	 * @param $event
	 */
	onAlertClose($event) {
		this.hasFormErrors = false;
	}

	onSubmit() {
		let data: any = {
			IdGroup: this.NhomNguoiDungDPS.IdGroup,
			Quyens: this.selected
		};
		this.disabledBtn = true;
		this.nhomnguoidungdpssService.updateQuyen(data).subscribe(res => {
			this.disabledBtn = false;
			if (res && res.status == 1) {
				this.layoutUtilsService.showInfo("Cập nhật quyền cho vai trò thành công");
				this.closeDialog();
			} else {
				this.layoutUtilsService.showError(res.error.message);
			}
		});
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
	SelectItemTree(node) {
		// console.log("node", node.ParentID);
	}

	/** Whether all the descendants of the node are selected. */
	descendantsAllSelected(node: TodoItemNode): boolean {
		const descendants = this.treeControl.getDescendants(node);
		const descAllSelected = descendants.every(child =>
			this.checklistSelection.isSelected(child)
		);
		return descAllSelected;
	}

	/** Whether part of the descendants are selected */
	descendantsPartiallySelected(node: TodoItemNode): boolean {
		const descendants = this.treeControl.getDescendants(node);
		const result = descendants.some(child => this.checklistSelection.isSelected(child));
		return result && !this.descendantsAllSelected(node);
	}

	/** Toggle the to-do item selection. Select/deselect all the descendants node */
	todoItemSelectionToggle(node: TodoItemNode): void {
		this.checklistSelection.toggle(node);
		const descendants = this.treeControl.getDescendants(node);
		this.checklistSelection.isSelected(node)
			? this.checklistSelection.select(...descendants)
			: this.checklistSelection.deselect(...descendants);
		// Force update for the parent
		descendants.every(child => this.checklistSelection.isSelected(child));
		this.checkAllParentsSelection(node);
		descendants.forEach(child => {
			this.selectedChange(child);
		});
	}

	/** Toggle a leaf to-do item selection. Check all the parents to see if they changed */
	todoLeafItemSelectionToggle(node: TodoItemNode): void {
		this.checklistSelection.toggle(node);
		this.checkAllParentsSelection(node);
		this.selectedChange(node);
	}

	/* Checks all the parents when a leaf node is selected/unselected */
	checkAllParentsSelection(node: TodoItemNode): void {
		let parent: TodoItemNode | null = this.getParentNode(node);
		while (parent) {
			this.checkRootNodeSelection(parent);
			parent = this.getParentNode(parent);
		}
	}

	/** Check root node checked state and change it accordingly */
	checkRootNodeSelection(node: TodoItemNode): void {
		const nodeSelected = this.checklistSelection.isSelected(node);
		const descendants = this.treeControl.getDescendants(node);
		const descAllSelected = descendants.every(child =>
			this.checklistSelection.isSelected(child)
		);
		if (nodeSelected && !descAllSelected) {
			this.checklistSelection.deselect(node);
		} else if (!nodeSelected && descAllSelected) {
			this.checklistSelection.select(node);
		}
	}
	/* Get the parent node of a node */
	getParentNode(node: TodoItemNode): TodoItemNode | null {
		let arr = this.TREE_DATA;
		return this.getParent(node, arr);
	}

	getParent(node: TodoItemNode, nodes: TodoItemNode[]) {
		if (nodes) {
			for (var i = 0; i < nodes.length; i++) {
				if (nodes[i].children) {
					var index = nodes[i].children.indexOf(node);
					if (index >= 0) {
						return nodes[i];
					}
					var result = this.getParent(node, nodes[i].children);
					if (result)
						return result;
				}
			}
		}
		return null;
	}
	selectedChange(node: TodoItemNode) {
		if (node.value) {
			var i = this.selected.indexOf(node.value);
			if (this.checklistSelection.isSelected(node)) {
				if (i < 0)
					this.selected.push(node.value);
			}
			else {
				if (i >= 0)
					this.selected.splice(i, 1);
			}
		}
	}
}
