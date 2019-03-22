using System.Windows.Forms;

namespace AVS.CoreLib.WinForms.Utils
{
    public interface IGridCellFormatter
    {
        void FormatCell(DataGridView grid, DataGridViewCellFormattingEventArgs args);
    }
}