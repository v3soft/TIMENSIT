import { BaseModel } from "../../../../../../core/_base/crud";

export class OrgChartModel extends BaseModel {
	ID_PhongBan: string;
	ID_ChucDanh: string;
	ViTri: number;
	ID: string;
	drop_idfrom: string;
	drop_id_parent: string;
	drop_namefrom: string;
	drop_idto: string;
	drop_levelto: string;
	drop_nameto: string;
	drop_childrensto: string;
	StructureID: string;
	Id_parent: string;
	chucdanhParent: string;
	IsAbove: boolean;
	clear() {
		this.ID_PhongBan = '0';
		this.ID_ChucDanh = '0';
		this.ViTri = 0;
		this.StructureID = '0';
		this.Id_parent = '0';
		this.chucdanhParent = '';
		this.IsAbove = false;
	}
}
export class UpdateThongTinChucVuModel extends BaseModel {
	MaCD: string;
	SoNhanVien: string;
	ViTri: string;
	ID_ChucVu: string;
	ID_ChucDanh: string;
	TenChucVu: string;
	TenTiengAnh: string;
	ID_DonVi: string;
	ID_PhongBan: string;
	ID_Cap: number;
	HienThiDonVi: boolean;
	DungChuyenCap: boolean;
	HienThiCap: boolean;
	HienThiPhongBan: boolean;
	ID: number;
	ID_Parent: number;
	StructureID: string = '';
	HienThiID: boolean;
	clear() {
		this.MaCD = '';
		this.SoNhanVien = '';
		this.ViTri = '';
		this.ID_ChucVu = '';
		this.ID_ChucDanh = '';
		this.TenChucVu = '';
		this.TenTiengAnh = '';
		this.ID_DonVi = '';
		this.ID_PhongBan = '';
		this.ID_Cap = 0;
		this.HienThiDonVi = false;
		this.DungChuyenCap = false;
		this.HienThiCap = false
		this.HienThiPhongBan = false;
		this.ID = 0;
		this.ID_Parent = 0;
		this.StructureID = '';
		this.HienThiID = false;
	}
}
export class ChartStaffModel {
	id_nv: string;
	id_chucdanhmoi: string;
	constructor() { }
}
