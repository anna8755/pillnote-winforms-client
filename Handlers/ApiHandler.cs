using Flurl.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;

namespace pill_note_client.Handlers
{
    public static class ApiHandler
    {
        #region служебные функции
        private static void SaveUserDataToFile(string data)
        {
            File.WriteAllText(Variables.DATA_FILE, data);
        }
        public static JObject GetAuthDataFromFile()
        {
            if (!File.Exists(Variables.DATA_FILE))
                return null;

            return JObject.Parse(File.ReadAllText(Variables.DATA_FILE));
        }
        public static async Task<JObject> IsAuthorized()
        {
            try
            {
                return await GetUser();
            }
            catch (Exception ex)
            {
                return new JObject(ex);
            }
        }
        public static bool IsValidUserApiResponce(JObject obj) => (int)obj["StatusCode"] == 200;
        public static JObject ReturnException(Exception exception, int code = 400)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var res = JObject.FromObject(exception, JsonSerializer.Create(settings));
            return AddStatusCodeToJObject(res, code);
        }
        #endregion
        public static JObject AddStatusCodeToJObject(JObject obj, int statusCode)
        {
            if (obj["StatusCode"] == null)
                obj.Add("StatusCode", statusCode);
            return obj;
        }
        public static async Task<JObject> Refresh()
        {
            if (!File.Exists(Variables.DATA_FILE))
                throw new Exception();
            try
            {
                var link = $"{Variables.BASE_URL}user/refresh";

                var refreshToken = GetAuthDataFromFile()["refreshToken"].ToString();
                var responce = await link.WithCookie("refreshToken", refreshToken).GetAsync();
                var content = await responce.ResponseMessage.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(content);

                SaveUserDataToFile(jsonObject.ToString());

                return AddStatusCodeToJObject(jsonObject, 200);
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }
        public static async Task<JObject> TryGetUserAndCheckAuthStatus()
        {
            var unathorizated = new { StatusCode = 401 };

            if (!File.Exists(Variables.DATA_FILE))
                return JObject.FromObject(unathorizated);

            var userData = await GetUser();
            if (userData["email"] != null)
            {
                return userData;
            }
            else
            {
                //await Refresh();
                return AddStatusCodeToJObject(await Refresh(), 200);
            }
        }

        #region работаем с API
        public static async Task<JObject> GetUser()
        {
            try
            {
                var accessToken = GetAuthDataFromFile()["accessToken"].ToString();

                var link = $"{Variables.BASE_URL}user";

                var responce = await link.WithHeader("authorization", "Bearer " + accessToken).GetAsync();
                var content = await responce.ResponseMessage.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(content);

                AddStatusCodeToJObject(jsonObject, 200);

                return jsonObject;
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }
        public static async Task<JObject> Logout()
        {
            //throw new NotImplementedException();
            try
            {
                var refreshToken = GetAuthDataFromFile()["refreshToken"].ToString();
                var link = $"{Variables.BASE_URL}user/logout";

                var responce = await link.WithCookie("refreshToken", refreshToken).PostAsync();
                var content = await responce.ResponseMessage.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(content);

                return AddStatusCodeToJObject(jsonObject, 200);
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }
        public static async Task<JObject> LoginAsync(string email, string password)
        {
            try
            {
                var link = $"{Variables.BASE_URL}user/login";
                var data = new
                {
                    email,
                    password
                };

                var response = await link.PostJsonAsync(data);
                var content = await response.ResponseMessage.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(content);

                SaveUserDataToFile(jsonObject.ToString());

                return jsonObject;
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }
        public static async Task<JObject> RegistrateAsync(string fullname, string email, string password, string photoLink = "")
        {
            try
            {
                var link = $"{Variables.BASE_URL}user/";
                var data = new
                {
                    fullname,
                    email,
                    password,
                    photoLink
                };
                var response = await link.PostJsonAsync(data);
                var content = await response.ResponseMessage.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(content);

                SaveUserDataToFile(jsonObject.ToString());
                return AddStatusCodeToJObject(jsonObject, 200);
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }
        #endregion
    }
}