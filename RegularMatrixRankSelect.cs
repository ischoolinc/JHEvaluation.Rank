﻿using Aspose.Cells;
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
    public partial class RegularMatrixRankSelect : BaseForm
    {
        public static List<ICalculateRegularAssessmentExtension> ExtensionList { get; set; }
    = new List<ICalculateRegularAssessmentExtension>();

        bool _IsLoading = false;
        string _RankName, _RankMatrixID;
        private string _RefStudentID;
        private Dictionary<string, string> _DicMatrixID = new Dictionary<string, string>();
        //Dictionary<string, string> _MatrixIdDic = new Dictionary<string, string>();
        private Dictionary<string, DataGridViewRow> _DicMatrixInfoRow = new Dictionary<string, DataGridViewRow>();
        private Dictionary<string, DataGridViewRow> _DicMatrixInfoRow2 = new Dictionary<string, DataGridViewRow>();

        public RegularMatrixRankSelect(string rankMatrixId, string refStuID, string schoolYear, string semester, string scoreType, string scoreCategory, string examName, string itemName, string rankType, string rankName)
        {
            InitializeComponent();
            AddExtensionColumns(scoreType);
            lbSchoolYear.Text = schoolYear;
            lbSemester.Text = semester;
            lbScoreType.Text = scoreType;
            lbScoreCategory.Text = scoreCategory;
            lbExamName.Text = examName;
            lbItemName.Text = itemName;
            lbRankType.Text = rankType;
            _RankName = rankName;
            _RankMatrixID = rankMatrixId;
            _RefStudentID = refStuID;

        }

        private void MatrixRankSelect_Load(object sender, EventArgs e)
        {
            try
            {
                QueryHelper queryHelper = new QueryHelper();

                #region 要顯示的資料的sql字串
                string queryTable = @"WITH data AS(
SELECT 
	rank_matrix.id AS rank_matrix_id
    , rank_matrix.ref_batch_id
	, SUBSTRING(rank_matrix.item_type, 1, position('/' in rank_matrix.item_type) - 1) AS score_type
	, SUBSTRING(rank_matrix.item_type, position('/' in rank_matrix.item_type) + 1, LENGTH(rank_matrix.item_type)) AS score_category 
	, exam.exam_name 
	, rank_matrix.item_name 
	, rank_matrix.rank_type
	, rank_matrix.rank_name
	, rank_matrix.school_year
	, rank_matrix.semester 
	, rank_matrix.is_alive
	, rank_matrix.create_time
	, rank_matrix.matrix_count
	, rank_matrix.avg_top_25
	, rank_matrix.avg_top_50
	, rank_matrix.avg
	, rank_matrix.avg_bottom_50
	, rank_matrix.avg_bottom_25
	, rank_matrix.level_gte100
	, rank_matrix.level_90
	, rank_matrix.level_80
	, rank_matrix.level_70
	, rank_matrix.level_60
	, rank_matrix.level_50
	, rank_matrix.level_40
	, rank_matrix.level_30
	, rank_matrix.level_20
	, rank_matrix.level_10
	, rank_matrix.level_lt10
    , rank_matrix
    , rank_matrix.std_dev_pop
    , rank_matrix.pr_88
    , rank_matrix.pr_75
    , rank_matrix.pr_50
    , rank_matrix.pr_25
    , rank_matrix.pr_12
	, jsonb_array_elements(CASE WHEN rank_matrix.extension::TEXT = '[]' THEN '[null]'::JSONB ELSE rank_matrix.extension END) AS extension
FROM 
	rank_matrix AS source
    INNER JOIN rank_matrix
		ON rank_matrix.school_year = source.school_year
		AND rank_matrix.semester = source.semester
		AND rank_matrix.item_type = source.item_type
        AND rank_matrix.grade_year = source.grade_year
		AND rank_matrix.ref_exam_id = source.ref_exam_id
		AND rank_matrix.item_name = source.item_name
		AND rank_matrix.rank_type = source.rank_type
		AND rank_matrix.rank_name = source.rank_name
	LEFT OUTER JOIN exam 
        ON exam.id=rank_matrix.ref_exam_id
WHERE
	source.id = " + _RankMatrixID + @"::BIGINT
    AND rank_matrix.id IN (
        SELECT ref_matrix_id FROM rank_detail WHERE ref_student_id =  " + _RefStudentID + @"::BIGINT
    )
ORDER BY
	create_time DESC
	)
	SELECT
		rank_matrix_id
	    , ref_batch_id
		,  score_type
		, score_category 
		, exam_name 
		, item_name 
		, rank_type
		, rank_name
		, school_year
		, semester 
		, is_alive
		, create_time
		, matrix_count
		, avg_top_25
		, avg_top_50
		, avg
		, avg_bottom_50
		, avg_bottom_25
		, level_gte100
		, level_90
		, level_80
		, level_70
		, level_60
		, level_50
		, level_40
		, level_30
		, level_20
		, level_10
		, level_lt10
		, rank_matrix
		, std_dev_pop
		, pr_88
		, pr_75
		, pr_50
		, pr_25
		, pr_12
		, data.extension->>'extension_name' AS extension_name"
+ ", data.extension->>'A++' AS \"A++\""
+ ", data.extension->> 'A+' AS \"A+\""
+ ", data.extension->> 'A' AS \"A\""
+ ", data.extension->> 'B++' AS \"B++\""
+ ", data.extension->> 'B+' AS \"B+\""
+ ", data.extension->> 'B' AS \"B\""
+ "FROM data";
                #endregion

                DataTable dataTable = new DataTable();
                dataTable = queryHelper.Select(queryTable);

                #region 填入編號的ComboBox
                foreach (DataRow row in dataTable.Rows)
                {
                    //if (!cboBatchId.Items.Contains(row["ref_batch_id"] + "（計算時間：" + Convert.ToDateTime(row["create_time"]).ToString("yyyy/MM/dd HH:mm") + "）")
                    //    && !cboBatchId.Items.Contains(row["ref_batch_id"] + "（計算時間：" + Convert.ToDateTime(row["create_time"]).ToString("yyyy/MM/dd HH:mm") + "）-目前採計"))
                    //{
                    //    string isAlive = "";
                    //    if (!string.IsNullOrEmpty("" + row["is_alive"]))
                    //    {
                    //        if (Convert.ToBoolean(row["is_alive"]) == true)
                    //        {
                    //            isAlive = "-目前採計";
                    //        }
                    //    }
                    //    cboBatchId.Items.Add(row["ref_batch_id"] + "（計算時間：" + Convert.ToDateTime(row["create_time"]).ToString("yyyy/MM/dd HH:mm") + "）" + isAlive);
                    //    _MatrixIdDic.Add(row["ref_batch_id"] + "（計算時間：" + Convert.ToDateTime(row["create_time"]).ToString("yyyy/MM/dd HH:mm") + "）" + isAlive, "" + row["rank_matrix_id"]);
                    //}



                    bool tryParseBool = false;
                    var key = "" + row["ref_batch_id"] + "（計算時間：" + Convert.ToDateTime(row["create_time"]).ToString("yyyy/MM/dd HH:mm") + "）" + (bool.TryParse("" + row["is_alive"], out tryParseBool) && tryParseBool ? "-目前採計" : "");
                    var newIndex = cboBatchId.Items.Add(key);

                    var newRow = dgvMatrixInfo.Rows[dgvMatrixInfo.Rows.Add(
                        "" + row["matrix_count"]
                        , "" + row["std_dev_pop"]
                        , "" + row["level_gte100"]
                        , "" + row["level_90"]
                        , "" + row["level_80"]
                        , "" + row["level_70"]
                        , "" + row["level_60"]
                        , "" + row["level_50"]
                        , "" + row["level_40"]
                        , "" + row["level_30"]
                        , "" + row["level_20"]
                        , "" + row["level_10"]
                        , "" + row["level_lt10"]
                    )];
                    newRow.Visible = false;

                    var newRow2 = dgvMatrixInfo2.Rows[dgvMatrixInfo2.Rows.Add(
                         "" + row["avg_top_25"]
                        , "" + row["avg_top_50"]
                        , "" + row["avg"]
                        , "" + row["avg_bottom_50"]
                        , "" + row["avg_bottom_25"]

                        , "" + row["pr_88"]
                        , "" + row["pr_75"]
                        , "" + row["pr_50"]
                        , "" + row["pr_25"]
                        , "" + row["pr_12"]

                        , "" + row["A++"]
                        , "" + row["A+"]
                        , "" + row["A"]
                        , "" + row["B++"]
                        , "" + row["B+"]
                        , "" + row["B"]

                    )];
                    newRow2.Visible = false;


                    if (!_DicMatrixID.ContainsKey(key))
                        _DicMatrixID.Add(key, "" + row["rank_matrix_id"]);

                    if (!_DicMatrixInfoRow.ContainsKey(key))
                        _DicMatrixInfoRow.Add(key, newRow);

                    if (!_DicMatrixInfoRow2.ContainsKey(key))
                        _DicMatrixInfoRow2.Add(key, newRow2);

                    //if (!_DicDegreeRow.ContainsKey(key))
                    //    _DicDegreeRow.Add(key, newRow3);
                }

                if (cboBatchId.Items.Contains("-目前採計"))
                {
                    cboBatchId.SelectedIndex = cboBatchId.Items.IndexOf("-目前採計");
                }
                else
                {
                    cboBatchId.SelectedIndex = 0;
                }
                #endregion
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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Manual manual = new Manual();
            manual.ShowDialog();
        }

        private void LoadRowData(object sender, EventArgs e)
        {
            if (_IsLoading == true)
            {
                return;
            }
            btnExportToExcel.Enabled = false;
            btnExportToExcel.Text = "資料載入中";
            _IsLoading = true;
            // string matrixId = _MatrixIdDic[cboBatchId.Text];

            //Jean
            string matrixId = _DicMatrixID[cboBatchId.Text];

            //顯示對應的母群資訊
            foreach (DataGridViewRow row in dgvMatrixInfo.Rows)
            {
                if (row == _DicMatrixInfoRow[cboBatchId.Text])
                    row.Visible = true;
                else
                    row.Visible = false;
            }
            foreach (DataGridViewRow row in dgvMatrixInfo2.Rows)
            {
                if (row == _DicMatrixInfoRow2[cboBatchId.Text])
                    row.Visible = true;
                else
                    row.Visible = false;
            }

            #region 要顯示的資料的sql字串
            string queryString = @"
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
		, student.name AS student_name
        , CASE WHEN student.status = 1 then '一般'::TEXT
					 WHEN student.status = 2 then '延修' ::TEXT
					 WHEN student.status = 4 then '休學'::TEXT
					 WHEN student.status = 8 then '輟學'::TEXT
					 WHEN student.status = 16 then '畢業或離校'::TEXT
					 WHEN student.status = 256 then '刪除'::TEXT
					 ELSE ''||student.status
		END as student_status
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
Where rank_matrix_id = '" + matrixId + @"'
ORDER BY rank
";
            #endregion

            BackgroundWorker bkw = new BackgroundWorker();
            DataTable dt = new DataTable();
            Exception bkwException = null;

            bkw.DoWork += delegate
            {
                string query = queryString;
                try
                {
                    QueryHelper queryHelper = new QueryHelper();
                    dt = queryHelper.Select(query);
                }
                catch (Exception ex)
                {
                    bkwException = ex;
                }
            };

            bkw.RunWorkerCompleted += delegate
            {
                if (bkwException != null)
                {
                    throw new Exception("資料讀取錯誤", bkwException);
                }
                //  string selectedMatrixId = _MatrixIdDic[cboBatchId.Text];
                string selectedMatrixId = _DicMatrixID[cboBatchId.Text];
                if (matrixId != selectedMatrixId)
                {
                    _IsLoading = false;
                    LoadRowData(null, null);

                }
                else
                {
                    try
                    {
                        #region 塞資料進dataGridView
                        List<DataGridViewRow> gridViewRowList = new List<DataGridViewRow>();
                        dgvScoreRank.Rows.Clear();
                        dgvScoreRank.SuspendLayout();
                        for (int row = 0; row < dt.Rows.Count; row++)
                        {
                            int tryParseInt;
                            decimal tryParseDecimal;
                            DataGridViewRow gridViewRow = new DataGridViewRow();
                            gridViewRow.CreateCells(dgvScoreRank);
                            gridViewRow.Cells[0].Value = "" + dt.Rows[row]["rank_matrix_id"];
                            string scoreType;
                            if ("" + dt.Rows[row]["score_type"] == "定期評量")
                            {
                                scoreType = "定期評量_定期加平時";
                            }
                            else
                            {
                                scoreType = "" + dt.Rows[row]["score_type"];
                            }

                            gridViewRow.Cells[1].Value = scoreType;
                            gridViewRow.Cells[2].Value = "" + dt.Rows[row]["score_category"];
                            gridViewRow.Cells[3].Value = "" + dt.Rows[row]["exam_name"];
                            gridViewRow.Cells[4].Value = "" + dt.Rows[row]["item_name"];
                            gridViewRow.Cells[5].Value = "" + dt.Rows[row]["rank_type"];
                            gridViewRow.Cells[6].Value = "" + dt.Rows[row]["rank_name"];
                            gridViewRow.Cells[7].Value = "" + dt.Rows[row]["class_name"];
                            gridViewRow.Cells[8].Value = Int32.TryParse("" + dt.Rows[row]["seat_no"], out tryParseInt) ? (int?)tryParseInt : null;
                            gridViewRow.Cells[9].Value = "" + dt.Rows[row]["student_number"];
                            gridViewRow.Cells[10].Value = "" + dt.Rows[row]["student_name"];
                            gridViewRow.Cells[11].Value = "" + dt.Rows[row]["student_status"];
                            gridViewRow.Cells[12].Value = Decimal.TryParse("" + dt.Rows[row]["score"], out tryParseDecimal) ? (decimal?)tryParseDecimal : null;
                            gridViewRow.Cells[13].Value = Int32.TryParse("" + dt.Rows[row]["rank"], out tryParseInt) ? (int?)tryParseInt : null;
                            gridViewRow.Cells[14].Value = Int32.TryParse("" + dt.Rows[row]["pr"], out tryParseInt) ? (int?)tryParseInt : null;
                            gridViewRow.Cells[15].Value = Int32.TryParse("" + dt.Rows[row]["percentile"], out tryParseInt) ? (int?)tryParseInt : null;
                            gridViewRow.Cells[16].Value = "" + dt.Rows[row]["school_year"];
                            gridViewRow.Cells[17].Value = "" + dt.Rows[row]["semester"];
                            gridViewRowList.Add(gridViewRow);
                        }
                        dgvScoreRank.Rows.AddRange(gridViewRowList.ToArray());
                        dgvScoreRank.ResumeLayout();
                        #endregion

                        lbMemo.Text = "" + dt.Rows[0]["memo"];
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            };

            bkw.RunWorkerAsync();

            _IsLoading = false;
            btnExportToExcel.Text = "匯出";
            btnExportToExcel.Enabled = true;
        }

        public void AddExtensionColumns(string scoreType)
        {
            foreach (var extensionItem in ExtensionList)
            {
                //if (extensionItem.Title == "計算定期評量擴充功能：計算自訂等第")
                //{
                    extensionItem.AddDGVColumn(dgvMatrixInfo2, scoreType);
                //}
            }

        }

    }
}
