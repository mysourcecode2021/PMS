using Dapper;
using Newtonsoft.Json;
using WebApp.DBContext;
using WebApp.Models;

namespace WebApp.Repository
{
    public class MedicalInsuranceRepository : DapperContext
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

            var data = await this.GetListPagingAsync<dynamic>("sp_MedicalInsurance_Get", par, "SP");
            return data;
        }

        public async Task<List<dynamic>> GetDataByKey(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Param", d.Param.ToString());
            par.Add("@Type", d.Type.ToString());

            var data = await this.GetListAsync<dynamic>("sp_MedicalInsurance_Get", par, "SP");
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

            var data = await this.GetListAsync<dynamic>("sp_MedicalInsurance_Get", par, "SP");
            return data;
        }

        public async Task<List<dynamic>> SaveProcess(dynamic param, string userId)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            if (d.Action.ToString() == "Delete" || d.Action.ToString() == "UpdateDetail")
            {
                par.Add("@KeyValue", d.KeyValue.ToString());
            }
            else
            {
                par.Add("@MedicalId", d.MedicalId.ToString());
                par.Add("@Periode", d.Periode.ToString());
                par.Add("@NIK", d.NIK.ToString());
                par.Add("@Iuran", d.Iuran.ToString());
                par.Add("@TotalIuran", d.TotalIuran.ToString());
                par.Add("@Duration", d.Duration.ToString());
                par.Add("@UserId", userId);
            }
            par.Add("@Action", d.Action.ToString());

            var data = await this.GetListAsync<dynamic>("sp_MedicalInsurance_Process", par, "SP");
            return data;
        }
    }
}
