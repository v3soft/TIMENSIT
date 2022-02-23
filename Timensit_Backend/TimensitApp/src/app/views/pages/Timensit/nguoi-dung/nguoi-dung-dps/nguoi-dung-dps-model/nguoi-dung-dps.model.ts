import { BaseModel } from 'app/core/_base/crud';

export class NguoiDungDPSModel extends BaseModel {
	UserID: number
	Id: string
	UserName: string
	Password: string
	RePassword: string
	FullName: string
	Email: string
	PhoneNumber: string
	Status: number
	IdGroup: number
	Cap: number
	IDUserQL: number
	ViettelStudy: string;
	SimCA: string;
	LoaiChungThu: number;
	SerialToken: string;
	IdDonVi: number;
	IdChucVu: number;
	GioiTinh: number;
	MaNV: string;
	NhanLichDonVi: boolean;
	CMTND: string;
	NgaySinh: string;
	DonViQuanTam: any[];
	DonViLayHanXuLy: any[];
	lstDoiTuongNCC: any[];
	avatar:any;
	Avata:any;
	Sign:any;
	clear() {
		this.UserID = 0;
		this.Id = '';
		this.UserName = '';
		this.Password = '';
		this.RePassword = '';
		this.FullName = '';
		this.Email = '';
		this.PhoneNumber = '';
		this.Status = 1;
		this.IdGroup = 0;
		this.Cap = 0;
		this.IDUserQL = 0;
		this.ViettelStudy = '';
		this.SimCA = '';
		this.LoaiChungThu = 0;
		this.SerialToken = '';
		this.IdDonVi = 0;
		this.IdChucVu = 0;
		this.GioiTinh = 0;
		this.MaNV = '';
		this.NhanLichDonVi = false;
		this.CMTND = '';
		this.NgaySinh = '';
		this.DonViQuanTam = [];
		this.DonViLayHanXuLy = [];
		this.lstDoiTuongNCC = [];
		this.avatar={};
		this.Avata={};
		this.Sign={};
	}
	copy(item: NguoiDungDPSModel) {

		this.UserID = item.UserID;
		this.Id = item.Id;

		this.UserName = item.UserName;

		this.Password = item.Password;

		this.RePassword = item.RePassword;

		this.FullName = item.FullName;

		this.Email = item.Email;

		this.PhoneNumber = item.PhoneNumber;

		this.Status = item.Status;

		this.IdGroup = item.IdGroup;

		this.Cap = item.Cap;

		this.IDUserQL = item.IDUserQL;
		this.ViettelStudy = item.ViettelStudy;
		this.SimCA = item.SimCA;
		this.LoaiChungThu = item.LoaiChungThu;
		this.SerialToken = item.SerialToken;
		this.IdDonVi = item.IdDonVi;
		this.IdChucVu = item.IdChucVu;
		this.GioiTinh = item.GioiTinh;
		this.MaNV = item.MaNV;
		this.NhanLichDonVi = item.NhanLichDonVi;
		this.CMTND = item.CMTND;
		this.NgaySinh = item.NgaySinh;
		this.DonViQuanTam = item.DonViQuanTam;
		this.DonViLayHanXuLy = item.DonViLayHanXuLy;
		this.Avata = item.Avata;
		this.avatar = item.Avata;
		this.Sign = item.Sign;
	}
}
