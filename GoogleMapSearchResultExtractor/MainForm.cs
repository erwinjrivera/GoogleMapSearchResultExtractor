using GoogleMapSearchResultExtractor.Controller;
using GoogleMapSearchResultExtractor.Model;
using GoogleMapSearchResultExtractor.Service;
using GoogleMapSearchResultExtractor.View;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace GoogleMapSearchResultExtractor
{
    public partial class MainForm : Form, IMainFormView
    {
        private MapResultController _controller;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _controller = new MapResultController(
                this,
                new Model.MapResult(),
                new GoogleSearchService());

            _controller.Initialize();
        }

        public bool SearchBoxEnabled 
        { 
            set
            {
                txtSearch.Enabled = value;
            }
        }
        public bool SearchButtonEnabled 
        {
            set
            {
                btnSearch.Enabled = value;
            }
        }

        public bool ExtractButtonEnabled
        {
            set
            {
                txtResultCount.Enabled = value;
            }
        }

        public void ChangeButtonBackground(string backgroundName)
        {
            btnSearch.Image = imageList1.Images[backgroundName];
        }

        public void ClearItems()
        {
            flowLayoutPanel1.Controls.Clear();
        }

        public string ResultCount
        {
            set
            {
                txtResultCount.Text = value;
            }
        }

        public string StatusText
        {
            set
            {
                lblStatus.Text = value;
            }
        }

        public void AddItemResult(ICardView item)
        {
            flowLayoutPanel1.Controls.Add((CardItemUserControl)item);
        }

        private void On_Paint(object sender, PaintEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(BackColor))
            {
                e.Graphics.FillRectangle(brush, ClientRectangle);
            }
               
            e.Graphics.DrawRectangle(Pens.Gainsboro, 0, 0, splitContainer1.Panel2.Width, splitContainer1.Panel2.Height);
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                _controller.Search(txtSearch.Text);
            }
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            if (!Searching)
                btnSearch.Image = imageList1.Images["hover"];
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            if (!Searching)
                btnSearch.Image = imageList1.Images["inactive"];
        }

        public bool Searching { get; set; }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            _controller.Search(txtSearch.Text);

            //ExtractDomainFromUrl(txtSearch.Text);

            //_controller.TestFlowLayoutPanel(500);
        }

        private void ExtractDomainFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            string domain = url;

            if (domain.Contains(@"://"))
            {
                domain = url.Split(new string[] { "://" }, 2, StringSplitOptions.None)[1];
            }

            Debug.WriteLine(domain.Split('/')[0].Replace("www.", string.Empty));
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (!Searching)
                btnSearch.Image = imageList1.Images["active"];
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            if (!Searching)
                btnSearch.Image = imageList1.Images["inactive"];
        }

        private void flowLayoutPanel1_ControlAdded(object sender, ControlEventArgs e)
        {
            _controller.UpdateResultCount(flowLayoutPanel1.Controls.Count);
        }

        private void extractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (flowLayoutPanel1.Controls.Count > 0)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.Filter = "Text Documents|*.txt";
                saveFileDialog.CheckFileExists = false;
                saveFileDialog.Title = "Save result to Text";
                saveFileDialog.DefaultExt = ".txt";
                saveFileDialog.CheckPathExists = true;
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                saveFileDialog.OverwritePrompt = false;
                saveFileDialog.FileName = "Google Search Export - " + String.Format("{0:s}", DateTime.Now).Replace(":", "") + ".txt";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _controller.ExtractResultToTextFile(saveFileDialog.FileName);
                }
            }
        }
    }
}
