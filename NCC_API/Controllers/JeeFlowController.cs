using DpsLibs.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Timensit_API.Classes;
using Timensit_API.Controllers.Common;
using Timensit_API.Controllers.Users;
using Timensit_API.Models;
using Timensit_API.Models.Common;
using SignalRChat.Hubs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Timensit_API.Controllers
{
    /// <summary>
    /// Api test jee flow
    /// </summary>
    [Route("api/jee-flow")]
    [ApiController]
    public class JeeFlowController : ControllerBase
    {
        LoginController lc;
        private LiteController liteController;
        private NCCConfig _config;
        private readonly IHostingEnvironment _hostingEnvironment;

        public JeeFlowController(IOptions<NCCConfig> configLogin, IHttpContextAccessor accessor, IHostingEnvironment hostingEnvironment, IHubContext<ThongBaoHub> hub_context)
        {
            liteController = new LiteController(configLogin, accessor, hostingEnvironment, null);
            _hostingEnvironment = hostingEnvironment;
            _config = configLogin.Value;
            lc = new LoginController();
        }

        /// <summary>
        /// Lấy quyền dạng cây
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("tree-quyen")]
        public BaseModel<object> TreeQuyen()
        {
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                string sqlq = @"select * from Dps_RoleGroups where Disabled=0 order by GroupName 
                select* from Dps_Roles where disabled = 0 order by Role";
                DataSet ds = cnn.CreateDataSet(sqlq);
                if (ds == null || ds.Tables.Count == 0)
                    return JsonResultCommon.SQL(cnn.LastError != null ? cnn.LastError.Message : "");
                try
                {
                    var data = new List<object>(){new
                    {
                        Title = "Tất cả",
                        Children = from p in ds.Tables[0].AsEnumerable()
                                   where p["IdParent"]==DBNull.Value
                                   select new
                                   {
                                       Title = p["GroupName"],
                                       Children = from r in ds.Tables[0].AsEnumerable()
                                                  where r["IdParent"].ToString() == p["IdGroup"].ToString()
                                                   select new
                                                   {
                                                       Title = r["GroupName"],
                                                       Children = from c in ds.Tables[1].AsEnumerable()
                                                                  where c["RoleGroup"].ToString() == r["IdGroup"].ToString()
                                                                  select new
                                                                  {
                                                                      Title = c["Role"],
                                                                      RowID=c["IdRole"],
                                                                  },
                                                   }
                                   }
                    } };
                    return JsonResultCommon.ThanhCong(data);
                }
                catch (Exception ex)
                {
                    return JsonResultCommon.Exception(ex, ControllerContext);
                }
            }
        }

        /// <summary>
        /// Lấy ds quyền
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("list-quyen")]
        public BaseModel<object> ListQuyen()
        {

            DataTable Tb = null;
            using (DpsConnection Conn = new DpsConnection(_config.ConnectionString))
            {
                string sqlq = @"select * from Dps_Roles r where r.Disabled=0";
                Tb = Conn.CreateDataTable(sqlq);
                if (Tb == null)
                    return null;
                return JsonResultCommon.ThanhCong(from r in Tb.AsEnumerable()
                                                  select new
                                                  {
                                                      RowID = r["IdRole"],
                                                      Title = r["Role"]
                                                  });
            }
        }
    }
}
