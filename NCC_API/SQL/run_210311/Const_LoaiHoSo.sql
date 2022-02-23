USE [NGUOICOCONG]
GO
drop table [Const_LoaiHoSo]
/****** Object:  Table [dbo].[Const_LoaiHoSo]    Script Date: 3/11/2021 9:53:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Const_LoaiHoSo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MaLoaiHoSo] [nvarchar](100) NULL,
	[LoaiHoSo] [nvarchar](1000) NOT NULL,
	[MoTa] [nvarchar](500) NULL,
	[Disabled] [bit] NOT NULL,
	[Id_LoaiGiayTo] [bigint] NULL,
	[Id_LoaiGiayTo_CC] [bigint] NULL,
	[MauCongNhan] [varchar](10) NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Const_LoaiHoSo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[quytrinh_dieukien]    Script Date: 3/11/2021 9:53:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
drop table [quytrinh_dieukien]
CREATE TABLE [dbo].[quytrinh_dieukien](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DieuKien] [nvarchar](500) NULL,
	[Id_QuyTrinh] [bigint] NULL,
	[id_key] [int] NOT NULL,
	[value] [nvarchar](500) NULL,
	[operator] [varchar](5) NULL,
	[disabled] [bit] NOT NULL,
	[createddate] [datetime] NULL,
	[createdby] [bigint] NULL,
	[lastmodfied] [datetime] NULL,
	[modifiedby] [bigint] NULL,
 CONSTRAINT [PK_quytrinh_dieukien] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Const_LoaiHoSo] ON 
GO
INSERT [dbo].[Const_LoaiHoSo] ([Id], [MaLoaiHoSo], [LoaiHoSo], [MoTa], [Disabled], [Id_LoaiGiayTo], [Id_LoaiGiayTo_CC], [MauCongNhan], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, N'TTHC01', N'Giải quyết chế độ mai táng phí đối với cựu chiến binh', N'', 0, 9, NULL, NULL, 5, CAST(N'2020-10-20T00:00:00.000' AS DateTime), 5, CAST(N'2020-12-23T14:41:12.563' AS DateTime))
GO
INSERT [dbo].[Const_LoaiHoSo] ([Id], [MaLoaiHoSo], [LoaiHoSo], [MoTa], [Disabled], [Id_LoaiGiayTo], [Id_LoaiGiayTo_CC], [MauCongNhan], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (2, N'TTHC02', N'Giải quyết chế độ mai táng phí đối với thanh niên xung phong thời kỳ chống Pháp ', N'', 0, 6, 10, N'TB3', 5, CAST(N'2020-10-20T00:00:00.000' AS DateTime), 5, CAST(N'2020-12-31T13:33:54.537' AS DateTime))
GO
INSERT [dbo].[Const_LoaiHoSo] ([Id], [MaLoaiHoSo], [LoaiHoSo], [MoTa], [Disabled], [Id_LoaiGiayTo], [Id_LoaiGiayTo_CC], [MauCongNhan], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (3, N'TTHC03', N'Thủ tục giải quyết chế độ trợ cấp một lần đối với người được cử làm chuyên gia sang giúp Lào, Cam - pu - chia.', NULL, 0, NULL, 9, N'LS5', 5, CAST(N'2020-10-20T00:00:00.000' AS DateTime), 5, CAST(N'2020-12-16T20:56:58.287' AS DateTime))
GO
INSERT [dbo].[Const_LoaiHoSo] ([Id], [MaLoaiHoSo], [LoaiHoSo], [MoTa], [Disabled], [Id_LoaiGiayTo], [Id_LoaiGiayTo_CC], [MauCongNhan], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (4, N'TTHC04', N'Thủ tục giải quyết chế độ chế độ trợ cấp một lần đối với thân nhân người hoạt động kháng chiến được tặng huân chương, huy chương chết trước ngày 01 tháng 01 năm 1995 mà chưa được hưởng chế độ ưu đãi dân tộc, bảo vệ Tổ quốc và làm nghĩa vụ quốc tế.', NULL, 0, NULL, NULL, N'BB3', 5, CAST(N'2020-10-20T00:00:00.000' AS DateTime), 5, CAST(N'2020-12-16T20:57:41.600' AS DateTime))
GO
INSERT [dbo].[Const_LoaiHoSo] ([Id], [MaLoaiHoSo], [LoaiHoSo], [MoTa], [Disabled], [Id_LoaiGiayTo], [Id_LoaiGiayTo_CC], [MauCongNhan], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (5, N'TTHC05', N'Thủ tục hưởng mai táng phí, trợ cấp một lần khi người có công với cách mạng từ trần', N'', 0, NULL, NULL, N'LT3', 5, CAST(N'2020-10-20T00:00:00.000' AS DateTime), 5, CAST(N'2020-10-20T18:09:36.997' AS DateTime))
GO
INSERT [dbo].[Const_LoaiHoSo] ([Id], [MaLoaiHoSo], [LoaiHoSo], [MoTa], [Disabled], [Id_LoaiGiayTo], [Id_LoaiGiayTo_CC], [MauCongNhan], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (6, N'TTHC06', N'Thủ tục giải quyết trợ cấp tiền tuất hàng tháng cho thân nhân khi người có công từ trần', N'', 0, NULL, NULL, NULL, 5, CAST(N'2020-10-20T00:00:00.000' AS DateTime), 5, CAST(N'2021-02-22T15:28:36.543' AS DateTime))
GO
INSERT [dbo].[Const_LoaiHoSo] ([Id], [MaLoaiHoSo], [LoaiHoSo], [MoTa], [Disabled], [Id_LoaiGiayTo], [Id_LoaiGiayTo_CC], [MauCongNhan], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (7, N'TTHC07', N'Thủ tục giải quyết chế độ đối với thân nhân liệt sĩ', N'', 0, NULL, NULL, NULL, 5, CAST(N'2020-10-20T00:00:00.000' AS DateTime), 5, CAST(N'2020-12-16T20:58:32.370' AS DateTime))
GO
INSERT [dbo].[Const_LoaiHoSo] ([Id], [MaLoaiHoSo], [LoaiHoSo], [MoTa], [Disabled], [Id_LoaiGiayTo], [Id_LoaiGiayTo_CC], [MauCongNhan], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (8, N'TTHC08', N'Thủ tục giải quyết chế độ đối với Anh hùng lực lượng vũ trang nhân dân, Anh hùng lao động trong thời kỳ kháng chiến', N'', 0, 2, NULL, NULL, 5, CAST(N'2020-10-20T00:00:00.000' AS DateTime), 5, CAST(N'2021-02-23T13:50:36.220' AS DateTime))
GO
INSERT [dbo].[Const_LoaiHoSo] ([Id], [MaLoaiHoSo], [LoaiHoSo], [MoTa], [Disabled], [Id_LoaiGiayTo], [Id_LoaiGiayTo_CC], [MauCongNhan], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (9, N'TTHC09', N'Thủ tục giải quyết hưởng chế độ ưu đãi đối với người hoạt động kháng chiến bị nhiễm chất độc hóa học', NULL, 0, NULL, NULL, NULL, 5, CAST(N'2020-10-20T00:00:00.000' AS DateTime), 5, CAST(N'2020-12-16T20:59:07.087' AS DateTime))
GO
INSERT [dbo].[Const_LoaiHoSo] ([Id], [MaLoaiHoSo], [LoaiHoSo], [MoTa], [Disabled], [Id_LoaiGiayTo], [Id_LoaiGiayTo_CC], [MauCongNhan], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (10, N'TTHC10', N'Thủ tục giải quyết hưởng chế độ ưu đãi đối với con đẻ của người hoạt động kháng chiến bị nhiễm chất độc hóa học

', NULL, 0, 7, NULL, NULL, 5, CAST(N'2020-10-20T00:00:00.000' AS DateTime), 5, CAST(N'2020-12-16T20:59:18.950' AS DateTime))
GO
INSERT [dbo].[Const_LoaiHoSo] ([Id], [MaLoaiHoSo], [LoaiHoSo], [MoTa], [Disabled], [Id_LoaiGiayTo], [Id_LoaiGiayTo_CC], [MauCongNhan], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (11, N'TTHC11', N'Thủ tục giải quyết chế độ người hoạt động cách mạng hoặc hoạt động kháng chiến bị địch bắt tù, đày', NULL, 0, 8, NULL, NULL, 5, CAST(N'2020-10-20T00:00:00.000' AS DateTime), 5, CAST(N'2020-12-16T21:00:05.280' AS DateTime))
GO
INSERT [dbo].[Const_LoaiHoSo] ([Id], [MaLoaiHoSo], [LoaiHoSo], [MoTa], [Disabled], [Id_LoaiGiayTo], [Id_LoaiGiayTo_CC], [MauCongNhan], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (12, N'TTHC12', N'Thủ tục giải quyết chế độ người hoạt động kháng chiến giải phóng dân tộc, bảo vệ tổ quốc và làm nghĩa vụ quốc tế', NULL, 0, NULL, NULL, N'TKN3', 5, CAST(N'2020-10-20T00:00:00.000' AS DateTime), 5, CAST(N'2020-12-16T21:00:20.867' AS DateTime))
GO
INSERT [dbo].[Const_LoaiHoSo] ([Id], [MaLoaiHoSo], [LoaiHoSo], [MoTa], [Disabled], [Id_LoaiGiayTo], [Id_LoaiGiayTo_CC], [MauCongNhan], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (13, N'TTHC13', N'Giải quyết chế độ người có công giúp đỡ cách mạng', N'', 0, NULL, NULL, NULL, 5, CAST(N'2020-12-23T17:34:19.173' AS DateTime), 5, CAST(N'2020-12-23T17:50:30.500' AS DateTime))
GO
INSERT [dbo].[Const_LoaiHoSo] ([Id], [MaLoaiHoSo], [LoaiHoSo], [MoTa], [Disabled], [Id_LoaiGiayTo], [Id_LoaiGiayTo_CC], [MauCongNhan], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (14, N'TTHC14', N'Giải quyết chế độ trợ cấp thờ cúng liệt sĩ', N'', 0, NULL, NULL, NULL, 5, CAST(N'2021-03-10T11:52:11.420' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Const_LoaiHoSo] ([Id], [MaLoaiHoSo], [LoaiHoSo], [MoTa], [Disabled], [Id_LoaiGiayTo], [Id_LoaiGiayTo_CC], [MauCongNhan], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (15, N'TTHC15', N'Giải quyết chế độ ưu đãi đối với Bà mẹ Việt Nam anh hùng', N'', 0, NULL, NULL, N'LT3', 5, CAST(N'2020-10-20T00:00:00.000' AS DateTime), 5, CAST(N'2020-10-20T18:09:36.997' AS DateTime))
GO
INSERT [dbo].[Const_LoaiHoSo] ([Id], [MaLoaiHoSo], [LoaiHoSo], [MoTa], [Disabled], [Id_LoaiGiayTo], [Id_LoaiGiayTo_CC], [MauCongNhan], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (16, N'TTHC16', N'Xác nhận thương binh, người hưởng chính sách như thương binh đối với người bị thương không thuộc lực lượng công an, quân đội trong chiến tranh từ ngày 31/12/1991 trở về trước không còn giấy tờ', N'', 0, NULL, NULL, NULL, 5, CAST(N'2020-10-20T00:00:00.000' AS DateTime), 5, CAST(N'2021-02-22T15:28:36.543' AS DateTime))
GO
INSERT [dbo].[Const_LoaiHoSo] ([Id], [MaLoaiHoSo], [LoaiHoSo], [MoTa], [Disabled], [Id_LoaiGiayTo], [Id_LoaiGiayTo_CC], [MauCongNhan], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (17, N'TTHC17', N'Bổ sung tình hình thân nhân trong hồ sơ liệt sĩ', N'', 0, NULL, NULL, NULL, 5, CAST(N'2020-10-20T00:00:00.000' AS DateTime), 5, CAST(N'2020-12-16T20:58:32.370' AS DateTime))
GO
INSERT [dbo].[Const_LoaiHoSo] ([Id], [MaLoaiHoSo], [LoaiHoSo], [MoTa], [Disabled], [Id_LoaiGiayTo], [Id_LoaiGiayTo_CC], [MauCongNhan], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (18, N'TTHC18', N'Trợ cấp một lần đối với thanh niên xung phong đã hoàn thành nhiệm vụ trong kháng chiến', N'', 0, 2, NULL, NULL, 5, CAST(N'2020-10-20T00:00:00.000' AS DateTime), 5, CAST(N'2021-02-23T13:50:36.220' AS DateTime))
GO
INSERT [dbo].[Const_LoaiHoSo] ([Id], [MaLoaiHoSo], [LoaiHoSo], [MoTa], [Disabled], [Id_LoaiGiayTo], [Id_LoaiGiayTo_CC], [MauCongNhan], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (19, N'TTHC19', N'Trợ cấp hàng tháng đối với thanh niên xung phong đã hoàn thành nhiệm vụ trong kháng chiến', NULL, 0, NULL, NULL, NULL, 5, CAST(N'2020-10-20T00:00:00.000' AS DateTime), 5, CAST(N'2020-12-16T20:59:07.087' AS DateTime))
GO
INSERT [dbo].[Const_LoaiHoSo] ([Id], [MaLoaiHoSo], [LoaiHoSo], [MoTa], [Disabled], [Id_LoaiGiayTo], [Id_LoaiGiayTo_CC], [MauCongNhan], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (22, N'TTHC20', N'Thực hiện chế độ ưu đãi trong giáo dục đào tạo đối với người có công với cách mạng và con của họ', NULL, 0, 8, NULL, NULL, 5, CAST(N'2020-10-20T00:00:00.000' AS DateTime), 5, CAST(N'2020-12-16T21:00:05.280' AS DateTime))
GO
INSERT [dbo].[Const_LoaiHoSo] ([Id], [MaLoaiHoSo], [LoaiHoSo], [MoTa], [Disabled], [Id_LoaiGiayTo], [Id_LoaiGiayTo_CC], [MauCongNhan], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (23, N'TTHC21', N'Lập Sổ theo dõi và cấp phương tiện trợ giúp, dụng cụ chỉnh hình ', NULL, 0, NULL, NULL, N'TKN3', 5, CAST(N'2020-10-20T00:00:00.000' AS DateTime), 5, CAST(N'2020-12-16T21:00:20.867' AS DateTime))
GO
INSERT [dbo].[Const_LoaiHoSo] ([Id], [MaLoaiHoSo], [LoaiHoSo], [MoTa], [Disabled], [Id_LoaiGiayTo], [Id_LoaiGiayTo_CC], [MauCongNhan], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (24, N'TTHC22', N'Hỗ trợ, di chuyển hài cốt liệt sĩ', N'', 0, NULL, NULL, NULL, 5, CAST(N'2020-12-23T17:34:19.173' AS DateTime), 5, CAST(N'2020-12-23T17:50:30.500' AS DateTime))
GO
INSERT [dbo].[Const_LoaiHoSo] ([Id], [MaLoaiHoSo], [LoaiHoSo], [MoTa], [Disabled], [Id_LoaiGiayTo], [Id_LoaiGiayTo_CC], [MauCongNhan], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (25, N'TTHC23', N'Giải quyết trợ cấp một lần đối với người có thành tích tham gia kháng chiến đã được tặng Bằng khen của Thủ tướng Chính phủ, Bằng khen của Chủ tịch Hội đồng Bộ trưởng hoặc Bằng khen của Bộ trưởng, Thủ trưởng cơ quan ngang bộ, Thủ trưởng cơ quan thuộc Chính phủ, Bằng khen của Chủ tịch Ủy ban nhân dân tỉnh, thành phố trực thuộc Trung ương', N'', 0, NULL, NULL, NULL, 5, CAST(N'2020-12-23T17:34:19.173' AS DateTime), 5, CAST(N'2020-12-23T17:50:30.500' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[Const_LoaiHoSo] OFF
GO
SET IDENTITY_INSERT [dbo].[quytrinh_dieukien] ON 
GO
INSERT [dbo].[quytrinh_dieukien] ([Id], [DieuKien], [Id_QuyTrinh], [id_key], [value], [operator], [disabled], [createddate], [createdby], [lastmodfied], [modifiedby]) VALUES (1, N'Người hoạt động cách mạng trước ngày 01/01/1945', 2, 13, N'1', N'=', 1, CAST(N'2021-02-20T00:00:00.000' AS DateTime), 1, NULL, NULL)
GO
INSERT [dbo].[quytrinh_dieukien] ([Id], [DieuKien], [Id_QuyTrinh], [id_key], [value], [operator], [disabled], [createddate], [createdby], [lastmodfied], [modifiedby]) VALUES (2, N'Giải quyết chế độ trợ cấp thờ cúng liệt sĩ', 2, 13, N'14', N'=', 0, CAST(N'2021-02-20T00:00:00.000' AS DateTime), 1, CAST(N'2021-03-11T20:37:55.353' AS DateTime), 5)
GO
INSERT [dbo].[quytrinh_dieukien] ([Id], [DieuKien], [Id_QuyTrinh], [id_key], [value], [operator], [disabled], [createddate], [createdby], [lastmodfied], [modifiedby]) VALUES (3, N'test', 2, 13, N'8', N'=', 1, CAST(N'2021-02-23T16:27:15.483' AS DateTime), 5, CAST(N'2021-02-23T16:54:42.453' AS DateTime), 5)
GO
INSERT [dbo].[quytrinh_dieukien] ([Id], [DieuKien], [Id_QuyTrinh], [id_key], [value], [operator], [disabled], [createddate], [createdby], [lastmodfied], [modifiedby]) VALUES (4, N'Giải quyết chế độ mai táng phí đối với cựu chiến binh', 2, 13, N'1', N'=', 0, CAST(N'2021-02-23T16:55:34.973' AS DateTime), 5, CAST(N'2021-03-11T20:39:25.810' AS DateTime), 5)
GO
INSERT [dbo].[quytrinh_dieukien] ([Id], [DieuKien], [Id_QuyTrinh], [id_key], [value], [operator], [disabled], [createddate], [createdby], [lastmodfied], [modifiedby]) VALUES (5, N'test', 2, 13, N'10', N'=', 1, CAST(N'2021-02-23T16:57:28.697' AS DateTime), 5, CAST(N'2021-02-23T17:00:54.840' AS DateTime), 5)
GO
INSERT [dbo].[quytrinh_dieukien] ([Id], [DieuKien], [Id_QuyTrinh], [id_key], [value], [operator], [disabled], [createddate], [createdby], [lastmodfied], [modifiedby]) VALUES (6, N'hhh', 2, 13, N'2', N'=', 1, CAST(N'2021-03-11T20:18:55.657' AS DateTime), 5, CAST(N'2021-03-11T20:21:26.827' AS DateTime), 5)
GO
INSERT [dbo].[quytrinh_dieukien] ([Id], [DieuKien], [Id_QuyTrinh], [id_key], [value], [operator], [disabled], [createddate], [createdby], [lastmodfied], [modifiedby]) VALUES (7, N'giải quyết chế độ trợ cấp một lần đối với người được cử làm chuyên gia sang giúp Lào, Cam - pu – chia', 2, 13, N'3', N'=', 0, CAST(N'2021-03-11T20:23:25.523' AS DateTime), 5, CAST(N'2021-03-11T20:42:28.777' AS DateTime), 5)
GO
INSERT [dbo].[quytrinh_dieukien] ([Id], [DieuKien], [Id_QuyTrinh], [id_key], [value], [operator], [disabled], [createddate], [createdby], [lastmodfied], [modifiedby]) VALUES (8, N'Giải quyết chế độ trợ cấp một lần đối với thân nhân người hoạt động kháng chiến được tặng huân chương, huy chương chết trước ngày 01 tháng 01 năm 1995 mà chưa được hưởng chế độ ưu đãi', 2, 13, N'4', N'=', 0, CAST(N'2021-03-11T20:43:17.183' AS DateTime), 5, NULL, NULL)
GO
INSERT [dbo].[quytrinh_dieukien] ([Id], [DieuKien], [Id_QuyTrinh], [id_key], [value], [operator], [disabled], [createddate], [createdby], [lastmodfied], [modifiedby]) VALUES (9, N'Thủ tục hưởng mai táng phí, trợ cấp một lần khi người có công với cách mạng từ trần', 2, 13, N'5', N'=', 0, CAST(N'2021-03-11T20:44:29.583' AS DateTime), 5, NULL, NULL)
GO
INSERT [dbo].[quytrinh_dieukien] ([Id], [DieuKien], [Id_QuyTrinh], [id_key], [value], [operator], [disabled], [createddate], [createdby], [lastmodfied], [modifiedby]) VALUES (10, N'Giải quyết trợ cấp tiền tuất hàng tháng cho thân nhân khi người có công từ trần ', 2, 13, N'6', N'=', 0, CAST(N'2021-03-11T20:45:12.750' AS DateTime), 5, NULL, NULL)
GO
INSERT [dbo].[quytrinh_dieukien] ([Id], [DieuKien], [Id_QuyTrinh], [id_key], [value], [operator], [disabled], [createddate], [createdby], [lastmodfied], [modifiedby]) VALUES (11, N'Thủ tục giải quyết chế độ đối ưu đãi với thân nhân liệt sĩ', 2, 13, N'7', N'=', 0, CAST(N'2021-03-11T20:46:05.013' AS DateTime), 5, NULL, NULL)
GO
INSERT [dbo].[quytrinh_dieukien] ([Id], [DieuKien], [Id_QuyTrinh], [id_key], [value], [operator], [disabled], [createddate], [createdby], [lastmodfied], [modifiedby]) VALUES (12, N'Thủ tục giải quyết chế độ đối với Anh hùng lực lượng vũ trang nhân dân, Anh hùng lao động trong thời kỳ kháng chiến', 2, 13, N'8', N'=', 0, CAST(N'2021-03-11T20:46:38.940' AS DateTime), 5, NULL, NULL)
GO
INSERT [dbo].[quytrinh_dieukien] ([Id], [DieuKien], [Id_QuyTrinh], [id_key], [value], [operator], [disabled], [createddate], [createdby], [lastmodfied], [modifiedby]) VALUES (13, N'Giải quyết chế độ người hoạt động cách mạng hoặc hoạt động kháng chiến bị địch bắt tù, đày ', 2, 13, N'11', N'=', 0, CAST(N'2021-03-11T20:51:06.433' AS DateTime), 5, NULL, NULL)
GO
INSERT [dbo].[quytrinh_dieukien] ([Id], [DieuKien], [Id_QuyTrinh], [id_key], [value], [operator], [disabled], [createddate], [createdby], [lastmodfied], [modifiedby]) VALUES (14, N'Giải quyết chế độ người hoạt động kháng chiến giải phóng dân tộc, bảo vệ Tổ quốc và làm nghĩa vụ quốc tế', 2, 13, N'12', N'=', 0, CAST(N'2021-03-11T20:51:38.233' AS DateTime), 5, NULL, NULL)
GO
INSERT [dbo].[quytrinh_dieukien] ([Id], [DieuKien], [Id_QuyTrinh], [id_key], [value], [operator], [disabled], [createddate], [createdby], [lastmodfied], [modifiedby]) VALUES (15, N'Thủ tục giải quyết chế độ người có công giúp đỡ cách mạng', 2, 13, N'13', N'=', 0, CAST(N'2021-03-11T20:52:53.057' AS DateTime), 5, NULL, NULL)
GO
INSERT [dbo].[quytrinh_dieukien] ([Id], [DieuKien], [Id_QuyTrinh], [id_key], [value], [operator], [disabled], [createddate], [createdby], [lastmodfied], [modifiedby]) VALUES (16, N'Thủ tục giải quyết chế độ ưu đãi đối với Bà mẹ Việt Nam anh hùng', 2, 13, N'15', N'=', 0, CAST(N'2021-03-11T20:56:20.190' AS DateTime), 5, NULL, NULL)
GO
INSERT [dbo].[quytrinh_dieukien] ([Id], [DieuKien], [Id_QuyTrinh], [id_key], [value], [operator], [disabled], [createddate], [createdby], [lastmodfied], [modifiedby]) VALUES (17, N'Thủ tục bổ sung tình hình thân nhân trong hồ sơ liệt sĩ', 2, 13, N'17', N'=', 0, CAST(N'2021-03-11T20:57:46.523' AS DateTime), 5, NULL, NULL)
GO
INSERT [dbo].[quytrinh_dieukien] ([Id], [DieuKien], [Id_QuyTrinh], [id_key], [value], [operator], [disabled], [createddate], [createdby], [lastmodfied], [modifiedby]) VALUES (18, N'Thủ tục thực hiện chế độ ưu đãi trong giáo dục đào tạo đối với người có công với cách mạng và con của họ', 2, 13, N'22', N'=', 0, CAST(N'2021-03-11T20:59:13.167' AS DateTime), 5, NULL, NULL)
GO
INSERT [dbo].[quytrinh_dieukien] ([Id], [DieuKien], [Id_QuyTrinh], [id_key], [value], [operator], [disabled], [createddate], [createdby], [lastmodfied], [modifiedby]) VALUES (19, N'Lập Sổ theo dõi và cấp phương tiện trợ giúp, dụng cụ chỉnh hình', 2, 13, N'23', N'=', 0, CAST(N'2021-03-11T20:59:51.703' AS DateTime), 5, NULL, NULL)
GO
INSERT [dbo].[quytrinh_dieukien] ([Id], [DieuKien], [Id_QuyTrinh], [id_key], [value], [operator], [disabled], [createddate], [createdby], [lastmodfied], [modifiedby]) VALUES (20, N'Hỗ trợ, di chuyển hài cốt liệt sĩ', 2, 13, N'24', N'=', 0, CAST(N'2021-03-11T21:00:53.650' AS DateTime), 5, NULL, NULL)
GO
INSERT [dbo].[quytrinh_dieukien] ([Id], [DieuKien], [Id_QuyTrinh], [id_key], [value], [operator], [disabled], [createddate], [createdby], [lastmodfied], [modifiedby]) VALUES (21, N'Giải quyết trợ cấp một lần đối với người có thành tích tham gia kháng chiến đã được tặng Bằng khen của Thủ tướng Chính phủ, Bằng khen của Chủ tịch Hội đồng Bộ trưởng hoặc Bằng khen của Bộ trưởng, Thủ trưởng cơ quan ngang bộ, Thủ trưởng cơ quan thuộc Chính phủ, Bằng khen của Chủ tịch Ủy ban nhân dân tỉnh, thành phố trực thuộc Trung ương', 2, 13, N'25', N'=', 0, CAST(N'2021-03-11T21:02:14.920' AS DateTime), 5, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[quytrinh_dieukien] OFF
GO
ALTER TABLE [dbo].[Const_LoaiHoSo] ADD  CONSTRAINT [DF_Const_LoaiHoSo_Disabled]  DEFAULT ((0)) FOR [Disabled]
GO
ALTER TABLE [dbo].[Const_LoaiHoSo] ADD  CONSTRAINT [DF_Const_LoaiHoSo_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[quytrinh_dieukien] ADD  CONSTRAINT [DF_quytrinh_dieukien_operator]  DEFAULT ('=') FOR [operator]
GO
ALTER TABLE [dbo].[quytrinh_dieukien] ADD  CONSTRAINT [DF_quytrinh_dieukien_disabled]  DEFAULT ((0)) FOR [disabled]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Const_LoaiHoSo', @level2type=N'COLUMN',@level2name=N'Id_LoaiGiayTo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Const_LoaiHoSo', @level2type=N'COLUMN',@level2name=N'Id_LoaiGiayTo_CC'
GO
