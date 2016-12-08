using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrackConverter.Lib.Classes;

namespace TrackConverter.Lib.Data.Providers.Local.ETOPO2
{
    /// <summary>
    /// База данных в двоичном файле .bin с заголовочным .hdr . 
    /// Поддерживаются бинарные файлы с целыми числами в ячейках с big-endian и little-endian порядком байт. 
    /// </summary>
    class FloatDatabase : BaseGrid
    {
        private float[,] matrix;
        private float noData;
        private float minimum;
        private float maximum;


        /// <summary>
        /// значение в ячейках, для которых нет данных
        /// </summary>
        public float NoData { get { return this.noData; } }

        /// <summary>
        /// Минимальное значение в базе данных
        /// </summary>
        public float Minimum { get { return this.minimum; } }

        /// <summary>
        /// Максимальное значение в базе данных
        /// </summary>
        public float Maximum { get { return this.maximum; } }

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
        public FloatDatabase(string fHeader, string fData)
            : base(fHeader, fData)
        {
            this.Type = ETOPO2DBType.Float;
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
                    float nodata = -1;

                    //координаты нижнего левого угла
                    double xllcorner = double.NaN;
                    double yllcorner = double.NaN;

                    //контрольные значения максимальной и минимальной высоты
                    float min = -1;
                    float max = -1;

                    //размер ячейки в градусах
                    double cellSize = double.NaN;

                    //если истина, то старший байт первый в файле
                    bool? isMostByteFirst = null;

                    #region заголовочный файл

                    do
                    {
                        string line = rhead.ReadLine();
                        if (line.Contains("NCOLS"))
                        {
                            int _ = line.IndexOf(" ");
                            string num = line.Substring(_);
                            columns = int.Parse(num.Trim());
                        }
                        if (line.Contains("NROWS"))
                        {
                            int _ = line.IndexOf(" ");
                            string num = line.Substring(_);
                            rows = int.Parse(num.Trim());
                        } if (line.Contains("XLLCORNER"))
                        {
                            int _ = line.IndexOf(" ");
                            string num = line.Substring(_);
                            xllcorner = double.Parse(num.Trim().Replace('.', Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]));
                        } if (line.Contains("YLLCORNER"))
                        {
                            int _ = line.IndexOf(" ");
                            string num = line.Substring(_);
                            yllcorner = double.Parse(num.Trim().Replace('.', Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]));
                        } if (line.Contains("CELLSIZE"))
                        {
                            int _ = line.IndexOf(" ");
                            string num = line.Substring(_);
                            cellSize = double.Parse(num.Trim().Replace('.', Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]));
                        } if (line.Contains("NODATA_VALUE"))
                        {
                            int _ = line.IndexOf(" ");
                            string num = line.Substring(_);
                            nodata = float.Parse(num.Trim().Replace('.', Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]));
                        } if (line.Contains("MIN_VALUE"))
                        {
                            int _ = line.IndexOf(" ");
                            string num = line.Substring(_);
                            min = float.Parse(num.Trim().Replace('.', Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]));
                        } if (line.Contains("MAX_VALUE"))
                        {
                            int _ = line.IndexOf(" ");
                            string num = line.Substring(_);
                            max = float.Parse(num.Trim().Replace('.', Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator[0]));
                        } if (line.Contains("BYTEORDER"))
                        {
                            int _ = line.IndexOf(" ");
                            string order = line.Substring(_);
                            if (order.Trim() == "MSBFIRST")
                                isMostByteFirst = true;
                            if (order.Trim() == "LSBFIRST")
                                isMostByteFirst = false;
                        }
                    }
                    while (!rhead.EndOfStream);

                    //проверка заголовочных данных
                    if (columns == -1 ||
                        rows == -1 ||
                        nodata == -1 ||
                        double.IsNaN( xllcorner) ||
                        double.IsNaN( yllcorner ) ||
                        min == -1 ||
                        max == -1 ||
                        double.IsNaN( cellSize) ||
                        isMostByteFirst == null)
                        throw new Exception("Ошибка при чтении заголовочного файла. Не все данные прочитаны");

                    #endregion

                    #region основной файл данных


                    float[,] fileArray = new float[rows, columns];

                    //заполнение массива
                    for (int i = 0; i < rows; i++)
                        for (int j = 0; j < columns; j++)
                        {
                            //чтение двух байт из файла
                            byte b1 = (byte)rbin.ReadByte();
                            byte b2 = (byte)rbin.ReadByte();
                            byte b3 = (byte)rbin.ReadByte();
                            byte b4 = (byte)rbin.ReadByte();

                            //преобразование в Float
                            float val = (bool)isMostByteFirst
                                ? BitConverter.ToSingle(new byte[] { b4, b3, b2, b1 }, 0)
                                : BitConverter.ToSingle(new byte[] { b1, b2, b3, b4 }, 0);

                            fileArray[i, j] = val;
                        }

                    //проверка, что дошли до конца файла
                    bool end = rbin.Length == rbin.Position;
                    if (!end)
                        throw new Exception("Ошибка при чтении файла данных. Просмотр файла осуществлен не до конца");

                    //проверка максимумов и минимумов
                    float nmin = float.MaxValue;
                    float nmax = float.MinValue;
                    for (int i = 0; i < rows; i++)
                        for (int j = 0; j < columns; j++)
                        {
                            if (fileArray[i, j] != nodata)
                            {
                                if (fileArray[i, j] > nmax)
                                    nmax = fileArray[i, j];
                                if (fileArray[i, j] < nmin)
                                    nmin = fileArray[i, j];
                            }
                        }

                    if (nmax != max || nmin != min)
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
                    this.LLCorner = new Coordinate( yllcorner,xllcorner);

                    //rhead.Close();
                }

                //rbin.Close();
            }
        }
    }
}
