﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using System.Linq;
using Tortuga.Chain.CommandBuilders;

#if SQL_SERVER_SDS

using System.Data.SqlClient;

#elif SQL_SERVER_MDS

using Microsoft.Data.SqlClient;

#endif

namespace Tortuga.Chain.SqlServer
{
    internal static class Utilities
    {
        /// <summary>
        /// Gets the parameters from a SQL Builder.
        /// </summary>
        /// <param name="sqlBuilder">The SQL builder.</param>
        /// <returns></returns>
        public static List<SqlParameter> GetParameters(this SqlBuilder<SqlDbType> sqlBuilder)
        {
            return sqlBuilder.GetParameters(ParameterBuilderCallback);
        }

        public static SqlParameter ParameterBuilderCallback(SqlBuilderEntry<SqlDbType> entry)
        {
            var result = new SqlParameter();
            result.ParameterName = entry.Details.SqlVariableName;
            result.Value = entry.ParameterValue;

            if (entry.Details.DbType.HasValue)
                result.SqlDbType = entry.Details.DbType.Value;

            if (entry.ParameterValue is DbDataReader)
                result.SqlDbType = SqlDbType.Structured;

            return result;
        }

        /// <summary>
        /// Triggers need special handling for OUTPUT clauses.
        /// </summary>
        public static void UseTableVariable<TDbType>(this SqlBuilder<TDbType> sqlBuilder, SqlServerTableOrViewMetadata<TDbType> Table, out string? header, out string? intoClause, out string? footer) where TDbType : struct
        {
            if (sqlBuilder.HasReadFields && Table.HasTriggers)
            {
                header = "DECLARE @ResultTable TABLE( " + string.Join(", ", sqlBuilder.GetSelectColumnDetails().Select(c => c.QuotedSqlName + " " + c.FullTypeName + " NULL")) + ");" + Environment.NewLine;
                intoClause = " INTO @ResultTable ";
                footer = Environment.NewLine + "SELECT * FROM @ResultTable";
            }
            else
            {
                header = null;
                intoClause = null;
                footer = null;
            }
        }
    }
}
