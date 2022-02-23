// Angular
import { Component, OnInit, ChangeDetectorRef, ChangeDetectionStrategy, ViewEncapsulation, ViewChild, ElementRef, HostListener } from '@angular/core';
// Lodash
import { forEach, shuffle } from 'lodash';
// Services
// Widgets model
//import { LayoutConfigService, SparklineChartOptions } from '../../../core/_base/layout';
//import { Widget4Data } from '../../partials/content/widgets/widget4/widget4.component';
import { CommonService } from '../Timensit/services/common.service';
import { ChartType, ChartOptions } from 'chart.js';
import { SingleDataSet, Label } from 'ng2-charts';
import { BehaviorSubject, Observable } from 'rxjs';
import { Router } from '@angular/router';
import { SignalRService, ThongBaoModel } from '../Timensit/services/signalR.service';
import { QueryParamsModel } from '../../../core/_base/crud';
import 'chartjs-plugin-labels';
@Component({
	selector: 'kt-dashboard',
	templateUrl: './dashboard.component.html',
	styleUrls: ['dashboard.component.scss'],
	changeDetection: ChangeDetectionStrategy.OnPush,
	encapsulation: ViewEncapsulation.None

})
export class DashboardComponent implements OnInit {
	ThongKeDashboard: any[] = [];



	public pieChartOptions: ChartOptions = {
		responsive: true,
		plugins: {
			labels: {
				render: 'value',
				fontSize: 14,
				fontStyle: 'bold',
				fontColor: '#000',
				fontFamily: '"Lucida Console", Monaco, monospace'
			}
		}
	};
	public pieChartLabels: Label[] = [];
	public pieChartData: SingleDataSet = [300, 500, 100, 50];
	public pieChartType: ChartType = 'pie';
	public pieChartLegend = true;
	public pieChartPlugins = [];


	public pieChartOptions_1: ChartOptions = {
		responsive: true,
		plugins: {
			labels: {
				render: 'value',
				fontSize: 14,
				fontStyle: 'bold',
				fontColor: '#000',
				fontFamily: '"Lucida Console", Monaco, monospace'
			}
		}
	};
	public pieChartLabels_1: Label[] = [];
	public pieChartData_1: SingleDataSet = [300, 500, 100, 50];
	public pieChartType_1: ChartType = 'pie';
	public pieChartLegend_1 = true;
	public pieChartPlugins_1 = [];

	public pieChartOptions_2: ChartOptions = {
		responsive: true,
		plugins: {
			labels: {
				render: 'value',
				fontSize: 14,
				fontStyle: 'bold',
				fontColor: '#000',
				fontFamily: '"Lucida Console", Monaco, monospace'
			}
		}
	};
	public pieChartLabels_2: Label[] = [];
	public pieChartData_2: SingleDataSet = [300, 500, 100, 50];
	public pieChartType_2: ChartType = 'pie';
	public pieChartLegend_2 = true;
	public pieChartPlugins_2 = [];

	lastestFeedback = [];
	queryFB: QueryParamsModel = new QueryParamsModel({});
	pageTotalFB: number = 0;
	isStopScroll = false;
	scrollTop = false;
	@ViewChild('scrollView', { static: false }) scrollView: ElementRef;
	@HostListener('scroll', ['$event'])
	scrollViewHandler(event, item) {
		if (this.scrollView.nativeElement.scrollTop > 0) {
			this.scrollTop = true;
		}
		else {
			this.scrollTop = false;
		}
		if (this.isStopScroll) return;
		if (this.scrollView) {
			let total = this.scrollView.nativeElement.scrollHeight - this.scrollView.nativeElement.offsetHeight;
			try {
				if (this.scrollView.nativeElement.scrollTop + 5 >= total) {
					if (total > 0) {
						if (this.queryFB.pageNumber < (this.pageTotalFB / this.queryFB.pageSize)) {
							this.queryFB.pageNumber++;
							this.getListFeedBack(true);
						}
					}
				}
			} catch (err) {

			}
		}
	}
	constructor(
		private router: Router,
		//private layoutConfigService: LayoutConfigService,
		private commonService: CommonService,
		private signalRService: SignalRService,
		private changeDetectorRefs: ChangeDetectorRef) {
	}

	ngOnInit(): void {
		this.queryFB.sortOrder = "desc";
		this.queryFB.sortField = "CreatedDate";
		this.queryFB.pageNumber = 0;
		this.queryFB.pageSize = 10;
		this.commonService.ThongKeDasboard().subscribe(res => {
			// console.log("thong ke", res);
			if (res.status == 1) {
				this.ThongKeDashboard = res.data;
				this.changeDetectorRefs.detectChanges();
			}
		});

		this.commonService.BieuDoThongKeVanBan().subscribe(res => {

			if (res.status == 1) {
				this.pieChartLabels = ['Hoàn thành đúng hạn', 'Hoàn thành trễ hạn', 'Đang xử lý', 'Trễ hạn'];
				this.pieChartData = [res.data.VanBanDen.HTDungHan, res.data.VanBanDen.HTTreHan, res.data.VanBanDen.DangLam, res.data.VanBanDen.TreHan]
				this.pieChartLabels_1 = ['Hoàn thành đúng hạn', 'Hoàn thành trễ hạn', 'Đang xử lý', 'Trễ hạn'];
				this.pieChartData_1 = [res.data.VanBanDi.HTDungHan, res.data.VanBanDi.HTTreHan, res.data.VanBanDi.DangLam, res.data.VanBanDi.TreHan]
				this.pieChartLabels_2 = ['Hoàn thành đúng hạn', 'Hoàn thành trễ hạn', 'Đang xử lý', 'Trễ hạn'];
				this.pieChartData_2 = [res.data.SoLieu.HTDungHan, res.data.SoLieu.HTTreHan, res.data.SoLieu.DangLam, res.data.SoLieu.TreHan]
				this.changeDetectorRefs.detectChanges();
			}
		});

		this.getListFeedBack();

		this.signalRService.notifyReceived.subscribe(response => {
			this.getListFeedBackLastest();
			//response.forEach(res => {
			//	let index = this.lastestFeedback.findIndex(x => x.IdRow == res.IdRow);
			//	if (index >= 0) {
			//		if (res.Disabled || res.IsRead) {
			//			this.lastestFeedback.splice(index, 1);
			//			this.pageTotalFB--;
			//		}
			//	} else {
			//		if (res.IsNew) {
			//			this.lastestFeedback.unshift(res);
			//			this.pageTotalFB++;
			//		}
			//	}
			//});
			//this.changeDetectorRefs.detectChanges();
		})
	}

	getListFeedBack(more: boolean = false) {
		this.queryFB.filter.lastID = "";

		this.commonService.LastestFeedbackDasboard(this.queryFB).subscribe(res => {
			if (res.status == 1) {
				if (more) {
					this.lastestFeedback = this.lastestFeedback.concat(res.data);
				}
				else {
					this.lastestFeedback = res.data;
				}
				this.pageTotalFB = res.page.TotalCount;
				this.changeDetectorRefs.detectChanges();
			}
		})
	}

	getListFeedBackLastest() {
		this.queryFB.filter.lastID = this.lastestFeedback[0].IdRow;
		this.commonService.LastestFeedbackDasboard(this.queryFB).subscribe(res => {
			console.log("notify dashboard", res);
			if (res.status == 1) {
				if (res.data && res.data.length > 0) {
					res.data.forEach(element => {
						this.lastestFeedback.unshift(element);
					});
				}
				this.pageTotalFB = res.page.TotalCount;
				this.changeDetectorRefs.detectChanges();
			}
		})
	}

	onChangePage(event) {
		this.queryFB.pageNumber = event.pageIndex;
		this.getListFeedBack();
	}

	ReadFeedBack(item) {
		this.commonService.ReadNotify(item.IdRow).subscribe(res => {
			// console.log("res", res);
			this.router.navigate([item.Link, {}]);
		})
	}

	ScrollTop(e) {
		this.scrollView.nativeElement.scrollTo({ left: 0, top: 0, behavior: 'smooth' });
	}
}
