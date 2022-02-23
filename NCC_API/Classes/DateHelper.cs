using DpsLibs.Data;
using Timensit_API.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Timensit_API.Classes
{
    public class DateHelper<T>
    {
        public static bool ChuyenNgay(out DateTime kq, string ngay_ddMMyyy)
        {
            kq = new DateTime();
            try
            {
                bool rs = DateTime.TryParseExact(ngay_ddMMyyy, new string[] { "yyyy-MM-dd", "yyyy-M-d", "yyyy-MM-dd HH:mm:ss", "yyyy-M-d HH:mm:ss" }, CultureInfo.CurrentCulture, DateTimeStyles.None, out kq);
                return rs;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static IEnumerable<T> sortListByStringDate(IEnumerable<T> data, string sortOrder, string sortField)
        {
            if ("asc".Equals(sortOrder))
                return data.OrderBy(x =>
               {
                   var value = x.GetType().GetProperty(sortField).GetValue(x, null);
                   if (value == null) return DateTime.MinValue;
                   DateTime dt;
                   if (!DateTime.TryParseExact(value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)) return DateTime.MinValue;
                   return dt;
               });
            else
                return data.OrderByDescending(x =>
                {
                    var value = x.GetType().GetProperty(sortField).GetValue(x, null);
                    if (value == null) return DateTime.MinValue;
                    DateTime dt;
                    if (!DateTime.TryParseExact(value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)) return DateTime.MinValue;
                    return dt;
                });
        }

        public static IEnumerable<DateTime> getDaysBetween(DateTime start, DateTime end)
        {
            for (DateTime i = start; i < end; i = i.AddDays(1))
            {
                yield return i;
            }
        }

        public static List<NgayLe> getNgaysLe(int nam, DpsConnection cnn)
        {
            string sqlq = @"select * from Tbl_NgayLe";
            DataTable dt = cnn.CreateDataTable(sqlq);
            List<NgayLe> ngayles = new List<NgayLe>();
            if(cnn.LastError != null || dt == null)
            {
                return ngayles;
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                int ngay = int.Parse(row["ngay"].ToString());
                int thang = int.Parse(row["thang"].ToString());
                DateTime date = new DateTime(nam, thang, ngay);
                if (row["loai"].ToString().Equals("1")) //lễ ta
                { 
                    int nhuan = int.Parse(row["isnhuan"].ToString());
                    int nam2 = int.Parse(row["istrunam"].ToString()) == 1 ? nam - 1 : nam;
                    int[] ngay1 = ConvertCalendar.convertLunar2Solar(ngay, thang, nam2, nhuan);
                    date = new DateTime(ngay1[2], ngay1[1], ngay1[0]);
                }
                NgayLe n = new NgayLe
                {
                    giatri = date,
                    songaynghi = int.Parse(row["songaynghi"].ToString())
                };
                ngayles.Add(n);
            }
            return ngayles;
        }

        public static DateTime calDeadlineWithoutHoliday(DateTime beginDate, DateTime endDate, List<NgayLe> holidays)
        {
            var num = 0;
            foreach(var h in holidays)
            {
                if(beginDate.Date < h.giatri.Date && h.giatri.Date <= beginDate.Date)
                {
                    num += h.songaynghi;
                    h.isdel = true;
                }
            }
            if (num == 0) //đk dừng
                return endDate;

            var new_hol = holidays.Where(x => !x.isdel).ToList(); //bỏ các ngày lễ đã xét rồi
            return calDeadlineWithoutHoliday(beginDate, endDate.AddDays(num), new_hol);
        }

        public static DateTime getDeadline(DateTime beginDate, DateTime endDate, DpsConnection cnn, int bd_lam, int kt_lam)
        {
            DateTime deadline =  endDate;
            var ngayles = getNgaysLe(beginDate.Year, cnn);
            if(ngayles.Count > 0)
                deadline = calDeadlineWithoutHoliday(beginDate, deadline, ngayles);

            var weekends = getDaysBetween(beginDate, deadline).Where(d => (int)d.DayOfWeek < bd_lam || (int)d.DayOfWeek > kt_lam);
            
            return deadline.AddDays(weekends.Count());
        }
    }
}
