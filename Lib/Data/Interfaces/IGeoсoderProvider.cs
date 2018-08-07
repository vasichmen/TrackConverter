using GMap.NET;
using System;
using System.Collections.Generic;
using TrackConverter.Lib.Classes;
namespace TrackConverter.Lib.Data.Interfaces
{
   //TODO: описания методов
    interface IGeoсoderProvider
    {
        string GetAddress(Coordinate coordinate);
    
        Coordinate GetCoordinate(string address);

        TimeZoneInfo GetTimeZone(Coordinate coordinate);
    }
}
