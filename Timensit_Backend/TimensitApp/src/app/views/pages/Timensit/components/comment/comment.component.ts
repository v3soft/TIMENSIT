// Angular
import { Component, OnInit, ChangeDetectionStrategy, OnDestroy, ChangeDetectorRef, Inject, Output, Input, EventEmitter, SimpleChange, AfterViewInit, ElementRef, ViewChild, Pipe } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
// Material
import { MatPaginator, MatSort, MatDialog, MatTableDataSource, MAT_DIALOG_DATA, MatDialogRef, MatMenuTrigger } from '@angular/material';
// RxJS
import { Observable, BehaviorSubject, Subscription, Subject, interval } from 'rxjs';
// NGRX
// Service
import { LayoutUtilsService, MessageType } from '../../../../../core/_base/crud';
import { CommentService } from './comment.service';
import { EmotionDialogComponent } from '../emotion-dialog/emotion-dialog.component';
import { GlobalVariable } from '../../../../pages/global';
import { CommentEditDialogComponent } from './comment-edit-dialog/comment-edit-dialog.component';
import { PopoverContentComponent } from 'ngx-smart-popover';
import { DomSanitizer } from '@angular/platform-browser';
import { TokenStorage } from '../../../../../core/auth/_services/token-storage.service';
import { CommonService } from '../../services/common.service';
import { AuthService } from '../../../../../core/auth';
import { ReviewExportComponent } from '../review-export/review-export.component';

@Component({
	selector: 'kt-comment',
	templateUrl: './comment.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CommentComponent implements OnInit, OnDestroy, AfterViewInit {
	@Output() ListResult: EventEmitter<any> = new EventEmitter<any>();//event for component
	@Output() inserted: EventEmitter<any> = new EventEmitter<any>();//event for component
	@Input() Id: number;//Id của đối tượng
	@Input() View?: boolean = false;//view cho xem hay ko
	//@Input() ListChecked?: any[] = [];//tên node roof có chứa các con của node, default = children
	@Input() Loai: number;//1: đề xuất,2 hồ sơ, 3: số liệu
	// nếu không cả Mã và Tên đều Emty thì nút xuất file word sẽ không xuất hiện

	listResult = new Subject();

	// Public properties
	ItemData: any = {};
	FormControls: FormGroup;
	hasFormErrors: boolean = false;
	disBtnSubmit: boolean = false;
	loadingSubject = new BehaviorSubject<boolean>(true);
	loading$: Observable<boolean>;
	viewLoading: boolean = false;
	isChange: boolean = false;
	isZoomSize: boolean = false;
	LstDanhMucKhac: any[] = [];
	public datatreeDonVi: BehaviorSubject<any[]> = new BehaviorSubject([]);
	private componentSubscriptions: Subscription;
	ListDonViCon: any[] = [];
	ListVanBan: any[] = [];
	datasource: any;

	ListAttachFile: any[] = [];
	ListYKien: any[] = [];
	AcceptInterval: boolean = true;
	NguoiNhan: string = '';
	//NguoiNhans:any[]=[{FullName:'người 1'},{FullName:'người 2'}];

	Comment: string = '';
	AttachFileComment: any[] = [];
	fileControl: FormControl;
	setting: any = {
		ACCEPT_DINHKEM: '',
		MAX_SIZE: 0
	};
	files: any = {};
	//reload: boolean = true;
	UserData: any = {};
	emotions: any = {};
	accounts: any = {};
	icons: any[] = [];
	public anchors;
	//tag username
	@ViewChild('myPopoverC', { static: true }) myPopover: PopoverContentComponent;
	selected: any[] = [];
	selectedChild: any[] = [];
	listUser: any[] = [];
	options: any = {};
	@ViewChild('matInput', { static: true }) matInput: ElementRef;
	@ViewChild('hiddenText', { static: true }) textEl: ElementRef;
	CommentTemp: string = '';
	indexxxxx: number = -1;
	item_choose: number = -1;
	@ViewChild('myPopoverB', { static: true }) myPopoverU: PopoverContentComponent;
	it: any = {};
	constructor(
		private FormControlFB: FormBuilder,
		public dialog: MatDialog,
		private layoutUtilsService: LayoutUtilsService,
		private changeDetectorRefs: ChangeDetectorRef,
		private tokenStorage: TokenStorage,
		private service: CommentService,
		public weworkService: CommonService,
		private elementRef: ElementRef,
		private auth: AuthService,
		private sanitized: DomSanitizer) {

	}
	transform(value) {
		return this.sanitized.bypassSecurityTrustHtml(value);
	}

	/**
	 * On init
	 */

	ngOnChanges(changes: SimpleChange) {
		if (changes['Id']) {
			this.ngOnInit();
		}
	}

	async ngOnInit() {
		if (GlobalVariable.icons.length == 0) {
			this.auth.getDictionary().subscribe(res => {
				if (res && res.status == 1) {
					res.data.emotions.map(x => {
						GlobalVariable.emotions[x.key] = x.value;
					})
					res.data.accounts.map(x => {
						GlobalVariable.accounts[x.key] = x.value;
					})
					GlobalVariable.icons = res.data.icons;

					this.emotions = GlobalVariable.emotions;
					this.accounts = GlobalVariable.accounts;
					this.icons = GlobalVariable.icons;
				}
			})
		}
		this.emotions = GlobalVariable.emotions;
		this.accounts = GlobalVariable.accounts;
		this.icons = GlobalVariable.icons;
		this.options = this.getOptions();
		this.weworkService.getDSNguoiDungLite().subscribe(res => {
			this.changeDetectorRefs.detectChanges();
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
			}
			this.options = this.getOptions();
			this.changeDetectorRefs.detectChanges();
		});
		this.tokenStorage.getUserInfo().subscribe(res => {
			this.UserData = {
				Image: res.avata,
				HoTen: res.fullname,
				ChucVu: res.tenvaitro,
				Username: res.username,
			};
		})
		this.AcceptInterval = true;
		this.viewLoading = true;
		if (this.Id > 0) {
			this.getDSYKien();
			//setInterval(() => {
			//	if (this.AcceptInterval)
			//		this.getDSYKien_Interval();
			//}, 500);
		}
	}
	ngAfterViewInit() {
		this.anchors = this.elementRef.nativeElement.querySelectorAll('.inline-tag');
		this.anchors.forEach((anchor: HTMLAnchorElement) => {
			anchor.addEventListener('click', this.clickOnUser)
		})
	}

	getDSYKien() {
		this.service.getDSYKien(this.Id, this.Loai).subscribe(res => {
			if (res && res.status == 1) {
				this.ListYKien = res.data;
				//console.log("lst Y Kien", this.ListYKien)
				//for (var i = 0; i < this.ListYKien.length; i++) {
				//	for (var a = 0; a < this.ListYKien[i].NguoiNhans.length; a++) {
				//		this.NguoiNhan += this.ListYKien[i].NguoiNhans[a].FullName + '\n';
				//	}
				//}
				this.createForm();
				this.changeDetectorRefs.detectChanges();
			}
		});
	}

	getDSYKien_Interval() {
		//this.NguoiNhan='';
		var NguoiNhan_Tam = '';
		this.service.getDSYKien(this.Id, this.Loai).subscribe(res => {
			if (res && res.status == 1) {
				let data: any = res.data;

				for (var j = 0; j < data.length; j++) {
					let check: boolean = false;
					for (var i = 0; i < this.ListYKien.length; i++) {
						if (data[j].IdRow == this.ListYKien[i].IdRow) {
							check = true;
							this.ListYKien[i].CreatedDate = data[j].CreatedDate;
							this.ListYKien[i].comment = data[j].comment;
							this.ListYKien[i].NguoiTao.hoten = data[j].NguoiTao.hoten;
							this.ListYKien[i].NguoiTao.image = data[j].NguoiTao.image;
							this.ListYKien[i].Attachments = data[j].Attachment;

							for (var a = 0; a < this.ListYKien[i].NguoiNhans.length; a++) {
								NguoiNhan_Tam += this.ListYKien[i].NguoiNhans[a].NguoiTao.hoten + '\n';
							}
							this.NguoiNhan = NguoiNhan_Tam;
							this.Children_Interval(data[j].Children, this.ListYKien[i].Children);
						}
					}
					if (!check) {
						this.ListYKien.push(data[j]);
						//if (data[j].Attachment != undefined)
						//	this.AttachFileComment.push(data[j].Attachment);
						//else
						//	this.AttachFileComment.push([]);
					}
				}
				this.changeDetectorRefs.detectChanges();
			}
		});
	}

	Children_Interval(data: any, children: any) {
		for (var j = 0; j < data.length; j++) {
			let check: boolean = false;
			for (var i = 0; i < children.length; i++) {
				if (data[j].IdRow == children[i].IdRow) {
					check = true;
					children[i].CreatedDate = data[j].CreatedDate;
					children[i].comment = data[j].comment;
					children[i].NguoiTao.hoten = data[j].NguoiTao.hoten;
					children[i].NguoiTao.image = data[j].NguoiTao.image;
					children[i].Attachments = data[j].Attachment;
					this.changeDetectorRefs.detectChanges();
				}
			}
			if (!check) {
				children.push(data[j]);
				this.ListAttachFile.push([])
			}
		}
	}


	CheckedChange(p: any, e: any) {
		p.check = e;
	}


	DateChanged(value: any, ind: number) {
		if (ind == 1) {
			let batdau = value.targetElement.value.replace(/-/g, '/').split('T')[0].split('/');
			if (+batdau[0] < 10 && batdau[0].length < 2)
				batdau[0] = '0' + batdau[0];
			if (+batdau[1] < 10 && batdau[1].length < 2)
				batdau[1] = '0' + batdau[1];

			this.FormControls.controls['bDNghi'].setValue(batdau[2] + '-' + batdau[1] + '-' + batdau[0]);
		}
		if (ind == 2) {
			let ketthuc = value.targetElement.value.replace(/-/g, '/').split('T')[0].split('/');
			if (+ketthuc[0] < 10 && ketthuc[0].length < 2)
				ketthuc[0] = '0' + ketthuc[0];
			if (+ketthuc[1] < 10 && ketthuc[1].length < 2)
				ketthuc[1] = '0' + ketthuc[1];

			this.FormControls.controls['kTNghi'].setValue(ketthuc[2] + '-' + ketthuc[1] + '-' + ketthuc[0]);
		}
	}

	/**
	 * On destroy
	 */
	ngOnDestroy() {
		if (this.componentSubscriptions) {
			this.componentSubscriptions.unsubscribe();
		}

		// if (this.interval) {
		// 	clearInterval(this.interval);
		// }

		this.AcceptInterval = false;
	}

	/**
	 * Create form
	 */
	createForm() {
		// this.FormControls = this.FormControlFB.group({
		// });

		// for (var i = 0; i < this.ListYKien.length; i++) {
		// 	//this.itemForm.addControl(this.Listbiuldview[i].BuildEditor, new FormControl('', Validators.required));
		// 	this.FormControls.addControl("FctAttachFile" + i, new FormControl([{filename:'Khoatest.docx',StrBase64:'1234553543534'}]));
		// }


		for (var i = 0; i < this.ListYKien.length; i++) {
			//if (this.ListYKien[i].Attachment != undefined)
			//	this.ListAttachFile.push(this.ListYKien[i].Attachment)
			//else
			this.ListAttachFile.push([])
			//{filename:'Khoatest.docx',StrBase64:'1234553543534'}
			//this.ListAttachFile['FctAttachFile'+i]={filename:'Khoatest.docx',StrBase64:'1234553543534'};
		}
	}

	GetListAttach(ind: number): any {
		return this.ListAttachFile[ind];
	}

	/**
	 * Check control is invalid
	 * @param controlName: string
	 */
	isControlInvalid(controlName: string): boolean {
		const control = this.FormControls.controls[controlName];
		const result = control.invalid && control.touched;
		return result;
	}

	/**
	 * Save data
	 *
	 * @param withBack: boolean
	 */
	onSubmit(type: boolean) {
		let ArrDVC: any[] = [];
		for (var i = 0; i < this.ListDonViCon.length; i++) {
			if (this.ListDonViCon[i].check) {
				ArrDVC.push(this.ListDonViCon[i]);
			}
		}
		if (type) {
			//this.dialogRef.close(ArrDVC);
		}
		else {
			//this.dialogRef.close();
		}
	}

	ShowOrHideComment(ind: number) {
		var x = document.getElementById("ykchild" + ind);
		//var a = document.getElementById("btnHideyk" + ind);
		//var b = document.getElementById("btnShowyk" + ind);
		if (!x.style.display || x.style.display === "none") {
			x.style.display = "block";
			//a.style.display = "block";
			//b.style.display = "none";
		} else {
			x.style.display = "none";
			//a.style.display = "none";
			//b.style.display = "block";
		}
	}


	//type=1: comment, type=2: reply
	CommentInsert(e: any, Parent: number, ind: number, type: number) {
		var objSave: any = {};
		objSave.comment = e.trim();
		if (type == 1) { objSave.Attachments = this.AttachFileComment; }
		else { objSave.Attachments = this.ListAttachFile[ind]; }
		if (objSave.comment == '' && (objSave.Attachments == null || objSave.Attachments.length == 0)) {
			return;
		}
		objSave.id_parent = Parent;
		objSave.object_type = this.Loai;
		objSave.object_id = this.Id;
		if (type == 1) {
			objSave.Attachments = this.AttachFileComment;
			objSave.Users = this.selected;
		}
		else {
			objSave.Attachments = this.ListAttachFile[ind];
			objSave.Users = this.getListTagUser(e, this.selectedChild);
		}
		this.service.getDSYKienInsert(objSave).subscribe(res => {
			if (res && res.status == 1) {
				if (type == 1) { this.Comment = ''; this.AttachFileComment = []; }
				else {
					(<HTMLInputElement>document.getElementById("CommentRep" + ind)).value = "";
					this.ListAttachFile[ind] = [];
				}
				if (Parent == 0) {
					this.ListYKien.unshift(res.data);
					this.inserted.emit(res.data);
					//if (res.data.Attachment != undefined)
					//	this.ListAttachFile.unshift(res.data.Attachment);
					//else
					//	this.ListAttachFile.unshift([]);
				} else {
					this.ListYKien[ind].Children.unshift(res.data);
				}
				this.changeDetectorRefs.detectChanges();
			}
		});
	}

	selectFile_PDF(ind) {
		if (ind == -1) {
			let f = document.getElementById("PDFInpdd");
			f.click();
		}
		else {
			let f = document.getElementById("PDFInpdd" + ind);
			f.click();
		}

	}
	onSelectFile_PDF(event, ind) {
		// event.target.type='text';
		// event.target.type='file';
		if (event.target.files && event.target.files[0]) {
			var filesAmount = event.target.files[0];
			var Strfilename = filesAmount.name.split('.');
			// if (Strfilename[Strfilename.length - 1] != 'docx' && Strfilename[Strfilename.length - 1] != 'doc') {
			// 	this.layoutUtilsService.showInfo("File không đúng định dạng");
			// 	return;
			// }
			if (ind == -1) {
				for (var i = 0; i < this.AttachFileComment.length; i++) {
					if (filesAmount.name == this.AttachFileComment[i].filename) {
						this.layoutUtilsService.showInfo("File đã tồn tại");
						return;
					}
				}
			}
			else {
				for (var i = 0; i < this.ListAttachFile[ind].length; i++) {
					if (filesAmount.name == this.ListAttachFile[ind][i].filename) {
						this.layoutUtilsService.showInfo("File đã tồn tại");
						return;
					}
				}
			}

			event.target.type = 'text';
			event.target.type = 'file';
			var reader = new FileReader();
			//this.FileAttachName = filesAmount.name;
			let base64Str: any;
			reader.onload = (event) => {
				base64Str = event.target["result"]
				var metaIdx = base64Str.indexOf(';base64,');
				base64Str = base64Str.substr(metaIdx + 8); // Cắt meta data khỏi chuỗi base64

				//this.FileAttachStrBase64 = base64Str;
				if (ind == -1) {
					this.AttachFileComment.push({ filename: filesAmount.name, strBase64: base64Str });
					this.changeDetectorRefs.detectChanges();
				}
				else {
					this.ListAttachFile[ind].push({ filename: filesAmount.name, strBase64: base64Str });
					//console.log(this.ListAttachFile);
					this.changeDetectorRefs.detectChanges();
				}
			}

			reader.readAsDataURL(filesAmount);

		}
	}
	DeleteFile_PDF(ind, ind1) {
		let id;
		if (ind == -1)
			id = this.AttachFileComment[ind1].id_row;
		else
			id = this.ListAttachFile[ind][ind1].id_row;
		if (!id) {
			if (ind == -1)
				this.AttachFileComment.splice(ind1, 1);
			else
				this.ListAttachFile[ind].splice(ind1, 1);
			return;
		}
		//this.ListAttachFile[ind].push({filename:filesAmount.name,StrBase64:base64Str});
		this.weworkService.Delete_FileDinhKem(id).subscribe(res => {
			if (res && res.status == 1) {
				if (ind == -1) {
					this.AttachFileComment.splice(ind1, 1);
				}
				else {
					this.ListAttachFile[ind].splice(ind1, 1);
				}
			}
		})
	}


	DownloadFile(link) {
		window.open(link);
	}
	preview(link) {
		window.open(link);
		//this.layoutUtilsService.showInfo("preview:" + link);
	}
	reply(item, index) {
		var ele = (<HTMLInputElement>document.getElementById("CommentRep" + index));
		ele.value = "@" + item.NguoiTao.username + " ";
		ele.focus();
	}
	openEmotionDialog(ind, id_p) {
		const dialogRef = this.dialog.open(EmotionDialogComponent, { data: {}, width: '500px' });
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}
			else {
				this.CommentInsert(res, id_p, ind, 2);
			}
		});
	}
	clickOnUser = (event) => {
		// Prevent opening anchors the default way
		event.preventDefault();
		event.stopPropagation();
		const anchor = event.target as HTMLAnchorElement;

		this.it = this.listUser.find(x => x.username == anchor.getAttribute('data-username'));
		this.changeDetectorRefs.detectChanges();
		this.myPopoverU.show();
		//let el = event.parentElement;
		//this.myPopoverU.top = el.offsetTop;
		//this.myPopoverU.left = el.offsetLeft;
	}
	clickonbox($event) {
		this.myPopoverU.hide();
	}

	parseHtml(str) {
		if (!str)
			return '';
		var html = str;
		var reg = /@\w*(\.[A-Za-z]\w*)|\@[A-Za-z]\w*/gm
		var reg1 = /\:[A-Za-z]\w*\:/gm
		var match = html.match(reg);
		if (match != null) {
			for (var i = 0; i < match.length; i++) {
				var key = match[i] + '';
				var username = key.slice(1);
				if (this.accounts[key]) {
					var re = `<span class="url inline-tag" data-username="${username}">${this.accounts[key]}</span>`;
					html = html.replace(key, re);
				}
			}
		}
		match = html.match(reg1);
		if (match != null) {
			for (var i = 0; i < match.length; i++) {
				var key = match[i] + '';
				if (this.emotions[key]) {
					var re = `<img src="${this.emotions[key]}" />`;
					html = html.replace(key, re);
				}
			}
		}
		setTimeout(() => {
			this.ngAfterViewInit();
		}, 10)
		//return html;
		return this.sanitized.bypassSecurityTrustHtml(html)
	}
	like(item, icon) {
		this.service.like(item.id_row, icon).subscribe(res => {
			if (res && res.status == 1) {
				item.Like = res.data.Like;
				item.Likes = res.data.Likes;
				this.changeDetectorRefs.detectChanges();
			}
		})
	}
	remove(item, index, indexc = -1) {
		this.service.remove(item.id_row).subscribe(res => {
			if (res && res.status == 1) {
				if (indexc >= 0)//xoa con
					this.ListYKien[index].Children.splice(indexc, 1);
				else
					this.ListYKien.splice(index, 1);
				this.changeDetectorRefs.detectChanges();
			}
		})
	}

	initUpdate(item, index, indexc = -1) {
		var data = Object.assign({}, item);
		const dialogRef = this.dialog.open(CommentEditDialogComponent, { data: data, width: '500px' });
		dialogRef.afterClosed().subscribe(res => {
			if (res) {
				item.comment = res.comment
				this.changeDetectorRefs.detectChanges();
			}
		});
	}
	//#region tag username
	getOptions() {
		var options: any = {
			showSearch: false,
			keyword: this.getKeyword(),
			data: this.listUser.filter(x => this.selected.findIndex(y => x.id_nv == y.id_nv) < 0),
		};
		return options;
	}
	getKeyword() {
		let i = this.CommentTemp.lastIndexOf('@');
		if (i >= 0) {
			let temp = this.CommentTemp.slice(i);
			if (temp.includes(' '))
				return '';
			return this.CommentTemp.slice(i);
		}
		return '';
	}
	ItemSelected(data) {
		if (this.item_choose == -1) {
			this.selected.push(data);
		} else {
			this.selectedChild.push(data);
		}

		let i = this.CommentTemp.lastIndexOf('@');
		this.CommentTemp = this.CommentTemp.substr(0, i) + '@' + data.username + ' ';
		this.myPopover.hide();
		let ele = (<HTMLInputElement>this.matInput.nativeElement);
		if (this.item_choose >= 0)
			ele = (<HTMLInputElement>document.getElementById("CommentRep" + this.item_choose));
		ele.value = this.CommentTemp;
		ele.focus();

		this.changeDetectorRefs.detectChanges();
	}
	click($event, vi = -1) {
		this.myPopover.hide();
	}
	onSearchChange($event, vi = -1) {


		this.item_choose = vi;
		if (vi >= 0)
			this.CommentTemp = (<HTMLInputElement>document.getElementById("CommentRep" + vi)).value;
		else
			this.CommentTemp = this.Comment;
		if (this.selected.length > 0) {
			var reg = /@\w*(\.[A-Za-z]\w*)|\@[A-Za-z]\w*/gm
			var match = this.CommentTemp.match(reg);
			if (match != null && match.length > 0) {
				let arr = match.map(x => x);
				this.selected = this.selected.filter(x => arr.includes('@' + x.username));
			} else {
				this.selected = [];
			}
		}



		this.options = this.getOptions();
		if (this.options.keyword) {
			console.log(" key: ", this.options.keyword);
			let el = $event.currentTarget;
			let rect = el.getBoundingClientRect();
			console.log(rect);
			if (this.item_choose >= 0) {
				var ele = (<HTMLInputElement>document.getElementById("inputtext" + this.item_choose));
				console.log(ele.offsetTop)
				var w = this.textEl.nativeElement.offsetWidth + 100;
				var h = ele.offsetTop + 200;
				this.myPopover.show();
				this.myPopover.top = el.offsetTop + h;
				this.myPopover.left = el.offsetLeft + w;
			} else {
				var w = this.textEl.nativeElement.offsetWidth + 25;
				var h = 0;
				this.myPopover.show();
				this.myPopover.top = el.offsetTop + h;
				this.myPopover.left = el.offsetLeft + w;
			}
			//this.myPopover.top = rect.y + h;
			//this.myPopover.left = w ;
			this.changeDetectorRefs.detectChanges();
		} else {
			this.myPopover.hide();
		}
	}

	getListTagUser(chuoi, array) {
		var arr = [];
		var user = []
		chuoi.split(' ').forEach(element => {
			if (element[0] == '@') {
				user.push(element);
			}
		});;
		user = Array.from(new Set(user));
		user.forEach(element => {
			var x = array.find(x => x.username == element.substr(1));
			if (x) {
				arr.push(x)
			}
		})
		return arr;

	}
	inhuongdan(id_quatrinh_lichsu) {
		
	}



	//#endregion
}

// var ele = (<HTMLInputElement>document.getElementById("inputtext" + this.item_choose));
// 			console.log(ele.offsetTop)
// 			var w = this.textEl.nativeElement.offsetWidth + 60;
// 			var h = ele.offsetTop + 30;
// 			this.myPopoverChild.show();
// 			this.myPopoverChild.top = el.offsetTop + h;
// 			this.myPopoverChild.left = el.offsetLeft + w;
// 			//this.myPopover.top = rect.y + h;
// 			//this.myPopover.left = w ;
// 			this.changeDetectorRefs.detectChanges();
