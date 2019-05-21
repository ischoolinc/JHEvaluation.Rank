using FISCA.Presentation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using K12.Data;
using FISCA.Data;
using FISCA.Presentation;
using System.Xml;

namespace JHEvaluation.Rank
{
    public partial class CalculateRegularAssessmentRank : BaseForm
    {
        DataTable _SchoolYearTable = new DataTable();
        string _DefaultSchoolYear = "";
        string _DefaultSemester = "";
        List<ExamRecord> _ExamList = new List<ExamRecord>();
        List<TagConfigRecord> _TagConfigRecord = new List<TagConfigRecord>();
        List<StudentRecord> _StudentRecord = new List<StudentRecord>();
        List<CheckBox> _CheckBoxList = new List<CheckBox>();
        List<StudentRecord> _FilterStudentList = new List<StudentRecord>();
        int _FormWidth = 0, _FormHeight = 0;

        public CalculateRegularAssessmentRank()
        {
            InitializeComponent();

            #region 查詢學年度、學期
            string queryString = @"
SELECT
	course.school_year
	, course.semester
FROM
		course
Order BY course.school_year, course.semester
";
            #endregion

            QueryHelper queryHelper = new QueryHelper();

            try
            {
                _SchoolYearTable = queryHelper.Select(queryString);
                _DefaultSchoolYear = K12.Data.School.DefaultSchoolYear;
                _DefaultSemester = K12.Data.School.DefaultSemester;
                _ExamList = K12.Data.Exam.SelectAll();
                _TagConfigRecord = K12.Data.TagConfig.SelectByCategory(TagCategory.Student);
                _StudentRecord = K12.Data.Student.SelectAll().ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("資料讀取失敗", ex);
            }
        }

        private void CacluateRegularAssessmentRank_Load(object sender, EventArgs e)
        {
            #region 讓Form回到起始狀態
            plStudentView.Visible = false;
            this.Width = 580;
            this.Height = 330;
            #endregion

            #region 篩選資料
            _StudentRecord = _StudentRecord.Where(x => !string.IsNullOrEmpty(x.RefClassID)
                                                    && (x.Status == StudentRecord.StudentStatus.一般)
                                                    && x.Class.GradeYear != null).ToList();
            #endregion

            #region 填資料進ComboBox
            cboSchoolYear.Items.Clear();
            cboSemester.Items.Clear();
            cboExamType.Items.Clear();
            cboStudentFilter.Items.Clear();
            cboStudentTag1.Items.Clear();
            cboStudentTag2.Items.Clear();

            cboSchoolYear.Items.Add(_DefaultSchoolYear);//加入預設的學年度
            cboSemester.Items.Add(_DefaultSemester);//加入預設的學年度
            foreach (DataRow row in _SchoolYearTable.Rows)
            {
                #region 現階段先不用匯入其他學年度及學期
                //現階段先不用匯入其他學年度
                //if (!string.IsNullOrEmpty("" + row["school_year"]) && !cboSchoolYear.Items.Contains("" + row["school_year"]))
                //{
                //    cboSchoolYear.Items.Add("" + row["school_year"]);
                //} 

                //if (!string.IsNullOrEmpty("" + row["semester"]) && !cboSemester.Items.Contains("" + row["semester"]))
                //{
                //    cboSemester.Items.Add("" + row["semester"]);
                //}
                #endregion
            }
            cboSchoolYear.SelectedIndex = 0;

            if (cboSemester.Items.Contains(_DefaultSemester))
            {
                cboSemester.SelectedIndex = cboSemester.Items.IndexOf(_DefaultSemester);
            }
            else
            {
                cboSemester.SelectedIndex = 0;
            }

            cboStudentFilter.Items.Add("");
            cboStudentTag1.Items.Add("");
            cboStudentTag2.Items.Add("");
            foreach (var item in _TagConfigRecord.Select(x => x.Prefix).Distinct())
            {
                cboStudentFilter.Items.Add("[" + item + "]");
                cboStudentTag1.Items.Add("[" + item + "]");
                cboStudentTag2.Items.Add("[" + item + "]");
            }
            cboStudentFilter.SelectedIndex = 0;
            cboStudentTag1.SelectedIndex = 0;
            cboStudentTag2.SelectedIndex = 0;

            foreach (var item in _ExamList.Select(x => x.Name).Distinct())
            {
                cboExamType.Items.Add(item);
            }
            cboExamType.SelectedIndex = 0;
            #endregion

            //整理年級的清單
            List<int> gradeList = _StudentRecord.Select(x => Convert.ToInt32(x.Class.GradeYear)).Distinct().OrderBy(x => x).ToList();

            #region 動態產生年級的CheckBox
            this.Height += 32 * (((gradeList.Count % 4) == 0 ? (gradeList.Count == 4 ? 0 : (gradeList.Count / 4) - 1) : gradeList.Count / 4) + 1);//每多一排checkBox，Form的高度+32
            for (int i = 0; i < gradeList.Count; i++)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.AutoSize = true;
                checkBox.Location = new System.Drawing.Point(13 + (133 * (i % 4)), 8 + (32 * (i / 4))); //第一個checkBox的位置X=13, y=8，兩個checkBox的X差距133，兩個checkBox的Y差距32
                checkBox.Name = "ch" + gradeList[i];
                checkBox.Size = new System.Drawing.Size(101, 23);
                checkBox.TabIndex = 7 + i;
                checkBox.Text = "" + gradeList[i] + "年級";
                checkBox.UseVisualStyleBackColor = true;
                checkBox.Checked = true;
                gpRankPeople.Controls.Add(checkBox);
            }
            #endregion
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            //紀錄第一頁的Form的大小
            _FormWidth = this.Width;
            _FormHeight = this.Height;

            plStudentView.Visible = true;
            this.Width = 810;
            this.Height = 510;
            lbExam.Text = cboExamType.Text;
            lbSemester.Text = cboSemester.Text;
            lbSchoolYear.Text = cboSchoolYear.Text;

            #region 依據勾選的項目動態產生CheckBox
            int checkBoxCount = 0;
            foreach (CheckBox checkBox in gpRankPeople.Controls.OfType<CheckBox>())
            {
                if (checkBox.Checked == true)
                {
                    CheckBox newCheckBox = new CheckBox();
                    newCheckBox.Location = new System.Drawing.Point(65 + (97 * (checkBoxCount % 4)), 44 + (27 * (checkBoxCount / 4)));//第一個checkBox的位置X=65, y=44，兩個checkBox的X差距97，兩個checkBox的Y差距27
                    newCheckBox.Name = "new" + checkBox.Name;
                    newCheckBox.Size = new System.Drawing.Size(91, 21);
                    newCheckBox.TabIndex = 26 + checkBoxCount;
                    newCheckBox.Text = checkBox.Text;
                    newCheckBox.UseVisualStyleBackColor = true;
                    newCheckBox.Enabled = false;
                    newCheckBox.Checked = true;
                    newCheckBox.Visible = false;//因為不需要且_CheckBoxList後面用的到，所以保留此功能但不顯示

                    plStudentView.Controls.Add(newCheckBox);
                    _CheckBoxList.Add(newCheckBox);
                    checkBoxCount++;
                }
            }

            //每多一排CheckBox就把Form的高加23，dataGridView的位置往下加23並把高減少23
            int height = 23 * ((checkBoxCount % 4) == 0 ? (checkBoxCount == 4 ? 0 : checkBoxCount / 4) : checkBoxCount / 4);
            this.Height += height;
            dgvStudentList.Location = new Point(3, dgvStudentList.Location.Y + height);
            dgvStudentList.Height -= height;
            #endregion

            #region 讀取學生清單
            _FilterStudentList = new List<StudentRecord>();
            string studentFilter = cboStudentFilter.Text.Trim('[', ']');
            string studentTag1 = cboStudentTag1.Text.Trim('[', ']');
            string studentTag2 = cboStudentTag2.Text.Trim('[', ']');
            Exception bkwException = null;
            BackgroundWorker bkw = new BackgroundWorker();
            bkw.WorkerReportsProgress = true;

            btnPrevious.Enabled = false;

            bkw.ProgressChanged += delegate (object s1, ProgressChangedEventArgs e1)
            {
                MotherForm.SetStatusBarMessage("資料載入中", e1.ProgressPercentage);
            };

            bkw.DoWork += delegate
            {
                try
                {
                    #region 依據條件篩選學生
                    bkw.ReportProgress(1);
                    foreach (string gradeYear in _CheckBoxList.Select(x => x.Text))
                    {
                        _FilterStudentList.AddRange(_StudentRecord.Where(x => x.Class.GradeYear == Convert.ToInt32(gradeYear.Trim('年', '級'))).ToList());
                    }

                    bkw.ReportProgress(50);
                    if (!string.IsNullOrEmpty(studentFilter))
                    {
                        List<string> studentFilterTagIDs = _TagConfigRecord.Where(x => x.Prefix == studentFilter).Select(x => x.ID).ToList();
                        List<string> filterStudentID = K12.Data.StudentTag.SelectAll().Where(x => studentFilterTagIDs.Contains(x.RefTagID)).Select(x => x.RefStudentID).ToList();
                        _FilterStudentList = _FilterStudentList.Where(x => !filterStudentID.Contains(x.ID)).ToList();
                    }

                    bkw.ReportProgress(100);
                    #endregion
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
                    throw new Exception("資料讀取失敗", bkwException);
                }

                if (_FilterStudentList.Count == 0)
                {
                    btnCacluate.Enabled = false;
                    MessageBox.Show("沒有找到符合條件的學生");
                    btnPrevious.Enabled = true;
                    return;
                }

                #region 將學生清單顯示在dataGridView上
                var studentView = (from s in _FilterStudentList
                                   select new
                                   {
                                       studentId = s.ID,
                                       ClassName = s.Class.Name,
                                       s.SeatNo,
                                       s.StudentNumber,
                                       s.Name,
                                       RankGradeYear = "" + s.Class.GradeYear + "年級",
                                       RankClassName = s.Class.Name,
                                   }).ToList();

                #region 取得符合類別的學生
                List<string> tag1IDs = new List<string>();
                List<string> tag2IDs = new List<string>();
                if (!string.IsNullOrEmpty(studentTag1))
                {
                    tag1IDs = _TagConfigRecord.Where(x => x.Prefix == studentTag1).Select(x => x.ID).ToList();
                }
                if (!string.IsNullOrEmpty(studentTag2))
                {
                    tag2IDs = _TagConfigRecord.Where(x => x.Prefix == studentTag2).Select(x => x.ID).ToList();
                }
                List<StudentTagRecord> studentTag1List = K12.Data.StudentTag.SelectAll().Where(x => tag1IDs.Contains(x.RefTagID)).ToList();
                List<StudentTagRecord> studentTag2List = K12.Data.StudentTag.SelectAll().Where(x => tag2IDs.Contains(x.RefTagID)).ToList();
                #endregion

                List<DataGridViewRow> rowList = new List<DataGridViewRow>();
                for (int rowIndex = 0; rowIndex < studentView.Count; rowIndex++)
                {
                    string tag1 = "", tag2 = "";
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(dgvStudentList);
                    row.Tag = studentView[rowIndex].studentId;
                    row.Cells[0].Value = studentView[rowIndex].ClassName;
                    row.Cells[1].Value = studentView[rowIndex].SeatNo;
                    row.Cells[2].Value = studentView[rowIndex].StudentNumber;
                    row.Cells[3].Value = studentView[rowIndex].Name;
                    row.Cells[4].Value = studentView[rowIndex].RankGradeYear;
                    row.Cells[5].Value = studentView[rowIndex].RankClassName;
                    if (studentTag1List.Select(x => x.RefStudentID).Contains(studentView[rowIndex].studentId))
                    {
                        tag1 = studentTag1List.First(x => x.RefStudentID == studentView[rowIndex].studentId).Name;
                        row.Cells[6].Value = tag1;
                    }
                    if (studentTag2List.Select(x => x.RefStudentID).Contains(studentView[rowIndex].studentId))
                    {
                        tag2 = studentTag2List.First(x => x.RefStudentID == studentView[rowIndex].studentId).Name;
                        row.Cells[7].Value = tag2;
                    }

                    rowList.Add(row);
                }
                dgvStudentList.Rows.AddRange(rowList.ToArray());
                btnPrevious.Enabled = true;
                #endregion
            };

            bkw.RunWorkerAsync();
            #endregion
        }

        private void btnCacluate_Click(object sender, EventArgs e)
        {
            List<string> studentSqlList = new List<string>();
            string studentListSql = "";
            foreach (DataGridViewRow row in dgvStudentList.Rows)
            {
                #region 每一筆學生先組好先加進List裡
                studentSqlList.Add(@"
    SELECT
        '" + row.Tag + @"'::BIGINT AS student_id
        ,'" + row.Cells[3].Value + @"'::TEXT AS student_name
        ,'" + ("" + row.Cells[4].Value).Trim('年', '級') + @"'::INT AS rank_grade_year
        ,'" + "" + row.Cells[5].Value + @"'::TEXT AS rank_class_name
        ,'" + "" + row.Cells[6].Value + @"'::TEXT AS rank_tag1
        ,'" + "" + row.Cells[7].Value + @"'::TEXT AS rank_tag2
    ");
                #endregion
            }

            #region 將剛剛裝學生sql的list拆開
            studentListSql = @"
WITH student_list AS 
(
    " + string.Join(@"
    UNION ALL", studentSqlList) + @"
)";
            #endregion

            btnCacluate.Enabled = false;
            btnPrevious.Enabled = false;
            string schoolYear = lbSchoolYear.Text;
            string semester = lbSemester.Text;
            string examName = lbExam.Text;
            string examId = _ExamList.First(x => x.Name == examName).ID;
            string tag1 = cboStudentTag1.Text.Trim('[', ']');
            string tag2 = cboStudentTag2.Text.Trim('[', ']');
            string studentFilter = cboStudentFilter.Text.Trim('[', ']');
            List<int> gradeYearList = new List<int>();
            foreach (CheckBox checkBox in _CheckBoxList)
            {
                gradeYearList.Add(Convert.ToInt32(checkBox.Text.Trim('年', '級')));
            }

            Exception bkwException = null;
            BackgroundWorker bkw = new BackgroundWorker();
            bkw.WorkerReportsProgress = true;
            pbLoading.Visible = true;

            bkw.ProgressChanged += delegate (object s1, ProgressChangedEventArgs e1)
            {
                MotherForm.SetStatusBarMessage("計算排名中", e1.ProgressPercentage);
            };

            bkw.DoWork += delegate
            {
                try
                {
                    bkw.ReportProgress(1);
                    List<string> rawSqlList = new List<string>();
                    string calculationSetting = "";

                    #region 產生計算設定的字串
                    XmlDocument doc = new XmlDocument();
                    var settingEle = doc.CreateElement("Setting");
                    settingEle.SetAttribute("學年度", "" + schoolYear);
                    settingEle.SetAttribute("學期", "" + semester);
                    settingEle.SetAttribute("考試名稱", "" + examName);
                    settingEle.SetAttribute("不排名學生類別", "" + studentFilter);
                    settingEle.SetAttribute("類別一", "" + tag1);
                    settingEle.SetAttribute("類別二", "" + tag2);
                    foreach (var gradeYear in gradeYearList)
                    {
                        var gradeYearEle = doc.CreateElement("年級");
                        gradeYearEle.InnerText = "" + gradeYear;
                        settingEle.AppendChild(gradeYearEle);
                    }
                    calculationSetting = settingEle.OuterXml;
                    #endregion

                    for (int index = 0; index < gradeYearList.Count; index++)
                    {
                        #region 每一筆raw(包含GradeYear, SchoolYear, Semester, ExamName)先組好加進List
                        rawSqlList.Add(@"
	SELECT
		'" + gradeYearList[index] + @"'::TEXT  AS rank_grade_year
		, '" + schoolYear + @"'::TEXT AS rank_school_year
		, '" + semester + @"'::TEXT AS rank_semester
		, '" + examName + @"'::TEXT AS rank_exam_name
        , '" + calculationSetting + @"'::TEXT AS calculation_setting
");
                        #endregion
                    }

                    bkw.ReportProgress(20);

                    #region 將剛剛組好的rawList拆開
                    string rawSql = @"
, raw AS
(
	" + string.Join(@"
    UNION ALL", rawSqlList) + @"
)";
                    #endregion

                    #region 計算排名的SQL
                    string insertRankSql = @"
" + studentListSql + @"
" + rawSql + @"
,score_detail AS
( 
	SELECT
		student_list.student_id
   		, student_list.student_name
		, sc_attend.id AS sc_attend_id
		, course.course_name
		, course.school_year AS rank_school_year
		, course.semester AS rank_semester
		, course.subject
		, course.domain
		, course.credit
		, exam_template.id AS template_id
		, exam_template.name AS template_name
		, ('0'||unnest(xpath('/Extension/ScorePercentage/text()',xmlparse(content exam_template.extension)))::text)::Decimal AS exam_weight
		, 100::Decimal- ('0'||unnest(xpath('/Extension/ScorePercentage/text()',xmlparse(content exam_template.extension)))::text)::Decimal AS assignment_weight
		, exam.id AS exam_id
		, exam.exam_name
		, student_list.rank_class_name
		, student_list.rank_grade_year
		, student_list.rank_tag1
		, student_list.rank_tag2
		,CASE
			WHEN  xpath('/Extension/Score/text()',xmlparse(content sce_take.extension)) IS NULL OR array_length(xpath('/Extension/Score/text()',xmlparse(content sce_take.extension)),1) IS NULL 
			THEN  NULL 
			ELSE 	('0'||unnest(xpath('/Extension/Score/text()',xmlparse(content sce_take.extension)))::text)::Decimal  
		 END AS exam_score
		,CASE
			WHEN	 xpath('/Extension/AssignmentScore/text()',xmlparse(content sce_take.extension)) IS NULL OR array_length(xpath('/Extension/AssignmentScore/text()',xmlparse(content sce_take.extension)),1) IS NULL 
			THEN  NULL 
		ELSE	('0'||unnest(xpath('/Extension/AssignmentScore/text()',xmlparse(content sce_take.extension)))::text)::Decimal 
		END AS assignment_score	
	FROM  sce_take
		LEFT JOIN sc_attend 
			ON ref_sc_attend_id = sc_attend.id
		LEFT JOIN exam 
			ON ref_exam_id = exam.id
		LEFT JOIN course 
			ON sc_attend.ref_course_id = course.id
		INNER JOIN student_list
			ON sc_attend.ref_student_id = student_list.student_id
		LEFT JOIN exam_template
			ON  exam_template.id = course.ref_exam_template_id
		INNER JOIN raw
			ON course.school_year = raw.rank_school_year::int
			AND course.semester = raw.rank_semester::int
			AND student_list.rank_grade_year = raw.rank_grade_year::int
			AND exam.exam_name= raw.rank_exam_name
)
,score_detail_avge AS
(
	SELECT	
		score_detail.*
		,CASE
			WHEN exam_score IS NOT NULL AND assignment_score IS NOT NULL
			THEN (
					exam_score::Decimal * exam_weight::Decimal
					+
					assignment_score::Decimal * assignment_weight::Decimal
				)/(
					exam_weight
					+
					assignment_weight
				)
			WHEN exam_score IS NOT NULL AND assignment_score IS NULL
			THEN exam_score::Decimal
			WHEN assignment_score IS NOT NULL AND exam_score IS NULL
			THEN assignment_score::Decimal
		END AS score
	FROM 
		score_detail
	WHERE 
	(
		exam_score IS NOT NULL
		OR assignment_score IS NOT NULL 
	)
	AND template_id IS NOT NULL
	AND 
	(
		exam_weight IS NOT NULL
		OR assignment_weight IS NOT NULL
	)
)
-----領域排名所需成績
,group_score AS
(
	SELECT 
		student_id
		,student_name
		,rank_school_year
		,rank_semester
		,rank_grade_year
		,rank_class_name
		,exam_id
		,domain
		,rank_tag1
		,rank_tag2
		,SUM
		(
			score_detail_avge.score::decimal * score_detail_avge.credit::decimal
		) / 
		SUM
		( 
			CASE
				WHEN score_detail_avge.credit = 0
				THEN 1
				ELSE score_detail_avge.credit::decimal
			END
		)AS domain_score
	FROM  
		score_detail_avge
	WHERE 
		score_detail_avge.score IS NOT NULL
	GROUP BY
		domain,rank_school_year, rank_semester, rank_grade_year, rank_class_name, exam_id, student_id, student_name, rank_tag1, rank_tag2
)
------加權平均排名所需成績
,scoreWavge AS
(
	SELECT
		student_id
		,student_name
		,rank_school_year
		,rank_semester
		,rank_grade_year
		,rank_class_name
		,exam_id
		,rank_tag1
		,rank_tag2
		,SUM
		(
			score_detail_avge.score::decimal * score_detail_avge.credit::decimal 
		) / 
		SUM
		( 
			CASE
				WHEN score_detail_avge.credit = 0
				THEN 1
				ELSE score_detail_avge.credit::decimal
			END
		)  AS avge
	FROM 
		score_detail_avge
	WHERE
		score_detail_avge.score IS NOT NULL
	GROUP BY 
		student_id, student_name, rank_school_year, rank_semester, rank_grade_year, rank_class_name, exam_id, rank_tag1, rank_tag2
)
-------計算領域排名
,domain_rank_raw AS
(
	SELECT
		group_score.student_id
		, group_score.rank_tag1
		, group_score.rank_tag2
		, '定期評量/領域成績'::TEXT AS item_type
		, group_score.domain::TEXT AS item_name
		, group_score.rank_school_year
		, group_score.rank_semester
		, group_score.rank_grade_year
		, group_score.rank_class_name
		, group_score.exam_id
		, group_score.domain_score AS score
		, RANK() OVER(PARTITION BY group_score.rank_grade_year ,group_score.domain ORDER BY group_score.domain_score DESC) AS grade_rank
		, RANK() OVER(PARTITION BY group_score.rank_class_name ,group_score.domain ORDER BY group_score.domain_score DESC) AS class_rank
		, RANK() OVER(PARTITION BY group_score.rank_grade_year, group_score.rank_tag1, group_score.domain ORDER BY group_score.domain_score DESC) AS tag1_rank
		, RANK() OVER(PARTITION BY group_score.rank_grade_year, group_score.rank_tag2, group_score.domain ORDER BY group_score.domain_score DESC) AS tag2_rank
		, COUNT (group_score.student_id) OVER(PARTITION BY group_score.rank_grade_year ,group_score.domain ) AS grade_count
		, COUNT (group_score.student_id) OVER(PARTITION BY group_score.rank_class_name, group_score.domain) AS class_count
		, COUNT (group_score.student_id) OVER(PARTITION BY group_score.rank_grade_year, group_score.rank_tag1, group_score.domain) AS tag1_count
		, COUNT (group_score.student_id) OVER(PARTITION BY group_score.rank_grade_year, group_score.rank_tag2, group_score.domain) AS tag2_count
	FROM 
		group_score
	WHERE
		group_score.domain IS NOT NULL
)
, domain_rank_expand AS 
(
	SELECT  
		domain_rank_raw.*
		,FLOOR((grade_rank::DECIMAL-1)*100::DECIMAL / grade_count)+1 AS graderank_percentage
		,FLOOR((class_rank::DECIMAL-1)*100::DECIMAL / class_count)+1 AS classrank_percentage
		,FLOOR((tag1_rank::DECIMAL-1)*100::DECIMAL / tag1_count)+1 AS tag1rank_percentage
		,FLOOR((tag2_rank::DECIMAL-1)*100::DECIMAL / tag1_count)+1 AS tag2rank_percentage
	FROM 
		domain_rank_raw
)
--------計算科目排名
,subject_rank_raw AS
(
	SELECT
		score_detail_avge.student_id
		, score_detail_avge.rank_tag1
		, score_detail_avge.rank_tag2
		, '定期評量/科目成績'::TEXT AS item_type
		, score_detail_avge.subject AS item_name
		, score_detail_avge.rank_school_year
		, score_detail_avge.rank_semester
		, score_detail_avge.rank_grade_year
		, score_detail_avge.rank_class_name
		, score_detail_avge.exam_id
		, score_detail_avge.score
		, RANK() OVER(PARTITION BY score_detail_avge.rank_grade_year,score_detail_avge.subject ORDER BY score DESC) AS grade_rank
		, RANK() OVER(PARTITION BY score_detail_avge.rank_class_name ,score_detail_avge.subject ORDER BY score DESC) AS class_rank
		, RANK() OVER(PARTITION BY score_detail_avge.rank_grade_year, rank_tag1, score_detail_avge.subject ORDER BY score DESC) AS tag1_rank
		, RANK() OVER(PARTITION BY score_detail_avge.rank_grade_year, rank_tag2, score_detail_avge.subject ORDER BY score DESC) AS tag2_rank
		, COUNT (score_detail_avge.student_id) OVER(PARTITION BY score_detail_avge.rank_grade_year,score_detail_avge.subject ) AS grade_count
		, COUNT (score_detail_avge.student_id) OVER(PARTITION BY score_detail_avge.rank_class_name, score_detail_avge.subject) AS class_count
		, COUNT (score_detail_avge.student_id) OVER(PARTITION BY score_detail_avge.rank_grade_year, rank_tag1, score_detail_avge.subject) AS tag1_count
		, COUNT (score_detail_avge.student_id) OVER(PARTITION BY score_detail_avge.rank_grade_year, rank_tag2, score_detail_avge.subject) AS tag2_count
	FROM score_detail_avge
	WHERE score_detail_avge.subject IS NOT NULL
)
, subject_rank_expand AS 
(
	SELECT  
		subject_rank_raw.*
		,FLOOR((grade_rank::DECIMAL-1)*100::DECIMAL/grade_count)+1 AS graderank_percentage
		,FLOOR((class_rank::DECIMAL-1)*100::DECIMAL/class_count)+1 AS classrank_percentage
		,FLOOR((tag1_rank::DECIMAL-1)*100::DECIMAL/tag1_count)+1 AS tag1rank_percentage
		,FLOOR((tag2_rank::DECIMAL-1)*100::DECIMAL/tag2_count)+1 AS tag2rank_percentage
	FROM 
		subject_rank_raw
)
-----------計算加權平均排名
,weigth_rank_raw AS
(
	SELECT 
		scoreWavge.student_id
		, scoreWavge.rank_tag1
		, scoreWavge.rank_tag2
		, '定期評量/總計成績'::text AS item_type
		, '加權平均'::TEXT As item_name
		, scoreWavge.rank_school_year
		, scoreWavge.rank_semester
		, scoreWavge.rank_grade_year
		, scoreWavge.rank_class_name
		, scoreWavge.exam_id
		, scoreWavge.avge AS score
		, RANK() OVER(PARTITION BY scoreWavge.rank_grade_year ORDER BY avge DESC) AS grade_rank
		, RANK() OVER(PARTITION BY scoreWavge.rank_class_name ORDER BY avge DESC) AS class_rank
		, RANK() OVER(PARTITION BY scoreWavge.rank_grade_year, scoreWavge.rank_tag1 ORDER BY avge DESC) AS tag1_rank
		, RANK() OVER(PARTITION BY scoreWavge.rank_grade_year, scoreWavge.rank_tag2 ORDER BY avge DESC) AS tag2_rank
		, COUNT (*) OVER(PARTITION BY scoreWavge.rank_grade_year) AS grade_count
		, COUNT (*) OVER(PARTITION BY scoreWavge.rank_class_name) AS class_count
		, COUNT (*) OVER(PARTITION BY scoreWavge.rank_grade_year, scoreWavge.rank_tag1) AS tag1_count
		, COUNT (*) OVER(PARTITION BY scoreWavge.rank_grade_year, scoreWavge.rank_tag2) AS tag2_count
	FROM 
		scoreWavge
)
, weigth_rank_expand AS
(
	SELECT  
		weigth_rank_raw.*
		,FLOOR((grade_rank::DECIMAL-1)*100::DECIMAL/grade_count)+1 AS graderank_percentage
		,FLOOR((class_rank::DECIMAL-1)*100::DECIMAL/class_count)+1 AS classrank_percentage
		,FLOOR((tag1_rank::DECIMAL-1)*100::DECIMAL/tag1_count)+1 AS tag1rank_percentage
		,FLOOR((tag2_rank::DECIMAL-1)*100::DECIMAL/tag2_count)+1 AS tag2rank_percentage
	FROM 
		weigth_rank_raw
)
, score_list AS
(
	SELECT
		rank_school_year 
		, rank_semester
		, rank_grade_year
		, item_type
		, exam_id AS ref_exam_id
		, item_name
		, '年排名'::TEXT AS rank_type
		, '' || rank_grade_year || '年級'::TEXT AS rank_name
		, true AS is_alive
		, grade_count AS matrix_count
		, AVG(domain_rank_expand.Score::Decimal) FILTER(WHERE grade_rank <= TRUNC(grade_count * 0.25)) OVER(PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.item_name) AS avg_top_25
		, AVG(domain_rank_expand.Score::Decimal) FILTER(WHERE grade_rank <= TRUNC(grade_count * 0.5)) OVER(PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.item_name) AS avg_top_50
		, AVG(domain_rank_expand.Score::Decimal) OVER(PARTITION BY domain_rank_expand.rank_grade_year,domain_rank_expand.item_name) AS avg
		, AVG(domain_rank_expand.Score::Decimal) FILTER(WHERE grade_rank >= TRUNC(grade_count * 0.5)) OVER(PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.item_name) AS avg_bottom_50
		, AVG(domain_rank_expand.Score::Decimal) FILTER(WHERE grade_rank >= TRUNC(grade_count * 0.75)) OVER(PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.item_name) AS avg_bottom_25
		, COUNT(*) FILTER (WHERE 100::DECIMAL<=domain_rank_expand.score::DECIMAL ) OVER(PARTITION BY domain_rank_expand.rank_grade_year,domain_rank_expand.item_name)AS level_gte100 
		, COUNT(*) FILTER (WHERE 90::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <100::DECIMAL) OVER(PARTITION BY domain_rank_expand.rank_grade_year,domain_rank_expand.item_name) AS level_90
		, COUNT(*) FILTER (WHERE 80::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <90::DECIMAL)  OVER(PARTITION BY domain_rank_expand.rank_grade_year,domain_rank_expand.item_name) AS level_80
		, COUNT(*) FILTER (WHERE 70::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <80::DECIMAL)  OVER(PARTITION BY domain_rank_expand.rank_grade_year,domain_rank_expand.item_name)AS level_70
		, COUNT(*) FILTER (WHERE 60::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <70::DECIMAL)  OVER(PARTITION BY domain_rank_expand.rank_grade_year,domain_rank_expand.item_name) AS level_60
		, COUNT(*) FILTER (WHERE 50::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <60::DECIMAL)  OVER(PARTITION BY domain_rank_expand.rank_grade_year,domain_rank_expand.item_name)AS level_50
		, COUNT(*) FILTER (WHERE 40::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <50::DECIMAL)  OVER(PARTITION BY domain_rank_expand.rank_grade_year,domain_rank_expand.item_name)AS level_40
		, COUNT(*) FILTER (WHERE 30::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <40::DECIMAL)  OVER(PARTITION BY domain_rank_expand.rank_grade_year,domain_rank_expand.item_name)AS level_30
		, COUNT(*) FILTER (WHERE 20::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <30::DECIMAL)  OVER(PARTITION BY domain_rank_expand.rank_grade_year,domain_rank_expand.item_name) AS level_20
		, COUNT(*) FILTER (WHERE 10::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <20::DECIMAL)  OVER(PARTITION BY domain_rank_expand.rank_grade_year,domain_rank_expand.item_name)AS level_10
		, COUNT(*) FILTER (WHERE domain_rank_expand.score<10::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_grade_year,domain_rank_expand.item_name)AS level_lt10 
		, student_id
		, score
		, grade_rank AS rank
		, graderank_percentage AS percentile
		, rank_class_name
		, rank_tag1
		, rank_tag2
	FROM
		domain_rank_expand
	UNION ALL
	SELECT
		rank_school_year 
		, rank_semester
		, rank_grade_year
		, item_type
		, exam_id AS ref_exam_id
		, item_name
		, '班排名'::TEXT AS rank_type
		, rank_class_name AS rank_name
		, true AS is_alive
		, class_count AS matrix_count
		, AVG(domain_rank_expand.Score::Decimal) FILTER(WHERE class_rank <= TRUNC(class_count * 0.25)) OVER(PARTITION BY domain_rank_expand.rank_class_name,domain_rank_expand.item_name) AS avg_top_25
		, AVG(domain_rank_expand.Score::Decimal) FILTER(WHERE class_rank <= TRUNC(class_count * 0.5)) OVER(PARTITION BY domain_rank_expand.rank_class_name,domain_rank_expand.item_name) AS avg_top_50
		, AVG(domain_rank_expand.Score::Decimal) OVER(PARTITION BY domain_rank_expand.rank_class_name,domain_rank_expand.item_name) AS avg
		, AVG(domain_rank_expand.Score::Decimal) FILTER(WHERE class_rank >= TRUNC(class_count * 0.5)) OVER(PARTITION BY domain_rank_expand.rank_class_name,domain_rank_expand.item_name) AS avg_bottom_50
		, AVG(domain_rank_expand.Score::Decimal) FILTER(WHERE class_rank >= TRUNC(class_count * 0.75)) OVER(PARTITION BY domain_rank_expand.rank_class_name,domain_rank_expand.item_name) AS avg_bottom_25
		, COUNT(*) FILTER (WHERE 100::DECIMAL<=domain_rank_expand.score::DECIMAL ) OVER(PARTITION BY domain_rank_expand.rank_class_name,domain_rank_expand.item_name)AS level_gte100 
		, COUNT(*) FILTER (WHERE 90::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <100::DECIMAL)OVER (PARTITION BY domain_rank_expand.rank_class_name,domain_rank_expand.item_name) AS level_90
		, COUNT(*) FILTER (WHERE 80::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <90::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_class_name,domain_rank_expand.item_name) AS level_80
		, COUNT(*) FILTER (WHERE 70::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <80::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_class_name,domain_rank_expand.item_name)AS level_70
		, COUNT(*) FILTER (WHERE 60::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <70::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_class_name,domain_rank_expand.item_name) AS level_60
		, COUNT(*) FILTER (WHERE 50::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <60::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_class_name,domain_rank_expand.item_name)AS level_50
		, COUNT(*) FILTER (WHERE 40::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <50::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_class_name,domain_rank_expand.item_name)AS level_40
		, COUNT(*) FILTER (WHERE 30::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <40::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_class_name,domain_rank_expand.item_name)AS level_30
		, COUNT(*) FILTER (WHERE 20::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <30::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_class_name,domain_rank_expand.item_name) AS level_20
		, COUNT(*) FILTER (WHERE 10::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <20::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_class_name,domain_rank_expand.item_name)AS level_10
		, COUNT(*) FILTER (WHERE domain_rank_expand.score<10::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_class_name,domain_rank_expand.item_name)AS level_lt10 
		, student_id
		, score
		, class_rank AS rank
		, classrank_percentage AS percentile
		, rank_class_name
		, rank_tag1
		, rank_tag2
	FROM
		domain_rank_expand
	UNION ALL
	SELECT
		rank_school_year 
		, rank_semester
		, rank_grade_year
		, item_type
		, exam_id AS ref_exam_id
		, item_name
		, '類別1排名'::TEXT AS rank_type
		, rank_tag1 AS rank_name
		, true AS is_alive
		, tag1_count AS matrix_count
		, AVG(domain_rank_expand.Score::Decimal)FILTER(WHERE tag1_rank <= TRUNC(tag1_count * 0.25)) OVER(PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag1, domain_rank_expand.item_name) AS avg_top_25
		, AVG(domain_rank_expand.Score::Decimal)FILTER(WHERE tag1_rank <= TRUNC(tag1_count * 0.5)) OVER(PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag1, domain_rank_expand.item_name) AS avg_top_50
		, AVG(domain_rank_expand.Score::Decimal)OVER(PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag1, domain_rank_expand.item_name) AS avg
		, AVG(domain_rank_expand.Score::Decimal)FILTER(WHERE tag1_rank >= TRUNC(tag1_count * 0.5)) OVER(PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag1, domain_rank_expand.item_name) AS avg_bottom_50
		, AVG(domain_rank_expand.Score::Decimal)FILTER(WHERE tag1_rank >= TRUNC(tag1_count * 0.75)) OVER(PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag1, domain_rank_expand.item_name) AS avg_bottom_25
		, COUNT(*) FILTER (WHERE 100::DECIMAL<=domain_rank_expand.score::DECIMAL ) OVER(PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag1, domain_rank_expand.item_name)AS level_gte100 
		, COUNT(*) FILTER (WHERE 90::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <100::DECIMAL)OVER (PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag1, domain_rank_expand.item_name) AS level_90
		, COUNT(*) FILTER (WHERE 80::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <90::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag1, domain_rank_expand.item_name) AS level_80
		, COUNT(*) FILTER (WHERE 70::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <80::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag1, domain_rank_expand.item_name)AS level_70
		, COUNT(*) FILTER (WHERE 60::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <70::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag1, domain_rank_expand.item_name) AS level_60
		, COUNT(*) FILTER (WHERE 50::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <60::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag1, domain_rank_expand.item_name)AS level_50
		, COUNT(*) FILTER (WHERE 40::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <50::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag1, domain_rank_expand.item_name)AS level_40
		, COUNT(*) FILTER (WHERE 30::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <40::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag1, domain_rank_expand.item_name)AS level_30
		, COUNT(*) FILTER (WHERE 20::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <30::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag1, domain_rank_expand.item_name) AS level_20
		, COUNT(*) FILTER (WHERE 10::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <20::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag1, domain_rank_expand.item_name)AS level_10
		, COUNT(*) FILTER (WHERE domain_rank_expand.score<10::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag1, domain_rank_expand.item_name)AS level_lt10 
		, student_id
		, score
		, tag1_rank AS rank
		, tag1rank_percentage AS percentile
		, rank_class_name
		, rank_tag1
		, rank_tag2
	FROM
		domain_rank_expand
	WHERE domain_rank_expand.rank_tag1 IS NOT NULL
	AND domain_rank_expand.rank_tag1 <> ''
	UNION ALL
	SELECT
		rank_school_year 
		, rank_semester
		, rank_grade_year
		, item_type
		, exam_id AS ref_exam_id
		, item_name
		, '類別2排名'::TEXT AS rank_type
		, rank_tag2 AS rank_name
		, true AS is_alive
		, tag2_count AS matrix_count
		, AVG(domain_rank_expand.Score::Decimal)FILTER(WHERE tag2_rank <= TRUNC(tag2_count * 0.25)) OVER(PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag2, domain_rank_expand.item_name) AS avg_top_25
		, AVG(domain_rank_expand.Score::Decimal)FILTER(WHERE tag2_rank <= TRUNC(tag2_count * 0.5)) OVER(PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag2, domain_rank_expand.item_name) AS avg_top_50
		, AVG(domain_rank_expand.Score::Decimal)OVER(PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag2, domain_rank_expand.item_name) AS avg
		, AVG(domain_rank_expand.Score::Decimal)FILTER(WHERE tag2_rank >= TRUNC(tag2_count * 0.5)) OVER(PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag2, domain_rank_expand.item_name) AS avg_bottom_50
		, AVG(domain_rank_expand.Score::Decimal)FILTER(WHERE tag2_rank >= TRUNC(tag2_count * 0.75)) OVER(PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag2, domain_rank_expand.item_name) AS avg_bottom_25
		, COUNT(*) FILTER (WHERE 100::DECIMAL<=domain_rank_expand.score::DECIMAL ) OVER(PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag2, domain_rank_expand.item_name)AS level_gte100 
		, COUNT(*) FILTER (WHERE 90::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <100::DECIMAL)OVER (PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag2, domain_rank_expand.item_name) AS level_90
		, COUNT(*) FILTER (WHERE 80::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <90::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag2, domain_rank_expand.item_name) AS level_80
		, COUNT(*) FILTER (WHERE 70::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <80::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag2, domain_rank_expand.item_name)AS level_70
		, COUNT(*) FILTER (WHERE 60::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <70::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag2, domain_rank_expand.item_name) AS level_60
		, COUNT(*) FILTER (WHERE 50::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <60::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag2, domain_rank_expand.item_name)AS level_50
		, COUNT(*) FILTER (WHERE 40::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <50::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag2, domain_rank_expand.item_name)AS level_40
		, COUNT(*) FILTER (WHERE 30::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <40::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag2, domain_rank_expand.item_name)AS level_30
		, COUNT(*) FILTER (WHERE 20::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <30::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag2, domain_rank_expand.item_name) AS level_20
		, COUNT(*) FILTER (WHERE 10::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <20::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag2, domain_rank_expand.item_name)AS level_10
		, COUNT(*) FILTER (WHERE domain_rank_expand.score<10::DECIMAL) OVER (PARTITION BY domain_rank_expand.rank_grade_year, domain_rank_expand.rank_tag2, domain_rank_expand.item_name)AS level_lt10 
		, student_id
		, score
		, tag2_rank AS rank
		, tag2rank_percentage AS percentile
		, rank_class_name
		, rank_tag1
		, rank_tag2
	FROM
		domain_rank_expand
	WHERE 
		domain_rank_expand.rank_tag2 IS NOT NULL
	AND domain_rank_expand.rank_tag2 <> ''
	UNION ALL
	SELECT
		rank_school_year 
		, rank_semester
		, rank_grade_year
		, item_type
		, exam_id AS ref_exam_id
		, item_name
		, '年排名'::TEXT AS rank_type
		, '' || rank_grade_year || '年級'::TEXT AS rank_name
		, true AS is_alive
		, grade_count AS matrix_count
		, AVG(subject_rank_expand.Score::Decimal)FILTER(WHERE grade_rank <= TRUNC(grade_count * 0.25)) OVER(PARTITION BY subject_rank_expand.rank_grade_year,subject_rank_expand.item_name) AS avg_top_25
		, AVG(subject_rank_expand.Score::Decimal)FILTER(WHERE grade_rank <= TRUNC(grade_count * 0.5)) OVER(PARTITION BY subject_rank_expand.rank_grade_year,subject_rank_expand.item_name) AS avg_top_50
		, AVG(subject_rank_expand.Score::Decimal)OVER(PARTITION BY subject_rank_expand.rank_grade_year,subject_rank_expand.item_name) AS avg
		, AVG(subject_rank_expand.Score::Decimal)FILTER(WHERE grade_rank >= TRUNC(grade_count * 0.5)) OVER(PARTITION BY subject_rank_expand.rank_grade_year,subject_rank_expand.item_name) AS avg_bottom_50
		, AVG(subject_rank_expand.Score::Decimal)FILTER(WHERE grade_rank >= TRUNC(grade_count * 0.75)) OVER(PARTITION BY subject_rank_expand.rank_grade_year,subject_rank_expand.item_name) AS avg_bottom_25
		, COUNT(*) FILTER (WHERE 100::DECIMAL<=subject_rank_expand.score::DECIMAL ) OVER(PARTITION BY subject_rank_expand.rank_grade_year,subject_rank_expand.item_name)AS level_gte100 
		, COUNT(*) FILTER (WHERE 90::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <100::DECIMAL)  OVER(PARTITION BY subject_rank_expand.rank_grade_year,subject_rank_expand.item_name) AS level_90
		, COUNT(*) FILTER (WHERE 80::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <90::DECIMAL)  OVER(PARTITION BY subject_rank_expand.rank_grade_year,subject_rank_expand.item_name) AS level_80
		, COUNT(*) FILTER (WHERE 70::DECIMAL<=subject_rank_expand.score AND  subject_rank_expand.score <80::DECIMAL)  OVER(PARTITION BY subject_rank_expand.rank_grade_year,subject_rank_expand.item_name)AS level_70
		, COUNT(*) FILTER (WHERE 60::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <70::DECIMAL) OVER(PARTITION BY subject_rank_expand.rank_grade_year,subject_rank_expand.item_name) AS level_60
		, COUNT(*) FILTER (WHERE 50::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <60::DECIMAL)  OVER (PARTITION BY subject_rank_expand.rank_grade_year,subject_rank_expand.item_name)AS level_50
		, COUNT(*) FILTER (WHERE 40::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <50::DECIMAL) OVER (PARTITION BY subject_rank_expand.rank_grade_year,subject_rank_expand.item_name)AS level_40
		, COUNT(*) FILTER (WHERE 30::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <40::DECIMAL)   OVER (PARTITION BY subject_rank_expand.rank_grade_year,subject_rank_expand.item_name)AS level_30
		, COUNT(*) FILTER (WHERE 20::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <30::DECIMAL)  OVER (PARTITION BY subject_rank_expand.rank_grade_year,subject_rank_expand.item_name) AS level_20
		, COUNT(*) FILTER (WHERE 10::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <20::DECIMAL)   OVER (PARTITION BY subject_rank_expand.rank_grade_year,subject_rank_expand.item_name)AS level_10
		, COUNT(*) FILTER (WHERE subject_rank_expand.score<10::DECIMAL) OVER (PARTITION BY subject_rank_expand.rank_grade_year,subject_rank_expand.item_name)AS level_lt10
		, student_id
		, score
		, grade_rank AS rank
		, graderank_percentage AS percentile
		, rank_class_name
		, rank_tag1
		, rank_tag2
	FROM
		subject_rank_expand
	UNION ALL
	SELECT
		rank_school_year 
		, rank_semester
		, rank_grade_year
		, item_type
		, exam_id AS ref_exam_id
		, item_name
		, '班排名'::TEXT AS rank_type
		, rank_class_name AS rank_name
		, true AS is_alive
		, class_count AS matrix_count
		, AVG(subject_rank_expand.Score::Decimal)FILTER(WHERE class_rank <= TRUNC(class_count * 0.25)) OVER(PARTITION BY subject_rank_expand.rank_class_name,subject_rank_expand.item_name) AS avg_top_25
		, AVG(subject_rank_expand.Score::Decimal)FILTER(WHERE class_rank <= TRUNC(class_count * 0.5)) OVER(PARTITION BY subject_rank_expand.rank_class_name,subject_rank_expand.item_name) AS avg_top_50
		, AVG(subject_rank_expand.Score::Decimal)OVER(PARTITION BY subject_rank_expand.rank_class_name,subject_rank_expand.item_name) AS avg
		, AVG(subject_rank_expand.Score::Decimal)FILTER(WHERE class_rank >= TRUNC(class_count * 0.5)) OVER(PARTITION BY subject_rank_expand.rank_class_name,subject_rank_expand.item_name) AS avg_bottom_50
		, AVG(subject_rank_expand.Score::Decimal)FILTER(WHERE class_rank >= TRUNC(class_count * 0.75)) OVER(PARTITION BY subject_rank_expand.rank_class_name,subject_rank_expand.item_name) AS avg_bottom_25
		, COUNT(*) FILTER (WHERE 100::DECIMAL<=subject_rank_expand.score::DECIMAL ) OVER(PARTITION BY subject_rank_expand.rank_class_name,subject_rank_expand.item_name)AS level_gte100 
		, COUNT(*) FILTER (WHERE 90::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <100::DECIMAL)  OVER(PARTITION BY subject_rank_expand.rank_class_name,subject_rank_expand.item_name) AS level_90
		, COUNT(*) FILTER (WHERE 80::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <90::DECIMAL)  OVER(PARTITION BY subject_rank_expand.rank_class_name,subject_rank_expand.item_name) AS level_80
		, COUNT(*) FILTER (WHERE 70::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <80::DECIMAL)  OVER(PARTITION BY subject_rank_expand.rank_class_name,subject_rank_expand.item_name)AS level_70
		, COUNT(*) FILTER (WHERE 60::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <70::DECIMAL) OVER(PARTITION BY subject_rank_expand.rank_class_name,subject_rank_expand.item_name) AS level_60
		, COUNT(*) FILTER (WHERE 50::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <60::DECIMAL)  OVER (PARTITION BY subject_rank_expand.rank_class_name,subject_rank_expand.item_name)AS level_50
		, COUNT(*) FILTER (WHERE 40::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <50::DECIMAL) OVER (PARTITION BY subject_rank_expand.rank_class_name,subject_rank_expand.item_name)AS level_40
		, COUNT(*) FILTER (WHERE 30::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <40::DECIMAL)   OVER (PARTITION BY subject_rank_expand.rank_class_name,subject_rank_expand.item_name)AS level_30
		, COUNT(*) FILTER (WHERE 20::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <30::DECIMAL)  OVER (PARTITION BY subject_rank_expand.rank_class_name,subject_rank_expand.item_name) AS level_20
		, COUNT(*) FILTER (WHERE 10::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <20::DECIMAL)   OVER (PARTITION BY subject_rank_expand.rank_class_name,subject_rank_expand.item_name)AS level_10
		, COUNT(*) FILTER (WHERE subject_rank_expand.score<10::DECIMAL) OVER (PARTITION BY subject_rank_expand.rank_class_name,subject_rank_expand.item_name)AS level_lt10 
		, student_id
		, score
		, class_rank AS rank
		, classrank_percentage AS percentile
		, rank_class_name
		, rank_tag1
		, rank_tag2
	FROM
		subject_rank_expand
	UNION ALL
	SELECT
		rank_school_year 
		, rank_semester
		, rank_grade_year
		, item_type
		, exam_id AS ref_exam_id
		, item_name
		, '類別1排名'::TEXT AS rank_type
		, rank_tag1 AS rank_name
		, true AS is_alive
		, tag1_count AS matrix_count
		, AVG(subject_rank_expand.Score::Decimal)FILTER(WHERE tag1_rank <= TRUNC(tag1_count * 0.25)) OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag1, subject_rank_expand.item_name) AS avg_top_25
		, AVG(subject_rank_expand.Score::Decimal)FILTER(WHERE tag1_rank <= TRUNC(tag1_count * 0.5)) OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag1, subject_rank_expand.item_name) AS avg_top_50
		, AVG(subject_rank_expand.Score::Decimal)OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag1, subject_rank_expand.item_name) AS avg
		, AVG(subject_rank_expand.Score::Decimal)FILTER(WHERE tag1_rank >= TRUNC(tag1_count * 0.5)) OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag1, subject_rank_expand.item_name) AS avg_bottom_50
		, AVG(subject_rank_expand.Score::Decimal)FILTER(WHERE tag1_rank >= TRUNC(tag1_count * 0.75)) OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag1, subject_rank_expand.item_name) AS avg_bottom_25
		, COUNT(*) FILTER (WHERE 100::DECIMAL<=subject_rank_expand.score::DECIMAL ) OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag1, subject_rank_expand.item_name)AS level_gte100 
		, COUNT(*) FILTER (WHERE 90::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <100::DECIMAL)OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag1, subject_rank_expand.item_name) AS level_90
		, COUNT(*) FILTER (WHERE 80::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <90::DECIMAL) OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag1, subject_rank_expand.item_name) AS level_80
		, COUNT(*) FILTER (WHERE 70::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <80::DECIMAL)OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag1, subject_rank_expand.item_name)AS level_70
		, COUNT(*) FILTER (WHERE 60::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <70::DECIMAL) OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag1, subject_rank_expand.item_name) AS level_60
		, COUNT(*) FILTER (WHERE 50::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <60::DECIMAL) OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag1, subject_rank_expand.item_name)AS level_50
		, COUNT(*) FILTER (WHERE 40::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <50::DECIMAL) OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag1, subject_rank_expand.item_name)AS level_40
		, COUNT(*) FILTER (WHERE 30::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <40::DECIMAL) OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag1, subject_rank_expand.item_name)AS level_30
		, COUNT(*) FILTER (WHERE 20::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <30::DECIMAL) OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag1, subject_rank_expand.item_name) AS level_20
		, COUNT(*) FILTER (WHERE 10::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <20::DECIMAL) OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag1, subject_rank_expand.item_name)AS level_10
		, COUNT(*) FILTER (WHERE subject_rank_expand.score<10::DECIMAL) OVER (PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag1, subject_rank_expand.item_name)AS level_lt10
		, student_id
		, score
		, tag1_rank AS rank
		, tag1rank_percentage AS percentile
		, rank_class_name
		, rank_tag1
		, rank_tag2
	FROM
		subject_rank_expand
	WHERE
		subject_rank_expand.rank_tag1 IS NOT NULL
		AND subject_rank_expand.rank_tag1 <> ''
	UNION ALL
	SELECT
		rank_school_year 
		, rank_semester
		, rank_grade_year
		, item_type
		, exam_id AS ref_exam_id
		, item_name
		, '類別2排名'::TEXT AS rank_type
		, rank_tag2 AS rank_name
		, true AS is_alive
		, tag2_count AS matrix_count
		, AVG(subject_rank_expand.Score::Decimal)FILTER(WHERE tag2_rank <= TRUNC(tag2_count * 0.25)) OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag2, subject_rank_expand.item_name) AS avg_top_25
		, AVG(subject_rank_expand.Score::Decimal)FILTER(WHERE tag2_rank <= TRUNC(tag2_count * 0.5)) OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag2, subject_rank_expand.item_name) AS avg_top_50
		, AVG(subject_rank_expand.Score::Decimal)OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag2, subject_rank_expand.item_name) AS avg
		, AVG(subject_rank_expand.Score::Decimal)FILTER(WHERE tag2_rank >= TRUNC(tag2_count * 0.5)) OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag2, subject_rank_expand.item_name) AS avg_bottom_50
		, AVG(subject_rank_expand.Score::Decimal)FILTER(WHERE tag2_rank >= TRUNC(tag2_count * 0.75)) OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag2, subject_rank_expand.item_name) AS avg_bottom_25
		, COUNT(*) FILTER (WHERE 100::DECIMAL<=subject_rank_expand.score::DECIMAL ) OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag2, subject_rank_expand.item_name)AS level_gte100 
		, COUNT(*) FILTER (WHERE 90::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <100::DECIMAL)OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag2, subject_rank_expand.item_name) AS level_90
		, COUNT(*) FILTER (WHERE 80::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <90::DECIMAL) OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag2, subject_rank_expand.item_name) AS level_80
		, COUNT(*) FILTER (WHERE 70::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <80::DECIMAL)OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag2, subject_rank_expand.item_name)AS level_70
		, COUNT(*) FILTER (WHERE 60::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <70::DECIMAL) OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag2, subject_rank_expand.item_name) AS level_60
		, COUNT(*) FILTER (WHERE 50::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <60::DECIMAL) OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag2, subject_rank_expand.item_name)AS level_50
		, COUNT(*) FILTER (WHERE 40::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <50::DECIMAL) OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag2, subject_rank_expand.item_name)AS level_40
		, COUNT(*) FILTER (WHERE 30::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <40::DECIMAL) OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag2, subject_rank_expand.item_name)AS level_30
		, COUNT(*) FILTER (WHERE 20::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <30::DECIMAL) OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag2, subject_rank_expand.item_name) AS level_20
		, COUNT(*) FILTER (WHERE 10::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <20::DECIMAL) OVER(PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag2, subject_rank_expand.item_name)AS level_10
		, COUNT(*) FILTER (WHERE subject_rank_expand.score<10::DECIMAL) OVER (PARTITION BY subject_rank_expand.rank_grade_year, subject_rank_expand.rank_tag2, subject_rank_expand.item_name)AS level_lt10
		, student_id
		, score
		, tag2_rank AS rank
		, tag2rank_percentage AS percentile
		, rank_class_name
		, rank_tag1
		, rank_tag2
	FROM
		subject_rank_expand
	WHERE
		subject_rank_expand.rank_tag2 IS NOT NULL
		AND subject_rank_expand.rank_tag2 <> ''
	UNION ALL
	SELECT
		rank_school_year 
		, rank_semester
		, rank_grade_year
		, item_type
		, exam_id AS ref_exam_id
		, item_name
		, '年排名'::TEXT AS rank_type
		, '' || rank_grade_year || '年級'::TEXT AS rank_name
		, true AS is_alive
		, grade_count AS matrix_count
		, AVG(weigth_rank_expand.Score::Decimal)FILTER(WHERE grade_rank <= TRUNC(grade_count * 0.25)) OVER(PARTITION BY weigth_rank_expand.rank_grade_year) AS avg_top_25
		, AVG(weigth_rank_expand.Score::Decimal)FILTER(WHERE grade_rank <= TRUNC(grade_count * 0.5)) OVER(PARTITION BY weigth_rank_expand.rank_grade_year) AS avg_top_50
		, AVG(weigth_rank_expand.Score::Decimal)OVER(PARTITION BY weigth_rank_expand.rank_grade_year) AS avg
		, AVG(weigth_rank_expand.Score::Decimal)FILTER(WHERE grade_rank >= TRUNC(grade_count * 0.5)) OVER(PARTITION BY weigth_rank_expand.rank_grade_year) AS avg_bottom_50
		, AVG(weigth_rank_expand.Score::Decimal)FILTER(WHERE grade_rank >= TRUNC(grade_count * 0.75)) OVER(PARTITION BY weigth_rank_expand.rank_grade_year) AS avg_bottom_25
		, COUNT(*) FILTER (WHERE 100::DECIMAL<=weigth_rank_expand.score::DECIMAL ) OVER(PARTITION BY weigth_rank_expand.rank_grade_year)AS level_gte100 
		, COUNT(*) FILTER (WHERE 90::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <100::DECIMAL)  OVER(PARTITION BY weigth_rank_expand.rank_grade_year) AS level_90
		, COUNT(*) FILTER (WHERE 80::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <90::DECIMAL)  OVER(PARTITION BY weigth_rank_expand.rank_grade_year) AS level_80
		, COUNT(*) FILTER (WHERE 70::DECIMAL<=weigth_rank_expand.score AND  weigth_rank_expand.score <80::DECIMAL)  OVER(PARTITION BY weigth_rank_expand.rank_grade_year)AS level_70
		, COUNT(*) FILTER (WHERE 60::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <70::DECIMAL) OVER(PARTITION BY weigth_rank_expand.rank_grade_year) AS level_60
		, COUNT(*) FILTER (WHERE 50::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <60::DECIMAL)  OVER (PARTITION BY weigth_rank_expand.rank_grade_year)AS level_50
		, COUNT(*) FILTER (WHERE 40::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <50::DECIMAL) OVER (PARTITION BY weigth_rank_expand.rank_grade_year)AS level_40
		, COUNT(*) FILTER (WHERE 30::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <40::DECIMAL)   OVER (PARTITION BY weigth_rank_expand.rank_grade_year)AS level_30
		, COUNT(*) FILTER (WHERE 20::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <30::DECIMAL)  OVER (PARTITION BY weigth_rank_expand.rank_grade_year) AS level_20
		, COUNT(*) FILTER (WHERE 10::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <20::DECIMAL)   OVER (PARTITION BY weigth_rank_expand.rank_grade_year)AS level_10
		, COUNT(*) FILTER (WHERE weigth_rank_expand.score<10::DECIMAL) OVER (PARTITION BY weigth_rank_expand.rank_grade_year)AS level_lt10
		, student_id
		, score
		, grade_rank AS rank
		, graderank_percentage AS percentile
		, rank_class_name
		, rank_tag1
		, rank_tag2
	FROM
		weigth_rank_expand
	UNION ALL
	SELECT
		rank_school_year 
		, rank_semester
		, rank_grade_year
		, item_type
		, exam_id AS ref_exam_id
		, item_name
		, '班排名'::TEXT AS rank_type
		, rank_class_name AS rank_name
		, true AS is_alive
		, class_count AS matrix_count
		, AVG(weigth_rank_expand.score::Decimal)FILTER(WHERE class_rank <= TRUNC(class_count * 0.25)) OVER(PARTITION BY weigth_rank_expand.rank_class_name) AS avg_top_25
		, AVG(weigth_rank_expand.score::Decimal)FILTER(WHERE class_rank <= TRUNC(class_count * 0.5)) OVER(PARTITION BY weigth_rank_expand.rank_class_name) AS avg_top_50
		, AVG(weigth_rank_expand.score::Decimal)OVER(PARTITION BY weigth_rank_expand.rank_class_name) AS avg
		, AVG(weigth_rank_expand.score::Decimal)FILTER(WHERE class_rank >= TRUNC(class_count * 0.5)) OVER(PARTITION BY weigth_rank_expand.rank_class_name) AS avg_bottom_50
		, AVG(weigth_rank_expand.score::Decimal)FILTER(WHERE class_rank >= TRUNC(class_count * 0.75)) OVER(PARTITION BY weigth_rank_expand.rank_class_name) AS avg_bottom_25
		, COUNT(*) FILTER (WHERE 100::DECIMAL<=weigth_rank_expand.score::DECIMAL ) OVER(PARTITION BY weigth_rank_expand.rank_class_name)AS level_gte100 
		, COUNT(*) FILTER (WHERE 90::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <100::DECIMAL) OVER(PARTITION BY weigth_rank_expand.rank_class_name) AS level_90
		, COUNT(*) FILTER (WHERE 80::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <90::DECIMAL) OVER(PARTITION BY weigth_rank_expand.rank_class_name) AS level_80
		, COUNT(*) FILTER (WHERE 70::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <80::DECIMAL) OVER(PARTITION BY weigth_rank_expand.rank_class_name)AS level_70
		, COUNT(*) FILTER (WHERE 60::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <70::DECIMAL) OVER(PARTITION BY weigth_rank_expand.rank_class_name) AS level_60
		, COUNT(*) FILTER (WHERE 50::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <60::DECIMAL) OVER(PARTITION BY weigth_rank_expand.rank_class_name)AS level_50
		, COUNT(*) FILTER (WHERE 40::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <50::DECIMAL) OVER(PARTITION BY weigth_rank_expand.rank_class_name)AS level_40
		, COUNT(*) FILTER (WHERE 30::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <40::DECIMAL) OVER(PARTITION BY weigth_rank_expand.rank_class_name)AS level_30
		, COUNT(*) FILTER (WHERE 20::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <30::DECIMAL) OVER(PARTITION BY weigth_rank_expand.rank_class_name) AS level_20
		, COUNT(*) FILTER (WHERE 10::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <20::DECIMAL) OVER(PARTITION BY weigth_rank_expand.rank_class_name)AS level_10
		, COUNT(*) FILTER (WHERE weigth_rank_expand.score<10::DECIMAL) OVER (PARTITION BY weigth_rank_expand.rank_class_name)AS level_lt10 
		, student_id
		, score
		, class_rank AS rank
		, classrank_percentage AS percentile
		, rank_class_name
		, rank_tag1
		, rank_tag2
	FROM
		weigth_rank_expand
	UNION ALL
	SELECT
		rank_school_year 
		, rank_semester
		, rank_grade_year
		, item_type
		, exam_id AS ref_exam_id
		, item_name
		, '類別1排名'::TEXT AS rank_type
		, rank_tag1 AS rank_name
		, true AS is_alive
		, tag1_count AS matrix_count
		, AVG(weigth_rank_expand.Score::Decimal)FILTER(WHERE tag1_rank <= TRUNC(tag1_count * 0.25)) OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag1) AS avg_top_25
		, AVG(weigth_rank_expand.Score::Decimal)FILTER(WHERE tag1_rank <= TRUNC(tag1_count * 0.5)) OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag1) AS avg_top_50
		, AVG(weigth_rank_expand.Score::Decimal)OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag1) AS avg
		, AVG(weigth_rank_expand.Score::Decimal)FILTER(WHERE tag1_rank >= TRUNC(tag1_count * 0.5)) OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag1) AS avg_bottom_50
		, AVG(weigth_rank_expand.Score::Decimal)FILTER(WHERE tag1_rank >= TRUNC(tag1_count * 0.75)) OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag1) AS avg_bottom_25
		, COUNT(*) FILTER (WHERE 100::DECIMAL<=weigth_rank_expand.score::DECIMAL ) OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag1)AS level_gte100 
		, COUNT(*) FILTER (WHERE 90::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <100::DECIMAL)OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag1) AS level_90
		, COUNT(*) FILTER (WHERE 80::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <90::DECIMAL) OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag1) AS level_80
		, COUNT(*) FILTER (WHERE 70::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <80::DECIMAL) OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag1)AS level_70
		, COUNT(*) FILTER (WHERE 60::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <70::DECIMAL) OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag1) AS level_60
		, COUNT(*) FILTER (WHERE 50::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <60::DECIMAL) OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag1)AS level_50
		, COUNT(*) FILTER (WHERE 40::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <50::DECIMAL) OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag1)AS level_40
		, COUNT(*) FILTER (WHERE 30::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <40::DECIMAL) OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag1)AS level_30
		, COUNT(*) FILTER (WHERE 20::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <30::DECIMAL) OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag1) AS level_20
		, COUNT(*) FILTER (WHERE 10::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <20::DECIMAL) OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag1)AS level_10
		, COUNT(*) FILTER (WHERE weigth_rank_expand.score<10::DECIMAL) OVER (PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag1)AS level_lt10
		, student_id
		, score
		, tag1_rank AS rank
		, tag1rank_percentage AS percentile
		, rank_class_name
		, rank_tag1
		, rank_tag2
	FROM
		weigth_rank_expand
	WHERE
		weigth_rank_expand.rank_tag1 IS NOT NULL
		AND weigth_rank_expand.rank_tag1 <> ''
	UNION ALL
	SELECT
		rank_school_year 
		, rank_semester
		, rank_grade_year
		, item_type
		, exam_id AS ref_exam_id
		, item_name
		, '類別2排名'::TEXT AS rank_type
		, rank_tag2 AS rank_name
		, true AS is_alive
		, tag2_count AS matrix_count
		, AVG(weigth_rank_expand.Score::Decimal)FILTER(WHERE tag2_rank <= TRUNC(tag2_count * 0.25)) OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag2) AS avg_top_25
		, AVG(weigth_rank_expand.Score::Decimal)FILTER(WHERE tag2_rank <= TRUNC(tag2_count * 0.5)) OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag2) AS avg_top_50
		, AVG(weigth_rank_expand.Score::Decimal)OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag2) AS avg
		, AVG(weigth_rank_expand.Score::Decimal)FILTER(WHERE tag2_rank >= TRUNC(tag2_count * 0.5)) OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag2) AS avg_bottom_50
		, AVG(weigth_rank_expand.Score::Decimal)FILTER(WHERE tag2_rank >= TRUNC(tag2_count * 0.75)) OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag2) AS avg_bottom_25
		, COUNT(*) FILTER (WHERE 100::DECIMAL<=weigth_rank_expand.score::DECIMAL ) OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag2)AS level_gte100 
		, COUNT(*) FILTER (WHERE 90::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <100::DECIMAL)OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag2) AS level_90
		, COUNT(*) FILTER (WHERE 80::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <90::DECIMAL) OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag2) AS level_80
		, COUNT(*) FILTER (WHERE 70::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <80::DECIMAL) OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag2)AS level_70
		, COUNT(*) FILTER (WHERE 60::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <70::DECIMAL) OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag2) AS level_60
		, COUNT(*) FILTER (WHERE 50::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <60::DECIMAL) OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag2)AS level_50
		, COUNT(*) FILTER (WHERE 40::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <50::DECIMAL) OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag2)AS level_40
		, COUNT(*) FILTER (WHERE 30::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <40::DECIMAL) OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag2)AS level_30
		, COUNT(*) FILTER (WHERE 20::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <30::DECIMAL) OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag2) AS level_20
		, COUNT(*) FILTER (WHERE 10::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <20::DECIMAL) OVER(PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag2)AS level_10
		, COUNT(*) FILTER (WHERE weigth_rank_expand.score<10::DECIMAL) OVER (PARTITION BY weigth_rank_expand.rank_grade_year, weigth_rank_expand.rank_tag2)AS level_lt10
		, student_id
		, score
		, tag2_rank AS rank
		, tag2rank_percentage AS percentile
		, rank_class_name
		, rank_tag1
		, rank_tag2
	FROM
		weigth_rank_expand
	WHERE
		weigth_rank_expand.rank_tag2 IS NOT NULL
		AND weigth_rank_expand.rank_tag2 <> ''
)
, update_data AS
(
	UPDATE
		rank_matrix
	SET
		is_alive = NULL
	FROM score_list
	WHERE
		rank_matrix.is_alive = true
		AND rank_matrix.school_year = score_list.rank_school_year
		AND rank_matrix.semester = score_list.rank_semester
		AND rank_matrix.grade_year = score_list.rank_grade_year
		AND rank_matrix.ref_exam_id = score_list.ref_exam_id

	RETURNING rank_matrix.*
)
, insert_batch_data AS
(
	INSERT INTO rank_batch
		(
			school_year
			, semester
			, calculation_description
			, setting
		)
		SELECT
			DISTINCT
			raw.rank_school_year::INT
			, raw.rank_semester::INT
			, raw.rank_school_year||' '||raw.rank_semester||' 計算'||raw.rank_exam_name||'排名' AS calculation_description
			, raw.calculation_setting
		FROM
			raw

	RETURNING *
)
, insert_matrix_data AS
(
	INSERT INTO rank_matrix
		(
			ref_batch_id
			, school_year
			, semester
			, grade_year
			, item_type
			, ref_exam_id
			, item_name
			, rank_type
			, rank_name
			, is_alive
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
		)
		SELECT
			insert_batch_data.id AS ref_batch_id
			, score_list.rank_school_year
			, score_list.rank_semester
			, score_list.rank_grade_year
			, score_list.item_type
			, score_list.ref_exam_id
			, score_list.item_name
			, score_list.rank_type
			, score_list.rank_name
			, score_list.is_alive
			, score_list.matrix_count
			, score_list.avg_top_25
			, score_list.avg_top_50
			, score_list.avg
			, score_list.avg_bottom_50
			, score_list.avg_bottom_25
			, score_list.level_gte100
			, score_list.level_90
			, score_list.level_80
			, score_list.level_70
			, score_list.level_60
			, score_list.level_50
			, score_list.level_40
			, score_list.level_30
			, score_list.level_20
			, score_list.level_10
			, score_list.level_lt10
		FROM
			score_list
			LEFT OUTER JOIN update_data
				ON update_data.id  < 0 --永遠為false，只是為了讓insert等待update執行完
			CROSS JOIN insert_batch_data
		GROUP BY
			insert_batch_data.id
			, score_list.rank_school_year
			,score_list.rank_semester
			,score_list.rank_grade_year
			,score_list.item_type
			,score_list.ref_exam_id
			,score_list.item_name
			,score_list.rank_type
			,score_list.rank_name
			,score_list.is_alive
			,score_list.matrix_count
			,score_list.avg_top_25
			,score_list.avg_top_50
			,score_list.avg
			,score_list.avg_bottom_50
			,score_list.avg_bottom_25
			,score_list.level_gte100
			,score_list.level_90
			,score_list.level_80
			,score_list.level_70
			,score_list.level_60
			,score_list.level_50
			,score_list.level_40
			,score_list.level_30
			,score_list.level_20
			,score_list.level_10
			,score_list.level_lt10

	RETURNING *
)
, insert_batch_student_data AS
(
	INSERT INTO rank_batch_student
		(
			ref_batch_id
			, ref_student_id
			, grade_year
			, matrix_grade
			, matrix_class
			, matrix_tag1
			, matrix_tag2
		)
		SELECT
			insert_batch_data.id AS ref_batch_id
			, score_list.student_id
			, score_list.rank_grade_year
			, score_list.rank_grade_year||'年級' AS matrix_grade
			, score_list.rank_class_name
			, score_list.rank_tag1
			, score_list.rank_tag2
		FROM
			score_list
			CROSS JOIN insert_batch_data
)
, insert_detail_data AS
(
	INSERT INTO
		rank_detail
		(
			ref_matrix_id
			, ref_student_id
			, score
			, rank
			, percentile
		)
		SELECT
			insert_matrix_data.id AS ref_matrix_id
			, score_list.student_id AS ref_student_id
			, score_list.score AS score
			, score_list.rank AS rank
			, score_list.percentile AS percentile
		FROM
			score_list
			LEFT OUTER JOIN
				insert_matrix_data
					ON insert_matrix_data.school_year = score_list.rank_school_year
					AND insert_matrix_data.semester = score_list.rank_semester
					AND insert_matrix_data.grade_year = score_list.rank_grade_year
					AND insert_matrix_data.item_type = score_list.item_type
					AND insert_matrix_data.ref_exam_id = score_list.ref_exam_id
					AND insert_matrix_data.item_name = score_list.item_name
					AND insert_matrix_data.rank_type = score_list.rank_type
					AND insert_matrix_data.rank_name = score_list.rank_name
)
SELECT
	score_list.rank_school_year
	, score_list.rank_semester
	, score_list.rank_grade_year
	, score_list.item_type
	, score_list.ref_exam_id
	, score_list.item_name
	, score_list.rank_type
	, score_list.rank_name
	, score_list.student_id
FROM 
	score_list
	LEFT OUTER JOIN insert_matrix_data
		ON insert_matrix_data.school_year = score_list.rank_school_year
		AND insert_matrix_data.semester = score_list.rank_semester
		AND insert_matrix_data.grade_year = score_list.rank_grade_year
		AND insert_matrix_data.item_type = score_list.item_type
		AND insert_matrix_data.ref_exam_id = score_list.ref_exam_id
		AND insert_matrix_data.item_name = score_list.item_name
		AND insert_matrix_data.rank_type = score_list.rank_type
		AND insert_matrix_data.rank_name = score_list.rank_name
";
                    #endregion

                    bkw.ReportProgress(50);
                    QueryHelper queryHelper = new QueryHelper();
                    queryHelper.Select(insertRankSql);
                    bkw.ReportProgress(100);
                }
                catch (Exception exception)
                {
                    bkwException = exception;
                }
            };

            bkw.RunWorkerCompleted += delegate
            {
                if (bkwException != null)
                {
                    btnCacluate.Enabled = true;
                    btnPrevious.Enabled = true;
                    throw new Exception("計算排名失敗", bkwException);
                }
                MessageBox.Show("計算完成");
                MotherForm.SetStatusBarMessage("排名計算完成");
                pbLoading.Visible = false;
                btnCacluate.Enabled = true;
                btnPrevious.Enabled = true;
            };

            bkw.RunWorkerAsync();
        }

        private void cboStudentTag1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboStudentTag1.SelectedIndex != 0)
            {
                cboStudentTag2.Enabled = true;
            }
            else
            {
                cboStudentTag2.SelectedIndex = 0;
                cboStudentTag2.Enabled = false;
            }
            if (cboStudentTag1.Text == cboStudentTag2.Text || cboStudentTag1.Text == cboStudentFilter.Text)
            {
                cboStudentTag1.SelectedIndex = 0;
            }
        }

        private void cboStudentTag2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboStudentTag2.Text == cboStudentTag1.Text || cboStudentTag2.Text == cboStudentFilter.Text)
            {
                cboStudentTag2.SelectedIndex = 0;
            }
        }

        private void cboStudentFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboStudentFilter.Text == cboStudentTag1.Text || cboStudentFilter.Text == cboStudentTag2.Text)
            {
                cboStudentFilter.SelectedIndex = 0;
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {

        }

        private void btnExport_Click(object sender, EventArgs e)
        {

        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            #region 清除Panel裡的CheckBox
            foreach (CheckBox checkBox in _CheckBoxList)
            {
                plStudentView.Controls.Remove(checkBox);
            }
            #endregion

            btnCacluate.Enabled = true;
            plStudentView.Visible = false;
            this.Width = _FormWidth;
            this.Height = _FormHeight;
            _CheckBoxList = new List<CheckBox>();
            if (dgvStudentList.Rows.Count > 0)
            {
                dgvStudentList.Rows.Clear();
            }
        }
    }
}
