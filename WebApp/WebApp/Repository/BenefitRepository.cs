using Dapper;
using Newtonsoft.Json;
using WebApp.DBContext;
using WebApp.Models;

namespace WebApp.Repository
{
    public class BenefitRepository : DapperContext
    {
        public async Task<PagingModel<List<dynamic>>> GetDataPaging(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Param", d.Param.ToString());
            par.Add("@PageNumber", d.PageNumber.ToString());
            par.Add("@RowsOfPage", d.RowsOfPage.ToString());

            var data = await this.GetListPagingAsync<dynamic>("sp_Benefit_Get", par, "SP");
            return data;
        }

        public async Task<List<dynamic>> GetDataByKey(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Param", d.Param.ToString());
            par.Add("@Type", d.Type.ToString());

            var data = await this.GetListAsync<dynamic>("sp_Benefit_Get", par, "SP");
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
                par.Add("@CategoryId", d.CategoryId.ToString());
                par.Add("@CategoryName", d.CategoryName.ToString());
                par.Add("@PostingDate", d.PostingDate.ToString());
                par.Add("@DocumentNo", d.DocumentNo.ToString());
                par.Add("@ExtDocumentNo", d.ExtDocumentNo.ToString());
                par.Add("@Description", d.Description.ToString());
                par.Add("@VATAmount", d.VATAmount.ToString());
                par.Add("@Debit", d.Debit.ToString());
                par.Add("@Credit", d.Credit.ToString());
                par.Add("@Balance", d.Balance.ToString());
                par.Add("@Amount", d.Amount.ToString());
                par.Add("@UserId", userId);
            }
            par.Add("@Action", d.Action.ToString());

            var data = await this.GetListAsync<dynamic>("sp_Benefit_Process", par, "SP");
            return data;
        }

        public async Task<List<dynamic>> GetDataExport(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Param", d.Param.ToString());
            par.Add("@Type", d.Type.ToString());

            var data = await this.GetListAsync<dynamic>("sp_Benefit_Get", par, "SP");
            return data;
        }

        public async Task<string> SaveBenefitUpload(IList<string[]> data, string categoryId, string categoryName, string userId)
        {
            try
            {
                var sql = "";
                if (data.Count > 0)
                {
                    foreach (var r in data)
                    {
                        if (r[0] != "")
                        {
                            var par = new DynamicParameters();
                            var postDt = r[0].Split("/");
                            par.Add("@NIK", r[16]);
                            par.Add("@CategoryId", categoryId);
                            par.Add("@CategoryName", categoryName);
                            par.Add("@PostingDate", postDt[1] + "/" + postDt[0] + "/" + postDt[2]);
                            par.Add("@DocumentNo", r[1]);
                            par.Add("@ExtDocumentNo", r[2]);
                            par.Add("@Description", r[3]);
                            par.Add("@VATAmount", (r[5] == "") ? null : r[5].Replace(",", "."));
                            par.Add("@Debit", (r[6] == "") ? null : r[6].Replace(",", "."));
                            par.Add("@Credit", (r[9] == "") ? null : r[9].Replace(",", "."));
                            par.Add("@Balance", (r[10] == "") ? null : r[10].Replace(",", "."));
                            par.Add("@Amount", (r[17] == "") ? null : r[17].Replace(",", "."));
                            par.Add("@UserId", userId);

                            sql = "INSERT INTO Benefit ([NIK], [CategoryId], [CategoryName], [PostingDate], [DocumentNo], [ExtDocumentNo], [Description], [VATAmount], [Debit], [Credit], [Balance], [Amount], CreatedBy, CreatedDate)";
                            sql += " VALUES (@NIK, @CategoryId, @CategoryName, @PostingDate, @DocumentNo, @ExtDocumentNo, @Description, @VATAmount, @Debit, @Credit, @Balance, @Amount, @UserId, GETDATE())";
                            await this.ExecuteAsync<dynamic>(sql, par, "TEXT");
                        }
                    }
                }

                return "Success";
            }
            catch (Exception ex)
            {
                return "Failed";
            }
        }
    }
}
