using System;

namespace WebForms_MovieManager.Components.Base
{
    public interface IComponentView
    {
        string ComponentId { get; }
        event EventHandler ComponentLoaded;
        event EventHandler ComponentDatachanged;
        void ShowError(string message);
        void ShowMessage(string message);
        void SetLoadingState(bool isLoading);
    }

    public interface IComponentView<TModel> 
        : IComponentView where TModel : class
    {
        TModel DataSource { get; set; }
        void BindData();
        void ClearData();
    }
    
}