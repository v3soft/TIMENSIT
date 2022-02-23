alter table tbl_mausolieu
add IsMauTheoPhong bit default(0)

update tbl_mausolieu set IsMauTheoPhong=0

alter table tbl_mausolieu_Detail drop column [value]

alter table tbl_mausolieu 
add IdParent bigint null, nam int