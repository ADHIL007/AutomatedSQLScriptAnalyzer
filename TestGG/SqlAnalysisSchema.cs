

namespace SSMExtension
{
    public static class SqlAnalysisSchema
    {
        public static readonly string Schema = @"
    {
      ""type"": ""object"",
      ""properties"": {
        ""response"": {
          ""type"": ""object"",
          ""properties"": {
            ""queryInsights"": {
              ""type"": ""object"",
              ""properties"": {
                ""nolockLines"": {
                  ""type"": ""array"",
                  ""items"": { ""type"": ""integer"" },
                  ""description"": ""List of line numbers where NOLOCK hints are used.""
                },
                ""temporaryTablesNotDropped"": {
                  ""type"": ""array"",
                  ""items"": { ""type"": ""string"" },
                  ""description"": ""List of temporary tables that were not dropped.""
                }
              }
            },
            ""errors"": {
              ""type"": ""array"",
              ""items"": {
                ""type"": ""object"",
                ""properties"": {
                  ""lineNo"": { ""type"": ""integer"", ""description"": ""Line number of the error."" },
                  ""errorMsg"": { ""type"": ""string"", ""description"": ""Error message."" },
                  ""errorType"": { 
                    ""type"": ""string"", 
                    ""enum"": [""syntax"", ""logical"", ""performance"", ""security""],
                    ""description"": ""Type of error: syntax, logical, performance, or security.""
                  }
                }
              }
            },
            ""optimization"": {
              ""type"": ""object"",
              ""properties"": {
                ""optimizable"": { ""type"": ""boolean"", ""description"": ""Indicates if the query can be optimized."" },
                ""recommendations"": {
                  ""type"": ""array"",
                  ""items"": { ""type"": ""string"" },
                  ""description"": ""List of optimization suggestions.""
                }
              }
            },
            ""performanceMetrics"": {
              ""type"": ""object"",
              ""properties"": {
                ""executionTime"": { ""type"": ""string"", ""description"": ""Estimated execution time of the query."" },
                ""indexUsage"": { ""type"": ""string"", ""description"": ""Indicates if indexes are properly utilized."" },
                ""tableScans"": { ""type"": ""integer"", ""description"": ""Number of full table scans detected."" }
              }
            },
            ""codeQuality"": {
              ""type"": ""string"",
              ""enum"": [""very good"", ""good"", ""average"", ""bad"", ""poor""],
              ""description"": ""Overall quality rating of the SQL query.""
            },
            ""securityIssues"": {
              ""type"": ""array"",
              ""items"": {
                ""type"": ""object"",
                ""properties"": {
                  ""lineNo"": { ""type"": ""integer"", ""description"": ""Line number of the security issue."" },
                  ""issueType"": { 
                    ""type"": ""string"", 
                    ""enum"": [""SQL Injection"", ""Data Exposure"", ""Access Violation""],
                    ""description"": ""Type of security risk detected.""
                  },
                  ""recommendation"": { ""type"": ""string"", ""description"": ""Suggested fix for the security issue."" }
                }
              }
            }
          }
        }
      }
    }";
    }

}
