use [Nguoicocong]

alter table dm_doituongncc
add Id_Template	bigint	null,
Id_Template_ThanNhan	bigint	null,
Id_Template_CongNhan	bigint	null


UPDATE
    dm_doituongncc 
SET
    dm_doituongncc.Id_Template = RAN.Id_Template,
    dm_doituongncc.Id_Template_ThanNhan = RAN.Id_Template_ThanNhan,
    dm_doituongncc.Id_Template_CongNhan = RAN.Id_Template_CongNhan
FROM
    dm_doituongncc SI
INNER JOIN
    Const_LoaiHoSo RAN
ON 
    SI.Id_LoaiHoSo = RAN.Id


alter table const_loaihoso
drop column Id_Template,Id_Template_ThanNhan,Id_Template_CongNhan


alter table tbl_ncc
add Id_LoaiHoSo	bigint null


UPDATE
    tbl_ncc 
SET
    tbl_ncc.Id_LoaiHoSo = RAN.Id_LoaiHoSo
FROM
    tbl_ncc SI
INNER JOIN
    dm_doituongncc RAN
ON 
    SI.Id_DoiTuongNCC = RAN.Id


alter table dm_doituongncc
drop column Id_LoaiHoSo

