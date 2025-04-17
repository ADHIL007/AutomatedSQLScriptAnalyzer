using Newtonsoft.Json;

namespace TestGG
{
    public static class SchemaStructure
    {
        public static readonly object AnalyseResponseFormat = new
        {
            type = "json_schema",
            json_schema = new
            {
                name = "sql_analysis",
                strict = true,
                schema = new
                {
                    type = "object",
                    additionalProperties = false,
                    properties = new
                    {
                        Errors = new
                        {
                            type = "object",
                            additionalProperties = false,
                            properties = new
                            {
                                SyntaxErrors = new
                                {
                                    type = "array",
                                    items = new
                                    {
                                        type = "object",
                                        properties = new
                                        {
                                            lineNumber = new { type = "integer" },
                                            Message = new { type = "string" }
                                        },
                                        required = new[] { "lineNumber", "Message" },
                                        additionalProperties = false
                                    }
                                },
                                LogicErrors = new
                                {
                                    type = "array",
                                    items = new
                                    {
                                        type = "object",
                                        properties = new
                                        {
                                            lineNumber = new { type = "integer" },
                                            Message = new { type = "string" }
                                        },
                                        required = new[] { "lineNumber", "Message" },
                                        additionalProperties = false
                                    }
                                },
                                JoinErrors = new
                                {
                                    type = "array",
                                    items = new
                                    {
                                        type = "object",
                                        properties = new
                                        {
                                            lineNumber = new { type = "integer" },
                                            Message = new { type = "string" }
                                        },
                                        required = new[] { "lineNumber", "Message" },
                                        additionalProperties = false
                                    }
                                }
                            },
                            required = new[] { "JoinErrors", "LogicErrors", "SyntaxErrors" }
                        },
                        performanceIssues = new
                        {
                            type = "object",
                            additionalProperties = false,
                            properties = new
                            {
                                MissingNolock = new { type = "array", items = new { type = "integer" } },
                                TempTableNotDropped = new
                                {
                                    type = "array",
                                    items = new
                                    {
                                        type = "object",
                                        properties = new
                                        {
                                            lineNumber = new { type = "integer" },
                                            message = new { type = "string" }
                                        },
                                        required = new[] { "lineNumber", "message" },
                                        additionalProperties = false
                                    }
                                },
                                selectStarUsage = new
                                {
                                    type = "array",
                                    items = new
                                    {
                                        type = "object",
                                        properties = new
                                        {
                                            lineNumber = new { type = "integer" },
                                            message = new { type = "string" }
                                        },
                                        required = new[] { "lineNumber", "message" },
                                        additionalProperties = false
                                    }
                                },
                                indexSuggestions = new
                                {
                                    type = "array",
                                    items = new
                                    {
                                        type = "object",
                                        properties = new
                                        {
                                            lineNumber = new { type = "integer" },
                                            column = new { type = "string" },
                                            table = new { type = "string" },
                                            suggestion = new { type = "string" }
                                        },
                                        required = new[] { "lineNumber", "column", "table", "suggestion" },
                                        additionalProperties = false
                                    }
                                },
                                expensiveSortOperations = new
                                {
                                    type = "array",
                                    items = new
                                    {
                                        type = "object",
                                        properties = new
                                        {
                                            lineNumber = new { type = "integer" },
                                            message = new { type = "string" }
                                        },
                                        required = new[] { "lineNumber", "message" },
                                        additionalProperties = false
                                    }
                                }
                            },
                            required = new[] { "MissingNolock", "TempTableNotDropped", "selectStarUsage", "indexSuggestions", "expensiveSortOperations" } // Ensure required performance issues
                        },
                        securityIssues = new
                        {
                            type = "array",
                            items = new
                            {
                                type = "object",
                                properties = new
                                {
                                    lineNumber = new { type = "integer" },
                                    message = new { type = "string" }
                                },
                                required = new[] { "lineNumber", "message" },
                                additionalProperties = false
                            }
                        }
                    },
                    required = new[] { "Errors", "performanceIssues", "securityIssues" }
                }
            }
        };

        public static readonly object AnalyseChatSchema = new
        {
            type = "json_schema",
            json_schema = new
            {
                name = "sql_analysis",
                strict = true,
                schema = new
                {
                    type = "object",
                    properties = new
                    {
                        summary = new
                        {
                            type = "string",
                            description = "Identified issues in the provided SQL query, using the exact table name."
                        },
                        query = new
                        {
                            type = "string",
                            description = "SQL query required for further analysis, or null if no execution is needed. replace {table_name } with orginal table names",
                            @enum = new[]
                      {
                        "SELECT COUNT(*) FROM {table_name};",
                        "DESCRIBE {table_name};",
                        "SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, COLUMN_DEFAULT FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{table_name}';",
                        "SELECT CONSTRAINT_NAME, TABLE_NAME, COLUMN_NAME, REFERENCED_TABLE_NAME, REFERENCED_COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_NAME = '{table_name}';",
                        "EXPLAIN SELECT * FROM {table_name};",
                        "SHOW INDEX FROM {table_name};",
                        "SELECT CONSTRAINT_NAME, CONSTRAINT_TYPE FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE TABLE_NAME = '{table_name}';",
                        "SELECT COLUMN_NAME, STATISTIC_VALUE FROM INFORMATION_SCHEMA.STATISTICS WHERE TABLE_NAME = '{table_name}';",
                        "SELECT TABLE_NAME, ROUND((DATA_LENGTH + INDEX_LENGTH) / 1024 / 1024, 2) AS Size_MB FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{table_name}';",
                        "null"
                    }
                        }
                    },
                    required = new[] { "summary", "query" }
                }
            }
        };

        public static readonly object AnalyseChunks = new
        {
            type = "json_schema",
            json_schema = new
            {
                name = "sql_analysis",
                strict = true,
                schema = new
                {
                    type = "object",
                    properties = new
                    {
                        summary = new
                        {
                            type = "string",
                            description = "Identified issues in the provided SQL query, using the exact table name."
                        }
                    },
                    required = new[] { "summary", "query" }
                }
            }
        };

        public static readonly object ResponseSchemaFix = new
        {
            type = "json_schema",
            json_schema = new
            {
                name = "sql_query",
                strict = true,
                schema = new
                {
                    type = "object",
                    additionalProperties = false,
                    properties = new
                    {
                        SqlQuery = new
                        {
                            type = "string"
                        }
                    },
                    required = new[] { "SqlQuery" }
                }
            }
        };

        public static string GetResponseFromatChunks()
        {
            return JsonConvert.SerializeObject(AnalyseChunks, Formatting.Indented);
        }
        public static string GetResponseSchemaFix()
        {
            return JsonConvert.SerializeObject(ResponseSchemaFix, Formatting.Indented);
        }
        public static string GetAnalyseChatSchema()
        {
            return JsonConvert.SerializeObject(AnalyseChatSchema, Formatting.Indented);
        }
        public static object GetResponseFormatJson()  
        {
            return AnalyseResponseFormat;  
        }
        public static object GetResponseSQLFormatJson()
        {
            return ResponseSchemaFix;
        }

    }
}
