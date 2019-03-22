using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AVS.CoreLib.WinForms.MVC
{
    public abstract class ControllerBase
    {
        protected void SafeExecute(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                var stackTrace = ex.StackTrace;
                MessageBox.Show(msg);
            }
        }

        protected T SafeExecute<T>(Func<T> action, bool rethrow = true)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                var stackTrace = ex.StackTrace;
                if(rethrow)
                    throw ex;
                return default(T);
            }
        }

        protected async Task SafeExecute(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                var stackTrace = ex.StackTrace;
                throw ex;
            }
        }
    }

    public abstract class ControllerBase<TView> : ControllerBase, IViewController
        where TView : class
    {
        /// <summary>
        /// returns the view, if it is null throws NotInitializedViewException
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="view"></param>
        /// <returns></returns>
        protected T GetView<T>(T view) where T : class
        {
            if (view == null)
                throw new NotInitializedViewException(typeof(T).Name);
            return view;
        }

        private TView _view;
        protected TView View => GetView(_view);

        public void SetView(TView view)
        {
            _view = view;
        }

        public virtual void SetView(object view)
        {
            SetView((TView)view);
        }
    }
}