using Timensit_API.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCore_API.Models
{
    public class FileImport
    {
        public long? idLoai { get; set; }
        public string base64 { get; set; }
        public byte[] fileByte { get; set; } = null;
        public string filename { get; set; }
        public string dienGiai { get; set; } = "";
        public string extension { get; set; }
    }

    public class FileImportVanBan
    {
        public string base64 { get; set; }
        public string filename { get; set; }
    }

    public class ImportError
    {
        public int IndexRow { get; set; } = -1;
        public string ModelName { get; set; } = "";
        public string PropertyName { get; set; } = "";
        public string ErrorMeg { get; set; } = "";
    }

    public class DSFileImport
    {
        public List<FileImport> listImport { get; set; }
    }

    public class FileDinhKem
    {
        public long IdFile { get; set; }
        public string FileName { get; set; }
        public string GhiChu { get; set; }
    }

    public class FileUpload : FileImportVanBan
    {
        public string Phonenumber { get; set; } = "84378347714";
        public SignPosition Position { get; set; } = new SignPosition(260, 530, 320, 600);
    }
}
