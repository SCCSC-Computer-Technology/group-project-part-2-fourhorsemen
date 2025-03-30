using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace group_project_part_2_fourhorsemen_layout
{
    public partial class Form1 : Form
    {

        public String sText, sType;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            searchTypeBox.Items.Add("Game System");
            searchTypeBox.Items.Add("Title");
            searchTypeBox.Items.Add("Genre");
            searchTypeBox.Items.Add("Developer");
            searchTypeBox.Items.Add("Year of Release");

            sType = "All";
        }

        private void clear()
        {
            searchBox.Text = "";
            searchTypeBox.SelectedIndex = -1;
            resultsBox.Text = "";
            resultsBox.Visible = false;
            sType = "All";
        }

        private void hpButton_Click(object sender, EventArgs e)
        {
            resultsBox.Text = "";
            resultsBox.Visible = false;
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            sText = searchBox.Text;
            resultsBox.Text = "You searched for [" + sText + "] by <" + sType + ">\n\nThis information will be available very soon. Please be patient whuile we build the best Video game database on the web!!!";
            resultsBox.Visible = true;
        }

        private void searchTypeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            sType = (String)searchTypeBox.SelectedItem;
        }
    }
}
