using System;
using System.Data.SqlClient;
using System.Text;

namespace AVS.CoreLib.Data.EF
{
    public class SqlBulkCopyException : Exception
    {
        public string Details { get;}
        public SqlBulkCopyException(string message, SqlBulkCopyColumnMappingCollection mappings):base(message)
        {
            Details = StringifyColumnMappings(mappings);
        }

        private string StringifyColumnMappings(SqlBulkCopyColumnMappingCollection mappings)
        {
            var sb = new StringBuilder($"Column mappings [{mappings.Count}]:");
            foreach (SqlBulkCopyColumnMapping mapping in mappings)
            {
                sb.AppendLine($"{mapping.SourceColumn} => {mapping.DestinationColumn}");
            }

            sb.AppendLine("Check columns mapping match column names (case sencetive) in database table.");
            sb.AppendLine("Mark entity property with [NotMapped] attribute if any is missing in database");
            return sb.ToString();
        }

        public override string ToString()
        {
            return $"{Message}\r\n{Details}\r\nStackTrace: {StackTrace}";
        }
    }
}