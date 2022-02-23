import { BaseModel } from '../../../../../../core/_base/crud';

export class capquanlyModel extends BaseModel {
	RowID: number;
	Title: string;
	Summary: string;
	Range: string;
	clear() {
		this.RowID = 0;
		this.Title = '';
		this.Summary = '';
		this.Range = '';
	}
}
