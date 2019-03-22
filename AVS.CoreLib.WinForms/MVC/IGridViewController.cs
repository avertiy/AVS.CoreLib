using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AVS.CoreLib.Utils;
using AVS.CoreLib.WinForms.Grid;
using AVS.CoreLib._System.ComponentModel;

namespace AVS.CoreLib.WinForms.MVC
{
    public interface IGridViewController : IViewController
    {
        object LoadData(object argument);
        object ToSortableBindingList(object dataSource);
        void BindData(object dataSource);
        void GridSelectionChanged();
    }

    /// <summary>
    /// GridViewController base class, helps bind data
    /// </summary>
    /// <typeparam name="TView">View</typeparam>
    /// <typeparam name="TEntity">Entity or Model</typeparam>
    public abstract class GridViewController<TView, TEntity> : ControllerBase<TView>, IGridViewController
        where TView : class, IGridView
    {
        protected DataGridView DataGrid => View.GridControl.DataGrid;
        protected IGridSelectionHelper GridSelectionHelper;

        public abstract object LoadData(object argument);

        public virtual object ToSortableBindingList(object dataSource)
        {
            if (dataSource == null)
                return null;
            if (dataSource is IEnumerable<TEntity> list)
                return new SortableBindingList<TEntity>(list);
            throw new ArgumentException($"DataSource is expected of type IList<{typeof(TEntity).Name}>");
        }

        /// <summary>
        /// GridControl passes source of a SortableBindingList type
        /// </summary>
        /// <param name="source"></param>
        public virtual void BindData(object source)
        {
            View.GridControl.DataSource = source;
            BindData_SummaryView(source);
        }

        protected virtual void BindData_SummaryView(object source)
        {
            if (View.SummaryView != null)
                View.SummaryView.DataSource = source;
        }

        public virtual void GridSelectionChanged()
        {
            View.GridControl.GridSummaryText = GridSelectionHelper.GetSelectedCellsSum(DataGrid);
        }

        public override void SetView(object view)
        {
            base.SetView(view);
            //we can't initialize GridSelectionHelper in c-tor due to View is set after the Controller c-tor is called
            InitializeGridSelectionHelper();
        }

        public void DisplaySelectedCellsSummary()
        {
            View.SelectedCellsSummaryView?.Initialize(DataGrid.SelectedCells);
        }

        protected virtual void InitializeGridSelectionHelper()
        {
            GridSelectionHelper = new GridSelectionHelper();
            GridSelectionHelper.Initialize(DataGrid.Columns);
        }
    }

    
}