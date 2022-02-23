alter table Tbl_ThanNhan
add NamSinh int

alter table Tbl_ThanNhan
add IsChet bit default(0)

update Tbl_ThanNhan set IsChet=0

alter table Tbl_ThanNhan
add NgayChet datetime

alter table Tbl_ThanNhan
add SoKhaiTu nvarchar(100)