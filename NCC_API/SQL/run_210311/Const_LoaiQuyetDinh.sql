USE [NGUOICOCONG]
GO

drop table Const_LoaiQuyetDinh

/****** Object:  Table [dbo].[Const_LoaiQuyetDinh]    Script Date: 3/10/2021 9:57:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Const_LoaiQuyetDinh](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[LoaiQuyetDinh] [nvarchar](200) NOT NULL,
	[MoTa] [nvarchar](500) NULL,
	[Disabled] [bit] NOT NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_DM_LoaiHoSo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Const_LoaiQuyetDinh] ON 
GO
INSERT [dbo].[Const_LoaiQuyetDinh] ([Id], [LoaiQuyetDinh], [MoTa], [Disabled], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, N'Bệnh Binh', NULL, 0, 5, CAST(N'2021-03-10T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Const_LoaiQuyetDinh] ([Id], [LoaiQuyetDinh], [MoTa], [Disabled], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (2, N'BMVNAH', NULL, 0, 5, CAST(N'2021-03-10T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Const_LoaiQuyetDinh] ([Id], [LoaiQuyetDinh], [MoTa], [Disabled], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (3, N'CĐHH', NULL, 0, 5, CAST(N'2021-03-10T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Const_LoaiQuyetDinh] ([Id], [LoaiQuyetDinh], [MoTa], [Disabled], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (4, N'Liệt Sĩ', NULL, 0, 5, CAST(N'2021-03-10T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Const_LoaiQuyetDinh] ([Id], [LoaiQuyetDinh], [MoTa], [Disabled], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (5, N'Người có công', NULL, 0, 5, CAST(N'2021-03-10T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Const_LoaiQuyetDinh] ([Id], [LoaiQuyetDinh], [MoTa], [Disabled], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (6, N'QĐ 49', NULL, 0, 5, CAST(N'2021-03-10T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Const_LoaiQuyetDinh] ([Id], [LoaiQuyetDinh], [MoTa], [Disabled], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (7, N'QĐ 62', NULL, 0, 5, CAST(N'2021-03-10T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Const_LoaiQuyetDinh] ([Id], [LoaiQuyetDinh], [MoTa], [Disabled], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (8, N'QĐ 142', NULL, 0, 5, CAST(N'2021-03-10T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Const_LoaiQuyetDinh] ([Id], [LoaiQuyetDinh], [MoTa], [Disabled], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (9, N'QĐ 290', NULL, 0, 5, CAST(N'2021-03-10T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Const_LoaiQuyetDinh] ([Id], [LoaiQuyetDinh], [MoTa], [Disabled], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (10, N'quyết định dưới xã', NULL, 0, 5, CAST(N'2021-03-10T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Const_LoaiQuyetDinh] ([Id], [LoaiQuyetDinh], [MoTa], [Disabled], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (11, N'Thương binh', NULL, 0, 5, CAST(N'2021-03-10T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Const_LoaiQuyetDinh] ([Id], [LoaiQuyetDinh], [MoTa], [Disabled], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (12, N'TKN', NULL, 0, 5, CAST(N'2021-03-10T00:00:00.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Const_LoaiQuyetDinh] ([Id], [LoaiQuyetDinh], [MoTa], [Disabled], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (13, N'Tù Dày', NULL, 0, 5, CAST(N'2021-03-10T00:00:00.000' AS DateTime), NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Const_LoaiQuyetDinh] OFF
GO
ALTER TABLE [dbo].[Const_LoaiQuyetDinh] ADD  CONSTRAINT [DF_Const_LoaiQuyetDinh_Disabled]  DEFAULT ((0)) FOR [Disabled]
GO
ALTER TABLE [dbo].[Const_LoaiQuyetDinh] ADD  CONSTRAINT [DF_Const_LoaiQuyetDinh_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
