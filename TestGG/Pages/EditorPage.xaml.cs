using System.Windows.Controls;
using System.IO;
using System.Reflection;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.Xml;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TestGG.Utilities;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TestGG.Pages
{
    public class AnalysisIssue
    {
        public string Category { get; set; }
        public string Message { get; set; }
        public int LineNumber { get; set; }
        public string Severity { get; set; } // Add this line
    }
    public class AlertMessage
    {
        public string Icon { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Severity { get; set; } 
    }
    public class SqlExtractResult
    {
        public int StartLine { get; set; }
        public int EndLine { get; set; }
        public string Query { get; set; }
    }
    public partial class EditorPage : Page
    {
        // Add this line at the top of your class
        public ObservableCollection<AnalysisIssue> ResponseList { get; set; } = new ObservableCollection<AnalysisIssue>();
        private int maxtoken = 10000;

        public EditorPage()
        {
            InitializeComponent();
            this.DataContext = this;
            errorLineBackgroundRenderer = new ErrorLineBackgroundRenderer(textEditor);


            //ApplyBtn.Opacity = 1;
            //ApplyBtn.IsEnabled = true;
            //ApplyBtn.Foreground = Brushes.White;




            DbAnalysisSwitch.Checked += DbAnalysisSwitch_Checked;
            DbAnalysisSwitch.Unchecked += DbAnalysisSwitch_Unchecked;

            textEditor.TextArea.TextView.BackgroundRenderers.Add(errorLineBackgroundRenderer);
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TestGG.Resources.SQLSyntax.xshd"))
            {
                if (stream != null)
                {
                    using (var reader = new System.Xml.XmlTextReader(stream))
                    {
                        textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    }
                }
            }

            Common.CommonModule.Log("Fetching Queries From Active Document");
            textEditor.Text = Common.CommonModule.GetActiveDocumentText();

            int tokenCount = CountTokens(textEditor.Text);
            tokenText.Text = tokenCount.ToString();
         

            if (textEditor.Text.Length != 0)
            {
                Common.CommonModule.Log("Queries Fetched ,Token count" + tokenText.Text);
                UpdateStatus(this, "Query Fetched", 1);
                analyzeBtn.Opacity = 1;
                analyzeBtn.IsEnabled = true;
                analyzeBtn.Foreground = Brushes.White;
            }
            else
            {
                Common.CommonModule.Log("Query is Empty");
                UpdateStatus(this, "There is no Query in Active document", 2);
                analyzeBtn.Opacity = 0.8;
            }
        }


  

        private void DbAnalysisSwitch_Checked(object sender, RoutedEventArgs e)
        {
            
            Console.WriteLine("Database-level analysis is enabled.");
        }

        private void DbAnalysisSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
           
            Console.WriteLine("Database-level analysis is disabled.");
        }

    
        private void AnalyzeBtn_Click(object sender, RoutedEventArgs e)
        {
            HandleAnalyzeBtnAsync();
        }

        private void HandleFixAndOptimize(object sender, RoutedEventArgs e)
        {
            HandleFixAndOptimizeAsync();
        }

        private void HandleApplyButton_click(object sender, RoutedEventArgs e)
        {
           Common.CommonModule.ReplaceTextInActiveDocument( textEditor.Text);
        }

        private async void HandleAnalyzeBtnAsync()
        {
            AnalyseResponseStore.Instance.ClearAnalyseResponse();
            analysisstack.Children.Clear();
            string sqlQuery = textEditor.Text;

            await UpdateStatusWithDelay(this, "Analyzing query...", 0);
            Common.CommonModule.Log("Analyzing query...");
            List<string> chunks = null;

            try
            {
                await UpdateStatusWithDelay(this, "Cleaning the SQL query...", 0);
                sqlQuery = await CleaningAsync(sqlQuery,true);
                Debug.WriteLine(sqlQuery);
                int tokenValue = 0;
                ApiResponse response = null;

                if (int.TryParse(tokenText.Text, out tokenValue))
                {
                    if (tokenValue > maxtoken)
                    {
                        await UpdateStatusWithDelay(this, "SQL query is too large; chunking required...", 0);
                        Common.CommonModule.Log("SQL query is large; chunking required.");
                        chunks = Chunking(sqlQuery);

                        await UpdateStatusWithDelay(this, "Chunking completed... proceeding to send chunks.", 0);

                        //for(int i = 0; i < chunks.Count ;i++)
                        //{


                        //}


                    }
                    else
                    {
                        await UpdateStatusWithDelay(this, "Analyzing your SQL query, please wait...", 0);
                        var apiHandler = new ApiHandler();
                        response = await apiHandler.ModelConnectorAsync(sqlQuery);

                    }


                    if (response != null && response.statusCode == 200)
                    {
                        await UpdateStatusWithDelay(this, "Analysis completed successfully.", 1);
                        Common.CommonModule.Log("Analysis completed successfully.");
                        ProcessApiResponse(response);


                    }
                    else
                    {
                        string errorMessage = HandleApiErrorResponse(response);
                        await UpdateStatusWithDelay(this, errorMessage, 2);
                        Common.CommonModule.Log(errorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Common.CommonModule.Log($"Error during analysis: {ex.Message}");
                Common.CommonModule.Log($"Stack Trace: {ex.StackTrace}");

                await UpdateStatusWithDelay(this, "Analysis failed. Please check the query.", 2);
            }
        }

        private async Task ProcessApiResponse(ApiResponse response)
        {
            if (response != null)
            {
 
                Common.CommonModule.Log($"Success: {response.Success}, StatusCode: {response.statusCode}, Content: {response.Content}");

                try
                {
                    var content = JsonConvert.DeserializeObject<AnalyseResponse>(response.Content);
                    AnalyseResponseStore.Instance.SetAnalyseResponse(content);

              
                    string jsonContent = JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.Indented);
                    Debug.WriteLine("Deserialized response: " + jsonContent);

                 
                    int totalErrors = content.Errors.SyntaxErrors.Count +
                                      content.Errors.LogicErrors.Count +
                                      content.Errors.JoinErrors.Count;

                    int nolockCnt = content.PerformanceIssues.MissingNolock.Count;
                    int tempTableCnt = content.PerformanceIssues.TempTableNotDropped.Count;

             

                
                    FixBtn.Opacity = 1;
                    FixBtn.IsEnabled = true;
                    FixBtn.Foreground = Brushes.White;

                  
                    AddErrorCards(content.Errors.SyntaxErrors, "❌", "Syntax Error", "#FFEBEE", "#F44336");

                   
                    AddErrorCards(content.Errors.LogicErrors, "⚠️", "Logic Error", "#FFF3E0", "#FF9800");

                
                    AddErrorCards(content.Errors.JoinErrors, "🔗", "Join Error", "#E3F2FD", "#2196F3");

                 
                    foreach (var lineNumber in content.PerformanceIssues.MissingNolock)
                    {
                        AddErrorCard("🔒", "Missing Nolock", lineNumber, "No lock Missing", "#E0F7FA", "#00BCD4");
                    }

                    
                    foreach (var issue in content.PerformanceIssues.TempTableNotDropped)
                    {
                        AddErrorCard("🛑", "Temp Table Not Dropped", issue.LineNumber, issue.Message, "#FFEBEE", "#E91E63");
                    }

                    
                    foreach (var issue in content.PerformanceIssues.SelectStarUsage)
                    {
                        AddErrorCard("⭐", "Select Star Usage", issue.LineNumber, issue.Message, "#FFFDE7", "#FFEB3B");
                    }

                  
                    foreach (var issue in content.PerformanceIssues.IndexSuggestions)
                    {
                        AddErrorCard("🔍", "Index Suggestion", issue.LineNumber, $"Suggest index on column {issue.Column} in table {issue.Table}", "#E8F5E9", "#4CAF50");
                    }

                  
                    foreach (var issue in content.PerformanceIssues.ExpensiveSortOperations)
                    {
                        AddErrorCard("💸", "Expensive Sort Operation", issue.LineNumber, issue.Message, "#FCE4EC", "#E91E63");
                    }

                 
                    foreach (var issue in content.SecurityIssues)
                    {
                        AddErrorCard("🔐", "Security Issue", issue.LineNumber, issue.Message, "#FFF8E1", "#FFC107");
                    }
                }
                catch (Exception ex)
                {
                    Common.CommonModule.Log($"Error processing response: {ex.Message}");
                    Common.CommonModule.Log($"Stack Trace: {ex.StackTrace}");
                }
            }
            else
            {
                Common.CommonModule.Log("Response is null");
            }
        }

        private void AddErrorCards<T>(List<T> errorList, string icon, string issueType, string backgroundColor, string borderColor) where T : AnalyseResponse.errorSchema
        {
            foreach (var error in errorList)
            {
                AddErrorCard(icon, issueType, error.lineNumber, error.Message, backgroundColor, borderColor);
            }
        }

        private void AddErrorCard(string icon, string issueType, int? lineNumber, string message, string backgroundColor, string borderColor)
        {
            Border border = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(backgroundColor)),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(borderColor)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(8),
                Margin = new Thickness(4, 2, 4, 2)
            };

            StackPanel stackPanel = new StackPanel
            {
                Orientation = System.Windows.Controls.Orientation.Vertical,
                Margin = new Thickness(0)
            };

            TextBlock iconText = new TextBlock
            {
                Text = icon,
                FontSize = 14,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(borderColor)),
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 2)
            };

            TextBlock issueTypeText = new TextBlock
            {
                Text = issueType,
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333")),
                Margin = new Thickness(0, 0, 0, 4)
            };

            TextBlock lineText = new TextBlock
            {
                Text = $"Line: {lineNumber}",
                FontSize = 10,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#666666")),
                Margin = new Thickness(0, 0, 0, 2)
            };

            TextBlock errorTextBlock = new TextBlock
            {
                Text = $"Message: {message}",
                FontSize = 10,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333")),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 0)
            };

            //System.Windows.Controls.Button quickFixButton = new System.Windows.Controls.Button
            //{
            //    Content = "Quick Fix",
            //    FontSize = 10,
            //    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007ACC")), 
            //    Foreground = Brushes.White,
            //    Padding = new Thickness(5, 2, 5, 2),
            //    HorizontalAlignment = System.Windows.HorizontalAlignment.Left, 
            //    Cursor = System.Windows.Input.Cursors.Hand,
            //    Margin = new Thickness(0, 4, 0, 0)
            //};

            System.Windows.Controls.Button quickFix = new System.Windows.Controls.Button
            {
                Content = "Quick Fix⚡",
                FontSize = 8,
                Background = Brushes.Transparent,
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#800080")),
                BorderThickness = new Thickness(1),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#800080")), 
                Padding = new Thickness(5, 2, 5, 2),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(0, 4, 0, 0),
                Template = CreateRoundedButtonTemplate() 
            };

            quickFix.Click += (sender, e) =>
            {
                if (lineNumber.HasValue)
                {
                    HandleQuickFixClickAsync(lineNumber.Value);
                }
            };


            stackPanel.Children.Add(iconText);
            stackPanel.Children.Add(issueTypeText);
            stackPanel.Children.Add(lineText);
            stackPanel.Children.Add(errorTextBlock);
            stackPanel.Children.Add(quickFix);


            border.Child = stackPanel;
            border.MouseLeftButtonDown += (sender, e) => HandleErrorCardClick(issueType, (int)lineNumber);
            border.Background = border.Background ?? Brushes.Transparent;
            analysisstack.Children.Add(border);
        }
        private static ControlTemplate CreateRoundedButtonTemplate()
        {
            var template = new ControlTemplate(typeof(System.Windows.Controls.Button));

            // Define the visual structure of the button
            FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(10)); // Rounded corners
            border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(System.Windows.Controls.Button.BackgroundProperty));
            border.SetValue(Border.BorderBrushProperty, new TemplateBindingExtension(System.Windows.Controls.Button.BorderBrushProperty));
            border.SetValue(Border.BorderThicknessProperty, new TemplateBindingExtension(System.Windows.Controls.Button.BorderThicknessProperty));

            // Add content presenter for the button's content
            FrameworkElementFactory contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetValue(ContentPresenter.MarginProperty, new Thickness(5));
            contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, System.Windows.HorizontalAlignment.Center);
            contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            border.AppendChild(contentPresenter);

            template.VisualTree = border;
            return template;
        }
        private async Task HandleQuickFixClickAsync(int lineNumber)
        {
            Debug.WriteLine("Quick Fix " + lineNumber);
            if (lineNumber < 1 || lineNumber > textEditor.Document.LineCount)
            {
                Debug.WriteLine($"Invalid line number: {lineNumber}");
                return;
            }

            var sqlQuery = await CleaningAsync(textEditor.Text, false);

            var result = FindNearestSelectQuery(sqlQuery, lineNumber);

            Debug.WriteLine($"Start Line: {result.StartLine}");
            Debug.WriteLine($"End Line: {result.EndLine}");
            Debug.WriteLine("Extracted SQL Query:");

            errorLineBackgroundRenderer.SetHighlightedLine(lineNumber, "magic");


            textEditor.ScrollToLine(lineNumber);
        }

        static SqlExtractResult FindNearestSelectQuery(string sql, int targetLine)
        {
            string[] lines = sql.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var selectQueries = new List<(int startLine, int endLine, string query)>();

            // Regular expression to match lines containing SELECT
            Regex selectRegex = new Regex(@"^(.*SELECT.*)$", RegexOptions.IgnoreCase);
            Regex integerRegex = new Regex(@"\d+", RegexOptions.IgnoreCase);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                // Check for SELECT lines
                if (selectRegex.IsMatch(line))
                {
                    int startLine = i + 1; // Line number starts from 1
                    int endLine = startLine;

                    // Find integers in the current line
                    string numbersInLine = string.Join(",", integerRegex.Matches(line).Cast<Match>().Select(m => m.Value));

                    // Debugging output
                    Debug.WriteLine($"Checking line {startLine}: {line.Trim()}");
                    Debug.WriteLine($"Nearby integers: {numbersInLine}");

                    // Add the found SELECT statement to the list
                    selectQueries.Add((startLine, endLine, line.Trim()));
                }
            }

            // Debugging output: Show the list of found SELECT queries
            Debug.WriteLine("All SELECT queries found:");
            foreach (var query in selectQueries)
            {
                Debug.WriteLine($"Start Line: {query.startLine}, Query: {query.query}");
            }

            // Find the nearest SELECT query based on the target line
            var nearestQuery = selectQueries.OrderBy(query => Math.Abs(query.startLine - targetLine)).FirstOrDefault();

            if (nearestQuery.startLine == 0)
            {
                return new SqlExtractResult { Query = "No SELECT statement found.", StartLine = -1, EndLine = -1 };
            }

            // Debugging output: nearest SELECT query
            Debug.WriteLine($"Nearest SELECT found at line {nearestQuery.startLine}: {nearestQuery.query}");

            return new SqlExtractResult
            {
                StartLine = nearestQuery.startLine,
                EndLine = nearestQuery.endLine,
                Query = nearestQuery.query
            };
        }





        private ErrorLineBackgroundRenderer errorLineBackgroundRenderer;
        private void HandleErrorCardClick(string issueType, int lineNumber)
        {
            
            if (lineNumber < 1 || lineNumber > textEditor.Document.LineCount)
            {
                Debug.WriteLine($"Invalid line number: {lineNumber}");
                return;
            }

          
            errorLineBackgroundRenderer.SetHighlightedLine(lineNumber, issueType);

           
            textEditor.ScrollToLine(lineNumber);
        }
        private string HandleApiErrorResponse(ApiResponse response)
        {
            if (response == null)
                return "No response from the API.";

            switch (response.statusCode)
            {
                case 400:
                    return "Bad Request: Please check the response format.";
                case 401:
                    return "Unauthorized: Invalid or missing API key.";
                case 403:
                    return "Forbidden: You do not have permission to access this resource.";
                case 404:
                    return "Not Found: The requested resource could not be found.";
                case 409:
                    return "Conflict Error: There is a conflict with the request.";
                case 429:
                    string time = ExtractRetryTime(response.Content);
                    return $"Rate Limit Exceeded: Too many requests, try again after {time}.";
                case 500:
                    return "Internal Server Error: The server encountered an error.";
                case 502:
                    return "Bad Gateway: Invalid response from upstream server.";
                case 503:
                    return "Service Unavailable: The service is temporarily unavailable.";
                default:
                    return $"Unknown Error (Status Code: {response?.statusCode}): {response?.Content}";
            }
        }

        static string ExtractRetryTime(string errorMessage)
        {
            
            string pattern = @"Please retry after (\d+)\s+(\w+)";

          
            Match match = Regex.Match(errorMessage, pattern);

          
            if (match.Success)
            {
                
                string number = match.Groups[1].Value; 
                string unit = match.Groups[2].Value; 


                return $"{number} {unit}";
            }

            
            return null;
        }
        private async Task UpdateStatusWithDelay(EditorPage editorPage, string message, int status)
        {
            UpdateStatus(editorPage, message, status);
            await Task.Delay(500);
        }

        private List<string> Chunking(string query)
        {
            var tokens = query.Split(new[] { ' ', ',', ';', '(', ')', '.', '=', '+', '-', '*', '/', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> chunks = new List<string>();
            int chunkSize = 20;
            int totalTokens = tokens.Length;

            for (int i = 0; i < totalTokens; i += chunkSize)
            {
                int end = Math.Min(i + chunkSize, totalTokens);
                var chunk = string.Join(" ", tokens.Skip(i).Take(end - i));
                chunks.Add(chunk);
            }
             
            return chunks;
        }

        static string AddLineNumbersAtStart(string sqlScript)
        {
            
            string[] lines = sqlScript.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            
            List<string> numberedLines = new List<string>();

            
            for (int i = 0; i < lines.Length; i++)
            {
               
                numberedLines.Add($"{i + 1}: {lines[i]}");
            }

           
            return string.Join(Environment.NewLine, numberedLines);
        }

        private async Task<string> CleaningAsync(string query ,Boolean needLineNumber)
        {
            if (string.IsNullOrWhiteSpace(query))
                return string.Empty;

            if (needLineNumber)
            {
                query = AddLineNumbersAtStart(query);
            }
       

            string cleanedQuery = System.Text.RegularExpressions.Regex.Replace(
                query, @"(--[^\r\n]*)|(/\*[\s\S]*?\*/)", string.Empty, System.Text.RegularExpressions.RegexOptions.Multiline);

           
            cleanedQuery = System.Text.RegularExpressions.Regex.Replace(cleanedQuery, @"[\r\n]+", " ");

           
            cleanedQuery = System.Text.RegularExpressions.Regex.Replace(cleanedQuery, @"\s+", " ").Trim();

          
            cleanedQuery = System.Text.RegularExpressions.Regex.Replace(cleanedQuery, @"(\w)(\()", "$1 $2");
            cleanedQuery = System.Text.RegularExpressions.Regex.Replace(cleanedQuery, @"(\))(\w)", "$1 $2");

            string[] keywords = { "SELECT", "FROM", "WHERE", "JOIN", "LEFT JOIN", "RIGHT JOIN", "INNER JOIN", "OUTER JOIN",
                          "UPDATE", "SET", "DELETE", "INSERT", "INTO", "VALUES", "CREATE", "PROCEDURE", "BEGIN", "END" };

            foreach (string keyword in keywords)
            {
                cleanedQuery = System.Text.RegularExpressions.Regex.Replace(
                    cleanedQuery, $@"\b{keyword.ToLower()}\b", keyword, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }

            await UpdateStatusWithDelay(this, "Query cleaned", 1);

            return cleanedQuery;
        }



        public static void UpdateStatus(EditorPage editorPage, string message, int status)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                string basePath = "pack://application:,,,/TestGG;component/Resources/Assets/";


                editorPage.statusLog.Text = message;


                switch (status)
                {
                    case 0: // Loading (Spinner)
                        SetAnimatedGif(editorPage.statusIcon, basePath + "icons8-sand-clock.gif");
                        break;
                    case 1: // Success (Tick)
                        SetAnimatedGif(editorPage.statusIcon, basePath + "icons8-tick.gif");
                        break;
                    case 2: // Failed (Cancel)
                        SetAnimatedGif(editorPage.statusIcon, basePath + "icons8-cancel.gif");
                        break;
                    default: // Unknown status
                        editorPage.statusIcon.Source = null;
                        break;
                }
            });
        }

        private static void SetAnimatedGif(System.Windows.Controls.Image img, string fileName)
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(fileName, UriKind.Absolute);  // Load the GIF
            image.EndInit();

            // Use ImageBehavior to set the animated GIF
            ImageBehavior.SetAnimatedSource(img, image);
        }

        static int CountTokens(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0;

            char[] delimiters = { ' ', ',', '.', ';', ':', '!', '?', '\t', '\n', '\r', '(', ')', '[', ']', '{', '}', '"' };

            string[] tokens = input.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            return tokens.Length;
        }


        private async void HandleFixAndOptimizeAsync()
        {
            errorLineBackgroundRenderer.ClearHighlights();
            Common.CommonModule.Log("Fix and optimization process started...");
            await UpdateStatusWithDelay(this, "Starting optimization... Please wait.", 0);

            try
            {
                string sqlQuery = textEditor.Text;

               
                await UpdateStatusWithDelay(this, "Cleaning the SQL query...", 0);
                sqlQuery = await CleaningAsync(sqlQuery, false);
                Common.CommonModule.Log("SQL query cleaned successfully.");

                ApiResponse response = null;

                
                await UpdateStatusWithDelay(this, "Optimizing SQL query, please wait...", 0);
                var apiHandler = new ApiHandler();
                response = await apiHandler.ModelConnectorFixAsync(sqlQuery);

               
                if (response != null && response.Content != null)
                {
                    string rawResponse = response.Content;
                    Common.CommonModule.Log($"Raw response: {rawResponse}");

                    
                    if (response.statusCode == 200)
                    {
                        try
                        {
                            AnalyseResponseStore.Instance.ClearAnalyseResponse();
                            var content = JsonConvert.DeserializeObject<SqlResponse>(rawResponse);

                            if (content != null && content.SqlQuery != null)
                            {
                                string cleanedQuery = content.SqlQuery.ToString();

                                Common.CommonModule.Log("Fixed Query" + cleanedQuery);


                                await UpdateStatusWithDelay(this, "SQL query optimization completed successfully.", 1);
                                Common.CommonModule.Log("Optimization completed successfully.");
                                if(content.SqlQuery != null)
                                {
                                    textEditor.Text = cleanedQuery;
                                    AnalyseResponseStore.Instance.ClearAnalyseResponse();
                                    analysisstack.Children.Clear();

                                }


                                ApplyBtn.Opacity = 1;
                                ApplyBtn.IsEnabled = true;
                                ApplyBtn.Foreground = Brushes.White;
                            }
                            else
                            {
                                
                                Common.CommonModule.Log("Error: SQL query is missing in the response.");
                                await UpdateStatusWithDelay(this, "Optimization failed: SQL query is missing in the response.", 2);
                            }
                        }
                        catch (JsonReaderException jsonEx)
                        {
                           
                            Common.CommonModule.Log($"Error during JSON deserialization: {jsonEx.Message}");
                            await UpdateStatusWithDelay(this, "Optimization failed: Invalid JSON format in response.", 2);
                        }
                    }
                    else
                    {
                        string errorMessage = HandleApiErrorResponse(response);
                        await UpdateStatusWithDelay(this, $"Optimization failed: {errorMessage}", 2);
                        Common.CommonModule.Log($"Error during optimization: {errorMessage}");
                    }
                }
                else
                {
                    // Handle the case where response.Content is null
                    Common.CommonModule.Log("Error: Response content is null.");
                    await UpdateStatusWithDelay(this, "Optimization failed: Response content is null.", 2);
                }
            }
            catch (Exception ex)
            {
                // Catch unexpected exceptions
                Common.CommonModule.Log($"Unexpected error during optimization: {ex.Message}");
                Common.CommonModule.Log($"Stack Trace: {ex.StackTrace}");

                await UpdateStatusWithDelay(this, "An error occurred during optimization. Please check the logs for details.", 2);
            }
        }






    }

}
