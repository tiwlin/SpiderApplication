using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using System.Data.SqlClient;
using System.Data;
using DBUtility;

namespace DAL
{
    public class ShopDAL
    {
        public bool Insert(ShopModel model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("INSERT INTO Shop ( ");

            strSql.Append("Category,Region,Name,Address,Range,RangeID,DisID,DisName,DPshopID,MapUrl,TrafficInfo,Phone,Latlng,City,Url,CityName,Status,SubwayName,SubwayDis,SubwaySlug,AppointmentDay,ShopID,CreateTime,UpdateTime ");
            strSql.Append(") VALUES (");
            strSql.Append("@Category,@Region,@Name,@Address,@Range,@RangeID,@DisID,@DisName,@DPshopID,@MapUrl,@TrafficInfo,@Phone,@Latlng,@City,@Url,@CityName,@Status,@SubwayName,@SubwayDis,@SubwaySlug,@AppointmentDay,@ShopID,GETDATE(),GETDATE() ");
            strSql.Append(")");
            SqlParameter[] parameters = {
                    new SqlParameter("@Category", SqlDbType.VarChar),
                    new SqlParameter("@Region", SqlDbType.VarChar),
                    new SqlParameter("@Name", SqlDbType.VarChar),
                    new SqlParameter("@Address", SqlDbType.VarChar),
                    new SqlParameter("@Range", SqlDbType.VarChar),
                    new SqlParameter("@RangeID", SqlDbType.Int),
                    new SqlParameter("@DisID", SqlDbType.Int),
                    new SqlParameter("@DisName", SqlDbType.VarChar),
                    new SqlParameter("@DPshopID", SqlDbType.Int),
                    new SqlParameter("@MapUrl", SqlDbType.VarChar),
                    new SqlParameter("@TrafficInfo", SqlDbType.VarChar),
                    new SqlParameter("@Phone", SqlDbType.VarChar),
                    new SqlParameter("@Latlng", SqlDbType.VarChar),
                    new SqlParameter("@City", SqlDbType.Int),
                    new SqlParameter("@Url", SqlDbType.VarChar),
                    new SqlParameter("@CityName", SqlDbType.VarChar),
                    new SqlParameter("@Status", SqlDbType.Bit),
                    new SqlParameter("@SubwayName", SqlDbType.VarChar),
                    new SqlParameter("@SubwaySlug", SqlDbType.VarChar),
                    new SqlParameter("@SubwayDis", SqlDbType.VarChar),
                    new SqlParameter("@AppointmentDay", SqlDbType.VarChar),
                    new SqlParameter("@ShopID", SqlDbType.Int)};
            parameters[0].Value = model.category;
            parameters[1].Value = model.region;
            parameters[2].Value = model.name;
            parameters[3].Value = model.address;
            parameters[4].Value = model.range;
            parameters[5].Value = model.rangeid;
            parameters[6].Value = model.disid;
            parameters[7].Value = model.disname;
            parameters[8].Value = model.dpshopid;
            parameters[9].Value = model.mapurl;
            parameters[10].Value = model.trafficinfo;
            parameters[11].Value = model.phone;
            parameters[12].Value = model.latlng;
            parameters[13].Value = model.city;
            parameters[14].Value = model.url;
            parameters[15].Value = model.cityname;
            parameters[16].Value = model.status;
            parameters[17].Value = model.subwayname;
            parameters[18].Value = model.subwayslug;
            parameters[19].Value = model.subwaydis;
            parameters[20].Value = model.appointmentDay;
            parameters[21].Value = model.ShopID;

            int rows = DBHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
