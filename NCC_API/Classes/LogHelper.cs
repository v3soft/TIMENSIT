using DpsLibs.Data;
using Timensit_API.Models;
using Timensit_API.Models.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.ComponentModel;

namespace Timensit_API.Classes
{
    /// <summary>
    /// Lưu các thao tác vào Tbl_Log
    /// </summary>
    public class LogHelper
    {
        public string Error = "";
        private int Loai;
        private string Ten_Loai;
        private IHttpContextAccessor _accessor;
        public NCCConfig _config;
        private readonly IHostingEnvironment _hostingEnvironment;
        public LogHelper()
        {
        }
        /// <summary>
        /// Tbl_Log.IdLoaiLog==Tbl_Log_Loai.IdRow, =0 sẽ k lò được Tbl_Log
        /// </summary>
        /// <param name="configLogin"></param>
        /// <param name="accessor"></param>
        /// <param name="idLoai">7	Công việc, 9	Văn bản đến, 10	Văn bản đi</param>
        public LogHelper(NCCConfig configLogin, IHttpContextAccessor accessor, IHostingEnvironment hostingEnvironment, int idLoai)
        {
            _config = configLogin;
            _accessor = accessor;
            Loai = idLoai;
            _hostingEnvironment = hostingEnvironment;
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                DataTable dt = cnn.CreateDataTable("select * from Tbl_Log_Loai where IdRow=" + Loai);
                if (dt.Rows.Count > 0)
                    Ten_Loai = dt.Rows[0]["LoaiLog"].ToString();
            }
        }
        public long LogTai(long idUser, string text = "")
        {
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                string ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                Hashtable val = new Hashtable();
                val.Add("IdLoaiLog", Loai);
                val.Add("IdHanhDong", 7);
                val.Add("IP", ip);
                val.Add("NoiDung", "Xuất danh sách " + Ten_Loai.ToLower() + " " + text);
                val.Add("CreatedBy", idUser);
                val.Add("CreatedDate", DateTime.Now);
                int kq = cnn.Insert(val, "Tbl_Log");
                string idc = cnn.ExecuteScalar("select IDENT_CURRENT('Tbl_Log')").ToString();
                cnn.Disconnect();
                if (kq == 1)
                    return long.Parse(idc);
                else
                    return 0;
            }
        }
        public long LogImport(long idUser, string text = "")
        {
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                string ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                Hashtable val = new Hashtable();
                val.Add("IdLoaiLog", Loai);
                val.Add("IdHanhDong", 7);
                val.Add("IP", ip);
                val.Add("NoiDung", "Import danh sách " + Ten_Loai.ToLower() + " " + text);
                val.Add("CreatedBy", idUser);
                val.Add("CreatedDate", DateTime.Now);
                int kq = cnn.Insert(val, "Tbl_Log");
                string idc = cnn.ExecuteScalar("select IDENT_CURRENT('Tbl_Log')").ToString();
                cnn.Disconnect();
                if (kq == 1)
                    return long.Parse(idc);
                else
                    return 0;
            }
        }
        public long LogXemDS(long idUser, string text = "")
        {
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                string ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                Hashtable val = new Hashtable();
                val.Add("IdLoaiLog", Loai);
                val.Add("IdHanhDong", 1);
                val.Add("IP", ip);
                val.Add("NoiDung", "Xem danh sách " + Ten_Loai.ToLower() + " " + text);
                val.Add("CreatedBy", idUser);
                val.Add("CreatedDate", DateTime.Now);
                int kq = cnn.Insert(val, "Tbl_Log");
                string idc = cnn.ExecuteScalar("select IDENT_CURRENT('Tbl_Log')").ToString();
                cnn.Disconnect();
                if (kq == 1)
                    return long.Parse(idc);
                else
                    return 0;
            }
        }
        public long LogXemCT(long idDoiTuong, long idUser, string text = "")
        {
            if (idDoiTuong <= 0)
            {
                Error = "Vui lòng truyền id đối tượng để log";
                return 0;
            }

            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                string ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                Hashtable val = new Hashtable();
                val.Add("IdLoaiLog", Loai);
                val.Add("IdHanhDong", 1);
                val.Add("IP", ip);
                val.Add("NoiDung", "Xem chi tiết " + Ten_Loai.ToLower() + " " + text);
                val.Add("IdDoiTuong", idDoiTuong);
                val.Add("CreatedBy", idUser);
                val.Add("CreatedDate", DateTime.Now);
                int kq = cnn.Insert(val, "Tbl_Log");
                string idc = cnn.ExecuteScalar("select IDENT_CURRENT('Tbl_Log')").ToString();
                cnn.Disconnect();
                if (kq == 1)
                    return long.Parse(idc);
                else
                    return 0;
            }
        }
        public long LogThem(long idDoiTuong, long idUser, string text = "")
        {
            if (idDoiTuong <= 0)
            {
                Error = "Vui lòng truyền id đối tượng để log";
                return 0;
            }
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                string ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                Hashtable val = new Hashtable();
                val.Add("IdLoaiLog", Loai);
                val.Add("IdHanhDong", 2);
                val.Add("IP", ip);
                val.Add("NoiDung", "Thêm mới " + Ten_Loai.ToLower() + " " + text);
                val.Add("IdDoiTuong", idDoiTuong);
                val.Add("CreatedBy", idUser);
                val.Add("CreatedDate", DateTime.Now);
                int kq = cnn.Insert(val, "Tbl_Log");
                string idc = cnn.ExecuteScalar("select IDENT_CURRENT('Tbl_Log')").ToString();
                cnn.Disconnect();
                if (kq == 1)
                    return long.Parse(idc);
                else
                    return 0;
            }
        }
        public long LogSua(long idDoiTuong, long idUser, string text = "", string note = "")
        {
            if (idDoiTuong <= 0)
            {
                Error = "Vui lòng truyền id đối tượng để log";
                return 0;
            }
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                string ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                Hashtable val = new Hashtable();
                val.Add("IdLoaiLog", Loai);
                val.Add("IdHanhDong", 3);
                val.Add("IP", ip);
                string nd = "Cập nhật " + Ten_Loai.ToLower() + " " + text;
                if (note != "")
                    nd += ": " + note;
                val.Add("NoiDung", nd);
                val.Add("IdDoiTuong", idDoiTuong);
                val.Add("CreatedBy", idUser);
                val.Add("CreatedDate", DateTime.Now);
                int kq = cnn.Insert(val, "Tbl_Log");
                string idc = cnn.ExecuteScalar("select IDENT_CURRENT('Tbl_Log')").ToString();
                cnn.Disconnect();
                if (kq == 1)
                    return long.Parse(idc);
                else
                    return 0;
            }
        }
        public long LogXoa(long idDoiTuong, long idUser, string text = "")
        {
            if (idDoiTuong <= 0)
            {
                Error = "Vui lòng truyền id đối tượng để log";
                return 0;
            }
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                string ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                Hashtable val = new Hashtable();
                val.Add("IdLoaiLog", Loai);
                val.Add("IdHanhDong", 4);
                val.Add("IP", ip);
                val.Add("NoiDung", "Xóa " + Ten_Loai.ToLower() + " " + text);
                val.Add("IdDoiTuong", idDoiTuong);
                val.Add("CreatedBy", idUser);
                val.Add("CreatedDate", DateTime.Now);
                int kq = cnn.Insert(val, "Tbl_Log");
                string idc = cnn.ExecuteScalar("select IDENT_CURRENT('Tbl_Log')").ToString();
                cnn.Disconnect();
                if (kq == 1)
                    return long.Parse(idc);
                else
                    return 0;
            }
        }
        public long LogSuas(List<long> idDoiTuongs, long idUser, string text = "", string note = "")
        {
            if (idDoiTuongs.Count <= 0)
            {
                Error = "Vui lòng truyền id đối tượng để log";
                return 0;
            }
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                string ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                Hashtable val = new Hashtable();
                val.Add("IdLoaiLog", Loai);
                val.Add("IdHanhDong", 3);
                val.Add("IP", ip);
                string nd = "Cập nhật " + Ten_Loai.ToLower() + " " + text;
                if (note != "")
                    nd += ": " + note;
                val.Add("NoiDung", nd);
                val.Add("IdDoiTuong", 0);
                val.Add("ids", string.Join(",", idDoiTuongs));
                val.Add("CreatedBy", idUser);
                val.Add("CreatedDate", DateTime.Now);
                int kq = cnn.Insert(val, "Tbl_Log");
                string idc = cnn.ExecuteScalar("select IDENT_CURRENT('Tbl_Log')").ToString();
                cnn.Disconnect();
                if (kq == 1)
                    return long.Parse(idc);
                else
                    return 0;
            }
        }
        public long LogXoas(List<long> idDoiTuongs, long idUser, string text = "")
        {
            if (idDoiTuongs.Count <= 0)
            {
                Error = "Vui lòng truyền id đối tượng để log";
                return 0;
            }
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                string ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                Hashtable val = new Hashtable();
                val.Add("IdLoaiLog", Loai);
                val.Add("IdHanhDong", 4);
                val.Add("IP", ip);
                val.Add("NoiDung", "Xóa " + Ten_Loai.ToLower() + " " + text);
                val.Add("IdDoiTuong", 0);
                val.Add("ids", string.Join(",", idDoiTuongs));
                val.Add("CreatedBy", idUser);
                val.Add("CreatedDate", DateTime.Now);
                int kq = cnn.Insert(val, "Tbl_Log");
                string idc = cnn.ExecuteScalar("select IDENT_CURRENT('Tbl_Log')").ToString();
                cnn.Disconnect();
                if (kq == 1)
                    return long.Parse(idc);
                else
                    return 0;
            }
        }
        public long Log(long idHanhDong, long idUser, string noidung, List<long> ids)
        {
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                string ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                Hashtable val = new Hashtable();
                val.Add("IdLoaiLog", Loai);
                val.Add("IdHanhDong", idHanhDong);
                val.Add("IP", ip);
                val.Add("NoiDung", noidung);
                val.Add("IdDoiTuong", 0);
                if (ids.Count > 0)
                    val.Add("ids", string.Join(",", ids));

                val.Add("CreatedBy", idUser);
                val.Add("CreatedDate", DateTime.Now);
                int kq = cnn.Insert(val, "Tbl_Log");
                string idc = cnn.ExecuteScalar("select IDENT_CURRENT('Tbl_Log')").ToString();
                cnn.Disconnect();
                if (kq == 1)
                    return long.Parse(idc);
                else
                    return 0;
            }
        }
        public long Log(long idHanhDong, long idUser, string noidung, long idDoituong = 0)
        {
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                string ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                Hashtable val = new Hashtable();
                val.Add("IdLoaiLog", Loai);
                val.Add("IdHanhDong", idHanhDong);
                val.Add("IP", ip);
                val.Add("NoiDung", noidung);
                if (idDoituong > 0)
                    val.Add("IdDoiTuong", idDoituong);

                val.Add("CreatedBy", idUser);
                val.Add("CreatedDate", DateTime.Now);
                int kq = cnn.Insert(val, "Tbl_Log");
                string idc = cnn.ExecuteScalar("select IDENT_CURRENT('Tbl_Log')").ToString();
                cnn.Disconnect();
                if (kq == 1)
                    return long.Parse(idc);
                else
                    return 0;
            }
        }

        /// <summary>
        /// Lưu lịch sử Tbl_LichSu 
        /// </summary>
        /// <param name="Loai">1: văn bản đến, 2: văn bản đi, 3: hồ sơ, 4: công việc</param>
        /// <param name="hanhDong"></param>
        /// <param name="idUser"></param>
        /// <param name="noidung"></param>
        /// <param name="idDoituong"></param>
        /// <param name="cnn"></param>
        /// <param name="IdProcess"></param>
        /// <param name="LSDetails">IdRow của bảng bị tác động</param>
        /// <returns></returns>
        public static long LuuLichSu(int Loai, string hanhDong, long idUser, string noidung, bool email, bool sms, long idDoituong, DpsConnection cnn, long IdProcess = 0, bool traLoi = false)
        {
            Hashtable val = new Hashtable();
            val.Add("Loai", Loai);
            val.Add("HanhDong", hanhDong);
            val.Add("NoiDung", string.IsNullOrEmpty(noidung) ? "" : noidung);
            val.Add("Email", email);
            val.Add("SMS", sms);
            val.Add("YeuCauTraLoi", traLoi);
            val.Add("Id", idDoituong);

            val.Add("CreatedBy", idUser);
            val.Add("CreatedDate", DateTime.Now);
            if (IdProcess > 0)
                val.Add("IdProcess", IdProcess);
            int kq = cnn.Insert(val, "Tbl_LichSu");
            if (kq == 1)
            {
                var id = cnn.ExecuteScalar("select IDENT_CURRENT ('Tbl_LichSu')");
                return long.Parse(id.ToString());
            }
            else
                return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">1: Update, 2: Check, 3: Get</param>
        /// <param name="request">reqtest json khi bắn API</param>
        /// <param name="response">response json khi API trả về</param>
        /// <param name="ErrorCode">ErrorCode</param>
        /// <param name="ErrorMessage">ErrorMessage</param>
        /// <returns></returns>
        public bool LogJeedocs(int type, string request, string response, string ProcessCode, string ProcessMessage, int idUser, bool Success)
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    Hashtable val = new Hashtable();
                    //val.Add("type", Loai);
                    val.Add("ProcessCode", ProcessCode);
                    val.Add("ProcessMessage", ProcessMessage);
                    val.Add("ResponseJson", response);
                    val.Add("Request", request);
                    val.Add("Type", type);
                    val.Add("Success", Success);

                    val.Add("CreatedBy", idUser);
                    val.Add("CreatedDate", DateTime.Now);

                    int kq = cnn.Insert(val, "Tbl_JeeDocs_Log");
                    cnn.Disconnect();
                    if (kq == 1)
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #region log file
        /// <summary>
        /// Lưu log 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="username"></param>
        /// <param name="logEdit">Lưu log thêm/sửa</param>
        public void Ghilogfile<T>(T data, LoginData loginData, string hanhDong, long Id_Log, string text = "")
        {
            string str = ObjectToString<T>(data);
            string LogContent = "(Id_Log:" + Id_Log + ")" + hanhDong + " " + Ten_Loai.ToLower();
            if (!string.IsNullOrEmpty(text))
                LogContent += " " + text;
            LogContent += ": " + str;
            if (!"".Equals(LogContent))
            {
                WriteLogByFunction(LogContent, loginData);
                WriteLogByDay(LogContent, loginData);
                WriteLogByUser(LogContent, loginData);
            }
        }
        public void Ghilogfile<T>(List<T> datas, LoginData loginData, string hanhDong, long Id_Log, string text = "")
        {
            string str = "[";
            foreach (var data in datas)
            {
                if (str != "")
                    str += ", ";
                str += ObjectToString<T>(data);
            }
            str += "]";
            string LogContent = "(Id_Log:" + Id_Log + ")" + hanhDong + " " + Ten_Loai.ToLower();
            if (!string.IsNullOrEmpty(text))
                LogContent += " " + text;
            LogContent += ": " + str;
            if (!"".Equals(LogContent))
            {
                WriteLogByFunction(LogContent, loginData);
                WriteLogByDay(LogContent, loginData);
                WriteLogByUser(LogContent, loginData);
            }
        }
        public void WriteLogByFunction(string content, LoginData loginData)
        {
            string _basePath = Path.Combine(_hostingEnvironment.ContentRootPath, Constant.RootUpload + "/");
            StreamWriter w;
            string pathdir = _basePath + "/Logs/theochucnang/";
            if (!Directory.Exists(pathdir))
            {
                Directory.CreateDirectory(pathdir);
            }
            string fullpath_filename = pathdir + Ten_Loai + ".txt";

            if (!File.Exists(fullpath_filename))
            {
                w = File.CreateText(fullpath_filename);
            }
            else w = File.AppendText(fullpath_filename);
            try
            {

                w.WriteLine(loginData.Id + "(" + loginData.UserName + ") - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - " + content);
                w.Flush();
                w.Close();
            }
            catch (Exception ex)
            {
                w.Flush();
                w.Close();
            }

        }
        public void WriteLogByDay(string content, LoginData loginData)
        {
            string _basePath = Path.Combine(_hostingEnvironment.ContentRootPath, Constant.RootUpload + "/");
            string pathdir = _basePath + "/Logs/theongay/";
            if (!Directory.Exists(pathdir))
            {
                Directory.CreateDirectory(pathdir);
            }
            string fullpath_filename = pathdir + DateTime.Today.ToString("yyyyMMdd") + ".txt";
            StreamWriter w;
            if (!File.Exists(fullpath_filename))
            {
                w = File.CreateText(fullpath_filename);
            }
            else w = File.AppendText(fullpath_filename);
            w.WriteLine(loginData.Id + "(" + loginData.UserName + ") - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - " + content);
            w.Flush();
            w.Close();
        }
        public void WriteLogByUser(string content, LoginData loginData)
        {
            string _basePath = Path.Combine(_hostingEnvironment.ContentRootPath, Constant.RootUpload + "/");
            string pathdir = _basePath + "/Logs/theonguoidung/";
            if (!Directory.Exists(pathdir))
            {
                Directory.CreateDirectory(pathdir);
            }
            string fullpath_filename = pathdir + loginData.Id + ".txt";
            StreamWriter w;
            if (!File.Exists(fullpath_filename))
            {
                w = File.CreateText(fullpath_filename);
            }
            else w = File.AppendText(fullpath_filename);
            w.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " - " + content);
            w.Flush();
            w.Close();
        }
        public static string ObjectToString<T>(T data)
        {
            try
            {
                string strJ = JsonConvert.SerializeObject(data);//, Formatting.Indented
                return strJ;
            }
            catch (Exception ex)
            {
                string re = "";
                PropertyDescriptorCollection properties =
                    TypeDescriptor.GetProperties(typeof(T));
                foreach (PropertyDescriptor prop in properties)
                {
                    object value = prop.GetValue(data) ?? DBNull.Value;
                    if (re != "")
                        re += ", ";
                    re += string.Format("{0}({1}): {2}", prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType, value);
                }
                return "{" + re + "}";
            }
        }
        #endregion
    }

    /// <summary>
    /// Map Tbl_LichSu_Detail
    /// </summary>
    public class LSDetailModel
    {
        /// <summary>
        /// Tbl_LichSu.IdRow
        /// </summary>
        public long IdLS { get; set; }
        /// <summary>
        /// 1:user, 2 đơn vị
        /// </summary>
        public int Loai { get; set; }
        /// <summary>
        /// IdRow của bảng bị tác động
        /// </summary>
        public long IdRow { get; set; }
    }
}
