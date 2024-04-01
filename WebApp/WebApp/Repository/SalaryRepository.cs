using Dapper;
using Newtonsoft.Json;
using WebApp.DBContext;
using WebApp.Models;

namespace WebApp.Repository
{
    public class SalaryRepository : DapperContext
    {
        public async Task<PagingModel<List<dynamic>>> GetDataPaging(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Param", d.Param.ToString());
            par.Add("@PageNumber", d.PageNumber.ToString());
            par.Add("@RowsOfPage", d.RowsOfPage.ToString());

            var data = await this.GetListPagingAsync<dynamic>("sp_Salary_Get", par, "SP");
            return data;
        }

        public async Task<List<dynamic>> GetDataByKey(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Param", d.Param.ToString());
            par.Add("@Type", d.Type.ToString());

            var data = await this.GetListAsync<dynamic>("sp_Salary_Get", par, "SP");
            return data;
        }

        public async Task<List<dynamic>> GetDataExport(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Param", d.Param.ToString());
            par.Add("@Type", d.Type.ToString());

            var data = await this.GetListAsync<dynamic>("sp_Salary_Get", par, "SP");
            return data;
        }

        public async Task<string> SaveProcess(dynamic param, string userId)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var p = d.Param.ToString().Split("|");
            var NIK = p[0].Split(";")[0];
            await this.ExecuteAsync<dynamic>("DELETE FROM Salary WHERE NIK = '" + NIK + "'", null, "TEXT");

            for(var x = 0; x < p.Length; x++)
            {
                var par = new DynamicParameters();
                par.Add("@NIK", NIK);
                par.Add("@ComponentId", p[x].Split(";")[1].Split(" - ")[0]);
                par.Add("@Amount", p[x].Split(";")[2]);
                par.Add("@UserId", userId);
                await this.ExecuteAsync<dynamic>("INSERT INTO Salary (NIK, ComponentId, Amount, CreatedBy, CreatedDate) VALUES (@NIK, @ComponentId, @Amount, @UserId, GETDATE())", par, "TEXT");
            }
            
            return "Success";
        }

        public async Task<List<dynamic>> ReportToBank(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Periode", d.Periode.ToString());

            var data = await this.GetListAsync<dynamic>("sp_ReportToBank", par, "SP");
            return data;
        }
    }
}
