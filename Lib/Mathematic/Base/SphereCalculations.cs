using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Mathematic.Geodesy;
using TrackConverter.Lib.Mathematic.Geodesy.Models;

namespace TrackConverter.Lib.Mathematic.Base
{
    public class SphereCalculations
    {


        /// <summary>
        /// рассчет угла криволинейного треугольника на сфере. 
        /// Возвращает угол ВАС в градусах.  Угол отсчитывается по часовой стрелке!!
        /// </summary>
        /// <param name="A">точка А (первая точка)</param>
        /// <param name="B">точка В (полюс)</param>
        /// <param name="C">точка С (вторая точка)</param>
        /// <param name="Measure">единица измерения угла для результата</param>
        /// <returns></returns>
        public static double CalculateAngle(Coordinate A, Coordinate B, Coordinate C, AngleMeasure Measure)
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


        /// <summary>
        /// рассчет расстояния между двумя точками в градусах по модифицированной теореме гаверсинусов.
        /// Для получения расстояния надо умножить на радиус сферы
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double CalculateDistance(Coordinate p1, Coordinate p2)
        {
            double rad1d = Math.PI / 180;
            double f1 = p1.Latitude.TotalDegrees * rad1d; //широта первой точки в радианах
            double f2 = p2.Latitude.TotalDegrees * rad1d; //широта второй точки в радианах
            double l1 = p1.Longitude.TotalDegrees * rad1d; //долгота первой точки в радианах
            double l2 = p2.Longitude.TotalDegrees * rad1d; //долгота второй точки в радианах

            double f_1 = Math.Pow((Math.Cos(f2) * Math.Sin(l2 - l1)), 2) + Math.Pow((Math.Cos(f1) * Math.Sin(f2) - Math.Sin(f1) * Math.Cos(f2) * Math.Cos(l2 - l1)), 2);

            double f_2 = Math.Sin(f1) * Math.Sin(f2) + Math.Cos(f1) * Math.Cos(f2) * Math.Cos(l2 - l1);

            double f_0 = Math.Sqrt(f_1) / f_2;

            double d = Math.Atan(f_0);

            //расстояние по окружности в метрах
            return d;
        }

        /// <summary>
        /// прямая геодезическая задача (найти координаты второй точки, зная координаты старта, азимут и расстояние)
        /// Источник: http://wiki.gis-lab.ru/w/%D0%97%D0%B0%D0%B4%D0%B0%D1%87%D0%B8_%D0%BD%D0%B0_%D1%81%D1%84%D0%B5%D1%80%D0%B5:_%D0%BF%D1%80%D1%8F%D0%BC%D0%B0%D1%8F_%D0%B3%D0%B5%D0%BE%D0%B4%D0%B5%D0%B7%D0%B8%D1%87%D0%B5%D1%81%D0%BA%D0%B0%D1%8F_%D0%B7%D0%B0%D0%B4%D0%B0%D1%87%D0%B0
        /// </summary>
        /// <param name="start">координаты точки старта</param>
        /// <param name="azimuth">азимут в градусах</param>
        /// <param name="length">расстояние в метрах</param>
        /// <param name="radius">радиус сферы в метрах</param>
        /// <returns></returns>
        public static Coordinate SphereDirect(Coordinate start, double azimuth, double length, double radius)
        {

            /*
 * Решение прямой геодезической задачи
 *
 * Аргументы исходные:
 *     pt1  - {широта, долгота} точки Q1
 *     azimuth  - азимут начального направления
 *     dist - расстояние (сферическое)
 *
 * Аргументы определяемые:
 *     pt2  - {широта, долгота} точки Q2
 */

            double in1grad = Math.PI / 180; //преобразователь в градусы и обратно
            double[] pt1 = { start.Latitude.TotalDegrees * in1grad, start.Longitude.TotalDegrees * in1grad }; //исходная точка
            double[] pt2 = { -999, -999 }; //результат

            double[] pt = new double[2];
            double[] x = new double[3]; //декартовы координаты
            double dist = length / radius; //длина в радианах

            pt[0] = Math.PI / 2 - dist; //длина дуги расстояния
            pt[1] = Math.PI - azimuth * in1grad; //перевод азимута в радианы
            SpherToCart(pt, x);         // сферические -> декартовы
            Rotate(x, pt1[0] - Math.PI / 2, 1);  // первое вращение
            Rotate(x, -pt1[1], 2);      // второе вращение
            CartToSpher(x, pt2);                // декартовы -> сферические

            return new Coordinate(pt[0] / in1grad, pt[1] / in1grad); //вывод результата с переводом в градусы

        }
        /*
 * Преобразование сферических координат в вектор
 *
 * Аргументы исходные:
 *     y - {широта, долгота}
 *
 * Аргументы определяемые:
 *     x - вектор {x, y, z}
 */
        static void SpherToCart(double[] y, double[] x)
        {
            double p;

            p = Math.Cos(y[0]);
            x[2] = Math.Sin(y[0]);
            x[1] = p * Math.Sin(y[1]);
            x[0] = p * Math.Cos(y[1]);

            return;
        }
        /*
 * Вращение вокруг координатной оси
 *
 * Аргументы:
 *     x - входной/выходной 3-вектор
 *     a - угол вращения
 *     i - номер координатной оси (0..2)
 */
        static void Rotate(double[] x, double a, int i)
        {
            double c, s, xj;
            int j, k;

            j = (i + 1) % 3;
            k = (i - 1) % 3;
            c = Math.Cos(a);
            s = Math.Sin(a);
            xj = x[j] * c + x[k] * s;
            x[k] = -x[j] * s + x[k] * c;
            x[j] = xj;

            return;
        }
        /*
 * Преобразование вектора в сферические координаты
 *
 * Аргументы исходные:
 *     x - {x, y, z}
 *
 * Аргументы определяемые:
 *     y - {широта, долгота}
 *
 * Возвращает:
 *     длину вектора
 */
        static double CartToSpher(double[] x, double[] y)
        {
            double p;

            p = hypot(x[0], x[1]);
            y[1] = Math.Atan2(x[1], x[0]);
            y[0] = Math.Atan2(x[2], p);

            return hypot(p, x[2]);
        }
        /// <summary>
        /// возвращает длину гипотенузы по двум катетам
        /// </summary>
        /// <param name="x">длина катета 1</param>
        /// <param name="y">длина катета 2</param>
        /// <returns></returns>
        static double hypot(double x, double y)
        {
            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
        }

    }
}
