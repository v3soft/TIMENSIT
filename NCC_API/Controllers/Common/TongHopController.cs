using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DpsLibs.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Timensit_API.Classes;
using Timensit_API.Controllers.Users;
using Timensit_API.Models;
using Timensit_API.Models.Common;

namespace Timensit_API.Controllers.Common
{
    [Route("api/tong-hop")]
    [ApiController]
    public class TongHopController : ControllerBase
    {
        LoginController lc;
        private NCCConfig _config;
        private readonly IHostingEnvironment _hostingEnvironment;
        public TongHopController(IOptions<NCCConfig> configLogin, IHttpContextAccessor accessor, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _config = configLogin.Value;
            lc = new LoginController();
        }

        [HttpPost]
        public BaseModel<object> Create([FromBody] TongHopModel data)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                if (data.ids == null || data.ids.Count() == 0)
                    return JsonResultCommon.BatBuoc("Đề xuất");
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    cnn.BeginTransaction();
                    if (!create(data, loginData, cnn))
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    cnn.EndTransaction();
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }

        public static bool create(TongHopModel data, LoginData loginData, DpsConnection cnn)
        {
            Hashtable val = new Hashtable();
            val["Loai"] = data.loai;
            val["Note"] = string.IsNullOrEmpty(data.note) ? "" : data.note;
            val["CreatedDate"] = DateTime.Now;
            val["CreatedBy"] = loginData.Id;
            if (cnn.Insert(val, "Tbl_TongHop") <= 0)
                return false;
            data.Id = int.Parse(cnn.ExecuteScalar("select IDENT_CURRENT('Tbl_TongHop') AS Current_Identity; ").ToString());
            Hashtable val1 = new Hashtable();
            val1["Id_TongHop"] = data.Id;
            foreach (var id in data.ids)
            {
                val1["Id_Phieu"] = id;
                if (cnn.Insert(val1, "Tbl_TongHop_Detail") != 1)
                    return false;
            }
            return true;
        }
    }
}
