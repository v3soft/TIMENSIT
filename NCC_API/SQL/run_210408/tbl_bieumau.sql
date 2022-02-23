alter table tbl_bieumau 
add isTinh bit default(1)
alter table tbl_bieumau 
add isHuyen bit default(1)
alter table tbl_bieumau 
add isXa bit default(1)

update tbl_bieumau set istinh=1, ishuyen=1, isxa=1
