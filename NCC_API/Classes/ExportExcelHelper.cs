using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using OfficeOpenXml.Style.XmlAccess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using Border = DocumentFormat.OpenXml.Spreadsheet.Border;
using Color = DocumentFormat.OpenXml.Spreadsheet.Color;
using Font = DocumentFormat.OpenXml.Spreadsheet.Font;
using FontSize = DocumentFormat.OpenXml.Spreadsheet.FontSize;

namespace Timensit_API
{
    public class ExportExcelHelper
    {
        public ExportExcelHelper()
        {
        }

        public Stylesheet GenerateStylesheet()
        {
            Stylesheet styleSheet = null;


            Fonts fonts = new Fonts(
                new Font( // Index 0 - default
                    new FontSize() { Val = 14 }


                ),
                new Font( // Index 1 - header
                    new FontSize() { Val = 14 },
                    new Color() { Rgb = "FFFFFF" }

                ),
                 new Font( // Index 2 - body
                    new FontSize() { Val = 16 },
                      new Bold()
                ),
                 new Font( // Index 3 - header BÁO CÁO
                    new FontSize() { Val = 20 },
                      new Bold()
                ),
                 new Font( // Index 4 - header TỪ NGÀY - ĐẾN NGÀY
                    new FontSize() { Val = 11 }
                ),
                 new Font( // Index 5 - header table
                    new FontSize() { Val = 12 },
                    new Bold()
                //new FontName() { Val="" }
                ),
                new Font( // Index 5 - header table
                    new FontSize() { Val = 15 },
                    new Bold()
                //new FontName() { Val="" }
                )

            );

            Fills fills = new Fills(
                    new Fill(new PatternFill() { PatternType = PatternValues.None }), // Index 0 - default
                    new Fill(new PatternFill() { PatternType = PatternValues.Gray125 }), // Index 1 - default
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "66666666" } })
                    { PatternType = PatternValues.Solid }),// Index 2 - header
                     new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "6600CCFF" } })
                     { PatternType = PatternValues.Solid })// Index 3 - header table bg
                );

            Borders borders = new Borders(
                    new Border(), // index 0 default
                    new Border( // index 1 black border
                        new LeftBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new RightBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new TopBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new BottomBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new DiagonalBorder())
                );

            CellFormats cellFormats = new CellFormats(
                    new CellFormat { Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } }, // default                    
                    new CellFormat { FontId = 3, FillId = 0, BorderId = 0, ApplyBorder = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } }, // body
                    new CellFormat { FontId = 1, FillId = 2, BorderId = 1, ApplyFill = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } }, // header
                    new CellFormat { FontId = 1, FillId = 2, BorderId = 1, ApplyFill = true, Alignment = new Alignment { WrapText = true } },
                    new CellFormat { FontId = 4, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center } }, // cho cell
                    new CellFormat { FontId = 4, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Right }, }, // cho số tiền
                    new CellFormat { FontId = 5, FillId = 3, BorderId = 1, ApplyBorder = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } }, // body
                    new CellFormat { FontId = 4, FillId = 0, BorderId = 0, ApplyBorder = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } }, // header từ ngày đến ngày
                    new CellFormat { FontId = 3, Alignment = new Alignment { WrapText = false, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } }, // title                    
                    new CellFormat { FontId = 4, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Center } }, // cho cell number
                    new CellFormat { FontId = 4, FillId = 0, BorderId = 0, ApplyBorder = true, Alignment = new Alignment { WrapText = true, Vertical = VerticalAlignmentValues.Center, Horizontal = HorizontalAlignmentValues.Left } } // header canh trái

                );



            styleSheet = new Stylesheet(fonts, fills, borders, cellFormats);

            return styleSheet;
        }
        //public Cell ConstructCell(string value, CellValues dataType, uint styleIndex = 0)
        //{
        //    Cell cell = new Cell();
        //    cell.CellValue = new CellValue(value);
        //    long number = 0;
        //    bool kq = long.TryParse(value, out number);
        //    if (kq)
        //    {
        //        cell.DataType = new EnumValue<CellValues>(CellValues.Number);
        //    }
        //    else
        //    {
        //        cell.DataType = new EnumValue<CellValues>(CellValues.String);
        //    }
        //    cell.DataType = new EnumValue<CellValues>(dataType);
        //    cell.StyleIndex = styleIndex;
        //    return cell;
        //}
        public Cell ConstructCell(string value, CellValues dataType, uint styleIndex = 0)
        {
            return new DocumentFormat.OpenXml.Spreadsheet.Cell()
            {
                CellValue = new CellValue(value),
                DataType = new EnumValue<CellValues>(dataType),
                StyleIndex = styleIndex,
            };
        }

        public static string ExportToExcel(DataTable source, string title, string[] header, string[] columnwidth, string rowheight, string headerheight, Hashtable format, bool stt = true)
        {
            bool ismergeRow = false;
            StringBuilder result = new StringBuilder();
            string border = @"<Borders>
        <Border ss:Position=""Bottom"" ss:LineStyle=""Continuous"" ss:Weight=""1""/>
        <Border ss:Position=""Left"" ss:LineStyle=""Continuous"" ss:Weight=""1""/>
        <Border ss:Position=""Right"" ss:LineStyle=""Continuous"" ss:Weight=""1""/>
        <Border ss:Position=""Top"" ss:LineStyle=""Continuous"" ss:Weight=""1""/>
      </Borders>";
            string numberformat = @"<NumberFormat ss:Format=""_(* #,##0.00_);_(* \(#,##0.00\);_(* &quot;-&quot;??_);_(@_)""/>";
            string startExcelXML = @"<xml version><Workbook 
                          xmlns=""urn:schemas-microsoft-com:office:spreadsheet""
                           xmlns:o=""urn:schemas-microsoft-com:office:office"" 
                          xmlns:x=""urn:schemas-    microsoft-com:office:
                          excel"" xmlns:ss=""urn:schemas-microsoft-com:
                          office:spreadsheet"">
                          <Styles>
                           <Style ss:ID=""Default"" ss:Name=""Normal""> <Alignment ss:Vertical=""Center"" ss:WrapText=""1""/><Font/> <Interior/> <NumberFormat/> <Protection/> </Style>
                           <Style ss:ID=""BoldColumn""><Alignment ss:WrapText=""1""/>  {0} <Font x:Family=""Swiss"" ss:Bold=""1""/> </Style>
                           <Style ss:ID=""StringLiteral""><Alignment ss:WrapText=""1""/>  {0} <NumberFormat ss:Format=""@""/> </Style>
                           <Style ss:ID=""Decimal""><Alignment ss:WrapText=""1""/>  {0} {1} </Style>
                           <Style ss:ID=""Integer""><Alignment ss:WrapText=""1""/>  {0} <NumberFormat ss:Format=""#,##0""/> </Style>
                           <Style ss:ID=""DateLiteral""><Alignment ss:WrapText=""1""/>  {0} <NumberFormat ss:Format=""dd/mm/yyyy;@""/> </Style>
                           <Style ss:ID=""TimeLiteral""> <Alignment ss:WrapText=""1""/>  {0} <NumberFormat ss:Format=""HH:mm;@""/> </Style>
                           <Style ss:ID=""defaultrow""> <Alignment ss:Vertical=""Center"" ss:WrapText=""1""/></Style>
                           <Style ss:ID=""centercolumn""> <Alignment ss:Horizontal=""Center"" ss:Vertical=""Center"" ss:WrapText=""1""/> {0} </Style>
                           <Style ss:ID=""titlecolumn""> <Alignment ss:Horizontal=""Center"" ss:Vertical=""Center"" ss:WrapText=""1""/>  {0} <Font ss:FontName=""Arial"" x:Family=""Swiss"" ss:Size=""16"" ss:Bold=""1""/> </Style>
                           <Style ss:ID=""header""><Alignment ss:Horizontal=""Center"" ss:Vertical=""Center"" ss:WrapText=""1""/> {0} <Font ss:FontName=""Arial"" x:Family=""Swiss"" ss:Color=""#FFFFFF"" ss:Bold=""1""/> <Interior ss:Color=""#8DB4E3"" ss:Pattern=""Solid""/> </Style>
                           <Style ss:ID=""mergerow""><Alignment ss:Horizontal=""Left"" ss:Vertical=""Center"" ss:WrapText=""1""/> {0} <Font ss:FontName=""Arial"" x:Family=""Swiss"" ss:Bold=""1""/> <Interior ss:Color=""#D6DCE4"" ss:Pattern=""Solid""/> </Style>
                           <Style ss:ID=""StringLiteralMerge""><Alignment ss:WrapText=""1""/>  {0} <NumberFormat ss:Format=""@""/> <Interior ss:Color=""#D6DCE4"" ss:Pattern=""Solid""/> </Style>
                           <Style ss:ID=""DecimalMerge""><Alignment ss:WrapText=""1""/>  {0} {1} <Interior ss:Color=""#D6DCE4"" ss:Pattern=""Solid""/> </Style>
                           <Style ss:ID=""IntegerMerge""><Alignment ss:WrapText=""1""/>  {0} <NumberFormat ss:Format=""#,##0""/>  <Interior ss:Color=""#D6DCE4"" ss:Pattern=""Solid""/></Style>
                          </Styles> ";
            startExcelXML = string.Format(startExcelXML, border, numberformat);
            const string endExcelXML = "</Workbook>";
            string ten = "";
            result.Append(startExcelXML);
            result.Append("<Worksheet ss:Name=\"Sheet1\">");
            result.Append("<Table x:FullColumns=\"1\" x:FullRows=\"1\">");
            result.Append("<Column ss:Width=\"25\"/>");
            for (int j = 0; j < columnwidth.Length; j++)
            {
                result.Append("<Column ss:Width=\"" + columnwidth[j] + "\"/>");
            }
            int socot = header.Length;
            if (socot > source.Columns.Count) socot = source.Columns.Count;
            result.Append("<Row>");
            result.Append("<Cell ss:MergeAcross=\"" + socot.ToString() + "\" ss:StyleID=\"titlecolumn\"><Data ss:Type=\"String\">");
            result.Append(title);
            result.Append("</Data></Cell>");
            result.Append("</Row>");
            result.Append("<Row  ss:Height=\"" + headerheight + "\">");

            if (stt)
                result.Append("<Cell ss:StyleID=\"header\"><Data ss:Type=\"String\">STT</Data></Cell>");
            for (int x = 0; x < header.Length; x++)
            {
                result.Append("<Cell ss:StyleID=\"header\"><Data ss:Type=\"String\">");
                result.Append(header[x]);
                result.Append("</Data></Cell>");
            }
            result.Append("</Row>");
            int i = 1;
            if (source.Rows.Count > 0)
            {
                //Kiểm tra cột merge_row có tồn tại
                DataColumnCollection columns = source.Columns;
                if (columns.Contains("merge_row")) ismergeRow = true;
                foreach (DataRow x in source.Rows)
                {
                    bool styleMerge = false;//set style merge cho dòng
                    if (columns.Contains("style_merge"))
                    {
                        if (x["style_merge"] != DBNull.Value)
                        {
                            styleMerge = x["style_merge"].Equals(bool.TrueString);
                            if (!styleMerge)
                                styleMerge = (bool)x["style_merge"];
                        }
                    }
                    bool mergeRow = false;
                    if (ismergeRow)
                    {
                        if (x["merge_row"] != DBNull.Value)
                        {
                            mergeRow = x["merge_row"].Equals(bool.TrueString);
                            if (!mergeRow)
                                mergeRow = (bool)x["merge_row"];
                        }
                    }
                    if (mergeRow)
                    {
                        int from = 0, to = 0;//đánh số từ 0
                        if (columns.Contains("merge_from") && x["merge_from"] != DBNull.Value)
                            from = (Int16)x["merge_from"];
                        if (columns.Contains("merge_to") && x["merge_to"] != DBNull.Value)
                            to = (Int16)x["merge_to"];
                        int merge_col = to - from + 1;
                        if (merge_col == 1)
                            merge_col = socot;
                        result.Append("<Row ss:AutoFitHeight=\"0\" ss:Height=\"" + rowheight + "\" ss:StyleID=\"defaultrow\">");
                        for (var ii = 0; ii < from; ii++)
                            result.Append(genCell(x, ii, styleMerge, format));
                        result.Append("<Cell  ss:MergeAcross=\"" + merge_col.ToString() + "\" ss:StyleID=\"mergerow\">" +
                              "<Data ss:Type=\"String\">");
                        string XMLstring = x["merge_title"].ToString();
                        result.Append(XMLstring);
                        result.Append("</Data></Cell>");
                        if (to > 0)
                        {
                            for (var ii = to; ii < socot; ii++)
                                result.Append(genCell(x, ii, styleMerge, format));
                        }
                        result.Append("</Row>");
                    }
                    else
                    {
                        result.Append("<Row ss:AutoFitHeight=\"0\" ss:Height=\"" + rowheight + "\" ss:StyleID=\"defaultrow\">");
                        if (stt)
                        {
                            result.Append("<Cell ss:StyleID=\"centercolumn\"><Data ss:Type=\"Number\">");
                            result.Append(i.ToString());
                            result.Append("</Data></Cell>");
                            i++;
                        }
                        for (int y = 0; y < socot; y++)
                        {
                            result.Append(genCell(x, y, styleMerge, format));
                        }
                        result.Append("</Row>");
                    }
                }
            }
            result.Append("</Table>");
            result.Append(" </Worksheet>");
            result.Append(endExcelXML);
            return result.ToString();
        }

        private static StringBuilder genCell(DataRow x, int y, bool styleMerge, Hashtable format)
        {
            string template = "";
            IFormatProvider fm = new CultureInfo("en-US", true);
            string StringLiteral = "StringLiteral";
            string Integer = "Integer";
            if (styleMerge)
            {
                StringLiteral = "StringLiteralMerge";
                Integer = "IntegerMerge";
            }
            StringBuilder result = new StringBuilder();
            System.Type rowType;
            rowType = x[y].GetType();
            switch (rowType.ToString())
            {
                case "System.String":
                    string XMLstring = x[y].ToString();
                    XMLstring = XMLstring.Trim();
                    XMLstring = XMLstring.Replace("&", "&");
                    XMLstring = XMLstring.Replace(">", ">");
                    XMLstring = XMLstring.Replace("<", "<");
                    result.Append("<Cell ss:StyleID=\"" + StringLiteral + "\">" +
                          "<Data ss:Type=\"String\">");
                    result.Append(XMLstring);
                    result.Append("</Data></Cell>");
                    break;
                case "System.DateTime":
                    if (x[y] == DBNull.Value)
                        break;
                    DateTime XMLDate = (DateTime)x[y];
                    string XMLDatetoString = ""; //Excel Converted Date
                    XMLDatetoString = XMLDate.Year.ToString() +
                          "-" +
                          (XMLDate.Month < 10 ? "0" +
                          XMLDate.Month.ToString() : XMLDate.Month.ToString()) +
                          "-" +
                          (XMLDate.Day < 10 ? "0" +
                          XMLDate.Day.ToString() : XMLDate.Day.ToString()) +
                          "T" +
                          (XMLDate.Hour < 10 ? "0" +
                          XMLDate.Hour.ToString() : XMLDate.Hour.ToString()) +
                          ":" +
                          (XMLDate.Minute < 10 ? "0" +
                          XMLDate.Minute.ToString() : XMLDate.Minute.ToString()) +
                          ":" +
                          (XMLDate.Second < 10 ? "0" +
                          XMLDate.Second.ToString() : XMLDate.Second.ToString()) + ".000";
                    string f = "DateLiteral";
                    if (format.Contains(y.ToString()))
                        f = format[y.ToString()].ToString();
                    result.Append("<Cell ss:StyleID=\"" + f + "\">" +
                          "<Data ss:Type=\"DateTime\" >");
                    result.Append(XMLDatetoString);
                    result.Append("</Data></Cell>");
                    break;
                case "System.Boolean":
                    result.Append("<Cell ss:StyleID=\"" + StringLiteral + "\">" +
                          "<Data ss:Type=\"String\">");
                    result.Append(x[y].ToString());
                    result.Append("</Data></Cell>");
                    break;
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.Byte":
                    result.Append("<Cell ss:StyleID=\"" + Integer + "\">" +
                          "<Data ss:Type=\"Number\"  >");
                    result.Append(x[y].ToString());
                    result.Append("</Data></Cell>");
                    break;
                case "System.Single":
                case "System.Decimal":
                case "System.Double":
                case "System.float":
                    if (styleMerge)
                        template = @"<Cell ss:StyleID=""DecimalMerge""><Data ss:Type=""Number"">{0}</Data></Cell>";
                    else
                        template = @"<Cell ss:StyleID=""Decimal""><Data ss:Type=""Number"">{0}</Data></Cell>";
                    result.AppendFormat(fm, template, new object[] { x[y] });
                    break;
                case "System.DBNull":
                    result.Append("<Cell ss:StyleID=\"" + StringLiteral + "\">" +
                          "<Data ss:Type=\"String\">");
                    result.Append("</Data></Cell>");
                    break;
                default:
                    throw (new Exception(rowType.ToString() + " not handled."));
            }
            return result;
        }

        public static string ExportToExcel(List<ExportExcelModel> ds)
        {
            StringBuilder result = new StringBuilder();
            string border = @"<Borders>
        <Border ss:Position=""Bottom"" ss:LineStyle=""Continuous"" ss:Weight=""1""/>
        <Border ss:Position=""Left"" ss:LineStyle=""Continuous"" ss:Weight=""1""/>
        <Border ss:Position=""Right"" ss:LineStyle=""Continuous"" ss:Weight=""1""/>
        <Border ss:Position=""Top"" ss:LineStyle=""Continuous"" ss:Weight=""1""/>
      </Borders>";
            string numberformat = @"<NumberFormat ss:Format=""_(* #,##0.00_);_(* \(#,##0.00\);_(* &quot;-&quot;??_);_(@_)""/>";
            string startExcelXML = @"<xml version><Workbook 
                          xmlns=""urn:schemas-microsoft-com:office:spreadsheet""
                           xmlns:o=""urn:schemas-microsoft-com:office:office"" 
                          xmlns:x=""urn:schemas-    microsoft-com:office:
                          excel"" xmlns:ss=""urn:schemas-microsoft-com:
                          office:spreadsheet"">
                          <Styles>
                           <Style ss:ID=""Default"" ss:Name=""Normal""> <Alignment ss:Vertical=""Center"" ss:WrapText=""1""/><Font/> <Interior/> <NumberFormat/> <Protection/> </Style>
                           <Style ss:ID=""BoldColumn""><Alignment ss:WrapText=""1""/>  {0} <Font x:Family=""Swiss"" ss:Bold=""1""/> </Style>
                           <Style ss:ID=""StringLiteral""><Alignment ss:WrapText=""1""/>  {0} <NumberFormat ss:Format=""@""/> </Style>
                           <Style ss:ID=""Decimal""><Alignment ss:WrapText=""1""/>  {0} {1} </Style>
                           <Style ss:ID=""Integer""><Alignment ss:WrapText=""1""/>  {0} <NumberFormat ss:Format=""#,##0""/> </Style>
                           <Style ss:ID=""DateLiteral""><Alignment ss:WrapText=""1""/>  {0} <NumberFormat ss:Format=""dd/mm/yyyy;@""/> </Style>
                           <Style ss:ID=""TimeLiteral""> <Alignment ss:WrapText=""1""/>  {0} <NumberFormat ss:Format=""HH:mm;@""/> </Style>
                           <Style ss:ID=""defaultrow""> <Alignment ss:Vertical=""Center"" ss:WrapText=""1""/></Style>
                           <Style ss:ID=""centercolumn""> <Alignment ss:Horizontal=""Center"" ss:Vertical=""Center"" ss:WrapText=""1""/> {0} </Style>
                           <Style ss:ID=""titlecolumn""> <Alignment ss:Horizontal=""Center"" ss:Vertical=""Center"" ss:WrapText=""1""/>  {0} <Font ss:FontName=""Arial"" x:Family=""Swiss"" ss:Size=""16"" ss:Bold=""1""/> </Style>
                           <Style ss:ID=""header""><Alignment ss:Horizontal=""Center"" ss:Vertical=""Center"" ss:WrapText=""1""/> {0} <Font ss:FontName=""Arial"" x:Family=""Swiss"" ss:Color=""#FFFFFF"" ss:Bold=""1""/> <Interior ss:Color=""#8DB4E3"" ss:Pattern=""Solid""/> </Style>
                           <Style ss:ID=""mergerow""><Alignment ss:Horizontal=""Left"" ss:Vertical=""Center"" ss:WrapText=""1""/> {0} <Font ss:FontName=""Arial"" x:Family=""Swiss"" ss:Bold=""1""/> <Interior ss:Color=""#D6DCE4"" ss:Pattern=""Solid""/> </Style>
                           <Style ss:ID=""StringLiteralMerge""><Alignment ss:WrapText=""1""/>  {0} <NumberFormat ss:Format=""@""/> <Interior ss:Color=""#D6DCE4"" ss:Pattern=""Solid""/> </Style>
                           <Style ss:ID=""DecimalMerge""><Alignment ss:WrapText=""1""/>  {0} {1} <Interior ss:Color=""#D6DCE4"" ss:Pattern=""Solid""/> </Style>
                           <Style ss:ID=""IntegerMerge""><Alignment ss:WrapText=""1""/>  {0} <NumberFormat ss:Format=""#,##0""/>  <Interior ss:Color=""#D6DCE4"" ss:Pattern=""Solid""/></Style>
                          </Styles> ";
            startExcelXML = string.Format(startExcelXML, border, numberformat);
            const string endExcelXML = "</Workbook>";
            string ten = "";
            result.Append(startExcelXML);
            for (int index = 0; index < ds.Count; index++)
            {
                result.Append(genString(ds[index]));
                //new ExportExcelModel(source, title, header, columnwidth, rowheight, headerheight, format, stt, sheet[index])
            }
            result.Append(endExcelXML);
            return result.ToString();
        }

        public static StringBuilder genString(ExportExcelModel data)
        {
            bool ismergeRow = false;
            StringBuilder result = new StringBuilder();

            result.Append("<Worksheet ss:Name=\"" + data.sheetName + "\">");
            result.Append("<Table x:FullColumns=\"1\" x:FullRows=\"1\">");
            result.Append("<Column ss:Width=\"25\"/>");
            for (int j = 0; j < data.columnwidth.Length; j++)
            {
                result.Append("<Column ss:Width=\"" + data.columnwidth[j] + "\"/>");
            }
            int socot = data.header.Length;
            if (socot > data.source.Columns.Count) socot = data.source.Columns.Count;
            foreach (string title in data.titles)
            {
                result.Append("<Row>");
                result.Append("<Cell ss:MergeAcross=\"" + socot.ToString() + "\" ss:StyleID=\"titlecolumn\"><Data ss:Type=\"String\">");
                result.Append(title);
                result.Append("</Data></Cell>");
                result.Append("</Row>");
            }
            result.Append("<Row  ss:Height=\"" + data.headerheight + "\">");

            if (data.stt)
                result.Append("<Cell ss:StyleID=\"header\"><Data ss:Type=\"String\">STT</Data></Cell>");
            for (int x = 0; x < data.header.Length; x++)
            {
                result.Append("<Cell ss:StyleID=\"header\"><Data ss:Type=\"String\">");
                result.Append(data.header[x]);
                result.Append("</Data></Cell>");
            }
            result.Append("</Row>");
            int i = 1;
            if (data.source.Rows.Count > 0)
            {
                string template = "";
                IFormatProvider fm = new CultureInfo("en-US", true);
                //Kiểm tra cột merge_row có tồn tại
                DataColumnCollection columns = data.source.Columns;
                if (columns.Contains("merge_row")) ismergeRow = true;
                foreach (DataRow x in data.source.Rows)
                {
                    bool styleMerge = false;//set style merge cho dòng
                    if (columns.Contains("style_merge"))
                    {
                        if (x["style_merge"] != DBNull.Value)
                        {
                            styleMerge = x["style_merge"].Equals(bool.TrueString);
                            if (!styleMerge)
                                styleMerge = (bool)x["style_merge"];
                        }
                    }
                    bool mergeRow = false;
                    if (ismergeRow)
                    {
                        if (x["merge_row"] != DBNull.Value)
                        {
                            mergeRow = x["merge_row"].Equals(bool.TrueString);
                            if (!mergeRow)
                                mergeRow = (bool)x["merge_row"];
                        }
                    }
                    if (mergeRow)
                    {
                        result.Append("<Row ss:AutoFitHeight=\"0\" ss:Height=\"" + data.rowheight + "\" ss:StyleID=\"defaultrow\">");
                        result.Append("<Cell  ss:MergeAcross=\"" + socot.ToString() + "\" ss:StyleID=\"mergerow\">" +
                              "<Data ss:Type=\"String\">");
                        string XMLstring = x["merge_title"].ToString();
                        result.Append(XMLstring);
                        result.Append("</Data></Cell>");
                        result.Append("</Row>");
                    }
                    else
                    {
                        result.Append("<Row ss:AutoFitHeight=\"0\" ss:Height=\"" + data.rowheight + "\" ss:StyleID=\"defaultrow\">");
                        if (data.stt)
                        {
                            result.Append("<Cell ss:StyleID=\"centercolumn\"><Data ss:Type=\"Number\">");
                            result.Append(i.ToString());
                            result.Append("</Data></Cell>");
                            i++;
                        }
                        string StringLiteral = "StringLiteral";
                        string Integer = "Integer";
                        if (styleMerge)
                        {
                            StringLiteral = "StringLiteralMerge";
                            Integer = "IntegerMerge";
                        }
                        for (int y = 0; y < socot; y++)
                        {
                            System.Type rowType;
                            rowType = x[y].GetType();
                            switch (rowType.ToString())
                            {
                                case "System.String":
                                    string XMLstring = x[y].ToString();
                                    XMLstring = XMLstring.Trim();
                                    XMLstring = XMLstring.Replace("&", "&");
                                    XMLstring = XMLstring.Replace(">", ">");
                                    XMLstring = XMLstring.Replace("<", "<");
                                    result.Append("<Cell ss:StyleID=\"" + StringLiteral + "\">" +
                                          "<Data ss:Type=\"String\">");
                                    result.Append(XMLstring);
                                    result.Append("</Data></Cell>");
                                    break;
                                case "System.DateTime":
                                    if (x[y] == DBNull.Value)
                                        break;
                                    DateTime XMLDate = (DateTime)x[y];
                                    string XMLDatetoString = ""; //Excel Converted Date
                                    XMLDatetoString = XMLDate.Year.ToString() +
                                          "-" +
                                          (XMLDate.Month < 10 ? "0" +
                                          XMLDate.Month.ToString() : XMLDate.Month.ToString()) +
                                          "-" +
                                          (XMLDate.Day < 10 ? "0" +
                                          XMLDate.Day.ToString() : XMLDate.Day.ToString()) +
                                          "T" +
                                          (XMLDate.Hour < 10 ? "0" +
                                          XMLDate.Hour.ToString() : XMLDate.Hour.ToString()) +
                                          ":" +
                                          (XMLDate.Minute < 10 ? "0" +
                                          XMLDate.Minute.ToString() : XMLDate.Minute.ToString()) +
                                          ":" +
                                          (XMLDate.Second < 10 ? "0" +
                                          XMLDate.Second.ToString() : XMLDate.Second.ToString()) + ".000";
                                    string f = "DateLiteral";
                                    if (data.format.Contains(y.ToString()))
                                        f = data.format[y.ToString()].ToString();
                                    result.Append("<Cell ss:StyleID=\"" + f + "\">" +
                                          "<Data ss:Type=\"DateTime\" >");
                                    result.Append(XMLDatetoString);
                                    result.Append("</Data></Cell>");
                                    break;
                                case "System.Boolean":
                                    result.Append("<Cell ss:StyleID=\"" + StringLiteral + "\">" +
                                          "<Data ss:Type=\"String\">");
                                    result.Append(x[y].ToString());
                                    result.Append("</Data></Cell>");
                                    break;
                                case "System.Int16":
                                case "System.Int32":
                                case "System.Int64":
                                case "System.Byte":
                                    result.Append("<Cell ss:StyleID=\"" + Integer + "\">" +
                                          "<Data ss:Type=\"Number\"  >");
                                    result.Append(x[y].ToString());
                                    result.Append("</Data></Cell>");
                                    break;
                                case "System.Single":
                                case "System.Decimal":
                                case "System.Double":
                                case "System.float":
                                    if (styleMerge)
                                        template = @"<Cell ss:StyleID=""DecimalMerge""><Data ss:Type=""Number"">{0}</Data></Cell>";
                                    else
                                        template = @"<Cell ss:StyleID=""Decimal""><Data ss:Type=""Number"">{0}</Data></Cell>";
                                    result.AppendFormat(fm, template, new object[] { x[y] });
                                    break;
                                case "System.DBNull":
                                    result.Append("<Cell ss:StyleID=\"" + StringLiteral + "\">" +
                                          "<Data ss:Type=\"String\">");
                                    result.Append("</Data></Cell>");
                                    break;
                                default:
                                    throw (new Exception(rowType.ToString() + " not handled."));
                            }
                        }
                        result.Append("</Row>");
                    }
                }
            }
            result.Append("</Table>");
            result.Append(" </Worksheet>");
            return result;
        }

        public static byte[] ExportToExcelXLSX(List<ExportExcelModel> datas)
        {//DataTable source, List<string> titles, string[] header, string[] columnwidth, string rowheight, string headerheight, Hashtable format, bool stt = true, Image img = null

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                string _fileName = "";
                package.Workbook.Styles.CreateNamedStyle("BoldColumn").Style.Font.Bold = true;
                package.Workbook.Styles.CreateNamedStyle("StringLiteral").Style.Numberformat.Format = "@";
                package.Workbook.Styles.CreateNamedStyle("Decimal");
                package.Workbook.Styles.CreateNamedStyle("Integer").Style.Numberformat.Format = "#,##0";
                package.Workbook.Styles.CreateNamedStyle("DateLiteral").Style.Numberformat.Format = "dd/mm/yyyy;@";
                package.Workbook.Styles.CreateNamedStyle("TimeLiteral").Style.Numberformat.Format = "HH:mm;@";

                try
                {
                    //var datas = new List<ExportExcelModel>() { new ExportExcelModel(source, titles, header, columnwidth, rowheight, rowheight, format, true, "Danh sách người nhận") };
                    int index = 0;
                    foreach (ExportExcelModel data in datas)
                    {
                        package.Workbook.Worksheets.Add(data.sheetName);
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[index];
                        createSheet(ref worksheet, data);
                        index++;
                    }
                    package.Save();
                    var bytearr = package.GetAsByteArray();
                    return bytearr;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="data"></param>
        public static void createSheet(ref ExcelWorksheet worksheet, ExportExcelModel data)
        {
            bool ismergeRow = false;

            int row = 1;
            int Height = 30;
            int Width = 150;
            //WebClient wc = new WebClient();
            //wc.Proxy = null;
            //byte[] bytes = wc.DownloadData($@"{_query.filter["Image"]}");
            //MemoryStream ms = new MemoryStream(bytes);
            //Image img = Image.FromStream(ms);
            //try
            //{
            //    string pathImg = Path.Combine(_hostingEnvironment.ContentRootPath, "data");
            //    string fileNameImg = pathImg + "/" + loginData.IdCustomer + "/Logo.png";
            //    Image img = Image.FromFile(fileNameImg);
            //    ExcelPicture pic = worksheet.Drawings.AddPicture("Logo", img);
            //    pic.SetPosition(row, 0, 0, 0);
            //    pic.SetSize(Width, Height);
            //}
            //catch
            //{ }
            if (data.img != null)
            {
                ExcelPicture pic = worksheet.Drawings.AddPicture("Logo", data.img);
                pic.SetPosition(row, 0, 0, 0);
                pic.SetSize(Width, Height);
            }
            List<string> headers = data.header.ToList();
            List<string> columnwidths = data.columnwidth.ToList();
            if (data.stt)
            {
                headers.Insert(0, "STT");
                columnwidths.Insert(0, "50");
            }
            for (int i = 0; i < columnwidths.Count; i++)
            {
                var col = worksheet.Column(i + 1);
                double w = double.Parse(columnwidths[i]) / 7.05;
                col.Width = w;

                string f = "";
                System.Type rowType;
                rowType = data.source.Columns[i].DataType;
                switch (rowType.ToString())
                {
                    case "System.String":
                        f = "StringLiteral";
                        break;
                    case "System.DateTime":
                        f = "DateLiteral";
                        break;
                    case "System.Boolean":
                        f = "StringLiteral";
                        break;
                    case "System.Int16":
                    case "System.Int32":
                    case "System.Int64":
                    case "System.Byte":
                        f = "Integer";
                        break;
                    case "System.Single":
                    case "System.Decimal":
                    case "System.Double":
                    case "System.float":
                        f = "Decimal";
                        break;
                    case "System.DBNull":
                        f = "StringLiteral";
                        break;
                    default:
                        throw (new Exception(rowType.ToString() + " not handled."));
                }
                col.StyleName = f;
            }
            int socot = headers.Count;
            foreach (string title in data.titles)
            {
                worksheet.Cells[row, 1].Value = title;
                worksheet.Cells[row, 1, row, socot].Merge = true;
                ExcelRange rg22 = worksheet.Cells[row, 1, row, socot];
                rg22.Style.Font.Bold = true;
                rg22.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rg22.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                row++;
            }
            for (int i = 0; i < headers.Count; i++)
            {
                worksheet.Cells[row, i + 1].Value = headers[i];
                ExcelRange rg22 = worksheet.Cells[row, i + 1];
                rg22.Style.Font.Bold = true;
                rg22.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rg22.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                //worksheet.Column(i + 1).Width = double.Parse(columnwidths[i]);
            }
            ExcelRange rg = worksheet.Cells[row, 1, row, socot];
            rg.Style.Font.Bold = true;
            rg.Style.Fill.PatternType = ExcelFillStyle.Solid;
            rg.Style.Fill.BackgroundColor.SetColor(1, 141, 180, 227);// new ExcelFill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "66666666" } })
                                                                     //{ PatternType = PatternValues.Solid });// Index 2 - header
            row++;
            int startIndexTable = row;
            int index = 1;
            //Kiểm tra cột merge_row có tồn tại
            DataColumnCollection columns = data.source.Columns;
            if (columns.Contains("merge_row")) ismergeRow = true;
            foreach (DataRow x in data.source.Rows)
            {
                bool styleMerge = false;//set style merge cho dòng
                if (columns.Contains("style_merge"))
                {
                    if (x["style_merge"] != DBNull.Value)
                    {
                        styleMerge = x["style_merge"].Equals(bool.TrueString);
                        if (!styleMerge)
                            styleMerge = (bool)x["style_merge"];
                    }
                }
                bool mergeRow = false;
                if (ismergeRow)
                {
                    if (x["merge_row"] != DBNull.Value)
                    {
                        mergeRow = x["merge_row"].Equals(bool.TrueString);
                        if (!mergeRow)
                            mergeRow = (bool)x["merge_row"];
                    }
                }
                if (mergeRow)
                {
                    int from = 0, to = 0;//đánh số từ 0
                    if (columns.Contains("merge_from") && x["merge_from"] != DBNull.Value)
                        from = (Int16)x["merge_from"];
                    if (columns.Contains("merge_to") && x["merge_to"] != DBNull.Value)
                        to = (Int16)x["merge_to"];
                    int merge_col = to - from + 1;
                    if (merge_col == 1)
                        merge_col = socot;
                    for (var ii = 0; ii < from; ii++)
                    {
                        worksheet.Cells[row, ii + 1].Value = x[ii];
                    }
                    worksheet.Cells[row, from + 1].Value = x["merge_title"];
                    worksheet.Cells[row, from + 1, row, merge_col].Merge = true;
                    if (to > 0)
                        for (var ii = to; ii < socot; ii++)
                        {
                            worksheet.Cells[row, ii + 1].Value = x[ii];
                        }
                    ExcelRange rg1 = worksheet.Cells[row, 1, row, socot];
                    rg1.Style.Font.Bold = true;
                    rg1.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rg1.Style.Fill.BackgroundColor.SetColor(1, 214, 220, 228);
                    row++;
                    continue;
                }
                int add = 0;
                for (int y = 0; y < socot; y++)
                {
                    if (data.stt && y == 0)
                    {
                        add = 1;
                        worksheet.Cells[row, y + 1].Value = index++;
                        continue;
                    }
                    if (x[y - add] == DBNull.Value)
                        worksheet.Cells[row, y + 1].Value = "";
                    else
                    {
                        worksheet.Cells[row, y + 1].Value = x[y - add];
                    }
                }
                if (styleMerge)
                {
                    ExcelRange rg1 = worksheet.Cells[row, 1, row, socot];
                    rg1.Style.Font.Bold = true;
                    rg1.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    rg1.Style.Fill.BackgroundColor.SetColor(1, 214, 220, 228);
                }
                row++;
            }

            var modelTable = worksheet.Cells[startIndexTable - 1, 1, row, socot];

            // Assign borders
            modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        }
    }
    public class ExportExcelModel
    {
        public DataTable source { get; set; }
        public List<string> titles { get; set; }
        public string[] header { get; set; }
        public string[] columnwidth { get; set; }
        public string rowheight { get; set; }
        public string headerheight { get; set; }
        public Hashtable format { get; set; }
        public bool stt { get; set; } = true;
        public Image img { get; set; } = null;
        public string sheetName { get; set; } = "Sheet1";
        public ExportExcelModel(DataTable _source, List<string> _titles, string[] _header, string[] _columnwidth, string _rowheight, string _headerheight, Hashtable _format, bool _stt = true, string _sheetName = "Sheet1")
        {
            source = _source;
            titles = _titles;
            header = _header;
            columnwidth = _columnwidth;
            rowheight = _rowheight;
            headerheight = _headerheight;
            format = _format;
            stt = _stt;
            sheetName = _sheetName;
        }
    }
}