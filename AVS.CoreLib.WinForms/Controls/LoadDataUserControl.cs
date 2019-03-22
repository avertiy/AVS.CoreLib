using System;
using System.ComponentModel;
using System.Diagnostics;

namespace AVS.CoreLib.WinForms.Controls
{
    [DefaultProperty("DataSource")]
    public partial class LoadDataUserControl : UserControlEx
    {
        [DefaultValue(null)]
        public object DataSource
        {
            get => this.bindingSource.DataSource;
            set
            {
                this.bindingSource.DataSource = value;
                OnDataBound(value);
            }
        }
        
        protected override void Initialize()
        {
            base.Initialize();
            InitializeComponent();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                e.Result = DoWork(e.Argument);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message, "Background worker error");
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Result == null || e.Error != null || e.Cancelled)
                return;
            WorkCompleted(e.Result);
        }

        protected virtual void WorkCompleted(object result)
        {
            DataSource = result;
        }

        protected virtual void RunWorkerAsync(object arg)
        {
            if (backgroundWorker.IsBusy)
            {
                return;
            }
            backgroundWorker.RunWorkerAsync(arg);
        }

        protected virtual object DoWork(object argument)
        {
            return null;
        }

        protected virtual void OnDataBound(object dataSource)
        {
        }
    }
}
