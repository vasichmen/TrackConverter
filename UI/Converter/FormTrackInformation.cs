using System;
using System.Diagnostics;
using System.Windows.Forms;
using TrackConverter.Lib.Tracking;

namespace TrackConverter.UI.Converter {

    /// <summary>
    /// окно подробной информации о маршруте
    /// </summary>
    public partial class FormTrackInformation: Form {

        /// <summary>
        /// маршрут для которого выводится информация
        /// </summary>
        private BaseTrack trackFile;

        /// <summary>
        /// создает окно подробной информации о маршруте
        /// </summary>
        /// <param name="trackFile">маршрут, о котором будет выведена информация</param>
        public FormTrackInformation(BaseTrack trackFile ) {
            InitializeComponent();
            this.trackFile = trackFile;

            textBoxCount.Text = trackFile.Count.ToString();
            textBoxDescription.Text = trackFile.Description;
            textBoxDistance.Text = trackFile.Distance.ToString();
            if (trackFile.Count > 0)
            {
                textBoxStart.Text = trackFile[0].Coordinates.ToString("{lat}\r\n{lon}", "ddºmm'ss.s\"H");
                textBoxFinish.Text = trackFile[trackFile.Count - 1].Coordinates.ToString("{lat}\r\n{lon}", "ddºmm'ss.s\"H");
            }
            textBoxName.Text = trackFile.Name;
            textBoxSpeed.Text = trackFile.KmphSpeed.ToString();
            textBoxTime.Text = trackFile.Time.ToString();
            pictureBoxColor.BackColor = trackFile.Color;
        }

        /// <summary>
        /// результат работы диалога
        /// </summary>
        public BaseTrack Result { get; set; }

        /// <summary>
        /// открытие маршрута в яндекс
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOpenYandex_Click( object sender, EventArgs e ) {
            string ss = Serializer.Serialize( 100.ToString(), trackFile, FileFormats.YandexLink );
            Process.Start( ss );
        }

        /// <summary>
        /// открыте маршрута в викимапиа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOpenWikimapia_Click( object sender, EventArgs e ) {
            Process.Start( Serializer.Serialize( 100.ToString(), trackFile, FileFormats.WikimapiaLink ) );
        }

        /// <summary>
        /// сохранение изменений
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click( object sender, EventArgs e ) {
            trackFile.Name = textBoxName.Text;
            trackFile.Description = textBoxDescription.Text;
            trackFile.Color=pictureBoxColor.BackColor;
            Result = trackFile;
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// закрыть окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClose_Click( object sender, EventArgs e ) {
            Close();
        }

        /// <summary>
        /// открытие начальной точки в яндекс
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabelStartYandex_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e ) {
            Process.Start( trackFile[0].Coordinates.ExportYandex() );
        }

        /// <summary>
        /// открытие начальной точки в гугл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabelStartGoogle_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e ) {
            Process.Start( trackFile[0].Coordinates.ExportGoogle() );

        }

        /// <summary>
        /// открыть конечную точку в яндекс
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabelFinishYandex_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e ) {
            Process.Start( trackFile[trackFile.Count - 1].Coordinates.ExportYandex() );

        }

        /// <summary>
        /// открытие конечной точки в гугл
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabelFinishGoogle_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e ) {
            Process.Start( trackFile[trackFile.Count - 1].Coordinates.ExportGoogle() );
        }

        /// <summary>
        /// нажатие и изменение цвета маршрута
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = pictureBoxColor.BackColor;
            if (cd.ShowDialog(this) == DialogResult.OK)
                pictureBoxColor.BackColor = cd.Color;
        }
    }
}
