namespace FutureState.AppCore.Data.Tests.Helpers.CustomTypes
{
    public class CustomDialect : ICustomDialect
    {
        public string LatLong { get {return "double(9, 6)"; }}
    }

    public interface ICustomDialect
    {
        string LatLong { get; }
    }
}