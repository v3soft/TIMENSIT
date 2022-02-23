USE [NGUOICOCONG]
GO

drop table [quytrinh_dieukien_capquanly]

/****** Object:  Table [dbo].[quytrinh_dieukien_capquanly]    Script Date: 3/11/2021 10:20:05 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[quytrinh_dieukien_capquanly](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Id_DieuKien] [bigint] NULL,
	[Id_QuyTrinh_CapQuanLy] [bigint] NULL,
	[SoNgay] [float] NULL,
 CONSTRAINT [PK_quytrinh_dieukien_capquanly] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[quytrinh_dieukien_capquanly] ON 
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (11, 3, 6, 1)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (12, 3, 7, 2)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (13, 3, 8, 0)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (14, 3, 9, 0)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (15, 3, 10, 2)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (21, 5, 6, 0)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (22, 5, 7, 0)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (23, 5, 8, 0)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (24, 5, 9, 0)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (25, 5, 10, 0)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (41, 6, 6, 0)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (42, 6, 7, 0)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (43, 6, 8, 0)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (44, 6, 9, 0)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (45, 6, 10, 0)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (51, 2, 6, 8)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (52, 2, 7, 2)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (53, 2, 8, 12)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (54, 2, 9, 1.5)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (55, 2, 10, 2)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (56, 4, 6, 8)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (57, 4, 7, 2)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (58, 4, 8, 6.5)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (59, 4, 9, 1)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (60, 4, 10, 2.5)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (61, 7, 6, 5.5)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (62, 7, 7, 2.5)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (63, 7, 8, 6.5)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (64, 7, 9, 1)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (65, 7, 10, 4.5)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (66, 8, 6, 8)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (67, 8, 7, 2)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (68, 8, 8, 7)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (69, 8, 9, 1.5)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (70, 8, 10, 2)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (71, 9, 6, 8)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (72, 9, 7, 2)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (73, 9, 8, 6.5)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (74, 9, 9, 1)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (75, 9, 10, 2.5)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (76, 10, 6, 8)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (77, 10, 7, 2)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (78, 10, 8, 7)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (79, 10, 9, 1.5)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (80, 10, 10, 2)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (81, 11, 6, 3)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (82, 11, 7, 2)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (83, 11, 8, 7)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (84, 11, 9, 1.5)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (85, 11, 10, 2)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (86, 12, 6, 3)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (87, 12, 7, 2)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (88, 12, 8, 7)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (89, 12, 9, 1.5)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (90, 12, 10, 2)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (91, 13, 6, 8)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (92, 13, 7, 2)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (93, 13, 8, 8)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (94, 13, 9, 1.5)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (95, 13, 10, 2)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (96, 14, 6, 8)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (97, 14, 7, 2)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (98, 14, 8, 7)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (99, 14, 9, 1.5)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (100, 14, 10, 2)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (101, 15, 6, 8)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (102, 15, 7, 2)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (103, 15, 8, 7)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (104, 15, 9, 1.5)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (105, 15, 10, 2)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (106, 16, 6, 3)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (107, 16, 7, 2)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (108, 16, 8, 7)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (109, 16, 9, 1.5)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (110, 16, 10, 2)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (111, 17, 6, 8)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (112, 17, 7, 2)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (113, 17, 8, 0)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (114, 17, 9, 0)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (115, 17, 10, 0)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (116, 18, 6, 3)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (117, 18, 7, 2)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (118, 18, 8, 3)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (119, 18, 9, 1)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (120, 18, 10, 1.5)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (121, 19, 6, 8)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (122, 19, 7, 1)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (123, 19, 8, 13)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (124, 19, 9, 1)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (125, 19, 10, 1.5)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (126, 20, 6, 0.5)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (127, 20, 7, 1)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (128, 20, 8, 1)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (129, 20, 9, 0.5)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (130, 20, 10, 1.5)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (131, 21, 6, 6)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (132, 21, 7, 1)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (133, 21, 8, 8)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (134, 21, 9, 1)
GO
INSERT [dbo].[quytrinh_dieukien_capquanly] ([Id], [Id_DieuKien], [Id_QuyTrinh_CapQuanLy], [SoNgay]) VALUES (135, 21, 10, 1.5)
GO
SET IDENTITY_INSERT [dbo].[quytrinh_dieukien_capquanly] OFF
GO
