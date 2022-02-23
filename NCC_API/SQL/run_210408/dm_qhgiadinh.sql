alter table dm_qhgiadinh 
add IsChuYeu bit default(0)

update dm_qhgiadinh set IsChuYeu=0 where ByQua=0