using ConsoleApp2;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace RedisTest
{
    public class AzureDatabaseAccessLayer
    {

        //SqlConnection con = new SqlConnection(ConsoleApp2.Util.ConnectionString.CName);

        public List<Site> GetSites()
        {
            List<Site> lstSite = new List<Site>();
            using (SqlConnection con = new SqlConnection(ConsoleApp2.Util.ConnectionString.CName))
            {
                SqlCommand cmd = new SqlCommand("sp_sites_get", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Site site = new Site();
                    site.ID = Convert.ToInt32(rdr["ID"]);
                    site.Name = rdr["Name"].ToString();
                    site.Zones = new List<Zone>();                   

                    lstSite.Add(site);
                }
                con.Close();
            }
            return lstSite;
        }

    }
}
