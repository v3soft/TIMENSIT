using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JeeOfficeAPI.Models
{
    public class DPStep
    {
        public DPStep() { }
        public DPStep(int id, int form, string text)
        {
            IdRow = id;
            ButtonText = text;
            IdForm = form;
        }
        public DPStep(int id, int form, string text, Boolean isComeBack = false, Boolean isLoop = false, Boolean xLKhac = false)
        {
            IdRow = id;
            ButtonText = text;
            IdForm = form;
            IsComeBack = isComeBack;
            IsLoop = isLoop;
            XLKhac = xLKhac;

        }
        public int IdRow { get; set; } = -2;
        public int IdLuong { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ButtonText { get; set; }
        public string BorderColor { get; set; }
        /// <summary>
        /// 1- Theo đơn vị được phân phối, 2 -Theo quyền, 3 - Theo đơn vị+quyền
        /// </summary>
        public int Type { get; set; }
        public int IdQuyen { get; set; }
        public int IdForm { get; set; }
        public int Prior { get; set; }

        /// <summary>
        /// 0: bắt đầu,-1 kết thúc, khác là 1
        /// </summary>
        public int Loai { get; set; } = 1;
        public Boolean IsComeBack { get; set; } = false;
        public Boolean IsLoop { get; set; } = false;/// quy trinh co de quy
        public Boolean XLKhac { get; set; } = false;
    }
}