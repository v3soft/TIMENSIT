alter table Tbl_trocap
add ThangThuHoi nvarchar(200)

alter table Tbl_trocap
add TuNam int

alter table Tbl_trocap
add NamCat int

ALTER TABLE Tbl_trocap ALTER COLUMN TuNgay datetime NULL;

update Tbl_TroCap set tunam= year(tungay)
update Tbl_TroCap set namcat= year(NgayCat) where iscat=1

alter table Tbl_trocap
add LyDoKhongGiaiQuyet nvarchar(2000)

alter table Tbl_trocap
add LyDoKhongMaiTangPhi nvarchar(2000)

--đình chỉ
alter table Tbl_trocap
add NgayDinhChi datetime,
	LyDoDinhChi nvarchar(2000),
	LyDoTamDC nvarchar(2000) NULL,
	ThuHoiDCTu int NULL,
	ThuHoiDCDen int NULL,
	TuThang nvarchar(150) NULL,
	TiLeTroCap float NULL