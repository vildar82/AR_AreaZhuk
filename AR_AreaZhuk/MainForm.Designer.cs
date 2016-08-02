using System.Windows.Forms;
using Zuby.ADGV;
namespace AR_AreaZhuk
{

    public class DataGridViewEx : AdvancedDataGridView
    {
        public DataGridViewEx()
            : base()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }
    }
   
    partial class MainForm
    {



       
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dg = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnViewPercentsge = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnStartScan = new System.Windows.Forms.Button();
            this.lblCountObjects = new System.Windows.Forms.Label();
            this.chkDominant = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.chkListP1 = new System.Windows.Forms.CheckedListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.chkListP2 = new System.Windows.Forms.CheckedListBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.chkListP3 = new System.Windows.Forms.CheckedListBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.chkListP4 = new System.Windows.Forms.CheckedListBox();
            this.txtOffsetDominants = new System.Windows.Forms.NumericUpDown();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.chkEnableDominant = new System.Windows.Forms.CheckBox();
            this.numDomCountFloor = new System.Windows.Forms.NumericUpDown();
            this.numMainCountFloor = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlMenu = new System.Windows.Forms.Panel();
            this.pnlMenuGroup3 = new System.Windows.Forms.Panel();
            this.btnMenuGroup3 = new System.Windows.Forms.Button();
            this.pb = new System.Windows.Forms.PictureBox();
            this.contextMenuStripImage = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.сохранитьКакToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlMenuGroup2 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.btnMenuGroup2 = new System.Windows.Forms.Button();
            this.pnlMenuGroup1 = new System.Windows.Forms.Panel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnMenuGroup1 = new System.Windows.Forms.Button();
            this.GetFile = new System.Windows.Forms.Button();
            this.dg2 = new AR_AreaZhuk.DataGridViewEx();
            ((System.ComponentModel.ISupportInitialize)(this.dg)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtOffsetDominants)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDomCountFloor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMainCountFloor)).BeginInit();
            this.pnlMenu.SuspendLayout();
            this.pnlMenuGroup3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb)).BeginInit();
            this.contextMenuStripImage.SuspendLayout();
            this.pnlMenuGroup2.SuspendLayout();
            this.panel5.SuspendLayout();
            this.pnlMenuGroup1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dg2)).BeginInit();
            this.SuspendLayout();
            // 
            // dg
            // 
            this.dg.AllowUserToAddRows = false;
            this.dg.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dg.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dg.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column3,
            this.Column2,
            this.Column4});
            this.dg.Location = new System.Drawing.Point(1, 25);
            this.dg.Name = "dg";
            this.dg.RowHeadersVisible = false;
            this.dg.Size = new System.Drawing.Size(346, 289);
            this.dg.TabIndex = 0;
            this.dg.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dg_CellEndEdit);
            this.dg.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dg_CellValueChanged);
            this.dg.SelectionChanged += new System.EventHandler(this.dg_SelectionChanged);
            // 
            // Column1
            // 
            this.Column1.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            this.Column1.HeaderText = "Зона";
            this.Column1.Items.AddRange(new object[] {
            "Студия",
            "Однокомн.",
            "Двухкомн.",
            "Трехкомн.",
            "Четырехкомн."});
            this.Column1.Name = "Column1";
            this.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column1.Width = 80;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Площадь (м2.)";
            this.Column3.Name = "Column3";
            this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column3.Width = 90;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Кол-во (%)";
            this.Column2.Name = "Column2";
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column2.Width = 80;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Допуск (%)";
            this.Column4.Name = "Column4";
            this.Column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column4.Width = 80;
            // 
            // btnViewPercentsge
            // 
            this.btnViewPercentsge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnViewPercentsge.Enabled = false;
            this.btnViewPercentsge.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewPercentsge.Location = new System.Drawing.Point(348, 289);
            this.btnViewPercentsge.Name = "btnViewPercentsge";
            this.btnViewPercentsge.Size = new System.Drawing.Size(66, 23);
            this.btnViewPercentsge.TabIndex = 3;
            this.btnViewPercentsge.Text = "Показать";
            this.btnViewPercentsge.UseVisualStyleBackColor = true;
            this.btnViewPercentsge.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(879, 784);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Количество:";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(684, 806);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.dg2);
            this.groupBox2.Location = new System.Drawing.Point(416, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(612, 781);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Подробная информация";
            // 
            // btnStartScan
            // 
            this.btnStartScan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStartScan.Enabled = false;
            this.btnStartScan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartScan.Location = new System.Drawing.Point(422, 787);
            this.btnStartScan.Name = "btnStartScan";
            this.btnStartScan.Size = new System.Drawing.Size(75, 23);
            this.btnStartScan.TabIndex = 12;
            this.btnStartScan.Text = "Пуск";
            this.btnStartScan.UseVisualStyleBackColor = true;
            this.btnStartScan.Click += new System.EventHandler(this.btnStartScan_Click);
            // 
            // lblCountObjects
            // 
            this.lblCountObjects.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCountObjects.AutoSize = true;
            this.lblCountObjects.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblCountObjects.Location = new System.Drawing.Point(954, 784);
            this.lblCountObjects.Name = "lblCountObjects";
            this.lblCountObjects.Size = new System.Drawing.Size(16, 16);
            this.lblCountObjects.TabIndex = 14;
            this.lblCountObjects.Text = "0";
            // 
            // chkDominant
            // 
            this.chkDominant.AutoSize = true;
            this.chkDominant.Location = new System.Drawing.Point(267, 11);
            this.chkDominant.Name = "chkDominant";
            this.chkDominant.Size = new System.Drawing.Size(128, 30);
            this.chkDominant.TabIndex = 2;
            this.chkDominant.Text = "Разность шагов \r\nвысотных доминант";
            this.chkDominant.UseVisualStyleBackColor = true;
            this.chkDominant.CheckedChanged += new System.EventHandler(this.chkDominant_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.Controls.Add(this.groupBox1);
            this.groupBox3.Controls.Add(this.txtOffsetDominants);
            this.groupBox3.Controls.Add(this.chkDominant);
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Location = new System.Drawing.Point(3, 1);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(416, 237);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tabControl1);
            this.groupBox1.Location = new System.Drawing.Point(6, 82);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(348, 142);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Пятна объекта";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(6, 19);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(336, 117);
            this.tabControl1.TabIndex = 2;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.chkListP1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(328, 91);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "P1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // chkListP1
            // 
            this.chkListP1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.chkListP1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chkListP1.CheckOnClick = true;
            this.chkListP1.FormattingEnabled = true;
            this.chkListP1.Items.AddRange(new object[] {
            "Первая секция",
            "Вторая секция",
            "Третья секция",
            "Предпоследняя секция",
            "Последняя секция"});
            this.chkListP1.Location = new System.Drawing.Point(6, 7);
            this.chkListP1.Name = "chkListP1";
            this.chkListP1.Size = new System.Drawing.Size(316, 75);
            this.chkListP1.TabIndex = 2;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.chkListP2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(328, 91);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "P2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // chkListP2
            // 
            this.chkListP2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chkListP2.CheckOnClick = true;
            this.chkListP2.FormattingEnabled = true;
            this.chkListP2.Items.AddRange(new object[] {
            "Первая секция",
            "Вторая секция",
            "Третья секция",
            "Предпоследняя секция",
            "Последняя секция"});
            this.chkListP2.Location = new System.Drawing.Point(6, 7);
            this.chkListP2.Name = "chkListP2";
            this.chkListP2.Size = new System.Drawing.Size(316, 75);
            this.chkListP2.TabIndex = 3;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.chkListP3);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(328, 91);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "P3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // chkListP3
            // 
            this.chkListP3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chkListP3.CheckOnClick = true;
            this.chkListP3.FormattingEnabled = true;
            this.chkListP3.Items.AddRange(new object[] {
            "Первая секция",
            "Вторая секция",
            "Третья секция",
            "Предпоследняя секция",
            "Последняя секция"});
            this.chkListP3.Location = new System.Drawing.Point(6, 7);
            this.chkListP3.Name = "chkListP3";
            this.chkListP3.Size = new System.Drawing.Size(316, 75);
            this.chkListP3.TabIndex = 3;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.chkListP4);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(328, 91);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "P4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // chkListP4
            // 
            this.chkListP4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chkListP4.CheckOnClick = true;
            this.chkListP4.FormattingEnabled = true;
            this.chkListP4.Items.AddRange(new object[] {
            "Первая секция",
            "Вторая секция",
            "Третья секция",
            "Предпоследняя секция",
            "Последняя секция"});
            this.chkListP4.Location = new System.Drawing.Point(6, 7);
            this.chkListP4.Name = "chkListP4";
            this.chkListP4.Size = new System.Drawing.Size(316, 75);
            this.chkListP4.TabIndex = 3;
            // 
            // txtOffsetDominants
            // 
            this.txtOffsetDominants.Enabled = false;
            this.txtOffsetDominants.Location = new System.Drawing.Point(234, 17);
            this.txtOffsetDominants.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.txtOffsetDominants.Name = "txtOffsetDominants";
            this.txtOffsetDominants.Size = new System.Drawing.Size(31, 20);
            this.txtOffsetDominants.TabIndex = 2;
            this.txtOffsetDominants.ValueChanged += new System.EventHandler(this.txtOffsetDominants_ValueChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.chkEnableDominant);
            this.groupBox4.Controls.Add(this.numDomCountFloor);
            this.groupBox4.Controls.Add(this.numMainCountFloor);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Location = new System.Drawing.Point(6, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(192, 64);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Этажность";
            // 
            // chkEnableDominant
            // 
            this.chkEnableDominant.AutoSize = true;
            this.chkEnableDominant.Checked = true;
            this.chkEnableDominant.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEnableDominant.Location = new System.Drawing.Point(102, 12);
            this.chkEnableDominant.Name = "chkEnableDominant";
            this.chkEnableDominant.Size = new System.Drawing.Size(84, 17);
            this.chkEnableDominant.TabIndex = 4;
            this.chkEnableDominant.Text = "Доминанта";
            this.chkEnableDominant.UseVisualStyleBackColor = true;
            this.chkEnableDominant.CheckedChanged += new System.EventHandler(this.chkEnableDominant_CheckedChanged);
            // 
            // numDomCountFloor
            // 
            this.numDomCountFloor.Location = new System.Drawing.Point(114, 34);
            this.numDomCountFloor.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.numDomCountFloor.Name = "numDomCountFloor";
            this.numDomCountFloor.Size = new System.Drawing.Size(39, 20);
            this.numDomCountFloor.TabIndex = 16;
            this.numDomCountFloor.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // numMainCountFloor
            // 
            this.numMainCountFloor.Location = new System.Drawing.Point(18, 34);
            this.numMainCountFloor.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.numMainCountFloor.Name = "numMainCountFloor";
            this.numMainCountFloor.Size = new System.Drawing.Size(45, 20);
            this.numMainCountFloor.TabIndex = 15;
            this.numMainCountFloor.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Основная";
            // 
            // pnlMenu
            // 
            this.pnlMenu.Controls.Add(this.pnlMenuGroup3);
            this.pnlMenu.Controls.Add(this.pnlMenuGroup2);
            this.pnlMenu.Controls.Add(this.pnlMenuGroup1);
            this.pnlMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlMenu.Location = new System.Drawing.Point(0, 0);
            this.pnlMenu.Name = "pnlMenu";
            this.pnlMenu.Size = new System.Drawing.Size(416, 855);
            this.pnlMenu.TabIndex = 20;
            // 
            // pnlMenuGroup3
            // 
            this.pnlMenuGroup3.Controls.Add(this.btnMenuGroup3);
            this.pnlMenuGroup3.Controls.Add(this.pb);
            this.pnlMenuGroup3.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMenuGroup3.Location = new System.Drawing.Point(0, 582);
            this.pnlMenuGroup3.Name = "pnlMenuGroup3";
            this.pnlMenuGroup3.Size = new System.Drawing.Size(416, 273);
            this.pnlMenuGroup3.TabIndex = 33;
            // 
            // btnMenuGroup3
            // 
            this.btnMenuGroup3.BackColor = System.Drawing.Color.DimGray;
            this.btnMenuGroup3.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnMenuGroup3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMenuGroup3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnMenuGroup3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnMenuGroup3.Location = new System.Drawing.Point(0, 0);
            this.btnMenuGroup3.Name = "btnMenuGroup3";
            this.btnMenuGroup3.Size = new System.Drawing.Size(416, 25);
            this.btnMenuGroup3.TabIndex = 0;
            this.btnMenuGroup3.Text = "Эскиз";
            this.btnMenuGroup3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMenuGroup3.UseVisualStyleBackColor = false;
            this.btnMenuGroup3.Click += new System.EventHandler(this.btnMenuGroup3_Click);
            // 
            // pb
            // 
            this.pb.ContextMenuStrip = this.contextMenuStripImage;
            this.pb.Location = new System.Drawing.Point(50, 31);
            this.pb.Name = "pb";
            this.pb.Size = new System.Drawing.Size(307, 239);
            this.pb.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pb.TabIndex = 13;
            this.pb.TabStop = false;
            this.pb.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pb_MouseClick);
            this.pb.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pb_MouseDoubleClick);
            // 
            // contextMenuStripImage
            // 
            this.contextMenuStripImage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.сохранитьКакToolStripMenuItem});
            this.contextMenuStripImage.Name = "contextMenuStripImage";
            this.contextMenuStripImage.Size = new System.Drawing.Size(163, 26);
            // 
            // сохранитьКакToolStripMenuItem
            // 
            this.сохранитьКакToolStripMenuItem.Name = "сохранитьКакToolStripMenuItem";
            this.сохранитьКакToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.сохранитьКакToolStripMenuItem.Text = "Сохранить как...";
            this.сохранитьКакToolStripMenuItem.Click += new System.EventHandler(this.сохранитьКакToolStripMenuItem_Click);
            // 
            // pnlMenuGroup2
            // 
            this.pnlMenuGroup2.Controls.Add(this.panel5);
            this.pnlMenuGroup2.Controls.Add(this.btnMenuGroup2);
            this.pnlMenuGroup2.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMenuGroup2.Location = new System.Drawing.Point(0, 315);
            this.pnlMenuGroup2.Name = "pnlMenuGroup2";
            this.pnlMenuGroup2.Size = new System.Drawing.Size(416, 267);
            this.pnlMenuGroup2.TabIndex = 2;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.groupBox3);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel5.Location = new System.Drawing.Point(0, 25);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(422, 242);
            this.panel5.TabIndex = 14;
            // 
            // btnMenuGroup2
            // 
            this.btnMenuGroup2.BackColor = System.Drawing.Color.DimGray;
            this.btnMenuGroup2.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnMenuGroup2.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnMenuGroup2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMenuGroup2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnMenuGroup2.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnMenuGroup2.Location = new System.Drawing.Point(0, 0);
            this.btnMenuGroup2.Name = "btnMenuGroup2";
            this.btnMenuGroup2.Size = new System.Drawing.Size(416, 25);
            this.btnMenuGroup2.TabIndex = 0;
            this.btnMenuGroup2.Text = "Условия";
            this.btnMenuGroup2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMenuGroup2.UseVisualStyleBackColor = false;
            this.btnMenuGroup2.Click += new System.EventHandler(this.btnMenuGroup2_Click);
            // 
            // pnlMenuGroup1
            // 
            this.pnlMenuGroup1.Controls.Add(this.btnAdd);
            this.pnlMenuGroup1.Controls.Add(this.btnRemove);
            this.pnlMenuGroup1.Controls.Add(this.dg);
            this.pnlMenuGroup1.Controls.Add(this.btnMenuGroup1);
            this.pnlMenuGroup1.Controls.Add(this.btnViewPercentsge);
            this.pnlMenuGroup1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMenuGroup1.Location = new System.Drawing.Point(0, 0);
            this.pnlMenuGroup1.Name = "pnlMenuGroup1";
            this.pnlMenuGroup1.Size = new System.Drawing.Size(416, 315);
            this.pnlMenuGroup1.TabIndex = 1;
            // 
            // btnAdd
            // 
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(27, 290);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(25, 23);
            this.btnAdd.TabIndex = 5;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Location = new System.Drawing.Point(3, 290);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(25, 23);
            this.btnRemove.TabIndex = 4;
            this.btnRemove.Text = "-";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnMenuGroup1
            // 
            this.btnMenuGroup1.BackColor = System.Drawing.Color.DimGray;
            this.btnMenuGroup1.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnMenuGroup1.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnMenuGroup1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMenuGroup1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnMenuGroup1.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnMenuGroup1.Location = new System.Drawing.Point(0, 0);
            this.btnMenuGroup1.Name = "btnMenuGroup1";
            this.btnMenuGroup1.Size = new System.Drawing.Size(416, 25);
            this.btnMenuGroup1.TabIndex = 0;
            this.btnMenuGroup1.Text = "Квартирография";
            this.btnMenuGroup1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMenuGroup1.UseVisualStyleBackColor = false;
            this.btnMenuGroup1.Click += new System.EventHandler(this.btnMenuGroup1_Click_1);
            // 
            // GetFile
            // 
            this.GetFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.GetFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.GetFile.Location = new System.Drawing.Point(529, 787);
            this.GetFile.Name = "GetFile";
            this.GetFile.Size = new System.Drawing.Size(106, 23);
            this.GetFile.TabIndex = 22;
            this.GetFile.Text = "Файл инсоляции";
            this.GetFile.UseVisualStyleBackColor = true;
            this.GetFile.Click += new System.EventHandler(this.GetFile_Click);
            // 
            // dg2
            // 
            this.dg2.AllowUserToAddRows = false;
            this.dg2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dg2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dg2.FilterAndSortEnabled = true;
            this.dg2.Location = new System.Drawing.Point(6, 19);
            this.dg2.MultiSelect = false;
            this.dg2.Name = "dg2";
            this.dg2.ReadOnly = true;
            this.dg2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dg2.Size = new System.Drawing.Size(598, 756);
            this.dg2.TabIndex = 1;
            this.dg2.SortStringChanged += new System.EventHandler(this.dg2_SortStringChanged);
            this.dg2.FilterStringChanged += new System.EventHandler(this.dg2_FilterStringChanged);
            this.dg2.SelectionChanged += new System.EventHandler(this.dg2_SelectionChanged);
            this.dg2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dg2_MouseClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1032, 855);
            this.Controls.Add(this.GetFile);
            this.Controls.Add(this.pnlMenu);
            this.Controls.Add(this.lblCountObjects);
            this.Controls.Add(this.btnStartScan);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label3);
            this.MinimumSize = new System.Drawing.Size(978, 514);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "\"Жучки\". В разработке. ";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dg)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtOffsetDominants)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDomCountFloor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMainCountFloor)).EndInit();
            this.pnlMenu.ResumeLayout(false);
            this.pnlMenuGroup3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pb)).EndInit();
            this.contextMenuStripImage.ResumeLayout(false);
            this.pnlMenuGroup2.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.pnlMenuGroup1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dg2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dg;
        private System.Windows.Forms.Button btnViewPercentsge;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnStartScan;
        private Label lblCountObjects;
        private DataGridViewEx dg2;
        private CheckBox chkDominant;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private Label label1;
        private NumericUpDown txtOffsetDominants;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private TabPage tabPage4;
        private NumericUpDown numDomCountFloor;
        private NumericUpDown numMainCountFloor;
        private CheckedListBox chkListP1;
        private CheckedListBox chkListP2;
        private CheckedListBox chkListP3;
        private CheckedListBox chkListP4;
        private Panel pnlMenu;
        private Panel pnlMenuGroup2;
        private Panel panel5;
        private Button btnMenuGroup2;
        private Panel pnlMenuGroup1;
        private Button btnMenuGroup1;
        private Panel pnlMenuGroup3;
        private Button btnMenuGroup3;
        private PictureBox pb;
        private GroupBox groupBox1;
        private ContextMenuStrip contextMenuStripImage;
        private ToolStripMenuItem сохранитьКакToolStripMenuItem;
        private CheckBox chkEnableDominant;
        private Button GetFile;
        private Button btnAdd;
        private Button btnRemove;
        private DataGridViewComboBoxColumn Column1;
        private DataGridViewTextBoxColumn Column3;
        private DataGridViewTextBoxColumn Column2;
        private DataGridViewTextBoxColumn Column4;
    }
}

