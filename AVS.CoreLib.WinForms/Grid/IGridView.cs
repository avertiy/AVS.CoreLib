using System.Windows.Forms;

namespace AVS.CoreLib.WinForms.Grid
{
    public interface IGridView
    {
        IGridControl GridControl { get; }
        ISummaryView SummaryView { get; }
        ISelectedCellsSummaryView SelectedCellsSummaryView { get; }
    }

    public interface ISummaryView
    {
        object DataSource { get; set; }
    }
    public interface ISelectedCellsSummaryView
    {
        void Initialize(DataGridViewSelectedCellCollection selectedCells);
    }
}