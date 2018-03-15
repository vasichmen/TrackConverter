using GMap.NET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackConverter.UI.Converter;
using TrackConverter.Lib.Tracking;
using GMap.NET.WindowsForms;

namespace TrackConverter.UI.Map
{
    /// <summary>
    /// окно редактирования маршрута
    /// </summary>
    public partial class FormEditRoute : Form
    {
        /// <summary>
        /// метод, выполняемый после редактирования маршрута
        /// </summary>
        private Action<TrackFile> actionAfter;

        /// <summary>
        /// действие после отмены
        /// </summary>
        private Action cancelAction;
        private GMapOverlay overlay;
        private TrackFile route;
        private bool userCancel = true;

        /// <summary>
        /// создет новое окно редактирования маршрута
        /// </summary>
        /// <param name="actionAfter">метод, который будет выполнен после нажатия на кнопку СОХРАНИТЬ</param>
        /// <param name="cancelAction">Действие, выполняемое после нажатия кнопки отменить или закрытии окна</param>
        /// <param name="overlay">слой для вывода редактируемого маршрута</param>
        /// <param name="route">редактируемый маршрут</param>
        /// <param name="Title">Заголовок окна редактирования</param>
        public FormEditRoute(string Title, TrackFile route,GMapOverlay overlay, Action<TrackFile> actionAfter, Action cancelAction)
        {
            
            InitializeComponent();
            this.actionAfter = actionAfter;
            this.cancelAction = cancelAction;
            this.route = route;
            this.overlay = overlay;
            this.Text = Title;
            Program.winMain.selectedPointIndex = route.Count-1;
        }

        /// <summary>
        /// удаление выделенной точки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDeletePoint_Click(object sender, EventArgs e)
        {
            if (route == null) { return; }

            //если удалять нечего - выходим
            if (this.route.Count == 0) { return; }

            //удаление из списка точек
            this.route.Remove(Program.winMain.selectedPointIndex);
            //удаление из списка маркеров
            this.overlay.Markers.RemoveAt(Program.winMain.selectedPointIndex);

            //если можно, уменьшение индекса
            if (Program.winMain.selectedPointIndex > 0)
                Program.winMain.selectedPointIndex--;
            else
                //если уменьшать некуда, но есть еще точки, то выделяем первую точку
                if (this.route.Count > 0)
                    Program.winMain.selectedPointIndex = 0;

            //обновление карты
            if (Program.winMain.isCreatingRoute)
                Program.winMain.mapHelper.ShowCreatingRoute(Program.winMain.creatingRouteOverlay, Program.winMain.creatingRoute);
            if (Program.winMain.isRuling)
                Program.winMain.mapHelper.ShowCreatingRoute(Program.winMain.rulerRouteOverlay, Program.winMain.rulerRoute);
                

            this.route.CalculateAll();
            Program.winMain.toolStripStatusLabelInfo.Text = "Расстояние: " + this.route.Distance + " км";
        }

        /// <summary>
        /// сохранение изменений
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (this.route.Color.IsEmpty)
                this.route.Color = Vars.Options.Converter.GetColor();
            //выполнение действия после создания маршрута
            if (actionAfter != null)
                actionAfter.Invoke(this.route);
            this.userCancel = false;
            Close();
        }

        /// <summary>
        /// отмена созодания маршрута
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// отмена создания маршрута
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormEditRoute_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.userCancel && cancelAction != null)
                cancelAction.Invoke();
            if (Program.winMain.isCreatingRoute)
            {
                Program.winMain.isCreatingRoute = false;
                Program.winMain.creatingRouteOverlay.Clear();
            }
            if (Program.winMain.isRuling)
                Program.winMain.isRuling = false;
            Program.winMain.toolStripStatusLabelInfo.Text = "";
            this.overlay.Clear();
            Program.winMain.selectedPointIndex = 0;
            Program.winMain.gmapControlMap.DragButton = MouseButtons.Left;
        }
    }
}
