export class QueryResultsModel {
	// fields
	status: number;
	data: any[];
	dataExtra: any[];
	items: any[];
	page: any;
	totalCount: number;
	errorMessage: string;
	error: any;
	Visible: boolean=true;

	constructor(_items: any[] = [], _totalCount: number = 0, _errorMessage: string = '') {
		this.items = this.data = _items;
		this.totalCount = _totalCount;
	}
}

