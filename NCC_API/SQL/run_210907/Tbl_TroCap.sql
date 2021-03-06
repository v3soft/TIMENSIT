/*
   September 06, 20219:31:04 AM
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
ALTER TABLE dbo.Tbl_TroCap ADD
	ThangCat nvarchar(150) NULL,
	ThangDaNhan nvarchar(150) NULL
GO
DECLARE @v sql_variant 
SET @v = N'Trợ cấp đã nhận đến tháng'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'Tbl_TroCap', N'COLUMN', N'ThangDaNhan'
GO
ALTER TABLE dbo.Tbl_TroCap SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
