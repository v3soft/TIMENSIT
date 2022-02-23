using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timensit_API.Models.ExcelTemplate
{
    public class ExcelDto
    {
        public Dictionary<string, string> rowsData
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {"Rows",@" <row spans=""1:4"" s=""5"" customFormat=""1"" x14ac:dyDescent=""0.25"">
                                 {0}                        
                                </row>"
                    }
                    };
            }
        }
        public Dictionary<string, string> rowsTitle
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {"Rows",@" <row  spans=""1:4"" s=""3"" customFormat=""1"" ht=""23.25"" customHeight=""1"" x14ac:dyDescent=""0.25"">
                                 {0}                                 
                                </row>"
                    }
                    };
            }
        }
        public Dictionary<string, string> StyleWord
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {"bold",@"  <r>
                                   <rPr>
                                    <b/>
                                    <sz val=""13""/>
                                    <color theme=""1""/>
                                    <rFont val=""Times New Roman""/>
                                    <family val=""1""/>
                                    <scheme val=""minor""/>
                                  </rPr>
                                    <t xml:space=""preserve"">{0}</t>
                                  </r>"
                    },
                     {"normal",@" <r>
                                  <rPr>
                                    <sz val=""13""/>
                                    <color theme=""1""/>
                                    <rFont val=""Times New Roman""/>
                                    <family val=""1""/>
                                  </rPr>
                                    <t>{0}</t>
                                  </r>"
                    }
                    };
            }
        }
        /// <summary>
        /// rows data van ban den
        /// </summary>
        public Dictionary<string, string> rowsDataBCInSo
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {"Rows",@"<row  spans=""1:8"" s=""5"" customFormat=""1"" x14ac:dyDescent=""0.25"">
                                  <c  s=""7"" t=""n"">
                                    <v>{0}</v>
                                  </c>
                                  <c  s=""7"" t=""str"">
                                    <v>{1}</v>
                                  </c>
                                  <c  s=""7"" t=""n"">
                                    <v>{2}</v>
                                  </c>
                                  <c  s=""7"" t=""str"">
                                    <v>{3}</v>
                                  </c>
                                  <c  s=""7"" t=""str"">
                                    <v>{4}</v>
                                  </c>
                                  <c  s=""7"" t=""str"">
                                    <v>{5}</v>
                                  </c>
                                  <c  s=""7"" t=""str"">
                                    <v>{6}</v>
                                  </c>
                                  <c  s=""7"" t=""str"">
                                    <v>{7}</v>
                                  </c>
                                </row>"
                                                }
                    };
            }
        }
        public Dictionary<string, string> rowsDataBCInSoTitle
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {"Rows",@" <row  spans=""1:8"" s=""5"" customFormat=""1"" x14ac:dyDescent=""0.25"">
                              <c  s=""12"" t=""str"">
                                <v>{0}</v>
                              </c>
                              <c  s=""13""/>
                              <c  s=""14""/>
                              <c  s=""14""/>
                              <c  s=""14""/>
                              <c  s=""14""/>
                              <c s=""14""/>
                              <c  s=""15""/>
                            </row>"
                                         }
                };
            }
        }
        /// <summary>
        /// rows data van ban di
        /// </summary>
        public Dictionary<string, string> rowsDataBCInSoVBDi
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {"Rows",@"<row   spans=""1:10""  x14ac:dyDescent=""0.25"">
                                  <c  s=""14"" t=""n"">
                                    <v>{0}</v>
                                  </c>
                                  <c  s=""14"" t=""n"">
                                    <v>{1}</v>
                                  </c>
                                  <c  s=""14"" t=""str"">
                                    <v>{2}</v>
                                  </c>
                                  <c  s=""14"" t=""str"">
                                    <v>{3}</v>
                                  </c>
                                  <c  s=""14"" t=""str"">
                                    <v>{4}</v>
                                  </c>
                                  <c  s=""14"" t=""str"">
                                    <v>{5}</v>
                                  </c>
                                  <c  s=""14"" t=""str"">
                                    <v>{6}</v>
                                  </c>
                                  <c  s=""14"" t=""str"">
                                    <v>{7}</v>
                                  </c>
                                  <c  s=""14"" t=""str"">
                                    <v>{8}</v>
                                  </c>
                                  <c  s=""14"" t=""str"">
                                    <v>{9}</v>
                                  </c>
                                </row>"
                         }
                    };
            }
        }

    }

    public class WordDto
    {
        public Dictionary<string, string> rowsData
        {
            get
            {
                return new Dictionary<string, string> { { "Rows", @"   <w:tr w:rsidR=""00C57788"" w:rsidRPr=""00C57788"" w:rsidTr=""00EB71B8""> {0} </w:tr>" } };
            }
        }
        public Dictionary<string, string> CellTitle
        {
            get
            {
                return new Dictionary<string, string>
                    {
                        //0: num colspan, 1: text
                        {"colspan",@"   <w:tc>
                                          <w:tcPr>
                                            <w:tcW w:w=""211.25pt"" w:type=""dxa""/>
                                            <w:gridSpan w:val=""{0}""/>
                                            <w:shd w:val=""clear"" w:color=""auto"" w:fill=""D9D9D9"" w:themeFill=""background1"" w:themeFillShade=""D9""/>
                                            <w:vAlign w:val=""center""/>
                                          </w:tcPr>
                                          <w:p w:rsidR=""00C57788"" w:rsidRPr=""00C57788"" w:rsidRDefault=""00C57788"" w:rsidP=""00EB71B8"">
                                            <w:pPr>
                                              <w:jc w:val=""center""/>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                                <w:b/>
                                                <w:sz w:val=""24""/>
                                                <w:szCs w:val=""24""/>
                                              </w:rPr>
                                            </w:pPr>
                                            <w:proofErr w:type=""spellStart""/>
                                            <w:r w:rsidRPr=""00C57788"">
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                                <w:b/>
                                                <w:sz w:val=""24""/>
                                                <w:szCs w:val=""24""/>
                                              </w:rPr>
                                              <w:t>{1}</w:t>
                                            </w:r>
                                            <w:proofErr w:type=""spellEnd""/>
                                          </w:p>
                                        </w:tc>"
                        },
                        //0: text
                     {"normal",@"  <w:tc>
                                      <w:tcPr>
                                        <w:tcW w:w=""256.25pt"" w:type=""dxa""/>
                                        <w:shd w:val=""clear"" w:color=""auto"" w:fill=""D9D9D9"" w:themeFill=""background1"" w:themeFillShade=""D9""/>
                                        <w:vAlign w:val=""center""/>
                                      </w:tcPr>
                                      <w:p w:rsidR=""00C57788"" w:rsidRPr=""00C57788"" w:rsidRDefault=""00C57788"" w:rsidP=""00EB71B8"">
                                        <w:pPr>
                                          <w:jc w:val=""center""/>
                                          <w:rPr>
                                            <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                            <w:b/>
                                            <w:sz w:val=""24""/>
                                            <w:szCs w:val=""24""/>
                                          </w:rPr>
                                        </w:pPr>
                                        <w:r w:rsidRPr=""00C57788"">
                                          <w:rPr>
                                            <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                            <w:b/>
                                            <w:sz w:val=""24""/>
                                            <w:szCs w:val=""24""/>
                                          </w:rPr>
                                          <w:t>{0}</w:t>
                                        </w:r>
                                      </w:p>
                                    </w:tc>"
                    }
                    };
            }
        }
        public Dictionary<string, string> Cell
        {
            get
            {
                return new Dictionary<string, string>
                    {
                        //0: num colspan, 1: ptext
                        {"rowspanStart",@"  <w:tc>
                                              <w:tcPr>
                                                <w:tcW w:w=""155.80pt"" w:type=""dxa""/>
                                                <w:vMerge w:val=""restart""/>
                                                <w:vAlign w:val=""center""/>
                                              </w:tcPr>
                                                {0}
                                            </w:tc>"
                        },
                       {"rowspanEnd",@"  <w:tc>
                                          <w:tcPr>
                                            <w:tcW w:w=""155.80pt"" w:type=""dxa""/>
                                            <w:vMerge/>
                                            <w:vAlign w:val=""center""/>
                                          </w:tcPr>
                                          <w:p w:rsidR=""00C57788"" w:rsidRPr=""00C57788"" w:rsidRDefault=""00C57788"" w:rsidP=""00EB71B8"">
                                            <w:pPr>
                                              <w:jc w:val=""center""/>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                                <w:b/>
                                                <w:sz w:val=""24""/>
                                                <w:szCs w:val=""24""/>
                                              </w:rPr>
                                            </w:pPr>
                                          </w:p>
                                        </w:tc>"
                        },
                        {"alignCenter",@" <w:tc>
                                          <w:tcPr>
                                            <w:tcW w:w=""55.45pt"" w:type=""dxa""/>
                                            <w:vAlign w:val=""center""/>
                                          </w:tcPr>
                                          {0}
                                        </w:tc>"
                        },
                         {"normal",@"<w:tc>
                                      <w:tcPr>
                                        <w:tcW w:w=""256.25pt"" w:type=""dxa""/>
                                        <w:vAlign w:val=""center""/>
                                      </w:tcPr>
                                        {0}
                                    </w:tc>"
                        },
                          {"colspan",@"<w:tc>
                                      <w:tcPr>
                                        <w:tcW w:w=""256.25pt"" w:type=""dxa""/>
                                            <w:gridSpan w:val=""{0}""/>
                                        <w:vAlign w:val=""center""/>
                                      </w:tcPr>
                                        {1}
                                    </w:tc>"
                        },
                    };
            }
        }
        public Dictionary<string, string> StyleWP
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {"bold",@"  <w:p w:rsidR=""00C57788"" w:rsidRPr=""00C57788"" w:rsidRDefault=""00C57788"" w:rsidP=""00EB71B8"">
                                    <w:pPr>
                                      <w:rPr>
                                        <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                        <w:b/>
                                        <w:sz w:val=""24""/>
                                        <w:szCs w:val=""24""/>
                                      </w:rPr>
                                    </w:pPr>
                                        {0}
                                  </w:p>"
                    },
                     {"alignCenter",@" <w:p w:rsidR=""00C57788"" w:rsidRPr=""00C57788"" w:rsidRDefault=""00C57788"" w:rsidP=""00EB71B8"">
                                      <w:pPr>
                                      <w:jc w:val=""center""/>
                                      <w:rPr>
                                        <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                        <w:sz w:val=""24""/>
                                        <w:szCs w:val=""24""/>
                                      </w:rPr>
                                    </w:pPr>
                                        {0} 
                                   </w:p>"
                    },
                      {"normal",@" <w:p w:rsidR=""00C57788"" w:rsidRPr=""00C57788"" w:rsidRDefault=""00C57788"" w:rsidP=""00EB71B8"">
                                    <w:pPr>
                                      <w:rPr>
                                        <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                        <w:bCs/>
                                        <w:color w:val=""212529""/>
                                        <w:sz w:val=""24""/>
                                        <w:szCs w:val=""24""/>
                                      </w:rPr>
                                    </w:pPr>
                                        {0} 
                                   </w:p>"
                    }
                    };
            }
        }
        public Dictionary<string, string> StyleWR
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {"bold",@" <w:r w:rsidRPr=""00C57788"">  
                                    <w:rPr>
                                    <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                    <w:b/>
                                    <w:bCs/>
                                    <w:color w:val=""212529""/>
                                    <w:sz w:val=""24""/>
                                    <w:szCs w:val=""24""/>
                                    </w:rPr>
                                    <w:t xml:space=""preserve"">{0}</w:t>
                                </w:r>"
                    },
                     {"normal",@"<w:r w:rsidRPr=""00C57788"">
                                      <w:rPr>
                                        <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                        <w:bCs/>
                                        <w:color w:val=""212529""/>
                                        <w:sz w:val=""24""/>
                                        <w:szCs w:val=""24""/>
                                      </w:rPr>
                                      <w:t xml:space=""preserve"">{0}</w:t>
                                    </w:r>"
                    },
                     {"separator",@"<w:proofErr w:type=""spellStart""/>
                                        {0}
                                       <w:proofErr w:type=""spellEnd""/>"
                     }
                    };
            }
        }
        public Dictionary<string, string> RowsBCInSoVBDi
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {"Rows",@"       <w:tr w:rsidR=""003975E9"" w:rsidRPr=""001B7F21"" w:rsidTr=""001B7F21"">
                                        <w:tc>
                                          <w:tcPr>
                                            <w:tcW w:w=""40.25pt"" w:type=""dxa""/>
                                          </w:tcPr>
                                          <w:p w:rsidR=""003975E9"" w:rsidRPr=""001B7F21"" w:rsidRDefault=""003975E9"" w:rsidP=""003975E9"">
                                            <w:pPr>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                            </w:pPr>
                                            <w:r>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                              <w:t>{0}</w:t>
                                            </w:r>
                                          </w:p>
                                        </w:tc>
                                      <w:tc>
                                          <w:tcPr>
                                            <w:tcW w:w=""40.25pt"" w:type=""dxa""/>
                                          </w:tcPr>
                                          <w:p w:rsidR=""003975E9"" w:rsidRPr=""001B7F21"" w:rsidRDefault=""003975E9"" w:rsidP=""003975E9"">
                                            <w:pPr>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                            </w:pPr>
                                            <w:r>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                              <w:t>{1}</w:t>
                                            </w:r>
                                          </w:p>
                                        </w:tc><w:tc>
                                          <w:tcPr>
                                            <w:tcW w:w=""40.25pt"" w:type=""dxa""/>
                                          </w:tcPr>
                                          <w:p w:rsidR=""003975E9"" w:rsidRPr=""001B7F21"" w:rsidRDefault=""003975E9"" w:rsidP=""003975E9"">
                                            <w:pPr>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                            </w:pPr>
                                            <w:r>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                              <w:t>{2}</w:t>
                                            </w:r>
                                          </w:p>
                                        </w:tc><w:tc>
                                          <w:tcPr>
                                            <w:tcW w:w=""40.25pt"" w:type=""dxa""/>
                                          </w:tcPr>
                                          <w:p w:rsidR=""003975E9"" w:rsidRPr=""001B7F21"" w:rsidRDefault=""003975E9"" w:rsidP=""003975E9"">
                                            <w:pPr>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                            </w:pPr>
                                            <w:r>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                              <w:t>{3}</w:t>
                                            </w:r>
                                          </w:p>
                                        </w:tc><w:tc>
                                          <w:tcPr>
                                            <w:tcW w:w=""40.25pt"" w:type=""dxa""/>
                                          </w:tcPr>
                                          <w:p w:rsidR=""003975E9"" w:rsidRPr=""001B7F21"" w:rsidRDefault=""003975E9"" w:rsidP=""003975E9"">
                                            <w:pPr>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                            </w:pPr>
                                            <w:r>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                              <w:t>{4}</w:t>
                                            </w:r>
                                          </w:p>
                                        </w:tc><w:tc>
                                          <w:tcPr>
                                            <w:tcW w:w=""40.25pt"" w:type=""dxa""/>
                                          </w:tcPr>
                                          <w:p w:rsidR=""003975E9"" w:rsidRPr=""001B7F21"" w:rsidRDefault=""003975E9"" w:rsidP=""003975E9"">
                                            <w:pPr>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                            </w:pPr>
                                            <w:r>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                              <w:t>{5}</w:t>
                                            </w:r>
                                          </w:p>
                                        </w:tc><w:tc>
                                          <w:tcPr>
                                            <w:tcW w:w=""40.25pt"" w:type=""dxa""/>
                                          </w:tcPr>
                                          <w:p w:rsidR=""003975E9"" w:rsidRPr=""001B7F21"" w:rsidRDefault=""003975E9"" w:rsidP=""003975E9"">
                                            <w:pPr>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                            </w:pPr>
                                            <w:r>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                              <w:t>{6}</w:t>
                                            </w:r>
                                          </w:p>
                                        </w:tc>
		                                <w:tc>
                                          <w:tcPr>
                                            <w:tcW w:w=""40.25pt"" w:type=""dxa""/>
                                          </w:tcPr>
                                          <w:p w:rsidR=""003975E9"" w:rsidRPr=""001B7F21"" w:rsidRDefault=""003975E9"" w:rsidP=""003975E9"">
                                            <w:pPr>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                            </w:pPr>
                                            <w:r>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                              <w:t>{7}</w:t>
                                            </w:r>
                                          </w:p>
                                        </w:tc>		
		                                <w:tc>
                                          <w:tcPr>
                                            <w:tcW w:w=""40.25pt"" w:type=""dxa""/>
                                          </w:tcPr>
                                          <w:p w:rsidR=""003975E9"" w:rsidRPr=""001B7F21"" w:rsidRDefault=""003975E9"" w:rsidP=""003975E9"">
                                            <w:pPr>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                            </w:pPr>
                                            <w:r>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                              <w:t>{8}</w:t>
                                            </w:r>
                                          </w:p>
                                        </w:tc>		

		                                <w:tc>
                                          <w:tcPr>
                                            <w:tcW w:w=""40.25pt"" w:type=""dxa""/>
                                          </w:tcPr>
                                          <w:p w:rsidR=""003975E9"" w:rsidRPr=""001B7F21"" w:rsidRDefault=""003975E9"" w:rsidP=""003975E9"">
                                            <w:pPr>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                            </w:pPr>
                                            <w:r>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                              <w:t>{9}</w:t>
                                            </w:r>
                                          </w:p>
                                        </w:tc>		

                                      </w:tr>
                                "
                    }
                };
            }
        }
        public Dictionary<string, string> RowsBCInSo
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {"Rows",@"       <w:tr w:rsidR=""003975E9"" w:rsidRPr=""001B7F21"" w:rsidTr=""001B7F21"">
                                        <w:tc>
                                          <w:tcPr>
                                            <w:tcW w:w=""40.25pt"" w:type=""dxa""/>
                                          </w:tcPr>
                                          <w:p w:rsidR=""003975E9"" w:rsidRPr=""001B7F21"" w:rsidRDefault=""003975E9"" w:rsidP=""003975E9"">
                                            <w:pPr>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                            </w:pPr>
                                            <w:r>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                              <w:t>{0}</w:t>
                                            </w:r>
                                          </w:p>
                                        </w:tc>
                                      <w:tc>
                                          <w:tcPr>
                                            <w:tcW w:w=""40.25pt"" w:type=""dxa""/>
                                          </w:tcPr>
                                          <w:p w:rsidR=""003975E9"" w:rsidRPr=""001B7F21"" w:rsidRDefault=""003975E9"" w:rsidP=""003975E9"">
                                            <w:pPr>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                            </w:pPr>
                                            <w:r>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                              <w:t>{1}</w:t>
                                            </w:r>
                                          </w:p>
                                        </w:tc><w:tc>
                                          <w:tcPr>
                                            <w:tcW w:w=""40.25pt"" w:type=""dxa""/>
                                          </w:tcPr>
                                          <w:p w:rsidR=""003975E9"" w:rsidRPr=""001B7F21"" w:rsidRDefault=""003975E9"" w:rsidP=""003975E9"">
                                            <w:pPr>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                            </w:pPr>
                                            <w:r>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                              <w:t>{2}</w:t>
                                            </w:r>
                                          </w:p>
                                        </w:tc><w:tc>
                                          <w:tcPr>
                                            <w:tcW w:w=""40.25pt"" w:type=""dxa""/>
                                          </w:tcPr>
                                          <w:p w:rsidR=""003975E9"" w:rsidRPr=""001B7F21"" w:rsidRDefault=""003975E9"" w:rsidP=""003975E9"">
                                            <w:pPr>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                            </w:pPr>
                                            <w:r>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                              <w:t>{3}</w:t>
                                            </w:r>
                                          </w:p>
                                        </w:tc><w:tc>
                                          <w:tcPr>
                                            <w:tcW w:w=""40.25pt"" w:type=""dxa""/>
                                          </w:tcPr>
                                          <w:p w:rsidR=""003975E9"" w:rsidRPr=""001B7F21"" w:rsidRDefault=""003975E9"" w:rsidP=""003975E9"">
                                            <w:pPr>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                            </w:pPr>
                                            <w:r>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                              <w:t>{4}</w:t>
                                            </w:r>
                                          </w:p>
                                        </w:tc><w:tc>
                                          <w:tcPr>
                                            <w:tcW w:w=""40.25pt"" w:type=""dxa""/>
                                          </w:tcPr>
                                          <w:p w:rsidR=""003975E9"" w:rsidRPr=""001B7F21"" w:rsidRDefault=""003975E9"" w:rsidP=""003975E9"">
                                            <w:pPr>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                            </w:pPr>
                                            <w:r>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                              <w:t>{5}</w:t>
                                            </w:r>
                                          </w:p>
                                        </w:tc><w:tc>
                                          <w:tcPr>
                                            <w:tcW w:w=""40.25pt"" w:type=""dxa""/>
                                          </w:tcPr>
                                          <w:p w:rsidR=""003975E9"" w:rsidRPr=""001B7F21"" w:rsidRDefault=""003975E9"" w:rsidP=""003975E9"">
                                            <w:pPr>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                            </w:pPr>
                                            <w:r>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                              <w:t>{6}</w:t>
                                            </w:r>
                                          </w:p>
                                        </w:tc>
		                                <w:tc>
                                          <w:tcPr>
                                            <w:tcW w:w=""40.25pt"" w:type=""dxa""/>
                                          </w:tcPr>
                                          <w:p w:rsidR=""003975E9"" w:rsidRPr=""001B7F21"" w:rsidRDefault=""003975E9"" w:rsidP=""003975E9"">
                                            <w:pPr>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                            </w:pPr>
                                            <w:r>
                                              <w:rPr>
                                                <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                              </w:rPr>
                                              <w:t>{7}</w:t>
                                            </w:r>
                                          </w:p>
                                        </w:tc>		                              
                                      </w:tr>
                                "
                    }
                };
            }
        }

        public Dictionary<string, string> RowsBCInSotitle
        {
            get
            {
                return new Dictionary<string, string>
                {
                        {"Rows",@" <w:tr w:rsidR=""003975E9"" w:rsidRPr=""001B7F21"" w:rsidTr=""00111936"">
                                    <w:tc>
                                      <w:tcPr>
                                        <w:tcW w:w=""508.25pt"" w:type=""dxa""/>
                                        <w:gridSpan w:val=""8""/>
                                      </w:tcPr>
                                      <w:p w:rsidR=""003975E9"" w:rsidRPr=""001B7F21"" w:rsidRDefault=""003975E9"" w:rsidP=""00D807CD"">
                                        <w:pPr>
                                          <w:rPr>
                                            <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                          </w:rPr>
                                        </w:pPr>
                                        <w:r>
                                          <w:rPr>
                                            <w:rFonts w:ascii=""Times New Roman"" w:hAnsi=""Times New Roman"" w:cs=""Times New Roman""/>
                                            <w:b/>
                                          </w:rPr>
                                          <w:t>{0}</w:t>
                                        </w:r>
                                      </w:p>
                                    </w:tc>
                                  </w:tr>"
                    }
                };
            }
        }

        public Dictionary<string, string> RowsBCTongHop
        {
            get
            {
                return new Dictionary<string, string>
                {
                        {"RowsTitle",@"<w:p w:rsidR=""00102FA1"" w:rsidRPr=""00102FA1"" w:rsidRDefault=""00102FA1"" w:rsidP=""00102FA1"">
                                                      <w:pPr>
                                                        <w:jc w:val=""center""/>
                                                        <w:rPr>
                                                          <w:b/>
                                                          <w:sz w:val=""{1}""/>
                                                          <w:szCs w:val=""{1}""/>
                                                        </w:rPr>
                                                      </w:pPr>
                                                      <w:r w:rsidRPr=""00102FA1"">
                                                        <w:rPr>
                                                          <w:b/>
                                                          <w:sz w:val=""{1}""/>
                                                          <w:szCs w:val=""{1}""/>
                                                        </w:rPr>
                                                        <w:t>{0}</w:t>
                                                      </w:r>
                                                    </w:p>"
                    },
                    {"RowsHeader",@"<w:tc>
                                                  <w:tcPr>
                                                    <w:tcW w:w=""35.75pt"" w:type=""dxa""/>
                                                    <w:shd w:val=""clear"" w:color=""auto"" w:fill=""DEEAF6"" w:themeFill=""accent1"" w:themeFillTint=""33""/>
                                                    <w:vAlign w:val=""center""/>
                                                  </w:tcPr>
                                                  <w:p w:rsidR=""00102FA1"" w:rsidRPr=""00102FA1"" w:rsidRDefault=""00102FA1"" w:rsidP=""00455A8D"">
                                                    <w:pPr>
                                                      <w:jc w:val=""center""/>
                                                      <w:rPr>
                                                        <w:b/>
                                                        <w:sz w:val=""24""/>
                                                        <w:szCs w:val=""24""/>
                                                      </w:rPr>
                                                    </w:pPr>
                                                    <w:r w:rsidRPr=""00102FA1"">
                                                      <w:rPr>
                                                        <w:b/>
                                                        <w:sz w:val=""24""/>
                                                        <w:szCs w:val=""24""/>
                                                      </w:rPr>
                                                      <w:t>{0}</w:t>
                                                    </w:r>
                                                  </w:p>
                                                </w:tc>"
                    },
                     {"RowsBlueBold",@"<w:tc>
                                                  <w:tcPr>
                                                    <w:tcW w:w=""35.75pt"" w:type=""dxa""/>
                                                    <w:shd w:val=""clear"" w:color=""auto"" w:fill=""DEEAF6"" w:themeFill=""accent1"" w:themeFillTint=""33""/>                                                  
                                                  </w:tcPr>
                                                  <w:p w:rsidR=""00102FA1"" w:rsidRPr=""00102FA1"" w:rsidRDefault=""00102FA1"" w:rsidP=""00455A8D"">
                                                    <w:pPr>                                                   
                                                      <w:rPr>
                                                        <w:b/>
                                                        <w:sz w:val=""24""/>
                                                        <w:szCs w:val=""24""/>
                                                      </w:rPr>
                                                    </w:pPr>
                                                    <w:r w:rsidRPr=""00102FA1"">
                                                      <w:rPr>
                                                        <w:b/>
                                                        <w:sz w:val=""24""/>
                                                        <w:szCs w:val=""24""/>
                                                      </w:rPr>
                                                      <w:t xml:space=""preserve"">{0}</w:t>
                                                    </w:r>
                                                  </w:p>
                                                </w:tc>"
                    },
                     {"RowsBlueNormal",@"<w:tc>
                                                  <w:tcPr>
                                                    <w:tcW w:w=""35.75pt"" w:type=""dxa""/>
                                                    <w:shd w:val=""clear"" w:color=""auto"" w:fill=""DEEAF6"" w:themeFill=""accent1"" w:themeFillTint=""33""/>                                                    
                                                  </w:tcPr>
                                                  <w:p w:rsidR=""00102FA1"" w:rsidRPr=""00102FA1"" w:rsidRDefault=""00102FA1"" w:rsidP=""00455A8D"">
                                                    <w:pPr>                                                     
                                                      <w:rPr>                                                       
                                                        <w:sz w:val=""24""/>
                                                        <w:szCs w:val=""24""/>
                                                      </w:rPr>
                                                    </w:pPr>
                                                    <w:r w:rsidRPr=""00102FA1"">
                                                      <w:rPr>                                                      
                                                        <w:sz w:val=""24""/>
                                                        <w:szCs w:val=""24""/>
                                                      </w:rPr>
                                                      <w:t xml:space=""preserve"">{0}</w:t>
                                                    </w:r>
                                                  </w:p>
                                                </w:tc>"
                    },
                     {"RowsBold",@"<w:tc>
                                          <w:tcPr>
                                            <w:tcW w:w=""35.75pt"" w:type=""dxa""/>
                                          </w:tcPr>
                                          <w:p w:rsidR=""00102FA1"" w:rsidRPr=""00455A8D"" w:rsidRDefault=""00102FA1"" w:rsidP=""00455A8D"">
                                            <w:pPr>
                                              <w:jc w:val=""{1}""/>
                                              <w:rPr>
                                                <w:b/>
                                                <w:sz w:val=""24""/>
                                                <w:szCs w:val=""24""/>
                                              </w:rPr>
                                            </w:pPr>
                                                 <w:r w:rsidRPr=""00455A8D"">
                                                              <w:rPr>
                                                                <w:b/>
                                                                <w:sz w:val=""24""/>
                                                                <w:szCs w:val=""24""/>
                                                              </w:rPr>
                                                              <w:t xml:space=""preserve"">{0}</w:t>
                                                 </w:r>
                                          </w:p>
                                        </w:tc>"
                    },
                      {"RowsNormal",@"<w:tc>
                                      <w:tcPr>
                                        <w:tcW w:w=""35.75pt"" w:type=""dxa""/>
                                      </w:tcPr>
                                      <w:p w:rsidR=""00102FA1"" w:rsidRDefault=""00455A8D"" w:rsidP=""00455A8D"">
                                        <w:pPr>
                                          <w:jc w:val=""{1}""/>
                                          <w:rPr>
                                            <w:sz w:val=""24""/>
                                            <w:szCs w:val=""24""/>
                                          </w:rPr>
                                        </w:pPr>
                                        <w:r>
                                          <w:rPr>
                                            <w:sz w:val=""24""/>
                                            <w:szCs w:val=""24""/>
                                          </w:rPr>
                                          <w:t xml:space=""preserve"">{0}</w:t>
                                        </w:r>
                                      </w:p>
                                    </w:tc>"
                    }
                };
            }
        }
    }

}
