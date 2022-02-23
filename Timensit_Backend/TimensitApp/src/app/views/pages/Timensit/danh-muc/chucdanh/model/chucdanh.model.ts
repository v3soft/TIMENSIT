import { BaseModel } from '../../../../../../core/_base/crud';

export class ChucDanhModel extends BaseModel {
	Id_CV: number;
	MaCV: string;
	TenCV: string;
	Cap: string;
	IsManager: boolean;
	IsTaiXe: boolean;
	clear() {
		this.Id_CV = 0;
		this.MaCV = '';
		this.TenCV = '';
		this.Cap = '';
		this.IsManager = false;
		this.IsTaiXe = false;
	}
}
