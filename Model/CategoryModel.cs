using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class CategoryModel
    {
        public int ID { get; set; }

        public int CategoryID { get; set; }

        public string ParentCategory { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public long TimeStamp { get; set; }
    }
}
