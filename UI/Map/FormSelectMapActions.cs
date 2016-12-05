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
using TrackConverter.Lib.Classes;

namespace TrackConverter.UI.Map {

    /// <summary>
    /// окно выбора действий с выделенной областью карты
    /// </summary>
    public partial class FormSelectMapActions: Form {

        /// <summary>
        /// выделенная область
        /// </summary>
        private RectLatLng Selection;
        private FormMap map;

        /// <summary>
        /// создает окно операций с выделенной областью
        /// </summary>
        /// <param name="Selection">выделенная область</param>
        /// <param name="Map">окно карты</param>
        public FormSelectMapActions(FormMap Map, RectLatLng Selection ) 
        {
            InitializeComponent();
            this.Selection = Selection;
            this.map = Map;

            #region раздел загрузки

            //поставщик карты
            comboBoxLoadMapProvider.Items.Clear();
            foreach ( MapProviderRecord mpr in Vars.Options.Map.AllProviders ) {
                comboBoxLoadMapProvider.Items.Add( mpr.Title );
            }

            comboBoxLoadMapProvider.SelectedIndex = Vars.Options.Map.MapProvider.ID;

            //масштаб
            checkedListBoxZoom.CheckOnClick = true;
            checkedListBoxZoom.Items.Clear();
            for ( int i = 1; i <= 18; i++ ) {
                checkedListBoxZoom.Items.Add( i );
            }
            #endregion

            #region раздел сохранения

            comboBoxSaveImageFormat.SelectedIndex = 0;

            //поставщик карты
            comboBoxSaveMapProvider.Items.Clear();
            foreach ( MapProviderRecord mpr in Vars.Options.Map.AllProviders ) {
                comboBoxSaveMapProvider.Items.Add( mpr.Title );
            }

            comboBoxSaveMapProvider.SelectedIndex = Vars.Options.Map.MapProvider.ID;

            #endregion


        }

        /// <summary>
        /// выбор места сохранения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSelectSaveFile_Click( object sender, EventArgs e ) {
            SaveFileDialog sf = new SaveFileDialog();
            sf.InitialDirectory = Vars.Options.Common.LastFileSaveDirectory;
           
            if ( sf.ShowDialog( this ) == System.Windows.Forms.DialogResult.OK )
                textBoxSavePath.Text = sf.FileName;
        }

        /// <summary>
        /// начать операцию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStart_Click( object sender, EventArgs e ) {
            if ( tabControlActions.SelectedIndex == 0 )
                LoadMap();
            if ( tabControlActions.SelectedIndex == 1 )
                SaveMap();
        }

        /// <summary>
        /// сохранение карты
        /// </summary>
        private void SaveMap() {
            MapProviderRecord prov= MapProviderRecord.FromID( comboBoxSaveMapProvider.SelectedIndex );
            List<int> zooms = new List<int>();
            foreach ( int o in checkedListBoxZoom.SelectedIndices ) {
                zooms.Add( o );
            }

        }

        /// <summary>
        /// загрузка карты
        /// </summary>
        private void LoadMap() {
            MapProviderRecord prov = MapProviderRecord.FromID( comboBoxSaveMapProvider.SelectedIndex );


        }

        /// <summary>
        /// отмена действия
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click( object sender, EventArgs e ) {
            this.Close();
        }


    }
}
