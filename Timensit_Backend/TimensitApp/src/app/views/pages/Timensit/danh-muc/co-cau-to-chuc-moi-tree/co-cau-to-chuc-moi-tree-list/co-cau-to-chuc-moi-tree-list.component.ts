import { Component, OnInit, ViewEncapsulation, Input, Output, EventEmitter, ChangeDetectorRef } from '@angular/core';
import { NestedTreeControl } from '@angular/cdk/tree';
import { ArrayDataSource } from '@angular/cdk/collections';
import { Observable, BehaviorSubject, of } from 'rxjs';
import { cocautochucMoiTreeService } from '../Services/co-cau-to-chuc-moi-tree.service';
import { OrgStructureModel } from '../Model/CoCauToChuc.model';
import { CoCauToChucEditComponent } from '../co-cau-to-chuc-moi-tree-edit/co-cau-to-chuc-moi-tree-edit.component';
import { MatDialog } from '@angular/material';
import { TranslateService } from '@ngx-translate/core';
import { LayoutUtilsService } from '../../../../../../core/_base/crud';
import { CommonService } from '../../../services/common.service';
import { CoCauMapDialogComponent } from '../co-cau-map/co-cau-map-dialog.component';

interface TreeNode {
	Title: string;
	Children?: TreeNode[];
	RowID: string;
	Level: string;
	Select: boolean;
	ID_Goc: number;
	Parentid: string;
}
@Component({
	selector: 'm-co-cau-to-chuc-moi-tree-list',
	templateUrl: './co-cau-to-chuc-moi-tree-list.component.html',
	styleUrls: ['./co-cau-to-chuc-moi-tree-list.component.scss'],
	encapsulation: ViewEncapsulation.None
})
export class cocautochucmoitreeComponent implements OnInit {
	treeControl = new NestedTreeControl<TreeNode>(node => node.Children);
	dataSource;
	Title: string = "";
	selectedNode: any;
	Visible: boolean = false;
	public datatree: BehaviorSubject<any[]> = new BehaviorSubject([]);
	positionTo: string = '';
	parentTo: string = '';
	positionFrom: string = '';
	parentFrom: string = '';
	idFrom: string = '';
	idTo: string = '';
	nameTo: string = '';
	namefrom: string = '';
	showTruyCapNhanh: boolean = true;
	id_menu: number = 3;
	_itemchart: OrgStructureModel;
	// nestedNodeMap = new Map<TodoItemNode, TodoItemFlatNode>();
	dragNodeExpandOverWaitTimeMs = 300;
	dragNodeExpandOverNode: any;
	dragNodeExpandOverTime: number;
	dragNodeExpandOverArea: string;
	dragNode: any;
	viewLoading: boolean = false;
	loadingSubject = new BehaviorSubject<boolean>(false);
	loading$= this.loadingSubject.asObservable();
	_name = "";
	constructor(
		private _cocaumoiService: cocautochucMoiTreeService,
		private changeDetectorRefs: ChangeDetectorRef,
		public dialog: MatDialog,
		private danhMucService: CommonService,
		private translate: TranslateService,
		private layoutUtilsService: LayoutUtilsService
	) {
		this._name = this.translate.instant("CO_CAU_TO_CHUC.NAME");
	}

	async ngOnInit() {
		await this.getTreeValue();
	};

	hasChild = (_: number, node: TreeNode) => !!node.Children && node.Children.length > 0;


	getParent(data: TreeNode[], node: TreeNode) {
		if (data == undefined)
			data = [];
		for (var i = 0; i < data.length; i++) {
			const currentNode = data[i];
			if (currentNode.RowID == node.Parentid)
				return currentNode;
			let re = this.getParent(currentNode.Children, node)
			if (re == undefined)
				continue;
			return re;
		}
		return undefined;
	}
	//DEMO Treeview
	async getTreeValue() {
		this.loadingSubject.next(true);
		this._cocaumoiService.Get_CoCauToChuc().subscribe(res => {
			this.loadingSubject.next(false);
			if (res.Visible != undefined)
				this.Visible = res.Visible;
			if (res.data && res.data.length > 0) {
				this.dataSource = new ArrayDataSource(res.data);
				this.treeControl.dataNodes = res.data;
				this.treeControl.expandAll();
				this.changeDetectorRefs.detectChanges();
			}
		});
	}
	// Kéo
	handleDragStart(event, node) {
		this.idFrom = node.RowID;
		this.namefrom = node.Title;
		this.removeClass("dl-drag");
		let el = document.getElementById(node.RowID);
		el.classList.add("dl-drag");
		this.dragNode = node;
		this.treeControl.collapse(node);
	}
	removeClass(className) {
		let c = document.getElementsByClassName(className);
		for (let i = 0; i < c.length; i++) {
			c[i].classList.remove(className);
		}
	}
	handleDragOver(event, node) {
		event.preventDefault();
		this.removeClass("dl-drag-over");
		let el = document.getElementById(node.RowID);
		el.classList.add("dl-drag-over");
		if (node === this.dragNodeExpandOverNode) {
			if (this.dragNode !== node && !this.treeControl.isExpanded(node)) {
				if ((new Date().getTime() - this.dragNodeExpandOverTime) > this.dragNodeExpandOverWaitTimeMs) {
					this.treeControl.expand(node);
				}
			}
		} else {
			this.dragNodeExpandOverNode = node;
			this.dragNodeExpandOverTime = new Date().getTime();
		}

		// Handle drag area

		// const percentageX = event.offsetX / event.target.offsetWidth;
		// const percentageY = event.offsetY / event.target.offsetHeight;
		const percentageX = event.offsetX / event.target.clientWidth;
		const percentageY = event.offsetY / event.target.offsetParent.clientHeight;
		// console.log("percentageY",percentageY);

		if (percentageY < 0.25) {
			this.dragNodeExpandOverArea = 'above';
		} else if (percentageY > 0.75) {
			this.dragNodeExpandOverArea = 'below';
		}
		else {
			this.dragNodeExpandOverArea = 'center';
		}
	}
	getPosition(event) {
		let offsetLeft = 0;
		let offsetTop = 0;
		let el = event.srcElement;
		while (el) {
			offsetLeft += el.offsetLeft;
			offsetTop += el.offsetTop;
			el = el.offsetParent;
		}
		return { offsetTop: offsetTop, offsetLeft: offsetLeft }
	}
	//#region ==================== Thả
	handleDrop(event, node) {
		event.preventDefault();
		this._itemchart = new OrgStructureModel();
		this._cocaumoiService.Get_CoCauToChuc().subscribe(res => {
			;
			this.dataSource.data = res.data;

			if (!res.data.Visible) {
				this.layoutUtilsService.showError('Bạn không có quyền thao tác');
				this.ngOnInit();
				return null;
			}
		});
		this._itemchart.drop_namefrom = this.namefrom;
		this._itemchart.drop_idfrom = this.idFrom;
		this._itemchart.drop_idto = node.RowID;
		if (this._itemchart.drop_idfrom != this._itemchart.drop_idto) {//chính nó sẽ k chuyển
			this._itemchart.drop_nameto = node.Title;
			this.ngOnInit();
		}

		if (this.dragNodeExpandOverArea === 'above') // update vị trí phía trên item chọn
		{
			this._itemchart.IsAbove = true;
			this._cocaumoiService.handleDropLevel(this._itemchart).subscribe(res => {
				this.treeControl.expandAll();
			});
		} else
			if (this.dragNodeExpandOverArea === 'below') // Update vị trí phía dưới item chọn
			{
				this._itemchart.IsAbove = false;
				this._cocaumoiService.handleDropLevel(this._itemchart).subscribe(res => {
					this.treeControl.expandAll();
				});
			}
			else // update cha con -- center
			{
				let vitritieptheo = 0;
				if (node.Children.length > 0) {
					vitritieptheo = parseInt(node.Children[node.Children.length - 1].Level) + 1;
				}
				else
					vitritieptheo = 1;
				this._itemchart.level = '' + vitritieptheo;
				this._cocaumoiService.handleDropParent(this._itemchart).subscribe(res => {
					this.treeControl.expandAll();
				});
				// newItem = this.database.copyPasteItem(this.flatNodeMap.get(this.dragNode), this.flatNodeMap.get(node));
			}
		this.dragNode = null;
		this.dragNodeExpandOverNode = null;
		this.dragNodeExpandOverTime = 0;
	}
	//#endregion ===================== End di chuyển node
	handleDragEnd(event) {
		this.removeClass("dl-drag");
		this.removeClass("dl-drag-over");
		this.dragNode = null;
		this.dragNodeExpandOverNode = null;
		this.dragNodeExpandOverTime = 0;
	}
	SelectItemTree(node) {
		// console.log("node", node.ParentID);
	}
	Add(item: TreeNode) {
		let _item: any = {};
		_item.IDParent = item.RowID;
		_item.ParentID = item.RowID;
		_item.RowID = '0';
		_item.Title = '';
		_item.Position = '';
		_item.Level = '';
		_item.Code = '';
		_item.WorkingModeID = '';
		this.CapNhatCapCoCau(_item, item);
	}
	CapNhatCapCoCau(_item: TreeNode, parent: TreeNode = null) {
		if (parent == null)
			parent = this.getParent(this.treeControl.dataNodes, _item);
		let ID_Goc = null;
		if (parent)
			ID_Goc = parent.ID_Goc;
		let saveMessageTranslateParam = '';
		saveMessageTranslateParam += this.translate.instant('OBJECT.EDIT.UPDATE_MESSAGE', { name: this._name });
		const _saveMessage = this.translate.instant(saveMessageTranslateParam, { name: this._name });

		const dialogRef = this.dialog.open(CoCauToChucEditComponent, { data: { _item, ID_Goc } });
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				this.getTreeValue();
				this.changeDetectorRefs.detectChanges();
			}
			else {
				this.layoutUtilsService.showInfo(_saveMessage);
				this.getTreeValue();
				this.changeDetectorRefs.detectChanges();
			}
		});
	}
	removeNode(_item: OrgStructureModel) {
		const _title = this.translate.instant('OBJECT.DELETE.TITLE', { name: this._name.toLowerCase() });
		const _description = this.translate.instant('OBJECT.DELETE.DESCRIPTION', { name: this._name.toLowerCase() });
		const _waitDesciption = this.translate.instant('OBJECT.DELETE.WAIT_DESCRIPTION', { name: this._name.toLowerCase() });
		const _deleteMessage = this.translate.instant('OBJECT.DELETE.MESSAGE', { name: this._name });
		const dialogRef = this.layoutUtilsService.deleteElement(_title, _description, _waitDesciption);
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}
			this._cocaumoiService.Deleteorgstructure(_item.RowID, _item.Title).subscribe(res => {
				if (res && res.status === 1) {
					this.layoutUtilsService.showInfo(_deleteMessage);
				}
				else {
					this.layoutUtilsService.showError(res.error.message);
				}
				this.getTreeValue();
				this.changeDetectorRefs.detectChanges();
			});
		});
	}
	map() {
		const dialogRef = this.dialog.open(CoCauMapDialogComponent, { data: { allowEdit: this.Visible } });
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
			}
			else {
				this.layoutUtilsService.showInfo("Map thành công");
				this.getTreeValue();
				this.changeDetectorRefs.detectChanges();
			}
		});

	}
}
