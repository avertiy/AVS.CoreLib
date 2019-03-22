﻿using System;
using System.Windows.Forms;
using AVS.CoreLib.Infrastructure;
using AVS.CoreLib.WinForms.MVC;

namespace AVS.CoreLib.WinForms.Controls
{
    public class FormEx: Form
    {
        public string FormTitle
        {
            get => this.Text;
            set => this.Text = value;
        }
        protected object _lazy;
        protected T Lazy<T>() where T : class
        {
            if (_lazy == null)
            {
                _lazy = EngineContext.Current.Resolve<T>();

            }
            return (T)_lazy;
        }
        
        public FormEx()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            try
            {
                Initialize();
            }
            catch (NotInitializedEngineContextException ex)
            {
                //it's ok 
                //VS designer tries to initialize controls 
                //but the enginecontext is not initialized in this case
            }
            catch (Exception ex)
            {
                var type = ex.GetType().Name;
                MessageBox.Show(ex.ToString(), $"UserControlEx.Initialize=> {type}");
            }
        }

        /// <summary>
        /// You must override this method and initialize control properties 
        /// call InitializeComponent in this method not in control's contructor
        /// </summary>
        protected virtual void Initialize()
        {
        }

        protected T GetController<T>() where T : class, IViewController
        {
            var ctrl = EngineContext.Current.Resolve<T>();
            ctrl.SetView(this);
            return ctrl;
        }

        /// <summary>
        /// if click event is not triggered 
        /// ensure this method is called after InitializeComponent (i.e. child controls are initialized)
        /// </summary>
        protected void WireAllControls(Control control)
        {
            foreach (Control ctl in control.Controls)
            {
                ctl.Click += Ctrl_Click;
                if (ctl.HasChildren)
                {
                    WireAllControls(ctl);
                }
            }
        }

        protected virtual void Ctrl_Click(object sender, EventArgs e)
        {
            this.InvokeOnClick(this, EventArgs.Empty);
        }
    }
}