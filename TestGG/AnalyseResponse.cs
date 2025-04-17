using System;
using System.Collections.Generic;

namespace TestGG
{
    public class SqlResponse
    {
        public string SqlQuery { get; set; }
    }
    public class AnalyseResponse
    {
        public ErrorDetails Errors { get; set; }
        public PerformanceIssuesDetails PerformanceIssues { get; set; }  
        public List<SecurityIssue> SecurityIssues { get; set; }

        public class ErrorDetails
        {
            public List<errorSchema> SyntaxErrors { get; set; }
            public List<errorSchema> LogicErrors { get; set; }
            public List<errorSchema> JoinErrors { get; set; }
        }

        public class PerformanceIssuesDetails  
        {
            public List<int> MissingNolock { get; set; }
            public List<TempTableNotDropped> TempTableNotDropped { get; set; }
            public List<SelectStarUsage> SelectStarUsage { get; set; }
            public List<IndexSuggestion> IndexSuggestions { get; set; }
            public List<ExpensiveSortOperation> ExpensiveSortOperations { get; set; }
        }

        public class errorSchema
        {
            public int lineNumber { get; set; }
            public string Message { get; set; }
        }
        public class TempTableNotDropped
        {
            public int LineNumber { get; set; }
            public string Message { get; set; }
        }

        public class SelectStarUsage
        {
            public int LineNumber { get; set; }
            public string Message { get; set; }
        }

        public class IndexSuggestion
        {
            public int LineNumber { get; set; }
            public string Column { get; set; }
            public string Table { get; set; }
            public string Suggestion { get; set; }
        }

        public class ExpensiveSortOperation
        {
            public int LineNumber { get; set; }
            public string Message { get; set; }
        }

        public class SecurityIssue
        {
            public int LineNumber { get; set; }
            public string Message { get; set; }
        }
    }
}
