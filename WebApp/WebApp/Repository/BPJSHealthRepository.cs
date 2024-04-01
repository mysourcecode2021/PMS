using Dapper;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using WebApp.DBContext;
using WebApp.Models;

namespace WebApp.Repository
{
    public class BPJSHealthRepository : DapperContext
    {
        public async Task<PagingModel<List<dynamic>>> GetDataPaging(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Param", d.Param.ToString());
            if (d.ParamHeader != null)
            {
                par.Add("@ParamHeader", d.ParamHeader.ToString());
            }
            par.Add("@PageNumber", d.PageNumber.ToString());
            par.Add("@RowsOfPage", d.RowsOfPage.ToString());
            if (d.Type != null)
            {
                par.Add("@Type", d.Type.ToString());
            }

            var data = await this.GetListPagingAsync<dynamic>("sp_BPJSHealth_Get", par, "SP");
            return data;
        }

        public async Task<List<dynamic>> GetDataByKey(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Param", d.Param.ToString());
            par.Add("@Type", d.Type.ToString());

            var data = await this.GetListAsync<dynamic>("sp_BPJSHealth_Get", par, "SP");
            return data;
        }

        public async Task<List<dynamic>> GetDataExport(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Param", d.Param.ToString());
            if (d.ParamHeader != null)
            {
                par.Add("@ParamHeader", d.ParamHeader.ToString());
            }
            par.Add("@Type", d.Type.ToString());

            var data = await this.GetListAsync<dynamic>("sp_BPJSHealth_Get", par, "SP");
            return data;
        }

        public async Task<List<dynamic>> SaveProcess(dynamic param, string userId)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            if (d.Action.ToString() == "Delete")
            {
                par.Add("@KeyValue", d.KeyValue.ToString());
            }
            else
            {
                par.Add("@NIK", d.NIK.ToString());
                par.Add("@Periode", d.Periode.ToString());
                par.Add("@NoBPJS", d.NoBPJS.ToString());
                par.Add("@BasicSalary", d.BasicSalary.ToString());
                par.Add("@FixAllowance", d.FixAllowance.ToString());
                par.Add("@Salary", d.Salary.ToString());
                par.Add("@Class", d.Class.ToString());
                par.Add("@BaseCalculation", d.BaseCalculation.ToString());
                par.Add("@IuranCompany", d.IuranCompany.ToString());
                par.Add("@IuranEmployee", d.IuranEmployee.ToString());
                par.Add("@IuranBPJS", d.IuranBPJS.ToString());
                par.Add("@UserId", userId);
            }
            par.Add("@Action", d.Action.ToString());

            var data = await this.GetListAsync<dynamic>("sp_BPJSHealth_Process", par, "SP");
            return data;
        }

        public async Task<string> ProcessDetail(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@NIK", d.NIK.ToString());
            par.Add("@Name", d.Name.ToString());
            par.Add("@NoBPJS", d.NoBPJS.ToString());
            par.Add("@DOB", d.DOB.ToString());
            par.Add("@Status", d.Status.ToString());

            var data = await this.GetListAsync<dynamic>("SELECT * FROM BPJSHealthDetail WHERE NIK = '" + d.NIK.ToString() + "' AND Name = '" + d.Name.ToString() + "'", par, "TEXT");
            if (data.Count > 0)
            {
                await this.ExecuteAsync<dynamic>("UPDATE BPJSHealthDetail SET NoBPJS = @NoBPJS, DOB = @DOB, Status = @Status WHERE NIK = '" + d.NIK.ToString() + "' AND Name = '" + d.Name.ToString() + "'", par, "TEXT");
            }
            else
            {
                await this.ExecuteAsync<dynamic>("INSERT INTO BPJSHealthDetail (NIK, Name, NoBPJS, DOB, Status) VALUES (@NIK, @Name, @NoBPJS, @DOB, @Status)", par, "TEXT");
            }

            return "Success";
        }

        public async Task<string> DeleteProcessDetail(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@NIK", d.NIK.ToString());
            par.Add("@NoBPJS", d.NoBPJS.ToString());
            
            await this.ExecuteAsync<dynamic>("DELETE FROM BPJSHealthDetail WHERE NIK = '" + d.NIK.ToString() + "' AND NoBPJS = '" + d.NoBPJS.ToString() + "'", par, "TEXT");
            
            return "Success";
        }

        public async Task<List<dynamic>> GenerateBPJS(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Periode", d.Periode.ToString());
            par.Add("@Act", d.Act.ToString());

            var data = await this.GetListAsync<dynamic>("sp_BPJSHealth_Generate", par, "SP");
            return data;
        }
    }
}
