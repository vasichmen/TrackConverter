using GMap.NET.MapProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Data.Interfaces;
using GMap.NET;
using GMap.NET.WindowsForms;

namespace TrackConverter.Lib.Data.Providers.InternetServices
{
    //TODO: комментарии
    class GGC : BaseConnection
    {
        public GGC(string cacheDirectory, int duration = 24 * 7) : base(cacheDirectory, duration) { }

        public Guid ID
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int MaxAttempts
        {
            get
            {
                return 3;
            }
        }

        public override TimeSpan MinQueryInterval
        {
            get
            {
                return TimeSpan.FromMilliseconds(100);
            }
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public PureProjection Projection
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public GMapImage GetTileStream(GPoint point, int zoom)
        {
            throw new NotImplementedException();
        }
    }
}
