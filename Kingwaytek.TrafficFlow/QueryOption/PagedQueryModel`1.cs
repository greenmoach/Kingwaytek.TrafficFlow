namespace Kingwaytek.TrafficFlow
{
    public class PagedQueryModel<TModel, TFilter> : QueryOption<TModel, TFilter>, IQueryOptionMvc<TModel>
        where TFilter : new()
    {
        public PagedQueryModel() : base()
        {
            ActionName = "List";
        }

        public string ActionName { get; set; }

        public string ControllerName { get; set; }

        public object RouteValues { get; set; }
    }
}