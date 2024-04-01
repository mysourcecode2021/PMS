using Dapper;
using Newtonsoft.Json;
using WebApp.DBContext;
using WebApp.Models;

namespace WebApp.Repository
{
    public class GenerateRepository : DapperContext
    {
        public async Task<PagingModel<List<dynamic>>> GetDataPaging(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Param", d.Param.ToString());
            par.Add("@ParamHeader", d.ParamHeader.ToString());
            par.Add("@PageNumber", d.PageNumber.ToString());
            par.Add("@RowsOfPage", d.RowsOfPage.ToString());
            if (d.Type != null)
            {
                par.Add("@Type", d.Type.ToString());
            }

            var data = await this.GetListPagingAsync<dynamic>("sp_Payment_Get", par, "SP");
            return data;
        }

        public async Task<List<dynamic>> GetDataByKey(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Param", d.Param.ToString());
            par.Add("@Type", d.Type.ToString());

            var data = await this.GetListAsync<dynamic>("sp_Payment_Get", par, "SP");
            return data;
        }

        public async Task<List<dynamic>> GetDataExport(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Param", d.Param.ToString());
            par.Add("@ParamHeader", d.ParamHeader.ToString());
            par.Add("@Type", d.Type.ToString());

            var data = await this.GetListAsync<dynamic>("sp_Payment_Get", par, "SP");
            return data;
        }

        public async Task<List<dynamic>> GenerateSlip(dynamic param, string userId)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Type", d.Type.ToString());
            par.Add("@Periode", d.Periode.ToString());
            par.Add("@Act", d.Act.ToString());
            par.Add("@UserId", userId);

            var data = await this.GetListAsync<dynamic>("sp_GenerateSlip", par, "SP");
            return data;
        }

        public async Task<string> GetTextMoney(decimal amount)
        {
            var par = new DynamicParameters();
            par.Add("@Amount", amount);

            var data = await this.GetListAsync<dynamic>("SELECT dbo.MoneyToWords_EN(" + amount + ") Msg", par, "TEXT");
            if (data.Count > 0)
            {
                return data[0].Msg;
            }
            else
            {
                return "";
            }
        }

        public async Task<string> UpdateOvertime(dynamic param, string userId)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@NIK", d.NIK.ToString());
            par.Add("@Periode", d.Periode.ToString());
            par.Add("@BaseDay", d.BaseDay.ToString());
            par.Add("@AdditionalMeal", d.AdditionalMeal.ToString());
            par.Add("@AdjustmentMeal", d.AdjustmentMeal.ToString());
            par.Add("@AdditionalTransport", d.AdditionalTransport.ToString());
            par.Add("@AdjustmentTransport", d.AdjustmentTransport.ToString());
            par.Add("@TotalHour1", d.TotalHour1.ToString());
            par.Add("@TotalHour7", d.TotalHour7.ToString());
            par.Add("@TotalHour8", d.TotalHour8.ToString());
            par.Add("@TotalHour9", d.TotalHour9.ToString());

            var data = await this.ExecuteAsync<dynamic>("sp_UpdateOvertime", par, "SP");
            if (data != -1)
            {
                return "Success";
            }
            else
            {
                return "Data failed to process";
            }
        }
    }
}
