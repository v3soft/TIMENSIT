/*
   September 12, 20212:29:19 PM
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
ALTER TABLE dbo.Tbl_NCC ADD
	IsTraXa bit NULL
GO
ALTER TABLE dbo.Tbl_NCC ADD CONSTRAINT
	DF_Tbl_NCC_IsTraXa DEFAULT ((0)) FOR IsTraXa
GO
ALTER TABLE dbo.Tbl_NCC SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
