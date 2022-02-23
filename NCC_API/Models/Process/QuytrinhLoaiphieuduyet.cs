using System;
using System.Collections.Generic;

namespace Timensit_API.Models.Process
{
    public partial class QuytrinhLoaiphieuduyet
    {
        public QuytrinhLoaiphieuduyet()
        {
            QuytrinhQuatrinhduyet = new HashSet<QuytrinhQuatrinhduyet>();
        }

        public int Rowid { get; set; }
        public string Description { get; set; }
        public string Checkerfield { get; set; }
        public string Validfield { get; set; }
        public string Invalidfield { get; set; }
        public string Checkeddatefield { get; set; }
        public string Checknotefield { get; set; }
        public string Notifymailtitle { get; set; }
        public string Notifymailtemplate { get; set; }
        public string Acceptmailtitle { get; set; }
        public string Rejectmailtitle { get; set; }
        public string Completemailtemplate { get; set; }
        public string Tablename { get; set; }
        public string Primarykeyfield { get; set; }
        public bool? Isupdateprocesstext { get; set; }
        public string Processtextfield { get; set; }
        public string Statusfield { get; set; }
        public int? Processmethod { get; set; }
        public string Senderfield { get; set; }
        public string Dataviewname { get; set; }
        public string PageaddressList { get; set; }
        public string Notifylangkey { get; set; }
        public string Notifyicon { get; set; }
        public string AppaddressList { get; set; }
        public string Appnotifyicon { get; set; }
        public string LsNguoi { get; set; }
        public string LsNgay { get; set; }
        public string LsGhichu { get; set; }
        public string LsTinhtrang { get; set; }
        public string LsKhoangoai { get; set; }
        public string LsTable { get; set; }
        public string Outputmailtitle { get; set; }
        public string Outputmailtemplate { get; set; }
        public bool AllowDevChecker { get; set; }

        public virtual ICollection<QuytrinhQuatrinhduyet> QuytrinhQuatrinhduyet { get; set; }
    }
}
