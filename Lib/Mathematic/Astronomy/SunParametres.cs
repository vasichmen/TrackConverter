using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrackConverter.Lib.Classes;

namespace TrackConverter.Lib.Mathematic.Astronomy
{
    /// <summary>
    /// класс вычисления координат солнца и времени.
    /// Источник: http://lavresearch.com
    /// </summary>
     class SunParametres
    {

        const double C_T = 36525.0;

        // Амплитуды для определения коэффициента SK для Солнца
        double[] AskS = new double[53];
        // Амплитуды для определения коэффициента CK для Солнца
        double[] AckS = new double[49];
        // Амплитуды для определения коэффициента SR для Солнца
        double[] AsrS = new double[49];
        // Амплитуды для определения коэффициента CR для Солнца
        double[] AcrS = new double[53];

        // Коэффициенты для определения положения Солнца
        // Учёт собственных возмущений
        double[] sk1 = new double[245] {//4*5  // всего 49*5
					 0, 0, 0, 0, 0,
                     1, 0, 0, 0, 0,
                     2, 0, 0, 0, 0,
                     3, 0, 0, 0, 0,

// Учёт возмущений от Венеры //15*5
					 0, 1, 0, 0, 0,
                     1,-1, 0, 0, 0,
                     1,-2, 0, 0, 0,
                     2,-2, 0, 0, 0,
                     3,-2, 0, 0, 0,
                     3,-3, 0, 0, 0,
                     4,-3, 0, 0, 0,
                     5,-3, 0, 0, 0,
                     4,-4, 0, 0, 0,
                     5,-4, 0, 0, 0,
                     6,-4, 0, 0, 0,
                     5,-5, 0, 0, 0,
                     6,-6, 0, 0, 0,
                     7,-5, 0, 0, 0,
                     8,-5, 0, 0, 0,
// Учёт возмущений от Марса //10*5
					 1, 0,-1, 0, 0,
                     2, 0,-2, 0, 0,
                     1, 0,-2, 0, 0,
                     2, 0,-3, 0, 0,
                     2, 0,-4, 0, 0,
                     3, 0,-4, 0, 0,
                     3, 0,-5, 0, 0,
                     3, 0,-3, 0, 0,
                     4, 0,-3, 0, 0,
                     4, 0,-5, 0, 0,
// Учёт возмущений от Юпитера //12*5
					 0, 0, 0, 1, 0,
                     1, 0, 0,-3, 0,
                     1, 0, 0,-2, 0,
                     1, 0, 0,-1, 0,
                     1, 0, 0, 1, 0,
                     2, 0, 0,-4, 0,
                     2, 0, 0,-3, 0,
                     2, 0, 0,-2, 0,
                     2, 0, 0,-1, 0,
                     3, 0, 0,-4, 0,
                     3, 0, 0,-3, 0,
                     3, 0, 0,-2, 0,
// Учёт возмущений от Сатурна //4*5
					 0, 0, 0, 0, 1,
                     1, 0, 0, 0,-2,
                     1, 0, 0, 0,-1,
                     2, 0, 0, 0,-2,
// Учёт смешанных возмущений //4*5
					 0, 0, 0, 0, 1,
                     1, 0, 0, 0,-2,
                     1, 0, 0, 0,-1,
                     2, 0, 0, 0,-2};


        // Учёт возмущений от Луны
        double[] sk2 = new double[16]{
    //4*4
					 0, 0, 0, 1,
                     1, 0, 0, 1,
                     1, 0, 0,-1,
                     0, 1, 0,-1};


        /// <summary>
        /// Солнце
        ///Получение коэфф. для вычисления координат Солнца
        /// </summary>
        /// <param name="t">Юлианское время от J2000</param>
        void FillAmS(double t)
        {
            // Амплитуды для определения коэффициента SK для Солнца
            //double AskS[49];
            // Амплитуды для определения коэффициента CK для Солнца
            //double AсkS[49];
            // Амплитуды для определения коэффициента SR для Солнца
            //double AsrS[49];
            // Амплитуды для определения коэффициента CR для Солнца
            //double AсrS[49];
            double T = (t + 36525) / C_T;//+ перевод во время от эпохи 1900 г.
            AskS[0] = 0; AckS[0] = 0; AcrS[0] = 30570e-9 - 150e-9 * T; AsrS[0] = 0;
            AskS[1] = 33502e-6 - 83.58e-6 * T - 0.25e-6 * T * T; AckS[1] = 0; AcrS[1] = -7274120e-9 + 18140e-9 * T + 5e-9 * T * T; AsrS[1] = 0;
            AskS[2] = 351e-6 - 1.75e-6 * T; AckS[2] = 0; AcrS[2] = -91380e-9 + 460e-9 * T; AsrS[2] = 0;
            AskS[3] = 5e-6; AckS[3] = 0; AcrS[3] = -1450e-9 + 10e-9 * T; AsrS[3] = 0;

            AskS[4] = 0; AckS[4] = 0; AsrS[4] = 0; AcrS[4] = 85e-9;
            AskS[5] = -20e-6; AckS[5] = 11e-6; AsrS[5] = -1146e-9; AcrS[5] = -2062e-9;
            AskS[6] = 0; AckS[6] = 0; AsrS[6] = 136e-9; AcrS[6] = 84e-9;
            AskS[7] = 14e-6; AckS[7] = -23e-6; AsrS[7] = 5822e-9; AcrS[7] = 3593e-9;
            AskS[8] = -8e-6; AckS[7] = 9e-6; AsrS[8] = -632e-9; AcrS[8] = -596e-9;
            AskS[9] = 0; AckS[8] = -3e-6; AsrS[9] = 1044e-9; AcrS[9] = 0;
            AskS[10] = -2e-6; AckS[10] = 7e-6; AsrS[10] = -1448e-9; AcrS[10] = -381e-9;
            AskS[11] = -3e-6; AckS[11] = 4e-6; AsrS[11] = 148e-9; AcrS[11] = 126e-9;
            AskS[12] = 0; AckS[12] = -1e-6; AsrS[12] = 337e-9; AcrS[12] = -166e-9;
            AskS[13] = 0; AckS[13] = 0; AsrS[13] = 189e-9; AcrS[13] = 0;
            AskS[14] = 0; AckS[14] = 1e-6; AsrS[14] = -91e-9; AcrS[14] = 0;
            AskS[15] = 0; AckS[15] = 0; AsrS[15] = 93e-9; AcrS[15] = -134e-9;
            AskS[16] = 0; AckS[16] = 0; AsrS[16] = 0e-9; AcrS[16] = -80e-9;
            AskS[17] = 0; AckS[17] = 0; AsrS[17] = 136e-9; AcrS[17] = 0;
            AskS[18] = 0; AckS[18] = 1e-6; AsrS[18] = 0; AcrS[18] = 0;

            AskS[19] = 1e-6; AckS[19] = -1e-6; AsrS[19] = -119e-9; AcrS[19] = -92e-9;
            AskS[20] = 3e-6; AckS[20] = 10e-6; AsrS[20] = 1976e-9; AcrS[20] = -573e-9;
            AskS[21] = 3e-6; AckS[21] = -8e-6; AsrS[21] = 137e-9; AcrS[21] = 0;
            AskS[22] = 1e-6; AckS[22] = 2e-6; AsrS[22] = 201e-9; AcrS[22] = -77e-9;
            AskS[23] = 1e-6; AckS[23] = 3e-6; AsrS[23] = -96e-9; AcrS[23] = 0;
            AskS[24] = -2e-6; AckS[24] = 0; AsrS[24] = -125e-9; AcrS[24] = 461e-9;
            AskS[25] = -1e-6; AckS[25] = 0; AsrS[25] = 0; AcrS[25] = 87e-9;
            AskS[26] = 0; AckS[26] = 0; AsrS[26] = 0; AcrS[26] = -154e-9;
            AskS[27] = 0; AckS[27] = 0; AsrS[27] = -94e-9; AcrS[27] = -102e-9;
            AskS[28] = 0; AckS[28] = 0; AsrS[28] = 0; AcrS[28] = 87e-9;

            AskS[29] = -13e-6; AckS[29] = -1e-6; AsrS[29] = -89e-9; AcrS[29] = 227e-9;
            AskS[30] = -1e-6; AckS[30] = 0; AsrS[30] = 0; AcrS[30] = 172e-9;
            AskS[31] = -7e-6; AckS[31] = -3e-6; AsrS[31] = -486e-9; AcrS[31] = 1376e-9;
            AskS[32] = 0; AckS[32] = -35e-6; AsrS[32] = -7067e-9; AcrS[32] = 0;
            AskS[33] = 0; AckS[33] = 0; AsrS[33] = 0; AcrS[33] = 79e-9;
            AskS[34] = 0; AckS[34] = 0; AsrS[34] = 0; AcrS[34] = 110e-9;
            AskS[35] = -3e-6; AckS[35] = 0; AsrS[35] = 104e-9; AcrS[35] = 796e-9;
            AskS[36] = -13e-6; AckS[36] = 0; AsrS[36] = 203e-9; AcrS[36] = 4021e-9;
            AskS[37] = 0; AckS[37] = -1e-6; AsrS[37] = -193e-9; AcrS[37] = -78e-9;
            AskS[38] = 0; AckS[38] = 0; AsrS[38] = -73e-9; AcrS[38] = 0;
            AskS[39] = 0; AckS[39] = -1e-6; AsrS[39] = -278e-9; AcrS[39] = 0;
            AskS[40] = 0; AckS[40] = 0; AsrS[40] = 0; AcrS[40] = 102e-9;

            AskS[41] = -2e-6; AckS[41] = 0; AsrS[41] = 0; AcrS[41] = 0;
            AskS[42] = 0; AckS[42] = 0; AsrS[42] = 0; AcrS[42] = -103e-9;
            AskS[43] = -2e-6; AckS[43] = 0; AsrS[43] = -79e-9; AcrS[43] = 422e-9;
            AskS[44] = 0; AckS[44] = 0; AsrS[44] = 0; AcrS[44] = -152e-9;

            AskS[45] = 0; AckS[45] = 0; AsrS[45] = 0; AcrS[45] = 91e-9;
            AskS[46] = 25e-6; AckS[46] = 18e-6; AsrS[46] = 0; AcrS[46] = 0;
            AskS[47] = 0; AckS[47] = 0; AsrS[47] = 0; AcrS[47] = -91e-9;
            AskS[48] = 0; AckS[48] = -1e-6; AsrS[48] = 0; AcrS[48] = 0;
            // возмущения от луны
            AskS[49] = 31e-6; AcrS[49] = 13360e-9;
            AskS[50] = 1e-6; AcrS[50] = 0;
            AskS[51] = 2e-6; AcrS[51] = -1330e-9;
            AskS[52] = -1e-6; AcrS[52] = 0;

        }

        /// <summary>
        /// Перевод эфемеридной даты в Юлианское время
        /// от 2000 года 1го числа 12 часов
        /// Возврашает число юлианских дней (от начала 2000г.)
        /// </summary>
        /// <param name="newtime"></param>
        /// <returns></returns>
        private double getJG(TimeStruct newtime)
        {
            double km = 12 * (newtime.Year + 1900 + 4800) + newtime.Mon - 2;// в годах
            double vj = (2 * (km - 12 * Math.Floor(km / 12)) + 7 + 365 * km) / 12;
            vj = Math.Floor(vj) + newtime.Day + Math.Floor(km / 48) - 32083;
            if (vj > 2299171)
                vj += Math.Floor(km / 4800) - Math.Floor(km / 1200) + 38;
            vj += -2451545 + (newtime.Hour / 24.0) + (newtime.Min / (24.0 * 60)) + (newtime.Sec / (24 * 3600.0)) - 0.5;
            return vj;
        }


        /// <summary>
        /// Истинный наклон эклиптики к экватору земли без учета нутации         в градусах 
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        double getE(double T)
        {
            double ret;
            double T1 = T / C_T;
            ret = (84381.448 - 46.8150 * T1 - 0.00059 * T1 * T1 + 0.008813 * T1 * T1 * T1) / 3600.0;
            ret = ret % 360.0;
            if (ret < 0) ret += 360.0;
            return ret;
        }

        /// <summary>
        /// Средняя аномалия Луны в градусах
        /// </summary>
        /// <param name="T">эпоха J2000</param>
        /// <returns></returns>
        double retlJ(double T)
        {
            double ret;
            double T1 = T / C_T;
            ret = 485866.733 + (1325 * 1296000 + 715922.633) * T1 + 31.310 * T1 * T1 + 0.064 * T1 * T1 * T1;
            ret /= 3600;//перевод в градусы
            ret = ret % 360.0;
            if (ret < 0) ret += 360.0;
            return ret;
        }

        /// <summary>
        /// Средняя аномалия Cолнца в градусах 
        /// </summary>
        /// <param name="T">эпоха J2000</param>
        /// <returns></returns>
        double retl1J(double T)
        {
            double ret;
            double T1 = T / C_T;
            ret = 1287099.804 + (99 * 1296000 + 1292581.224) * T1 - 0.577 * T1 * T1 - 0.012 * T1 * T1 * T1;
            ret /= 3600;//перевод в градусы
            ret = ret % 360.0;
            if (ret < 0) ret += 360.0;
            return ret;
        }

        /// <summary>
        /// *Венера** - Средняя долгота в градусах 
        /// </summary>
        /// <param name="T">эпоха J2000</param>
        /// <returns></returns>
        double retl_Wn(double T)
        {
            double ret;
            double T1 = T / C_T;
            ret = 181 * 3600 + 58 * 60 + 47.283 + 210669166.909 * T1 + 1.1182 * T1 * T1 + 0.0001 * T1 * T1 * T1;
            ret /= 3600;// перевод в градусы из секунд
            ret = ret % 360.0;
            if (ret < 0) ret += 360.0;
            return ret;
        }

        /// <summary>
        /// *Венера** - Средняя долгота перигея в градусах 
        /// </summary>
        /// <param name="T">эпоха J2000</param>
        /// <returns></returns>
        double retp_Wn(double T)
        {
            double ret;
            double T1 = T / C_T;
            ret = 131 * 3600 + 33 * 60 + 49.346 + 5047.994 * T1 - 3.8618 * T1 * T1 - 0.0189 * T1 * T1 * T1;
            ret /= 3600;// перевод в градусы из секунд
            ret = ret % 360.0;
            if (ret < 0) ret += 360.0;
            return ret;
        }

        /// <summary>
        /// *Марс** - Средняя долгота в градусах
        /// </summary>
        /// <param name="T">эпоха J2000</param>
        /// <returns></returns>
        double retl_Mar(double T)
        {
            double ret;
            double T1 = T / C_T;
            ret = 355 * 3600 + 25 * 60 + 59.789 + 68910107.309 * T1 + 1.1195 * T1 * T1 + 0.0001 * T1 * T1 * T1;
            ret /= 3600;// перевод в градусы из секунд
            ret = ret % 360.0;
            if (ret < 0) ret += 360.0;
            return ret;
        }

        /// <summary>
        /// *Марс** - Средняя долгота перигея в градусах
        /// </summary>
        /// <param name="T">эпоха J2000</param>
        /// <returns></returns>
        double retp_Mar(double T)
        {
            double ret;
            double T1 = T / C_T;
            ret = 336 * 3600 + 3 * 60 + 36.842 + 6627.759 * T1 + 0.4864 * T1 * T1 + 0.0010 * T1 * T1 * T1;
            ret /= 3600;// перевод в градусы из секунд
            ret = ret % 360.0;
            if (ret < 0) ret += 360.0;
            return ret;
        }

        /// <summary>
        /// *Юпитер** - Средняя долгота в градусах
        /// </summary>
        /// <param name="T">эпоха J2000</param>
        /// <returns></returns>
        double retl_Jup(double T)
        {
            double ret;
            double T1 = T / C_T;
            ret = 34 * 3600 + 21 * 60 + 5.342 + 10930690.040 * T1 + 0.8055 * T1 * T1 + 0.0001 * T1 * T1 * T1;
            ret /= 3600;// перевод в градусы из секунд
            ret = ret % 360.0;
            if (ret < 0) ret += 360.0;
            return ret;
        }

        /// <summary>
        /// *Юпитер** - Средняя долгота перигея в градусах
        /// </summary>
        /// <param name="T">эпоха J2000</param>
        /// <returns></returns>
        double retp_Jup(double T)
        {
            double ret;
            double T1 = T / C_T;
            ret = 14 * 3600 + 19 * 60 + 52.713 + 5805.497 * T1 + 3.7132 * T1 * T1 - 0.0159 * T1 * T1 * T1;
            ret /= 3600;// перевод в градусы из секунд
            ret = ret % 360.0;
            if (ret < 0) ret += 360.0;
            return ret;
        }

        /// <summary>
        /// *Сатурн** - Средняя долгота в градусах
        /// </summary>
        /// <param name="T">эпоха J2000</param>
        /// <returns></returns>
        double retl_Sat(double T)
        {
            double ret;
            double T1 = T / C_T;
            ret = 50 * 3600 + 4 * 60 + 38.897 + 4404639.651 * T1 + 1.8703 * T1 * T1;
            ret /= 3600;// перевод в градусы из секунд
            ret = ret % 360.0;
            if (ret < 0) ret += 360.0;
            return ret;
        }

        /// <summary>
        /// *Сатурн** - Средняя долгота перигея в градусах
        /// </summary>
        /// <param name="T">эпоха J2000</param>
        /// <returns></returns>
        double retp_Sat(double T)
        {
            double ret;
            double T1 = T / C_T;
            ret = 93 * 3600 + 3 * 60 + 24.434 + 7069.538 * T1 + 3.0150 * T1 * T1 + 0.0181 * T1 * T1 * T1;
            ret /= 3600;// перевод в градусы из секунд
            ret = ret % 360.0;
            if (ret < 0) ret += 360.0;
            return ret;
        }

        /// <summary>
        /// Средний аргумент широты Луны, в градусах
        /// </summary>
        /// <param name="T">эпоха J2000</param>
        /// <returns></returns>
        double retFJ(double T)
        {
            double ret;
            double T1 = T / C_T;
            ret = 335778.877 + (1342 * 1296000 + 295263.137) * T1 - 13.257 * T1 * T1 + 0.011 * T1 * T1 * T1;
            ret /= 3600;//перевод в градусы
            ret = ret % 360.0;
            if (ret < 0) ret += 360.0;
            return ret;
        }

        /// <summary>
        /// Разность средних долгот Луны и Солнца (Элонгация)в градусах
        /// </summary>
        /// <param name="T">эпоха J2000</param>
        /// <returns></returns>
        double retDJ(double T)
        {
            double ret;
            double T1 = T / C_T;
            ret = 1072261.307 + (1236 * 1296000 + 1105601.328) * T1 - 6.891 * T1 * T1 + 0.019 * T1 * T1 * T1;
            ret /= 3600;//перевод в градусы
            ret = ret % 360.0;
            if (ret < 0) ret += 360.0;
            return ret;
        }

        /// <summary>
        /// Средняя долгота солнца - в градусах
        /// </summary>
        /// <param name="T">эпоха J2000</param>
        /// <returns></returns>
        double retl_Sl(double T)
        {
            double ret;
            double T1 = T / C_T;
            ret = 1009667.850 + (129600000 + 2771.270) * T1 + 1.089 * T1 * T1;
            ret /= 3600;// перевод в градусы из секунд
            ret = ret % 360.0;
            if (ret < 0) ret += 360.0;
            return ret;
        }

        /// <summary>
        /// Получение долготы Солнца  (в градусах) (ошибка до +(2-8) секунд) 
        /// </summary>
        /// <param name="t">Юлианское время от J2000</param>
        /// <returns></returns>
        double get_lon(double t)
        {
            double T = t / C_T;
            double ret = 0,
                    vl = 0;
            double l, l1, D, F,
                    g1, g3, g4, g5;
            // Средняя аномалия Луны
            l = retlJ(t);
            // Средняя аномалия Cолнца
            l1 = retl1J(t);
            // Средняя аномалия Венеры
            g1 = retl_Wn(t) - retp_Wn(t);
            // Средняя аномалия Марса
            g3 = retl_Mar(t) - retp_Mar(t);
            // Средняя аномалия Юпитера
            g4 = retl_Jup(t) - retp_Jup(t);
            // Средняя аномалия Сатурна
            g5 = retl_Sat(t) - retp_Sat(t);
            l *= Math.PI / 180.0;
            l1 *= Math.PI / 180.0;
            g1 *= Math.PI / 180.0;
            g3 *= Math.PI / 180.0;
            g4 *= Math.PI / 180.0;
            g5 *= Math.PI / 180.0;
            FillAmS(t);
            for (int i = 0; i < 49; i++)
            {
                vl += AskS[i] * Math.Sin(sk1[i * 5] * l1 +
                                    (sk1[i * 5 + 1]) * g1 +
                                    (sk1[i * 5 + 2]) * g3 +
                                    (sk1[i * 5 + 3]) * g4 +
                                    (sk1[i * 5 + 4]) * g5);
                vl += AskS[i] * Math.Cos((sk1[i * 5 + 0]) * l1 +
                                    (sk1[i * 5 + 1]) * g1 +
                                    (sk1[i * 5 + 2]) * g3 +
                                    (sk1[i * 5 + 3]) * g4 +
                                    (sk1[i * 5 + 4]) * g5);
            }
            double a = 13 * l1 - 8 * g1 + 3.8990655 + 0.0785398 * (t + 36525) / C_T;
            vl += 6e-6 * Math.Cos(a) + 7e-6 * Math.Sin(a);
            // Средний аргумент широты Луны
            F = retFJ(t);
            // Разность средних долгот Луны и Солнца
            D = retDJ(t);
            F *= Math.PI / 180.0;
            D *= Math.PI / 180.0;
            for (int i = 0; i < 4; i++)
            {
                vl += AskS[i + 49] * Math.Sin((sk2[i * 4 + 0]) * l +
                                        (sk2[i * 4 + 1]) * l1 +
                                        (sk2[i * 4 + 2]) * F +
                                        (sk2[i * 4 + 3]) * D);
            }
            ret = retl_Sl(t);// средняя долгота солнца
            vl *= 180 / Math.PI;
            ret += vl;
            ret = ret % 360;
            if (ret < 0) ret += 360;
            return ret;

        }

        /// <summary>
        /// Получение Радиус-вектора солнца в средних расстояниях от земли до солнца (до 5 знака после запятой) (в астрономических еденицах)
        /// </summary>
        /// <param name="t">Юлианское время от J2000</param>
        /// <returns></returns>
        double get_ri(double t)
        {
            double T = t / C_T;
            double ret = 0,
                    vl = 0;
            double l, l1, D, F,
                    g1, g3, g4, g5;
            // Средняя аномалия Луны
            l = retlJ(t);
            // Средняя аномалия Cолнца
            l1 = retl1J(t);
            // Средняя аномалия Венеры
            g1 = retl_Wn(t) - retp_Wn(t);
            // Средняя аномалия Марса
            g3 = retl_Mar(t) - retp_Mar(t);
            // Средняя аномалия Юпитера
            g4 = retl_Jup(t) - retp_Jup(t);
            // Средняя аномалия Сатурна
            g5 = retl_Sat(t) - retp_Sat(t);
            l *= Math.PI / 180.0;
            l1 *= Math.PI / 180.0;
            g1 *= Math.PI / 180.0;
            g3 *= Math.PI / 180.0;
            g4 *= Math.PI / 180.0;
            g5 *= Math.PI / 180.0;
            FillAmS(t);
            for (int i = 0; i < 49; i++)
            {
                vl += AskS[i] * Math.Sin(sk1[i * 5] * l1 +
                                   (sk1[i * 5 + 1]) * g1 +
                                   (sk1[i * 5 + 2]) * g3 +
                                   (sk1[i * 5 + 3]) * g4 +
                                   (sk1[i * 5 + 4]) * g5);
                vl += AskS[i] * Math.Cos((sk1[i * 5 + 0]) * l1 +
                                    (sk1[i * 5 + 1]) * g1 +
                                    (sk1[i * 5 + 2]) * g3 +
                                    (sk1[i * 5 + 3]) * g4 +
                                    (sk1[i * 5 + 4]) * g5);

            }
            // Средний аргумент широты Луны
            F = retFJ(t);
            // Разность средних долгот Луны и Солнца
            D = retDJ(t);
            F *= Math.PI / 180.0;
            D *= Math.PI / 180.0;
            for (int i = 0; i < 4; i++)
            {
                vl += AcrS[+i + 49] * Math.Cos((sk2[i * 4 + 0]) * l +
                                        (sk2[i * 4 + 1]) * l1 +
                                        (sk2[i * 4 + 2]) * F +
                                        (sk2[i * 4 + 3]) * D);
            }
            //vl /= (3600);
            ret = Math.Pow(10.0, vl);
            ret = Math.Pow(10.0, vl);
            //if (ret<0)	ret+=360;
            return ret;
        }

        /// <summary>
        ///  Гривническое звездное время на меридиане гринвича (звездное время в часах)
        /// </summary>
        /// <param name="t">Число Юлианских дней от J2000</param>
        /// <returns></returns>
        double star_time(double t)
        {
            double t0, // Число Юлианских дней до гривнической полуночи
                    so, // Гринвическое время от полуночи в часах
                    tmp,
                    M, M1; // Гринвическое время от полуночи в часах

            // Время от начала суток
            M1 = t - (int)t;
            if (M1 >= 0.5)
            {
                M = M1 - 0.5;
                t0 = (int)(t) + 0.5;// Число Юлианских дней до гривнической полуночи
            }
            else
            {
                M = M1 + 0.5;
                t0 = (int)(t) - 0.5;// Число Юлианских дней до гривнической полуночи
            }

            M *= 24;// перевод в часы;
                    // Звездное время в гривническую полночь текущего дня
            tmp = t0 / 36525.0;//tmp = (t0-2415020.0)/36525.0;
            so = (21600 + 2460 + 50.54841 + 8640184.812866 * tmp + 0.093104 * tmp * tmp - 6.2 * tmp * tmp * tmp);// в секундах
                                                                                                                 // снова считаем число Юл. дней, но уже не от полуночи, а на тек. время
            tmp = t / 36525.0;
            // добавим нутацию
            //double na1 = (cos(rete(tmp*36525.0)*PI/180.0)/15.0)*retDfJ(tmp*36525.0);
            //double na2 = (cos(rete(tmp*36525.0)*PI/180.0)/15.0)*retdfJ(tmp*36525.0);
            //so += (cos(rete(tmp*36525.0)*PI/180.0)/15.0)*retDfJ(tmp*36525.0);
            //so += (cos(rete(tmp*36525.0)*PI/180.0)/15.0)*retdfJ(tmp*36525.0);
            so /= 3600.0; // в часах
            so += M * 1.0027379093;// плюс текущее время от гривнической полуночи
            so = so % 24.0;
            if (so < 0) so += 24;
            return so;
        }

        /// <summary>
        /// Получение видимого прямого восхождения и склонения
        /// </summary>
        /// <param name="La">широта - станет прямым восхождением - в градусах</param>
        /// <param name="Lo">долгота - станет склонением  - в градусах</param>
        /// <param name="e">угол поворота( наклон эклиптики) - в градусах</param>
        void get_LaLo(ref double La, ref double Lo, double e)
        {
            double z;
            double f,// широта
                    t,// часовой угол
                    l,// склонение
                    A;// 
                      //a;// прям. восхождение

            f = 90 - e;
            t = 90 - Lo;
            l = La;

            f *= (Math.PI / 180.0);
            t *= (Math.PI / 180.0);
            l *= (Math.PI / 180.0);

            z = Math.Sin(f) * Math.Sin(l) + Math.Cos(f) * Math.Cos(l) * Math.Cos(t);
            //ASSERT(z>=-1 && z<=1);
            double z1 = Math.Acos(z);
            z1 = Math.Acos(z);// повтор, иначе не работает!!!
            z1 = z1 * (180.0 / Math.PI);
            Lo = 90 - z1;// склонение
            Lo = Lo % 360;
            if (Lo < 0) Lo += 360;

            A = Math.Atan2((Math.Cos(l) * Math.Sin(t)), (-Math.Sin(l) * Math.Cos(f) + Math.Cos(l) * Math.Sin(f) * Math.Cos(t)));
            A *= (180.0 / Math.PI);
            La = 90 - A;
            La = La % 360;
            if (La < 0) La += 360;
        }

        /// <summary>
        /// Вычисление восхода и захода солнца (везде время местное).
        /// 1 : все ОК 
        /// или -1 : сегодня не всходит (весь день темно или полярная ночь) 
        /// или -2 : сегодня не заходит (весь день светло или полярный день)
        /// </summary>
        /// <param name="loc_time">локальное время (любое время текущего дня)</param>
        /// <param name="time_zone"> часовой сдвиг от гринвича (например для Москвы зимой=4, летом=5)</param>
        /// <param name="latA"> широта местоположения  (в градусах)</param>
        /// <param name="lonA">долгота местоположения  (в градусах)</param>
        /// <param name="t_rise">указатель на число, которое будет содержать время восхода солнца (в часах)</param>
        /// <param name="t_set">указатель на число, которое будет содержать время захода солнца (в часах)</param>
        /// <returns></returns>
        public int GetSunSet(TimeStruct loc_time, double time_zone, double latA, double lonA, ref double t_rise, ref double t_set)
        {
            double t0;// Юлианское время в гринвичскую полночь
            double t;// текущее Юлианское время
            double lat, lon;// широта, долгота солнца
            double e;// наклон эклиптики к экватору
            double r;// нормированный радиус-вектор ~1
            double s1, s2;// звездное время восхода и захода
            double s0;//звездное время
            double z;//зенитное расстояние горизонта
            double dz;//добавка к зенитному расстоянию горизонта
            double cost;// значение косинуса (м.б. > 1!)
            double v = 1 - 0.0027304336;// для перевода из звездного в обычное время

            // переводим во время на середину дня
            loc_time.Hour = 12 - (int)time_zone;
            loc_time.Min = 0;
            loc_time.Sec = 0;
            // определяем Юлианское время (на середину текущего дня)
            t = getJG(loc_time);

            // переводим во время на гринвческую полночь
            loc_time.Hour = 0;
            // определяем Юлианское время (на гринвческую полночь)
            t0 = getJG(loc_time);

            // Определяем положение солнца на эклиптике
            e = getE(t);// наклон эклиптики к экватору
            lon = get_lon(t);// долгота солнца - longitude
            lat = 0;// широта солнца - latitude
            r = get_ri(t);// нормированный радиус-вектор ~1

            //вычисляем добавку к зенитному расстоянию горизонта
            dz = 0;
            dz += 34.5 / 60.0;// рефракция=35'
                              //dz+= d;// высота места
            dz += (961.18 / r) / 3600.0;// угловой радиус солнца ~ 16'
            dz -= (8.794 / r) / 3600.0;// параллакс ~ 9"
            z = 90 + dz;

            // поиск видимого прямого восхождения и склонения
            // lat: широта  -> прямое восхождение (в градусах)
            // lon: долгота -> склонение (в градусах)
            get_LaLo(ref lat, ref lon, e);
            lat /= 15;// переводим прямое восхождение из градусов в часы


            //вычисляем местное звездное время
            s0 = star_time(t0) * 15;
            s0 += lonA;//местное звездное время
            if (s0 < 0) s0 += 360;
            s0 = s0 % 360 / 15.0;// перевод в часы

            // перевод в радианы
            lon *= (Math.PI / 180.0);
            z *= (Math.PI / 180.0);
            latA *= (Math.PI / 180.0);
            cost = (Math.Cos(z) - Math.Sin(latA) * Math.Sin(lon)) / (Math.Cos(latA) * Math.Cos(lon));
            if (cost > 1)// солнце не всходит
                return -1;
            if (cost < -1)// солнце не заходит
                return -2;
            // смещения времени от истинного полудня
            double dt = Math.Acos(cost);
            dt *= (12.0 / Math.PI);// переводим в часы из радиан
            if (dt < 0) dt += 24;
            if (dt <= 12)
            {
                s1 = lat - dt;
                s2 = lat + dt;
            }
            else
            {
                s1 = lat + dt;
                s2 = lat - dt;
            }

            t_rise = (s1 - s0) * v + time_zone;

            t_set = (s2 - s0) * v + time_zone;
            if ((t_rise) < 0) (t_rise) += 24;
            if ((t_set) < 0) (t_set) += 24;

            return 1;
        }

    }

}