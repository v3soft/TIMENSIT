using DpsLibs.Data;
using Timensit_API.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Timensit_API.Classes
{
    public class SMSHelper
    {
        public static string ErrorMessage = "";
        public static decimal checkBalance(CauHinhSMSModel brand)
        {
            ViettelSMSService.CcApiClient client = new ViettelSMSService.CcApiClient();
            var task = client.checkBalanceAsync(brand.UserName, brand.Password, brand.ServiceId);
            ViettelSMSService.cpBalance result = task.Result.@return;
            if (result.errCode == 0)
            {
                return result.balance;
            }
            else
            {
                ErrorMessage = "Thông tin tài khoản không đúng hoặc tài khoản không trả trước (" + result.errDesc + ")";
                return 0;
            }
        }
        public static long SendViettel(CauHinhSMSModel brand, string noiDung, string SDT, string RequestId)
        {
            ErrorMessage = "";
            noiDung = System.Net.WebUtility.HtmlDecode(noiDung);
            ViettelSMSService.CcApiClient client = new ViettelSMSService.CcApiClient();
            string sdt = ChuoiDTViettel(SDT);
            var task = client.wsCpMtAsync(brand.UserName, brand.Password, brand.ServiceId, RequestId, sdt, sdt, brand.Brandname, "bulksms", noiDung, "1");
            ViettelSMSService.result result = task.Result.@return;
            if (result.result1 != 1)
            {
                //thất bại
                ErrorMessage = result.message;
            }
            return result.result1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cnn"></param>
        /// <param name="Loai">1: văn bản đến, 2: văn bản đi, 3: hồ sơ, 4: công việc, 5 thông báo, 6 lịch, 7 lịch sử</param>
        /// <param name="Id">Id tương ứng với loại</param>
        /// <returns></returns>
        public static bool SendSMS(SMSModel data, DpsConnection cnn, int Loai, long Id)
        {
            string find = @"select ch.* from Tbl_CauHinh_SMS ch left join DM_DonVi dv on ch.DonVi=dv.Id
left join DPs_User u on dv.Id=u.IdDonVi
where ch.Disabled=0 and ch.Locked=0 and (UserId=@Id or ch.DonVi=0)

select ch.* from Tbl_CauHinh_SMS ch 
join Tbl_CauHinh_DonViCon con on ch.Id=con.IdCauHinh
join DM_DonVi dv on con.DonVi=dv.Id
join DPs_User u on dv.Id=u.IdDonVi
where ch.Disabled=0 and ch.Locked=0 and UserId=@Id";
            DataSet ds = cnn.CreateDataSet(find, new SqlConditions() { { "Id", data.CreatedBy } });
            if (cnn.LastError != null || ds == null)
            {
                ErrorMessage = "Có gì đó không đúng, vui lòng thử lại sau";
                return false;
            }
            DataRow dataRow = null;
            if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                dataRow = ds.Tables[0].Rows[0];
            else
            {
                if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                    dataRow = ds.Tables[1].Rows[0];
            }
            if (dataRow == null)
            {
                ErrorMessage = "Không tìm thấy cấu hình SMS của đơn vị";
                return false;
            }
            data.brand = new CauHinhSMSModel();
            data.brand.Id = long.Parse(dataRow["Id"].ToString());
            data.brand.Brandname = dataRow["Brandname"].ToString();
            data.brand.UserName = dataRow["UserName"].ToString();
            data.brand.Password = dataRow["Password"].ToString();
            data.brand.ServiceId = dataRow["ServiceId"].ToString();
            if (data.Users == null || data.Users.Count == 0)
            {
                ErrorMessage = "Không có người nhận";
                return false;
            }
            string sql = "select distinct UserID, fullname, PhoneNumber from DPS_User where UserId in (" + string.Join(",", data.Users) + ")";
            DataTable dt = cnn.CreateDataTable(sql);
            if (dt == null || dt.Rows.Count == 0)
            {
                ErrorMessage = "Không tìm thấy người nhận";
                return false;
            }
            try
            {
                List<SMS_Detail_Model> details = new List<SMS_Detail_Model>();
                Hashtable val = new Hashtable();
                val["IdBrandname"] = data.brand.Id;
                val["Brandname"] = data.brand.Brandname;
                val["Username"] = data.brand.UserName;
                val["Password"] = data.brand.Password;
                val["Message"] = data.Message;
                val["CreatedBy"] = data.CreatedBy;
                val["Total"] = dt.Rows.Count;
                val["Loai"] = Loai;
                val["Id"] = Id;
                cnn.BeginTransaction();
                if (cnn.Insert(val, "Sys_SMS") <= 0)
                {
                    ErrorMessage = "Có gì đó không đúng, vui lòng thử lại sau";
                    cnn.RollbackTransaction();
                    return false;
                }
                var temp = cnn.ExecuteScalar("select IDENT_CURRENT('SYS_SMS')");
                data.IdRow = long.Parse(temp.ToString());
                foreach (DataRow dr in dt.Rows)
                {
                    var detail = new SMS_Detail_Model();
                    Hashtable val1 = new Hashtable();
                    val1["IdSMS"] = detail.IdSMS = data.IdRow;
                    val1["SDT"] = detail.SDT = dr["PhoneNumber"] == DBNull.Value ? "" : dr["PhoneNumber"].ToString();
                    val1["UserID"] = detail.UserID = long.Parse(dr["UserID"].ToString());
                    if (string.IsNullOrEmpty(detail.SDT))
                    {
                        val1["Status"] = detail.Status = "0";
                        val1["NoiDungLoi"] = detail.NoiDungLoi = "Chưa cập nhật SDT";
                    }

                    if (cnn.Insert(val1, "Sys_SMS_Detail") <= 0)
                    {
                        ErrorMessage = "Có gì đó không đúng, vui lòng thử lại sau";
                        cnn.RollbackTransaction();
                        return false;
                    }
                    temp = cnn.ExecuteScalar("select IDENT_CURRENT('SYS_SMS_Detail')");
                    long idCT = long.Parse(temp.ToString());
                    detail.IdRow = idCT;
                    details.Add(detail);
                }
                cnn.EndTransaction();

                int count = 0;
                string RequestId = "1";
                foreach (var detail in details)
                {
                    Hashtable val2 = new Hashtable();
                    if (string.IsNullOrEmpty(detail.Status))//có sdt
                    {
                        long re = SendViettel(data.brand, data.Message, detail.SDT, RequestId);
                        val2["status"] = re;
                        if (re == 1)
                            count++;
                        else
                            val2["NoiDungLoi"] = ErrorMessage;
                        cnn.Update(val2, new SqlConditions() { { "IdRow", detail.IdRow } }, "Sys_SMS_Detail");
                        RequestId = "4";
                    }
                }
                Hashtable val4 = new Hashtable();
                val4["Success"] = count;
                cnn.Update(val4, new SqlConditions() { { "IdRow", data.IdRow } }, "Sys_SMS");
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// +84 để phù hợp vs cú pháp gửi sms của viettel
        /// </summary>
        /// <param name="sdt"></param>
        /// <returns></returns>
        public static string ChuoiDTViettel(string sdt)
        {
            string kq = sdt;
            string sdt_1 = sdt.Substring(0, 1);
            string sdt_2 = sdt.Substring(1);
            if (sdt_1 == "0")
                sdt_1 = "84";
            kq = sdt_1 + sdt_2;
            return kq;
        }
        public static string getError(long result)
        {
            switch (result)
            {
                case 0:
                    return "invalid username or password";
                case 1:
                    return "invalid brandname";
                case 2: return "invalid phonenumber";
                case 3: return "Brandname chua khai báo";
                case 4:
                    return "Partner chưa khai báo";
                case 5: return "template chua khai báo";
                case 6: return "login telco system fail";
                case 7: return "error sending sms to telco";
                case 8:
                    return "tin nhắn spam, mỗi số điện thoại nhận tối đa 2 lần cho cũng một nội dung trên một ngày, và 50 lần cho nội dung khác nhau trên một ngày";
                default: return "";
            }
        }
    }
    public class SMSModel
    {
        public long IdRow { get; set; }
        public string Message { get; set; }
        public CauHinhSMSModel brand { get; set; }
        /// <summary>
        /// Iduser của người gửi sms
        /// </summary>
        public long CreatedBy { get; set; }
        public List<long> Users { get; set; }

    }

    public class SMS_Detail_Model
    {
        public long IdRow { get; set; }
        public long IdSMS { get; set; }
        public long UserID { get; set; }
        public string SDT { get; set; }
        public string Status { get; set; }
        public string NoiDungLoi { get; set; }
    }
}
