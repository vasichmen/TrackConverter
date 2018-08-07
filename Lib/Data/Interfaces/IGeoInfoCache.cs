using System;
using System.Collections.Generic;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Data.Interfaces
{
    //TODO: описания методов
    public interface IGeoInfoCache
    {
        void ClearAltitudes();
        double GetElevation(Coordinate coordinate);
        TimeZoneInfo GetTimeZone(Coordinate coordinates);
        void PutGeoInfo(Coordinate coordinates, TimeZoneInfo tzi);
        void PutGeoInfo(Coordinate Coordinate, double Altitude);
        void PutGeoInfo(BaseTrack track, List<double> els, Action<string> callback = null);
        bool TryGetElevations(ref BaseTrack track);
    }
}