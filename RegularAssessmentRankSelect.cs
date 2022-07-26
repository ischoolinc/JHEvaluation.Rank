using Aspose.Cells;
using FISCA.Data;
using FISCA.Presentation.Controls;
using JHEvaluation.Rank.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JHEvaluation.Rank
{
    public partial class RegularAssessmentRankSelect : BaseForm
    {
        private bool _IsClosing = false;
        private bool _IsLoading = false;
        private List<DataGridViewRow> _RowCollection = new List<DataGridViewRow>();
        private string _LoadSchoolYear = "", _LoadSemester = "", _LoadScoreType = "", _LoadScoreCategory = ""
                        , _FilterExamName = "", _FilterItemName = "", _FilterRankType = "", _FilterStudentNumber = "";

        // 讀取分批使用
        List<string> gradeYearList = new List<string>();

        public RegularAssessmentRankSelect()
        {
            InitializeComponent();
            //目前loading圖片因不明原因背景色會自動變成透明，所以加這行讓他變成白色
            pbLoading.BackColor = Color.White;
        }

        private void RegularRankSelect_Load(object sender, EventArgs e)
        {

            QueryHelper queryHelper = new QueryHelper();

            #region 取得學生年級

            gradeYearList.Clear();
            string grQuery = "SELECT DISTINCT class.grade_year FROM class INNER JOIN student ON student.ref_class_id = class.id WHERE student.status = 1 AND class.grade_year IS NOT NULL ORDER BY class.grade_year ASC;";
            DataTable dtG = queryHelper.Select(grQuery);
            foreach (DataRow dr in dtG.Rows)
            {
                string gr = dr["grade_year"].ToString();
                gradeYearList.Add(gr);
            }

            #endregion


            #region 要塞進前4個ComboBox的資料的sql字串
            string queryFilter = @"
SELECT rank_matrix.school_year
	, rank_matrix.semester
    , SUBSTRING(rank_matrix.item_type, 1, position('/' in rank_matrix.item_type) - 1) as score_type
	, SUBSTRING(rank_matrix.item_type, position('/' in rank_matrix.item_type) + 1, LENGTH(rank_matrix.item_type)) as score_category
	, exam.exam_name 
	, rank_matrix.item_name 
	, rank_matrix.rank_type 
FROM rank_matrix LEFT OUTER JOIN 
	exam ON exam.id=rank_matrix.ref_exam_id
WHERE rank_matrix.is_alive = true
	AND SUBSTRING(rank_matrix.item_type, 1, position('/' in rank_matrix.item_type) - 1) LIKE '定期評量%'";
            #endregion

            queryHelper = new QueryHelper();
            DataTable dt = queryHelper.Select(queryFilter);

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("無可檢視的資料");
                return;
            }

            #region 填入前4個ComboBox
            //學年度ComboBox
            foreach (DataRow row in dt.Rows)
            {
                string value = "" + row[0];
                if (!cboSchoolYear.Items.Contains(value))
                {
                    cboSchoolYear.Items.Add(value);
                }
            }

            if (cboSchoolYear.Items.Contains(K12.Data.School.DefaultSchoolYear))
            {
                cboSchoolYear.SelectedIndex = cboSchoolYear.Items.IndexOf(K12.Data.School.DefaultSchoolYear);
            }
            else
            {
                cboSchoolYear.SelectedIndex = 0;
            }

            //學期ComboBox
            foreach (DataRow row in dt.Rows)
            {
                string value = "" + row[1];
                if (!cboSemester.Items.Contains(value))
                {
                    cboSemester.Items.Add(value);
                }
            }

            if (cboSemester.Items.Contains(K12.Data.School.DefaultSemester))
            {
                cboSemester.SelectedIndex = cboSemester.Items.IndexOf(K12.Data.School.DefaultSemester);
            }
            else
            {
                cboSemester.SelectedIndex = 0;
            }

            //類型ComboBox
            List<ItemTypeMapping> itemTypeMappings = new List<ItemTypeMapping>();
            foreach (DataRow row in dt.Rows)
            {
                string value = "" + row[2];
                string name = value;
                string displayName = value;
                if (value == "定期評量")
                {
                    displayName = "定期評量_定期加平時";
                }

                if (!itemTypeMappings.Any(x => x.Name == name))
                {
                    ItemTypeMapping itemTypeMapping = new ItemTypeMapping(name, displayName);
                    itemTypeMappings.Add(itemTypeMapping);
                }
            }


            //顯示於類型ComboBox

            cboScoreType.Items.AddRange(itemTypeMappings.ToArray());

            cboScoreType.SelectedIndex = 0;

            //類別ComboBox
            foreach (DataRow row in dt.Rows)
            {
                string value = "" + row[3];
                if (!cboScoreCategory.Items.Contains(value))
                {
                    cboScoreCategory.Items.Add(value);
                }
            }
            cboScoreCategory.SelectedIndex = 0;
            #endregion

        }

        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            saveFileDialog.Title = "匯出排名資料";
            saveFileDialog.FileName = "匯出排名資料.xlsx";
            saveFileDialog.Filter = "Excel (*.xlsx)|*.xlsx|所有檔案 (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DialogResult dialogResult = new DialogResult();
                    if (dgvScoreRank.Columns.Count > 0)
                    {
                        Workbook workbook = new Workbook();
                        Worksheet worksheet = workbook.Worksheets[0];
                        worksheet.Name = "排名資料";

                        int colIndex = 0;
                        foreach (DataGridViewColumn column in dgvScoreRank.Columns)
                        {
                            if (column.Visible == true && column.HeaderText != "檢視")
                            {
                                worksheet.Cells[0, colIndex].PutValue(column.HeaderText);
                                colIndex++;
                            }
                        }

                        colIndex = 0;
                        foreach (DataGridViewColumn column in dgvScoreRank.Columns)
                        {
                            if (column.Visible == true && column.HeaderText != "檢視")
                            {
                                for (int rowIndex = 0; rowIndex < dgvScoreRank.Rows.Count; rowIndex++)
                                {
                                    worksheet.Cells[rowIndex + 1, colIndex].PutValue("" + dgvScoreRank[column.Index, rowIndex].Value);
                                }
                                colIndex++;
                            }
                        }

                        workbook.Save(saveFileDialog.FileName);
                    }

                    dialogResult = MessageBox.Show("檔案儲存完成，是否開啟？", "是否開啟", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        try
                        {
                            Process.Start(saveFileDialog.FileName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("檔案開啟失敗：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("檔案儲存失敗：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadRowData(object sender, EventArgs e)
        {
            if (_IsLoading) 
                return;

            if (!string.IsNullOrEmpty(cboSchoolYear.Text)
                && !string.IsNullOrEmpty(cboSemester.Text)
                && ((ItemTypeMapping)cboScoreType.SelectedItem) != null
                && !string.IsNullOrEmpty(cboScoreCategory.Text))
            {
                btnExportToExcel.Enabled = false;
                _IsLoading = true;
                dgvScoreRank.Rows.Clear();
                _LoadSchoolYear = cboSchoolYear.Text;
                _LoadSemester = cboSemester.Text;
                _LoadScoreType = ((ItemTypeMapping)cboScoreType.SelectedItem).Name;
                _LoadScoreCategory = cboScoreCategory.Text;



                DataTable dt = null;
                BackgroundWorker bkw = new BackgroundWorker();
                Exception bkwException = null;
                bkw.WorkerReportsProgress = true;
                pbLoading.Visible = true;
                int pr = 20;


                bkw.DoWork += delegate
                {
                    try
                    {
                        bkw.ReportProgress(0);
                        bool isFirst = true;

                        QueryHelper queryHelper = new QueryHelper();

                        foreach (string gr in gradeYearList)
                        {
                            #region 要顯示的資料的sql字串
                            string queryString = @"
SELECT 
    *
FROM
(
    SELECT rank_matrix.id AS rank_matrix_id 
        , SUBSTRING(rank_matrix.item_type, 1, position('/' in rank_matrix.item_type) - 1) AS score_type
        , SUBSTRING(rank_matrix.item_type, position('/' in rank_matrix.item_type) + 1, LENGTH(rank_matrix.item_type)) AS score_category 
        , exam.exam_name 
        , rank_matrix.item_name 
        , rank_matrix.rank_type 
        , rank_matrix.rank_name 
        , class.class_name 
        , student.seat_no 
        , student.student_number
        , student.name AS student_name
        , rank_detail.score
        , rank_detail.rank
        , rank_detail.pr
        , rank_detail.percentile
        , rank_matrix.school_year
        , rank_matrix.semester 
        , rank_matrix.create_time
        , rank_detail.ref_student_id
        , class.grade_year
    FROM rank_matrix 
        LEFT OUTER JOIN 
            rank_detail ON rank_detail.ref_matrix_id = rank_matrix.id 
        LEFT OUTER JOIN 
            student ON student.id = rank_detail.ref_student_id 
        LEFT OUTER JOIN 
            class ON class.id = student.ref_class_id 
        LEFT OUTER JOIN 
            exam ON exam.id=rank_matrix.ref_exam_id
    WHERE 
        rank_matrix.is_alive = true
    ORDER BY rank_matrix.rank_type
        , rank_matrix.rank_name
        , rank_detail.rank
        , rank_matrix.create_time DESC
) AS Rank_Table
WHERE  
    school_year = " + _LoadSchoolYear + @"
    And semester = " + _LoadSemester + @"
    And score_type = '" + _LoadScoreType + @"'
    And score_category = '" + _LoadScoreCategory + "' AND grade_year = '" + gr + "'";
                            #endregion

                            if (isFirst)
                            {
                                dt = queryHelper.Select(queryString);
                                isFirst = false;
                            }
                            else
                            {
                                DataTable dt1 = queryHelper.Select(queryString);

                                foreach (DataRow dr in dt1.Rows)
                                {
                                    dt.ImportRow(dr);
                                }
                            }

                            bkw.ReportProgress(pr);
                            pr += 20;
                        }


                        bkw.ReportProgress(100);
                    }
                    catch (Exception exc)
                    {
                        bkwException = exc;
                    }
                };

                bkw.ProgressChanged += delegate (object s1, ProgressChangedEventArgs e1)
                {
                    FISCA.Presentation.MotherForm.SetStatusBarMessage("資料讀取中", e1.ProgressPercentage);
                };

                bkw.RunWorkerCompleted += delegate
                {
                    if (bkwException != null)
                    {
                        throw new Exception("資料讀取錯誤", bkwException);
                    }
                    if (
                        _LoadSchoolYear != cboSchoolYear.Text
                        || _LoadSemester != cboSemester.Text
                        || _LoadScoreType != ((ItemTypeMapping)cboScoreType.SelectedItem).Name
                        || _LoadScoreCategory != cboScoreCategory.Text
                    )
                    {
                        _IsLoading = false;
                        LoadRowData(null, null);
                    }
                    else
                    {
                        #region 填入最後3個ComboBox
                        //試別ComboBox
                        cboExamName.Items.Clear();
                        cboExamName.Items.Add("全部");
                        foreach (DataRow row in dt.Rows)
                        {
                            string value = "" + row[3];
                            if (!cboExamName.Items.Contains(value))
                            {
                                cboExamName.Items.Add(value);
                            }
                        }
                        cboExamName.SelectedIndex = 0;

                        //項目ComboBox
                        cboItemName.Items.Clear();
                        cboItemName.Items.Add("全部");
                        foreach (DataRow row in dt.Rows)
                        {
                            string value = "" + row[4];
                            if (!cboItemName.Items.Contains(value))
                            {
                                cboItemName.Items.Add(value);
                            }
                        }
                        cboItemName.SelectedIndex = 0;

                        //母群ComboBox
                        cboRankType.Items.Clear();
                        cboRankType.Items.Add("全部");
                        foreach (DataRow row in dt.Rows)
                        {
                            string value = "" + row[5];
                            if (!cboRankType.Items.Contains(value))
                            {
                                cboRankType.Items.Add(value);
                            }
                        }
                        cboRankType.SelectedIndex = 0;
                        #endregion

                        #region 整理資料
                        _RowCollection = new List<DataGridViewRow>();
                        for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                        {
                            int tryParseInt;
                            decimal tryParseDecimal;
                            DataGridViewRow gridViewRow = new DataGridViewRow();
                            gridViewRow.CreateCells(dgvScoreRank);
                            gridViewRow.Cells[0].Value = "" + dt.Rows[rowIndex]["rank_matrix_id"];

                            string scoreType;

                            if ("" + dt.Rows[rowIndex]["score_type"] == "定期評量")
                            {
                                scoreType = "定期評量_定期加平時";
                            }
                            else
                            {
                                scoreType = "" + dt.Rows[rowIndex]["score_type"];

                            }
                            gridViewRow.Cells[1].Value = scoreType;
                            gridViewRow.Cells[2].Value = "" + dt.Rows[rowIndex]["score_category"];
                            gridViewRow.Cells[3].Value = "" + dt.Rows[rowIndex]["exam_name"];
                            gridViewRow.Cells[4].Value = "" + dt.Rows[rowIndex]["item_name"];
                            gridViewRow.Cells[5].Value = "" + dt.Rows[rowIndex]["rank_type"];
                            gridViewRow.Cells[6].Value = "" + dt.Rows[rowIndex]["rank_name"];
                            gridViewRow.Cells[7].Value = "" + dt.Rows[rowIndex]["class_name"];
                            gridViewRow.Cells[8].Value = Int32.TryParse("" + dt.Rows[rowIndex]["seat_no"], out tryParseInt) ? (int?)tryParseInt : null;
                            gridViewRow.Cells[9].Value = "" + dt.Rows[rowIndex]["student_number"];
                            gridViewRow.Cells[10].Value = "" + dt.Rows[rowIndex]["student_name"];
                            gridViewRow.Cells[11].Value = Decimal.TryParse("" + dt.Rows[rowIndex]["score"], out tryParseDecimal) ? (decimal?)tryParseDecimal : null;
                            gridViewRow.Cells[12].Value = Int32.TryParse("" + dt.Rows[rowIndex]["rank"], out tryParseInt) ? (int?)tryParseInt : null;
                            gridViewRow.Cells[13].Value = Int32.TryParse("" + dt.Rows[rowIndex]["pr"], out tryParseInt) ? (int?)tryParseInt : null;
                            gridViewRow.Cells[14].Value = Int32.TryParse("" + dt.Rows[rowIndex]["percentile"], out tryParseInt) ? (int?)tryParseInt : null;
                            gridViewRow.Cells[16].Value = "" + dt.Rows[rowIndex]["school_year"];
                            gridViewRow.Cells[17].Value = "" + dt.Rows[rowIndex]["semester"];
                            gridViewRow.Tag = "" + dt.Rows[rowIndex]["ref_student_id"];
                            _RowCollection.Add(gridViewRow);
                        }
                        #endregion

                        FISCA.Presentation.MotherForm.SetStatusBarMessage("資料讀取完成");
                        pbLoading.Visible = false;
                        _IsLoading = false;
                        btnExportToExcel.Text = "資料載入中";
                        FillingDataGridView(null, null);
                        btnExportToExcel.Text = "匯出";
                        btnExportToExcel.Enabled = true;
                    }
                };

                bkw.RunWorkerAsync();
            }
        }

        private void FillingDataGridView(object sender, EventArgs e)
        {
            if (_IsLoading)
                return;

            _IsLoading = true;
            btnExportToExcel.Enabled = false;
            btnExportToExcel.Text = "資料載入中";
            dgvScoreRank.Rows.Clear();
            _FilterExamName = cboExamName.Text;
            _FilterItemName = cboItemName.Text;
            _FilterRankType = cboRankType.Text;
            _FilterStudentNumber = txtStudentNum.Text;

            List<DataGridViewRow> newList = new List<DataGridViewRow>();
            foreach (DataGridViewRow gridViewRow in _RowCollection)
            {
                var show = true;
                if (_FilterExamName != "" && _FilterExamName != "全部" && _FilterExamName != ("" + gridViewRow.Cells[3].Value))
                {
                    show = show & false;
                }
                if (_FilterItemName != "" && _FilterItemName != "全部" && _FilterItemName != ("" + gridViewRow.Cells[4].Value))
                {
                    show = show & false;
                }
                if (_FilterRankType != "" && _FilterRankType != "全部" && _FilterRankType != ("" + gridViewRow.Cells[5].Value))
                {
                    show = show & false;
                }
                if (_FilterStudentNumber != "" && !("" + gridViewRow.Cells[9].Value).Contains(_FilterStudentNumber))
                {
                    show = show & false;
                }
                if (show)
                    newList.Add(gridViewRow);
                if (newList.Count > 200)
                {
                    dgvScoreRank.Rows.AddRange(newList.ToArray());
                    newList.Clear();
                    Application.DoEvents();

                    if (cboSchoolYear.Text != _LoadSchoolYear
                        || cboSemester.Text != _LoadSemester
                        || ((ItemTypeMapping)cboScoreType.SelectedItem).Name != _LoadScoreType
                        || cboScoreCategory.Text != _LoadScoreCategory)
                    {
                        _IsLoading = false;
                        LoadRowData(null, null);
                        return;
                    }

                    if (_FilterExamName != cboExamName.Text
                        || _FilterItemName != cboItemName.Text
                        || _FilterRankType != cboRankType.Text
                        || _FilterStudentNumber != txtStudentNum.Text)

                    {
                        _IsLoading = false;

                        FillingDataGridView(null, null);
                        return;
                    }

                    if (_IsClosing)
                    {
                        _IsClosing = false;
                        _IsLoading = false;
                        return;
                    }
                }
            }
            if (newList.Count > 0)
            {
                dgvScoreRank.Rows.AddRange(newList.ToArray());
                newList.Clear();
            }

            _IsLoading = false;
            btnExportToExcel.Text = "匯出";
            btnExportToExcel.Enabled = true;
        }

        private void dgvScoreRank_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            if (dgvScoreRank.Columns[e.ColumnIndex].HeaderText != "檢視")
            {
                return;
            }

            //處理一下用詞
            string scoreType;



            RegularMatrixRankSelect frm = new RegularMatrixRankSelect("" + dgvScoreRank[0, e.RowIndex].Value
                                                      , "" + dgvScoreRank.Rows[e.RowIndex].Tag
                                                      , "" + dgvScoreRank[16, e.RowIndex].Value
                                                      , "" + dgvScoreRank[17, e.RowIndex].Value
                                                      , "" + dgvScoreRank[1, e.RowIndex].Value

                                                      , "" + dgvScoreRank[2, e.RowIndex].Value
                                                      , "" + dgvScoreRank[3, e.RowIndex].Value
                                                      , "" + dgvScoreRank[4, e.RowIndex].Value
                                                      , "" + dgvScoreRank[5, e.RowIndex].Value
                                                      , "" + dgvScoreRank[6, e.RowIndex].Value);
            frm.ShowDialog();
        }

        private void RegularAssessmentRankSelect_Resize(object sender, EventArgs e)
        {
            //調整Loading圖案的位置
            pbLoading.Location = new Point(this.Width / 2 - 20, this.Height / 2 - 20);
        }

        private void RegularAssessmentRankSelect_FormClosing(object sender, FormClosingEventArgs e)
        {
            _IsClosing = true;
        }

        //private string GetMappingWord(string itemType)
        //{
        //    string reault = itemType;

        //    if (itemType == "定期評量")
        //    {
        //        reault = "定期評量_定期加平時";
        //    }
        //    return reault;
        //}

    }
}