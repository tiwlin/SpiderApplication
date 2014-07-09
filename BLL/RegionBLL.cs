using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using Model;

namespace BLL
{
    public class RegionBLL
    {
        private readonly RegionDAL _dal = new RegionDAL();

        public bool Insert(RegionModel model)
        {
            return _dal.Insert(model);
        }
    }
}
