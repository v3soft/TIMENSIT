import { BaseModel } from 'app/core/_base/crud';

export class CauHinhEmailModel extends BaseModel {
	Id: number

	DanhMuc: string

	MaDanhMuc: string

	DonVi: number

	Locked: boolean

	Priority: string

	clear() {
		this.Id = 0;

		this.DanhMuc = '';

		this.MaDanhMuc = '';

		this.DonVi = null;

		this.Locked = false;

		this.Priority = '';
	}
	copy(item: CauHinhEmailModel) {

		this.Id = item.Id;

		this.DanhMuc = item.DanhMuc;

		this.MaDanhMuc = item.MaDanhMuc;

		this.DonVi = item.DonVi;

		this.Locked = item.Locked;

		this.Priority = item.Priority;

	}
}
