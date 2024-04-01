using Dapper;
using Newtonsoft.Json;
using WebApp.DBContext;
using WebApp.Models;

namespace WebApp.Repository
{
    public class LoanRepository : DapperContext
    {
        public async Task<PagingModel<List<dynamic>>> GetDataPaging(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Param", d.Param.ToString());
            par.Add("@PageNumber", d.PageNumber.ToString());
            par.Add("@RowsOfPage", d.RowsOfPage.ToString());

            var data = await this.GetListPagingAsync<dynamic>("sp_Loan_Get", par, "SP");
            return data;
        }

        public async Task<List<dynamic>> GetDataByKey(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Param", d.Param.ToString());
            par.Add("@Type", d.Type.ToString());

            var data = await this.GetListAsync<dynamic>("sp_Loan_Get", par, "SP");
            return data;
        }

        public async Task<List<dynamic>> GetDataExport(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Param", d.Param.ToString());
            par.Add("@Type", d.Type.ToString());

            var data = await this.GetListAsync<dynamic>("sp_Loan_Get", par, "SP");
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
                par.Add("@LoanId", d.LoanId.ToString());
                par.Add("@NIK", d.NIK.ToString());
                //par.Add("@LoanInstallment", d.LoanInstallment.ToString());
                par.Add("@LoanAmount", d.LoanAmount.ToString());
                par.Add("@Duration", d.Duration.ToString());
                par.Add("@Interest", (d.Interest.ToString() == "") ? "0" : d.Interest.ToString());
                par.Add("@LoanDate", d.LoanDate.ToString());
                par.Add("@UserId", userId);
            }
            par.Add("@Action", d.Action.ToString());

            var data = await this.GetListAsync<dynamic>("sp_Loan_Process", par, "SP");
            return data;
        }
    }
}
