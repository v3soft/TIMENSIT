import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef, Inject, ViewChild, ElementRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { startWith } from 'rxjs/operators';
import { DropEffect, DndDropEvent } from 'ngx-drag-drop';
import { OrgChartService } from '../Services/so-do-to-chuc.service';
import { ChartStaffModel } from '../Model/so-do-to-chuc.model';
import { MatDialog } from '@angular/material';
import { LayoutUtilsService } from '../../../../../../core/_base/crud';
import * as jspdf from 'jspdf';
import html2canvas from 'html2canvas';
@Component({
	selector: 'm-draw-chart-list',
	templateUrl: './draw-chart-list.component.html',
	styleUrls: ['./draw-chart-list.component.css'],
	changeDetection: ChangeDetectionStrategy.OnPush
})
export class DrawListComponent implements OnInit {
	// Table fields
	dataSourceChart: any[];
	ListItemChart: any[];
	//lưu lại giá trị drag
	index_drag: number;
	list_drag: any[];
	viewLoading: boolean = false;

	// row: number;
	constructor(
		private activatedRoute: ActivatedRoute,
		private translate: TranslateService,
		public dialog: MatDialog,
		private _orgChartService: OrgChartService,
		private changeDetectorRefs: ChangeDetectorRef,
		private layoutUtilsService: LayoutUtilsService
		// private dataRows: any
	) { }
	ID: string = '';
	_widthpage = 0;
	/** LOAD DATA */
	ngOnInit() {
		this.activatedRoute.params.subscribe(params => {
			//  
			this.ID = '' + params.ID;
		});
		this.index_drag = 0;
		this.list_drag = [];
		this.ListItemChart = [];
		this.getDatasourceChart();
	}

	/** FILTRATION */
	filterConfiguration(): any {
		const filter: any = {};
		return filter;
	}
	getTitle(): string {
		let result = '';
		result = this.translate.instant('SO_DO_TO_CHUC.vesodotochuc') + ` (${this.ID})`;
		return result;
	}
	/**CREATE BY DU LAM CHART**/
	dq(data) {
		var _w = 0;
		data.children.forEach(element => {
			var _cw = 1;
			if (element.level_jobtitle >= data.level_jobtitle) {
				element.level_jobtitle = data.level_jobtitle - 1;
			}
			element.diff = data.level_jobtitle - element.level_jobtitle;
			//cho bien i chạy khi đủ tổng chiều dài thì gán biến  last child	
			if (element.children && element.children.length)
				_cw = this.dq(element);//element.children
			else {
				element.width = 1;
				element.children.width = 1;
			}
			_w += _cw;
		});
		data.children.width = _w;
		data.width = _w;
		return _w;
	}
	XemLyLich(val: any) {

		//const dialogRef = this.dialog.open(LyLichNhanVienListComponent, {
		//	data: {
		//		idnv: val,
		//	},
		//	disableClose: false,
		//});
	}
	genArr(root, arr, level, startindex) {
		if (!arr[level]) {
			arr[level] = [];
		}
		var _offset = 0;
		var total_w = 0;
		var idx = 0;
		root.forEach(element => {
			var _cw = null;
			element.offset = startindex + _offset;
			if (idx == 0) {
				element.firstchild = true;
			}
			total_w += element.children.width;
			if (element.children && element.children.length) {
				_cw = this.genArr(element.children, arr, level + 1, startindex + _offset);
			}
			idx++;
			if (total_w == root.width) {
				element.lastchild = true;
			}
			if (element.children.width)
				_offset += element.children.width;
			else
				_offset += 1;
			arr[level].push(element);
		});
		return null;
	}
	addFakeCol(arr_chart) {
		for (var i = 0; i < arr_chart.length; i++) {
			var j = 0;//vị trí mảng chạy
			var index = 0;//vị trí hiện tại	
			if (!arr_chart[i]) arr_chart[i] = [];
			while (j < arr_chart[i].length) {
				if (arr_chart[i][j].offset > index) {
					var item_fake = {
						offset: index,
						children: {
							width: arr_chart[i][j].offset - index
						},
						width: arr_chart[i][j].offset - index,
						isfake: true
					};
					arr_chart[i].splice(j, 0, item_fake);
					j++;
					index = arr_chart[i][j].offset;
				}
				index += arr_chart[i][j].children.width;
				j++;
			}
		}
	}
	SortLevelTree(arr_chart) {
		;
		var level_max = arr_chart[0][0].level_jobtitle;
		var chart2 = [];
		for (var i = 0; i < arr_chart.length; i++) {
			for (var j = 0; j < arr_chart[i].length; j++) {
				var el = arr_chart[i][j];
				var elTopLevel = level_max - el.level_jobtitle;
				// var elLevel=level_max - el.item.level_jobtitle;
				if (!el.diff) el.diff = 1;
				for (var k = el.diff - 1; k >= 0; k--) {
					var elLevel = elTopLevel - k;
					if (!chart2[elLevel]) chart2[elLevel] = [];
					var offsetIndex = 0;
					while (offsetIndex < chart2[elLevel].length && el.offset > chart2[elLevel][offsetIndex].offset) {
						offsetIndex++;
					}
					if (k == 0) {
						if (el.diff > 1) {
							el.firstchild = true;
							el.lastchild = true;
						}
						chart2[elLevel].splice(offsetIndex, 0, el);
					}
					else {
						let fakeItem: any = {
							fake: true,
							Name: el.Name,
							children: el.children,
							width: el.width,
							offset: el.offset,
							firstchild: true,
							lastchild: true

						};
						if (k == el.diff - 1) {
							fakeItem.firstchild = el.firstchild;
							fakeItem.lastchild = el.lastchild;
						}

						chart2[elLevel].splice(offsetIndex, 0, fakeItem);
					}

				}
			}
		}
		return chart2;
	}
	goBack() {
		window.history.back();
	}
	getDatasourceChart() {
		this.viewLoading = true;
		// this.changeDetectorRefs.detectChanges();
		this._orgChartService.GetOrganizationalChartById(this.ID).subscribe(res => {
			this.viewLoading = false;
			// this.changeDetectorRefs.detectChanges();
			this.dataSourceChart = res.data;
			this.ListItemChart = [];
			var total_col = this.dq(this.dataSourceChart[0]);
			this.genArr(this.dataSourceChart, this.ListItemChart, 0, 0);
			this.ListItemChart = this.SortLevelTree(this.ListItemChart);
			this.addFakeCol(this.ListItemChart);
			this.changeDetectorRefs.detectChanges();
		});
	}
	onMoved_Staff(item: any, list: any[], parent: any, effect: DropEffect) {
		const index = list.indexOf(item);
		list.splice(index, 1);
	}
	onDrop_Staff(event: DndDropEvent, parent: any, list?: any[]) {
		if (list) {
			let index = event.index;
			if (typeof index === "undefined") {
				index = list.length;
			}
			// console.log("onDrop", list);
			//Gọi API
			let itemMove = new ChartStaffModel();
			itemMove.id_nv = event.data.ID_NV;
			itemMove.id_chucdanhmoi = parent.ID;
			this._orgChartService.handleDropStaff(itemMove).subscribe(res => {
				;
				if (res.status == 1) {
					list.splice(index, 0, event.data);
					this.changeDetectorRefs.detectChanges();
					return;
				}
				else {
					this.layoutUtilsService.showError(res.error.message);
					this.getDatasourceChart();
					return;
				}
			});
		}
	}

	@ViewChild('scrollOne', { static: true }) scrollOne: ElementRef;
	@ViewChild('scrollTwo', { static: true }) scrollTwo: ElementRef;
	GetwidthPage() {
		var width = document.getElementById('print-drawchart').offsetWidth;
		return width;
	}

	getTitlePrint() {
		if (this.GetwidthPage() < (window.innerWidth - 50)) {
			return 'In';
		}
		else {
			return 'Xuất PDF';
		}
	}

	PrintAndExport() {
		if (this.GetwidthPage() < (window.innerWidth - 50)) {
			this.printMePls();
		}
		else {
			this.convetToPDF();
		}
	}

	public convetToPDF() {
		window.scrollTo(0, 0);
		var convertA4 = false;
		this.viewLoading = true;
		var data = document.getElementById('print-drawchart');
		html2canvas(data).then(canvas => {
			// Few necessary setting options
			var imgWidth = 512;
			var pageHeight = 295;
			var imgHeight = canvas.height * imgWidth / canvas.width;
			var heightLeft = imgHeight;
			const contentDataURL = canvas.toDataURL('image/png')
			let pdf;
			pdf = new jspdf.jsPDF('l', 'mm', [imgHeight, imgWidth]);
			var x = 0;
			var y = 0;
			pdf.addImage(contentDataURL, 'PNG', x, y, imgWidth, imgHeight)
			pdf.save('so-do-to-chuc.pdf'); // Generated PDF
		});
		this.viewLoading = false;
	}

	@ViewChild('printme', { static: true }) printme: ElementRef;
	printMePls() {
		const printme = this.printme.nativeElement as HTMLElement;
		printme.click();
	}

	updateScroll(top = false) {
		const scrollOne = this.scrollOne.nativeElement as HTMLElement;
		const scrollTwo = this.scrollTwo.nativeElement as HTMLElement;

		// do logic and set
		if (!top) {
			scrollTwo.scrollLeft = scrollOne.scrollLeft;
		} else {
			scrollOne.scrollLeft = scrollTwo.scrollLeft;
		}
	}
}
