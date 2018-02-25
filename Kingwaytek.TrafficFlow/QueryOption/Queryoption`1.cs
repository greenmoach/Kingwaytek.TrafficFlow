namespace Kingwaytek.TrafficFlow
{
    public class QueryOption<TModel, TFilter> : QueryOption<TModel>, IQueryOption<TModel, TFilter>
        where TFilter : new()
    {
        public QueryOption() : base()
        {
            this.Filter = new TFilter();
        }

        public virtual TFilter Filter { get; set; }
    }
}