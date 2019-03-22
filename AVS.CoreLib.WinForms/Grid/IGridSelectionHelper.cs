using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AVS.CoreLib.WinForms.Grid
{
    public interface IGridSelectionHelper
    {
        string GetSelectedCellsSum(DataGridView grid);
        void Initialize(DataGridViewColumnCollection gridColumns);
    }

    public class GridSelectionHelper : IGridSelectionHelper
    {
        protected Dictionary<string, string> Columns = new Dictionary<string, string>();

        public virtual void Initialize(DataGridViewColumnCollection gridColumns)
        {
            foreach (DataGridViewColumn column in gridColumns)
            {
                var key = column.Name.Replace("Column", "");
                if (Columns.ContainsKey(key))
                    Columns[key] = column.Name;
                else
                    Columns.Add(key, column.Name);
            }
        }

        public string GetSelectedCellsSum(DataGridView grid)
        {
            var colIndex = -1;
            int rows = 0;
            double sum = 0;

            foreach (DataGridViewCell cell in grid.SelectedCells)
            {

                if (cell.Value is string || !cell.ValueType.IsValueType || !cell.ValueType.IsPrimitive)
                    continue;
                if (colIndex == -1)
                    colIndex = cell.ColumnIndex;
                if (colIndex != cell.ColumnIndex)
                    continue;
                rows++;
                sum += Convert.ToDouble(cell.Value);
            }

            return SelectedCellsSumFormat(rows, grid.RowCount, sum);
        }

        protected virtual string SelectedCellsSumFormat(int rows, int rowCount, double sum)
        {
            return $"Rows {rows} from {rowCount}   Sum: {sum:0.00}";
        }
    }
}