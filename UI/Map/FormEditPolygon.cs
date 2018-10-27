using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.UI.Map
{
    public partial class FormEditPolygon : Form
    {
        /// <summary>
        /// метод, выполняемый после редактирования маршрута
        /// </summary>
        private Action<TrackFile> actionAfter;

        /// <summary>
        /// действие после отмены
        /// </summary>
        private Action cancelAction;
        private TrackFile Polygon;
        private bool userCancel = true;


        public FormEditPolygon(string title, TrackFile creatingPolygon, Action<TrackFile> afterAction, Action cancelAction)
        {
            InitializeComponent();
            this.actionAfter = afterAction;
            this.cancelAction = cancelAction;
            this.Polygon = creatingPolygon;
            this.Text = title;
            Program.winMain.selectedPolygonPointIndex = Polygon.Count - 1;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (this.Polygon.Color.IsEmpty)
                this.Polygon.Color = Vars.Options.Converter.GetColor();
            Program.winMain.creatingPolygonOverlay.Clear();

            //выполнение действия после создания маршрута
            if (actionAfter != null)
                actionAfter.Invoke(this.Polygon);
            this.userCancel = false;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonDeletePoint_Click(object sender, EventArgs e)
        {
            if (Polygon == null) { return; }

            //если удалять нечего - выходим
            if (this.Polygon.Count == 0) { return; }

            //удаление из списка точек
            this.Polygon.Remove(Program.winMain.selectedPolygonPointIndex);
            //удаление из списка маркеров
            Program.winMain.creatingPolygonOverlay.Markers.RemoveAt(Program.winMain.selectedPolygonPointIndex);

            //если можно, уменьшение индекса
            if (Program.winMain.selectedPolygonPointIndex > 0)
                Program.winMain.selectedPolygonPointIndex--;
            else
                //если уменьшать некуда, но есть еще точки, то выделяем первую точку
                if (this.Polygon.Count > 0)
                Program.winMain.selectedPolygonPointIndex = 0;

            //обновление карты
            if (Program.winMain.isCreatingPolygon)
                Program.winMain.mapHelper.ShowCreatingRoute(Program.winMain.creatingPolygonOverlay, Program.winMain.creatingPolygon);
        }

        private void formEditPolygon_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.userCancel)
            {
                if (cancelAction != null)
                    cancelAction.Invoke();
                Program.winMain.creatingPolygonOverlay.Clear();
            }

            Program.winMain.selectedPolygonPointIndex = 0;
            Program.winMain.gmapControlMap.DragButton = MouseButtons.Left;
        }
    }
}
