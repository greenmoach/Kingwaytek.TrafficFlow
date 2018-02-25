namespace Kingwaytek.TrafficFlow
{
    public interface IQueryOptionMvc<out TModel> : IQueryOption<TModel>
    {
        string ActionName { get; set; }

        string ControllerName { get; set; }

        object RouteValues { get; set; }
    }
}