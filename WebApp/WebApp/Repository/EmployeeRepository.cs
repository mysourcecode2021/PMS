using Dapper;
using Newtonsoft.Json;
using WebApp.DBContext;
using WebApp.Models;
using static ICSharpCode.SharpZipLib.Zip.ExtendedUnixData;

namespace WebApp.Repository
{
    public class EmployeeRepository : DapperContext
    {
        public async Task<PagingModel<List<dynamic>>> GetDataPaging(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Param", d.Param.ToString());
            par.Add("@PageNumber", d.PageNumber.ToString());
            par.Add("@RowsOfPage", d.RowsOfPage.ToString());

            var data = await this.GetListPagingAsync<dynamic>("sp_Employee_Get", par, "SP");
            return data;
        }

        public async Task<List<dynamic>> GetDataByKey(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Param", d.Param.ToString());
            par.Add("@Type", d.Type.ToString());

            var data = await this.GetListAsync<dynamic>("sp_Employee_Get", par, "SP");
            return data;
        }

        public async Task<List<dynamic>> GetDataExport(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Param", d.Param.ToString());
            par.Add("@Type", d.Type.ToString());

            var data = await this.GetListAsync<dynamic>("sp_Employee_Get", par, "SP");
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
                par.Add("@FingerId", d.FingerId.ToString());
                par.Add("@Name", d.Name.ToString());
                par.Add("@Gender", d.Gender.ToString());
                par.Add("@PlaceDOB", d.PlaceDOB.ToString());
                par.Add("@DOB", d.DOB.ToString());
                par.Add("@Address", d.Address.ToString());
                par.Add("@City", d.City.ToString());
                par.Add("@ContactNo", d.ContactNo.ToString());
                par.Add("@IDCard", d.IDCard.ToString());
                par.Add("@Email", d.Email.ToString());
                par.Add("@BankName", d.BankName.ToString());
                par.Add("@BankAccount", d.BankAccount.ToString());
                par.Add("@DepartmentId", d.DepartmentId.ToString());
                par.Add("@PositionId", d.PositionId.ToString());
                par.Add("@UserId", userId);
                par.Add("@NPWP", d.NPWP.ToString());
                par.Add("@MaritalStatus", d.MaritalStatus.ToString());
                par.Add("@JoinDate", d.JoinDate.ToString());
                par.Add("@Status", d.Status.ToString());
                par.Add("@FlagSalary", d.FlagSalary.ToString());
                par.Add("@PKWTT", d.PKWTT.ToString());
                par.Add("@MaritalType", d.MaritalType.ToString());
                par.Add("@FlagOT", d.FlagOT.ToString());
                par.Add("@KodeObjekPajak", d.KodeObjekPajak.ToString());
                par.Add("@StatusKryAsing", d.StatusKryAsing.ToString());
                par.Add("@KodeNegara", d.KodeNegara.ToString());
                par.Add("@JumlahTanggungan", d.JumlahTanggungan.ToString());
            }
            par.Add("@Action", d.Action.ToString());

            var data = await this.GetListAsync<dynamic>("sp_Employee_Process", par, "SP");
            return data;
        }

        public async Task<List<dynamic>> GenerateNIK()
        {
            var data = await this.GetListAsync<dynamic>("sp_GenerateNIK", null, "SP");
            return data;
        }
    }
}
