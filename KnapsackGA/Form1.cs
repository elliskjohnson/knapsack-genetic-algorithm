using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnapsackGA
{
    public partial class Form1 : Form
    {
        private static List<int> _items = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private static List<int> _weights = new List<int> { 6, 14, 13, 9, 11, 16, 20, 17, 3, 5 };
        private static List<int> _values = new List<int> { 18, 60, 47, 55, 53, 72, 90, 83, 21, 16 };

        private bool button2Pressed;
        private int _population;
        private int _maxWeight;
        private double _mutationProb;
        private int _generations;
        private Knapsack _knapsack;
        private bool outputEveryGeneration;

        public int Population { get => _population; set => _population = value; }
        public int MaxWeight { get => _maxWeight; set => _maxWeight = value; }
        public double MutationProb { get => _mutationProb; set => _mutationProb = value; }
        public int Generations { get => _generations; set => _generations = value; }
        public int ConvergenceSens { get; set; }


        #region LoadTable
        public DataTable GetTableValues()
        {
            DataTable d = new DataTable();

            d.Columns.Add("Items");
            d.Columns.Add("Weight");
            d.Columns.Add("Value");

            for (int i = 0; i < _items.Count; i++)
            {
                d.Rows.Add();
            }
            for (int i = 0; i < _items.Count; i++)
            {
                d.Rows[i][0] = _items[i];
                d.Rows[i][1] = _weights[i];
                d.Rows[i][2] = _values[i];
            }

            return d;
        }
        #endregion
        public Form1()
        {
            InitializeComponent();
            dataGridView1.DataSource = GetTableValues();
            button2Pressed = false;
            outputEveryGeneration = false;
        }

        // Run All Generations
        private void button1_Click(object sender, EventArgs e)
        {
            ConvertFields();
            CreateKnapsack();
            if (Generations == 0)
            {
                _knapsack.Optimize(richTextBox1, outputEveryGeneration);
            }
            else
            {
                _knapsack.Optimize(Generations, outputEveryGeneration, richTextBox1);
            }
            label8.Text = GetCurrentItems();
        }

        private string GetCurrentItems()
        {
            this.label8.ResetText();
            StringBuilder builder = new StringBuilder();
            string curMaxItem = this._knapsack.CurMaxString();
            int i = 0;
            int total_val = 0;
            int total_weight = 0;
            foreach (char x in curMaxItem)
            {
                if (x == '1')
                {
                    total_val += _values[i];
                    total_weight += _weights[i];
                    builder.Append("Item " + _items[i].ToString() + " with weight " + _weights[i].ToString() + " and value " + _values[i].ToString() + "\n");
                }
                i++;
            }
            builder.Append("Total Weight: " + total_weight.ToString() + " with total value: " + total_val.ToString() + "\n");
            return builder.ToString();
        }

        // Run Single Generation
        private void button2_Click(object sender, EventArgs e)
        {
            button2Pressed = true;
            if (_knapsack != null)
            {
                if (Generations == 0 && (this._knapsack.GenerationNoChangeCount < (ConvergenceSens * Population)))
                {
                    this._knapsack.RunSingleGenerationButton(richTextBox1);
                    richTextBox1.ScrollToCaret();
                    label8.Text = GetCurrentItems();
                }
                else if (_knapsack.Generation < Generations)
                {
                    _knapsack.RunSingleGenerationButton(richTextBox1);
                    richTextBox1.ScrollToCaret();
                    label8.Text = GetCurrentItems();
                }
                else
                {
                    if (Generations == 0)
                    {
                        richTextBox1.AppendText("Reached Auto Convergence\n");
                        richTextBox1.ScrollToCaret();
                    }
                    else
                    {
                        richTextBox1.AppendText("Reached Specified Number of Generations\n");
                        richTextBox1.ScrollToCaret();
                    }
                    
                }
            }

        }

        private void ConvertFields()
        {
            Population = Convert.ToInt32(numericUpDown2.Value);
            MaxWeight = Convert.ToInt32(numericUpDown1.Value);
            MutationProb = double.Parse(textBox1.Text);
            Generations = int.Parse(textBox2.Text);
            ConvergenceSens = Convert.ToInt32(numericUpDown3.Value);
        }

        private void CreateKnapsack()
        {
            this._knapsack = new Knapsack(Population, MaxWeight, 10, MutationProb, Generations, ConvergenceSens);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ConvertFields();
            CreateKnapsack();
            this._knapsack.InitializePopulation();
            this.richTextBox1.Clear();
            this.label8.ResetText();
        }

        private void button4_Click(object sender, EventArgs e)
        {

            this.richTextBox1.Clear();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            outputEveryGeneration = checkBox1.Checked;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (button2Pressed)
            {
                if (Generations == 0)
                {
                    _knapsack.CompleteOptimizeAutoConverge(richTextBox1, outputEveryGeneration);
                }
                else
                {
                    _knapsack.CompleteOptimizeByGenerations(Generations, outputEveryGeneration, richTextBox1);
                }
                label8.Text = GetCurrentItems();
            }
        }
    }
}
