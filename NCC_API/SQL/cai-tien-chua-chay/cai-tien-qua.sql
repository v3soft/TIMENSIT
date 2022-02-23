declare @dot bigint=23
select dx.Id as Id_DeXuat, dxd.*, de.*, ncc.Id as Id_NCC,dot.Id_NhomLeTet, dt.DoiTuong, coalesce(dem.SoTien,0) as SoTien, dem.*, ncc.HoTen, ncc.SoHoSo, ncc.DiaChi,ncc.Id_KhomAp,dx.Id_Xa, ncc.Title, ncc.DistrictName,ncc.KhomAp, NguoiThoCungLietSy as ThanNhan, ncc.QHGiaDinh
from Tbl_DotTangQua dot 
join Tbl_DotTangQua_Detail de on dot.Id=de.Id_DotTangQua
join DM_DoiTuongNhanQua dt on de.Id_DoiTuongNCC=dt.Id
join Vw_ncc ncc on dt.Id=ncc.Id_DoiTuongNCC
join Tbl_DeXuatTangQua dx on dx.Id_DotTangQua=dot.Id
left join Vw_Muc dem on dem.Id_DotTangQua=@dot and dem.Id_DoiTuongNCC= de.Id_DoiTuongNCC
--left join Tbl_DeXuatTangQua_Detail dxd on dxd.Disabled=0 and IsGiam=0 and dxd.Id_NCC=ncc.Id and dxd.Id_NguonKinhPhi=dem.Id_NguonKinhPhi and dxd.Id_DeXuat=dx.Id and dxd.Id_Dot<=@dot
--left join Tbl_DeXuatTangQua_Detail g  on dxd.Id=g.Id_Detail_Giam and g.Id_Dot<=@dot and g.IsGiam=1 and g.Disabled=0
--left join DM_NguonKinhPhi ng on dem.Id_NguonKinhPhi=ng.Id
left join Vw_Detail dxd on dxd.Id_NCC=ncc.Id and dxd.Id_NguonKinhPhi=dem.Id_NguonKinhPhi and dxd.Id_DeXuat=dx.Id and dxd.Id_Dot_Tang<=@dot and (dxd.Id_Dot_Giam is null or dxd.Id_Dot_Giam <= @dot )
where dot.Disabled=0 and de.Disabled=0 and dt.Disabled=0 and dx.Disabled=0 and dot.Id<=@dot
and dxd.Id_DeXuat_Detail is not null 
and ((NgayChuyen is null or NgayChuyen<dx.CreatedDate) or Id_Xa_Old= ncc.Id_Xa ) and dxd.IdGiam is null and dem.SoTien is not null and dem.SoTien>0;

select de1.id as Id_DotTangQua_Detail, dx.Id as Id_DeXuat, dxd.Id as Id_DeXuat_Detail,dxd.IsTang, dxd.Id_Dot as Id_Dot_Tang, dxd.GhiChuGiam as GhiChuTang, de.*, ncc.Id as Id_NCC,dot.Id_NhomLeTet, dt.DoiTuong, coalesce(dem.SoTien,0) as SoTien, dem.*, ng.NguonKinhPhi, ncc.HoTen, ncc.SoHoSo, ncc.DiaChi,ncc.Id_KhomAp,dx.Id_Xa, xa.Title, h.DistrictName, ap.Title as KhomAp, NguoiThoCungLietSy as ThanNhan, qh.QHGiaDinh, g.Id as IdGiam , g.LyDo, g.GhiChuGiam
from Tbl_DotTangQua dot 
join Tbl_DotTangQua_Detail de on dot.Id=de.Id_DotTangQua
join DM_DoiTuongNhanQua dt on de.Id_DoiTuongNCC=dt.Id
join (select ncc1.*, chuyen.Id_Xa_Old, NgayChuyen from Tbl_DoiTuongNhanQua ncc1
left join Tbl_DoiTuongNhanQua_DiChuyen chuyen on chuyen.disabled=0 and chuyen.Id_NCC=ncc1.Id ) ncc on dt.Id=ncc.Id_DoiTuongNCC
join DM_Wards xa on xa.RowID=ncc.Id_Xa
join DM_KhomAp ap on ap.RowID=ncc.Id_KhomAp
join DM_District h on xa.DistrictID=h.Id_row
join Tbl_DeXuatTangQua dx on dx.Id_DotTangQua=dot.Id
left join Tbl_DotTangQua_Detail de1 on de1.Disabled=0 and de1.Id_DotTangQua=@dot and de1.Id_DoiTuongNCC= de.Id_DoiTuongNCC
left join Tbl_DotTangQua_Detail_Muc dem on dem.Id_Detail=de1.Id
left join Tbl_DeXuatTangQua_Detail dxd on dxd.Disabled=0 and IsGiam=0 and dxd.Id_NCC=ncc.Id and dxd.Id_NguonKinhPhi=dem.Id_NguonKinhPhi and dxd.Id_DeXuat=dx.Id and dxd.Id_Dot<=@dot
left join Tbl_DeXuatTangQua_Detail g  on dxd.Id=g.Id_Detail_Giam and g.Id_Dot<=@dot and g.IsGiam=1 and g.Disabled=0
left join DM_QHGiaDinh qh on qh.Id=Id_QHGiaDinh
left join DM_NguonKinhPhi ng on dem.Id_NguonKinhPhi=ng.Id
where dot.Disabled=0 and de.Disabled=0 and dt.Disabled=0 and ncc.Disabled=0 and dx.Disabled=0 and dot.Id<=@dot
and dxd.Id is not null
and ((ncc.Id_Xa=xa.RowID and (NgayChuyen is null or NgayChuyen<dx.CreatedDate)) or Id_Xa_Old= xa.RowID ) 
--18470: 1'03->44
