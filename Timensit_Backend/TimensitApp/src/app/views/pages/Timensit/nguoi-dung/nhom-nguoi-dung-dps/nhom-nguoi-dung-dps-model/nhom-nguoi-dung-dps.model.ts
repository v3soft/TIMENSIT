import { BaseModel } from 'app/core/_base/crud';

export class NhomNguoiDungDPSModel extends BaseModel {
	IdGroup: number
	GroupName: string
	Ma: string
	DisplayOrder: number
	Locked: boolean
	GhiChu: string
	DonVi: number
	ChucVu: number
	IsDefault: boolean

	clear() {
		this.IdGroup = 0;
		this.GroupName = '';
		this.Ma = '';
		this.GhiChu = '';
		this.DisplayOrder = 1;
		this.Locked = false;
		this.DonVi = 0;
		this.ChucVu = 0;
		this.IsDefault = false;
	}

	copy(item: NhomNguoiDungDPSModel) {
		this.IdGroup = item.IdGroup;
		this.GroupName = item.GroupName;
		this.Ma = item.Ma;
		this.GhiChu = item.GhiChu;
		this.DisplayOrder = item.DisplayOrder;
		this.Locked = item.Locked;
		this.DonVi = item.DonVi;
		this.ChucVu = item.ChucVu;
		this.IsDefault = item.IsDefault;

	}
}
