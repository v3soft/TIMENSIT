import { BaseModel } from 'app/core/_base/crud';
import { environment } from 'environments/environment';
import { SelectionModel } from '@angular/cdk/collections';

export class TableModel extends BaseModel {
	pageSize: number[] = environment.pageSize;
	//#region filter on grid
	haveFilter: boolean = false;
	filterGroupDataChecked: any = {};
	filterGroupDataCheckedFake: any = {};
	filterGroupData: any = {};
	filterGroupArray: any = {};
	filterText: any = {};
	tmpfilterText: any = {};
	disableButtonFilter: any = {};
	isClearAll: boolean = false;
	
	//#endregion

	//#region drag drop
	previousIndex: number;
	availableColumns: any[];
	selectedColumns: SelectionModel<any>;
	displayedColumns: string[];
	//#endregion

	lstChip: any[];
	clear() {
		this.pageSize = environment.pageSize;
		this.haveFilter = false;
		this.filterGroupDataChecked = {};
		this.filterGroupDataCheckedFake = {};
		this.filterGroupData = {};
		this.filterGroupArray = {};
		this.filterText = {};
		this.tmpfilterText = {};
		this.disableButtonFilter = {};
		this.isClearAll = false;
		this.previousIndex = 0;
		this.availableColumns = [];
		this.selectedColumns = new SelectionModel<any>();
		this.displayedColumns = [];
		this.lstChip = [];
		
	}
	copy(item: TableModel) {
		this.pageSize = item.pageSize;
		this.haveFilter = item.haveFilter;
		this.filterGroupDataChecked = item.filterGroupDataChecked;
		this.filterGroupDataCheckedFake = item.filterGroupDataCheckedFake;
		this.filterGroupData = item.filterGroupData;
		this.filterGroupArray = item.filterGroupArray;
		this.filterText = item.filterText;
		this.tmpfilterText = item.tmpfilterText;
		this.disableButtonFilter = item.disableButtonFilter;
		this.isClearAll = item.isClearAll;
		this.previousIndex = item.previousIndex;
		this.availableColumns = item.availableColumns;
		this.selectedColumns = item.selectedColumns;
		this.displayedColumns = item.displayedColumns;
		this.lstChip = item.lstChip;
	}
}
