using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrackConverter.Lib.Classes;

namespace TrackConverter.Lib.Data.Providers.Local.ETOPO
{
    /// <summary>
    /// База данных в двоичном файле .bin с заголовочным .hdr . 
    /// Поддерживаются бинарные файлы с целыми числами в ячейках с big-endian и little-endian порядком байт. 
    /// </summary>
    class Int16Database : BaseGrid
    {
        private Int16[,] matrix;
        private Int16 noData;
        private Int16 minimum;
        private Int16 maximum;


        /// <summary>
        /// значение в ячейках, для которых нет данных
        /// </summary>
        public Int16 NoData { get { return this.noData; } }

        /// <summary>
        /// Минимальное значение в базе данных
        /// </summary>
        public Int16 Minimum { get { return this.minimum; } }

        /// <summary>
        /// Максимальное значение в базе данных
        /// </summary>
        public Int16 Maximum { get { return this.maximum; } }

        /// <summary>
        /// Возвращает элемент по заданным столбцу и строке
        /// </summary>
        /// <param name="i">номер строки</param>
        /// <param name="j">номер столбца</param>
        /// <returns></returns>
        public override double this[int i, int j] { get { return matrix[i, j]; } }




        /// <summary>
        /// Загружает базу данных из заданных файлов
        /// </summary>
        /// <param name="fHeader">Путь к заголовочному файлу</param>
        /// <param name="fData">Путь к файлу данных</param>
        public Int16Database(string fHeader, string fData)
            : base(fHeader, fData)
        {
            this.Type = ETOPODBType.Int16;
            this.LoadDatabase();
        }

        /// <summary>
        /// Загружает базу данных. Перед вызовом этого метода в полях headerFile и dataFile должны быть значения.
        /// </summary>
        protected override void LoadDatabase()
        {
            if (headerFile == null || headerFile == "")
                throw new Exception("Не задано имя заголовочного файла");
            if (!File.Exists(headerFile))
                throw new FileNotFoundException("Заголовочный файл не найден. " + headerFile);
            if (dataFile == null || dataFile == "")
                throw new Exception("Не задано имя файла данных");
            if (!File.Exists(dataFile))
                throw new FileNotFoundException("Файл данных не найден. " + dataFile);

            using (FileStream rbin = new FileStream(this.dataFile, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader rhead = new StreamReader(new FileStream(this.headerFile, FileMode.Open, FileAccess.Read)))
                {
                    //количество строк и столбцов
                    int columns = -1;
                    int rows = -1;

                    //значение для обозначения неизвестных участков
                    short nodata = -1;

                    //координаты нижнего левого угла
                    double xllcorner = double.NaN;
                    double yllcorner = double.NaN;

                    //контрольные значения максимальной и минимальной высоты
                    short min = -1;
                    short max = -1;

                    //размер ячейки в градусах
                    double cellSize = double.NaN;

                    //если истина, то старший байт первый в файле
                    bool? isMostByteFirst = null;

                    #region заголовочный файл

                    do
                    {
                        string line = rhead.ReadLine();
                        if (line.ToLower().Contains("ncols"))
                        {
                            int _ = line.IndexOf(" ");
                            string num = line.Substring(_);
                            columns = int.Parse(num.Trim());
                        }
                        if (line.ToLower().Contains("nrows"))
                        {
                            int _ = line.IndexOf(" ");
                            string num = line.Substring(_);
                            rows = int.Parse(num.Trim());
                        }
                        if (line.ToLower().Contains("xllcorner"))
                        {
                            int _ = line.IndexOf(" ");
                            string num = line.Substring(_);
                            xllcorner = double.Parse(num.Trim().Replace('.', Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]));
                        }
                        if (line.ToLower().Contains("yllcorner"))
                        {
                            int _ = line.IndexOf(" ");
                            string num = line.Substring(_);
                            yllcorner = double.Parse(num.Trim().Replace('.', Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]));
                        }
                        if (line.ToLower().Contains("yllcenter") || line.ToLower().Contains("xllcenter"))
                        {
                            throw new ApplicationException("Заголовочный файл должен содержать записи xllcorner и yllcorner.\r\nСкорее всего, указана grid-registred БД. Используйте cell-registred БД.\r\nПроблема в файле " + this.headerFile);
                        }
                        if (line.ToLower().Contains("cellsize"))
                        {
                            int _ = line.IndexOf(" ");
                            string num = line.Substring(_);
                            cellSize = double.Parse(num.Trim().Replace('.', Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]));
                        }
                        if (line.ToLower().Contains("nodata_value"))
                        {
                            int _ = line.IndexOf(" ");
                            string num = line.Substring(_);
                            nodata = short.Parse(num.Trim().Replace('.', Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]));
                        }
                        if (line.ToLower().Contains("min_value"))
                        {
                            int _ = line.IndexOf(" ");
                            string num = line.Substring(_);
                            min = short.Parse(num.Trim().Replace('.', Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]));
                        }
                        if (line.ToLower().Contains("max_value"))
                        {
                            int _ = line.IndexOf(" ");
                            string num = line.Substring(_);
                            max = short.Parse(num.Trim().Replace('.', Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]));
                        }
                        if (line.ToLower().Contains("byteorder"))
                        {
                            int _ = line.IndexOf(" ");
                            string order = line.Substring(_);
                            if (order.ToLower().Trim() == "msbfirst")
                                isMostByteFirst = true;
                            if (order.ToLower().Trim() == "lsbfirst")
                                isMostByteFirst = false;
                        }
                    }
                    while (!rhead.EndOfStream);

                    //проверка заголовочных данных
                    if (columns == -1 ||
                        rows == -1 ||
                        nodata == -1 ||
                        double.IsNaN(xllcorner) ||
                        double.IsNaN(yllcorner) ||
                        min == -1 ||
                        max == -1 ||
                        double.IsNaN(cellSize) ||
                        isMostByteFirst == null)
                        throw new Exception("Ошибка при чтении заголовочного файла. Не все данные прочитаны");

                    #endregion

                    #region основной файл данных


                    Int16[,] fileArray = new Int16[rows, columns];

                    //заполнение массива
                    float nmin = float.MaxValue;
                    float nmax = float.MinValue;
                    for (int i = 0; i < rows; i++)
                        for (int j = 0; j < columns; j++)
                        {
                            //чтение двух байт из файла
                            int imost = rbin.ReadByte();
                            int ileast = rbin.ReadByte();
                            byte most = (byte)imost;
                            byte least = (byte)ileast;

                            //преобразование в ShortInt
                            Int16 val = (bool)isMostByteFirst
                                ? BitConverter.ToInt16(new byte[] { least, most }, 0)
                                : BitConverter.ToInt16(new byte[] { most, least }, 0);

                            fileArray[i, j] = val;

                            if (fileArray[i, j] != nodata)
                            {
                                if (fileArray[i, j] > nmax)
                                    nmax = fileArray[i, j];
                                if (fileArray[i, j] < nmin)
                                    nmin = fileArray[i, j];
                            }
                        }

                    //проверка, что дошли до конца файла
                    bool end = rbin.Length == rbin.Position;
                    if (!end)
                        throw new Exception("Ошибка при чтении файла данных. Просмотр файла осуществлен не до конца");



                    if (Math.Abs(nmax - max) > 50 || Math.Abs(nmin - min) > 50)
                        throw new Exception("Ошибка при чтении файла данных. Не совпадают контрольные значения");

                    #endregion

                    //запись результатов
                    this.matrix = fileArray;
                    this.cellSize = cellSize;
                    this.columns = columns;
                    this.noData = nodata;
                    this.rows = rows;
                    this.minimum = min;
                    this.maximum = max;
                    this.LLCorner = new Coordinate(yllcorner, xllcorner);
                }
            }
        }
    }
}
