
namespace WebForms_MovieManager.Components.Communication
{
    public static class EventAggregatorProvider
    {
        private static readonly IComponentEventAggregator _instance = new ComponentEventAggregator();
        public static IComponentEventAggregator Instance => _instance;
    }
}