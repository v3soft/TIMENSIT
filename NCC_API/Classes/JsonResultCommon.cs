﻿using Microsoft.AspNetCore.Mvc;
using Timensit_API.Models;
using Timensit_API.Models.Common;
using System;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Timensit_API.Classes
{
    public static class JsonResultCommon
    {
        public static BaseModel<object> KhongTonTai(string name = "")
        {
            return new BaseModel<object>
            {
                status = 0,
                error = new ErrorModel()
                {
                    message = string.IsNullOrEmpty(name) ? "Không tồn tại" : name + " không tồn tại",
                    code = Constant.ERRORDATA
                }
            };
        }
        public static BaseModel<object> Trung(string name, bool allowForce = false)
        {
            return new BaseModel<object>
            {
                status = 0,
                error = new ErrorModel()
                {
                    message = "Thông tin: " + name + " đã tồn tại",
                    code = Constant.ERRORDATA,
                    allowForce = allowForce
                }
            };
        }
        public static BaseModel<object> BatBuoc(string str_required)
        {
            if (!string.IsNullOrEmpty(str_required))
                str_required = str_required.ToLower();
            return new BaseModel<object>()
            {
                status = 0,
                error = new ErrorModel()
                {
                    message = "Thông tin " + str_required + " là bắt buộc",
                    code = Constant.ERRORDATA
                }
            };
        }
        public static BaseModel<object> Custom(string str_custom)
        {
            return new BaseModel<object>()
            {
                status = 0,
                error = new ErrorModel()
                {
                    message = str_custom,
                    code = Constant.ERRORDATA
                }
            };
        }
        public static BaseModel<object> PhanQuyen(string quyen = "")
        {
            return new BaseModel<object>()
            {
                status = 0,
                error = new ErrorModel()
                {
                    message = "Không có quyền thực hiện chức năng " + (quyen == "" ? "này" : quyen),
                    code = Constant.ERRORCODE_ROLE
                }
            };
        }

        public static BaseModel<object> DangNhap()
        {
            return new BaseModel<object>
            {
                status = 0,
                error = new ErrorModel()
                {
                    message = "Phiên đăng nhập hết hiệu lực. Vui lòng đăng nhập lại!",
                    code = Constant.ERRORCODE
                },
            };
        }
        public static BaseModel<object> SQL(string last_error)
        {
            return new BaseModel<object>()
            {
                status = 0,
                error = new ErrorModel()
                {
                    message = "Có gì đó không đúng, vui lòng thử lại sau",
                    error = last_error,
                    code = Constant.ERRORCODE_SQL
                }
            };
        }
        public static BaseModel<object> Exception(Exception last_error, ControllerContext ControllerContext = null)
        {
            string noidung = last_error != null ? last_error.Message : "";
            if (last_error!=null && last_error.Data != null)
            {
                string noidungmail = noidung;
                if (ControllerContext != null)
                    noidungmail += "<br>Tại: " + ControllerContext.ActionDescriptor.ControllerName + "/" + ControllerContext.ActionDescriptor.ActionName;
                if (last_error != null)
                    noidungmail += "<br>Chi tiết:<br>" + last_error.StackTrace;
                AutoSendMail.SendErrorReport(noidungmail);
            }
            return new BaseModel<object>()
            {
                status = 0,
                error = new ErrorModel()
                {
                    message = "Có gì đó không đúng, vui lòng thử lại sau",
                    error = last_error != null ? last_error.Message : "",
                    code = Constant.ERRORCODE_EXCEPTION
                }
            };
        }
        public static BaseModel<object> Exception(string ex)
        {
            return new BaseModel<object>()
            {
                status = 0,
                error = new ErrorModel
                {
                    message = "Có gì đó không đúng, vui lòng thử lại sau",
                    code = Constant.ERRORCODE_EXCEPTION,
                    error = ex
                },
            };
        }
        public static BaseModel<object> PhanTrang()
        {
            return new BaseModel<object>()
            {
                status = 0,
                error = new ErrorModel()
                {
                    message = "Dữ liệu phân trang không đúng",
                    code = Constant.ERRORCODE
                },
            };
        }
        public static BaseModel<object> ThanhCong()
        {
            return new BaseModel<object>
            {
                status = 1,
            };
        }
        public static BaseModel<object> ThanhCong(object data)
        {
            return new BaseModel<object>
            {
                status = 1,
                data = data
            };
        }
        public static BaseModel<object> ThanhCong(object data, PageModel pageModel)
        {
            return new BaseModel<object>
            {
                status = 1,
                data = data,
                page = pageModel
            };
        }
        public static BaseModel<object> ThanhCong(object data, PageModel pageModel, bool Visible)
        {
            return new BaseModel<object>
            {
                status = 1,
                data = data,
                page = pageModel,
                Visible = Visible
            };
        }
        public static BaseModel<object> ThanhCong(object data, object dataExtra, PageModel pageModel)
        {
            return new BaseModel<object>
            {
                status = 1,
                data = data,
                dataExtra = dataExtra,
                page = pageModel
            };
        }
    }
}
