alter table Tbl_DiChuyenNCc
add IsBanChinh bit NOT NULL default(1)

update Tbl_DiChuyenNCc set IsBanChinh=1

alter table Tbl_DiChuyenNCC
add GhiChu nvarchar(1000)