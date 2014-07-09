using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using Model;

namespace BLL
{
    public class InitUrlsBLL
    {
        private readonly InitUrlsDAL _dal = new InitUrlsDAL();

        public bool Insert(InitUrlsModel model)
        {
            return _dal.Insert(model);
        }
    }
}
