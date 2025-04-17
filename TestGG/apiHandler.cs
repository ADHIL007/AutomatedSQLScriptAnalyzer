using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestGG;

public class ApiResponse
{
    public bool Success { get; set; }
    public int statusCode { get; set; }
    public string Content { get; set; }
}


public class ApiHandler
{
    private static readonly HttpClient client = new HttpClient();
    private readonly string endpoint = "ENDPONT";
    private readonly string apiKey =  "YOURAPIKEY"


    public async Task<ApiResponse> ModelConnectorChunksAsync(string sqlQuery ,int chunkIndex ,int chunkCount)
    {
        try
        {
            var message = Array.Empty<object>(); 
            if (chunkIndex == 0)
            {
                message = new[]
                {
                    new { role = "system", content = "" },
                    new { role = "user", content = sqlQuery }
                };
            }else if (chunkIndex == chunkCount-1)
            {
                message = new[]
              {
                    new { role = "system", content = "" },
                    new { role = "user", content = sqlQuery }
                };
            }
            else
            {
                message = new[]
              {
                    new { role = "system", content = "" },
                    new { role = "user", content = sqlQuery }
                };
            }

            var responseFormat = SchemaStructure.GetResponseFormatJson();

            var request = new
            {
                messages = message,
                max_tokens = 500,
                response_format = responseFormat
            };


            string jsonBody = JsonConvert.SerializeObject(request);

            Debug.WriteLine("Request Body:");
            Debug.WriteLine(jsonBody);

            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("api-key", apiKey);

                HttpResponseMessage response = await client.PostAsync(endpoint, content);

                Debug.WriteLine("Response Status: " + response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    JObject parsedJson = JObject.Parse(jsonResult);

                    TestGG.Common.CommonModule.Log("Reponse data" + jsonResult.ToString());
                    string contents = parsedJson["choices"]?[0]?["message"]?["content"]?.ToString();

                    return new ApiResponse
                    {
                        Success = true,
                        Content = contents ?? "Content not found",
                        statusCode = (int)response.StatusCode
                    };
                }
                else
                {
                    string errorDetails = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("Failed to fetch the response. Status code: " + response.StatusCode + ", Details: " + errorDetails);

                    return new ApiResponse
                    {
                        Success = false,
                        Content = $"API call failed. Status Code: {response.StatusCode}, Details: {errorDetails}",
                        statusCode = (int)response.StatusCode
                    };
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("An error occurred: " + ex.Message);

            return new ApiResponse
            {
                Success = false,
                Content = "An error occurred: " + ex.Message,
                statusCode = 500
            };
        }
    }

    public async Task<ApiResponse> ModelConnectorAsync(string sqlQuery)
    {
        try
        {
            var responseFormat = SchemaStructure.GetResponseFormatJson();

            var request = new
            {
                messages = new[]
      {
        new
        {
            role = "system",
            content = "You are an SQL analyzer. Analyze the query and strictly follow line numbers. Identify:\n" +
                      "- **Errors**: Syntax, logic, and join issues.\n" +
                      "- **Performance Issues**: Missing NOLOCK (ONLY consider select queries which is takeing from Physical table and deosnot have nolock), SELECT *, sorting inefficiencies, and index suggestions.\n" +
                      "- **Security Risks**: SQL injection and unsafe inputs.\n" +
                      "Return only relevant findings with exact line numbers."
        },
        new
        {
            role = "user",
            content = "```sql\n" + sqlQuery + "\n```"
        }
    },
                max_tokens = 2500,  
                temperature = 0,   
                response_format = responseFormat
            };


            string jsonBody = JsonConvert.SerializeObject(request);

            Debug.WriteLine("Request Body:");
            Debug.WriteLine(jsonBody);

            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("api-key", apiKey);

                HttpResponseMessage response = await client.PostAsync(endpoint, content);

                Debug.WriteLine("Response Status: " + response.StatusCode);
                
                if (response.IsSuccessStatusCode)
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    JObject parsedJson = JObject.Parse(jsonResult);

                    TestGG.Common.CommonModule.Log("Reponse data" + jsonResult.ToString());
                    string contents = parsedJson["choices"]?[0]?["message"]?["content"]?.ToString();

                    return new ApiResponse
                    {
                        Success = true,
                        Content = contents ?? "Content not found",
                        statusCode = (int)response.StatusCode
                    };
                }
                else
                {
                    string errorDetails = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("Failed to fetch the response. Status code: " + response.StatusCode + ", Details: " + errorDetails);

                    return new ApiResponse
                    {
                        Success = false,
                        Content = $"API call failed. Status Code: {response.StatusCode}, Details: {errorDetails}",
                        statusCode = (int)response.StatusCode
                    };
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("An error occurred: " + ex.Message);

            return new ApiResponse
            {
                Success = false,
                Content = "An error occurred: " + ex.Message ,
                statusCode = 500
            };
        }
    }

    public async Task<ApiResponse> ModelConnectorFixAsync(string sqlQuery)
    {
        try
        {
            var storedData = AnalyseResponseStore.Instance.CurrentAnalyseResponse;

            string jsonContent = JsonConvert.SerializeObject(storedData, Newtonsoft.Json.Formatting.Indented);

            var responseFormat = SchemaStructure.GetResponseSQLFormatJson();

            var request = new
            {
                messages = new[]
             {
                new { role = "system", content = "Fix and optimize the Provided Sql ,drop temptable if exist,add nolock to table ,fix joins,and logical issue"  },
                new { role = "user", content = sqlQuery }
            },
                max_tokens = 10000,
                response_format = responseFormat
            };

            string jsonBody = JsonConvert.SerializeObject(request);

            Debug.WriteLine("Request Body:");
            Debug.WriteLine(jsonBody);

            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("api-key", apiKey);

                HttpResponseMessage response = await client.PostAsync(endpoint, content);

                Debug.WriteLine("Response Status: " + response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    JObject parsedJson = JObject.Parse(jsonResult);

                    TestGG.Common.CommonModule.Log("Reponse data" + jsonResult.ToString());
                    string contents = parsedJson["choices"]?[0]?["message"]?["content"]?.ToString();

                    return new ApiResponse
                    {
                        Success = true,
                        Content = contents ?? "Content not found",
                        statusCode = (int)response.StatusCode
                    };
                }
                else
                {
                    string errorDetails = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("Failed to fetch the response. Status code: " + response.StatusCode + ", Details: " + errorDetails);

                    return new ApiResponse
                    {
                        Success = false,
                        Content = $"API call failed. Status Code: {response.StatusCode}, Details: {errorDetails}",
                        statusCode = (int)response.StatusCode
                    };
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("An error occurred: " + ex.Message);

            return new ApiResponse
            {
                Success = false,
                Content = "An error occurred: " + ex.Message,
                statusCode = 500
            };
        }
    }

}
