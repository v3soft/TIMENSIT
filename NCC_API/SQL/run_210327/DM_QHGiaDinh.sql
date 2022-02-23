alter table DM_QHGiaDinh
add ByQua bit default(0)
update DM_QHGiaDinh set ByQua=1

INSERT INTO [dbo].[DM_QHGiaDinh]
           ([QHGiaDinh]
           ,[Locked]
           ,[Priority]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[UpdatedBy]
           ,[UpdatedDate]
           ,[Disabled]
           ,[ByQua])
select [QHGiaDinh]
           ,[Locked]
           ,[Priority]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[UpdatedBy]
           ,[UpdatedDate]
           ,[Disabled]
           ,0 from DM_QHGiaDinh where Id in (select Id_QHGiaDinh from Tbl_ThanNhan)



--select * from DM_QHGiaDinh