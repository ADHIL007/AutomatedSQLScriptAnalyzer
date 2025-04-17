
namespace TestGG.Utilities
{
    public class DBQueryDictionary
    {
        public const string FETCH_ROW_COUNT = @"SELECT COUNT(*) FROM {0}";

        public const string FETCH_TABLE_FIELDS = @"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{0}'";

        public const string CHECK_TABLE_EXISTS = @"SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{0}'";

        public const string CHECK_SP_EXISTS = @"SELECT * FROM sys.procedures WHERE name = '{0}'";

        public const string MISSING_INDEXES = @"
        SELECT 
            mid.[name] AS IndexName, 
            mid.[type_desc] AS IndexType, 
            ms.[name] AS query, 
            mid.[description] AS IndexDescription 
        FROM sys.dm_db_missing_index_details AS mid
        INNER JOIN sys.tables AS ms 
            ON mid.[object_id] = ms.[object_id] 
        WHERE ms.name = '{0}'";

        public const string EXECUTION_PLAN = @"
        SET SHOWPLAN_ALL ON;
        {0};
        SET SHOWPLAN_ALL OFF;";

        public const string TOP_EXPENSIVE_QUERIES = @"
        SELECT TOP 10 
            qs.total_elapsed_time/1000.0 AS TotalSeconds, 
            qs.execution_count, 
            qs.total_elapsed_time/qs.execution_count AS AvgTime, 
            qs.query_hash, 
            SUBSTRING(qt.text, qs.statement_start_offset/2, 
                      (CASE WHEN qs.statement_end_offset = -1 
                            THEN LEN(CONVERT(NVARCHAR(MAX), qt.text)) * 2 
                            ELSE qs.statement_end_offset END - qs.statement_start_offset)/2) AS QueryText
        FROM sys.dm_exec_query_stats AS qs
        CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) AS qt
        ORDER BY TotalSeconds DESC;";

        public const string TABLE_SIZE = @"
        SELECT 
            t.name AS query, 
            SUM(p.rows) AS RowCount, 
            SUM(a.total_pages) * 8 AS TotalSpaceKB, 
            SUM(a.used_pages) * 8 AS UsedSpaceKB
        FROM sys.tables AS t
        INNER JOIN sys.indexes AS i ON t.[object_id] = i.[object_id]
        INNER JOIN sys.partitions AS p ON i.[object_id] = p.[object_id]
        INNER JOIN sys.allocation_units AS a ON p.partition_id = a.container_id
        WHERE t.name = '{0}'
        GROUP BY t.name;";

        public const string INDEX_FRAGMENTATION = @"
        SELECT 
            ix.name AS IndexName,
            ps.avg_fragmentation_in_percent AS FragmentationPercent
        FROM sys.dm_db_index_physical_stats (NULL, NULL, NULL, NULL, 'DETAILED') AS ps
        INNER JOIN sys.indexes AS ix ON ps.[object_id] = ix.[object_id]
        WHERE ix.object_id = OBJECT_ID('{0}')";

        public const string GET_TABLE_INDEXES = @"
        SELECT ix.name AS IndexName, 
               ix.type_desc AS IndexType, 
               ic.key_ordinal AS ColumnOrdinal, 
               c.name AS ColumnName 
        FROM sys.indexes ix
        INNER JOIN sys.index_columns ic 
            ON ix.object_id = ic.object_id AND ix.index_id = ic.index_id
        INNER JOIN sys.columns c 
            ON ic.object_id = c.object_id AND ic.column_id = c.column_id
        WHERE ix.object_id = OBJECT_ID('{0}')";

        public const string GET_INDEX_DETAILS = @"
        SELECT ix.name AS IndexName, 
               ix.type_desc AS IndexType, 
               ic.key_ordinal AS ColumnOrdinal, 
               c.name AS ColumnName, 
               ic.is_descending_key AS IsDescending
        FROM sys.indexes ix
        INNER JOIN sys.index_columns ic 
            ON ix.object_id = ic.object_id AND ix.index_id = ic.index_id
        INNER JOIN sys.columns c 
            ON ic.object_id = c.object_id AND ic.column_id = c.column_id
        WHERE ix.name = '{0}'";

        public const string CHECK_INDEX_EXISTS = @"
        SELECT 1 
        FROM sys.indexes 
        WHERE object_id = OBJECT_ID('{0}') AND name = '{1}'";

        public string GetQueryById(int id, string query = "", string indexName = "")
        {
            switch (id)
            {
                case 1000:
                    return string.Format(FETCH_ROW_COUNT, query);
                case 1001:
                    return string.Format(FETCH_TABLE_FIELDS, query);
                case 1002:
                    return string.Format(CHECK_TABLE_EXISTS, query);
                case 1003:
                    return string.Format(CHECK_SP_EXISTS, query);
                case 1004:
                    return string.Format(MISSING_INDEXES, query);
                case 1005:
                    return string.Format(EXECUTION_PLAN, query);
                case 1006:
                    return TOP_EXPENSIVE_QUERIES;
                case 1007:
                    return string.Format(TABLE_SIZE, query);
                case 1008:
                    return string.Format(INDEX_FRAGMENTATION, query);
                case 1009:
                    return string.Format(GET_TABLE_INDEXES, query);
                case 1010:
                    return string.Format(GET_INDEX_DETAILS, indexName);
                case 1011:
                    return string.Format(CHECK_INDEX_EXISTS, query, indexName);
                default:
                    return "Query not found!";
            }
        }
    }


}
