using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace TestGG.Common
{
    public static class CommonModule
    {
        private static bool debugFlag = true;
        public static void ShowMessage(AsyncPackage package, string message, string title, OLEMSGICON icon)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            VsShellUtilities.ShowMessageBox(
                package,
                message,
                title,
                icon,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        public static void Log(string message)
        {
            if (debugFlag)
            {
               
                Debug.WriteLine(message);
            }

           
            string logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SSMExtension", "Logs");

           
            Directory.CreateDirectory(logDirectory);

            string logFileName = $"{DateTime.Now:yyyy-MM-dd}.txt";  
            string logFilePath = Path.Combine(logDirectory, logFileName);

            try
            {
               
                File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
             
                Debug.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }

        public static string GetSelectedText()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                if (!(Package.GetGlobalService(typeof(SVsTextManager)) is IVsTextManager textManager))
                {
                    Debug.WriteLine("❌ IVsTextManager service not available.");
                    return null;
                }

                textManager.GetActiveView(1, null, out IVsTextView activeView);
                if (activeView == null)
                {
                    Debug.WriteLine("❌ No active view found.");
                    return null;
                }

                activeView.GetSelection(out int startLine, out int startColumn, out int endLine, out int endColumn);
                if (startLine == endLine && startColumn == endColumn)
                {
                    Debug.WriteLine("❌ No text selected.");
                    return null;
                }

                return activeView.GetTextStream(startLine, startColumn, endLine, endColumn, out string selectedText) == VSConstants.S_OK
                    ? selectedText.Trim()
                    : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error extracting selected text: {ex.Message}");
                return null;
            }
        }

       public static string GetActiveDocumentText()
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        try
        {
            if (!(Package.GetGlobalService(typeof(SVsTextManager)) is IVsTextManager textManager))
            {
                Log("❌ IVsTextManager service not available.");
                return null;
            }
                
            textManager.GetActiveView(1, null, out IVsTextView activeView);
            if (activeView == null)
            {
                Log("❌ No active view found.");
                return null;
            }

            if (activeView.GetBuffer(out IVsTextLines textLines) != VSConstants.S_OK)
            {
                Log("❌ Failed to get IVsTextLines from active view.");
                return null;
            }

            if (textLines.GetLineCount(out int totalLines) != VSConstants.S_OK)
            {
                Log("❌ Failed to get total line count.");
                return null;
            }

            int endLine = Math.Max(0, totalLines - 1);
            if (textLines.GetLengthOfLine(endLine, out int lastColumn) != VSConstants.S_OK)
            {
                Log("❌ Failed to get length of the last line.");
                return null;
            }

            return activeView.GetTextStream(0, 0, endLine, lastColumn, out string documentText) == VSConstants.S_OK
                ? documentText.Trim()
                : null;
        }
        catch (Exception ex)
        {
            Log($"❌ Error extracting document text: {ex.Message}");
            return null;
        }
    }
        public static void ReplaceTextInActiveDocument(string content)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                if (!(Package.GetGlobalService(typeof(SVsTextManager)) is IVsTextManager textManager))
                {
                    Log("❌ IVsTextManager service not available.");
                    return;
                }

                textManager.GetActiveView(1, null, out IVsTextView activeView);
                if (activeView == null)
                {
                    Log("❌ Failed to get active view.");
                    return;
                }

                if (activeView.GetBuffer(out IVsTextLines textLines) != VSConstants.S_OK)
                {
                    Log("❌ Failed to get IVsTextLines from the active document.");
                    return;
                }

                // Get the total line count
                if (textLines.GetLineCount(out int lineCount) != VSConstants.S_OK)
                {
                    Log("❌ Failed to get line count.");
                    return;
                }

                
                for (int i = lineCount - 1; i >= 0; i--)  
                {
                    textLines.GetLengthOfLine(i, out int lineLength);
                    activeView.ReplaceTextOnLine(i, 0, lineLength, "", 0);
                }

               
                activeView.ReplaceTextOnLine(0, 0, 0, content, content.Length);

                Log("✅ Successfully replaced content in the active document.");
            }
            catch (Exception ex)
            {
                Log($"❌ Error replacing text: {ex.Message}");
            }
        }







    }
}
//public async Task<string> AnalyzeAsync(string sqlQuery)
//{
//    IVsOutputWindow outputWindow = await ServiceProvider.GetServiceAsync(typeof(SVsOutputWindow)) as IVsOutputWindow;
//    OutputLogger logger = new OutputLogger(outputWindow);

//    logger.WriteMessage("🔹 AnalyzeAsync method started...");
//    Debug.WriteLine("🔹 AnalyzeAsync method started...");

//    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
//    logger.WriteMessage("🔹 TLS 1.2 set successfully.");
//    Debug.WriteLine("🔹 TLS 1.2 set successfully.");

//    string endpoint = $"{BaseUrl}models/{Model}:generateContent?key=AIzaSyBN_lu-m3ZJcgwK4bOGLgEr9AfCmJVoUSw"; // Masked API Key
//    logger.WriteMessage("🔹 Constructed API Endpoint (Hidden for security).");
//    Debug.WriteLine("🔹 Constructed API Endpoint (Hidden for security).");

//    //var requestBody = new
//    //{
//    //    contents = new[]
//    //                   {
//    //                   new { parts = new[] { new { text = $"Analyze and optimize this SQL query:\n\n{sqlQuery}" } } }
//    //               }
//    //};


//    var requestBody = new
//    {
//        contents = new[]
//      {
//        new
//        {
//            parts = new[]
//            {
//                new
//                {
//                    text = $"Analyze and optimize this SQL query:\n\n{sqlQuery}"
//                }
//            }
//        }
//    },
//        generationConfig = new[]
//      {
//        new
//        {
//            response_mime_type = "application/json",
//            response_schema = JsonDocument.Parse(SqlAnalysisSchema.Schema)
//        }
//    }
//    };



//    string jsonRequest = JsonConvert.SerializeObject(requestBody);
//    logger.WriteMessage("📤 JSON Request Payload prepared.");
//    Debug.WriteLine("📤 JSON Request Payload prepared.");

//    using (var httpClient = new HttpClient())
//    {
//        try
//        {
//            logger.WriteMessage("📡 Sending HTTP POST request...");
//            Debug.WriteLine("📡 Sending HTTP POST request...");

//            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

//            HttpResponseMessage response = await httpClient.PostAsync(endpoint, content);

//            string jsonResponse = await response.Content.ReadAsStringAsync();

//            logger.WriteMessage($"📥 HTTP Response received. Status Code: {response.StatusCode}");
//            Debug.WriteLine($"📥 HTTP Response received. Status Code: {response.StatusCode}");

//            if (!response.IsSuccessStatusCode)
//            {
//                logger.WriteMessage("❌ API Error encountered.");
//                Debug.WriteLine($"❌ API Error ({response.StatusCode}): {jsonResponse}");
//                return $"Error: {response.StatusCode} - {jsonResponse}";
//            }



//            logger.WriteMessage("✅ Gemini AI Response successfully received.");
//            Debug.WriteLine("✅ Gemini AI Response successfully received.");
//            Debug.WriteLine(jsonResponse);
//            return jsonResponse;


//        }
//        catch (Exception ex)
//        {
//            logger.WriteMessage($"❌ Unexpected Error: {ex.Message}");
//            Debug.WriteLine($"❌ Unexpected Error: {ex.Message}");
//            return $"Error: {ex.Message}";
//        }
//    }
//}




//public async Task<string> AnalyzeAsync(string sqlQuery)
//{
//    Debug.WriteLine("🔹 AnalyzeAsync method started...");


//    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
//    Debug.WriteLine("🔹 TLS 1.2 set successfully.");

//    string endpoint = $"{BaseUrl}models/{Model}:generateContent?key=AIzaSyBN_lu-m3ZJcgwK4bOGLgEr9AfCmJVoUSw";
//    Debug.WriteLine($"🔹 Constructed API Endpoint: {endpoint}");

//    //var requestBody = new
//    //{
//    //    contents = new[]
//    //    {
//    //    new { parts = new[] { new { text = $"Analyze and optimize this SQL query:\n\n{sqlQuery}" } } }
//    //}
//    //};

//    var requestBody = new
//    {
//        contents = new[]
//{
//               new
//               {
//                   parts = new[]
//                   {
//                       new
//                       {
//                           text = $"Analyze and optimize this SQL query:\n\n{sqlQuery}"
//                       }
//                   }
//               }
//           },
//        generationConfig = new[]
//{
//               new
//               {
//                   response_mime_type = "application/json",
//                   response_schema = JsonDocument.Parse(SqlAnalysisSchema.Schema)
//               }
//           }
//    };


//    string jsonRequest = JsonConvert.SerializeObject(requestBody);
//    Debug.WriteLine($"📤 JSON Request Payload: {jsonRequest}");

//    using (var httpClient = new HttpClient())
//    {
//        try
//        {
//            Debug.WriteLine("🔹 Initializing HTTP Client...");
//            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

//            Debug.WriteLine("📡 Sending HTTP POST request...");
//            HttpResponseMessage response = await httpClient.PostAsync(endpoint, content);

//            Debug.WriteLine($"📥 HTTP Response received. Status Code: {response.StatusCode}");

//            string jsonResponse = await response.Content.ReadAsStringAsync();
//            Debug.WriteLine($"📜 Raw API Response: {jsonResponse}");

//            if (!response.IsSuccessStatusCode)
//            {
//                Debug.WriteLine($"❌ API Error ({response.StatusCode}): {jsonResponse}");
//                return $"Error: {response.StatusCode} - {jsonResponse}";
//            }

//            Debug.WriteLine("✅ Gemini AI Response successfully received.");
//            return jsonResponse;
//        }
//        catch (HttpRequestException httpEx)
//        {
//            Debug.WriteLine($"❌ HTTP Error: {httpEx.Message}");
//            if (httpEx.InnerException != null)
//                Debug.WriteLine($"🔹 Inner Exception: {httpEx.InnerException.Message}");
//            return $"Error: {httpEx.Message}";
//        }
//        catch (Exception ex)
//        {
//            Debug.WriteLine($"❌ Unexpected Error: {ex.Message}");
//            return $"Error: {ex.Message}";
//        }
//    }
//}
//public async Task<string> AnalyzeAsync(string sqlQuery)
//{
//    Debug.WriteLine("🔹 AnalyzeAsync method started...");


//    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
//    Debug.WriteLine("🔹 TLS 1.2 set successfully.");

//    string endpoint = $"{BaseUrl}models/{Model}:generateContent?key={ApiKey}";
//    Debug.WriteLine($"🔹 Constructed API Endpoint: {endpoint}");

//    var requestBody = new
//    {
//        contents = new[]
//        {
//        new { parts = new[] { new { text = $"Analyze and optimize this SQL query:\n\n{sqlQuery}" } } }
//    }
//    };

//    string jsonRequest = JsonConvert.SerializeObject(requestBody);
//    Debug.WriteLine($"📤 JSON Request Payload: {jsonRequest}");

//    using (var httpClient = new HttpClient())
//    {
//        try
//        {
//            Debug.WriteLine("🔹 Initializing HTTP Client...");
//            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

//            Debug.WriteLine("📡 Sending HTTP POST request...");
//            HttpResponseMessage response = await httpClient.PostAsync(endpoint, content);

//            Debug.WriteLine($"📥 HTTP Response received. Status Code: {response.StatusCode}");

//            string jsonResponse = await response.Content.ReadAsStringAsync();
//            Debug.WriteLine($"📜 Raw API Response: {jsonResponse}");

//            if (!response.IsSuccessStatusCode)
//            {
//                Debug.WriteLine($"❌ API Error ({response.StatusCode}): {jsonResponse}");
//                return $"Error: {response.StatusCode} - {jsonResponse}";
//            }

//            Debug.WriteLine("✅ Gemini AI Response successfully received.");
//            return jsonResponse;
//        }
//        catch (HttpRequestException httpEx)
//        {
//            Debug.WriteLine($"❌ HTTP Error: {httpEx.Message}");
//            if (httpEx.InnerException != null)
//                Debug.WriteLine($"🔹 Inner Exception: {httpEx.InnerException.Message}");
//            return $"Error: {httpEx.Message}";
//        }
//        catch (Exception ex)
//        {
//            Debug.WriteLine($"❌ Unexpected Error: {ex.Message}");
//            return $"Error: {ex.Message}";
//        }
//    }
//}