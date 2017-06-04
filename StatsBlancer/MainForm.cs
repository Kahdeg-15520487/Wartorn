using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Wartorn;
using Wartorn.CustomJsonConverter;
using Wartorn.GameData;
using Wartorn.Utility;

namespace StatsBlancer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new UnitPairJsonConverter());
                settings.Converters.Add(new UnitTypeJsonConverter());
                settings.Converters.Add(new MovementTypeJsonConverter());
                settings.Converters.Add(new TerrainTypeJsonConverter());
                settings.Converters.Add(new RangeJsonConverter());
                settings.Converters.Add(new Dictionary_MovementType_Dictionary_TerrainType_int_JsonConverter());
                settings.Converters.Add(new Dictionary_UnitType_Dictionary_UnitType_int_JsonConverter());
                return settings;
            };

            InitializeComponent();
        }

        private void button_unit_Click(object sender, EventArgs e)
        {
            this.Hide();
            UnitEditor uniteditor = new UnitEditor();
            var dialogResult = uniteditor.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                //save
                MessageBox.Show("saved");
            }
            this.Show();
        }

        private void button_terrain_Click(object sender, EventArgs e)
        {
            this.Hide();
            TerrainEditor terraineditor = new TerrainEditor();
            var dialogResult = terraineditor.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                //save
                MessageBox.Show("saved");
            }
            this.Show();
        }
    }
}
