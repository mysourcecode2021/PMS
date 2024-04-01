using Dapper;
using Newtonsoft.Json;
using WebApp.DBContext;
using WebApp.Models;

namespace WebApp.Repository
{
    public class BPJSEmploymentRepository : DapperContext
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

            var data = await this.GetListPagingAsync<dynamic>("sp_BPJSEmployment_Get", par, "SP");
            return data;
        }

        public async Task<List<dynamic>> GetDataByKey(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Param", d.Param.ToString());
            par.Add("@Type", d.Type.ToString());

            var data = await this.GetListAsync<dynamic>("sp_BPJSEmployment_Get", par, "SP");
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

            var data = await this.GetListAsync<dynamic>("sp_BPJSEmployment_Get", par, "SP");
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
                par.Add("@KPJ", d.KPJ.ToString());
                par.Add("@KepAwal", d.KepAwal.ToString());
                par.Add("@Basicsalary", d.Basicsalary.ToString());
                par.Add("@Allowance", d.Allowance.ToString());
                par.Add("@Salary", d.Salary.ToString());
                par.Add("@IuranJKK", d.IuranJKK.ToString());
                par.Add("@IuranJKM", d.IuranJKM.ToString());
                par.Add("@IuranJHTTK", d.IuranJHTTK.ToString());
                par.Add("@IuranJHTCompany", d.IuranJHTCompany.ToString());
                par.Add("@BaseCalculation", d.BaseCalculation.ToString());
                par.Add("@IuranJPTK", d.IuranJPTK.ToString());
                par.Add("@IuranJPCompany", d.IuranJPCompany.ToString());
                par.Add("@TotalIuran", d.TotalIuran.ToString());
                par.Add("@UserId", userId);
            }
            par.Add("@Action", d.Action.ToString());

            var data = await this.GetListAsync<dynamic>("sp_BPJSEmployment_Process", par, "SP");
            return data;
        }

        public async Task<List<dynamic>> GenerateBPJS(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Periode", d.Periode.ToString());
            par.Add("@Act", d.Act.ToString());

            var data = await this.GetListAsync<dynamic>("sp_BPJSEmployment_Generate", par, "SP");
            return data;
        }
    }
}
