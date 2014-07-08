using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using Model;

namespace BLL
{
    public class ShopBLL
    {

        private readonly ShopDAL _dal = new ShopDAL();

        public bool Insert(ShopModel model)
        {
            return _dal.Insert(model);
        }
    }
}
