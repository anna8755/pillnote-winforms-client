using Flurl.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace pill_note_client.Handlers
{
    public partial class RemindersHandler
    {
        public static async Task<JObject> AddReminder(string medicine, DateTime time, string notes)
        {
            try
            {
                var link = $"{Variables.BASE_URL}reminder";

                var data = new
                {
                    medicine,
                    time,
                    notes
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
        public static async Task<JObject> GetReminders(int state = 0)
        {
            try
            {
                var link = $"{Variables.BASE_URL}reminder/";
                if (state == 1) link += "available/";
                if (state == 2) link += "viewed/";

                var accessToken = ApiHandler.GetAuthDataFromFile()["accessToken"].ToString();
                var responce = await link.WithHeader("Authorization", "Bearer " + accessToken).GetAsync();
                var content = await responce.ResponseMessage.Content.ReadAsStringAsync();

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
        public static async Task<JObject> DeleteReminder(JToken reminder)
        {
            try
            {
                var link = $"{Variables.BASE_URL}/reminder/{reminder["id"]}";
                var accessToken = ApiHandler.GetAuthDataFromFile()["accessToken"].ToString();

                var responce = await link.WithHeader("Authorization", "Bearer " + accessToken).DeleteAsync();
                var content = await responce.ResponseMessage.Content.ReadAsStringAsync();
                JObject jsonObject = JObject.Parse(content);

                return ApiHandler.AddStatusCodeToJObject(jsonObject, 200);
            }
            catch (Exception ex)
            {
                return ApiHandler.AddStatusCodeToJObject(ApiHandler.ReturnException(ex), 400);
            }
        }
        public static async Task<JObject> ChangeReminder(string id, string medicine, DateTime time, string notes)
        {
            try
            {
                var link = $"{Variables.BASE_URL}reminder/{id}";

                var data = new
                {
                    medicine,
                    time,
                    notes
                };
                var accessToken = ApiHandler.GetAuthDataFromFile()["accessToken"].ToString();
                var response = await link.WithHeader("Authorization", "Bearer " + accessToken).PatchJsonAsync(data);
                var content = await response.ResponseMessage.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(content);

                return ApiHandler.AddStatusCodeToJObject(jsonObject, 200);
            }
            catch (Exception ex)
            {
                return ApiHandler.AddStatusCodeToJObject(ApiHandler.ReturnException(ex), 400);
            }
        }
        public static async Task<JObject> MarkAsViewed(string id)
        {
            try
            {
                var link = $"{Variables.BASE_URL}reminder/view/{id}";
                var accessToken = ApiHandler.GetAuthDataFromFile()["accessToken"].ToString();
                var response = await link.WithHeader("Authorization", "Bearer " + accessToken).PostAsync();
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