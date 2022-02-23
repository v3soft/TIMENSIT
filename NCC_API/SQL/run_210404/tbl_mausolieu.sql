USE [NGUOICOCONG]
GO

INSERT INTO [dbo].[Tbl_MauSoLieu]
           ([MauSoLieu]
           ,[MoTa]
           ,[Locked]
           ,[Priority]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[UpdatedBy]
           ,[UpdatedDate]
           ,[Disabled]
           ,[IsMauTheoPhong]
           ,[IdParent]
           ,[nam])
     
           select N'Quản lý số liệu hàng năm'
           ,N'Ba mẫu kết hợp'
           ,[Locked]
           ,[Priority]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[UpdatedBy]
           ,[UpdatedDate]
           ,[Disabled]
           ,[IsMauTheoPhong]
           ,[IdParent]
           ,[nam] from Tbl_MauSoLieu where Id=2
GO

INSERT INTO [dbo].[Tbl_MauSoLieu_Detail]
           ([Id_MauSoLieu]
           ,[Id_SoLieu]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[UpdatedBy]
           ,[UpdatedDate]
           ,[Disabled]
           ,[_default])
     select
           24
           ,[Id_SoLieu]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[UpdatedBy]
           ,[UpdatedDate]
           ,[Disabled]
           ,[_default] from tbl_mausolieu_detail where Id_MauSoLieu in (2,3,4) and disabled=0
GO

INSERT INTO [dbo].[Tbl_MauSoLieu_Detail_Child]
           ([Id_Detail]
           ,[Id_PhiSoLieu]
           ,[CachNhap]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[UpdatedBy]
           ,[UpdatedDate]
           ,[Disabled]
           ,[_default])
     select temp.Id
           ,ch.[Id_PhiSoLieu]
           ,ch.[CachNhap]
           ,ch.[CreatedBy]
           ,ch.[CreatedDate]
           ,ch.[UpdatedBy]
           ,ch.[UpdatedDate]
           ,ch.[Disabled]
           ,ch.[_default]
from Tbl_MauSoLieu_Detail_Child ch
join Tbl_MauSoLieu_Detail de on ch.Id_Detail=de.Id 
left join (select dm.SoLieu, de.Id_SoLieu,de.Id
from Tbl_MauSoLieu_Detail de
join dm_solieu dm on de.Id_SoLieu=dm.Id
where de.Disabled=0 and de.Id_MauSoLieu =24) temp on temp.Id_SoLieu=de.Id_SoLieu
where ch.Disabled=0 and de.Disabled=0 and de.Id_MauSoLieu in (2,3,4)
GO

