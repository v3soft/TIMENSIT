use [Nguoicocong]
update Sys_ConfigGroup set ConfigGroup=N'Cấu hình password' where IdRow=	1
update Sys_ConfigGroup set ConfigGroup=N'Cấu hình upload file' where IdRow=	2
update Sys_ConfigGroup set ConfigGroup=N'Số hóa', Disabled=1 where IdRow=	3
update Sys_ConfigGroup set ConfigGroup=N'Đăng nhập/Đăng xuất' where IdRow=	5
update Sys_ConfigGroup set ConfigGroup=N'Ngày làm việc', Disabled=0 where IdRow=	10
update Sys_ConfigGroup set ConfigGroup=N'Lưu lịch sử', Disabled=1 where IdRow=	11
update Sys_ConfigGroup set ConfigGroup=N'Quy trình' where IdRow=	12
update Sys_ConfigGroup set ConfigGroup=N'Giao diện' where IdRow=	13
insert into Sys_ConfigGroup(ConfigGroup, Priority) values (N'Thông tin',7)

insert into Sys_Config(IdGroup, code, Value,Description, UpdatedBy, Type) values(14, 'INFO_SDT','02703.826306', N'SDT phòng Người có công',5, 'TEXT')
insert into Sys_Config(IdGroup, code, Value,Description, UpdatedBy, Type)  values (12, 'KHONG_NGAYNGHI','1', N'Tính thời hạn bỏ qua ngày nghỉ',5, 'BOOLEAN')
insert into Sys_Config(IdGroup, code, Value,Description, UpdatedBy, Type)  
values (	10,	'WEEK_START'	,'0'	,	N'Ngày bắt đầu tuần làm việc (0->6)'	,5,'NUMBER'),
		(	10	,'WEEK_END'	,'5'		,N'Ngày kết thúc tuần làm việc (0->6)',	5,'NUMBER')

update Sys_Config set Disabled=1 where IdRow in (46,47,49)