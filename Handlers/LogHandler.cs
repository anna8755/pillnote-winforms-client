using Flurl.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace pill_note_client.Handlers
{
    public static class LogHandler
    {
        public static async Task<JObject> AddLog(string reminderId, string notes)
        {
            try
            {
                var link = $"{Variables.BASE_URL}log";

                var data = new
                {
                    reminderId,
                    notes,
                    time = DateTime.Now,
                };

                var accessToken = ApiHandler.GetAuthDataFromFile()["accessToken"].ToString();
                var response = await link.WithHeader("Authorization", "Bearer " + accessToken).PostJsonAsync(data);
                var content = await response.ResponseMessage.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(content);

                return ApiHandler.AddStatusCodeToJObject(jsonObject, 200);
            }
            catch (Exception ex)
            {
                return ApiHandler.AddStatusCodeToJObject(ApiHandler.ReturnException(ex), 400);
            }
        }
        public static async Task<JObject> GetLogsByReminderId(string reminderId)
        {
            try
            {
                var link = $"{Variables.BASE_URL}log/{reminderId}";

                var accessToken = ApiHandler.GetAuthDataFromFile()["accessToken"].ToString();
                var response = await link.WithHeader("Authorization", "Bearer " + accessToken).GetAsync();
                var content = await response.ResponseMessage.Content.ReadAsStringAsync();
                var jsonArr = JArray.Parse(content);

                JObject jsonObject = new JObject
                {
                    { "array", jsonArr }
                };

                return ApiHandler.AddStatusCodeToJObject(jsonObject, 200);
            }
            catch (Exception ex)
            {
                return ApiHandler.AddStatusCodeToJObject(ApiHandler.ReturnException(ex), 400);
            }
        }
        public static async Task<JObject>DeleteLogById(string logId)
        {
            try
            {
                var link = $"{Variables.BASE_URL}log/{logId}";
                var accessToken = ApiHandler.GetAuthDataFromFile()["accessToken"].ToString();

                var response = await link.WithHeader("Authorization", "Bearer " + accessToken).DeleteAsync();
                var content = await response.ResponseMessage.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(content);

                return ApiHandler.AddStatusCodeToJObject(jsonObject, 200);
            }
            catch (Exception ex)
            {
                return ApiHandler.AddStatusCodeToJObject(ApiHandler.ReturnException(ex), 400);
            }
        }
    }
}