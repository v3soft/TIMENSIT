--update col float -> nvarchar(100)
alter table tbl_ncc
alter column TiLe nvarchar(100)

--add col 
ALTER TABLE dbo.Tbl_NCC ADD
	LyDoGTYKhoa nvarchar(1000) NULL,
	LyDoTamDC nvarchar(1000) NULL,
	LyDoDinhChi nvarchar(1000) NULL,
	TGThamGiaKC int NULL,
	BangKhenCacCap nvarchar(1000) NULL,
	NgayHS datetime NULL,
	NoiHS nvarchar(1000) NULL,
	NamHienTai int NULL,
	NgayHop datetime NULL,
	GioHop nvarchar(200) NULL,
	ThanhPhanHop nvarchar(2000) NULL,
	NoiDungHop nvarchar(2000) NULL,
	CanCuLietSy nvarchar(MAX) NULL,
	LyDoTangTuat nvarchar(2000) NULL,
	LyDoDinhChinhTN nvarchar(2000) NULL