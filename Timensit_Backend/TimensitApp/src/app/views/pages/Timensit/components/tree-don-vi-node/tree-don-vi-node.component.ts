import { Component, OnInit, EventEmitter, Input, Output, ViewChild, ElementRef, Renderer2, ChangeDetectorRef, ChangeDetectionStrategy } from '@angular/core';

import { MatCheckbox } from '@angular/material';
import { Subject } from 'rxjs';
@Component({
	selector: 'm-tree-don-vi-node',
	templateUrl: 'tree-don-vi-node.component.html',
	styleUrls: ['./tree-don-vi-node.component.scss'],
	changeDetection: ChangeDetectionStrategy.OnPush
})

export class TreeDonViNodeComponent implements OnInit {
	loadActive:boolean=true;//load  các node active trong lần load đầu tiên, nhưng khi chọn sẽ ẩn đi những active mặc định
	@Input() nodeCheckedChange= new Subject();
	@Input() nodeActiveChange= new Subject();
	@Input() nameNode?: string = "text";//default là text
	@Input() propNameChild?: string = "children";//tên node roof có chứa các con của node, default = children
	@Input() propNameCss?: string = "anCss";//tên thuộc tính sẽ chứa các định dạng css cho node, default = anCss
	@Input() valueDonVi: any = [];//a list roles whill be made tree authorize
	@Input() parentNode: any = [];//parent node, get it for checked node child will need this node
	@Input() showCheck?: boolean = true;//mặc định là true
	@Input() classIconLeaf?: string = "fas fa-folder";//mặc định là true
	@Input() classIconRoot?: string = "fas fa-folder-open";//mặc định là true
	@ViewChild('documentTreeDV', { static: true }) documentTreeDV: ElementRef;
	// @Output() NodeActiveChange: EventEmitter<any> = new EventEmitter<any>();//event for component
	// @Output() CheckedChanged: EventEmitter<any> = new EventEmitter<any>();//event for component
	//phải gọi public thì thằng cha mới có thể nhận dữ liệu
	constructor(
		private render: Renderer2) {

	}

	ngOnInit() {
		this.duyetTree(this.valueDonVi, {});
	}

	//duyệt node thêm một số thuộc tính để tiện cho việc xử lý dữ liệu
	duyetTree(node: any, parent: any) {
		//
		node.forEach((item, index) => {
			let anCss = {
				collapse: item[this.propNameChild] ? item[this.propNameChild].length > 0 ? true : false : false,
				lastChild: item[this.propNameChild] ? item[this.propNameChild].length > 0 ? false : true : true,
				state: 0,//trạng thái luôn luôn mở node này, 0 -> open, -1 -> close
				checked: false,
				parentChk: '',
				active:false
			};
			if (item[this.propNameCss] == undefined)
				item[this.propNameCss] = anCss;

			if (item[this.propNameChild]) {
				if (item[this.propNameChild].length > 0) {
					this.duyetTree(item[this.propNameChild], item);
				}
			}
		});
	}

	// => đang thao tác đóng - mở các nút
	collapseChanged(v: any, state: number) {
		v[this.propNameCss].state = state;
		v[this.propNameCss].collapse = state == 0;
	}

	// -> checked change các node
	checkedChanged(v: any, e: any) {
		v[this.propNameCss].checked = e.checked;
		//this.CheckedChanged.emit(v);
		this.nodeCheckedChange.next(v);
	}

	//-> checked for parent node
	checkedParentNode(currentNode: any, event: any) {
	}

	// phanQuyenChanged(v: any) {
	// 	//console.log("From child node");
	// }
	getItemNode(event, item: any) {		
		this.loadActive=false;
		if (this.showCheck) {
			return;
		}
		var blackDivs = document.querySelectorAll('.ea-quyen.dv-active');
		for (var i = 0; i < blackDivs.length; i++) {
			blackDivs[i].classList.remove('dv-active');
		}		
		this.render.addClass(event.target, 'dv-active');
		this.nodeActiveChange.next(item);
		return;
	}
	getActiveNode(item){
		if(item&&item.anCss&&item.anCss.active&&this.loadActive){
			this.loadActive=false;
			this.nodeActiveChange.next(item);
			return true;
		}
		else{
			return false;
		}
	}
}
