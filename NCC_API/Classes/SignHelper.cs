extern alias iText;

using DpsLibs.Data;
using iText::iTextSharp.text.pdf;
using Timensit_API.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace Timensit_API.Classes
{
    public class SignHelper
    {
        public static string errorString;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loaiKy">0: không,1 thường, 2 CA, 3 tất cả</param>
        /// <param name="stt">vị trí ký</param>
        /// <param name="total">tổng số chỗ ký</param>
        /// <returns></returns>
        public static async Task<List<SignDoc>> KyAsync(long IdUser, DpsConnection cnn, List<SignDoc> sources, int loaiKy, string stt)
        {
            errorString = "";
            if (loaiKy == 0)
                return null;
            bool re = true;
            List<SignDoc> docs = new List<SignDoc>();
            string sql = "select SIMCA, FullName from Dps_User where UserID=" + IdUser;
            DataTable dt = cnn.CreateDataTable(sql);
            if (dt == null || dt.Rows.Count == 0)
            {
                errorString = "Người dùng chưa thiết lập số Sim CA";
                return null;
            }
            string Phonenumber = dt.Rows[0]["SIMCA"].ToString();
            string FullName = dt.Rows[0]["FullName"].ToString();
            string tick = DateTime.Now.Ticks.ToString();
            foreach (var doc in sources)
            {
                var temp = doc.path.Split(".");
                string path = temp[0] + "_" + tick + "." + temp[1];
                doc.signedPath = path;
                var uri = new System.Uri(doc.signedPath);
                int length = uri.Segments.Length;
                var converted = "/" + uri.Segments[length - 3] + uri.Segments[length - 2] + uri.Segments[length - 1];
                SignPosition pos = getPostion(doc.path, stt);
                if (pos == null)
                {
                    doc.success = false;
                    doc.message = "Không tìm thấy vị trí ký";
                }
                else
                {
                    if (loaiKy == 3 || loaiKy == 1)//ký thường
                    {

                    }
                    if (loaiKy == 3 || loaiKy == 2)//ký CA
                    {
                        var kq = await signbySimCA(Phonenumber, doc, pos);
                        doc.success = kq.success;
                        doc.message = kq.message;
                    }
                }
                if (doc.success && doc.id > 0)//cần lưu file ký vào DB
                {
                    string sqlUp = "Update Tbl_LichSu_Ky set UpdatedDate=getdate(), UpdatedBy=" + IdUser + ", Disabled=0 where Loai=" + doc.loai + " and Id=" + doc.id;
                    if (cnn.ExecuteNonQuery(sqlUp) < 0)
                    {
                        doc.success = false;
                        doc.message = "Không thể lưu kết quả ký";
                    }
                    else
                    {
                        Hashtable val = new Hashtable();
                        val["Loai"] = doc.loai;
                        val["Id"] = doc.id;
                        val["FileKy"] = converted;
                        val["CreatedDate"] = DateTime.Now;
                        val["CreatedBy"] = IdUser;
                        if (cnn.Insert(val, "Tbl_LichSu_Ky") <= 0)
                        {
                            doc.success = false;
                            doc.message = "Không thể lưu kết quả ký";
                        }
                    }
                }
                if (!doc.success)
                    UploadHelper.DeleteFile(doc.signedPath);
                docs.Add(doc);
            }
            //xóa file ký hỏng + update link file ký vào DB

            return docs;
        }

        public static SignPosition getPostion(string src, string stt)
        {
            SignPosition pos = null;
            try
            {
                PdfReader reader = new PdfReader(src);
                int numberOfPages = reader.NumberOfPages;
                for (int i = 1; i <= numberOfPages; i++)
                {
                    PdfDictionary pageDict = reader.GetPageN(i);
                    if (stt == "")
                    {
                        PdfArray columnArray = pageDict.GetAsArray(PdfName.TABLE);
                        //PdfDictionary col = columnArray.GetAsDict(columnArray.Size - 1);
                        //PdfArray stickyRect = col.GetAsArray(PdfName.RECT);
                        ////PdfRectangle stickyRectangle = new PdfRectangle();
                        //pos = new SignPosition(stickyRect.GetAsNumber(0).IntValue, stickyRect.GetAsNumber(1).IntValue,
                        //    stickyRect.GetAsNumber(2).IntValue, stickyRect.GetAsNumber(3).IntValue);
                        return pos;
                    }
                    else
                    {
                        PdfArray annotArray = pageDict.GetAsArray(PdfName.ANNOTS);
                        if (annotArray != null)
                        {
                            for (int ii = 0; ii < annotArray.Size; ii++)
                            {
                                PdfDictionary curAnnot = annotArray.GetAsDict(ii);
                                PdfString name = curAnnot.GetAsString(PdfName.T);
                                PdfString contents = curAnnot.GetAsString(PdfName.CONTENTS);
                                if (contents != null)
                                    if (contents.ToString() == stt)
                                    {
                                        PdfArray stickyRect = curAnnot.GetAsArray(PdfName.RECT);
                                        //PdfRectangle stickyRectangle = new PdfRectangle();
                                        pos = new SignPosition(stickyRect.GetAsNumber(0).IntValue, stickyRect.GetAsNumber(1).IntValue,
                                            stickyRect.GetAsNumber(2).IntValue, stickyRect.GetAsNumber(3).IntValue);
                                        return pos;
                                    }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return pos;
        }

        /// <param name="src">path file chờ ký</param>
        /// <param name="pcb">path file sau khi ký</param>
        public static async Task<SignDoc> signbySimCA(string phonenumber, SignDoc doc, SignPosition pos, string dataDisplay = "")
        {
            try
            {
                Byte[] res = File.ReadAllBytes(doc.path);
                string certViettelCABase64 = "MIIEKDCCAxCgAwIBAgIKYQ4N5gAAAAAAETANBgkqhkiG9w0BAQUFADB+MQswCQYDVQQGEwJWTjEzMDEGA1UEChMqTWluaXN0cnkgb2YgSW5mb3JtYXRpb24gYW5kIENvbW11bmljYXRpb25zMRswGQYDVQQLExJOYXRpb25hbCBDQSBDZW50ZXIxHTAbBgNVBAMTFE1JQyBOYXRpb25hbCBSb290IENBMB4XDTE1MTAwMjAyMzIyMFoXDTIwMTAwMjAyNDIyMFowOjELMAkGA1UEBhMCVk4xFjAUBgNVBAoTDVZpZXR0ZWwgR3JvdXAxEzARBgNVBAMTClZpZXR0ZWwtQ0EwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDLdiGZcPhwSm67IiLUWELaaol8kHF+qHPmEdcG0VDKf0FtpSWiE/t6NPzqqmoF4gbIrue1/TzUs7ZeAj28o6Lb2BllA/zB6YFrXfppD4jKqHMO139970MeTbDrhHTbVugX4t2QHS+B/p8+8lszJpuduBrnZ/LWxbhnjeQRr21g89nh/W5q1VbIvZnq4ci5m0aDiJ8arhK2CKpvNDWWQ5E0L7NTVoot8niv6/Wjz19yvUCYOKHYsq97y7eBaSYmpgJosD1VtnXqLG7x4POdb6Q073eWXQB0Sj1qJPrXtOqWsnnmzbbKMrnjsoE4gg9B6qLyQS4kRMp0RrUV0z041aUFAgMBAAGjgeswgegwCwYDVR0PBAQDAgGGMBIGA1UdEwEB/wQIMAYBAf8CAQAwHQYDVR0OBBYEFAhg5h8bFNlIgAtep1xzJSwgDfnWMB8GA1UdIwQYMBaAFM1iceRhvf497LJAYNOBdd06rGvGMDwGA1UdHwQ1MDMwMaAvoC2GK2h0dHA6Ly9wdWJsaWMucm9vdGNhLmdvdi52bi9jcmwvbWljbnJjYS5jcmwwRwYIKwYBBQUHAQEEOzA5MDcGCCsGAQUFBzAChitodHRwOi8vcHVibGljLnJvb3RjYS5nb3Yudm4vY3J0L21pY25yY2EuY3J0MA0GCSqGSIb3DQEBBQUAA4IBAQCHtdHJXudu6HjO0571g9RmCP4b/vhK2vHNihDhWYQFuFqBymCota0kMW871sFFSlbd8xD0OWlFGUIkuMCz48WYXEOeXkju1fXYoTnzm5K4L3DV7jQa2H3wQ3VMjP4mgwPHjgciMmPkaBAR/hYyfY77I4NrB3V1KVNsznYbzbFtBO2VV77s3Jt9elzQw21bPDoXaUpfxIde+bLwPxzaEpe7KJhViBccJlAlI7pireTvgLQCBzepJJRerfp+GHj4Z6T58q+e3a9YhyZdtAHVisWYQ4mY113K1V7Z4D7gisjbxExF4UyrX5G4W0h0gXAR5UVOstv5czQyDraTmUTYtx5J";
                string priKey = "MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBAI1ZGghrDQryVQcDDe1gpJ1TQvRWZz0P59/04LlMOdcf0G7XGHy7tzmWffdKgVmlkTbw0tMGa37lg7Suxrm9JpmhcEQmTMo6L06SEmBYSiZfPWMyDnjdWhE8mPMP3ju3xx24UVXcpSclykQflTBr42yZgqsgM19ndsDFkwtz2uWDAgMBAAECgYAJzOPBMar111eN5OhSTSEcx2kdB+Cgmzm4jYIHVwGrqMkK5l8MRvetRoH1Y3UUgiZPaOM1Pny1j7RSEsw0lKjYY4Jawm5n7js13VkIs9tO8HhK00Oo/7a6ZRxAbczpfvGHmMdwaUQgHSGngzE7T3D8Eh4xx3Qu6fmTAIeKPNSMAQJBANfPb9gDkWIsQ/16siOQaTEfacASx/2MvucfrQ2WYGWbG1xNVfA1hkC2tmRRu3SRJp/1lhlERTvOSac4m9IBMasCQQCnq75nAlQTU+/1GvH8nLyEPrCudn40jMCKSEkMWJKKVuiKCrF2GJCZQipNs1DfMSyPggux3Z3hQ62JBuZfNvOJAkEAxIYCM5QMMHpe79Vrozc+k50nj+GKfTpOHeqajGUEI4K7x7IlMDmNqCC6t2A2dFA5/DCIHzosUeno6H6EZxjvQQJAY+IStgiUD0OEge4AU+0G/HzgAb5C5okmtfnj0j/9Y/3r3zgJiYGOuk3JJ6p3tc30brUYxGdyAtyvRx7eI8B3iQJBAJpa4qW6sJ36AKZFLq4D6EwaL2G3kc1bVFSwgRB0TFMB3Vak4O4mu1HWfgCWo20RvJCfcYCrIEdguvd3IunQ9Mc=";//Khai bao gia tri Prikey 
                string apID = "AP2";//Khai bao AppId

                MobileCA mobileCA = new MobileCA();
                var transInfo = await mobileCA.Signature(apID, phonenumber, priKey);

                var certParser = new iText.Org.BouncyCastle.X509.X509CertificateParser();
                iText.Org.BouncyCastle.X509.X509Certificate x509Cert = certParser.ReadCertificate(Convert.FromBase64String(transInfo.certList[0]));

                iText.Org.BouncyCastle.X509.X509Certificate[] certChain = null;

                byte[] bytes = Convert.FromBase64String(certViettelCABase64);

                X509Certificate2 a = new X509Certificate2(bytes);//new iText.Org.BouncyCastle.X509.X509Certificate; CertUtils.GetX509Cert(certViettelCABase64);//x509Cert;
                //iText.Org.BouncyCastle.X509.X509Certificate certViettelCA = ; //new Org.BouncyCastle.X509.X509Certificate(a);
                var certViettelCA = new iText.Org.BouncyCastle.X509.X509CertificateParser().ReadCertificate(a.GetRawCertData());

                if (certViettelCA != null)
                {
                    certChain = new iText.Org.BouncyCastle.X509.X509Certificate[] { x509Cert, certViettelCA };
                }
                // Set parameters
                PdfSignerSynchronous signer = new PdfSignerSynchronous();
                signer.SigTextFormat = PdfSignerSynchronous.FORMAT_TEXT_4;
                //signer.SigContact = UtilSigner.GetCNFromDN(x509Cert.SubjectDN.ToString());


                //tọa độ này sẽ nằm ở góc trái dưới trang A4
                //"OriginX": 50,
                //"OriginY": 50,
                //"CoordinateX": 0,
                //"CoordinateY": 0
                signer.SigLocation = "Hanoi";
                signer.IsMultiSignatures = true;
                signer.Visible = true;
                signer.OriginX = pos.OriginX;//260;
                signer.OriginY = pos.OriginY;//530;
                signer.CoordinateX = pos.CoordinateX;//320;
                signer.CoordinateY = pos.CoordinateY;// 600;
                signer.TsaClient = null;
                signer.UseTSA = false;
                DateTime signDate = DateTime.Now;

                //string dataDisplay = "Test chu ky so";
                FileStream signedPdf = new FileStream(doc.signedPath, FileMode.Create);
                byte[] hash = signer.CreateHash(res, signedPdf, certChain, signDate);
                string base64Hash = "";
                using (SHA1Managed sha1 = new SHA1Managed())
                {
                    base64Hash = Convert.ToBase64String(sha1.ComputeHash(hash));
                }
                string signature = await mobileCA.signSynchronouswithPrikey(base64Hash, apID, phonenumber, dataDisplay, priKey);
                if (signature == null)
                {
                    doc.message = "Không nhận được phản hồi";
                    doc.success = false;
                }
                else
                {
                    if (!signer.InsertSignature(Encoding.ASCII.GetBytes(signature)))
                    {
                        doc.message = "Không thể ký vào văn bản";
                        doc.success = false;
                    }
                    doc.success = true;
                }
                signedPdf.Close();
            }
            catch (Exception ex)
            {
                doc.message = ex.Message.ToString();
                doc.success = false;
            }
            return doc;
        }
    }
    public class SignPosition
    {
        public SignPosition()
        {
        }
        public SignPosition(int x1, int y1, int x2, int y2)
        {
            OriginX = x1;
            OriginY = y1;
            CoordinateX = x2;
            CoordinateY = y2;
        }
        public int OriginX { get; set; } = 260;
        public int OriginY { get; set; } = 530;
        public int CoordinateX { get; set; } = 320;
        public int CoordinateY { get; set; } = 600;
    }

    public class SignDoc
    {
        public long id { get; set; }
        /// <summary>
        /// 0; file đính kèm, 1: DF_Process_Detail
        /// </summary>
        public int loai { get; set; }
        public string path { get; set; }
        public string signedPath { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
    }
}
