using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Mathematic.Geodesy.Systems
{
    /// <summary>
    /// система координат
    /// </summary>
    public abstract class BaseSystem : IGeosystem
    {

        #region параметры эллипсоида

        /// <summary>
        /// скорость света в вакууме, м/с
        /// </summary>
        public abstract double LightSpeed { get; }

        /// <summary>
        /// Геоцентрическая гравитационная постоянная(с учетом атмосферы), м^3/с^2
        /// </summary>
        public abstract double f { get; }

        /// <summary>
        /// Угловая скорость вращения Земли, рад/с
        /// </summary>
        public abstract double AngleSpeed { get; }

        /// <summary>
        /// Больший радиус общеземного эллипсоида (к экватору), м
        /// </summary>
        public abstract double MaxAxis { get; }

        /// <summary>
        /// Полярное сжатие общеземного  эллипсоида. 
        /// Compression = (MaxAxis - MinAxis)/MaxAxis
        /// </summary>
        public abstract double Compression { get; }

        /// <summary>
        /// Средний радиус, м
        /// </summary>
        public double AverageRadius { get { return (MinAxis + MaxAxis) / 2; } }

        /// <summary>
        /// Малый радиус (к полюсу), м
        /// </summary>
        public double MinAxis { get { return MaxAxis - Compression * MaxAxis; } }

        /// <summary>
        /// Длина экватора, м
        /// </summary>
        public double Equator { get { return 2 * Math.PI * MaxAxis; } }

        /// <summary>
        /// Длина меридина, м
        /// </summary>
        public double Meridian { get { return 2 * Math.PI * MinAxis; } }

        /// <summary>
        /// Расстояние в одном градусе широты
        /// </summary>
        public double In1Lat { get { return Meridian / 360d; } }

        #endregion

        #region Реализации интерфейсов

        /// <summary>
        /// вычисление расстояния между точками в метрах
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public double CalculateDistance(TrackPoint p1, TrackPoint p2)
        {
            return CalcDistV4(p1,p2);
        }

        /// <summary>
        /// вычисление магнитного склонение в заданной точке
        /// </summary>
        /// <param name="p1"></param>
        /// <returns></returns>
        public double CalculateMagneticDeclination(TrackPoint p1)
        {
            TrackPoint p2 = new TrackPoint(new Coordinate(37.459201, 55.935163));
            double trA = CalculateTrueAzimuth(p1, p2);
            double mnA = CalculateMagneticAzimuth(p1, p2);
            return mnA - trA;
        }

        /// <summary>
        /// вычисление магнитного азимута в градусах
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public double CalculateMagneticAzimuth(TrackPoint p1, TrackPoint p2)
        {
            if (Vars.Options.Converter.NorthPoleLatitude == 0)
                Vars.Options.Converter.NorthPoleLatitude = 85.90000;
            if (Vars.Options.Converter.NorthPoleLongitude == 0)
                Vars.Options.Converter.NorthPoleLongitude = -147.00000;

            return CalculateAngle(p1.Coordinates, new Coordinate(Vars.Options.Converter.NorthPoleLatitude, Vars.Options.Converter.NorthPoleLongitude), p2.Coordinates, AngleMeasure.Degrees);

        }

        /// <summary>
        /// вычисление истинного азимута от p1 к р2 в градусах
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public double CalculateTrueAzimuth(TrackPoint p1, TrackPoint p2)
        {
            //перевод градусов в радианы
            double rad1deg = Math.PI / 180;
            double lat1 = p1.Coordinates.Latitude.TotalDegrees * rad1deg; //α1

            //sin α1 = 0 - это означает, что начальная точка A находится на полюсе земли. 
            //Если широта начальной точки 90° (северный полюс), 
            //то любое направление из этой точки идет на юг. 
            //Поэтому φ = 180°. Если широта начальной точки -90° (южный полюс), 
            //то любое направление из этой точки идет на север. Поэтому φ = 0°.
            if (Math.Sin(lat1) == 0)
                if (lat1 < 0)
                    return 0;
                else return 180;

            return CalculateAngle(p1.Coordinates, new Coordinate(90, 0), p2.Coordinates, AngleMeasure.Degrees);
        }

        #endregion

        #region вычисления на сфере

        #region расстояния

        /// <summary>
        /// расчет расстояния между двумя точками в метрах по теореме Пифагора.
        /// Подходит для малых расстояний
        /// </summary>
        /// <param name="p1">начальная точка</param>
        /// <param name="p2">конечная точка</param>
        /// <returns>расстояние между точками в метрах</returns>
        private double CalcDistV1(TrackPoint p1, TrackPoint p2)
        {
            double dist = 0;
            double latDiv = Math.Abs(p2.Coordinates.Latitude.TotalDegrees - p1.Coordinates.Latitude.TotalDegrees); //разница по широте  в градусах
            double lonDiv = Math.Abs(p2.Coordinates.Longitude.TotalDegrees - p1.Coordinates.Longitude.TotalDegrees); //разница по долготе в градусах
            double altDiv = (p2.MetrAltitude < 0 || p1.MetrAltitude < 0) ? 0 : Math.Abs(p2.MetrAltitude - p1.MetrAltitude); //разница высот в метрах
            double latinrad = p1.Coordinates.Latitude.TotalDegrees * Math.PI / 180; //широта в радианах
            double In1Lon = this.Equator * Math.Cos(latinrad) / 360; //в одном градусе долготы 
            double latDivM = latDiv * this.In1Lat; //разница в метрах по широте
            double lonDivM = lonDiv * In1Lon; //разница в метрах по долготе
            double altDivM = altDiv; //разница в метрах по высоте
            dist = Math.Sqrt(Math.Pow(latDivM, 2) + Math.Pow(lonDivM, 2) + Math.Pow(altDivM, 2)); // расстояние в метра между точками
            return dist;
        }

        /// <summary>
        /// рассчет расстояния между двумя точками в метрах по сферичиской теореме синусов
        /// Подходит для больших расстояний
        /// </summary>
        /// <param name="p1">начальная точка</param>
        /// <param name="p2">конечная точка</param>
        /// <returns></returns>
        private double CalcDistV2(TrackPoint p1, TrackPoint p2)
        {
            double rad1d = Math.PI / 180;
            double f1 = p1.Coordinates.Latitude.TotalDegrees * rad1d; //широта первой точки в радианах
            double f2 = p2.Coordinates.Latitude.TotalDegrees * rad1d; //широта второй точки в радианах
            double l1 = p1.Coordinates.Longitude.TotalDegrees * rad1d; //долгота первой точки в радианах
            double l2 = p2.Coordinates.Longitude.TotalDegrees * rad1d; //долгота второй точки в радианах
            //угловая длина между заданными точками в радианах
            double td = Math.Sin(f1) * Math.Sin(f2) + Math.Cos(f1) * Math.Cos(f2) * Math.Cos(l2 - l1);
            double d = Math.Acos(td);
            //расстояние по окружности в метрах
            double D = d * this.AverageRadius;
            return D;
        }

        /// <summary>
        /// рассчет расстояния между двумя точками в метрах по теореме гаверсинусов.
        /// Подходит для больших и малых расстояний для точек, не лежащих на концах диаметра
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private double CalcDistV3(TrackPoint p1, TrackPoint p2)
        {
            double rad1d = Math.PI / 180;
            double f1 = p1.Coordinates.Latitude.TotalDegrees * rad1d; //широта первой точки в радианах
            double f2 = p2.Coordinates.Latitude.TotalDegrees * rad1d; //широта второй точки в радианах
            double l1 = p1.Coordinates.Longitude.TotalDegrees * rad1d; //долгота первой точки в радианах
            double l2 = p2.Coordinates.Longitude.TotalDegrees * rad1d; //долгота второй точки в радианах
            double d_1 = Math.Pow(Math.Sin((f2 - f1) / 2), 2) +
                Math.Cos(f1) * Math.Cos(f2) * Math.Pow(Math.Sin((l2 - l1) / 2), 2);
            double d0 = Math.Sqrt(d_1);
            double d = 2 * Math.Asin(d0);
            //расстояние по окружности в метрах
            double D = d * this.AverageRadius;
            return D;
        }

        /// <summary>
        /// рассчет расстояния между двумя точками в метрах по модифицированной теореме гаверсинусов.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private double CalcDistV4(TrackPoint p1, TrackPoint p2)
        {
            double rad1d = Math.PI / 180;
            double f1 = p1.Coordinates.Latitude.TotalDegrees * rad1d; //широта первой точки в радианах
            double f2 = p2.Coordinates.Latitude.TotalDegrees * rad1d; //широта второй точки в радианах
            double l1 = p1.Coordinates.Longitude.TotalDegrees * rad1d; //долгота первой точки в радианах
            double l2 = p2.Coordinates.Longitude.TotalDegrees * rad1d; //долгота второй точки в радианах

            double f_1 = Math.Pow((Math.Cos(f2) * Math.Sin(l2 - l1)), 2) + Math.Pow((Math.Cos(f1) * Math.Sin(f2) - Math.Sin(f1) * Math.Cos(f2) * Math.Cos(l2 - l1)), 2);

            double f_2 = Math.Sin(f1) * Math.Sin(f2) + Math.Cos(f1) * Math.Cos(f2) * Math.Cos(l2 - l1);

            double f_0 = Math.Sqrt(f_1) / f_2;

            double d = Math.Atan(f_0);

            //расстояние по окружности в метрах
            double D = d * this.AverageRadius;
            return D;
        }

        #endregion

        ///// <summary>
        ///// рассчет угла криволинейного треугольника на сфере. 
        ///// Возвращает угол ВАС в градусах.  
        ///// </summary>
        ///// <param name="A">точка А (первая точка)</param>
        ///// <param name="B">точка В (полюс)</param>
        ///// <param name="C">точка С (вторая точка)</param>
        ///// <param name="Measure">единица измерения угла для результата</param>
        ///// <returns></returns>
        private static double CalculateAngle(Coordinate A, Coordinate B, Coordinate C, AngleMeasure Measure)
        {
            //В - полюс 
            //А - первая точка
            //С - вторая точка

            //Теория:
            //http://wiki.gis-lab.info/w/%D0%92%D1%8B%D1%87%D0%B8%D1%81%D0%BB%D0%B5%D0%BD%D0%B8%D0%B5_%D1%83%D0%B3%D0%BB%D0%B0_%D0%BE%D0%B1%D1%80%D0%B0%D0%B7%D0%BE%D0%B2%D0%B0%D0%BD%D0%BD%D0%BE%D0%B3%D0%BE_%D1%82%D1%80%D0%B5%D0%BC%D1%8F_%D1%82%D0%BE%D1%87%D0%BA%D0%B0%D0%BC%D0%B8_%D0%BD%D0%B0_%D1%81%D1%84%D0%B5%D1%80%D0%B5
            //http://www.aleprojects.com/ru/doc/simplegeo

            //перевод координат в радианы
            double rad1deg = Math.PI / 180;
            double lat1 = A.Latitude.TotalDegrees * rad1deg; //α1
            double lat2 = C.Latitude.TotalDegrees * rad1deg; //α2
            double lon1 = A.Longitude.TotalDegrees * rad1deg; //β1
            double lon2 = C.Longitude.TotalDegrees * rad1deg; //β2
            double NPlat = B.Latitude.TotalDegrees * rad1deg; //широта северного полюса
            double NPlon = B.Longitude.TotalDegrees * rad1deg; //долгота северного полюса

            //косинусы центральных углов (дуг) между точками и полюсом
            //cosY = cos α1 ∙ cos α2 ∙ (cos(β1 - β2) - 1) + cos(α1 - α2)
            //дуга p2-NP
            double cosYa = Math.Cos(NPlat) * Math.Cos(lat2) * (Math.Cos(lon2 - NPlon) - 1) + Math.Cos(lat2 - NPlat);
            //дуга p1-p2
            double cosYb = Math.Cos(lat1) * Math.Cos(lat2) * (Math.Cos(lon2 - lon1) - 1) + Math.Cos(lat2 - lat1);
            //дуга p1-NP
            double cosYc = Math.Cos(lat1) * Math.Cos(NPlat) * (Math.Cos(NPlon - lon1) - 1) + Math.Cos(NPlat - lat1);


            //синусы центральных углов через косинусы
            double sinYb = Math.Sqrt(1 - Math.Pow(cosYb, 2));
            double sinYc = Math.Sqrt(1 - Math.Pow(cosYc, 2));

            //если точки совпадают или находятя на краях диаметра, то азимут может быть любой
            if (sinYb == 0)
                return 0;

            //косинус угла  между направленем на магнитный полюс и направлением на p2
            //cos φ = (cos α2 - cos α1 ∙ cos γ) / (sin α1 ∙ sin γ)
            double cosF = (cosYa - cosYb * cosYc) / (sinYb * sinYc);

            //величина угла в радианах
            double F = Math.Acos(cosF);

            //Метод Math.Acos(double) возвращает значение в интервале [0, π]. 
            //Чтобы получить значение в интервале [0, 2π] (или 0-360°), надо сделать следующее:
            if (Math.Abs(lon2 - lon1) > Math.PI)
            {
                if (lon2 > 0) F = 2 * Math.PI - F;
            }
            else
                if (lon2 < lon1) F = 2 * Math.PI - F;

            switch (Measure)
            {
                case AngleMeasure.Degrees: return F / rad1deg;
                case AngleMeasure.Radians: return F;
                default: throw new ArgumentException("Данные единицы измерения углов не поддерживаются: " + Measure);
            }


        }

        #endregion

    }
}
