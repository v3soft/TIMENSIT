use [Nguoicocong]
alter table tbl_mausolieu_detail
add [default] decimal(18, 0) null

alter table tbl_mausolieu_detail_child
add [default] decimal(18, 0) null


alter table tbl_nhapsolieu_detail
add Note nvarchar(500) null

alter table tbl_nhapsolieu_detail_child
add Note nvarchar(500) null