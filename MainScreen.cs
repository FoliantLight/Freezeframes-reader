using System;
using System.Collections;
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
            }
            else
            {

            }

            if (tempOpenedFileCount == 1)
            {
                openedFileStatus.Text = "Найден 1 файл";
            }
            else
            {
                if (tempOpenedFileCount >= 2 && tempOpenedFileCount <= 4)
                {
                    openedFileStatus.Text = "Найдено " + tempOpenedFileCount + " файла";
                }
                else
                    openedFileStatus.Text = "Найдено " + tempOpenedFileCount + " файлов";
            }


        }

        private void addTab(string endName, string[] errorDescription, CurveList chart)
        {
            TabPage tabPage = new TabPage();
            GroupBox groupBox = new GroupBox();
            ZedGraphControl graphControl = new ZedGraphControl();

            tabPage.Name = endName;
            tabPage.Text = errorDescription[0] + " " + errorDescription[1];

            groupBox.Dock = DockStyle.Right;
            groupBox.Location = new Point(803, 3);
            groupBox.Name = "graph." + endName;
            groupBox.Width = 170;
            groupBox.TabIndex = 0;
            groupBox.TabStop = false;
            groupBox.Text = "Графики";

            graphControl.Dock = DockStyle.Fill;
            graphControl.Location = new Point(3, 3);
            graphControl.Margin = new Padding(3, 3, 4, 3);
            graphControl.Name = "graph " + endName;
            graphControl.ScrollGrace = 0D;
            graphControl.ScrollMaxX = 4D;
            graphControl.ScrollMaxY = 0D;
            graphControl.ScrollMaxY2 = 0D;
            graphControl.ScrollMinX = 0D;
            graphControl.ScrollMinY = 0D;
            graphControl.ScrollMinY2 = 0D;
            graphControl.Size = new Size(800, 581);
            graphControl.TabIndex = 1;
            graphControl.GraphPane.Title.Text = "";
            graphControl.GraphPane.Legend.IsVisible = false;
            graphControl.GraphPane.XAxis.Title.Text = "";
            graphControl.GraphPane.YAxis.Title.Text = "";

            graphControl.GraphPane.CurveList = chart;
            graphControl.GraphPane.AxisChange();



            for (int i = 0; i < 16; i++)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Text = "graph " + endName + " " + i;
                checkBox.Name = endName + "|" + i;
                checkBox.Location = new Point(15, 22 + i * 25);
                checkBox.CheckedChanged += changeGraphVisible;

                groupBox.Controls.Add(checkBox);
            }

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
                    curveList.Add(new LineItem(curveNames[i], curveData.ElementAt(1), curveData.ElementAt(i), Color.Blue, SymbolType.None));
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
            //MessageBox.Show(checkbox.Name + " " + (checkbox.Checked ? "true" : "false"));
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex != -1)
            {
                //MessageBox.Show("Hello");
            }            
        }
    }
}
