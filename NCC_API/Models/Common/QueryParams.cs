using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Timensit_API.Models.Common
{
    public class QueryParams
    {
        public bool more { get; set; } = false;
        public int page { get; set; } = 1;
        public int record { get; set; } = 10;
        public string sortOrder { get; set; } = "";
        public string sortField { get; set; } = "";
        public FilterModel filter { get; set; }
        public FilterGroupModel filterGroup { get; set; }
        public QueryParams()
        {
            filter = new FilterModel();
            filterGroup = new FilterGroupModel();
        }
    }
    public class FilterModel
    {
        public string keys { get; set; }
        public string vals { get; set; }
        private Dictionary<string, string> _dic = new Dictionary<string, string>();
        public FilterModel() { keys = vals = ""; }
        public FilterModel(string keys, string vals)
        {
            this.keys = keys;
            this.vals = vals;
            initDictionary();
        }

        private void initDictionary()
        {
            string[] arrKeys = keys.Split('|');
            string[] arrVals = vals.Split('|');
            for (int i = 0; i < arrKeys.Length && i < arrVals.Length; i++)
            {
                _dic.Add(arrKeys[i], arrVals[i]);
            }
        }

        public string this[string key]
        {
            get
            {
                if (keys.Length > 0 && _dic.Count == 0)
                    initDictionary();
                if (_dic.ContainsKey(key))
                    return _dic[key];
                return null;
            }
        }

    }
    public class FilterGroupModel
    {
        public string keys { get; set; }
        public string vals { get; set; }
        private Dictionary<string, string[]> _dic = new Dictionary<string, string[]>();
        public FilterGroupModel() { keys = vals = ""; }
        public FilterGroupModel(string keys, string vals)
        {
            this.keys = keys;
            this.vals = vals;
            initDictionary();
        }

        private void initDictionary()
        {
            if (string.IsNullOrEmpty(keys) || string.IsNullOrEmpty(vals)) return;
            string[] arrKeys = keys.Split('|');
            string[] arrVals = vals.Split('|');
            for (int i = 0; i < arrKeys.Length && i < arrVals.Length; i++)
            {
                var lst_val = arrVals[i].Split(',');
                if (lst_val != null)
                {
                    if (lst_val.Length <= 0 || string.IsNullOrEmpty(lst_val[0])) continue;
                    _dic.Add(arrKeys[i], lst_val);
                }
            }
        }

        public string[] this[string key]
        {
            get
            {
                if (keys.Length > 0 && _dic.Count == 0)
                    initDictionary();
                if (_dic.ContainsKey(key))
                    return _dic[key];
                return null;
            }
        }
    }
    public class PageModel
    {
        public int Page { get; set; } = 1;
        public int AllPage { get; set; } = 0;
        public int Size { get; set; } = 10;
        public int TotalCount { get; set; } = 0;
    }
}