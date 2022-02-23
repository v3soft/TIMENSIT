alter table Const_LoaiHoSo_GiayTo add IsRequired bit default(0)
update Const_LoaiHoSo_GiayTo set IsRequired=0
update Const_LoaiHoSo set Id_LoaiGiayTo=NULL,Id_LoaiGiayTo_CC=NULL

