using Dapper;
using Newtonsoft.Json;
using WebApp.DBContext;
using WebApp.Models;
using WebApp.Utils;

namespace WebApp.Repository
{
    public class CommonRepository : DapperContext
    {
        public async Task<List<dynamic>> GetAuth(string userid, string pass)
        {
            var encPass = AesOperation.EncryptString(pass);

            var par = new DynamicParameters();
            par.Add("@UserId", userid);
            par.Add("@Password", encPass);

            var data = await this.GetListAsync<dynamic>("SELECT * FROM UserLogin WHERE UserId = @UserId AND Password = @Password", par, "TEXT");
            return data;
        }

        public async Task<List<dynamic>> GetLookup(string type, string param = "")
        {
            var par = new DynamicParameters();
            par.Add("@Type", type);
            par.Add("@Param", param);

            var data = await this.GetListAsync<dynamic>("sp_Lookup", par, "SP");
            return data;
        }

        public async Task<string> GetSystemValue(string type, string param = "")
        {
            var par = new DynamicParameters();
            par.Add("@Type", type);
            par.Add("@Param", param);

            var data = await this.GetListAsync<dynamic>("sp_Lookup", par, "SP");
            if (data.Count > 0)
            {
                return data[0].SystemValue;
            }
            else
            {
                return "";
            }
        }

        public string GetSysVal(string type, string param = "")
        {
            var par = new DynamicParameters();
            par.Add("@Type", type);
            par.Add("@Param", param);

            var data = this.GetList<dynamic>("sp_Lookup", par, "SP");
            if (data.Count > 0)
            {
                return data[0].SystemValue;
            }
            else
            {
                return "";
            }
        }

        public async Task<string> ChangePass(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            
            var encPass = AesOperation.EncryptString(d.OldPassword.ToString());
            var encNewPass = AesOperation.EncryptString(d.NewPassword.ToString());

            var par = new DynamicParameters();
            par.Add("@UserId", d.UserId.ToString());
            par.Add("@OldPassword", encPass);
            par.Add("@NewPassword", encNewPass);

            var dataCheck = this.GetList<dynamic>("SELECT * FROM UserLogin WHERE UserId = @UserId AND Password = @OldPassword", par, "TEXT");
            if (dataCheck.Count == 0)
            {
                return "Old Password is wrong";
            }
            else
            {
                var data = await this.ExecuteAsync<dynamic>("UPDATE UserLogin SET Password = CASE WHEN ISNULL(@NewPassword, '') = '' THEN Password ELSE @NewPassword END WHERE UserId = @UserId", par, "TEXT");
                if (data > 0)
                {
                    return "Success";
                }
                else
                {
                    return "Failed Process";
                }
            }
        }
    }
}
