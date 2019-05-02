﻿namespace JHEvaluation.Rank
{
    partial class CalculateSemesterAssessmentRank
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.plBasicInfSelect = new DevComponents.DotNetBar.PanelEx();
            this.btnNext = new DevComponents.DotNetBar.ButtonX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.cboSchoolYear = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.gpRankPeople = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.listGradeYear = new DevComponents.DotNetBar.Controls.ListViewEx();
            this.cboStudentTag2 = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX5 = new DevComponents.DotNetBar.LabelX();
            this.cboStudentTag1 = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.cboStudentFilter = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.cboSemester = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.plStudentView = new DevComponents.DotNetBar.PanelEx();
            this.btnPrevious = new DevComponents.DotNetBar.ButtonX();
            this.dgvStudentList = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.colClass = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSeatNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStudentNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStudentName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSchoolRank = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colClassRank = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRankType1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRankType2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnCacluate = new DevComponents.DotNetBar.ButtonX();
            this.btnImport = new DevComponents.DotNetBar.ButtonX();
            this.btnExport = new DevComponents.DotNetBar.ButtonX();
            this.labelX13 = new DevComponents.DotNetBar.LabelX();
            this.lbSemester = new DevComponents.DotNetBar.LabelX();
            this.lbSchoolYear = new DevComponents.DotNetBar.LabelX();
            this.labelX8 = new DevComponents.DotNetBar.LabelX();
            this.labelX9 = new DevComponents.DotNetBar.LabelX();
            this.plBasicInfSelect.SuspendLayout();
            this.gpRankPeople.SuspendLayout();
            this.plStudentView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStudentList)).BeginInit();
            this.SuspendLayout();
            // 
            // plBasicInfSelect
            // 
            this.plBasicInfSelect.CanvasColor = System.Drawing.SystemColors.Control;
            this.plBasicInfSelect.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.plBasicInfSelect.Controls.Add(this.btnNext);
            this.plBasicInfSelect.Controls.Add(this.labelX1);
            this.plBasicInfSelect.Controls.Add(this.cboSchoolYear);
            this.plBasicInfSelect.Controls.Add(this.gpRankPeople);
            this.plBasicInfSelect.Controls.Add(this.labelX2);
            this.plBasicInfSelect.Controls.Add(this.cboSemester);
            this.plBasicInfSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plBasicInfSelect.Location = new System.Drawing.Point(0, 0);
            this.plBasicInfSelect.Name = "plBasicInfSelect";
            this.plBasicInfSelect.Size = new System.Drawing.Size(794, 471);
            this.plBasicInfSelect.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.plBasicInfSelect.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.plBasicInfSelect.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.plBasicInfSelect.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.plBasicInfSelect.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.plBasicInfSelect.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.plBasicInfSelect.Style.GradientAngle = 90;
            this.plBasicInfSelect.TabIndex = 0;
            // 
            // btnNext
            // 
            this.btnNext.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.BackColor = System.Drawing.Color.Transparent;
            this.btnNext.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnNext.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnNext.ForeColor = System.Drawing.Color.Black;
            this.btnNext.Location = new System.Drawing.Point(707, 434);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 25);
            this.btnNext.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnNext.TabIndex = 15;
            this.btnNext.Text = "下一步";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // labelX1
            // 
            this.labelX1.AutoSize = true;
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelX1.ForeColor = System.Drawing.Color.Black;
            this.labelX1.Location = new System.Drawing.Point(12, 12);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(62, 22);
            this.labelX1.TabIndex = 8;
            this.labelX1.Text = "學年度：";
            // 
            // cboSchoolYear
            // 
            this.cboSchoolYear.DisplayMember = "Text";
            this.cboSchoolYear.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSchoolYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSchoolYear.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cboSchoolYear.FormattingEnabled = true;
            this.cboSchoolYear.ItemHeight = 21;
            this.cboSchoolYear.Location = new System.Drawing.Point(80, 10);
            this.cboSchoolYear.Name = "cboSchoolYear";
            this.cboSchoolYear.Size = new System.Drawing.Size(88, 27);
            this.cboSchoolYear.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cboSchoolYear.TabIndex = 9;
            // 
            // gpRankPeople
            // 
            this.gpRankPeople.BackColor = System.Drawing.Color.Transparent;
            this.gpRankPeople.CanvasColor = System.Drawing.SystemColors.Control;
            this.gpRankPeople.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.gpRankPeople.Controls.Add(this.listGradeYear);
            this.gpRankPeople.Controls.Add(this.cboStudentTag2);
            this.gpRankPeople.Controls.Add(this.labelX5);
            this.gpRankPeople.Controls.Add(this.cboStudentTag1);
            this.gpRankPeople.Controls.Add(this.labelX4);
            this.gpRankPeople.Controls.Add(this.cboStudentFilter);
            this.gpRankPeople.Controls.Add(this.labelX3);
            this.gpRankPeople.DrawTitleBox = false;
            this.gpRankPeople.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.gpRankPeople.Location = new System.Drawing.Point(12, 53);
            this.gpRankPeople.Name = "gpRankPeople";
            this.gpRankPeople.Size = new System.Drawing.Size(770, 375);
            // 
            // 
            // 
            this.gpRankPeople.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.gpRankPeople.Style.BackColorGradientAngle = 90;
            this.gpRankPeople.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.gpRankPeople.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpRankPeople.Style.BorderBottomWidth = 1;
            this.gpRankPeople.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.gpRankPeople.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpRankPeople.Style.BorderLeftWidth = 1;
            this.gpRankPeople.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpRankPeople.Style.BorderRightWidth = 1;
            this.gpRankPeople.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpRankPeople.Style.BorderTopWidth = 1;
            this.gpRankPeople.Style.Class = "";
            this.gpRankPeople.Style.CornerDiameter = 4;
            this.gpRankPeople.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.gpRankPeople.Style.TextColor = System.Drawing.Color.Black;
            this.gpRankPeople.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.gpRankPeople.StyleMouseDown.Class = "";
            this.gpRankPeople.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.gpRankPeople.StyleMouseOver.Class = "";
            this.gpRankPeople.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.gpRankPeople.TabIndex = 14;
            this.gpRankPeople.Text = "排名對象";
            // 
            // listGradeYear
            // 
            this.listGradeYear.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.listGradeYear.Border.Class = "ListViewBorder";
            this.listGradeYear.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.listGradeYear.CheckBoxes = true;
            this.listGradeYear.Location = new System.Drawing.Point(13, 137);
            this.listGradeYear.Name = "listGradeYear";
            this.listGradeYear.Size = new System.Drawing.Size(745, 226);
            this.listGradeYear.TabIndex = 7;
            this.listGradeYear.UseCompatibleStateImageBehavior = false;
            this.listGradeYear.View = System.Windows.Forms.View.List;
            // 
            // cboStudentTag2
            // 
            this.cboStudentTag2.DisplayMember = "Text";
            this.cboStudentTag2.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboStudentTag2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStudentTag2.Enabled = false;
            this.cboStudentTag2.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cboStudentTag2.FormattingEnabled = true;
            this.cboStudentTag2.ItemHeight = 21;
            this.cboStudentTag2.Location = new System.Drawing.Point(111, 93);
            this.cboStudentTag2.Name = "cboStudentTag2";
            this.cboStudentTag2.Size = new System.Drawing.Size(242, 27);
            this.cboStudentTag2.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cboStudentTag2.TabIndex = 6;
            this.cboStudentTag2.SelectedIndexChanged += new System.EventHandler(this.cboStudentTag2_SelectedIndexChanged);
            // 
            // labelX5
            // 
            this.labelX5.AutoSize = true;
            this.labelX5.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX5.BackgroundStyle.Class = "";
            this.labelX5.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX5.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelX5.ForeColor = System.Drawing.Color.Black;
            this.labelX5.Location = new System.Drawing.Point(13, 95);
            this.labelX5.Name = "labelX5";
            this.labelX5.Size = new System.Drawing.Size(92, 22);
            this.labelX5.TabIndex = 5;
            this.labelX5.Text = "類別排名二：";
            // 
            // cboStudentTag1
            // 
            this.cboStudentTag1.DisplayMember = "Text";
            this.cboStudentTag1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboStudentTag1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStudentTag1.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cboStudentTag1.FormattingEnabled = true;
            this.cboStudentTag1.ItemHeight = 21;
            this.cboStudentTag1.Location = new System.Drawing.Point(111, 52);
            this.cboStudentTag1.Name = "cboStudentTag1";
            this.cboStudentTag1.Size = new System.Drawing.Size(242, 27);
            this.cboStudentTag1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cboStudentTag1.TabIndex = 4;
            this.cboStudentTag1.SelectedIndexChanged += new System.EventHandler(this.cboStudentTag1_SelectedIndexChanged);
            // 
            // labelX4
            // 
            this.labelX4.AutoSize = true;
            this.labelX4.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX4.BackgroundStyle.Class = "";
            this.labelX4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX4.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelX4.ForeColor = System.Drawing.Color.Black;
            this.labelX4.Location = new System.Drawing.Point(13, 54);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(92, 22);
            this.labelX4.TabIndex = 3;
            this.labelX4.Text = "類別排名一：";
            // 
            // cboStudentFilter
            // 
            this.cboStudentFilter.DisplayMember = "Text";
            this.cboStudentFilter.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboStudentFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStudentFilter.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cboStudentFilter.FormattingEnabled = true;
            this.cboStudentFilter.ItemHeight = 21;
            this.cboStudentFilter.Location = new System.Drawing.Point(141, 10);
            this.cboStudentFilter.Name = "cboStudentFilter";
            this.cboStudentFilter.Size = new System.Drawing.Size(242, 27);
            this.cboStudentFilter.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cboStudentFilter.TabIndex = 2;
            this.cboStudentFilter.SelectedIndexChanged += new System.EventHandler(this.cboStudentFilter_SelectedIndexChanged);
            // 
            // labelX3
            // 
            this.labelX3.AutoSize = true;
            this.labelX3.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX3.BackgroundStyle.Class = "";
            this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX3.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelX3.ForeColor = System.Drawing.Color.Black;
            this.labelX3.Location = new System.Drawing.Point(13, 12);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(122, 22);
            this.labelX3.TabIndex = 1;
            this.labelX3.Text = "不排名學生類別：";
            // 
            // labelX2
            // 
            this.labelX2.AutoSize = true;
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelX2.ForeColor = System.Drawing.Color.Black;
            this.labelX2.Location = new System.Drawing.Point(188, 12);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(47, 22);
            this.labelX2.TabIndex = 10;
            this.labelX2.Text = "學期：";
            // 
            // cboSemester
            // 
            this.cboSemester.DisplayMember = "Text";
            this.cboSemester.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSemester.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSemester.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cboSemester.FormattingEnabled = true;
            this.cboSemester.ItemHeight = 21;
            this.cboSemester.Location = new System.Drawing.Point(241, 10);
            this.cboSemester.Name = "cboSemester";
            this.cboSemester.Size = new System.Drawing.Size(82, 27);
            this.cboSemester.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cboSemester.TabIndex = 11;
            // 
            // plStudentView
            // 
            this.plStudentView.CanvasColor = System.Drawing.SystemColors.Control;
            this.plStudentView.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.plStudentView.Controls.Add(this.btnPrevious);
            this.plStudentView.Controls.Add(this.dgvStudentList);
            this.plStudentView.Controls.Add(this.btnCacluate);
            this.plStudentView.Controls.Add(this.btnImport);
            this.plStudentView.Controls.Add(this.btnExport);
            this.plStudentView.Controls.Add(this.labelX13);
            this.plStudentView.Controls.Add(this.lbSemester);
            this.plStudentView.Controls.Add(this.lbSchoolYear);
            this.plStudentView.Controls.Add(this.labelX8);
            this.plStudentView.Controls.Add(this.labelX9);
            this.plStudentView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plStudentView.Location = new System.Drawing.Point(0, 0);
            this.plStudentView.Name = "plStudentView";
            this.plStudentView.Size = new System.Drawing.Size(794, 471);
            this.plStudentView.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.plStudentView.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.plStudentView.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.plStudentView.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.plStudentView.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.plStudentView.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.plStudentView.Style.GradientAngle = 90;
            this.plStudentView.TabIndex = 0;
            // 
            // btnPrevious
            // 
            this.btnPrevious.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrevious.BackColor = System.Drawing.Color.Transparent;
            this.btnPrevious.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnPrevious.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnPrevious.ForeColor = System.Drawing.Color.Black;
            this.btnPrevious.Location = new System.Drawing.Point(610, 434);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(75, 25);
            this.btnPrevious.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnPrevious.TabIndex = 37;
            this.btnPrevious.Text = "上一步";
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // dgvStudentList
            // 
            this.dgvStudentList.AllowUserToAddRows = false;
            this.dgvStudentList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvStudentList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStudentList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colClass,
            this.colSeatNum,
            this.colStudentNum,
            this.colStudentName,
            this.colSchoolRank,
            this.colClassRank,
            this.colRankType1,
            this.colRankType2});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft JhengHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvStudentList.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvStudentList.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dgvStudentList.HighlightSelectedColumnHeaders = false;
            this.dgvStudentList.Location = new System.Drawing.Point(3, 68);
            this.dgvStudentList.Name = "dgvStudentList";
            this.dgvStudentList.ReadOnly = true;
            this.dgvStudentList.RowTemplate.Height = 24;
            this.dgvStudentList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvStudentList.Size = new System.Drawing.Size(788, 360);
            this.dgvStudentList.TabIndex = 36;
            // 
            // colClass
            // 
            this.colClass.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colClass.DataPropertyName = "ClassName";
            this.colClass.HeaderText = "班級";
            this.colClass.MinimumWidth = 59;
            this.colClass.Name = "colClass";
            this.colClass.ReadOnly = true;
            this.colClass.Width = 59;
            // 
            // colSeatNum
            // 
            this.colSeatNum.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colSeatNum.DataPropertyName = "SeatNo";
            this.colSeatNum.HeaderText = "座號";
            this.colSeatNum.MinimumWidth = 59;
            this.colSeatNum.Name = "colSeatNum";
            this.colSeatNum.ReadOnly = true;
            this.colSeatNum.Width = 59;
            // 
            // colStudentNum
            // 
            this.colStudentNum.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colStudentNum.DataPropertyName = "StudentNumber";
            this.colStudentNum.HeaderText = "學號";
            this.colStudentNum.MinimumWidth = 59;
            this.colStudentNum.Name = "colStudentNum";
            this.colStudentNum.ReadOnly = true;
            this.colStudentNum.Width = 59;
            // 
            // colStudentName
            // 
            this.colStudentName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colStudentName.DataPropertyName = "Name";
            this.colStudentName.HeaderText = "姓名";
            this.colStudentName.MinimumWidth = 59;
            this.colStudentName.Name = "colStudentName";
            this.colStudentName.ReadOnly = true;
            this.colStudentName.Width = 59;
            // 
            // colSchoolRank
            // 
            this.colSchoolRank.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colSchoolRank.DataPropertyName = "RankGradeYear";
            this.colSchoolRank.HeaderText = "母群：年排名";
            this.colSchoolRank.MinimumWidth = 111;
            this.colSchoolRank.Name = "colSchoolRank";
            this.colSchoolRank.ReadOnly = true;
            this.colSchoolRank.Width = 111;
            // 
            // colClassRank
            // 
            this.colClassRank.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colClassRank.DataPropertyName = "RankClassName";
            this.colClassRank.HeaderText = "母群：班排名";
            this.colClassRank.MinimumWidth = 111;
            this.colClassRank.Name = "colClassRank";
            this.colClassRank.ReadOnly = true;
            this.colClassRank.Width = 111;
            // 
            // colRankType1
            // 
            this.colRankType1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colRankType1.DataPropertyName = "StudentTag1";
            this.colRankType1.HeaderText = "母群：類別一";
            this.colRankType1.MinimumWidth = 111;
            this.colRankType1.Name = "colRankType1";
            this.colRankType1.ReadOnly = true;
            // 
            // colRankType2
            // 
            this.colRankType2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colRankType2.DataPropertyName = "StudentTag2";
            this.colRankType2.HeaderText = "母群：類別二";
            this.colRankType2.MinimumWidth = 111;
            this.colRankType2.Name = "colRankType2";
            this.colRankType2.ReadOnly = true;
            // 
            // btnCacluate
            // 
            this.btnCacluate.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnCacluate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCacluate.BackColor = System.Drawing.Color.Transparent;
            this.btnCacluate.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnCacluate.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnCacluate.ForeColor = System.Drawing.Color.Black;
            this.btnCacluate.Location = new System.Drawing.Point(701, 434);
            this.btnCacluate.Name = "btnCacluate";
            this.btnCacluate.Size = new System.Drawing.Size(81, 25);
            this.btnCacluate.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnCacluate.TabIndex = 35;
            this.btnCacluate.Text = "計算排名";
            this.btnCacluate.Click += new System.EventHandler(this.btnCacluate_Click);
            // 
            // btnImport
            // 
            this.btnImport.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImport.BackColor = System.Drawing.Color.Transparent;
            this.btnImport.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnImport.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnImport.ForeColor = System.Drawing.Color.Black;
            this.btnImport.Location = new System.Drawing.Point(519, 434);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 25);
            this.btnImport.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnImport.TabIndex = 34;
            this.btnImport.Text = "匯入";
            this.btnImport.Visible = false;
            // 
            // btnExport
            // 
            this.btnExport.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.BackColor = System.Drawing.Color.Transparent;
            this.btnExport.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExport.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.btnExport.ForeColor = System.Drawing.Color.Black;
            this.btnExport.Location = new System.Drawing.Point(428, 434);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 25);
            this.btnExport.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnExport.TabIndex = 33;
            this.btnExport.Text = "匯出";
            this.btnExport.Visible = false;
            // 
            // labelX13
            // 
            this.labelX13.AutoSize = true;
            this.labelX13.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX13.BackgroundStyle.Class = "";
            this.labelX13.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX13.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelX13.ForeColor = System.Drawing.Color.Black;
            this.labelX13.Location = new System.Drawing.Point(12, 40);
            this.labelX13.Name = "labelX13";
            this.labelX13.Size = new System.Drawing.Size(77, 22);
            this.labelX13.TabIndex = 32;
            this.labelX13.Text = "母群資料：";
            // 
            // lbSemester
            // 
            this.lbSemester.AutoSize = true;
            this.lbSemester.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lbSemester.BackgroundStyle.Class = "";
            this.lbSemester.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lbSemester.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbSemester.ForeColor = System.Drawing.Color.Black;
            this.lbSemester.Location = new System.Drawing.Point(199, 12);
            this.lbSemester.Name = "lbSemester";
            this.lbSemester.Size = new System.Drawing.Size(32, 22);
            this.lbSemester.TabIndex = 30;
            this.lbSemester.Text = "學期";
            // 
            // lbSchoolYear
            // 
            this.lbSchoolYear.AutoSize = true;
            this.lbSchoolYear.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lbSchoolYear.BackgroundStyle.Class = "";
            this.lbSchoolYear.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lbSchoolYear.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbSchoolYear.ForeColor = System.Drawing.Color.Black;
            this.lbSchoolYear.Location = new System.Drawing.Point(80, 12);
            this.lbSchoolYear.Name = "lbSchoolYear";
            this.lbSchoolYear.Size = new System.Drawing.Size(47, 22);
            this.lbSchoolYear.TabIndex = 29;
            this.lbSchoolYear.Text = "學年度";
            // 
            // labelX8
            // 
            this.labelX8.AutoSize = true;
            this.labelX8.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX8.BackgroundStyle.Class = "";
            this.labelX8.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX8.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelX8.ForeColor = System.Drawing.Color.Black;
            this.labelX8.Location = new System.Drawing.Point(146, 12);
            this.labelX8.Name = "labelX8";
            this.labelX8.Size = new System.Drawing.Size(47, 22);
            this.labelX8.TabIndex = 27;
            this.labelX8.Text = "學期：";
            // 
            // labelX9
            // 
            this.labelX9.AutoSize = true;
            this.labelX9.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX9.BackgroundStyle.Class = "";
            this.labelX9.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX9.Font = new System.Drawing.Font("Microsoft JhengHei", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelX9.ForeColor = System.Drawing.Color.Black;
            this.labelX9.Location = new System.Drawing.Point(12, 12);
            this.labelX9.Name = "labelX9";
            this.labelX9.Size = new System.Drawing.Size(62, 22);
            this.labelX9.TabIndex = 26;
            this.labelX9.Text = "學年度：";
            // 
            // CalculateSemesterAssessmentRank
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 471);
            this.Controls.Add(this.plStudentView);
            this.Controls.Add(this.plBasicInfSelect);
            this.DoubleBuffered = true;
            this.Name = "CalculateSemesterAssessmentRank";
            this.Text = "計算學期成績固定排名";
            this.Load += new System.EventHandler(this.CalculateSemesterAssessmentRank_Load);
            this.plBasicInfSelect.ResumeLayout(false);
            this.plBasicInfSelect.PerformLayout();
            this.gpRankPeople.ResumeLayout(false);
            this.gpRankPeople.PerformLayout();
            this.plStudentView.ResumeLayout(false);
            this.plStudentView.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStudentList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.PanelEx plBasicInfSelect;
        private DevComponents.DotNetBar.PanelEx plStudentView;
        private DevComponents.DotNetBar.Controls.DataGridViewX dgvStudentList;
        private System.Windows.Forms.DataGridViewTextBoxColumn colClass;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSeatNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStudentNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStudentName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSchoolRank;
        private System.Windows.Forms.DataGridViewTextBoxColumn colClassRank;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRankType1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRankType2;
        private DevComponents.DotNetBar.ButtonX btnCacluate;
        private DevComponents.DotNetBar.ButtonX btnImport;
        private DevComponents.DotNetBar.ButtonX btnExport;
        private DevComponents.DotNetBar.LabelX labelX13;
        private DevComponents.DotNetBar.LabelX lbSemester;
        private DevComponents.DotNetBar.LabelX lbSchoolYear;
        private DevComponents.DotNetBar.LabelX labelX8;
        private DevComponents.DotNetBar.LabelX labelX9;
        private DevComponents.DotNetBar.ButtonX btnNext;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSchoolYear;
        private DevComponents.DotNetBar.Controls.GroupPanel gpRankPeople;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboStudentTag2;
        private DevComponents.DotNetBar.LabelX labelX5;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboStudentTag1;
        private DevComponents.DotNetBar.LabelX labelX4;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboStudentFilter;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSemester;
        private DevComponents.DotNetBar.Controls.ListViewEx listGradeYear;
        private DevComponents.DotNetBar.ButtonX btnPrevious;
    }
}