using System;
using System.Reflection;
using System.Windows.Forms;
using TrackConverter.Lib.Data;
using TrackConverter.Lib.Exceptions;
using Word = Microsoft.Office.Interop.Word;

namespace TrackConverter.Lib.Tracking.Helpers
{
    /// <summary>
    /// методы для работы с форматами MS Office
    /// </summary>
    internal class OfficeHelper
    {
        /// <summary>
        /// записать маршрут в указанный файл .doc
        /// </summary>
        /// <param name="track">маршрут</param>
        /// <param name="fileName">имя файла .doc</param>
        internal static void WriteTrack(BaseTrack track, string fileName, Action<string> callback)
        {
            var coder = new GeoCoder(Vars.Options.DataSources.GeoCoderProvider);
            WordDocument wordDoc;
            wordDoc = new WordDocument(Application.StartupPath + Res.Properties.Resources.word_template);

            wordDoc.InsertTable(track.Count + 1, 4);


            wordDoc.SetColumnWidth(1, 25);
            wordDoc.SetColumnWidth(2, 45);
            wordDoc.SetColumnWidth(3, 100);
            wordDoc.SetColumnWidth(4, 400);

            wordDoc.SetSelectionToCell(1, 1);
            wordDoc.Selection.Text = "№";
            wordDoc.Selection.FontSize = 10;
            wordDoc.Selection.Bold = true;

            wordDoc.SetSelectionToCell(1, 3);
            wordDoc.Selection.Text = "Название, адрес";
            wordDoc.Selection.FontSize = 10;
            wordDoc.Selection.Bold = true;

            wordDoc.SetSelectionToCell(1, 2);
            wordDoc.Selection.Text = "Ш./Д." + WordDocument.NewLineChar + "От старта";
            wordDoc.Selection.FontSize = 10;
            wordDoc.Selection.Bold = true;

            wordDoc.SetSelectionToCell(1, 4);
            wordDoc.Selection.Text = "Описание";
            wordDoc.Selection.FontSize = 10;
            wordDoc.Selection.Bold = true;

            double all = track.Count;
            for (int i = 0; i < track.Count; i++)
            {
                if (callback != null)
                    callback.Invoke("Идет сохранение  файла. Завершено: " + (i / all * 100d).ToString("0.0") + "%");

                string adr = coder.GetAddress(track[i].Coordinates);

                wordDoc.SetSelectionToCell(i + 2, 1);
                wordDoc.Selection.Text = i.ToString();
                wordDoc.Selection.FontSize = 10;

                wordDoc.SetSelectionToCell(i + 2, 3);
                wordDoc.Selection.Text = track[i].Name + "\r\n" + adr;
                wordDoc.Selection.FontSize = 10;

                wordDoc.SetSelectionToCell(i + 2, 2);
                wordDoc.Selection.Text =
                    track[i].Coordinates.Latitude.ToString("00.000") + 'º' + WordDocument.NewLineChar +
                    track[i].Coordinates.Longitude.ToString("00.000") + 'º' + WordDocument.NewLineChar +
                    track[i].StartDistance.ToString("0.0") + " км";
                wordDoc.Selection.FontSize = 10;

                wordDoc.SetSelectionToCell(i + 2, 4);
                wordDoc.Selection.Text = track[i].Description;
                wordDoc.Selection.FontSize = 10;
            }

            wordDoc.Selection.FontSize = 10;

            wordDoc.Save(fileName);

            wordDoc.Visible = true;
        }



        /// Версия 1.8
        // Word запускается ОТДЕЛЬНЫМ приложением, которое должно быть установелено на компьютере, 
        //класс просто управляет им через интерфейс Word Interoperability, 
        //в проекте должна быть ссылка на Microsoft.Office.Interop.Word, соотвествующая библиотека .dll 
        //должна быть в папке с программой, 
        //----- класс позволяет создать новый документ по шаблону, произвести поиск и замену строк (одно вхождение или все),
        //изменить видимость документа, закрыть документ
        private class WordDocument
        {
            // фиксированные параметры для передачи приложению Word
            private object _missingObj = Missing.Value;
            private readonly object _trueObj = true;
            private object _falseObj = false;

            //рабочие параметры если использовать Word.Application и Word.Document получим предупреждение от компиллятора
            private Word._Application _application;
            private Word._Document _document;

            private readonly object _templatePathObj;

            private Word.Range _currentRange = null;

            private Word.Table _table = null;

            // обьект вставленного параграфа, представляет собой параграф с текстом, обертка над Range
            private WordSelection _selection;

            // вставленный параграф доступен только для чтения
            public WordSelection Selection
            {
                get { return _selection; }
                set { throw new Exception("Ошибка! Свойство InsertedParagraph только для чтения"); }
            }

            // СИМВОЛ МЯГКОГО ПЕРЕНОСА СТРОКИ В WORD (в ручную ставится через Shift + Enter)
            public static char NewLineChar { get { return (char)11; } }

            public bool Closed
            {
                get
                {
                    if (_application == null || _document == null)
                    { return true; }
                    else
                    { return false; }
                }
            }

            // видимость на экране приложения Word, по умолчанию false, документ создается невидимым и его надо явно сделать видимым после выполения необходимых операций
            public bool Visible
            {
                get
                {
                    if (Closed)
                    { throw new Exception("Ошибка при попытке изменить видимость Microsoft Word. Программа или документ уже закрыты."); }
                    return _application.Visible;

                }
                set
                {
                    if (Closed)
                    { throw new Exception("Ошибка при попытке изменить видимость Microsoft Word. Программа или документ уже закрыты."); }
                    _application.Visible = value;
                }
                // завершение public bool Visible  
            }

            // количество страниц
            public int PagesCount
            {
                get
                {
                    int pagesCount = 0;
                    Word.WdStatistic pagesStatType = Word.WdStatistic.wdStatisticPages;
                    pagesCount = _document.ComputeStatistics(pagesStatType, ref _missingObj);
                    return pagesCount;
                }
            }


            // 
            /// <summary>
            /// КОНСТРУКТОР ПУСТОЙ ДОКУМЕНТ
            /// </summary>
            /// <param name="startVisible"></param>
            /// <exception cref="TrackConverterException"></exception>
            public WordDocument(bool startVisible)
            {
                //создаем обьект приложения word
                _application = new Word.Application();

                // если вылетим не этом этапе, приложение останется открытым
                try
                {
                    _document = _application.Documents.Add(ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj);
                }
                catch (Exception error)
                {
                    this.Close();
                    throw new TrackConverterException(error.Message, error);
                }
                Visible = startVisible;

                // устанавливаем текущую позицию в начало документа
                SetSelectionToBegin();
            }

            public WordDocument() : this(false) { }


            /// <summary>
            /// КОНСТРУКТОР ШАБЛОН
            /// </summary>
            /// <param name="templatePath"></param>
            /// <param name="startVisible"></param>
            /// <exception cref="TrackConverterException"></exception>
            public WordDocument(string templatePath, bool startVisible)
            {
                //создаем обьект приложения word
                _application = new Word.Application();

                // создаем путь к файлу используя имя файла
                _templatePathObj = templatePath;

                // если вылетим не этом этапе, приложение останется открытым
                try
                {
                    _document = _application.Documents.Add(ref _templatePathObj, ref _missingObj, ref _missingObj, ref _missingObj);

                }
                catch (Exception error)
                {
                    this.Close();
                    throw new TrackConverterException(error.Message, error);
                }
                Visible = startVisible;

                // устанавливаем текущую позицию в начало документа
                SetSelectionToBegin();
            }

            public WordDocument(string templatePath)
                : this(templatePath, false) { }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="pathToTemplate"></param>
            /// <param name="fillWordDoc"></param>
            /// <exception cref="TrackConverterException"></exception>
            public static void FillShowTemplate(string pathToTemplate, Action<WordDocument> fillWordDoc)
            {

                // ошибку при открытии обработает вышестоящий код формы
                WordDocument wordDoc = null;
                try
                {
                    wordDoc = new WordDocument(pathToTemplate);
                    fillWordDoc(wordDoc);
                }
                catch (Exception error)
                {
                    if (wordDoc != null)
                    { wordDoc.Close(); }
                    throw new TrackConverterException(error.Message, error);
                }

                wordDoc.Visible = true;
            }

            // выбор текста в документе для свойства selectedText ИЩЕТ ПЕРВОЕ ВХОЖДЕНИЕ
            public void SetSelectionToText(string stringToFind)
            {
                Word.Range foundRange = findRangeByString(stringToFind);
                _currentRange = foundRange ?? throw new Exception("Ошибка при поиске текста в MS Word. Не удалось найти и выбрать заданный текст: " + stringToFind);
                _selection = new WordSelection(foundRange, false);
            }

            // поиск и выбор текста в документе Word внутри строки-контейнера, сначала ищется контейнер, потом текст внутри него
            public void SetSelectionToText(string containerStr, string stringToFind)
            {

                Word.Range containerRange = null;
                Word.Range foundRange = null;

                containerRange = findRangeByString(containerStr);
                if (containerRange != null)
                {
                    foundRange = findRangeByString(containerRange, stringToFind);
                }

                if (foundRange != null)
                {
                    _currentRange = foundRange;
                    _selection = new WordSelection(foundRange, false);
                }
                else
                {
                    throw new Exception("Ошибка при поиске текста в MS Word. Не удалось найти заданную область для поиска текста: " + containerStr);
                }
                // завершение public void searchSelectText(string containerStr, string stringToFind)
            }

            // встаем на закладку, то есть получаем обьект Range по имени закладки и заноми его в переменужж экземпляра класса, доступную для других методов
            public void SetSelectionToBookmark(string bookmarkName, bool isParagraph)
            {
                if (Closed)
                { throw new Exception("Ошибка при обращении к документу Word. Документ уже закрыт."); }

                Object bookmarkNameObj = bookmarkName;
                Word.Range bookmarkRange = null;
                try
                {
                    bookmarkRange = _document.Bookmarks.get_Item(ref bookmarkNameObj).Range;
                }
                catch (Exception error)
                {
                    throw new Exception("Ошибка при поиске закладки " + bookmarkName + " в документе Word. Сообщение от Word: " + error.Message);
                }
                _currentRange = bookmarkRange;
                _selection = new WordSelection(_currentRange, isParagraph);
            }

            public void SetSelectionToBookmark(string bookmarkName)
            {
                SetSelectionToBookmark(bookmarkName, false);
            }

            public void SetSelectionToBegin()
            {
                object start = 0;
                object end = 0;
                this._currentRange = this._document.Range(ref start, ref end);
                _selection = new WordSelection(_currentRange);
            }

            public void SetSelectionToCell(int rowIndex, int columnIndex)
            {
                if (_table == null)
                { throw new Exception("Ошибка при выборе ячейки в таблице Word, не выбрана текущая таблица."); }

                _currentRange = _table.Cell(rowIndex, columnIndex).Range;
                _selection = new WordSelection(_currentRange, false);
            }
            

            /// <summary>
            /// вставляем пустой абзац, доступ к его тексту и свойствам через
            /// </summary>
            /// <exception cref="TrackConverterException"></exception>
            public void InsertParagraphAfter()
            {
                if (Closed)
                { throw new Exception("Ошибка при обращении к документу Word. Документ уже закрыт."); }
                // сворачиваем текущую позицию и переходим в ее конец
                Object collapseDirection = Word.WdCollapseDirection.wdCollapseEnd;
                try
                {
                    _currentRange.Collapse(ref collapseDirection);
                    //вставляем абзац
                    _currentRange.InsertParagraphAfter();
                    _selection = new WordSelection(_currentRange);
                }
                catch (Exception wordError)
                {
                    throw new TrackConverterException(wordError.Message, wordError);
                }
            }

            // упрощенная функция для вставки текста с умолчальными параметрами
            public void InsertParagraphAfter(string textToInsert)
            {
                this.InsertParagraphAfter();
                this._selection.Text = textToInsert;
            }

            public void InsertTable(int numRows, int numColumns)
            {
                InsertTable(numRows, numColumns, BorderType.Single);
            }

            public void InsertTable(int numRows, int numColumns, BorderType border)
            {

                _table = _document.Tables.Add(_currentRange, numRows, numColumns, ref _missingObj, ref _missingObj);

                switch (border)
                {
                    case BorderType.None:
                        _table.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleNone;
                        _table.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleNone;
                        break;
                    case BorderType.Single:
                        _table.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                        _table.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                        break;
                    case BorderType.Double:
                        _table.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleDouble;
                        _table.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleDouble;
                        break;
                    default:
                        _table.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleNone;
                        _table.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleNone;
                        break;
                }


                _currentRange = _table.Range;
                _selection = new WordSelection(_currentRange, false);
            }

            public void SetColumnWidth(int columnIndex, int widthPixels)
            {
                if (_table == null)
                { throw new Exception("Ошибка при установке ширины колонки в таблице Word - текущая таблица не выбрана (SetColumnWidth(int columnIndex, int widthPixels))"); }
                _table.Columns[columnIndex].SetWidth(widthPixels, Word.WdRulerStyle.wdAdjustNone);
            }

            // ВСТАВЛЯЕМ ПУСТУЮ СТРАНИЦУ С ОДНИМ ПАРАГРАФОМ В КОНЦЕ, тупо добавляем пустые абзацы до появления следующей страницы
            public void InsertPageAtEnd()
            {
                object missing = Missing.Value;
                object what = Word.WdGoToItem.wdGoToLine;
                object which = Word.WdGoToDirection.wdGoToLast;
                Word.Range endRange = _document.GoTo(ref what, ref which, ref missing, ref missing);
                _currentRange = endRange;
                _selection = new WordSelection(endRange);

                // пока не изменится количество страниц вставляем пустые абзацы в конце
                int oldPagesCount = PagesCount;
                while (oldPagesCount == PagesCount)
                {
                    InsertParagraphAfter();
                }
                InsertParagraphAfter();
            }

            //ВСТАВЛЯЕМ ДОКУМЕНТ WORD ИЗ ФАЙЛА
            public void InsertFile(string pathToFile)
            {
                if (_currentRange == null)
                { throw new Exception("Ничего не выбрано"); }
                _currentRange.InsertFile(pathToFile);
            }

            // СОХРАНЯЕМ НА ДИСК С ПЕРЕЗАПИСЬЮ СУЩЕСТВУЮЩЕГО ФАЙЛА
            public void Save(string pathToSave)
            {
                Object pathToSaveObj = pathToSave;
                _document.SaveAs(ref pathToSaveObj, Word.WdSaveFormat.wdFormatDocument, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj, ref _missingObj);
            }

            // закрытие открытого документа и приложения
            public void Close()
            {

                if (_document != null)
                {
                    _document.Close(ref _falseObj, ref _missingObj, ref _missingObj);
                }
                _application.Quit(ref _missingObj, ref _missingObj, ref _missingObj);
                _document = null;
                _application = null;
            }

            // поиск строки и ее замена на заданную строку
            public void ReplaceAllStrings(string strToFind, string replaceStr)
            {
                if (Closed)
                { throw new Exception("Ошибка при обращении к документу Word. Документ уже закрыт."); }

                // обьектные строки для Word
                object strToFindObj = strToFind;
                object replaceStrObj = replaceStr;
                // диапазон документа Word
                Word.Range wordRange;
                //тип поиска и замены
                object replaceTypeObj;

                replaceTypeObj = Word.WdReplace.wdReplaceAll;

                try
                {
                    // обходим все разделы документа
                    for (int i = 1; i <= _document.Sections.Count; i++)
                    {
                        // берем всю секцию диапазоном
                        wordRange = _document.Sections[i].Range;

                        /*
                        Обходим редкий глюк в Find, ПРИЗНАННЫЙ MICROSOFT, метод Execute на некоторых машинах вылетает с ошибкой "Заглушке переданы неправильные данные / Stub received bad data"  Подробности: http://support.microsoft.com/default.aspx?scid=kb;en-us;313104
                        // выполняем метод поиска и  замены обьекта диапазона ворд
                        wordRange.Find.Execute(ref strToFindObj, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing, ref replaceStrObj, ref replaceTypeObj, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing);
                        */

                        Word.Find wordFindObj = wordRange.Find;


                        object[] wordFindParameters = new object[15] { strToFindObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj, replaceStrObj, replaceTypeObj, _missingObj, _missingObj, _missingObj, _missingObj };

                        wordFindObj.GetType().InvokeMember("Execute", BindingFlags.InvokeMethod, null, wordFindObj, wordFindParameters);
                    }
                }
                catch (Exception error)
                {
                    throw new Exception("Ошибка при выполнении замене всех строк  в документе Word.  " + error.Message + " (ReplaceAllStrings)");
                }
                // завершение функции поиска и замены SearchAndReplace
            }

            // ВЫБИРАЕМ ТАБЛИЦУ ПО ПОРЯДКОВОМ НОМЕРУ НАЧИНАЯ С ЕДИНИЦЫ
            public void SelectTable(int tableNumber)
            {
                try
                {
                    _table = _document.Tables[tableNumber];
                }
                catch (Exception error)
                {
                    throw new Exception("Таблица с номером " + tableNumber + " не найдена в документе Word. Подробности: " + error.Message);
                }
                _currentRange = _table.Range;
                _selection = new WordSelection(_table.Range, true, false);
            }


            public void AddRowToTable()
            {
                _table.Rows.Add(ref _missingObj);
            }

            // ИЩЕТ ПЕРВОЕ ВХОЖДЕНИЕ функция поиска Range  в документе Word строке, возвращает соответствующий строке Range, на входе строка для поиска
            private Word.Range findRangeByString(string stringToFind)
            {
                // проверяем, не закрыт ли документ или приложение ворд
                if (Closed)
                { throw new Exception("Ошибка при обращении к документу Word. Документ уже закрыт."); }
                // оформляем обьектные параметры
                object stringToFindObj = stringToFind;
                Word.Range wordRange;
                bool rangeFound;

                //в цикле обходим все разделы документа, получаем Range, запускаем поиск
                // если поиск вернул true, он долже ужать Range до найденное строки, выходим и возвращаем Range
                // обходим все разделы документа
                for (int i = 1; i <= _document.Sections.Count; i++)
                {
                    // берем всю секцию диапазоном
                    wordRange = _document.Sections[i].Range;

                    /*
                   // Обходим редкий глюк в Find, ПРИЗНАННЫЙ MICROSOFT, метод Execute на некоторых машинах вылетает с ошибкой "Заглушке переданы неправильные данные / Stub received bad data"  Подробности: http://support.microsoft.com/default.aspx?scid=kb;en-us;313104
                   // выполняем метод поиска и  замены обьекта диапазона ворд
                   rangeFound = wordRange.Find.Execute(ref stringToFindObj, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing);
                     */

                    Word.Find wordFindObj = wordRange.Find;

                    object[] wordFindParameters = new object[15] { stringToFindObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj };

                    rangeFound = (bool)wordFindObj.GetType().InvokeMember("Execute", BindingFlags.InvokeMethod, null, wordFindObj, wordFindParameters);

                    if (rangeFound)
                    { return wordRange; }

                }

                // если ничего не нашли, возвращаем null
                return null;
            }

            // ищет строку ВНУТРИ Range, при успехе возвращает Range  для этой строки
            private Word.Range findRangeByString(Word.Range containerRange, string stringToFind)
            {
                // проверяем, не закрыт ли документ или приложение ворд
                if (Closed)
                { throw new Exception("Ошибка при обращении к документу Word. Документ уже закрыт."); }
                // оформляем обьектные параметры
                object stringToFindObj = stringToFind;
                bool rangeFound;

                /*
                Обходим редкий глюк в Find, ПРИЗНАННЫЙ MICROSOFT, метод Execute на некоторых машинах вылетает с ошибкой "Заглушке переданы неправильные данные / Stub received bad data" 
                 http://support.microsoft.com/default.aspx?scid=kb;en-us;313104
                rangeFound = containerRange.Find.Execute(ref stringToFindObj, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing, ref wordMissing);
                 */

                Word.Find wordFindObj = containerRange.Find;

                object[] wordFindParameters = new object[15] { stringToFindObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj, _missingObj };

                rangeFound = (bool)wordFindObj.GetType().InvokeMember("Execute", BindingFlags.InvokeMethod, null, wordFindObj, wordFindParameters);





                if (rangeFound)
                { return containerRange; }
                else
                { return null; }

            }


            // завершение class WordDocument
        }

        public enum TextAligment { Left, Center, Right, Justify }

        public enum BorderType { None, Single, Double }

        /// Версия 1.3
        // класс - параграф MS Word, обертка над обьектом Range который соответствует параграфу (вставленному в документ), дает доступ к стилю текста, выравниваю, размеру шрифта (возможно дальнейшее расширение, по идее создается внутри класса документа при вставке абзаца как публичное свойство-обьект, позволяющее заполнять свои поля по необходимости
        private class WordSelection
        {
            private Word.Range _range;
            private readonly bool _insertParagrAfterText = true;

            // надо проверить возможно не нужно после последнего исправления (вставки параграфа после текста)
            private Word.WdParagraphAlignment _savedAligment;

            // конструктор принимает обьект Range
            public WordSelection(Word.Range inputRange) : this(inputRange, true, true)
            {
            }

            public WordSelection(Word.Range inputRange, bool insertParagrAfterText)
                : this(inputRange, insertParagrAfterText, true)
            {

            }

            public WordSelection(Word.Range inputRange, bool insertParagrAfterText, bool setDefaultStyle)
            {
                _range = inputRange;

                _insertParagrAfterText = insertParagrAfterText;

                if (setDefaultStyle)
                {
                    _savedAligment = _range.ParagraphFormat.Alignment;
                    _range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                    _savedAligment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                    _range.Italic = 0;
                    _range.Bold = 0;
                }
                else
                {
                    _savedAligment = _range.ParagraphFormat.Alignment;
                }
            }



            public bool Bold
            {
                get
                {
                    // нет точных данных о возможных значениях, 1 жирный, 0 нет... но по идее может быть и еще
                    if (_range.Bold == 1)
                    { return true; }
                    else
                    { return false; }
                }

                set
                {
                    if (value == true)
                    { _range.Bold = 1; }
                    else
                    { _range.Bold = 0; }
                }
                // завершение public bool Bold
            }

            public bool Italic
            {
                get
                {
                    // открытый вопрос с возможными занчениями в Word по умолчанию, документация плохая
                    if (_range.Italic == 1)
                    { return true; }
                    else
                    { return false; }
                }
                set
                {
                    if (value == true)
                    { _range.Italic = 1; }
                    else
                    { _range.Italic = 0; }
                }
                // завершение  public bool Italic
            }

            //свойство-перечисление выравнивания
            public TextAligment Aligment
            {
                get
                {
                    if (_range.ParagraphFormat.Alignment == Word.WdParagraphAlignment.wdAlignParagraphLeft)
                    { return TextAligment.Left; }
                    else if (_range.ParagraphFormat.Alignment == Word.WdParagraphAlignment.wdAlignParagraphCenter)
                    { return TextAligment.Center; }
                    else if (_range.ParagraphFormat.Alignment == Word.WdParagraphAlignment.wdAlignParagraphRight)
                    { return TextAligment.Right; }
                    else if (_range.ParagraphFormat.Alignment == Word.WdParagraphAlignment.wdAlignParagraphJustify)
                    { return TextAligment.Justify; }
                    else
                    { throw new Exception("Ошибка при определении типа вырвнивания текста"); }
                }
                set
                {
                    if (value == TextAligment.Left)
                    {
                        _range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                        _savedAligment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                    }
                    else if (value == TextAligment.Center)
                    {
                        _range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                        _savedAligment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    }
                    else if (value == TextAligment.Right)
                    {
                        _range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                        _savedAligment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                    }
                    else if (value == TextAligment.Justify)
                    {
                        _range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphJustify;
                        _savedAligment = Word.WdParagraphAlignment.wdAlignParagraphJustify;
                    }
                    else
                    { throw new Exception("Неизвестный тип выравнивания текста"); }
                }
                // завершение public TextAligment Aligment
            }

            //собственно текст параграфа
            public string Text
            {
                get { return _range.Text; }
                set
                {
                    _range.Text = value;
                    // обход глюка Word, при заполнении свойства "текст" параграф затирается и текст присоединяется к предыдущему параграфу, Range начинаеьт указывать на предыдущий параграф
                    if (_insertParagrAfterText)
                    {
                        _range.InsertParagraphAfter();
                    }
                    // обход глюка Word: при вставке текста выравнивание ставится по центру
                    _range.ParagraphFormat.Alignment = this._savedAligment;

                }
                // завершение public string Text
            }

            //свойство int размер шрифта
            public int FontSize
            {
                get { return Convert.ToInt32(this._range.Font.Size); }
                set
                {
                    if (value < 1)
                    { throw new Exception("Ошибка при установке размера шрифта  Word. Размер шрифта не может быть меньше 1."); }
                    float fontSize = (float)Convert.ToDouble(value);
                    this._range.Font.Size = fontSize;
                }
                // завершение public int FontSize
            }

            public BorderType Border
            {
                set
                {
                    switch (value)
                    {
                        case BorderType.None:
                            _range.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleNone;
                            break;
                        case BorderType.Single:
                            _range.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                            break;
                        case BorderType.Double:
                            _range.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleDouble;
                            break;
                        default:
                            _range.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleNone;
                            break;
                    }
                }

                get
                {
                    switch (_range.Borders.OutsideLineStyle)
                    {
                        case Word.WdLineStyle.wdLineStyleNone:
                            return BorderType.None;
                        case Word.WdLineStyle.wdLineStyleSingle:
                            return BorderType.Single;
                        case Word.WdLineStyle.wdLineStyleDouble:
                            return BorderType.Double;
                        default:
                            return BorderType.None;
                    }
                }
            }



            // завершение class WordParagraph
        }
    }
}