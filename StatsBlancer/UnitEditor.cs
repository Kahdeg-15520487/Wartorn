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

namespace StatsBlancer {
	public partial class UnitEditor : Form {
		private Dictionary<UnitType, Dictionary<UnitType, int>> _DammageTable;
		private Dictionary<UnitType, UnitStat> _UnitStat;

		private List<UnitType> unittypes;
		private List<MovementType> movementtypes;
		private List<TerrainType> terraintypes;
		private UnitType selectedUnit = UnitType.None;

		private Dictionary<string, TextBox> textboxs;

		bool isSavedToFile = false;

		public UnitEditor() {
			InitializeComponent();

			LoadData();

			dataGridView_unitdmg.ColumnHeadersHeight = 200;
			dataGridView_unitdmg.RowHeadersWidth = 120;
			dataGridView_unitdmg.TopLeftHeaderCell.Value = "                 Defender ▶" + Environment.NewLine + "Attacker ▼";
			foreach (UnitType unittype in unittypes) {
				listBox_unitSelect.Items.Add(unittype.ToString());

				dataGridView_unitdmg.Columns.Add(unittype.ToString(), unittype.ToString());
				dataGridView_unitdmg.Rows.Add();
				dataGridView_unitdmg.Rows[(int)unittype - 1].HeaderCell.Value = unittype.ToString();
			}

			foreach (var kvp in _DammageTable) {
				foreach (var kvp2 in kvp.Value) {
					dataGridView_unitdmg[(int)kvp2.Key - 1, (int)kvp.Key - 1].Value = kvp2.Value;
				}
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
			textboxs.Add(textBox_ammo.Name, textBox_ammo);
			textboxs.Add(textBox_actionpoint.Name, textBox_actionpoint);
		}

		#region load data
		private void LoadData() {
			unittypes = Enum.GetValues(typeof(UnitType)).Cast<UnitType>().ToList();
			unittypes.Remove(UnitType.None);

			movementtypes = Enum.GetValues(typeof(MovementType)).Cast<MovementType>().ToList();
			movementtypes.Remove(MovementType.None);

			terraintypes = Enum.GetValues(typeof(TerrainType)).Cast<TerrainType>().ToList();

			_DammageTable = new Dictionary<UnitType, Dictionary<UnitType, int>>();
			_UnitStat = new Dictionary<UnitType, UnitStat>();

			string dmgtable = File.ReadAllText(Path.GetFullPath(@"data\dmgtable.txt"));
			_DammageTable = JsonConvert.DeserializeObject<Dictionary<UnitType, Dictionary<UnitType, int>>>(dmgtable);

			string unitstat = File.ReadAllText(@"data\unitstat.txt");
			_UnitStat = JsonConvert.DeserializeObject<Dictionary<UnitType, UnitStat>>(unitstat);
		}
		#endregion

		private void ValidateInput(object sender, EventArgs e) {
			TextBox textbox = (TextBox)sender;
			if (!Regex.IsMatch(textbox.Text, "^[0-9]+$")) {
				textboxs[textbox.Name].Text = "0";
			}
		}

		private void listBox_unitSelect_SelectedIndexChanged(object sender, EventArgs e) {
			selectedUnit = listBox_unitSelect.SelectedItem.ToString().ToEnum<UnitType>();
			label_selectedUnit.Text = selectedUnit.ToString();

			textBox_cost.Text = _UnitStat[selectedUnit].Cost.ToString();
			textBox_move.Text = _UnitStat[selectedUnit].MovementRange.ToString();
			textBox_vision.Text = _UnitStat[selectedUnit].VisionRange.ToString();
			textBox_rangemax.Text = _UnitStat[selectedUnit].AttackRange.Max.ToString();
			textBox_rangemin.Text = _UnitStat[selectedUnit].AttackRange.Min.ToString();
			textBox_gas.Text = _UnitStat[selectedUnit].Gas.ToString();
			textBox_fuelperturn.Text = "0";
			textBox_ammo.Text = _UnitStat[selectedUnit].Ammo.ToString();
			textBox_actionpoint.Text = _UnitStat[selectedUnit].ActionPoint.ToString();
		}

		private void button_save_Click(object sender, EventArgs e) {
			if (selectedUnit == UnitType.None) {
				return;
			}
			UnitStat unitStat = new UnitStat() {
				Cost = int.Parse(textBox_cost.Text),
				MovementRange = int.Parse(textBox_move.Text),
				VisionRange = int.Parse(textBox_vision.Text),
				AttackRange = new Range(int.Parse(textBox_rangemax.Text), int.Parse(textBox_rangemin.Text)),
				Gas = int.Parse(textBox_gas.Text),
				Ammo = int.Parse(textBox_ammo.Text),
				ActionPoint = int.Parse(textBox_actionpoint.Text)
			};
			isSavedToFile = false;
		}

		private void button_done_Click(object sender, EventArgs e) {
			Directory.CreateDirectory(@"data\");
			File.WriteAllText(@"data\dmgtable.txt", JsonConvert.SerializeObject(_DammageTable, Formatting.Indented));
			File.WriteAllText(@"data\unitstat.txt", JsonConvert.SerializeObject(_UnitStat.ToArray(), Formatting.Indented));
			isSavedToFile = true;
		}

		private void dataGridView_unitdmg_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
			UnitType attacker;
			UnitType defender;
			int dmg;

			if (e.ColumnIndex < 0) {
				return;
			}

			if (!int.TryParse(dataGridView_unitdmg[e.RowIndex, e.ColumnIndex].Value.ToString(), out dmg)) {
				return;
			}

			attacker = (UnitType)(e.RowIndex + 1);
			defender = (UnitType)(e.ColumnIndex + 1);

			_DammageTable[attacker][defender] = dmg;

			isSavedToFile = false;
		}
	}
}