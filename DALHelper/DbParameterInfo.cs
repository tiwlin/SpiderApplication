using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace DALHelper
{
    [Serializable]
    public class DbParameterInfo
    {
        #region Construction
        public DbParameterInfo() { }

        /// <summary>
        /// 暂时由于是sql数据库,故写死(此处为了考虑output参数)
        /// </summary>
		private System.Data.SqlClient.SqlParameter _parm = new System.Data.SqlClient.SqlParameter();

        internal DbParameter Parm
        {
            get
            {
                return _parm;
            }
        }

        public DbParameterInfo(string pName, System.Data.DbType type, object parmValue)
        {
            this.ParameterName = pName;
			this.DbType = type;
			this.IsNullable = true;
			this.Value = parmValue;
		}

        public DbParameterInfo(string pName, System.Data.DbType type, int size, object parmValue)
        {
			this.ParameterName = pName;
			this.DbType = type;
			this.Size = size;
			this.IsNullable = true;
			this.Value = parmValue;
        }

        public DbParameterInfo(string pName, System.Data.DbType type, System.Data.ParameterDirection direction, object parmValue)
        {
			this.ParameterName = pName;
			this.DbType = type;
			this.Direction = direction;
			this.IsNullable = true;
			this.Value = parmValue;
        }

        public DbParameterInfo(string pName, System.Data.DbType type, System.Data.ParameterDirection direction, int size, object parmValue)
        {
			this.ParameterName = pName;
			this.DbType = type;
			this.Direction = direction;
			this.Size = size;
			this.IsNullable = true;
			this.Value = parmValue;
        }
        #endregion

        public System.Data.DbType DbType
        {
            get
            {
                return _parm.DbType;
            }
            set
            {
                _parm.DbType = value;
            }
        }

        public System.Data.ParameterDirection Direction
        {
            get
            {
                return _parm.Direction;
            }
            set
            {
                _parm.Direction = value;
            }
        }

        public string ParameterName
        {
            get
            {
                return _parm.ParameterName;
            }
            set
            {
                _parm.ParameterName = value;
            }
        }
        public int Size
        {
            get
            {
                return _parm.Size;
            }
            set
            {
                _parm.Size = value;
            }
        }
        
        public bool IsNullable
        {
			get{return _parm.IsNullable;}
			set{_parm.IsNullable = value;}
        }

        public object Value
        {
            get
            {
                return _parm.Value ?? DBNull.Value;
            }
            set
            {
                _parm.Value = value ?? DBNull.Value;
            }
        }
    }
}
