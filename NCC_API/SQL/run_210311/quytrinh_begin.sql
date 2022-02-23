USE [NGUOICOCONG]
GO

/****** Object:  Table [dbo].[quytrinh_begin]    Script Date: 3/6/2021 12:39:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[quytrinh_begin](
	[id_row] [bigint] IDENTITY(1,1) NOT NULL,
	[loai] [smallint] NOT NULL,
	[id_phieu] [bigint] NOT NULL,
	[nguoi_tao] [bigint] NOT NULL,
	[ngay_tao] [datetime] NULL,
	[deadline] [datetime] NULL,
	[nguoi_nhan] [varchar](500) NULL,
 CONSTRAINT [PK_quytrinh_begin] PRIMARY KEY CLUSTERED 
(
	[id_row] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


--insert into quytrinh_begin ( id_phieu, nguoi_tao, ngay_tao, loai)
--select IdDoiTuong as id_phieu, CreatedBy as nguoi_tao, CreatedDate as ngay_Tao, case IdLoaiLog when 8 then 2 when 14 then 1 when 16 then 3 end as loai from Tbl_Log 
--where IdLoaiLog in (8,14,16) and NoiDung =N'Gửi duyệt'

insert into quytrinh_begin ( id_phieu, nguoi_tao, ngay_tao, loai)
select id as id_phieu, SentBy as nguoi_tao, SentDate as ngay_Tao, 2 from Tbl_NCC 
where status <> 0 and sentby is not null

insert into quytrinh_begin ( id_phieu, nguoi_tao, ngay_tao, loai)
select id as id_phieu, SentBy as nguoi_tao, SentDate as ngay_Tao, 1 from Tbl_DeXuatTangQua 
where status <> 0 and sentby is not null

insert into quytrinh_begin ( id_phieu, nguoi_tao, ngay_tao, loai)
select id as id_phieu, SentBy as nguoi_tao, SentDate as ngay_Tao, 3 from Tbl_NhapSoLieu 
where status <> 0 and sentby is not null

UPDATE
    quytrinh_begin 
SET
    quytrinh_begin.nguoi_nhan = RAN.checkers
FROM
    quytrinh_begin SI
inner join  quytrinh_quatrinhduyet RAN ON SI.id_phieu=RAN.id_phieu AND si.loai=RAN.loai AND RAN.priority=1
where SI.nguoi_nhan is null;

UPDATE
    quytrinh_begin 
SET
    quytrinh_begin.nguoi_nhan = RAN.checker
FROM
    quytrinh_begin SI
inner join  quytrinh_quatrinhduyet RAN ON SI.id_phieu=RAN.id_phieu AND si.loai=RAN.loai AND RAN.priority=1
where SI.nguoi_nhan is null;