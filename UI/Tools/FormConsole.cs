using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackConverter.Lib.Data;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.UI.Tools
{
    /// <summary>
    /// окно консоли
    /// </summary>
    public partial class FormConsole : Form
    {
        List<string> commands = new List<string>();
        int pos = 0;

        /// <summary>
        /// создает новое окно консоли
        /// </summary>
        public FormConsole()
        {
            InitializeComponent();
            textBoxCommand.Text = Vars.Options.Common.LastConsoleCommand;
            textBoxCommand.SelectionStart = 4;
        }

        /// <summary>
        /// обработка команды
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxCommand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                string[] com = textBoxCommand.Text.Substring(4).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                textBoxResult.Text += textBoxCommand.Text.Substring(4) + "\r\n";
                Vars.Options.Common.LastConsoleCommand = textBoxCommand.Text;
                this.commands.Add(textBoxCommand.Text);
                pos++;

                if (com.Length == 0) return;
                switch (com[0].ToLower())
                {
                    case "exit":
                        this.Close();
                        break;
                    case "help":
                    case "?":
                        this.PrintHelp();
                        break;
                    case "open":
                        if (com.Length == 3 && com[1] == "file")
                            Program.winConverter.OpenFile(com[2], false);
                        if (com.Length == 3 && com[1] == "link")
                            Program.winConverter.OpenLink(com[2]);
                        break;
                    case "export":
                        if (com.Length == 5)
                        {
                            if (com[1] == "etopo2")
                                if (com[3] == "sql")
                                    if (GeoInfo.IsETOPO2Ready)
                                    {
                                        if (GeoInfo.ETOPO2Provider == null)
                                            Vars.TaskLoadingETOPO2.Wait();
                                        Program.winMain.BeginOperation();
                                        Task ts = new Task(new Action(() =>
                                        {
                                            GeoInfo.ETOPO2Provider.ExportSQLite(com[4], Program.winMain.setCurrentOperation);
                                            Program.winMain.EndOperation();
                                        }
                                        ));
                                        ts.Start();
                                    }
                                    else this.textBoxResult.Text += "База данных ETOPO2 не загружена!\r\n";
                        }
                        break;
                    default:
                        textBoxResult.Text += "Неизвестная команда \"" + com[0] + "\"\r\n";
                        break;
                }
                textBoxResult.Text += "com>";
                textBoxCommand.Clear();
            }
            if (e.KeyData == Keys.Up)
            {
                if (pos == 0) return;
                textBoxCommand.Text = "com>" + commands[pos - 1];
                pos--;
            }

        }

        /// <summary>
        /// вывод справки
        /// </summary>
        private void PrintHelp()
        {
            textBoxResult.Text += "СПРАВКА\r\n";
            textBoxResult.Text += "exit\t - выход из консоли\r\n";
            textBoxResult.Text += "?, help\t - вызов справки\r\n";
            textBoxResult.Text += "open [формат] [имя_файла]\t - открытие файла поддерживаемого формата\r\n";
            textBoxResult.Text += "\t[формат]\t - file - открытие файла, link - открытие ссылки\r\n";
            textBoxResult.Text += "\t[имя_файла]\t - путь к файлу маршрута. В пути не допускаются пробелы\r\n";
            textBoxResult.Text += "export etopo2 to sql [имя_файла] - сохранение базы данных ETOPO2 в указанный файл как базу данных SQLite. В названии файла не допускаются пробелы.\r\n";
        }

        /// <summary>
        /// запрет стирания com>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxCommand_TextChanged(object sender, EventArgs e)
        {
            if (textBoxCommand.Text.Length < 4)
            {
                textBoxCommand.Text = "com>";
                textBoxCommand.SelectionStart = 4;
            }
        }
    }
}
