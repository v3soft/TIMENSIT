USE [NGUOICOCONG]
GO
/****** Object:  Table [dbo].[Tbl_BieuMau_ThanhPhan]    Script Date: 4/8/2021 11:50:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tbl_BieuMau_ThanhPhan](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ThanhPhan] [nvarchar](200) NULL,
	[NoiDung] [nvarchar](max) NULL,
	[DieuKien] [nvarchar](500) NULL,
	[NoiDungFail] [nvarchar](max) NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
	[Disabled] [bit] NOT NULL,
 CONSTRAINT [PK_Tbl_BieuMau_ThanhPhan] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Tbl_BieuMau_ThanhPhan] ON 
GO
INSERT [dbo].[Tbl_BieuMau_ThanhPhan] ([Id], [ThanhPhan], [NoiDung], [DieuKien], [NoiDungFail], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Disabled]) VALUES (2, N'Thân nhân chủ yếu nhận trợ cấp', N'<p>Sinh năm:&nbsp;:NamSinh1:&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Nam/Nữ: :GioiTinh1:</p>
									<p>Nguyên quán: :NguyenQuan1:.</p>
									<p>Trú quán: :DiaChi1:</p>
									<p>Quan hệ với người có công với cách mạng từ trần:&nbsp;
										<span style="font-weight: 700;">:QHGiaDinh:</span>
										<br></p>
									<p>Mức trợ cấp: (Nhan(:TienTroCap(20):;3) đồng).&nbsp;</p><p>Bằng chữ: :Chu(Nhan(:TienTroCap(20):;3)))</p>', N'IsChuYeu=1', N'không giải quyết do không có thân nhân chủ yếu', 15, CAST(N'2021-04-06T00:00:00.000' AS DateTime), NULL, NULL, 0)
GO
INSERT [dbo].[Tbl_BieuMau_ThanhPhan] ([Id], [ThanhPhan], [NoiDung], [DieuKien], [NoiDungFail], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Disabled]) VALUES (3, N'Trợ cấp có truy lĩnh', N'<table style="height: 64px; width: 621px;">
											<tbody>
												<tr style="height: 26px;">
													<td rowspan="2" style="width: 24px; height: 39px; text-align: center; border-top: 1px solid; border-bottom: 1px solid; border-left: 1px solid; border-image: initial; border-right: none;">TT</td>
													<td rowspan="2" style="width: 125px; height: 39px; text-align: center; border-top: 1px solid; border-bottom: 1px solid; border-left: 1px solid; border-image: initial; border-right: none;">Họ tên</td>
													<td rowspan="2" style="width: 67px; height: 39px; text-align: center; border-top: 1px solid; border-bottom: 1px solid; border-left: 1px solid; border-image: initial; border-right: none;">Năm sinh</td>
													<td rowspan="2" style="width: 58px; height: 39px; text-align: center; border-top: 1px solid; border-bottom: 1px solid; border-left: 1px solid; border-image: initial; border-right: none;">Địa chỉ</td>
													<td rowspan="2" style="width: 71px; height: 39px; text-align: center; border-top: 1px solid; border-bottom: 1px solid; border-left: 1px solid; border-image: initial; border-right: none;">Mối quan hệ NCC</td>
													<td colspan="3" style="width: 153px; height: 26px; text-align: center; border-top: 1px solid; border-bottom: 1px solid; border-left: 1px solid; border-image: initial; border-right: none;">Mức Trợ Cấp</td>
													<td rowspan="2" style="width: 77px; height: 26px; text-align: center; border: 1px solid;">Thời điểm hưởng</td>
												</tr>
												<tr style="height: 13px;">
													<td style="width: 53px; height: 13px; text-align: center; border-right: none; border-bottom: 1px solid; border-image: initial; border-top: none; border-left: 1px solid;">Tiền tuất</td>
													<td style="width: 43px; height: 13px; text-align: center; border-right: none; border-bottom: 1px solid; border-image: initial; border-top: none; border-left: 1px solid;">Tuất ND</td>
													<td style="width: 45px; height: 13px; text-align: center; border-bottom: 1px solid; border-image: initial; border-top: none; border-left: 1px solid; border-right: none;">Tổng cộng</td>
												</tr>
												<tr style="height: 13px;">
													<td style="width: 24px; height: 13px; text-align: center; border-bottom: 1px solid; border-left: 1px solid; border-image: initial; border-right: none;">1</td>
													<td style="width: 125px; height: 13px; border-bottom: 1px solid; border-left: 1px solid; border-image: initial; border-right: none;">:NguoiThoCungLietSy:</td>
													<td style="width: 67px; height: 13px; border-bottom: 1px solid; border-left: 1px solid; border-image: initial; border-top: none; border-right: none;">:NamSinh1:</td>
													<td style="width: 58px; height: 13px; border-bottom: 1px solid; border-left: 1px solid; border-image: initial; border-top: none; border-right: none;">:DiaChi1:</td>
													<td style="width: 71px; height: 13px; border-bottom: 1px solid; border-left: 1px solid; border-image: initial; border-top: none; border-right: none;">:QHGiaDinh:</td>
													<td style="width: 53px; height: 13px; border-bottom: 1px solid; border-left: 1px solid; border-image: initial; border-top: none; border-right: none;">&nbsp;</td>
													<td style="width: 43px; height: 13px; border-bottom: 1px solid; border-left: 1px solid; border-image: initial; border-top: none; border-right: none;">&nbsp;</td>
													<td style="width: 45px; height: 13px; border-bottom: 1px solid; border-left: 1px solid; border-image: initial; border-top: none; border-right: none;">&nbsp;<em>:TienTroCap_Chu(23):</em>
													</td>
													<td style="width: 77px; height: 13px; border-right: 1px solid; border-bottom: 1px solid; border-left: 1px solid; border-image: initial; border-top: none;">:TruyLinh_From(23):</td>
												</tr>
											</tbody>
										</table>
										<p style="padding-left: 30px;">
											<em>
												<span style="font-weight: 700;">Trợ cấp truy lĩnh tháng :TruyLinh_To(23):: :TienTroCap(23): đồng x :SoThangTruyLinh(23): tháng = :TongTruyLinh(23): đồng<br>(Bằng chữ: :TongTruyLinh_Chu(23):)<br>
														</span>
													</em>
												</p>', N'IsTruyLinh=0', N' ', 15, CAST(N'2021-04-06T00:00:00.000' AS DateTime), NULL, NULL, 0)
GO
INSERT [dbo].[Tbl_BieuMau_ThanhPhan] ([Id], [ThanhPhan], [NoiDung], [DieuKien], [NoiDungFail], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Disabled]) VALUES (5, N'Trợ cấp sau khi từ trần', N'<p>Các chế độ trợ cấp sau khi từ trần bao gồm:</p>
								<p>‑ Trợ cấp một lần:</p>
								<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; + Mai táng phí: = :TienTroCap(20): đồng.</p>
								<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; + :SoThangTroCap(23): tháng trợ cấp, phụ cấp: ( :TienTroCap(23): X :SoThangTroCap(23):)  = Nhan(:TienTroCap(23):;:SoThangTroCap(23):) đồng.</p>
								<p><strong>Tổng cộng: = Cong(:TienTroCap(20):;Nhan(:TienTroCap(23):;:SoThangTroCap(23):)) đồng.</strong></p>', N'IsChuYeu=1', N'<p>Các chế độ trợ cấp sau khi từ trần bao gồm:</p>
<p>‑ Trợ cấp một lần:</p>
<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; + Mai táng phí: = :TienTroCap(20): đồng.</p>
<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; + Không còn thân nhân chủ yếu</p>
<p><strong>Tổng cộng: = :TienTroCap(20): đồng.</strong></p>', 15, CAST(N'2021-04-06T00:00:00.000' AS DateTime), NULL, NULL, 0)
GO
SET IDENTITY_INSERT [dbo].[Tbl_BieuMau_ThanhPhan] OFF
GO
ALTER TABLE [dbo].[Tbl_BieuMau_ThanhPhan] ADD  CONSTRAINT [DF_Tbl_BieuMau_ThanhPhan_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Tbl_BieuMau_ThanhPhan] ADD  CONSTRAINT [DF_Tbl_BieuMau_ThanhPhan_Disabled]  DEFAULT ((0)) FOR [Disabled]
GO
