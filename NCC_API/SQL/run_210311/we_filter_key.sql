USE [NGUOICOCONG]
GO

INSERT INTO [dbo].[we_filter_key]
           ([id_row]
           ,[title]
           ,[table_name]
           ,[key]
           ,[value]
           ,[description]
           ,[sql]
           ,[loai]
           ,[Disabled])
     VALUES
           (13,	N'Loại hồ sơ','tbl_ncc',	'Id_LoaiHoSo',	'Id_LoaiHoSo',	NULL	,N'select Id, LoaiHoSo as title from Const_LoaiHoSo where Disabled=0',	1,	0)
GO

update quytrinh_dieukien set id_key=13