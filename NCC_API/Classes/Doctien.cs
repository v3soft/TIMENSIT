
using System;
using System.Collections.Generic;
using System.Text;

namespace DPSLib.Doctien
{
    public static class Doctien
    {
        public static string Convert_NumtoText(string Sonhap)
        {
            bool soam = false;
            string result = "";
            if (Sonhap.StartsWith("-"))
            {
                soam = true;
                Sonhap = Sonhap.Replace("-", "");
            }
            switch (Sonhap.Length)
            {
                case 0:
                    result = "";
                    break;
                case 1:
                    result = Sodonvi(Sonhap);
                    break;
                case 2:
                    {

                        result = Sohangchuc(Sonhap);

                    }
                    break;
                case 3:
                    {
                        result = Sohangtram(Sonhap);
                    }
                    break;
                case 4:
                    {
                        result = Sohangngan(Sonhap);

                    }
                    break;
                case 5:
                    {
                        if (Sonhap.Substring(2, 3).Equals("000"))
                        {
                            result = Sohangchuc(Sonhap.Substring(0, 2)) + " nghìn ";
                        }
                        else
                            result = Sohangchuc(Sonhap.Substring(0, 2)) + " nghìn " + Sohangtram(Sonhap.Substring(2, 3));
                    }
                    break;
                case 6:
                    {
                        if (Sonhap.Substring(3, 3).Equals("000"))
                        {
                            result = Sohangtram(Sonhap.Substring(0, 3)) + " nghìn ";
                        }
                        else
                            result = Sohangtram(Sonhap.Substring(0, 3)) + " nghìn " + Sohangtram(Sonhap.Substring(3, 3));

                    }
                    break;
                case 7:
                    {
                        //if (Sonhap.Substring(3, 4).Equals("0000"))
                        //{
                        //    result = Sodonvi(Sonhap.Substring(0, 1)) + " triệu ";
                        //}
                        //else
                        result = Sodonvi(Sonhap.Substring(0, 1)) + " triệu " + Sohangtram(Sonhap.Substring(1, 3)) + " nghìn " + Sohangtram(Sonhap.Substring(4, 3));
                    }
                    break;
                case 8:
                    {
                        if (Sonhap.Substring(2, 6).Equals("000000"))
                        {
                            result = Sohangchuc(Sonhap.Substring(0, 2)) + " triệu ";
                        }
                        else
                            result = Sohangchuc(Sonhap.Substring(0, 2)) + " triệu " + Sohangtram(Sonhap.Substring(2, 3)) +
                                " nghìn " + Sohangtram(Sonhap.Substring(5, 3));
                    }
                    break;
                case 9:
                    {
                        if (Sonhap.Substring(3, 6).Equals("000000"))
                        {
                            result = Sohangtram(Sonhap.Substring(0, 3)) + " triệu ";
                        }
                        else
                            result = Sohangtram(Sonhap.Substring(0, 3)) + " triệu " + Sohangtram(Sonhap.Substring(3, 3)) +
                                " nghìn " + Sohangtram(Sonhap.Substring(6, 3));
                    }
                    break;
                case 10:
                    {
                        if (Sonhap.Substring(1, 9).Equals("000000000"))
                        {
                            result = Sodonvi(Sonhap.Substring(0, 1)) + " tỷ ";
                        }
                        else
                            result = Sodonvi(Sonhap.Substring(0, 1)) + " tỷ " + Sohangtram(Sonhap.Substring(1, 3)) + " triệu " + Sohangtram(Sonhap.Substring(4, 3)) +
                                " nghìn " + Sohangtram(Sonhap.Substring(7, 3));
                    }
                    break;
                case 11:
                    {
                        if (Sonhap.Substring(2, 9).Equals("000000000"))
                        {
                            result = Sohangchuc(Sonhap.Substring(0, 2)) + " tỷ ";
                        }
                        else
                            result = Sohangchuc(Sonhap.Substring(0, 2)) + " tỷ " + Sohangtram(Sonhap.Substring(2, 3)) + " triệu " + Sohangtram(Sonhap.Substring(5, 3)) +
                                " nghìn " + Sohangtram(Sonhap.Substring(6, 3));
                    }
                    break;
                case 12:
                    {
                        if (Sonhap.Substring(3, 9).Equals("000000000"))
                        {
                            result = Sohangtram(Sonhap.Substring(0, 3)) + " tỷ ";
                        }
                        else
                            result = Sohangtram(Sonhap.Substring(0, 3)) + " tỷ " + Sohangtram(Sonhap.Substring(3, 3)) + " triệu " + Sohangtram(Sonhap.Substring(6, 3)) +
                                " nghìn " + Sohangtram(Sonhap.Substring(9, 3));
                    }
                    break;

            }
            int vt = result.LastIndexOf("không trăm ");
            if (result.Length >= 12 && vt == result.Length - 12)
                result = result.Substring(0, vt);
            result = result.Replace("  ", " ").Replace("mươi một", "mươi mốt").Replace("mươi năm", "mươi lăm").Replace("mười năm", "mười lăm").TrimEnd();
            if (soam)
                result = "Âm " + result;
            return result;
        }
        private static string ChuanHoa(string st)
        {
            string st1 = "";
            return st1.Trim();
        }
        static string Sohangngan(string So)
        {
            string result = "";
            if (So.Equals("1000"))
                result = " một nghìn ";
            else
            {
                result = Sodonvi(So.Substring(0, 1)) + " nghìn ";
                if (So.Substring(1, 1).Equals("0"))
                {

                    if (So.Substring(2, 1).Equals("0"))
                    {
                        if (!So.Substring(3, 1).Equals("0"))
                        {
                            result += Sodonvi(So.Substring(1, 1)) + " trăm ";
                            result += " lẻ " + Sodonvi(So.Substring(So.Length - 1));
                        }
                    }
                    else
                    {
                        result += Sodonvi(So.Substring(1, 1)) + " trăm ";
                        result += Sohangchuc(So.Substring(2, 2));
                    }
                }
                else
                    result += Sohangtram(So.Substring(1, 3));
            }
            return result;


        }
        static string Sohangtram(string So)
        {
            string result = "";
            if (So.Equals("100"))
                result = " một trăm ";
            else
            {
                result += Sodonvi(So.Substring(0, 1)) + " trăm ";
                if (So.Substring(1, 1).Equals("0"))
                {
                    if (!So.Substring(2, 1).Equals("0"))
                        result += " lẻ " + Sodonvi(So.Substring(2, 1));
                }
                else
                    result += Sohangchuc(So.Substring(1, 2));
            }
            return result;

        }
        static string Sohangchuc(string So)
        {
            string result = "";
            if (So.Equals("10"))
                result = " mười ";
            else
            {
                if (So.Substring(0, 1).Equals("1"))
                    result += " mười " + Sodonvi(So.Substring(1, 1));
                else
                {
                    result += Sodonvi(So.Substring(0, 1)) + " mươi ";
                    if (!So.Substring(1, 1).Equals("0"))
                        result += Sodonvi(So.Substring(1, 1));


                }
            }
            return result;
        }
        static string Sodonvi(string So)
        {
            string result = "";
            switch (So)
            {
                case "0":
                    result += " không ";
                    break;
                case "1":
                    result += " một ";
                    break;
                case "2":
                    result += " hai ";
                    break;
                case "3":
                    result += " ba  ";
                    break;
                case "4":
                    result += " bốn ";
                    break;
                case "5":
                    result += " năm ";
                    break;
                case "6":
                    result += " sáu ";
                    break;
                case "7":
                    result += " bảy ";
                    break;
                case "8":
                    result += " tám ";
                    break;
                case "9":
                    result += " chín ";
                    break;

            }
            return result;
        }
    }
}
