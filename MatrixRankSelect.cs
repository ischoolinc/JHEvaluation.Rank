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
    public partial class MatrixRankSelect : BaseForm
    {
        public MatrixRankSelect(string SchoolYear, string Semester, string ScoreType, string ScoreCategory, string ExamName, string ItemName, string RankType)
        {
            InitializeComponent();

            lbSchoolYear.Text = SchoolYear;
            lbSemester.Text = Semester;
            lbScoreType.Text = ScoreType;
            lbScoreCategory.Text = ScoreCategory;
            lbExamName.Text = ExamName;
            lbItemName.Text = ItemName;
            lbRankType.Text = RankType;
        }

        BackgroundWorker _backgroundWorker = new BackgroundWorker();

        private void MatrixRankSelect_Load(object sender, EventArgs e)
        {
            QueryHelper queryHelper = new QueryHelper();

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
		, rank_matrix.school_year
		, rank_matrix.semester 
        , rank_matrix.is_alive
	FROM rank_matrix LEFT OUTER JOIN 
		rank_detail ON rank_detail.ref_matrix_id = rank_matrix.id LEFT OUTER JOIN 
		student ON student.id = rank_detail.ref_student_id LEFT OUTER JOIN 
		class ON class.id = student.ref_class_id LEFT OUTER JOIN 
		exam ON exam.id=rank_matrix.ref_exam_id) as Rank_Table
Where  school_year = " + Convert.ToInt32(lbSchoolYear.Text) +
"And semester = " + Convert.ToInt32(lbSemester.Text) +
"And score_type = '" + lbScoreType.Text + "'" +
"And score_category = '" + lbScoreCategory.Text + "'" +
"And exam_name = '" + lbExamName.Text + "'" +
"And item_name = '" + lbItemName.Text + "'" +
"And rank_type = '" + lbRankType.Text + "'";
            #endregion

            try
            {
                DataTable dataTable = new DataTable();
                dataTable = queryHelper.Select(queryTable);

                #region 填入編號的ComboBox
                foreach (DataRow row in dataTable.Rows)
                {
                    if (!cboMatrixId.Items.Contains("" + row["rank_matrix_id"]))
                    {
                        string isAlive = "";
                        if (row["is_alive"] != null)
                        {
                            if (Convert.ToBoolean(row["is_alive"]) == true)
                            {
                                isAlive = "*";
                            }
                        }
                        cboMatrixId.Items.Add(isAlive + row["rank_matrix_id"]);
                    }
                }
                #endregion
                cboMatrixId.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace.ToString());
            }


        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string query = (string)e.Argument;

            try
            {
                DataTable dt = new DataTable();

                QueryHelper queryHelper = new QueryHelper();
                dt = queryHelper.Select(query);

                e.Result = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("資料讀取失敗：" + ex.Message);
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DataTable dt = (DataTable)e.Result;

            try
            {
                #region 塞資料進dataGridView
                List<DataGridViewRow> gridViewRowList = new List<DataGridViewRow>();
                dgvScoreRank.Rows.Clear();
                dgvScoreRank.SuspendLayout();
                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    DataGridViewRow gridViewRow = new DataGridViewRow();
                    gridViewRow.CreateCells(dgvScoreRank);
                    for (int col = 0; col < dt.Columns.Count - 2; col++)
                    {
                        gridViewRow.Cells[col].Value = "" + dt.Rows[row][col];
                    }
                    gridViewRowList.Add(gridViewRow);
                }
                dgvScoreRank.Rows.AddRange(gridViewRowList.ToArray());
                dgvScoreRank.ResumeLayout();
                #endregion

                lbCreateTime.Text = Convert.ToDateTime(dt.Rows[0]["create_time"]).ToString("yyyy/MM/dd");
                lbMemo.Text = "" + dt.Rows[0]["memo"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            saveFileDialog.Title = "匯出排名母群資料";
            saveFileDialog.FileName = "匯出排名母群資料.xlsx";
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
                        worksheet.Name = "排名母群資料";

                        int colIndex = 0;
                        foreach (DataGridViewColumn column in dgvScoreRank.Columns)
                        {
                            if (column.Visible == true)
                            {
                                worksheet.Cells[0, colIndex].PutValue(column.HeaderText);
                                colIndex++;
                            }
                        }

                        colIndex = 0;
                        foreach (DataGridViewColumn column in dgvScoreRank.Columns)
                        {
                            if (column.Visible == true)
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

        private void LoadRowData(object sender, EventArgs e)
        {
            string MatrixID = cboMatrixId.Text.Trim('*');
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
        , CASE WHEN student.status = 1 then '一般'::TEXT
					 WHEN student.status = 2 then '延修' ::TEXT
					 WHEN student.status = 4 then '休學'::TEXT
					 WHEN student.status = 8 then '輟學'::TEXT
					 WHEN student.status = 16 then '畢業或離校'::TEXT
					 WHEN student.status = 256 then '刪除'::TEXT
					 ELSE ''||student.status
		END as status
		, rank_detail.score
		, rank_detail.rank
		, rank_detail.pr
		, rank_detail.percentile
		, rank_matrix.school_year
		, rank_matrix.semester
		, rank_matrix.create_time
		, rank_matrix.memo
	FROM rank_matrix LEFT OUTER JOIN 
		rank_detail ON rank_detail.ref_matrix_id = rank_matrix.id LEFT OUTER JOIN 
		student ON student.id = rank_detail.ref_student_id LEFT OUTER JOIN 
		class ON class.id = student.ref_class_id LEFT OUTER JOIN 
		exam ON exam.id=rank_matrix.ref_exam_id) as Rank_Table
Where rank_matrix_id = " + Convert.ToInt32(MatrixID);
            #endregion

            _backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            _backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            _backgroundWorker.RunWorkerAsync(queryTable);
        }
    }
}
