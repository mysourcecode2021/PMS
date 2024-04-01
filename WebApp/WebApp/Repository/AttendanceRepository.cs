using Dapper;
using Newtonsoft.Json;
using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;
using WebApp.DBContext;
using WebApp.Models;
using WebApp.Utils;

namespace WebApp.Repository
{
    public class AttendanceRepository : DapperContext
    {
        static IConfiguration conf = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build());

        public async Task<PagingModel<List<dynamic>>> GetDataPaging(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Param", d.Param.ToString());
            par.Add("@PageNumber", d.PageNumber.ToString());
            par.Add("@RowsOfPage", d.RowsOfPage.ToString());

            var data = await this.GetListPagingAsync<dynamic>("sp_Attendance_Get", par, "SP");
            return data;
        }

        public async Task<List<dynamic>> GetDataByKey(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Param", d.Param.ToString());
            par.Add("@Type", d.Type.ToString());

            var data = await this.GetListAsync<dynamic>("sp_Attendance_Get", par, "SP");
            return data;
        }

        public async Task<List<dynamic>> GetDataExport(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            var par = new DynamicParameters();
            par.Add("@Param", d.Param.ToString());
            par.Add("@Type", d.Type.ToString());

            var data = await this.GetListAsync<dynamic>("sp_Attendance_Get", par, "SP");
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
                par.Add("@FingerId", d.FingerId.ToString());
                par.Add("@DateIn", d.DateIn.ToString());
                par.Add("@TimeIn", d.TimeIn.ToString());
                par.Add("@TimeOut", d.TimeOut.ToString());
                par.Add("@CategoryOT", d.CategoryOT.ToString());
                par.Add("@TotalOT", (d.TotalOT.ToString() != "") ? d.TotalOT.ToString() : null);
                par.Add("@UserId", userId);
            }
            par.Add("@Action", d.Action.ToString());

            var data = await this.GetListAsync<dynamic>("sp_Attendance_Process", par, "SP");
            return data;
        }

        public async Task<string> SaveAttendaceUpload(IList<string[]> data)
        {
            try
            {
                if (data.Count > 0)
                {
                    foreach (var r in data)
                    {
                        var par = new DynamicParameters();
                        if (r.Length >= 5)
                        {
                            if (r[5] != "")
                            {
                                var dateIn = r[5].Split("/");
                                par.Add("@FingerId", r[0]);
                                par.Add("@DateIn", dateIn[2] + "/" + dateIn[1] + "/" + dateIn[0]);
                                par.Add("@TimeIn", (r.Length > 7) ? ((r[7] == "") ? null : r[7]) : null);
                                par.Add("@TimeOut", (r.Length > 9) ? ((r[9] == "") ? null : r[9]) : null);
                                await this.ExecuteAsync<dynamic>("sp_Attendance_Upload", par, "SP");
                            }
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

        public async Task<string> SendOTP(dynamic param, string otp)
        {
            string msg = "";
            try
            {
                dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
                var par = new DynamicParameters();
                par.Add("@Email", d.Email.ToString());
                par.Add("@ActivationCode", otp);

                await this.ExecuteAsync<dynamic>("DELETE FROM UserOTP WHERE Email = @Email", par, "QUERY");
                var ret = await this.ExecuteAsync<dynamic>("INSERT INTO UserOTP VALUES (@Email, @ActivationCode)", par, "QUERY");
                if (ret > 0)
                {
                    if (d.Type.ToString() == "Email")
                    {
                        EmailDTO dtoMail = new EmailDTO();
                        dtoMail.Subject = "Verificatio  OTP";
                        dtoMail.To = d.Email.ToString();
                        dtoMail.Content = "Kode Verifikasi Anda : " + otp;
                        SendMail.Send(dtoMail);

                        msg = "Success: OTP has been sent to email.";
                    }
                    else if (d.Type.ToString() == "Phone")
                    {
                        string accountSid = conf["PhoneSettings:SID"].ToString();
                        string authToken = conf["PhoneSettings:TokenID"].ToString();
                        TwilioClient.Init(accountSid, authToken);

                        var to = new PhoneNumber(d.Email.ToString());
                        var message = MessageResource.Create(
                            to,
                            from: new PhoneNumber(conf["PhoneSettings:FromNumber"].ToString()),
                            body: $"Kode Verifikasi Anda : {otp}");

                        msg = "Success: OTP has been sent to your number.";
                    }
                }
                else
                {
                    msg = "Failed: OTP failed to process.";
                }
            }
            catch(Exception ex)
            {
                await this.ExecuteAsync<dynamic>("DELETE FROM UserOTP WHERE ActivationCode = '" + otp + "'", null, "QUERY");
                msg = "Failed: " + ex.Message.ToString();
            }

            return msg;
        }

        public async Task<string> SubmitOTP(dynamic param)
        {
            dynamic d = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(param));
            string msg = "";
            var par = new DynamicParameters();
            par.Add("@ActivationCode", d.OTP.ToString());

            var ret = await this.ExecuteAsync<dynamic>("DELETE FROM UserOTP WHERE ActivationCode = @ActivationCode", par, "QUERY");
            if (ret > 0)
            {
                var parSP = new DynamicParameters();
                parSP.Add("@UserId", d.UserId.ToString());
                parSP.Add("@Category", d.Category.ToString());
                var data = await this.GetListAsync<dynamic>("sp_Attendance_Online", parSP, "SP");

                msg = data[0].MSG;
            }
            else
            {
                msg = "Failed: Wrong OTP Code";
            }

            return msg;
        }
    }
}
