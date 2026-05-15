
using System;

using WebForms_MovieManager.Services;

namespace WebForms_MovieManager.Components.Base
{
    public abstract class BaseComponentPresenter<TView, TModel>: IComponentPresenter<TView, TModel>
        where TView : IComponentView<TModel> 
        where TModel : class
    {
        public TView View { get; private set; }
        public IErrorLogger _logger;
        private bool _isLoadingData = false;

        protected BaseComponentPresenter(TView view, IErrorLogger logger=null)
        {
            View = view;
            _logger = logger ?? new ErrorLogger();
            SubscribeToviewEvents();
        }


        #region IComponentPresenter Methods
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

        public void LoadData()
        {
            if (_isLoadingData) return;

            try
            {
                View.SetLoadingState(true);
                TModel data = OnLoadData();
                View.DataSource = data;
                View.BindData();// This will fire ComponentDatachanged 
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"Error loading data for component {View.ComponentId}");
                View.ShowError("Failed to load data");
            }
            finally
            {
                _isLoadingData = false;
                View.SetLoadingState(false);
            }
        }

        public void Refresh()
        {
            LoadData();
        }
        #endregion

        #region BaseComponentPresenter Methods
       
        private void SubscribeToviewEvents()
        {
            if (View != null)
            {
                View.ComponentLoaded += OnComponentLoaded;
                View.ComponentDatachanged += OnComponentDatachanged;
            }
        }

        private void OnComponentDatachanged(object sender, EventArgs e)
        {
            if (!_isLoadingData)
                Refresh();
        }

        protected virtual void OnComponentLoaded(object sender, EventArgs e)
        {
            LoadData();
        }
        

        protected void RaiseDataChanged()
        {
            Refresh();
        }

        protected virtual void OnInitialize() { }

        protected abstract TModel OnLoadData();

        #endregion
    }
}