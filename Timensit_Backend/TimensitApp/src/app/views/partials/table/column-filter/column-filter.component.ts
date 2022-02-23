import { Component, ChangeDetectionStrategy, Input, OnInit, ChangeDetectorRef } from '@angular/core';
import { TableService } from '../table.service';

@Component({
	selector: 'kt-column-filter',
	templateUrl: './column-filter.component.html',
	changeDetection: ChangeDetectionStrategy.OnPush
})
export class ColumnFilterComponent implements OnInit {
	@Input() enable_sort: boolean = false;
	@Input() column_name: string;
	@Input() column_title: string;
	@Input() gridService: TableService
	constructor(
		private changeDetect: ChangeDetectorRef) { }
	ngOnInit() {
		this.gridService.getOutput().subscribe((val: any) => {
			if (!this.changeDetect['destroyed']) {
				this.changeDetect.detectChanges();
			}
		});
	}
}
