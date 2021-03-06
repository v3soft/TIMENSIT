/*
   September 29, 20212:59:15 PM
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
ALTER TABLE dbo.Tbl_ThanNhan ADD
	SoBangTQCC nvarchar(200) NULL,
	SoGCNTB nvarchar(200) NULL,
	TLThuongTat float(53) NULL
GO
ALTER TABLE dbo.Tbl_ThanNhan SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
