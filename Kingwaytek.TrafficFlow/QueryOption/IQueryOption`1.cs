namespace Kingwaytek.TrafficFlow
{
    public interface IQueryOption<out TModel, TFilter> : IQueryOption<TModel> where TFilter : new()
    {
        TFilter Filter { get; set; }
    }
}