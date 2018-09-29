using System.Drawing;
using GMap.NET;
using GMap.NET.WindowsForms;
using TrackConverter.Lib.Classes;

namespace TrackConverter.Lib.Maping.GMap
{

    /// <summary>
    /// маркер на карте
    /// </summary>
    public class MapMarker: GMapMarker
    {
        /// <summary>
        /// если истина, то на маркер можно нажать и появится окно редактирования
        /// </summary>
        public bool Clickable { get; private set; }

        /// <summary>
        /// информация о маркере, тип
        /// </summary>
        public new MarkerTag Tag;

        /// <summary>
        /// картинка маркера
        /// </summary>
        private readonly Icon image;

        /// <summary>
        /// создает новый маркер с указанным изображением и координатами
        /// </summary>
        /// <param name="point">географические координаты</param>
        /// <param name="image">изображение</param>
        public MapMarker(PointLatLng point, Icon image, bool clickable)
            : this(point, image, 0, 0,  clickable)
        { }

        /// <summary>
        /// создает новый маркер в указанно точке, заддной иконкой и отступами
        /// </summary>
        /// <param name="point">координаты точки</param>
        /// <param name="image">иконка маркера</param>
        /// <param name="offsetY">Сдвиг в пикселях по Y. Отрицательнвй сдвиг сдвигаем маркер вверх</param>
        /// <param name="offsetX">Сдвиг в пикселях по Х. Отрицательнвй сдвиг сдвигаем маркер влево</param>
        public MapMarker(PointLatLng point, Icon image, int offsetX, int offsetY, bool clickable)
            : base(point)
        {
            Size = new Size(image.Width, image.Height);
            Offset = new Point(-Size.Width / 2 + offsetX, -Size.Height / 2 + offsetY);
            this.image = image;
            this.Clickable = clickable;
        }

        /// <summary>
        /// создает новый маркер в указанно точке, заддной иконкой и отступами
        /// </summary>
        /// <param name="point">координаты точки</param>
        /// <param name="image">иконка маркера</param>
        /// <param name="offsets">Отступы по Х и по Y. Отрицательные отступы сдвигают влево и вверх соответственно</param>
        public MapMarker(PointLatLng point, Icon image, Point offsets, bool clickable)
            : this(point, image, offsets.X, offsets.Y,  clickable)
        { }

        /// <summary>
        /// отрисовка маркера на заданной поверхности
        /// </summary>
        /// <param name="g">поверхность для рисования</param>
        public override void OnRender(Graphics g)
        {
            if (image == null)
                return;

            Rectangle rect = new Rectangle(LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height);
            g.DrawIcon(image, rect);

        }
    }
}
