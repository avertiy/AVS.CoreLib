using System;
using System.Collections.Generic;
using System.Text;

namespace AVS.CoreLib.Data.Utils
{
    public class SqlStringBuilder
    {
        private readonly StringBuilder _sb = new StringBuilder();

        private readonly bool _unInstallMode = false;
        public SqlStringBuilder( bool unInstallMode)
        {
           // _ctx = ctx;
            _unInstallMode = unInstallMode;
        }

        public int Length { get { return _sb.Length; } set { _sb.Length = value; } }

        public void Append_FK_ConstraintScript(string fkTable, string pkTable, string fkColumnName, string pkColumnName = "Id")
        {
            var sql =
                "\r\nALTER TABLE [dbo].[{0}]  WITH CHECK ADD  CONSTRAINT [FK_{0}_{1}] FOREIGN KEY([{2}]) REFERENCES [dbo].[{1}] ([{3}]) ON DELETE CASCADE;\r\n";
            _sb.AppendFormat(sql, fkTable, pkTable, fkColumnName, pkColumnName);
        }

        public void Append_DROP_FK_ConstraintScript(string fkTable, string pkTable)
        {
            var sql = "\r\nALTER TABLE [dbo].[{0}] DROP CONSTRAINT [FK_{0}_{1}];\r\n";
            _sb.AppendFormat(sql, fkTable, pkTable);
        }

        public override string ToString()
        {
            return _sb.ToString();
        }

        public void Append(string value)
        {
            _sb.AppendLine(value);
        }

        public void Append_ForeignKey(string pkTable, string fkTable, string fkColumnName, string pkColumnName = "Id")
        {
            if(_unInstallMode)
                this.Append_DROP_FK_ConstraintScript(pkTable, fkTable);
            else
                this.Append_FK_ConstraintScript(pkTable, fkTable, fkColumnName, pkColumnName);
        }

        /// <summary>
        /// Drop a plugin table
        /// </summary>
        /// <param name="tableName">Table name</param>
        public void Append_DropTable(string tableName)
        {
            if (String.IsNullOrEmpty(tableName))
                throw new ArgumentNullException("tableName");

            //drop the table
            //"SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = {0}", tableName
            _sb.AppendFormat("DROP TABLE [{0}];\r\n", tableName);
        }

    }
}