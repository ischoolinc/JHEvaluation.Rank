using Aspose.Cells;
using FISCA.Data;
using FISCA.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JHEvaluation.Rank
{
    public partial class RegularRankSelect : BaseForm
    {
        private bool _IsFilling = false;

        public RegularRankSelect()
        {
            InitializeComponent();
        }

        private List<DataGridViewRow> _RowCollection = new List<DataGridViewRow>();

        private void RegularRankSelect_Load(object sender, EventArgs e)
        {

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
	rank_detail ON rank_detail.ref_matrix_id = rank_matrix.id LEFT OUTER JOIN
	exam ON exam.id=rank_matrix.ref_exam_id 
WHERE rank_matrix.is_alive = true";
            #endregion

            QueryHelper queryHelper = new QueryHelper();
            DataTable dt = queryHelper.Select(queryFilter);

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
            cboSchoolYear.SelectedIndex = 0;

            //學期ComboBox
            foreach (DataRow row in dt.Rows)
            {
                string value = "" + row[1];
                if (!cboSemester.Items.Contains(value))
                {
                    cboSemester.Items.Add(value);
                }
            }
            cboSemester.SelectedIndex = 0;

            //類型ComboBox
            foreach (DataRow row in dt.Rows)
            {
                string value = "" + row[2];
                if (!cboScoreType.Items.Contains(value))
                {
                    cboScoreType.Items.Add(value);
                }
            }
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

        private DataTable getDataAsync(string queryString)
        {
            string query = queryString;
            DataTable dt = new DataTable();
            try
            {
                dt = new DataTable();

                QueryHelper queryHelper = new QueryHelper();
                dt = queryHelper.Select(query);

            }
            catch (Exception ex)
            {
                MessageBox.Show("資料讀取失敗：" + ex.Message);
            }

            return dt;
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

        private async void LoadRowData(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cboSchoolYear.Text)
                && !string.IsNullOrEmpty(cboSemester.Text)
                && !string.IsNullOrEmpty(cboScoreType.Text)
                && !string.IsNullOrEmpty(cboScoreCategory.Text))
            {

                #region 要顯示的資料的sql字串
                string queryTable = @"
Select *
From
	(SELECT rank_matrix.id AS rank_matrix_id 
		, SUBSTRING(rank_matrix.item_type, 1, position('/' in rank_matrix.item_type) - 1) as score_type
		, SUBSTRING(rank_matrix.item_type, position('/' in rank_matrix.item_type) + 1, LENGTH(rank_matrix.item_type)) as score_category 
		, exam.exam_name 
		, rank_matrix.item_name 
		, rank_matrix.rank_type 
		, rank_matrix.rank_name 
		, class.class_name 
		, student.seat_no 
		, student.student_number
		, student.name 
		, rank_detail.score
		, rank_detail.rank
		, rank_detail.pr
		, rank_detail.percentile
		, rank_matrix.school_year
		, rank_matrix.semester 
	FROM rank_matrix LEFT OUTER JOIN 
		rank_detail ON rank_detail.ref_matrix_id = rank_matrix.id LEFT OUTER JOIN 
		student ON student.id = rank_detail.ref_student_id LEFT OUTER JOIN 
		class ON class.id = student.ref_class_id LEFT OUTER JOIN 
		exam ON exam.id=rank_matrix.ref_exam_id 
	WHERE rank_matrix.is_alive = true) as Rank_Table
Where  school_year = " + Convert.ToInt32(cboSchoolYear.Text) +
    "And semester = " + Convert.ToInt32(cboSemester.Text) +
    "And score_type = '" + cboScoreType.Text + "'" +
    "And score_category = '" + cboScoreCategory.Text + "'";
                #endregion

                DataTable dt = await Task.Run(() => getDataAsync(queryTable));

                if (dt.Rows.Count == 0)
                {
                    return;
                }

                try
                {
                    _IsFilling = true;
                    dgvScoreRank.Rows.Clear();
                    cboExamName.Items.Clear();
                    cboItemName.Items.Clear();
                    cboRankType.Items.Clear();

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

                    #region 塞資料進DataGridView
                    _RowCollection = new List<DataGridViewRow>();
                    for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
                    {
                        DataGridViewRow gridViewRow = new DataGridViewRow();
                        gridViewRow.CreateCells(dgvScoreRank);
                        for (int colindex = 0; colindex < dt.Columns.Count; colindex++)
                        {
                            if (colindex >= 14)
                            {
                                gridViewRow.Cells[colindex + 1].Value = "" + dt.Rows[rowIndex][colindex];
                            }
                            else
                            {
                                gridViewRow.Cells[colindex].Value = "" + dt.Rows[rowIndex][colindex];
                            }

                        }
                        _RowCollection.Add(gridViewRow);
                    }
                    #endregion

                    _IsFilling = false;
                    FillingDataGridView(null, null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void FillingDataGridView(object sender, EventArgs e)
        {
            if (_IsFilling)
                return;
            dgvScoreRank.Rows.Clear();
            List<DataGridViewRow> newList = new List<DataGridViewRow>();
            foreach (DataGridViewRow gridViewRow in _RowCollection)
            {
                var show = true;
                if (cboExamName.Text != "" && cboExamName.Text != "全部" && cboExamName.Text != ("" + gridViewRow.Cells[3].Value))
                {
                    show = show & false;
                }
                if (cboItemName.Text != "" && cboItemName.Text != "全部" && cboItemName.Text != ("" + gridViewRow.Cells[4].Value))
                {
                    show = show & false;
                }
                if (cboRankType.Text != "" && cboRankType.Text != "全部" && cboRankType.Text != ("" + gridViewRow.Cells[5].Value))
                {
                    show = show & false;
                }
                if (txtStudentNum.Text != "" && !("" + gridViewRow.Cells[9].Value).Contains(txtStudentNum.Text))
                {
                    show = show & false;
                }
                if (show)
                    newList.Add(gridViewRow);
            }
            dgvScoreRank.Rows.AddRange(newList.ToArray());
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

            MatrixRankSelect frm = new MatrixRankSelect("" + dgvScoreRank[16, e.RowIndex].Value
                                                      , "" + dgvScoreRank[17, e.RowIndex].Value
                                                      , "" + dgvScoreRank[1, e.RowIndex].Value
                                                      , "" + dgvScoreRank[2, e.RowIndex].Value
                                                      , "" + dgvScoreRank[3, e.RowIndex].Value
                                                      , "" + dgvScoreRank[4, e.RowIndex].Value
                                                      , "" + dgvScoreRank[5, e.RowIndex].Value);
            frm.ShowDialog();
        }
    }
}
