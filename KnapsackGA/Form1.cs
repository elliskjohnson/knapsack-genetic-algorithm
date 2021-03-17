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

        public int Population { get => _population; set => _population = value; }
        public int MaxWeight { get => _maxWeight; set => _maxWeight = value; }
        public double MutationProb { get => _mutationProb; set => _mutationProb = value; }
        public int Generations { get => _generations; set => _generations = value; }


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
        }

        // Run All Generations
        private void button1_Click(object sender, EventArgs e)
        {
            ConvertFields();
            CreateKnapsack();
            if (Generations == 0)
            {
                _knapsack.Optimize();
            }
            else
            {
                _knapsack.Optimize(Generations, true);
            }
        }

        // Run Single Generation
        private void button2_Click(object sender, EventArgs e)
        {
            ConvertFields();
            if (!button2Pressed)
            {
                CreateKnapsack();
                _knapsack.InitializePopulation();
            }
            if (Generations == 0 || _knapsack.Generation < Generations)
            {
                _knapsack.RunSingleGenerationOutput();
            }
        }

        private void ConvertFields()
        {
            Population = Convert.ToInt32(numericUpDown2.Value);
            MaxWeight = Convert.ToInt32(numericUpDown1.Value);
            MutationProb = double.Parse(textBox1.Text);
            Generations = int.Parse(textBox2.Text);
        }

        private void CreateKnapsack()
        {
            _knapsack = new Knapsack(Population, MaxWeight, 10, MutationProb, Generations);
        }

    }
}
