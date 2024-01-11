using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Messager.Helpers;
using Messager.Services.Authenticate;
using Messager.Services.Register;
using Newtonsoft.Json;

namespace Messager.Services
{
    public class  ServiceProvider
    {
        private static ServiceProvider _instance;
        private string _serverRootUrl = "https://94.19.228.225:6666";
        private DevHttpsConnectionHelper _devSslHelper = new(sslPort: 6666);

        public string _accessToken = "";


        public  async Task<AuthenticateResponse> Authenticate(AuthenticateRequest request)
        {
            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = HttpMethod.Post;
            httpRequestMessage.RequestUri = new Uri(_devSslHelper.DevServerRootUrl + "/Authenticate/Authenticate");

            if (request != null)
            {
                string jsonContent = JsonConvert.SerializeObject(request);
                var httpContent = new StringContent(jsonContent, encoding: Encoding.UTF8, "application/json"); ;
                httpRequestMessage.Content = httpContent;
            }

            try
            {
                var response = await _devSslHelper.HttpClient.SendAsync(httpRequestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<AuthenticateResponse>(responseContent);
                result.StatusCode = (int)response.StatusCode;

                if (result.StatusCode == 200)
                {
                    _accessToken = result.Token;
                }
                return result;
            }
            catch (Exception ex)
            {
                var result = new AuthenticateResponse
                {
                    StatusCode = 500,
                    StatusMessage = ex.Message
                };
                return result;
            }

        }
        public async Task<BaseResponse> Register(RegisterRequest request)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            using (HttpClient client = new HttpClient(clientHandler))
            {

                var httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage.Method = HttpMethod.Post;
                httpRequestMessage.RequestUri = new Uri(_serverRootUrl + "/Authenticate/Register");

                if (request != null)
                {
                    string jsonContent = JsonConvert.SerializeObject(request);
                    var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    httpRequestMessage.Content = httpContent;
                }

                try
                {
                    var response = await client.SendAsync(httpRequestMessage);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<BaseResponse>(responseContent);
                    result.StatusCode = (int)response.StatusCode;


                    return result;

                }
                catch (Exception e)
                {
                    var result = new BaseResponse()
                    {
                        StatusCode = 500,
                        StatusMessage = e.Message
                    };
                    return result;
                }
            }

        }
        public async Task<TResponse> CallWebApi<TRequest, TResponse>(
            string apiUrl, HttpMethod httpMethod, TRequest request) where TResponse : BaseResponse
        {
            var httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = HttpMethod.Post;
            httpRequestMessage.RequestUri = new Uri(_devSslHelper.DevServerRootUrl + apiUrl);
            httpRequestMessage.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

            if (request != null)
            {
                string jsonContent = JsonConvert.SerializeObject(request);
                var httpContent = new StringContent(jsonContent, encoding: Encoding.UTF8, "application/json"); ;
                httpRequestMessage.Content = httpContent;
            }

            try
            {
                var response = await _devSslHelper.HttpClient.SendAsync(httpRequestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<TResponse>(responseContent);
                result.StatusCode = (int)response.StatusCode;

                return result;
            }
            catch (Exception ex)
            {
                var result = Activator.CreateInstance<TResponse>();
                result.StatusCode = 500;
                result.StatusMessage = ex.Message;
                return result;
            }
        }
    }
    
}
