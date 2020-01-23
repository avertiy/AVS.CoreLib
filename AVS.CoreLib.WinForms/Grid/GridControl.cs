using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AVS.CoreLib.WinForms.MVC;
using AVS.CoreLib.WinForms.Utils;

namespace AVS.CoreLib.WinForms.Grid
{
    public interface IGridControl
    {
        object DataSource { get; set; }
        DataGridView DataGrid { get; }
        string GridSummaryText { get; set; }
        void ReportProgress(string message, int percentage);
        void SetError(string error);
    }

    public partial class GridControl : UserControl, IGridControl 
    {
        private static readonly object LoadDataCompletedKey = new object();
        #region Prop-s

        [Browsable(false)]
        public IGridViewController Controller { get; set; }

        [Browsable(false)]
        public GridHightlighter Hightlighter { get; } = new GridHightlighter();

        [Browsable(false)]
        public IGridCellFormatter CellFormatter { get; set; }

        /// <summary>
        /// requires StatusLabel to be initialized
        /// </summary>
        public string GridSummaryText
        {
            get => lblStatus.Text;
            set => lblStatus.Text = value;
        }

        public void SetError(string error)
        {
            lblStatus.Text = error;
            lblStatus.ForeColor = Color.Red;
        }

        public DataGridViewHeaderBorderStyle RowHeadersBorderStyle
        {
            get => grid.RowHeadersBorderStyle;
            set => grid.RowHeadersBorderStyle = value;
        }

        [Browsable(false)]
        public DataGridView DataGrid => grid;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(typeof(ExtendedDataGridViewColumnCollectionEditor), typeof(UITypeEditor))]
        [MergableProperty(false)]
        public DataGridViewColumnCollection Columns => this.grid.Columns;

        [Browsable(false)]
        public object DataSource
        {
            get => this.bindingSource.DataSource;
            set => this.bindingSource.DataSource = value;
        }
        
        /// <summary>Occurs when the background operation has completed, has been canceled, or has raised an exception.</summary>
        [Category("Async")]
        [Description("GridControl_LoadDataCompleted")]
        public event RunWorkerCompletedEventHandler LoadDataCompleted
        {
            add => this.Events.AddHandler(GridControl.LoadDataCompletedKey, (Delegate)value);
            remove => this.Events.RemoveHandler(GridControl.LoadDataCompletedKey, (Delegate)value);
        }

        protected virtual void OnLoadDataCompleted(RunWorkerCompletedEventArgs e)
        {
            var handler = (RunWorkerCompletedEventHandler)this.Events[GridControl.LoadDataCompletedKey];
            handler?.Invoke((object)this, e);
        }

        #endregion

        public GridControl()
        {
            InitializeComponent();
            GridSummaryText = string.Empty;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToOrderColumns = true;
            grid.AllowUserToResizeColumns = true;
        }

        public void RunLoadDataAsync(object argument)
        {
            GridSummaryText = "Loading data..";

            if (IsWorkerBusy())
            {
                throw new Exception("Background worker is still busy, please wait.");
            }

            backgroundWorker.RunWorkerAsync(argument);
        }
        
        public void ReportProgress(string message, int percentage)
        {
            backgroundWorker.ReportProgress(percentage, message);
        }

        

        public void BindData(object data)
        {
            GridSummaryText = "Binding data.. 90%";
            Controller.BindData(data);
            GridSummaryText = "Highlighting grid.. 95%";
            Hightlighter.Execute(grid);
            GridSummaryText = $"Row count {grid.RowCount}";
        }

        #region background worker

        private bool IsWorkerBusy()
        {
            if (backgroundWorker.IsBusy)
            {
                this.Cursor = Cursors.WaitCursor;
                var i = 0;
                while (backgroundWorker.IsBusy && i++ <= 10)
                    Thread.Sleep(1000);

                if (backgroundWorker.IsBusy)
                {
                    if (backgroundWorker.WorkerSupportsCancellation)
                    {
                        backgroundWorker.CancelAsync();
                        Thread.Sleep(1000);
                    }
                }
                this.Cursor = Cursors.Default;
            }

            return backgroundWorker.IsBusy;
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var message = e.UserState as string;
            GridSummaryText = $"{message} Progress: {e.ProgressPercentage}%";
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //we can only load data here, data binding is impossible in background process
            var data = Controller.LoadData(e.Argument);
            e.Result = Controller.ToSortableBindingList(data);
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                GridSummaryText = "Operation has been canceled";
            }
            else if (e.Error != null)
            {
                GridSummaryText = "Error occured: " + e.Error.Message;
            }
            else
            {
                BindData(e.Result);
            }
            OnLoadDataCompleted(e);
        } 
        #endregion

        private void grid_SelectionChanged(object sender, EventArgs e)
        {
            Controller.GridSelectionChanged();//grid
        }

        private void grid_Sorted(object sender, EventArgs e)
        {
            Hightlighter.Execute(grid);
        }

        private void grid_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataGridViewCell cell in grid.SelectedCells)
            {
                if (cell.Value != null)
                    sb.Append(cell.Value.ToString()+" ");
            }

            if (sb.Length > 1)
            {
                sb.Length--;
                Clipboard.SetText(sb.ToString());
            }
        }

        private void grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            CellFormatter?.FormatCell(grid, e);
        }
    }
}
