namespace Kingwaytek.TrafficFlow
{
    public interface IEditColumn
    {
        System.DateTime CreateTime { get; set; }

        System.DateTime? LastEditTime { get; set; }
    }
}