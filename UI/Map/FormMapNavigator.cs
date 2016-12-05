using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using TrackConverter.UI.Converter;
using TrackConverter.Lib.Classes;

namespace TrackConverter.UI.Map
{
    /// <summary>
    /// окно навигации в карте
    /// </summary>
    public partial class FormMapNavigator : Form
    {
        /// <summary>
        /// создает панель навигации для заданной карты
        /// </summary>
        public FormMapNavigator()
        {
            InitializeComponent();

            new ToolTip().SetToolTip(buttonDown, "Сдвинуть карту вниз");
            new ToolTip().SetToolTip(buttonUp, "Сдвинуть карту вверх");
            new ToolTip().SetToolTip(buttonZoomIn, "Приблизить");
            new ToolTip().SetToolTip(buttonZoomOut, "Отдалить");
            new ToolTip().SetToolTip(buttonLeft, "Сдвинуть карту влево");
            new ToolTip().SetToolTip(buttonRight, "Сдвинуть карту вправо");
        }

        /// <summary>
        /// активация панели навигации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMapNavigator_Activated(object sender, EventArgs e)
        {
            this.Opacity = 1;
        }

        /// <summary>
        /// деактивация панели навигации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMapNavigator_Deactivate(object sender, EventArgs e)
        {
            this.Opacity = 1;
        }

        /// <summary>
        /// уменьшение зума
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonZoomOut_Click(object sender, EventArgs e)
        {
            Program.winMap.gmapControlMap.Zoom--;
        }

        /// <summary>
        /// увеличение зума
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonZoomIn_Click(object sender, EventArgs e)
        {
            Program.winMap.gmapControlMap.Zoom++;
        }

      

        /// <summary>
        /// при отпускании кнопки остановка движения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonMoveMap_MouseUp(object sender, MouseEventArgs e)
        {
            Program.winMap.canMoveMap = false;
        }

        /// <summary>
        /// нажатие кнопки перемещения карты
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonMoveMap_MouseDown(object sender, MouseEventArgs e)
        {
            Program.winMap.canMoveMap = true;
            switch (((Button)sender).Name)
            {
                case "buttonLeft":
                    Program.winMap.canMoveMapDirection = Keys.Left;
                    break;
                case "buttonRight":
                    Program.winMap.canMoveMapDirection = Keys.Right;
                    break;
                case "buttonUp":
                    Program.winMap.canMoveMapDirection = Keys.Up;
                    break;
                case "buttonDown":
                    Program.winMap.canMoveMapDirection = Keys.Down;
                    break;

            }
        }

        /// <summary>
        /// переход на предыдущую позицию карты
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGoBack_Click(object sender, EventArgs e)
        {
            if (Program.winMap.PositionsStack.Count != 0)
            {
                Program.winMap.PositionsStack.Pop();
                if (Program.winMap.PositionsStack.Count == 0) return;
                KeyValuePair<string,Coordinate> ques = Program.winMap.PositionsStack.Peek();
                Program.winMap.gmapControlMap.Position = ques.Value.GMap;
                Program.winMap.toolStripComboBoxGoTo.Text = ques.Key;
            }

        }

    }
}
