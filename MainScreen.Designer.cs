namespace Freezeframes_reader
{
    partial class MainScreen
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            openFolderMenu = new ToolStripMenuItem();
            openFileMenu = new ToolStripMenuItem();
            clearAllMenu = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            openedFolder = new ToolStripStatusLabel();
            openedFileStatus = new ToolStripStatusLabel();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            label1 = new Label();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { openFolderMenu, openFileMenu, clearAllMenu });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(984, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // openFolderMenu
            // 
            openFolderMenu.Image = Properties.Resources.folder;
            openFolderMenu.Name = "openFolderMenu";
            openFolderMenu.Size = new Size(117, 20);
            openFolderMenu.Text = "Открыть папку";
            openFolderMenu.Click += openFolderMenu_Click;
            // 
            // openFileMenu
            // 
            openFileMenu.Enabled = false;
            openFileMenu.Image = Properties.Resources.file;
            openFileMenu.Name = "openFileMenu";
            openFileMenu.Size = new Size(114, 20);
            openFileMenu.Text = "Открыть файл";
            openFileMenu.Click += openFileMenu_Click;
            // 
            // clearAllMenu
            // 
            clearAllMenu.Enabled = false;
            clearAllMenu.Image = Properties.Resources.wrench;
            clearAllMenu.Name = "clearAllMenu";
            clearAllMenu.Size = new Size(134, 20);
            clearAllMenu.Text = "Очистить область";
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { openedFolder, openedFileStatus });
            statusStrip1.Location = new Point(0, 639);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(984, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // openedFolder
            // 
            openedFolder.Name = "openedFolder";
            openedFolder.Size = new Size(108, 17);
            openedFolder.Text = "Папка не выбрана";
            // 
            // openedFileStatus
            // 
            openedFileStatus.Name = "openedFileStatus";
            openedFileStatus.RightToLeft = RightToLeft.No;
            openedFileStatus.Size = new Size(861, 17);
            openedFileStatus.Spring = true;
            openedFileStatus.TextAlign = ContentAlignment.MiddleRight;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 24);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(984, 615);
            tabControl1.TabIndex = 3;
            tabControl1.Selecting += tabControl1_Selecting;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(label1);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(976, 587);
            tabPage1.TabIndex = 2;
            tabPage1.Text = "Нет данных";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(8, 3);
            label1.Name = "label1";
            label1.Size = new Size(580, 50);
            label1.TabIndex = 0;
            label1.Text = "Выберите папку для чтения файлов.\r\nПрограмма ищет и читает файлы freezeframe.00 ... freezeframe.15";
            // 
            // MainScreen
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 661);
            Controls.Add(tabControl1);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            MinimumSize = new Size(1000, 700);
            Name = "MainScreen";
            ShowIcon = false;
            Text = "SKAT-02 Freezeframe reader";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem openFolderMenu;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel openedFolder;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private ToolStripStatusLabel openedFileStatus;
        private ToolStripMenuItem openFileMenu;
        private ToolStripMenuItem clearAllMenu;
        private Label label1;
    }
}