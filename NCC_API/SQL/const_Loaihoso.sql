alter table const_Loaihoso
add Id_DoiTuongNCC bigint null


CREATE TABLE [dbo].[Const_LoaiHoSo_BieuMau](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Id_LoaiHoSo] [bigint] NOT NULL,
	[Id_BieuMau] [bigint] NOT NULL,
 CONSTRAINT [PK_Const_LoaiHoSo_BieuMau] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

