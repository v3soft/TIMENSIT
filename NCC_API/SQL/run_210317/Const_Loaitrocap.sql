update Const_Loaitrocap set Id_LoaiHoSo=6 where Id_LoaiHoSo=3
update Const_LoaiTroCap set MaTroCap=N'MTP', TroCap=N'Mai táng phí', Id_LoaiHoSo=NULL, Disabled=0 where id=19
update Const_LoaiTroCap set MaTroCap=N'1L', TroCap=N'Trợ cấp một lần', Id_LoaiHoSo=NULL, Disabled=0 where id=20
update Const_LoaiTroCap set MaTroCap=N'Thang', TroCap=N'Trợ cấp hàng tháng', IsHangThang=1, Id_LoaiHoSo=NULL, Disabled=0 where id=23