using DpsLibs.Data;
using JeeOfficeAPI.Controllers;
using JeeOfficeAPI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace JeeOfficeAPI.Classes
{
    public class Process
    {
        public string Error;
        public DpsConnection cnn;
        public long Id;
        public long IdLS;
        public Process(DpsConnection _cnn, long id, long idls)
        {
            cnn = _cnn;
            Id = id;
            IdLS = idls;
        }
        /// <summary>
        /// Tạo tiến trình và xác định checkers đầu tiên<para/>
        /// </summary>
        /// <returns></returns>
        public bool Begin()
        {
            string check = "select * from DP_Process where IdDoiTuong=19 and Id=" + Id;
            DataTable dtCheck = cnn.CreateDataTable(check);
            if (dtCheck.Rows.Count > 0)
            {
                Error = "Quy trình cho phản ánh đã được thiết lập";
                return false;
            }
            string sql = "select * from DP_Action where Disabled=0";
            DataTable dt = cnn.CreateDataTable(sql);
            if (dt == null || dt.Rows.Count == 0)
            {
                Error = "Không tìm thấy quy trình";
                return false;
            }
            Hashtable val;
            foreach (DataRow dr in dt.Rows)
            {
                val = new Hashtable();
                val["Id"] = Id;
                val["IdStep"] = dr["IdStep"];
                val["Next"] = dr["Next"];
                val["ButtonText"] = dr["ButtonText"];
                val["IsComeBackPro"] = dr["IsComeBack"];
                val["IdDoiTuong"] = 19;
                cnn.Insert(val, "DP_Process");
            }
            string lstUser = "";
            string str1 = @"SELECT distinct pro.IdRow, pro.IdStep, pro.Next, pro.IsComeBackPro from DP_Process pro
join DP_Step step on pro.IdStep = step.IdRow and step.disabled=0 where step.Loai=0 and Id=" + Id;
            DataTable dtProcess = cnn.CreateDataTable(str1);
            foreach (DataRow dr in dtProcess.Rows)
            {
                //Tim Checkers dau tien
                List<string> lst = GetCheckers(dr["IdStep"].ToString(), dr["Next"].ToString(), Convert.ToBoolean(dr["IsComeBackPro"]));
                if (!string.IsNullOrEmpty(Error))
                    return false;
                lstUser = string.Join(",", lst);
                foreach (string id in lst)
                {
                    Hashtable val1 = new Hashtable();
                    val1["IdProcess"] = dr["IdRow"];
                    val1["Checker"] = id;
                    cnn.Insert(val1, "DP_Process_Detail");
                }
            }
            if (string.IsNullOrEmpty(lstUser))
            {
                string str1084 = @"	select  distinct UserID from Dps_User_GroupUser gu inner join Dps_UserGroupRoles gr on gu.IdGroupUser=gr.IdGroupUser
                            inner join Dps_Roles on IdRole = gr.IDGroupRole
                            inner join DPS_User u on u.UserID = gu.IdUser
                             where IdRole = 1084 and Deleted = 0";
                DataTable dtUser1084 = cnn.CreateDataTable(str1084);
                var temp = dtUser1084.AsEnumerable().Select(x => x["UserID"].ToString()).ToList();
                lstUser = string.Join(",", temp);
            }
            string msg = "Bạn có một phản ánh góp ý mới cần xử lý.";
            return notify(lstUser, msg);
        }

        /// <summary>
        /// Cập nhật quy trình theo bước<para/>
        /// Quy trình kết thúc khi next=-1
        /// https://dpscomvn.visualstudio.com/Webcore%20version%202/_workitems/edit/16344/
        /// </summary>
        /// <param name="IdStep"></param>
        /// <param name="Next"></param>
        /// <param name="UserID"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        public bool Update(int IdStep, int Next, long UserID, string note, Boolean IsComeBack = false)
        {

            SqlConditions cond = new SqlConditions();
            cond.Add(new SqlCondition("Id", Id));
            cond.Add(new SqlCondition("IdStep", IdStep));
            cond.Add(new SqlCondition("Next", Next));
            cond.Add(new SqlCondition("Passed", 0));
            cond.Add(new SqlCondition("IsComeBack", IsComeBack));
            string sql = @"select * from DP_Process 
where Id = @Id and IdStep=@IdStep  ";
            sql += " select IdRow from DP_Process p where Id = @Id and IdStep=@IdStep and Next=@Next  and IsComeBackPro = @IsComeBack and (select count(*) from DP_Process_Detail d where p.IdRow=d.IdProcess and Checked=1 and Passed=0)=0";
            sql += " select * from Tbl_PhanAnhGopY where IsDel=0 and Id=@Id";
            DataSet ds = cnn.CreateDataSet(sql, cond);
            if (ds == null)
            {
                Error = "Có gì đó không đúng, vui lòng thử lại sau";
                return false;
            }
            if (ds.Tables[2] == null || ds.Tables[2].Rows.Count == 0)
            {
                Error = "Phản ánh không tồn tại";
                return false;
            }
            if (ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0 || ds.Tables[1] == null || ds.Tables[1].Rows.Count == 0)
            {
                Error = "Không tìm thấy bước xử lý hoặc bước xử lý đã được thực hiện";
                return false;
            }
            DateTime now = DateTime.Now;
            DataTable dt = ds.Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                string idProcess = dr["IdRow"].ToString();
                //Đánh dấu đã xử lý các DP_Process_Detail này
                Hashtable valPassed = new Hashtable();
                valPassed.Add("Passed", 1);

                if (IdLS == 0)
                {
                    valPassed.Add("IdLS", DBNull.Value);
                }
                else
                {
                    valPassed.Add("IdLS", IdLS);
                }
                valPassed.Add("CheckDate", now);
                cnn.Update(valPassed, new SqlConditions() { { "IdProcess", idProcess }, { "Passed", 0 } }, "DP_Process_Detail");
            };
            //cập nhật checker cho process này vào bảng DP_Process_Detail
            Hashtable val = new Hashtable();
            val.Add("Checked", 1);
            val.Add("CheckNote", note);
            cnn.Update(val, new SqlConditions() { { "IdProcess", ds.Tables[1].Rows[0][0].ToString() }, { "Checker", UserID } }, "DP_Process_Detail");

            //tìm checkers tiếp theo khi chưa hoàn tất
            if (Next != -1)
            {
                List<string> lstUserPos = new List<string>();
                string lstUser = "";
                sql = "exec sp_FindXuLyTiepTheo_Checker @Id ,@IdStep ,@Next,@IsComeBack ";
                DataSet dsNext = cnn.CreateDataSet(sql, new SqlConditions { { "Id", Id }, { "IdStep", IdStep }, { "Next", Next }, { "IsComeBack", IsComeBack } });

                DataTable dtNext = dsNext.Tables[0];
                foreach (DataRow dr in dtNext.Rows)//1, 3 IdStep=2
                {
                    List<string> lst = GetCheckers(dr["IdStep"].ToString(), dr["Next"].ToString(), Convert.ToBoolean(dr["IsComeBackPro"]));
                    if (!string.IsNullOrEmpty(Error))
                        return false;
                    if (lst.Count > 0 && dr["IdForm"].ToString() != "6")
                    {
                        ///huy thi k can thong bao
                        lstUserPos.AddRange(lst);
                    }

                    foreach (string id in lst)
                    {
                        Hashtable val1 = new Hashtable();
                        val1["IdProcess"] = dr["IdRow"];
                        val1["Checker"] = id;
                        cnn.Insert(val1, "DP_Process_Detail");
                        if (cnn.LastError != null) return false;
                    }
                }
                if (lstUserPos.Count > 0)
                {
                    lstUserPos = lstUserPos.Distinct().ToList();
                }
                lstUser = string.Join(",", lstUserPos);
                if (string.IsNullOrEmpty(lstUser))
                {
                    string str58 = @"	select  distinct UserID from Tbl_User_GroupUser gu inner join Dps_UserGroupRoles gr on gu.IdGroupUser=gr.IdGroupUser
                                        inner join Dps_Roles on IdRole = gr.IDGroupRole
                                        inner join DPS_User u on u.UserID = gu.IdUser
										inner join v_QuyenUser qu on qu.IdUser=u.UserID and qu.IdRole=Dps_Roles.IdRole
                                         where Dps_Roles.IdRole = 1084 and Deleted = 0 and u.Active =1 ";
                    DataTable dtUser58 = cnn.CreateDataTable(str58);
                    var temp = dtUser58.AsEnumerable().Select(x => x["UserID"].ToString()).ToList();
                    lstUser = string.Join(",", temp);
                }


            }
            return true;
        }

        /// <summary>
        /// Tìm checkers của phản ánh dựa vào bước hiện tại và kế tiếp 
        /// Trường hợp dv được phân phối: đảm bảo chỉ những phân phối mới đc active
        /// </summary>
        /// <param name="IdStep">Bước hiện tại của phản ánh</param>
        /// <param name="Next">Bước tiếp theo của phản ánh</param>
        /// <param name="IsComeBack">có quay lui l</param>
        /// <returns></returns>
        public List<string> GetCheckers(string IdStep, string Next, Boolean IsComeBack = false)
        {
            List<string> re = new List<string>();
            string sql = @" exec sp_GetCheckerXuLy @Loai, @IdStep , @Next ,@Id ,@IsComeBack,1 ";
            DataTable dtUser = cnn.CreateDataTable(sql, new SqlConditions() { { "Loai", 2 }, { "IdStep", IdStep }, { "Next", Next }, { "Id", Id }, { "IsComeBack", IsComeBack } });
            re = dtUser.AsEnumerable().Select(x => x["UserID"].ToString()).ToList();
            return re;
        }
        /// <summary>
        /// Tìm checkers của phản ánh dựa vào bước hiện tại và kế tiếp 
        /// Trường hợp dv được phân phối: đảm bảo chỉ những phân phối mới đc active, 
        /// 3/4/2020 các bộ phận nhận SMS trừ phân phối
        /// </summary>
        /// <param name="getAll">true = lấy tất cả </param>
        /// <param name="isComeBack">true = lay quay lui</param>
        /// <param name="IdStep">cac Bước tiếp theo của phản ánh can thong bao</param>
        /// <returns></returns>
        public List<UserForStepModel> DSUserDaDuocPhanCongVaoXuLyTiepDeThongBao(string IdStep, bool getAll = true, Boolean isComeBack = false)
        {
            List<UserForStepModel> re = new List<UserForStepModel>();
            string sql = @"
                		select distinct prod.Checker as UserID,Isnull(u.PhoneNumber,'') as PhoneNumber
                  from DP_Process p 
				  join DP_Process_Detail prod on p.IdRow = prod.IdProcess and prod.Passed=0
                  join v_BangMoTaQuyTrinh qtr on p.IdStep = qtr.IdFromStep and p.Next = qtr.IdToStep and p.IsComeBackPro = qtr.IsComeBack
				join Tbl_PhanAnhGopY pa on p.Id = pa.Id  and pa.IdStep = p.IdStep
				  join Dps_User u on prod.Checker = u.UserID and u.Active=1 and u.Deleted=0
				where p.Id=@Id and p.IdStep = pa.IdStep  and qtr.IsComeBack = @IsComeBack 
				and qtr.StatusToStep not in (99) and qtr.StatusToStep in (2,3,6)   ";
            if (getAll)
            {
                sql = @"
                		select distinct prod.Checker as UserID,Isnull(u.PhoneNumber,'') as PhoneNumber
                          from DP_Process p 
				          join DP_Process_Detail prod on p.IdRow = prod.IdProcess and prod.Passed=0
                          join v_BangMoTaQuyTrinh qtr on p.IdStep = qtr.IdFromStep and p.Next = qtr.IdToStep and p.IsComeBackPro = qtr.IsComeBack
				        join Tbl_PhanAnhGopY pa on p.Id = pa.Id  and pa.IdStep = p.IdStep
				          join Dps_User u on prod.Checker = u.UserID and u.Active=1 and u.Deleted=0
				        where p.Id=@Id and p.IdStep = pa.IdStep  and qtr.IsComeBack = @IsComeBack 
				        and qtr.StatusToStep not in (99)    ";
            }


            DataTable dtUser = cnn.CreateDataTable(sql, new SqlConditions() { { "Next", IdStep }, { "Id", Id }, { "IsComeBack", isComeBack } });
            re = (from rw in dtUser.AsEnumerable()
                  select new UserForStepModel
                  {
                      UserID = Convert.ToInt64(rw["UserID"].ToString()),
                      PhoneNummber = String.IsNullOrEmpty(rw["PhoneNumber"].ToString()) ? "" : rw["PhoneNumber"].ToString(),
                  }).ToList();
            return re;
        }






        /// <summary>
        /// Lấy danh sách số đt của user
        /// Xử lý phản ánh không gửi sms tới bộ phân phân phối quyền 22
        /// </summary>
        /// <param name="Users">List id user</param>
        /// <returns></returns>
        public List<string> GetNumberPhoneByUserID(List<string> Users)
        {
            string ids = String.Join(",", Users);
            List<string> re = new List<string>();
            string strQ = $@"	select  distinct UserID,phone.NumberPhone
                                from  DPS_User u 
                                left join Tbl_User_Phone phone on phone.IdUser=u.UserID
                                where  Deleted=0 and phone.NumberPhone is not null and UserID in ({ids})
								and UserID not in (select IdUser from v_QuyenUser where IdRole=22 and IdUser in ({ids}) ) ";
            DataTable dtUser = cnn.CreateDataTable(strQ);
            re = dtUser.AsEnumerable().Select(x => x["NumberPhone"].ToString()).ToList();
            return re;
        }
        /// <summary>
        /// Tìm các bước tiếp theo và bước trả lại<para/>
        /// Chưa khởi tạo process, Chưa xác định bước -> tìm các bước khởi đầu
        /// </summary>
        /// <param name="IdStep"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public List<DPStep> FindNext(string IdStep, long UserID)
        {
            var re = new List<DPStep>();
            if (string.IsNullOrEmpty(IdStep))
            {
                string sqlNew = "select * from DP_Step where Loai=0 and Disabled=0";
                DataTable dtNew = cnn.CreateDataTable(sqlNew);
                foreach (DataRow x in dtNew.Rows)
                {
                    if (checkStepByUser(x["Type"].ToString(), x["IdQuyen"].ToString(), UserID))
                    {
                        var temp = new DPStep(int.Parse(x["IdRow"].ToString()),
                                                 int.Parse(x["IdForm"].ToString()),
                                                 x["Description"].ToString()
                                                 , Convert.ToBoolean(x["IsComeBack"])
                                                 , Convert.ToBoolean(x["IsLoop"]));
                        re.Add(temp);
                    }
                }
                return re;
            }
            string sql = @" EXEC sp_FindNextThaoTac @Id ,@IdStep , @UserID ";
            DataTable dt = cnn.CreateDataTable(sql, new SqlConditions() { { "Id", Id }, { "IdStep", IdStep }, { "UserID", UserID } });
            if (dt != null && dt.Rows.Count > 0)
            {
                re = dt.AsEnumerable().Select(x => new DPStep(
                    int.Parse(x["IdRow"].ToString()), int.Parse(x["IdForm"].ToString()), x["ButtonText"].ToString()
                    , Convert.ToBoolean(x["IsComeBack"])
                    , Convert.ToBoolean(x["IsLoop"])
                    )).ToList();
            }
            return re;
        }

        public bool checkStepByUser(string Type, string IdQuyen, long IdUser)
        {
            switch (Type)
            {
                case "1":// Theo phân phối
                    {
                        return false;
                    }
                case "2":// Theo quyền
                    {
                        string sql = "select dbo.fnCheckRoleByUserID(" + IdUser + "," + IdQuyen + ")";
                        DataTable dt = cnn.CreateDataTable(sql);
                        if (dt != null && dt.Rows.Count > 0)
                            return true;
                        return false;
                    }
                case "3":// Theo quyền và phân phối
                    return false;
                case "4":// Người thực hiện bước trước
                    return false;
                case "5":// Theo vai trò
                    {
                        string sql = "select IdUser from Dps_User_GroupUser where Disabled=0 and IdGroupUser=" + IdQuyen + "  and IdUser=" + IdUser;
                        DataTable dtUser = cnn.CreateDataTable(sql);
                        if (dtUser != null && dtUser.Rows.Count > 0)
                            return true;
                        return false;
                    }
                case "6":// Quản lý trưc tiếp
                    return false;
                case "7 ":// Người tạo
                    {
                        string sql = "select IdRow from Tbl_VanBan where CreatedBy=" + IdUser;
                        DataTable dt = cnn.CreateDataTable(sql);
                        if (dt != null && dt.Rows.Count > 0)
                            return true;
                        return false;
                    }
                case "8"://Tất cả
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Tìm các bước tiếp theo và bước trả lại không theo quyền user<para/>
        /// </summary>
        /// <param name="IdStep"></param>
        /// <returns></returns>
        public List<DPStep> FindThaoTacTiepCuaPhanAnh(string IdStep)
        {
            List<DPStep> re = new List<DPStep>();
            string sql = @" EXEC sp_FindNextThaoTacPhanAnh @Id ,@IdStep ";
            DataTable dt = cnn.CreateDataTable(sql, new SqlConditions() { { "Id", Id }, { "IdStep", IdStep } });
            if (dt != null && dt.Rows.Count > 0)
            {
                re = dt.AsEnumerable().Select(x => new DPStep(
                    int.Parse(x["IdRow"].ToString()), int.Parse(x["IdForm"].ToString()), x["ButtonText"].ToString()
                    , Convert.ToBoolean(x["IsComeBack"])
                    , Convert.ToBoolean(x["IsLoop"])
                    )).ToList();
            }
            return re;
        }

        /// <summary>
        /// Kiểm tra quy trình đã hoàn thành chưa (tát cả các bước được thực hiện)
        /// </summary>
        /// <returns></returns>
        public bool IsFinal()
        {
            string sql = "select * from DP_Process where Checker is null and Id=" + Id;
            DataTable dt = cnn.CreateDataTable(sql);
            if (dt.Rows.Count > 0)
                return false;
            return true;
        }
        /// <summary>
        /// Lưu checker nếu truyền, k truyền thì tìm checker
        /// </summary>
        /// <param name="Checkers"></param>
        /// <param name="Next"></param>
        /// <returns></returns>
        public bool AddChecker(List<string> Checkers, string Next, Boolean IsComeBack)
        {
            string sql = @"SELECT IdRow, p.IdStep, Next,IsComeBackPro from DP_Process p
join Tbl_PhanAnhGopY pa on p.Id = pa.Id and p.IdStep = pa.IdStep
where pa.Id = @Id and p.Next=@Next and p.IsComeBackPro = @IsComeBackPro ";
            DataTable dtProcess = cnn.CreateDataTable(sql, new SqlConditions() { { "Id", Id }, { "Next", Next }, { "IsComeBackPro", IsComeBack } });
            if (dtProcess.Rows.Count == 0)
            {
                Error = "Không tìm thấy bước xử lý tiếp theo";
                return false;
            }
            DataRow dr = dtProcess.Rows[0];
            if (Checkers == null || Checkers.Count == 0)
            {
                Checkers = GetCheckers(dr["IdStep"].ToString(), Next, Convert.ToBoolean(dr["IsComeBackPro"]));
                if (!string.IsNullOrEmpty(Error))
                    return false;
            }
            foreach (string id in Checkers)
            {
                Hashtable val1 = new Hashtable();
                val1["IdProcess"] = dr["IdRow"];
                val1["Checker"] = id;
                if (cnn.Insert(val1, "DP_Process_Detail") < 1)
                {
                    cnn.RollbackTransaction();
                    Error = "Có gì đó không đúng, vui lòng thử lại sau";
                    return false;
                }
                try
                {
                    string udpate_DonVi = " EXEC sp_ThemDonViXuLy @Id , @Next, @IdUser,@IsComeBack ";
                    cnn.ExecuteNonQuery(udpate_DonVi, new SqlConditions { { "Id", Id }, { "Next", Next }, { "IdUser", id }, { "IsComeBack", IsComeBack } });
                }
                catch (Exception ex)
                {
                    cnn.RollbackTransaction();
                    cnn.ClearError();
                }
            }

            string lstUser = string.Join(",", Checkers);
            string msg = "Bạn có một phản ánh góp ý mới cần xử lý.";
            return notify(lstUser, msg);
        }

        /// <summary>
        /// Hàm push notify sử dụng fcm
        /// </summary>
        /// <param name="lstUser">Mảng Id user phân cách bằng dấu ,</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool notify(string lstUser, string message)
        {
            string domain = "";

            if (string.IsNullOrEmpty(message))
            {
                Error = "Thông báo không được rỗng";
                return false;
            }
            #region PushNotify notify fcm
            string sqlF = "select distinct token from FCM where IdUser in (" + lstUser + ") ";
            DataTable dtF = cnn.CreateDataTable(sqlF);
            if (cnn.LastError != null)
            {
                Error = "Có gì đó không đúng vui lòng thử lại sau";
                return false;
            }
            string link = domain + "/xu-ly-phan-anh";
            //FCMController controller = new FCMController();
            //foreach (DataRow dr in dtF.Rows)
            //{
            //    Task.Run(() => controller.pushFCM(dr[0].ToString(), new NofModel(message, link, "Phản ánh hiện trường")));
            //}
            return true;
            #endregion
        }

        /// <summary>
        /// Hàm push notify dùng chung sử dụng fcm
        /// </summary>
        /// <param name="lstUser">Mảng Id user phân cách bằng dấu ,</param>
        /// <param name="message"></param>
        /// <param name="url">phải có dấu/</param>
        /// <param name="tieude">Tiêu đề chức năng</param>
        /// <returns></returns>
        public bool notifyVanDeCus(string lstUser, string message, string url, string tieude)
        {
            string domain = "";

            if (string.IsNullOrEmpty(message))
            {
                Error = "Thông báo không được rỗng";
                return false;
            }
            #region PushNotify notify fcm
            string sqlF = "select distinct token from FCM where IdUser in (" + lstUser + ") ";
            DataTable dtF = cnn.CreateDataTable(sqlF);
            if (cnn.LastError != null)
            {
                Error = "Có gì đó không đúng vui lòng thử lại sau";
                return false;
            }
            string link = domain + url;
            //FCMController controller = new FCMController();
            //foreach (DataRow dr in dtF.Rows)
            //{
            //    Task.Run(() => controller.pushFCM(dr[0].ToString(), new NofModel(message, link, tieude)));
            //}
            return true;
            #endregion
        }

    }
    public class UserForStepModel
    {
        public long UserID { get; set; }
        public string PhoneNummber { get; set; }


    }
}
