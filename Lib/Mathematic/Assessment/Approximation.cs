using System;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.Lib.Mathematic.Assessment
{
    /// <summary>
    /// Класс для усреднения высот в маршрутах. Используется для уменьшения количества экстремумов
    /// Источник:
    /// http://www.alexeypetrov.narod.ru/C/sqr_less_about.html
    /// Перевoд на C#:
    /// http://www.cyberforum.ru/windows-forms/thread745949.html
    /// 
    /// </summary>
    internal static class Approximator
    {
        private class Polynomial
        {
            /// <summary>
            /// Сетка по X
            /// </summary>
            private double[] _x;

            /// <summary>
            /// Значения функции в точках _x
            /// </summary>
            private readonly double[] _y;

            /// <summary>
            /// Значения коэффициентов
            /// </summary>
            private readonly double[] _coefficients;

            public double[] Coefficients
            {
                get { return _coefficients; }
            }

            /// <summary>
            /// Степень полинома
            /// </summary>
            private readonly int _amount;

            public int Amount
            {
                get { return _amount; }
            }

            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="xval">Сетка по X</param>
            /// <param name="yval">Значения функции в точках X</param>
            /// <param name="amount"></param>
            /// <remarks>Размер массива X должен равняться размеру массива Y. В масивах должно быть хотя бы по три элемента. Массив X должен быть упорядочен по возрастанию.</remarks>
            public Polynomial(double[] xval, double[] yval, int amount)
            {
                if (xval.Length != yval.Length)
                {
                    throw new ArgumentException();
                }

                if (xval.Length < 3)
                {
                    throw new ArgumentException();
                }

                // 1, 2
                _amount = amount;
                _x = xval;
                _y = yval;

                int n = _amount + 1;
                int count = _x.Length;

                double[,] a = new double[n, n];
                double[] b = new double[n];
                double[] c = new double[2 * n];
                _coefficients = new double[n];

                // 3
                for (int i = 0; i < count; i++)
                {
                    double x = _x[i];
                    double y = _y[i];

                    double f = 1;

                    for (int j = 0; j < 2 * n - 1; j++)
                    {
                        if (j < n)
                        {
                            b[j] += y;
                            y *= x;
                        }

                        c[j] += f;
                        f *= x;
                    }
                }

                // 4
                for (int i = 0; i < n; i++)
                {
                    int k = i;

                    for (int j = 0; j < n; j++)
                    {
                        a[i, j] = c[k];
                        k++;
                    }
                }

                //5
                for (int i = 0; i < n - 1; i++)
                {
                    for (int j = i + 1; j < n; j++)
                    {
                        a[j, i] /= -a[i, i];

                        for (int k = i + 1; k < n; k++)
                        {
                            a[j, k] += a[j, i] * a[i, k];
                        }

                        b[j] += a[j, i] * b[i];
                    }
                }

                // 6
                _coefficients[n - 1] = b[n - 1] / a[n - 1, n - 1];

                //7
                for (int i = n - 2; i >= 0; i--)
                {
                    double h = b[i];

                    for (int j = i + 1; j < n; j++)
                    {
                        h -= _coefficients[j] * a[i, j];
                    }

                    _coefficients[i] = h / a[i, i];
                }
            }


            #region IApproximation Members

            public double GetValue(double xpoint)
            {
                double s = 0;
                for (int i = _amount; i >= 1; i--)
                {
                    s = (s + _coefficients[i]) * xpoint;
                }

                return s + _coefficients[0];
            }

            #endregion
        }

        /// <summary>
        /// аппроксимировать высоты
        /// </summary>
        /// <param name="route"></param>
        /// <param name="amount">степень полинома</param>
        /// <returns></returns>
        public static BaseTrack Approximate(BaseTrack route, int amount)
        {
            double[] x = new double[route.Count];
            for (int i = 0; i < route.Count; i++)
                x[i] = route[i].StartDistance;

            double[] y = new double[route.Count];
            y = route.AllAltitudes.ToArray();
            Polynomial pl = new Polynomial(x, y, amount);
            for (int i = 0; i < route.Count; i++)
                route[i].MetrAltitude = pl.GetValue(x[i]);

            return route;
        }

    }
}
