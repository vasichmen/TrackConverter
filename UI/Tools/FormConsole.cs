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
        /// <summary>
        /// список послдний вызванных команд
        /// </summary>
        Stack<string> commands = new Stack<string>();

        /// <summary>
        /// замена посленей строки вывода на указанную строку
        /// </summary>
        private Action<string> setLastLineOutput;

        /// <summary>
        /// создает новое окно консоли
        /// </summary>
        public FormConsole()
        {
            InitializeComponent();
            textBoxCommand.Text = Vars.Options.Common.LastConsoleCommand;
            textBoxCommand.SelectionStart = 4;

            //действие замены последней строке в окне вывода
            setLastLineOutput = new Action<string>((text) =>
            {
                try
                {
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() =>
                        {
                            string[] all = textBoxResult.Lines;
                            textBoxResult.Clear();
                            textBoxResult.Lines = all.Take(all.Length - 1).ToArray();
                            textBoxResult.Text += "\r\n" + text;
                            Application.DoEvents();
                        }));
                    }
                    else
                    {
                        string[] all = textBoxResult.Lines;
                        textBoxResult.Clear();
                        textBoxResult.Lines = all.Take(all.Length - 1).ToArray();
                        textBoxResult.Text += "\r\n" + text;
                        Application.DoEvents();
                    }
                }
                catch (Exception) { return; }
            });
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
                this.commands.Push(textBoxCommand.Text);

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
                                        {
                                            this.textBoxResult.Text += "База данных ETOPO2 не загружена!\r\n";
                                            break;
                                        }
                                        Task ts = new Task(new Action(() =>
                                        {
                                            GeoInfo.ETOPO2Provider.ExportSQLite(com[4], this.setLastLineOutput);
                                            this.Invoke(new Action(() =>
                                            {
                                                textBoxCommand.Enabled = true;
                                                this.setLastLineOutput.Invoke("Экспортирование ETOPO2 в SQLite успешно завершено!\r\ncom>");
                                            }));

                                        }
                                        ));
                                        ts.Start();
                                        textBoxCommand.Enabled = false;
                                    }
                                    else
                                    {
                                        this.textBoxResult.Text += "База данных ETOPO2 не подключена!\r\n";
                                        break;
                                    }
                        }
                        break;
                    default:
                        textBoxResult.Text += "Неизвестная команда \"" + com[0] + "\"\r\ncom>";
                        break;
                }
                textBoxResult.Text += "com>";
                textBoxCommand.Clear();
            }
            if (e.KeyData == Keys.Up)
            {
                if (commands.Count == 0) return;
                textBoxCommand.Text = commands.Pop();
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
