USE [NGUOICOCONG]
GO

/****** Object:  Table [dbo].[Tbl_NCC_HuongDan]    Script Date: 3/12/2021 9:19:18 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Tbl_NCC_HuongDan](
	[id_quytrinh_lichsu] [bigint] NOT NULL,
	[Id_NCC] [bigint] NULL,
	[NoiDung] [nvarchar](2000) NULL,
	[MoTa] [ntext] NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [datetime] NULL,
	[Disabled] [bit] NOT NULL,
 CONSTRAINT [PK_Tbl_NCC_HuongDan] PRIMARY KEY CLUSTERED 
(
	[id_quytrinh_lichsu] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Tbl_NCC_HuongDan] ADD  CONSTRAINT [DF_Tbl_NCC_HuongDan_Disabled]  DEFAULT ((0)) FOR [Disabled]
GO

