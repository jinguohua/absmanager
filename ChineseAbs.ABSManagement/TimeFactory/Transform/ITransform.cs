namespace ChineseAbs.ABSManagement.TimeFactory.Transform
{
    public interface ITransform
    {
        TimeSeriesFactory Transform(TimeSeriesFactory timeSeries);
    }
}
