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
    public partial class UnitEditor : Form
    {
        private Dictionary<UnitPair, int> _DammageTable;
        private Dictionary<UnitType, int> _Cost;
        private Dictionary<UnitType, int> _Gas;
        private Dictionary<UnitType, int> _MovementRange;
        private Dictionary<UnitType, int> _VisionRange;
        private Dictionary<UnitType, Range> _AttackRange;
        private Dictionary<MovementType, Dictionary<TerrainType, int>> _TravelCost;

        private List<UnitType> unittypes;
        private List<MovementType> movementtypes;
        private List<TerrainType> terraintypes;
        private UnitType selectedUnit = UnitType.None;

        private Dictionary<string, TextBox> textboxs;

        public UnitEditor()
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
                return settings;
            };

            InitializeComponent();

            LoadData();

            dataGridView_unitdmg.Columns.Add("", "");
            foreach (UnitType unittype in unittypes)
            {
                listBox_unitSelect.Items.Add(unittype.ToString());

                dataGridView_unitdmg.Columns.Add(unittype.ToString(), unittype.ToString());
                
                dataGridView_unitdmg.Rows.Add(unittype.ToString());
            }

            listBox_unitSelect.SetSelected(0, true);

            textboxs = new Dictionary<string, TextBox>();
            textboxs.Add(textBox_cost.Name, textBox_cost);
            textboxs.Add(textBox_move.Name, textBox_move);
            textboxs.Add(textBox_vision.Name, textBox_vision);
            textboxs.Add(textBox_rangemax.Name, textBox_rangemax);
            textboxs.Add(textBox_rangemin.Name, textBox_rangemin);
            textboxs.Add(textBox_gas.Name, textBox_gas);
            textboxs.Add(textBox_fuelperturn.Name, textBox_fuelperturn);
            textboxs.Add(textBox_movecostPlain.Name, textBox_movecostPlain);
            textboxs.Add(textBox_movecostRiver.Name, textBox_movecostRiver);
            textboxs.Add(textBox_movecostMountain.Name, textBox_movecostMountain);
            textboxs.Add(textBox_movecostForest.Name, textBox_movecostForest);
        }

        #region load data
        private void LoadData()
        {
            unittypes = Enum.GetValues(typeof(UnitType)).Cast<UnitType>().ToList();
            unittypes.Remove(UnitType.None);

            movementtypes = Enum.GetValues(typeof(MovementType)).Cast<MovementType>().ToList();
            movementtypes.Remove(MovementType.None);

            terraintypes = Enum.GetValues(typeof(TerrainType)).Cast<TerrainType>().ToList();

            _DammageTable = new Dictionary<UnitPair, int>();
            _Cost = new Dictionary<UnitType, int>();
            _Gas = new Dictionary<UnitType, int>();
            _MovementRange = new Dictionary<UnitType, int>();
            _VisionRange = new Dictionary<UnitType, int>();
            _AttackRange = new Dictionary<UnitType, Range>();
            _TravelCost = new Dictionary<MovementType, Dictionary<TerrainType, int>>();

            string dmgtable = File.ReadAllText(Path.GetFullPath(@"data\dmgtable.txt"));
            JsonConvert.DeserializeObject<KeyValuePair<UnitPair, int>[]>(dmgtable).ToList().ForEach(kvp =>
            {
                _DammageTable.Add(kvp.Key, kvp.Value);
            });

            string costtable = File.ReadAllText(@"data\costtable.txt");
            JsonConvert.DeserializeObject<KeyValuePair<UnitType, int>[]>(costtable).ToList().ForEach(kvp =>
            {
                _Cost.Add(kvp.Key, kvp.Value);
            });

            string gastable = File.ReadAllText(@"data\gastable.txt");
            JsonConvert.DeserializeObject<KeyValuePair<UnitType, int>[]>(gastable).ToList().ForEach(kvp =>
            {
                _Gas.Add(kvp.Key, kvp.Value);
            });

            string movrangetable = File.ReadAllText(@"data\movementrangetable.txt");
            JsonConvert.DeserializeObject<KeyValuePair<UnitType, int>[]>(movrangetable).ToList().ForEach(kvp =>
            {
                _MovementRange.Add(kvp.Key, kvp.Value);
            });

            string visionrangetable = File.ReadAllText(@"data\visionrangetable.txt");
            JsonConvert.DeserializeObject<KeyValuePair<UnitType, int>[]>(visionrangetable).ToList().ForEach(kvp =>
            {
                _VisionRange.Add(kvp.Key, kvp.Value);
            });

            string attackrangetable = File.ReadAllText(@"data\attackrangetable.txt");
            JsonConvert.DeserializeObject<KeyValuePair<UnitType, Range>[]>(attackrangetable).ToList().ForEach(kvp =>
            {
                _AttackRange.Add(kvp.Key, kvp.Value);
            });

            string traversecosttable = File.ReadAllText(@"data\traversecosttable.txt");
            _TravelCost = JsonConvert.DeserializeObject<Dictionary<MovementType, Dictionary<TerrainType, int>>>(traversecosttable);
        }
        #endregion

        private void ValidateInput(object sender, EventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            if (!Regex.IsMatch(textbox.Text, "^[0-9]+$"))
            {
                //this.Controls[textbox.Name].Text = "";
                textboxs[textbox.Name].Text = "0";
            }
        }

        private void listBox_unitSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedUnit = listBox_unitSelect.SelectedItem.ToString().ToEnum<UnitType>();
            label_selectedUnit.Text = selectedUnit.ToString();

            textBox_cost.Text = _Cost[selectedUnit].ToString();
            textBox_move.Text = _MovementRange[selectedUnit].ToString();
            textBox_vision.Text = _VisionRange[selectedUnit].ToString();
            textBox_rangemax.Text = _AttackRange[selectedUnit].Max.ToString();
            textBox_rangemin.Text = _AttackRange[selectedUnit].Min.ToString();
            textBox_gas.Text = _Gas[selectedUnit].ToString();
            textBox_fuelperturn.Text = "0";
            textBox_movecostPlain.Text = _TravelCost[selectedUnit.GetMovementType()][TerrainType.Plain].ToString();
            textBox_movecostRiver.Text = _TravelCost[selectedUnit.GetMovementType()][TerrainType.River].ToString();
            textBox_movecostMountain.Text = _TravelCost[selectedUnit.GetMovementType()][TerrainType.Mountain].ToString();
            textBox_movecostForest.Text = _TravelCost[selectedUnit.GetMovementType()][TerrainType.Tree].ToString();
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            if (selectedUnit == UnitType.None)
            {
                return;
            }

            _Cost[selectedUnit] = int.Parse(textBox_cost.Text);
            _MovementRange[selectedUnit] = int.Parse(textBox_move.Text);
            _VisionRange[selectedUnit] = int.Parse(textBox_vision.Text);
            _AttackRange[selectedUnit] = new Range(int.Parse(textBox_rangemax.Text), int.Parse(textBox_rangemin.Text));
            _Gas[selectedUnit] = int.Parse(textBox_gas.Text);

            _TravelCost[selectedUnit.GetMovementType()][TerrainType.Plain] = int.Parse(textBox_movecostPlain.Text);
            _TravelCost[selectedUnit.GetMovementType()][TerrainType.River] = int.Parse(textBox_movecostRiver.Text);
            _TravelCost[selectedUnit.GetMovementType()][TerrainType.Mountain] = int.Parse(textBox_movecostMountain.Text);
            _TravelCost[selectedUnit.GetMovementType()][TerrainType.Tree] = int.Parse(textBox_movecostForest.Text);
        }

        private void button_done_Click(object sender, EventArgs e)
        {
            Directory.CreateDirectory(@"data\");
            File.WriteAllText(@"data\dmgtable.txt", JsonConvert.SerializeObject(_DammageTable.ToArray(), Formatting.Indented));
            File.WriteAllText(@"data\costtable.txt", JsonConvert.SerializeObject(_Cost.ToArray(), Formatting.Indented));
            File.WriteAllText(@"data\gastable.txt", JsonConvert.SerializeObject(_Gas.ToArray(), Formatting.Indented));
            File.WriteAllText(@"data\movementrangetable.txt", JsonConvert.SerializeObject(_MovementRange.ToArray(), Formatting.Indented));
            File.WriteAllText(@"data\visionrangetable.txt", JsonConvert.SerializeObject(_VisionRange.ToArray(), Formatting.Indented));
            File.WriteAllText(@"data\attackrangetable.txt", JsonConvert.SerializeObject(_AttackRange.ToArray(), Formatting.Indented));
            File.WriteAllText(@"data\traversecosttable.txt", JsonConvert.SerializeObject(_TravelCost, Formatting.Indented));
        }

        private void dataGridView_unitdmg_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}