using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using WebForms_MovieManager.Services;

namespace WebForms_MovieManager.Components.Base
{
    public abstract class BaseComponentPresenter<TView, TModel>
        : IComponentPresenter<TView, TModel>
        where TView : IComponentView<TModel> 
        where TModel : class
    {
        public TView View { get; private set; }
        protected readonly IErrorLogger _logger;

        protected BaseComponentPresenter(TView view, IErrorLogger logger=null)
        {
            View = view;
            _logger = logger ?? new ErrorLogger();
            SubscribeToviewEvents();
        }

        private void SubscribeToviewEvents()
        {
            View.ComponentLoaded += OnComponentLoaded;
            View.ComponentDatachanged += OnComponentDatachanged;
        }

        private void OnComponentDatachanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnComponentLoaded(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            try
            {
                View.SetLoadingState(true);
                OnInitialize();
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"Error initializing component {View.ComponentId}");
                View.ShowError("Failed to initialize component");
            }
        }

        private void OnInitialize()
        {
            throw new NotImplementedException();
        }

        public void LoadData()
        {
            try
            {
                View.SetLoadingState(true);
                war data = OnLoadData();
                View.Datasource = data,
View.BindData();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void Refresh()
        {
            throw new NotImplementedException();
        }
    }
}