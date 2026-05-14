namespace WebForms_MovieManager.Components.Base
{
    public interface IComponentPresenter<TView, TModel>
        where TView : IComponentView<TModel>
        where TModel : class
    {
        TView View { get; }
        void Initialize();
        void LoadData();
        void Refresh();
    }
}
