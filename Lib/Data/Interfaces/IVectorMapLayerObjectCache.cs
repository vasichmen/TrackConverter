using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data.Providers.InternetServices;

namespace TrackConverter.Lib.Data.Interfaces
{
    //TODO: описание методов
    internal interface IVectorMapLayerObjectCache
    {
        bool ContainsVectorMapLayerObject(int id);

        Wikimapia.ExtInfo GetVectorMapLayerObject(int id);

        bool PutVectorMapLayerObject(Wikimapia.ExtInfo obj);

    }
}