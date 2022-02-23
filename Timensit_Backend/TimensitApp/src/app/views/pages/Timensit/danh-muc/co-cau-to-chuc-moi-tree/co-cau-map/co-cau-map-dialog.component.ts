import { Component, OnInit, Inject, ChangeDetectionStrategy, HostListener, ViewChild, ElementRef, ChangeDetectorRef } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AngularEditorConfig } from '@kolkov/angular-editor';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from '../../../services/common.service';
import { LayoutUtilsService, TypesUtilsService } from '../../../../../../core/_base/crud';
import { cocautochucMoiTreeService } from '../Services/co-cau-to-chuc-moi-tree.service';
import { NestedTreeControl } from '@angular/cdk/tree';
import { ArrayDataSource, SelectionModel } from '@angular/cdk/collections';
import { forEach } from 'lodash';
export class TodoItemNode {
	children?: TodoItemNode[];
	item: string;
	selected?: boolean;
	value?: any;
	data?: any;
}
@Component({
	selector: 'kt-co-cau-map',
	templateUrl: './co-cau-map-dialog.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
})

export class CoCauMapDialogComponent implements OnInit {
	hasFormErrors = false;
	viewLoading = false;
	loadingAfterSubmit = false;
	disabledBtn = false;
	allowEdit = false;
	isZoomSize: boolean = false;
	_NAME = '';
	lstNhomLoaiDoiTuongNCC: any[] = [];
	lstConstLoaiHoSo: any[] = [];
	treeControl: NestedTreeControl<TodoItemNode>;
	dataSource: ArrayDataSource<TodoItemNode>;

	hasChild = (_: number, node: TodoItemNode) => !!node.children && node.children.length > 0;

	TREE_DATA: TodoItemNode[] = [
		//{
		//	item: 'Fruit',
		//	value: 1,
		//	children: [
		//		{ item: 'Apple', value: 11, selected: true },
		//		{ item: 'Banana', value: 12 },
		//		{ item: 'Fruit loops', value: 13 },
		//	]
		//}, {
		//	item: 'Vegetables',
		//	value: 2,
		//	children: [
		//		{
		//			item: 'Green',
		//			value: 21,
		//			children: [
		//				{ item: 'Broccoli', value: 211, selected: true },
		//				{ item: 'Brussels sprouts', value: 212 },
		//			]
		//		}, {
		//			item: 'Orange',
		//			value: 22,
		//			children: [
		//				{ item: 'Pumpkins', value: 211, selected: true },
		//				{ item: 'Carrots', value: 212, selected: true },
		//			]
		//		},
		//	]
		//},
	];
	/** The selection for checklist */
	checklistSelection = new SelectionModel<TodoItemNode>(true /* multiple */);
	constructor(
		public dialogRef: MatDialogRef<CoCauMapDialogComponent>,
		@Inject(MAT_DIALOG_DATA) public data: any,
		private fb: FormBuilder,
		private danhMucService: CommonService,
		private layoutUtilsService: LayoutUtilsService,
		private changeDetectorRefs: ChangeDetectorRef,
		private typesUtilsService: TypesUtilsService,
		private service: cocautochucMoiTreeService,
		private translate: TranslateService) {
	}

	/** LOAD DATA */
	ngOnInit() {
		if (this.data.allowEdit)
			this.allowEdit = this.data.allowEdit;
		this.treeControl = new NestedTreeControl<TodoItemNode>(node => node.children);
		this.dataSource = new ArrayDataSource(this.TREE_DATA);
		this.viewLoading = true;
		this.service.getCoCauMap().subscribe(res => {
			if (res && res.status == 1) {
				this.TREE_DATA = res.data;
				this.treeControl = new NestedTreeControl<TodoItemNode>(node => node.children);
				this.dataSource = new ArrayDataSource(this.TREE_DATA);
				if (this.data && this.data.IsExpand) {//expand node
					this.treeControl.dataNodes = res.data;
					this.treeControl.expandAll();
				}
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
	}
	checkedChange($event, node: TodoItemNode) {
	}
	/** UI */
	getTitle(): string {
		return "Đơn vị hành chánh tương ứng";
	}

	/** ACTIONS */
	prepareCustomer(): any {
		let indetermine: any[] = [];
		this.TREE_DATA.forEach(t => {
			if (this.descendantsPartiallySelected(t)) {
				indetermine.push(t.data);
			}
			t.children.forEach(h => {
				if (this.descendantsPartiallySelected(h)) {
					indetermine.push(h.data);
				}
			})
		})
		let re = this.checklistSelection.selected.map(x => x.data);
		re = re.concat(indetermine);
		return re;
	}
	onSubmit(withBack: boolean = false) {
		const data = this.prepareCustomer();
		this.service.map(data).subscribe(res => {
			if (res && res.status == 1) {
				this.dialogRef.close(true);
			} else {
				this.layoutUtilsService.showError(res.error.message);
			}
		})
	}
	onAlertClose($event) {
		this.hasFormErrors = false;
	}
	close() {
		this.dialogRef.close();
	}
}
