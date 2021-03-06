USE [NGUOICOCONG]
GO

/****** Object:  Table [dbo].[Const_LoaiHoSo_DoiTuong]    Script Date: 5/21/2021 10:18:44 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Const_LoaiHoSo_DoiTuong](
	[Id_LoaiHoSo] [bigint] NOT NULL,
	[Id_DoiTuong] [bigint] NOT NULL,
 CONSTRAINT [PK_Const_LoaiHoSo_DoiTuongNCC] PRIMARY KEY CLUSTERED 
(
	[Id_LoaiHoSo] ASC,
	[Id_DoiTuong] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


