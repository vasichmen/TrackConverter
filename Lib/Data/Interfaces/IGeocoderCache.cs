using TrackConverter.Lib.Classes;

namespace TrackConverter.Lib.Data.Interfaces
{
    //TODO: описания методов
    public interface IGeocoderCache
    {
        void ClearGeocoder();
        string GetAddress(Coordinate coordinate);
        Coordinate GetCoordinate(string address);
        void PutGeocoder(Coordinate Coordinate, string Address);
    }
}