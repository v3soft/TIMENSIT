import { BaseModel } from "../../../../../../core/_base/crud";

export class chucvuModel extends BaseModel {
	Id_row: number;
	Id_CV: string;
	Tenchucdanh: string;
	Tentienganh: string;
	tenchucvu: string;


	clear() {
		this.Id_CV = '';
		this.Tenchucdanh = '';
		this.Tentienganh = '';
		this.tenchucvu = '';
		this.Id_row=0;
	}
}
