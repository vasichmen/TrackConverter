using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TrackConverter.Lib.Classes;
using TrackConverter.Lib.Data.Providers.InternetServices;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.UI.Map
{
    /// <summary>
    /// окно редактирвания объекта Wikimapia
    /// </summary>
    public partial class FormEditWikimapiaObject : Form
    {
        /// <summary>
        /// редактируемый объект
        /// </summary>
        private Wikimapia.ObjectEditInfo obj;
        private bool userCanc = true;
        private Action<Wikimapia.ObjectEditInfo> afterAction;
        private Action cancelAction;

        public FormEditWikimapiaObject(Wikimapia.ObjectEditInfo obj, Action<Wikimapia.ObjectEditInfo> afterAction, Action cancelAction = null)
        {
            InitializeComponent();
            this.obj = obj;
            this.afterAction = afterAction;
            this.cancelAction = cancelAction;
        }

        private void formEditWikimapiaObject_Shown(object sender, EventArgs e)
        {
            //название объекта
            if (string.IsNullOrEmpty(obj.Title))
                this.Text = "Новый объект";
            else
                this.Text = obj.Title;

            Program.winMain.gmapControlMap.ShowVectorLayerObject(new GMapPolygon(obj.Geometry.GMapPoints, "editing polygon"), -1, Program.winMain.creatingPolygonOverlay, true);


            //заполнение информации
            textBoxTitle.Text = obj.Title;
            textBoxDescription.Text = obj.Description;
            textBoxWikiLink.Text = obj.WikipediaLink;
            comboBoxStreet.Text = obj.StreetName;
            textBoxBuilding.Text = obj.BuildingNumber;
            checkBoxIsBuilding.Checked = obj.IsBuilding;

            //категории объекта
            listBoxCategories.Items.Clear();
            listBoxCategories.Items.AddRange(obj.Categories.ToArray());

            //список ближайших улиц
            List<Wikimapia.StreetInfo> strs = Wikimapia.ServiceEngine.GetNearestStreets(Program.winMain.gmapControlMap.GetViewArea());
            comboBoxStreet.Items.AddRange(strs.ToArray());
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            obj.IsBuilding = checkBoxIsBuilding.Checked;
            obj.BuildingNumber = textBoxBuilding.Text.Trim();
            obj.Description = textBoxDescription.Text;
            obj.Title = textBoxTitle.Text;
            obj.WikipediaLink = textBoxWikiLink.Text;
            obj.StreetName = comboBoxStreet.Text;
            if (comboBoxStreet.SelectedItem != null)
                obj.StreetID = (comboBoxStreet.SelectedItem as Wikimapia.StreetInfo).ID;
            else
                obj.StreetID = 0;
            obj.ObjectType = 1;
            userCanc = false;
            DialogResult = DialogResult.OK;
            if (this.afterAction != null)
                afterAction.Invoke(obj);
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonRemoveCategory_Click(object sender, EventArgs e)
        {

        }

        private void buttonAddCategory_Click(object sender, EventArgs e)
        {

        }

        private void textBoxCatSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonEditPhotos_Click(object sender, EventArgs e)
        {

        }

        private void buttonEditGeometry_Click(object sender, EventArgs e)
        {
            Program.winMain.creatingPolygonOverlay.Clear();
            this.WindowState = FormWindowState.Minimized;
            TrackFile old = obj.Geometry;
            Program.winMain.mapHelper.BeginEditPolygon(obj.Geometry, (track) =>
             {
                 obj.Geometry = track;
                 this.WindowState = FormWindowState.Normal;
                 Program.winMain.gmapControlMap.ShowVectorLayerObject(new GMapPolygon(obj.Geometry.GMapPoints, "editing polygon"), -1, Program.winMain.creatingPolygonOverlay, true);
                 Program.winMain.isCreatingPolygon = false;
             }, () =>
             {
                 this.WindowState = FormWindowState.Normal;
                 Program.winMain.gmapControlMap.ShowVectorLayerObject(new GMapPolygon(obj.Geometry.GMapPoints, "editing polygon"), -1, Program.winMain.creatingPolygonOverlay, true);
                 Program.winMain.isCreatingPolygon = false;
             });
        }

        private void formEditWikimapiaObject_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.winMain.creatingPolygonOverlay.Clear();
            this.DialogResult = DialogResult.Cancel;
            if (userCanc)
                if (cancelAction != null)
                    cancelAction.Invoke();
        }
    }
}
