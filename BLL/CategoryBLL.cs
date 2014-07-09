using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using Model;

namespace BLL
{
    public class CategoryBLL
    {
        private readonly CategoryDAL _dal = new CategoryDAL();

        public bool Insert(CategoryModel model)
        {
            return _dal.Insert(model);
        }
    }
}
