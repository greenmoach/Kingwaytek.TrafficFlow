namespace Kingwaytek.TrafficFlow
{
    public interface IIdentity<T>
    {
        T Id { get; set; }
    }
}