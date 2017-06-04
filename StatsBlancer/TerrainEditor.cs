using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using Wartorn.GameData;
using Wartorn.Utility;
using Wartorn.CustomJsonConverter;

using Newtonsoft.Json;
using System.IO;

namespace StatsBlancer
{
    public partial class TerrainEditor : Form
    {
        private Dictionary<TerrainType, int> _DefenseStar;
        private List<TerrainType> terraintypes;
        private TerrainType selectedTerrain = TerrainType.Sea;

        public TerrainEditor()
        {
            InitializeComponent();

            terraintypes = Enum.GetValues(typeof(TerrainType)).Cast<TerrainType>().ToList();

            foreach (TerrainType terraintype in terraintypes)
            {
                listBox_terraintypes.Items.Add(terraintype.ToString());
            }

            _DefenseStar = new Dictionary<TerrainType, int>();
            string defensestartable = File.ReadAllText(@"data\defensestartable.txt");
            JsonConvert.DeserializeObject<KeyValuePair<TerrainType, int>[]>(defensestartable).ToList().ForEach(kvp =>
            {
                _DefenseStar.Add(kvp.Key, kvp.Value);
            });
        }

        private void button_save_terrain_Click(object sender, EventArgs e)
        {
            int temp = 0;
            if (int.TryParse(textBox_defstar.Text, out temp))
            {
                _DefenseStar[selectedTerrain] = temp;
            }
        }

        private void button_done_Click(object sender, EventArgs e)
        {
            Directory.CreateDirectory(@"data\");
            File.WriteAllText(@"data\defensestartable.txt", JsonConvert.SerializeObject(_DefenseStar.ToArray(), Formatting.Indented));
        }

        private void listBox_terraintypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedTerrain = listBox_terraintypes.SelectedItem.ToString().ToEnum<TerrainType>();
            textBox_defstar.Text = _DefenseStar[selectedTerrain].ToString();
        }

        private void ValidateInput(object sender, EventArgs e)
        {
            if (!Regex.IsMatch(textBox_defstar.Text, "^[0-9]+$"))
            {
                textBox_defstar.Text = "0";
            }
        }
    }
}
