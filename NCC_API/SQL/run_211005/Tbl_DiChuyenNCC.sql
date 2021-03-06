/*
   October 03, 202111:15:15 AM
   User: 
   Server: DESKTOP-RKRHI4E\SQLEXPRESS
   Database: NGUOICOCONG
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Tbl_DiChuyenNCC ADD
	Id_KhomAp bigint NULL,
	Id_KhomAp_Old bigint NULL
GO
DECLARE @v sql_variant 
SET @v = N'Từ nơi khác chuyển đến cần thêm khóm ấp nơi chuyển đến (lấy từ tab HS)'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tbl_DiChuyenNCC', N'COLUMN', N'Id_KhomAp'
GO
ALTER TABLE dbo.Tbl_DiChuyenNCC SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
