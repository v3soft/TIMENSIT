using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Timensit_API.Models;
using Timensit_API.Models.Process;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;

namespace Timensit_API.Classes
{
    public class Employee
    {
        NGUOICOCONGContext _context;
        private string error_message;

        public Employee(NGUOICOCONGContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Lấy ID quản lý trực tiếp của 1 chức danh
        /// </summary>
        /// <param name="id_chucdanh">Chức danh</param>
        /// <param name="cnn"></param>
        /// <returns>ID quản lý trực tiếp</returns>
        public List<string> GetIDManager(string id_chucdanh, IDbContextTransaction transaction = null)
        {
            List<string> re = new List<string>();
            string sql = "select Id_row, IsStop from tbl_chucdanh where id_row in (select id_parent from tbl_chucdanh where id_row=" + id_chucdanh + ")";
            DataTable tmp = createTable(sql, transaction);
            if (tmp == null || tmp.Rows.Count <= 0)
                return re;
            string ids = string.Join(",", tmp.AsEnumerable().Select(x => x[0].ToString()));
            DataTable t = createTable("select UserID, IdChucVu from Dps_user where deleted=0 and active=1 and IdChucVu in (" + ids + ")", transaction);
            if (t == null)
                return re;
            if (t.Rows.Count == 0 && !bool.TrueString.Equals(tmp.Rows[0][1].ToString()))//dừng chuyển cấp
                return GetIDManager(tmp.Rows[0][0].ToString(), transaction);
            re.AddRange(t.AsEnumerable().Select(x => x[0].ToString()));
            return re;

        }

        /// <summary>
        /// Lấy quản lý theo cấp
        /// </summary>
        /// <param name="id_nv"></param>
        /// <param name="id_cap">Id cấp lưu trong table dm_capquanly</param>
        /// <returns></returns>
        public List<NguoiDungDPS> GetQuanlyByCap(int id_cap, string id_chucdanh, IDbContextTransaction transaction = null)
        {
            List<NguoiDungDPS> result = null;
            string id_manager = GetIDQuanlyByCap(id_chucdanh, id_cap.ToString(), false, transaction);
            if ("".Equals(id_manager)) return result;
            result = GetById(id_manager, transaction);
            return result;
        }

        /// <summary>
        /// Lấy danh sách idnv theo chức danh, cấp quản lý. Phân cách bằng dấu ,
        /// </summary>
        /// <param name="id_chucdanh"></param>
        /// <param name="capquanly"></param>
        /// <param name="IsLaychinhxaccap"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public string GetIDQuanlyByCap(string id_chucdanh, string capquanly, bool IsLaychinhxaccap, IDbContextTransaction transaction = null)
        {
            string select = "select tbl_chucdanh.id_row, tbl_chucdanh.Id_Capquanly, dm_capquanly.range from tbl_chucdanh inner join tbl_chucdanh as tbl_chucdanh1 on tbl_chucdanh.id_row=tbl_chucdanh1.id_parent left join DM_Capquanly on tbl_chucdanh.Id_Capquanly=dm_capquanly.rowid where tbl_chucdanh1.id_row=" + id_chucdanh;
            DataTable dt = createTable(select, transaction);
            if ((dt.Rows.Count <= 0) || ("0".Equals(dt.Rows[0][0].ToString()))) return "0";
            string result = "";
            if (capquanly.Equals(dt.Rows[0]["Id_Capquanly"].ToString()))
            {
                result = GetIDNVByChucdanh(dt.Rows[0][0].ToString(), transaction);
                if ("".Equals(result))
                {
                    var temp = GetIDManager(dt.Rows[0][0].ToString(), transaction);
                    return string.Join(",", temp);
                }
            }
            else
            {
                int rank = 0;
                if ((!IsLaychinhxaccap) && (!"".Equals(dt.Rows[0]["range"].ToString())) && (int.TryParse(dt.Rows[0]["range"].ToString(), out rank)))
                {
                    //Lấy rank của cấp quản lý
                    string sql = "select range from dm_capquanly where rowid=" + capquanly;
                    DataTable Dtcap = createTable(sql, transaction);
                    int rank_cap = 0;
                    if ((Dtcap.Rows.Count > 0) && (int.TryParse(Dtcap.Rows[0][0].ToString(), out rank_cap)) && (rank_cap <= rank))
                    {
                        //trường hợp cấp của chức danh quản lý lớn hơn cấp của chức danh cần lấy thì lấy chức danh quản lý
                        result = GetIDNVByChucdanh(dt.Rows[0][0].ToString(), transaction);
                        if ("".Equals(result))
                        {
                            var temp = GetIDManager(dt.Rows[0][0].ToString());
                            return string.Join(",", temp);
                        }
                    }
                }
                if ("".Equals(result)) return GetIDQuanlyByCap(dt.Rows[0][0].ToString(), capquanly, IsLaychinhxaccap, transaction);
            }
            return result;
        }

        /// <summary>
        /// Trả về các nhân viên có chức danh truyền vào. Phân cách bằng dấu ,
        /// </summary>
        /// <param name="id_chucdanh"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public string GetIDNVByChucdanh(string id_chucdanh, IDbContextTransaction transaction = null)
        {
            string sql = "select UserID as id_nv from dps_user where (deleted=0) and (Active=1) and ((IdChucVu=" + id_chucdanh + "))";
            DataTable nhanvien = createTable(sql, transaction);
            if (nhanvien.Rows.Count > 0)
            {
                var temp = nhanvien.AsEnumerable().Select(x => x[0].ToString());
                return string.Join(",", temp);
            }
            return "";
        }

        public List<NguoiDungDPS> GetById(string ids, IDbContextTransaction transaction = null)
        {
            List<NguoiDungDPS> re = new List<NguoiDungDPS>();
            if (ids == "")
                return re;
            string sqlq = @"select u.*, title as DonVi,Code as MaDinhDanh from Dps_User u left join Tbl_Cocautochuc dm on u.IdDonVi=dm.RowID
                                    where u.Deleted = 0 and u.Active=1 and UserID in (" + ids + ")";
            DataTable dt = createTable(sqlq, transaction);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow rw in dt.Rows)
                {
                    NguoiDungDPS _Dpsuser = new NguoiDungDPS();
                    _Dpsuser.UserID = long.Parse(rw["UserID"].ToString());
                    _Dpsuser.UserName = rw["UserName"].ToString();
                    _Dpsuser.FullName = rw["FullName"].ToString();
                    _Dpsuser.IdChucVu = int.Parse(rw["IdChucVu"].ToString());
                    _Dpsuser.Email = rw["Email"] != DBNull.Value ? rw["Email"].ToString() : "";
                    _Dpsuser.PhoneNumber = rw["PhoneNumber"] != DBNull.Value ? rw["PhoneNumber"].ToString() : "";
                    _Dpsuser.IdDonVi = rw["IdDonVi"] != DBNull.Value ? long.Parse(rw["IdDonVi"].ToString()) : 0;
                    _Dpsuser.TenDonVi = rw["DonVi"] != DBNull.Value ? rw["DonVi"].ToString() : "";
                    _Dpsuser.MaDonVi = rw["MaDinhDanh"] != DBNull.Value ? rw["MaDinhDanh"].ToString() : "";
                    _Dpsuser.Active = rw["Active"] != DBNull.Value ? int.Parse(rw["Active"].ToString()) : 0;
                    re.Add(_Dpsuser);
                }
            }
            return re;
        }

        public List<NguoiDungDPS> GetManager(string id_nv, IDbContextTransaction transaction = null)
        {
            string sql = "select id_chucdanh, tbl_chucdanh.id_parent from dps_user inner join tbl_chucdanh on id_chucdanh=id_row where id_nv=" + id_nv;
            DataTable chucdanh = createTable(sql, transaction);
            if (chucdanh.Rows.Count <= 0)
                return null;
            string select = "select id_row, IsStop from tbl_chucdanh where id_row =" + chucdanh.Rows[0][1].ToString();
            DataTable tmp = createTable(select, transaction);
            List<string> ids = new List<string>();
            if ((tmp.Rows.Count <= 0) || (tmp.Rows[0][0].ToString().Equals("0")))
            {
                error_message = "Không tìm thấy quản lý trực tiếp của người này";
                return null;
            }
            else
            {
                string id_manager = "";
                string id_chucdanh = tmp.Rows[0][0].ToString();
                sql = "select id_nv, id_chucdanh, kiemnhiem from dps_user where (deleted=0) and (tamnghi=0) and ((id_chucdanh=" + id_chucdanh + ") or (id_nv in (select id_nv from lslamviec where id_chucdanh=" + id_chucdanh + " and active=1 and disable=0 and hinhthuc=3)))";
                DataTable t = createTable(sql, transaction);
                if (t.Rows.Count == 0 && !bool.TrueString.Equals(tmp.Rows[0][1].ToString()))
                    ids = GetIDManager(id_chucdanh, transaction);
                else
                    ids.AddRange(t.AsEnumerable().Select(x => x[0].ToString()));
            }
            if (ids == null || ids.Count == 0)
            {
                error_message = "Không tìm thấy người quản lý trực tiếp";
                return null;
            }
            return GetById(string.Join(",", ids), transaction);
        }

        /// <summary>
        /// Lấy danh sách tất cả vị trí bên dưới 1 vị trí
        /// </summary>
        /// <param name="Id_parent">vị trí cha</param>
        /// <param name="cnn"></param>
        /// <returns>Collection id_chucdanh</returns>
        public StringCollection GetAllPosition(string Id_parent, IDbContextTransaction transaction = null)
        {
            DataTable dt = createTable("select id_row from tbl_chucdanh where Id_parent=" + Id_parent + " and disable=0", transaction);
            StringCollection id = new StringCollection();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                id.Add(dt.Rows[i][0].ToString());
                StringCollection child = GetAllPosition(dt.Rows[i][0].ToString(), transaction);
                for (int j = 0; j < child.Count; j++)
                {
                    id.Add(child[j]);
                }
            }
            return id;
        }

        public DataTable createTable(string sql, IDbContextTransaction transaction = null)
        {
            DataTable tmp = null;
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                if (transaction != null)
                    command.Transaction = transaction.GetDbTransaction();
                IDbDataAdapter dbDataAdapter = new SqlDataAdapter();
                dbDataAdapter.SelectCommand = command;
                DataSet dataSet = new DataSet();
                dbDataAdapter.Fill(dataSet);
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    tmp = dataSet.Tables[0];
                }
            }
            return tmp;
        }
    }
}