using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Timensit_API.Models
{
    public class BaseModel<T>
    {
        public BaseModel(StateCode code)
        {
            switch (code)
            {
                case StateCode.NoPermit:
                    status = 0;
                    error = new ErrorModel() { message = "Không có quyền truy cập", code = "8" };
                    break;
                case StateCode.CannotGetData:
                    status = 0;
                    error = new ErrorModel() { message = "Lấy dữ liệu thất bại", code = "0" };
                    break;
            }
        }
        public BaseModel()
        {
            error = new ErrorModel();
        }
        //khởi tạo nhanh trả về lỗi
        public BaseModel(string errorMessage)
        {
            status = 0;
            error = new ErrorModel() { message = errorMessage, code = "9" };
        }

        public int status { get; set; }
        public T data { get; set; }
        public T dataExtra { get; set; }
        public T page { get; set; }
        public ErrorModel error { get; set; }
        public Boolean Visible { get; set; }
    }


    public class ErrorModel
    {
        public string message { get; set; }
        public string code { get; set; }

        public string error { get; set; } = "";
        public bool allowForce { get; set; } = false;
    }
    public class ResultModel
    {
        public int status { get; set; }
        public object data { get; set; }

        public ErrorModel error { get; set; }
    }
    public class FileModel
    {
        public string strBase64 { get; set; }
        public string filename { get; set; }
    }
    public class LiteModelT<T>
    {
        public T id { get; set; }
        public string title { get; set; }
        public object disabled { get; set; }
        public object data { get; set; }
        public bool? IsDefault { get; set; }
    }

    public class LiteModel : LiteModelT<long>
    { }

    public class CCLiteModel : LiteModelT<long>
    {
        public int Capcocau { get; set; }
    }
    public class ListImageModel
    {
        public long IdRow { get; set; }
        public string strBase64 { get; set; }
        public string filename { get; set; }
        public string src { get; set; }
        public bool IsAdd { get; set; } = false;
        public bool IsDel { get; set; } = false;
        public bool IsImagePresent { get; set; } = false;
        public string Type { get; set; }
        public long size { get; set; }


    }
    public class ImportModel : ListImageModel
    {
        public bool review { get; set; } = true;
    }
    public class ImportDeXuatModel : ImportModel
    {
        public long Id_DotTangQua { get; set; }
    }
    public class NodeModel
    {
        public string item { get; set; }
        public List<NodeModel> children { get; set; }
        public object data { get; set; }
        public long value { get; set; }
        public bool selected { get; set; }
    }
    public class CheckFKModel
    {
        public string name { get; set; }
        public string TableName { get; set; }
        public string PKColumn { get; set; }
        public long Id { get; set; }
        public string DisabledColumn { get; set; }
        public string LockedColumn { get; set; }
        public string TitleColumn { get; set; }//cột tên theo id
        public string StrWhere { get; set; }//ByQua=0
    }

    public class NewFlowModel
    {
        public string id { get; set; }
        public string key { get; set; }
        public string parent { get; set; }
        /// <summary>
        /// Tên người xử lý
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// Tên đơn vị
        /// </summary>
        public string text2 { get; set; }
        /// <summary>
        /// Hình ảnh đại diện node
        /// </summary>
        public string image { get; set; }
        public string CheckDate { get; set; }
        public string Step { get; set; }
    }
    public class UserFlowModel
    {
        public string id { get; set; }
        public string key { get; set; }
        public string parent { get; set; }
        /// <summary>
        /// Tên người xử lý
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// Tên đơn vị
        /// </summary>
        public string FullNameG { get; set; }
        /// <summary>
        /// Hình ảnh đại diện node
        /// </summary>
        public string DonVi { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DonViG { get; set; }
        public string CheckDate { get; set; }
        public string Step { get; set; }
        public int LoaiXL { get; set; }
        public int Status { get; set; }
    }
    public class ThongKeDasboardModel
    {
        public int Status { get; set; }
        public string Description { get; set; }
        public long Quantity { get; set; }
    }
    public class MenuTabModel
    {
        public long id { get; set; }
        public string title { get; set; }
        public string icon { get; set; }
        public string page { get; set; }
        public long num { get; set; }
        public object exdata { get; set; }
    }

    public class NgayLe
    {
        public DateTime giatri;
        public int songaynghi;
        public bool isdel = false;
    }
}