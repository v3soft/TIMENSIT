import { BaseModel } from '../../../../../core/_base/crud';

export class ChonNhieuNhanVienListModel extends BaseModel {
	ID_NV: number;
	HoTen: string;
	TenDonVi: string;
	TenPhongBan: string;
	TenChucDanh: string;
	NgaySinh: Date;
	Phai: string;
	MaNv: string;
	Structure: string;
	TenChucVu: string;
	Email: string;
	clear() {
	}
}
