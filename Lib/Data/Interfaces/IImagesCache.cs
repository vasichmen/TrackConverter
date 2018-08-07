using System.Drawing;

namespace TrackConverter.Lib.Data.Interfaces
{
    //TODO: описания методов
    public interface IImagesCache
    {
        bool CheckImage(string name);
        Image GetImage(string name);
        bool PutImage(string name, Image data);
        
    }
}