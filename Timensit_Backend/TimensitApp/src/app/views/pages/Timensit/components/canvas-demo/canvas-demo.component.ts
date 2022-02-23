import { Component, OnInit, ViewChild, ElementRef, HostListener, ViewEncapsulation, OnChanges, ChangeDetectionStrategy, ChangeDetectorRef, Input, EventEmitter, Output } from '@angular/core';
import { DiagramCanVas } from './canvas.class';
import { CanvasDemoService } from './canvas-demo.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material';
import { LayoutUtilsService, MessageType } from '../../../../../../../src/app/core/_base/crud';
import { CanvasDemoDialogComponent } from './canvas-demo-dialog.component';
import { TokenStorage } from '../../../../../core/auth/_services/token-storage.service';
import { DynamicProcessService } from '../../services/dynamic-process.service';

@Component({
	selector: 'kt-canvas-demo',
	templateUrl: './canvas-demo.component.html',
	styleUrls: ['./canvas-demo.component.scss'],
	encapsulation: ViewEncapsulation.None,
	changeDetection: ChangeDetectionStrategy.OnPush
})
export class CanvasDemoComponent implements OnInit, OnChanges {
	@Input() API: string;
	@Input() width: number = 500;
	@Input() height: number = 700;
	@Input() item: any;
	@Input() ReadOnly: boolean = false;
	@Input() showViewCheckers: boolean = false;
	@Input() ColorComeBack: string = "#FF0000";
	@Output() reload = new EventEmitter();
	@Output() viewChecker = new EventEmitter();
	@ViewChild("DemoCanvas", { static: true }) DemoCanvas: ElementRef;
	@ViewChild("tooltip", { static: true }) tooltip: ElementRef;
	//const
	LineWidth = 1;
	GridSize = 20;
	widthheadarrow = 5;
	widthRect = 5;
	heightRect = 3;
	arrow_w = 1;
	arrow_h = 4;
	canvas: any;
	data: any;
	formNode: FormGroup;
	disableTitle = false;
	disableArrow = false;
	selectedNode: any = null;
	//
	diagram_canvas: DiagramCanVas;
	// arrNode: any[] = [
	//   { ID: 1, Title: "Tiếp nhận" },
	//   { ID: 2, Title: "Bàn giao cho người làm" },
	//   { ID: 3, Title: "Thực hiện trả lời" },
	//   { ID: 4, Title: "Duyệt thực hiện" },
	//   { ID: 5, Title: "Duyệt cấp trên" },
	// ];
	// arrArrow: any[] = [
	//   { ID: 1, Start: 1, End: 2 },
	//   { ID: 2, Start: 2, End: 3 },
	//   { ID: 3, Start: 2, End: 1 },
	//   { ID: 4, Start: 3, End: 4 },
	//   { ID: 5, Start: 4, End: 3 },
	//   { ID: 6, Start: 4, End: 5 },
	// ]; 
	// arrNode: any[] = [
	//   { ID: 1, Title: "Bàn giao cho nhân viên các đơn vị dưới", BorderColor: "#4FADCA" },
	//   { ID: 2, Title: "Step 2", BorderColor: "#4CAF50" },
	//   { ID: 3, Title: "Step 3", BorderColor: "#FF0000" },
	//   { ID: 4, Title: "Step 4", BorderColor: "#a24caf" }, 
	//   { ID: 5, Title: "Step 5", BorderColor: "#4FADCA" },
	//   { ID: 6, Title: "Step 6" },
	//   { ID: 7, Title: "Step 7", BorderColor: "#d2bd07" },
	//   { ID: 8, Title: "Step 8", BorderColor: "#0e0fcc" },
	//   { ID: 9, Title: "Step 9", BorderColor: "#104616" },
	//   // { ID: 10, Title: "Step 10" },
	//   { ID: 11, Title: "Step 11" },
	//   { ID: 12, Title: "Step 12" },
	// ];
	// arrArrow: any[] = [
	//   { ID: 1, Start: 1, End: 2 },
	//   { ID: 3, Start: 1, End: 7 },
	//   { ID: 4, Start: 1, End: 8 },
	//   { ID: 5, Start: 1, End: 9 },
	//   { ID: 6, Start: 2, End: 3 },
	//   { ID: 7, Start: 2, End: 4 },
	//   { ID: 8, Start: 2, End: 5 },
	//   { ID: 9, Start: 3, End: 6 },
	//   { ID: 10, Start: 5, End: 6 },
	//   { ID: 2, Start: 6, End: 1 },
	//   { ID: 11, Start: 6, End: 7 },
	//   { ID: 12, Start: 8, End: 1 },
	//   { ID: 13, Start: 8, End: 5 },
	//   { ID: 14, Start: 9, End: 6 },
	//   { ID: 15, Start: 5, End: 10 },
	//   { ID: 16, Start: 9, End: 10 },
	//   { ID: 17, Start: 9, End: 3 },
	//   { ID: 18, Start: 9, End: 4 },
	// ];
	rR: {};
	constructor(
		private canvasDemoService: CanvasDemoService,
		private formNodeFB: FormBuilder,
		public dialog: MatDialog,
		private changeDetectorRef: ChangeDetectorRef,
		private layoutUtilsService: LayoutUtilsService,
		private service: DynamicProcessService,
		private TokenStorage: TokenStorage

	) { }
	ngOnChanges() {
		if (this.API) {
			this.selectedNode = undefined;
			this.canvasDemoService.getData(this.API).subscribe(data => {
				if (data && data.status == 1) {
					this.data = data.data;
					//console.log("data canvas", this.data);
					this.canvas = this.DemoCanvas.nativeElement;
					this.diagram_canvas = new DiagramCanVas(this.canvas, this.data.arrNode, this.data.arrArrow, this.tooltip.nativeElement,
						{
							onDraw: this.OnDraw,
							nodeClicked: this.nodeClicked,
							onZoomed: this.OnZoomed,
							onMoved: this.OnMoved,
							_this: this
						});
					this.diagram_canvas.LineWidth = this.LineWidth;
					this.diagram_canvas.GridSize = this.GridSize;
					this.diagram_canvas.widthheadarrow = this.widthheadarrow;
					this.diagram_canvas.widthheadarrow = this.widthheadarrow;
					this.diagram_canvas.arrow_w = this.arrow_w;
					this.diagram_canvas.arrow_h = this.arrow_h;
					this.diagram_canvas.color_back = this.ColorComeBack;
					// this.diagram_canvas.ZoomPan();
					this.diagram_canvas.Draw();
					this.diagram_canvas.space = true;
					this.changeDetectorRef.detectChanges();
				}
				else {
					this.layoutUtilsService.showError(data.error.message);
				}
			});
		}
	}
	ngOnInit() {

		if(window.innerWidth < 625){
			this.width = 300;
			this.height = 1000;
		}

		this.TokenStorage.getUserRolesObject().subscribe(t => {
			this.rR = t;
		});

		//this.canvasDemoService.getJSON().subscribe(data => {
		//    this.data = data.data;
		//    this.canvas = this.DemoCanvas.nativeElement;
		//    this.diagram_canvas = new DiagramCanVas(this.canvas, this.data.arrNode, this.data.arrArrow, this.tooltip.nativeElement,
		//        {
		//            onDraw: this.OnDraw,
		//            nodeClicked: this.nodeClicked,
		//            onZoomed: this.OnZoomed,
		//            onMoved: this.OnMoved,
		//            _this: this
		//        });
		//    this.diagram_canvas.LineWidth = this.LineWidth;
		//    this.diagram_canvas.GridSize = this.GridSize;
		//    this.diagram_canvas.widthheadarrow = this.widthheadarrow;
		//    this.diagram_canvas.widthheadarrow = this.widthheadarrow;
		//    this.diagram_canvas.arrow_w = this.arrow_w;
		//    this.diagram_canvas.arrow_h = this.arrow_h;
		//    // this.diagram_canvas.ZoomPan();
		//    this.diagram_canvas.Draw();
		//});
	}

	//@HostListener('document:keydown', ['$event'])
	//_keydown(event) {
	//	//truyền space để biết là người dùng đã nhấn phím space chưa
	//	if (event.keyCode === 32 && this.diagram_canvas.isHover) {
	//		this.diagram_canvas.space = true;
	//		event.preventDefault();
	//		event.stopPropagation();
	//	}
	//}

	//@HostListener('document:keyup', ['$event'])
	//_keyup(event) {
	//	if (event.keyCode === 32) this.diagram_canvas.space = false;
	//}

	// ScaleDown() {
	//   this.diagram_canvas.scaleFactor /= 1.1;
	//   var cx = this.canvas.width / 2, cy = this.canvas.height / 2;
	//   let X = this.diagram_canvas.scaleFactor * cx;
	//   let Y = this.diagram_canvas.scaleFactor * cy;
	//   let panX = (cx - X);
	//   let panY = (cy - Y);
	//   this.diagram_canvas.panX = panX;
	//   this.diagram_canvas.panY = panY;
	//   this.diagram_canvas.ZoomPan();
	//   this.diagram_canvas.Draw();
	// }
	// ScaleUp() {
	//   this.diagram_canvas.scaleFactor *= 1.1;
	//   var cx = this.canvas.width / 2, cy = this.canvas.height / 2;
	//   let X = this.diagram_canvas.scaleFactor * cx;
	//   let Y = this.diagram_canvas.scaleFactor * cy;
	//   let panX = (cx - X);
	//   let panY = (cy - Y);
	//   this.diagram_canvas.panX = panX;
	//   this.diagram_canvas.panY = panY;
	//   this.diagram_canvas.ZoomPan();
	//   this.diagram_canvas.Draw();
	// }

	/* Callback event OnDraw */
	OnDraw() {
		////console.log("OnDraw");
	}
	OnZoomed(event) {
		////console.log("on zoom", event);
	}
	OnMoved(event) {
		////console.log("on move", event);
	}
	nodeClicked(node, _this) {
		//if (!_this.diagram_canvas.space) {
		_this.selectedNode = node;
		_this.changeDetectorRef.detectChanges();
		//}		
	}

	ScaleReset() {
		this.selectedNode = undefined;
		this.diagram_canvas.resetCanvas();
		this.diagram_canvas.Draw();
	}
	StartEditNode(event) {
		this.disableTitle = true;
		this.formNode = this.formNodeFB.group({
			Title: [this.selectedNode.Title, Validators.required]
		});
	}
	EditNodeSubmit() {
		this.selectedNode.Title = this.formNode.controls['Title'].value;
		this.diagram_canvas.UpdateTitleNode(this.selectedNode);
		this.disableTitle = false;
	}
	StartEditArrow(event, item, type) {
		// //console.log("edit arrow", event);
		// this.disableArrow=true;
		// //console.log("selectedNode", this.selectedNode);
		// //console.log("item", item);  
		this.openDialog(type, item);
	}
	openDialog(type, item): void {
		//console.log("item", item);
		const dialogRef = this.dialog.open(CanvasDemoDialogComponent, {
			width: '500px',
			data: {
				type: type,
				selectedNode: this.selectedNode,
				dataNode: this.data.arrNode,
				item: item
			}
		});

		dialogRef.afterClosed().subscribe(result => {
			if (result)
				this.reload.emit(true);
		});
	}
	StartDeleteArrow(event, item) {
		const _title: string = 'Xác nhận';
		const _description: string = 'Bạn chắc chắn xóa liên kết?';
		const _waitDesciption: string = 'Liên kết đang được xóa...';
		const _deleteMessage = 'Xóa liên kết thành công';

		const dialogRef = this.layoutUtilsService.deleteElement(_title, _description, _waitDesciption);
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}
			this.service.deleteNextStep(item.arrow.Start, item.arrow.End).subscribe(res => {
				if (res && res.status == 1) {
					this.layoutUtilsService.showInfo(_deleteMessage);
					this.reload.emit(true);
				} else {
					this.layoutUtilsService.showError(res.error.message);
				}
			});
		});
	}
	ngOnDestroy() {
		if (this.diagram_canvas)
			this.diagram_canvas.space = false
	}

	//#region step
	initCreateStep() {
		let data = {
			IdLuong: this.data.IdLuong,
			Title: "",
			Type: 1,
			IdQuyen: null,
			IdForm: null,
			Prior: 1
		};
		//const dialogRef = this.dialog.open(StepDialogComponent, { data: {_item:data} });
		//dialogRef.afterClosed().subscribe(res => {
		//	if (res)
		//		this.reload.emit(true);
		//});
	}

	editStep() {
		let _item = Object.assign({}, this.selectedNode.data);
		// console.log("item node", _item);
		// console.log("item node", this.data);
		//const dialogRef = this.dialog.open(StepDialogComponent, { data: { _item } });
		//dialogRef.afterClosed().subscribe(res => {
		//	if (res)
		//		this.reload.emit(true);
		//});
	}
	deleteStep() {
		let _item = Object.assign({}, this.selectedNode.data);
		const _title: string = 'Xác nhận';
		const _description: string = `Bạn chắc chắn xóa bước '${_item.Title}'`;
		const _waitDesciption: string = 'Bước đang được xóa...';
		const _deleteMessage = `Xóa bước ${_item.Title} thành công`;

		const dialogRef = this.layoutUtilsService.deleteElement(_title, _description, _waitDesciption);
		dialogRef.afterClosed().subscribe(res => {
			if (!res) {
				return;
			}
			this.service.deleteStep(_item.IdRow).subscribe(res => {
				if (res && res.status == 1) {
					this.layoutUtilsService.showInfo(_deleteMessage);
					this.reload.emit(true);
				} else {
					this.layoutUtilsService.showError(res.error.message);
				}
			});
		});
	}
	//#endregion
	viewCheckers() {
		this.viewChecker.emit(this.selectedNode.data);
	}
}


