using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using English_Learning_Website.Models;

namespace English_Learning_Website.Models
{
    public class TestBonus
    {
        public int Question_Code { get; set; }
        public string Question_Content { get; set; }
        public string Question_Type { get; set; }
        public string Answer_Content1 { get; set; }
        public string Answer_Content2 { get; set; }
        public string Answer_Content3 { get; set; }
        public string Answer_Content4 { get; set; }
        public string Answer_Selected { get; set; }
    }
}