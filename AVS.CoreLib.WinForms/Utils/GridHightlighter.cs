using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace AVS.CoreLib.WinForms.Utils
{
    public enum ColorSchemeScope
    {
        RowBackColor = 0,
        RowForeColor,
        CellBackColor,
        CellForeColor
    }
    public class GridColorScheme
    {
        public int ColumnIndex { get; set; }
        public IEnumerable Values { get; set; }
        public Color[] Colors { get; set; }
        public Func<IComparable, object, bool> Condition;

        public Func<object, IComparable> RowValueConverter;


        public ColorSchemeScope Scope { get; set; }

        public GridColorScheme WithDefaultCondition()
        {
            Condition = (rowValue, value) => rowValue.CompareTo(value) < 0;
            return this;
        }

        public GridColorScheme WithCondition(Func<IComparable, object, bool> condition)
        {
            Condition = condition;
            return this;
        }


        public GridColorScheme WithEquityCondition()
        {
            Condition = (rowValue, value) => rowValue.CompareTo(value) == 0;
            return this;
        }

        public GridColorScheme WithEnumEquityCondition()
        {
            Condition = (rowValue, value) => ((int)rowValue).CompareTo((int)value) == 0;
            return this;
        }

        public GridColorScheme UseDefaultBackColors()
        {
            Colors = new[]{
                    System.Drawing.SystemColors.Window,
                    Color.WhiteSmoke,
                    Color.LightYellow,
                    Color.LightCyan,
                    Color.LightBlue,
                    Color.Gold,
                    Color.Yellow,
                    Color.CornflowerBlue,
                    Color.LightGreen,
                    Color.Orange
                };
            return this;
        }

        private void SetColor(DataGridViewRow row, Color color)
        {
            switch (Scope)
            {
                case ColorSchemeScope.CellBackColor:
                    row.Cells[ColumnIndex].Style.BackColor = color;
                    break;
                case ColorSchemeScope.CellForeColor:
                    row.Cells[ColumnIndex].Style.ForeColor = color;
                    break;
                case ColorSchemeScope.RowBackColor:
                    row.DefaultCellStyle.BackColor = color;
                    break;
                case ColorSchemeScope.RowForeColor:
                    row.DefaultCellStyle.ForeColor = color;
                    break;
            }
        }

        public void Execute(DataGridViewRow row)
        {
            if ((!(row.Cells[ColumnIndex].Value is IComparable rowValueObj)))
                return;
            int i = 0;

            if (Colors == null)
                this.UseDefaultBackColors();

            if (RowValueConverter != null)
            {
                rowValueObj = RowValueConverter(row.Cells[ColumnIndex].Value);
            }

            foreach (var value in Values)
            {
                if (i >= Colors.Length)
                    break;

                if (Condition == null)
                {
                    if (rowValueObj.CompareTo(value) > 0)
                    {
                        SetColor(row, Colors[i]);
                    }
                }
                else if (rowValueObj is string)
                {
                    if (double.TryParse((string)rowValueObj, out double d) && Condition(d, (IComparable) value))
                    {
                        SetColor(row, Colors[i]);
                        break;
                    }
                }
                else if (Condition(rowValueObj, (IComparable)value))
                {
                    SetColor(row, Colors[i]);
                    break;
                }

                i++;
            }
        }
    }

    public class GridHightlighter
    {
        public GridHightlighter()
        {
            Schemes = new List<GridColorScheme>();
        }

        public List<GridColorScheme> Schemes { get; set; }

        public void AddScheme(GridColorScheme scheme)
        {
            Schemes.Add(scheme);
        }

        public void Execute(DataGridView grid)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                foreach (var scheme in Schemes)
                {
                    scheme.Execute(row);
                }
            }
        }
        public void Execute(DataGridView grid, Func<object, IComparable> valueConverter)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                foreach (var scheme in Schemes)
                {
                    scheme.RowValueConverter = valueConverter;
                    scheme.Execute(row);
                }
            }
        }

        public GridHightlighter WithScaleColorScheme(DataGridViewColumn column, double[] values)
        {
            var colorScheme = new GridColorScheme()
            {
                ColumnIndex = column.Index,
                Scope = ColorSchemeScope.RowBackColor,
                Values = values,
            };
            AddScheme(colorScheme.UseDefaultBackColors().WithDefaultCondition());
            return this;
        }

        public GridHightlighter WithScaleColorScheme(DataGridViewColumn column)
        {
            var colorScheme = new GridColorScheme()
            {
                ColumnIndex = column.Index,
                Scope = ColorSchemeScope.RowBackColor,
                Values = new[] { 1.0, 2.0, 5.0, 10, 15, 25, 50, 100 },
            };
            AddScheme(colorScheme.UseDefaultBackColors().WithDefaultCondition());
            return this;
        }

        public GridHightlighter WithLowScaleColorScheme(DataGridViewColumn column)
        {
            var colorScheme = new GridColorScheme()
            {
                ColumnIndex = column.Index,
                Scope = ColorSchemeScope.RowBackColor,
                Values = new[] { 0.01, 0.1, 1.0, 2.0, 5.0, 10, 15, 25 }
            };
            AddScheme(colorScheme.UseDefaultBackColors().WithDefaultCondition());
            return this;
        }
    }
}
