import { Component, OnInit, ChangeDetectorRef, ChangeDetectionStrategy, Inject, Input, Output, EventEmitter, OnChanges } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Observable, forkJoin, from, of, BehaviorSubject, Subject } from 'rxjs';
import { map, debounceTime, startWith, distinctUntilChanged, switchMap, filter } from 'rxjs/operators';
@Component({
	selector: 'm-tree-don-vi',
	templateUrl: './tree-don-vi.component.html',
	// changeDetection: ChangeDetectionStrategy.OnPush
})
export class TreeDonViComponent implements OnInit {
	loadingSubject = new BehaviorSubject<boolean>(false);
	loading$ = this.loadingSubject.asObservable();
	oldItem: any;
	lst_DonVi: any;
	lstNodeSelected: any[] = [];
	g_ParentOfLastNode: any = {};
	g_ParentOfParent: any = {};
	g_NodeSlected: any = {};
	@Output() nemoTreeChanged: EventEmitter<any> = new EventEmitter<any>();//event for component
	@Output() NodeActiveChange: EventEmitter<any> = new EventEmitter<any>();//event for component
	@Input() nameNode?: string = "text";//default là text
	@Input() propNameChild?: string = "children";//tên node roof có chứa các con của node, default = children
	@Input() propNameCss?: string = "anCss";//tên thuộc tính sẽ chứa các định dạng css cho node, default = anCss
	@Input() valuePhanQuyen: any[] = [];//a list roles whill be made tree authorize
	@Input() textFeildRoof?: string = "Tất cả";//mặc định là chữ tất cả
	@Input() idRoleKey?: string = "IdRole";//mặc định là IdRole
	@Input() permitted?: string = "Permitted";//mặc định là IdRole
	@Input() showCheck?: boolean = true;//mặc định là true
	@Input() classIconLeaf?: string = "fas fa-folder";//mặc định là true
	@Input() classIconRoot?: string = "fas fa-folder-open";//mặc định là true
	nodeCheckedChange= new Subject();
	nodeActiveChange= new Subject();
	//need it to set some properties for node gốc
	masterNode: any = {
		anCss: {
			collapse: true,
			lastChild: false,
			state: 0,//trạng thái luôn luôn mở node này, 0 -> open, -1 -> close
			checked: false,
			parentChk: '',
			active:false
		}
	}

	constructor(private activatedRoute: ActivatedRoute,
		private router: Router,
		// private changedDetector: ChangeDetectorRef,
	) { }

	ngOnInit() {
		this.oldItem = {};
		this.lst_DonVi = this.valuePhanQuyen;
		
		//load những node đã được selected, check các node được chọn từ db lên theo quyền
		if (this.lst_DonVi) {
			this.checkNodeFromQuyen_DB(this.lst_DonVi[0]);
			this.collapseNode(this.lst_DonVi[0]);
		}
		// console.log("test", this.eventBusService.test);
		
		this.nodeCheckedChange.subscribe(item => {
			this.phanQuyenBus(item);
			let r = this.onLuuPhanQuyenCay();
			this.nemoTreeChanged.emit({ treeview: true, captain:'Nemo', value: r });
		});
		this.nodeActiveChange.subscribe(res=>{
			this.NodeActiveChange.emit(res)
		})
	}
	onSumbit(withBack: boolean = false) {
		// chuẩn bị đối tượng, dể lát lưu người dùng
		let editedProduct = this.prepareProduct();
	}

	prepareProduct(): any {
	}

	//======================== Lưu quyền ==================================
	onLuuPhanQuyenCay() {

		let data: any = {};
		data.IDKHDPS = 0;
		data.ChiTiet = [];
		//lấy list những quyền được chọn
		data.ChiTiet = this.layQuyenDuocChon(this.lst_DonVi[0], data.ChiTiet);
		return data.ChiTiet;
	}

	layQuyenDuocChon(lstSrc: any, lst: any[]) {
		let quyen: any = {};
		if( lstSrc.text=="Tất cả" && lstSrc.anCss.checked && lstSrc.anCss.parentChk ==""){
			//console.log("this.lst root ", lstSrc);
			return [];
		}
		if (lstSrc[this.propNameCss].checked && lstSrc.anCss.parentChk =="") {
			if (lstSrc.data && lstSrc.data[this.idRoleKey] != undefined) {
				quyen[this.idRoleKey] = +lstSrc.data[this.idRoleKey];
				quyen[this.permitted] = true;
				lst.push(quyen);
			}
		}

		if (lstSrc[this.propNameChild]) {
			for (var i = 0; i < lstSrc[this.propNameChild].length; i++)
				this.layQuyenDuocChon(lstSrc[this.propNameChild][i], lst);
		}		
		return lst;
	}

	//load quyền từ db, duyệt tree
	checkNodeFromQuyen_DB(itemSrc: any) {

		let anCss = {
			collapse: false,
			lastChild: false,
			state: 0,//trạng thái luôn luôn mở node này, 0 -> open, -1 -> close
			checked: false,
			parentChk: '',
			active:false
		};

		//gán thuốc tính anCss cho node
		if (itemSrc[this.propNameCss]) { }
		else itemSrc[this.propNameCss] = Object.assign({}, anCss);

		//duyệt các node
		if (itemSrc[this.propNameChild]) {
			for (var i = 0; i < itemSrc[this.propNameChild].length; i++)
				this.checkNodeFromQuyen_DB(itemSrc[this.propNameChild][i]);
		}
		else {
			itemSrc[this.propNameCss].lastChild = true;
			itemSrc[this.propNameCss].checked = itemSrc.state.selected;
			this.phanQuyenBus(itemSrc);
		}
	}


	//====================================== all of these things below are serve for build a tree view =====================================
	// =========================      			I'm so happy when I CAN DO IT, recursive				======================================

	phanQuyenBus(item: any) {

		let obj, txtKey;

		if (item[this.propNameChild]) {
			//tìm node xem có children trong nó hay ko
			if (item[this.nameNode] == [this.textFeildRoof]) obj = this.lst_DonVi[0];
			else {
				this.findItemCheck(this.lst_DonVi[0], item);
				obj = Object.assign({}, this.g_NodeSlected);
				this.g_NodeSlected = {};
			}
		}
		else
			obj = item;

		if (obj)
			this.checkAllChild(obj);

		txtKey = item[this.nameNode];//gán text node để lát serach ra node này

		if (obj && obj[this.propNameChild] == undefined)//duyệt node cuối cùng của cấp
		{
			this.findParentOfLastNode(this.lst_DonVi[0], item[this.nameNode]);
			if (this.g_ParentOfLastNode != {}) {
				obj = this.g_ParentOfLastNode;
				txtKey = obj[this.nameNode];
				this.g_ParentOfLastNode = {};

				//gán class với checked cho node parent of last node
				obj[this.propNameCss].checked = item[this.propNameCss].checked ? true : (this.countLastCheck(obj) ? (this.countLastCheckNbr(obj) == 0 ? false : true) : false);
				obj[this.propNameCss].parentChk = obj[this.propNameCss].checked ? (this.countCheck(obj) ? 'chk-sty' : '') : '';

			}
		}

		//check các node parent
		this.checkAllParent(obj, txtKey);
	}

	//tìm vị trí node được checked changed
	findItemCheck(itemSrc: any, itemKey: any) {
		let r = undefined;
		if (itemSrc[this.propNameChild]) {

			for (var i = 0; i < itemSrc[this.propNameChild].length; i++) {
				let item = itemSrc[this.propNameChild][i];
				if (item[this.nameNode] == itemKey[this.nameNode]) {
					item[this.propNameCss].checked = itemKey[this.propNameCss].checked;

					if (item[this.propNameCss].checked && item[this.propNameChild]) item[this.propNameCss].parentChk = '';//set checkbox có dấu check về default
					this.g_NodeSlected = Object.assign({}, item);
					r = item;
					break;
				}
				else
					if (item[this.propNameChild]) {
						this.lstNodeSelected.push(item);
						r = this.findItemCheck(item, itemKey);
					}
			}

			return r;
		}
		return itemSrc;
	}

	//check all child của node này
	checkAllChild(itemA: any) {
		if (itemA[this.propNameChild]) {
			itemA[this.propNameChild].map((item, index) => {

				item[this.propNameCss].checked = itemA[this.propNameCss].checked;
				if (!item[this.propNameCss].checked) item[this.propNameCss].parentChk = '';//set checkbox mark default

				if (item[this.propNameChild])
					this.checkAllChild(item);
			});
		}
	}

	//check all parent của node này
	checkAllParent(itemA: any, key: any) {

		//let objParent = this.findParentRecursion(this.lst_DonVi[0], key);

		let objParent = Object.assign({}, itemA);

		if (itemA[this.nameNode] != [this.textFeildRoof]) {
			this.findParentRecursion(this.lst_DonVi[0], key);
			objParent = Object.assign({}, this.g_ParentOfParent);
			this.g_ParentOfParent = {};
		}


		let anCss = {
			collapse: false,
			lastChild: false,
			state: 0,//trạng thái luôn luôn mở node này, 0 -> open, -1 -> close
			checked: false,
			parentChk: '',
			active:false
		};

		//gán thuốc tính anCss cho node
		if (objParent[this.propNameCss]) { }
		else objParent[this.propNameCss] = Object.assign({}, anCss);

		if (objParent) {

			if (objParent[this.propNameCss]) { }
			else { }


			objParent[this.propNameCss].checked = itemA[this.propNameCss].checked ? true : (this.countCheck(objParent) ? (this.countCheckNbr(objParent) == 0 ? false : true) : false);
			objParent[this.propNameCss].parentChk = objParent[this.propNameCss].checked ? (this.countCheck(objParent) ? 'chk-sty' : '') : '';

			if (objParent[this.nameNode] == [this.textFeildRoof]) {

				this.masterNode[this.propNameCss].checked = objParent[this.propNameCss].checked;
				this.masterNode[this.propNameCss].parentChk = objParent[this.propNameCss].checked ? (this.countCheck(objParent) ? 'chk-sty' : '') : '';

				return;
			}//node cuối cùng
			else
				this.checkAllParent(objParent, objParent[this.nameNode]);

		}
	}

	//tìm parent của parent
	findParentRecursion(obj1: any, key: any) {

		let r = undefined;
		if (obj1) {

			if (obj1[this.propNameChild]) {
				for (var i = 0; i < obj1[this.propNameChild].length; i++) {
					if (obj1[this.propNameChild][i][this.nameNode] == key) {
						r = obj1;
						this.g_ParentOfParent = Object.assign({}, obj1);
						break;
					}
					else
						r = this.findParentRecursion(obj1[this.propNameChild][i], key);
				}
				return r;
			}

		}
		return r;
	}

	//check xem có check hết tất cả các check box con không
	countCheck(itemA: any) {
		let r = false;
		let dem = 0;
		if (itemA[this.propNameChild]) {
			for (var i = 0; i < itemA[this.propNameChild].length; i++) {

				if (itemA[this.propNameChild][i][this.propNameCss]) { }
				else { }

				if (itemA[this.propNameChild][i][this.propNameCss] && itemA[this.propNameChild][i][this.propNameCss].checked) {
					dem++;
				}
			}
			r = dem < itemA[this.propNameChild].length; //true -> toàn bộ con ko check hết
		}
		return r;
	}

	//check xem có check hết tất cả các check box con không
	countCheckNbr(itemA: any) {
		let r = 0;
		let dem = 0;
		if (itemA[this.propNameChild]) {
			for (var i = 0; i < itemA[this.propNameChild].length; i++) {

				if (itemA[this.propNameChild][i][this.propNameCss]) { }
				else { }

				if (itemA[this.propNameChild][i][this.propNameCss] && itemA[this.propNameChild][i][this.propNameCss].checked)
					dem++;
			}
			r = dem; //true -> toàn bộ con ko check hết
		}
		return r;
	}

	//mở hoặc đóng node này
	collapseNode(itemA: any) {
		if (itemA[this.propNameCss]) {
			if (itemA[this.propNameCss].checked) {//mở node này ra
				itemA[this.propNameCss].state = -1;//-1 mở node này
				itemA[this.propNameCss].collapse = true;//mở các node con
			}
			if (itemA[this.propNameChild])
				for (var i = 0; i < itemA[this.propNameChild].length; i++)
					this.collapseNode(itemA[this.propNameChild][i]);
		}
	}


	// ============================================================= FOR LAST NODE - IT HAS NO CHILDER ====================================

	//tìm parent trước 1 cấp của node cuối cùng của cấp
	findParentOfLastNode(obj1: any, key: any) {
		if (obj1) {

			if (obj1[this.propNameChild]) {
				for (var i = 0; i < obj1[this.propNameChild].length; i++) {
					if (obj1[this.propNameChild][i][this.nameNode] == key) {
						this.g_ParentOfLastNode = Object.assign({}, obj1);
						break;
					}
					else
						this.findParentOfLastNode(obj1[this.propNameChild][i], key);
				}
			}
		}

	}

	//check xem có check hết tất cả các check box con không
	countLastCheck(itemA: any) {
		let r = false;
		let dem = 0;
		if (itemA[this.propNameChild]) {
			for (var i = 0; i < itemA[this.propNameChild].length; i++) {
				if (itemA[this.propNameChild][i][this.propNameCss] && itemA[this.propNameChild][i][this.propNameCss].checked) {
					dem++;
				}
			}
			r = dem < itemA[this.propNameChild].length; //true -> toàn bộ con ko check hết
		}
		return r;
	}

	//check xem có check hết tất cả các check box con không
	countLastCheckNbr(itemA: any) {
		let r = 0;
		let dem = 0;
		if (itemA[this.propNameChild]) {
			for (var i = 0; i < itemA[this.propNameChild].length; i++) {
				if (itemA[this.propNameChild][i][this.propNameCss] && itemA[this.propNameChild][i][this.propNameCss].checked)
					dem++;
			}
			r = dem; //true -> toàn bộ con ko check hết
		}
		return r;
	}


	//================================================================================================================================


	//================================================================================================================================
	// =================================================== END BUILD TREE VIEW ========================================================

}
