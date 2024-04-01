using Dapper;
using System.Data;
using System.Data.SqlClient;
using WebApp.Models;

namespace WebApp.DBContext
{
    public class DapperContext
    {
        static IConfiguration conf = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build());
        public SqlConnection GetConnection(string db = "")
        {
            string connString = "";
            if (db == "")
            {
                connString = conf["ConnectionStrings:DBContext"].ToString();
            }
            else
            {
                connString = conf["ConnectionStrings:" + db].ToString();
            }

            return new SqlConnection(connString);
        }

        public List<TSource> GetList<TSource>(string query, DynamicParameters param, string cmdType = "", string dbCon = "")
        {
            SqlConnection con = GetConnection(dbCon);
            IEnumerable<TSource> dtList = new List<TSource>();

            try
            {
                con.Open();
                dtList = con.Query<TSource>(query, param, commandType: (cmdType == "SP") ? CommandType.StoredProcedure : CommandType.Text);
            }
            catch
            {
                throw;
            }
            finally
            {
                con.Close();
            }

            return dtList.ToList();
        }

        public async Task<List<TSource>> GetListAsync<TSource>(string query, DynamicParameters param, string cmdType = "", string dbCon = "")
        {
            SqlConnection con = GetConnection(dbCon);
            IEnumerable<TSource> dtList = new List<TSource>();

            try
            {
                con.Open();
                dtList = await con.QueryAsync<TSource>(query, param, commandType: (cmdType == "SP") ? CommandType.StoredProcedure : CommandType.Text);
            }
            catch
            {
                throw;
            }
            finally
            {
                con.Close();
            }

            return dtList.ToList();
        }

        public async Task<int> ExecuteAsync<TSource>(string query, DynamicParameters param, string cmdType = "", string dbCon = "")
        {
            SqlConnection con = GetConnection(dbCon);
            int count = 0;
            try
            {
                con.Open();
                count = await con.ExecuteAsync(query, param, commandType: (cmdType == "SP") ? CommandType.StoredProcedure : CommandType.Text);
            }
            catch
            {
                throw;
            }
            finally
            {
                con.Close();
            }

            return count;
        }

        public async Task<PagingModel<List<TSource>>> GetListPagingAsync<TSource>(string query, DynamicParameters param, string cmdType = "", string dbCon = "")
        {
            SqlConnection con = GetConnection(dbCon);
            PagingModel<List<TSource>> result = null!;
            try
            {
                con.Open();
                var reader = await con.QueryMultipleAsync(query, param, commandType: (cmdType == "SP") ? CommandType.StoredProcedure : CommandType.Text);

                int count = reader.Read<int>().FirstOrDefault();
                List<TSource> dtList = reader.Read<TSource>().ToList();

                var parametersLookup = (SqlMapper.IParameterLookup)param;
                var valPN = parametersLookup["PageNumber"];
                var valPG = parametersLookup["RowsOfPage"];
                int currentPage = (valPN != null) ? int.Parse(valPN.ToString()!) : 1;
                int pageSize = (valPG != null) ? int.Parse(valPG.ToString()!) : 10;
                result = new PagingModel<List<TSource>>(dtList, count, currentPage, pageSize);
            }
            catch
            {
                throw;
            }
            finally
            {
                con.Close();
            }

            return result;
        }
    }
}
