using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;

namespace AVS.CoreLib._System.Debug
{
    public interface IQuickWatch
    {
        string Watch(DataTable dt, int rowCount = 5);
        string Watch(DataTable dt, DataRow row);
        string Watch(XmlDocument xmlDoc);
    }

    public class QuickWatch: IQuickWatch
    {
        public int CallStackDepth { get; set; } = 4;

        public virtual string Watch(DataTable dt, int rowCount = 5)
        {
            var sb = new StringBuilder();
            if (dt == null)
            {
                sb.AppendLine("Table is NULL");
            }
            else
            {
                sb.AppendLine($"Table [{dt.TableName}] Rows - {dt.Rows.Count} Cols - {dt.Columns.Count}");
                sb.AppendLine("---------------------------------------------------------------------------");
                int i = 0;
                foreach (DataColumn col in dt.Columns)
                {
                    sb.Append($"[{col.ColumnName}]\t ");
                }
                foreach (DataRow row in dt.Rows)
                {
                    sb.AppendLine();
                    sb.Append($"row[{i}] => ");
                    foreach (DataColumn col in dt.Columns)
                    {
                        sb.Append($"\"{row[col.ColumnName]}\"\t ");
                    }
                    i++;
                    if (i > rowCount)
                        break;
                }
                sb.AppendLine("---------------------------------------------------------------------------");
            }
            if (CallStackDepth > 0)
                sb.AppendLine($"\r\ncall stack{DebugUtil.GetCallStack(CallStackDepth, 2)}\r\n");

            return sb.ToString();
        }

        public virtual string Watch(DataTable dt, DataRow row)
        {
            var sb = new StringBuilder();
            if (dt == null)
            {
                sb.AppendLine("Table is NULL");
            }
            else
            {
                sb.AppendLine($"Table [{dt.TableName}] Rows - {dt.Rows.Count} Cols - {dt.Columns.Count}");
                sb.AppendLine("---------------------------------------------------------------------------");
                int i = 0;
                foreach (DataColumn col in dt.Columns)
                {
                    sb.Append($"[{col.ColumnName}]\t ");
                }

                sb.AppendLine();
                sb.Append($"row[{i}] => ");
                foreach (DataColumn col in dt.Columns)
                {
                    sb.Append($"\"{row[col.ColumnName]}\"\t ");
                }
                sb.AppendLine("---------------------------------------------------------------------------");
            }
            if (CallStackDepth > 0)
                sb.AppendLine($"\r\ncall stack{DebugUtil.GetCallStack(CallStackDepth, 2)}\r\n");

            return sb.ToString();
        }

        public virtual string Watch(XmlDocument xmlDoc)
        {
            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                xmlDoc.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();

                var sb = stringWriter.GetStringBuilder();
                if (CallStackDepth > 0)
                    sb.AppendLine($"\r\ncall stack{DebugUtil.GetCallStack(CallStackDepth, 2)}\r\n");

                return sb.ToString();
            }
        }

        public virtual string Watch<T>(IList<T> dt, int count = 10)
        {
            throw new NotImplementedException();
        }

        public virtual string Watch<K, V>(IDictionary<K, V> dict, int count = 10)
        {
            throw new NotImplementedException();
        }
    }
}