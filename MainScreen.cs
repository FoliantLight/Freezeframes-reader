using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Freezeframes_reader
{
    public partial class MainScreen : Form
    {
        private FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
        private OpenFileDialog openFileDialog1 = new OpenFileDialog();

        private int freezeframeCount = 16;                  //Количество ошибок в журнале. Стандартно 16

        int tempOpenedFileCount = 0;                        //Количество найденых файлов в папке

        public MainScreen()
        {
            string tempString = "";

            InitializeComponent();

            for (byte i = 0; i < freezeframeCount; i++)
            {
                tempString += "*." + (i < 10 ? "0" : "") + i + ";";
            }
            openFileDialog1.Filter = "СКАТ-02 файлы ошибок|" + tempString;
        }

        private void readFreezeframes(string path, bool isFolder)
        {

            tabControl1.Controls.Clear();
            tempOpenedFileCount = 0;

            if (isFolder)
            {
                for (byte i = 0; i < freezeframeCount; i++)
                {
                    string endName = (i < 10 ? "0" : "") + i;
                    string filePath = path + "\\freezeframe." + endName;
                    if (File.Exists(filePath))
                    {
                        tempOpenedFileCount++;
                        
                        string[] errorDescription = getErrorDescription(filePath);
                        CurveList charts = parseFile(filePath);

                        if (errorDescription != null)
                            addTab(endName, errorDescription, charts);
                    }
                }

                openedFolder.Text = "Папка: " + path;
                tabControl1.SelectedIndex = 0;
            }
            else
            {

            }
        }

        private void addTab(string endName, string[] errorDescription, CurveList chart)
        {
            TabPage tabPage = new TabPage();
            GroupBox groupBox = new GroupBox();
            Panel panel = new Panel();
            ZedGraphControl graphControl = new ZedGraphControl();

            tabPage.Name = endName + "|" + errorDescription[0] + "|" + errorDescription[1] + "|" + errorDescription[2];
            tabPage.Text = "freezeframe." + endName;

            groupBox.Dock = DockStyle.Right;
            groupBox.Location = new Point(803, 3);
            groupBox.Name = "graph." + endName;
            groupBox.Width = 170;
            groupBox.TabIndex = 0;
            groupBox.TabStop = false;
            groupBox.Text = "Графики";

            panel.AutoScroll = true;
            panel.Dock = DockStyle.Fill;
            panel.Location = new Point(0, 0);
            panel.Name = "scroll" + endName;

            graphControl.Dock = DockStyle.Fill;
            graphControl.Location = new Point(0, 0);
            graphControl.Name = "graph" + endName;
            graphControl.ScrollGrace = 0D;
            graphControl.ScrollMaxX = 4D;
            graphControl.ScrollMaxY = 0D;
            graphControl.ScrollMaxY2 = 0D;
            graphControl.ScrollMinX = 0D;
            graphControl.ScrollMinY = 0D;
            graphControl.ScrollMinY2 = 0D;
            graphControl.Size = new Size(800, 581);
            graphControl.GraphPane.Title.Text = "";
            graphControl.GraphPane.Legend.IsVisible = false;
            graphControl.GraphPane.XAxis.Title.Text = "";
            graphControl.GraphPane.YAxis.Title.Text = "";
            graphControl.GraphPane.IsFontsScaled = false;
            graphControl.ZoomEvent += graphZoomEvent;
            graphControl.MouseMoveEvent += graphMouseMoveEvent;

            graphControl.GraphPane.CurveList = chart;

            //**** Настройка масштабирования графиков ****
            graphControl.GraphPane.YAxis.Scale.Min = -3500;
            graphControl.GraphPane.YAxis.Scale.Max = 3500;
            graphControl.GraphPane.XAxis.Scale.Min = -3.9;
            graphControl.GraphPane.XAxis.Scale.Max = 1;

            //**** Настройка магнитуд осей ****
            graphControl.GraphPane.XAxis.Scale.MajorStep = 1.0;
            graphControl.GraphPane.XAxis.Scale.MinorStep = 0.1;
            graphControl.GraphPane.XAxis.Scale.FontSpec.Size = 10;

            graphControl.GraphPane.YAxis.Scale.MajorStep = 500;
            graphControl.GraphPane.YAxis.Scale.MinorStep = 100;
            graphControl.GraphPane.YAxis.Scale.FontSpec.Size = 10;

            //**** Настройка сетки графиков ****
            //Видимость сеток
            graphControl.GraphPane.XAxis.MinorGrid.IsVisible = true;            
            graphControl.GraphPane.XAxis.MajorGrid.IsVisible = true;
            graphControl.GraphPane.YAxis.MinorGrid.IsVisible = true;
            graphControl.GraphPane.YAxis.MajorGrid.IsVisible = true;

            //Типы и цвет линии сеток 
            graphControl.GraphPane.XAxis.MinorGrid.DashOn = 1;
            graphControl.GraphPane.XAxis.MinorGrid.DashOff = 2;
            graphControl.GraphPane.XAxis.MinorGrid.Color = Color.Gray;

            graphControl.GraphPane.XAxis.MajorGrid.DashOn = 10;
            graphControl.GraphPane.XAxis.MajorGrid.DashOff = 5;
            graphControl.GraphPane.XAxis.MajorGrid.Color = Color.Gray;

            graphControl.GraphPane.YAxis.MinorGrid.DashOn = 1;
            graphControl.GraphPane.YAxis.MinorGrid.DashOff = 2;
            graphControl.GraphPane.YAxis.MinorGrid.Color = Color.Gray;

            graphControl.GraphPane.YAxis.MajorGrid.DashOn = 10;
            graphControl.GraphPane.YAxis.MajorGrid.DashOff = 5;
            graphControl.GraphPane.YAxis.MajorGrid.Color = Color.Gray;

            graphControl.AxisChange();
            graphControl.Invalidate();

            for (int i = 0; i < chart.Count; i++)
            {
                CheckBox checkBox = new CheckBox();
                //checkBox.Text = "graph " + endName + " " + i;
                checkBox.Text = chart.ElementAt(i).Label.Text;
                checkBox.Name = endName + "|" + i;
                checkBox.Location = new Point(15, 3 + i * 25);                
                checkBox.Checked = true;
                checkBox.CheckedChanged += changeGraphVisible;

                panel.Controls.Add(checkBox);
            }
            groupBox.Controls.Add(panel);

            tabPage.Controls.Add(graphControl);
            tabPage.Controls.Add(groupBox);
            tabControl1.Controls.Add(tabPage);            
        }

        #region "Обработчики нажатия кнопок меню"

        private void openFolderMenu_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                readFreezeframes(folderBrowserDialog1.SelectedPath, true);
            }
        }

        private void openFileMenu_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                readFreezeframes(folderBrowserDialog1.SelectedPath, false);
            }
        }

        #endregion

        #region Парсер файлов freezeframe.XX

        private string[] getErrorDescription(string filePath)
        {
            string[] errorDescription = new string[3];
            StreamReader reader = new StreamReader(filePath);
            try
            {
                string line = reader.ReadLine();
                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("\\s+", options);
                line = regex.Replace(line, "||");
                string[] detail = line.Split("||");

                for (int i = 0; i < detail.Length; i++)
                {
                    if (detail[i].Equals("Code"))
                        errorDescription[0] = detail[i + 1];

                    if (detail[i].Equals("Date"))
                        errorDescription[1] = detail[i + 1];

                    if (detail[i].Equals("Time"))
                        errorDescription[2] = detail[i + 1];
                }

                reader.Close();
                return errorDescription;
            }
            catch
            {
                MessageBox.Show("Ошибка чтения файла, возможно файл пустой");
                return null;
            }
        }
        private CurveList parseFile(string filePath)
        {
            CurveList curveList = new CurveList();

            StreamReader reader = new StreamReader(filePath);
            try
            {
                string[] curveNames;
                List<double[]> curveData = new List<double[]>();
                int curveCount;

                string line = reader.ReadLine();
                line = reader.ReadLine();
                RegexOptions options = RegexOptions.None;
                Regex regex = new Regex("\\s+", options);
                line = regex.Replace(line, "||");
                curveNames = line.Split("||");

                curveCount = curveNames.Length;

                for (int i = 0; i < curveCount; i++)
                {
                    curveData.Add(new double[50]);
                }

                int j = 0;

                line = reader.ReadLine();

                while (line != null)
                {
                    string[] tempString;
                    double[] tempDouble = new double[curveCount];
                    
                    line = regex.Replace(line, "||");
                    tempString = line.Split("||");

                    for (int i = 0; i < curveCount; i++)
                    {
                        try
                        {
                            tempDouble[i] = Double.Parse(tempString[i].Replace(".", ","));
                        }
                        catch
                        {

                        }
                    }

                    for (int i = 0; i < curveCount; i++)
                    {
                        curveData.ElementAt(i)[j] = tempDouble[i];
                    }

                    line = reader.ReadLine();
                    j++;
                }

                for (int i = 2; i < curveCount; i++)
                {

                    curveList.Add(new LineItem(curveNames[i], curveData.ElementAt(1), curveData.ElementAt(i), Color.Red, SymbolType.None));
                }

            }
            catch
            {
                MessageBox.Show("Ошибка чтения файла, возможно файл пустой");
                return curveList;
            }


            return curveList;
        }

        #endregion

        private void changeGraphVisible(object sender, EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;

            string graphName = "graph" + checkbox.Name.Split("|")[0];

            ZedGraphControl graph = (ZedGraphControl)tabControl1.SelectedTab.Controls[graphName];
            try 
            { 
                graph.GraphPane.CurveList.Find(x => x.Label.Text.Contains(checkbox.Text)).IsVisible = checkbox.Checked;
                graph.Invalidate();
            }
            catch { }

        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex != -1)
            {
                try
                {
                    string[] errorDescription = e.TabPage.Name.Split("|");
                    openedFileStatus.Text = "freezeframe." + errorDescription[0] + " Error code: " + errorDescription[1] + " Time: " + errorDescription[2] + " " + errorDescription[3].Replace(".", ":");
                }
                catch { }
                
                //MessageBox.Show("Hello");
            }            
        }

        private void graphZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {            
            GraphPane pane = sender.GraphPane;

            //MessageBox.Show(pane.XAxis.Scale.Min + "  " + pane.XAxis.Scale.Max + Environment.NewLine + pane.YAxis.Scale.Min + "  " + pane.YAxis.Scale.Max);

            if (pane.XAxis.Scale.Min <= -3.9)
            {
                pane.XAxis.Scale.Min = -3.9;
            }

            if (pane.XAxis.Scale.Max >= 1)
            {
                pane.XAxis.Scale.Max = 1;
            }

            if (pane.YAxis.Scale.Min <= -3500)
            {
                pane.YAxis.Scale.Min = -3500;
            }

            if (pane.YAxis.Scale.Max >= 3500)
            {
                pane.YAxis.Scale.Max = 3500;
            }

            double diff = pane.YAxis.Scale.Max - pane.YAxis.Scale.Min;

            if (diff > 1000)
            {
                pane.YAxis.Scale.MajorStep = 500;
                pane.YAxis.Scale.MinorStep = 100;
            }
            else if(diff < 1000 && diff > 100)
            {
                pane.YAxis.Scale.MajorStep = 100;
                pane.YAxis.Scale.MinorStep = 10;
            }
            else if (diff < 100)
            {
                pane.YAxis.Scale.MajorStep = 10;
                pane.YAxis.Scale.MinorStep = 1;
            }
            
        }

        private bool graphMouseMoveEvent(ZedGraphControl sender, System.Windows.Forms.MouseEventArgs e)
        {
            GraphPane pane = sender.GraphPane;
            if (e.Button == MouseButtons.Middle || (e.Button == MouseButtons.Left && (ModifierKeys & Keys.Control) == Keys.Control))
            {
                if (pane.XAxis.Scale.Min <= -3.9)
                {
                    pane.XAxis.Scale.Min = -3.9;
                }

                if (pane.XAxis.Scale.Max >= 1)
                {
                    pane.XAxis.Scale.Max = 1;
                }

                if (pane.YAxis.Scale.Min <= -3500)
                {
                    pane.YAxis.Scale.Min = -3500;
                }

                if (pane.YAxis.Scale.Max >= 3500)
                {
                    pane.YAxis.Scale.Max = 3500;
                }                
            }
            return false;
        }
    }
}
