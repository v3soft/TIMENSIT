declare @dot bigint=23
--18480 33s; 1625 8s
--1427 2s; 117 1s
select dot.Id as Id_DotTQ, dx.Id as Id_DX, dot.Nam, dxd.Id_DeXuat_Detail as Id_CTDX, dxd.Id_NCC,
ncc.Id_Xa, ncc.Title, ncc.DistrictID, ncc.DistrictName, ncc.ProvinceID, ncc.ProvinceName,
ncc.Id_DoiTuongNCC, dt.DoiTuong, ncc.HoTen, ncc.DiaChi, ncc.SoHoSo, ncc.NguoiThoCungLietSy, NgayChuyen, dx.CreatedDate,
SoTien as ChiTra ,dem.Id_NguonKinhPhi
from Tbl_DotTangQua dot 
join Tbl_DotTangQua_Detail de on dot.Id=de.Id_DotTangQua
join DM_DoiTuongNhanQua dt on de.Id_DoiTuongNCC=dt.Id
join Vw_ncc ncc on dt.Id=ncc.Id_DoiTuongNCC
join Tbl_DeXuatTangQua dx on  dx.Id_DotTangQua=dot.Id
left join Vw_Muc dem on dem.Id_DotTangQua=@dot and dem.Id_DoiTuongNCC= de.Id_DoiTuongNCC
left join Vw_Detail dxd on dxd.Id_Dot_Tang<=@dot and (dxd.Id_Dot_Giam is null or dxd.Id_Dot_Giam > @dot ) 
and SoTien is not null and SoTien>0
and dxd.Id_NCC=ncc.Id and dxd.Id_NguonKinhPhi=dem.Id_NguonKinhPhi and dxd.Id_DeXuat=dx.Id
where dot.Disabled=0 and de.Disabled=0 and dot.Id<=@dot and dxd.Id_DeXuat_Detail is not null and ProvinceID=61--and (DistrictID = 687)--
and dx.Disabled=0 and ((NgayChuyen is null or NgayChuyen<dx.CreatedDate) or Id_Xa_Old= ncc.Id_Xa )



declare @dot bigint=19--18480 47s; 1625 10s
--1427 2s; 117 1s
select dot.Id as Id_DotTQ, dx.Id as Id_DX, dot.Nam, dxd.Id as Id_CTDX, dxd.Id_NCC,
ncc.Id_Xa, xa.Title, xa.DistrictID, huyen.DistrictName, huyen.ProvinceID, tinh.ProvinceName,
ncc.Id_DoiTuongNCC, dt.DoiTuong, ncc.HoTen, ncc.DiaChi, ncc.SoHoSo, ncc.NguoiThoCungLietSy, NgayChuyen, dx.CreatedDate,
SoTien as ChiTra ,dem.Id_NguonKinhPhi
from Tbl_DotTangQua dot 
join Tbl_DotTangQua_Detail de on dot.Id=de.Id_DotTangQua
join DM_DoiTuongNhanQua dt on de.Id_DoiTuongNCC=dt.Id
join (select ncc1.*, chuyen.Id_Xa_Old, NgayChuyen from Tbl_DoiTuongNhanQua ncc1
left join Tbl_DoiTuongNhanQua_DiChuyen chuyen on chuyen.disabled=0 and chuyen.Id_NCC=ncc1.Id ) ncc on dt.Id=ncc.Id_DoiTuongNCC
join DM_Wards xa on xa.RowID=ncc.Id_Xa
join DM_District huyen on huyen.Id_row=xa.DistrictID
join DM_Provinces tinh on tinh.Id_row=huyen.ProvinceID 
join DM_KhomAp ap on ap.RowID=ncc.Id_KhomAp
join Tbl_DeXuatTangQua dx on dx.Id_DotTangQua=dot.Id
left join Tbl_DotTangQua_Detail de1 on de1.Disabled=0 and de1.Id_DotTangQua=@dot and de1.Id_DoiTuongNCC= de.Id_DoiTuongNCC
left join Tbl_DotTangQua_Detail_Muc dem on dem.Id_Detail=de1.Id
left join Tbl_DeXuatTangQua_Detail dxd on dxd.Disabled=0 and IsGiam=0 and dxd.Id_NCC=ncc.Id and dxd.Id_NguonKinhPhi=dem.Id_NguonKinhPhi and dxd.Id_DeXuat=dx.Id and dxd.Id_Dot<=@dot
left join Tbl_DeXuatTangQua_Detail g  on dxd.Id=g.Id_Detail_Giam and g.Id_Dot<=@dot and g.IsGiam=1 and g.Disabled=0
left join DM_QHGiaDinh qh on qh.Id=Id_QHGiaDinh
left join DM_NguonKinhPhi ng on dem.Id_NguonKinhPhi=ng.Id
where dot.Disabled=0 and de.Disabled=0 and dt.Disabled=0 and ncc.Disabled=0 and dx.Disabled=0 and dot.Id<=@dot
and g.Id is null and dxd.Id is not null and SoTien is not null and SoTien>0 
and ((ncc.Id_Xa=xa.RowID and (NgayChuyen is null or NgayChuyen<dx.CreatedDate)) or Id_Xa_Old= xa.RowID ) -- and (DistrictID = 687)
and ProvinceID=61
