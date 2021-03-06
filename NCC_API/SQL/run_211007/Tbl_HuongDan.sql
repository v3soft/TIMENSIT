USE [NGUOICOCONG]
GO
/****** Object:  Table [dbo].[Tbl_HuongDan]    Script Date: 10/7/21 9:47:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tbl_HuongDan](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TenHuongDan] [nvarchar](max) NOT NULL,
	[FileHuongDan] [nvarchar](max) NOT NULL,
	[NgayTao] [datetime] NULL,
	[NguoiTao] [bigint] NULL,
	[NgaySua] [datetime] NULL,
	[NguoiSua] [bigint] NULL,
	[Disabled] [bit] NULL,
 CONSTRAINT [PK_Tbl_HuongDan] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Tbl_HuongDan] ADD  CONSTRAINT [DF_Tbl_HuongDan_Disabled]  DEFAULT ((0)) FOR [Disabled]
GO
