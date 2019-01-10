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

namespace JHEvaluation.Rank
{
    public partial class CacluateRegularAssessmentRank : BaseForm
    {
        public CacluateRegularAssessmentRank()
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

        DataTable _SchoolYearTable = new DataTable();
        string _DefaultSchoolYear = "";
        string _DefaultSemester = "";
        List<ExamRecord> _ExamList = new List<ExamRecord>();
        List<TagConfigRecord> _TagConfigRecord = new List<TagConfigRecord>();
        List<StudentRecord> _StudentRecord = new List<StudentRecord>();
        List<CheckBox> _CheckBoxList = new List<CheckBox>();
        List<StudentRecord> _FilterStudentList = new List<StudentRecord>();
        string _StudentListSql = "";
        int _FormWidth = 0, _FormHeight = 0;

        private void CacluateRegularAssessmentRank_Load(object sender, EventArgs e)
        {
            #region 讓Form回到起始狀態
            plStudentView.Visible = false;
            this.Width = 580;
            this.Height = 330; 
            #endregion

            #region 篩選資料
            _StudentRecord = _StudentRecord.Where(x => !string.IsNullOrEmpty(x.RefClassID)
                                                    && (x.Status == StudentRecord.StudentStatus.一般 || x.Status == StudentRecord.StudentStatus.延修)
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
            foreach (DataRow row in _SchoolYearTable.Rows)
            {
                #region 現階段先不用匯入其他學年度
                //現階段先不用匯入其他學年度
                //if (!string.IsNullOrEmpty("" + row["school_year"]) && !cboSchoolYear.Items.Contains("" + row["school_year"]))
                //{
                //    cboSchoolYear.Items.Add("" + row["school_year"]);
                //} 
                #endregion

                if (!string.IsNullOrEmpty("" + row["semester"]) && !cboSemester.Items.Contains("" + row["semester"]))
                {
                    cboSemester.Items.Add("" + row["semester"]);
                }
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
            foreach (var item in _TagConfigRecord.Select(x => x.Name).Distinct())
            {
                cboStudentFilter.Items.Add(item);
                cboStudentTag1.Items.Add(item);
                cboStudentTag2.Items.Add(item);
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
            this.Height += 32 * (((gradeList.Count % 4) == 0 ? 0 : gradeList.Count / 4) + 1);//每多一排checkBox，Form的高度+32
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

                    plStudentView.Controls.Add(newCheckBox);
                    _CheckBoxList.Add(newCheckBox);
                    checkBoxCount++;
                }
            }

            //每多一排CheckBox就把Form的高加23，dataGridView的位置往下加23並把高減少23
            int height = 23 * ((checkBoxCount % 4) == 0 ? 0 : checkBoxCount / 4);
            this.Height += height;
            dgvStudentList.Location = new Point(3, dgvStudentList.Location.Y + height);
            dgvStudentList.Height -= height;
            #endregion

            #region 讀取學生清單
            _FilterStudentList = new List<StudentRecord>();
            string studentFilter = cboStudentFilter.Text;
            string studentTag1 = cboStudentTag1.Text;
            string studentTag2 = cboStudentTag2.Text;
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
                        string studentFilterTagID = _TagConfigRecord.First(x => x.Name == studentFilter).ID;
                        List<string> filterStudentID = K12.Data.StudentTag.SelectAll().Where(x => x.RefTagID == studentFilterTagID).Select(x => x.RefStudentID).ToList();
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

                #region 將學生清單顯示在dataGridView上以及將學生清單組成sql字串
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
                string tag1ID = "";
                string tag2ID = "";
                if (!string.IsNullOrEmpty(cboStudentTag1.Text))
                {
                    tag1ID = _TagConfigRecord.First(x => x.Name == cboStudentTag1.Text).ID;
                }
                if (!string.IsNullOrEmpty(cboStudentTag2.Text))
                {
                    tag2ID = _TagConfigRecord.First(x => x.Name == cboStudentTag2.Text).ID;
                }
                List<StudentTagRecord> studentTag1List = K12.Data.StudentTag.SelectAll().Where(x => x.RefTagID == tag1ID).ToList();
                List<StudentTagRecord> studentTag2List = K12.Data.StudentTag.SelectAll().Where(x => x.RefTagID == tag2ID).ToList(); 
                #endregion

                List<DataGridViewRow> rowList = new List<DataGridViewRow>();
                List<string> studentListSQL = new List<string>();
                for (int rowIndex = 0; rowIndex < studentView.Count; rowIndex++)
                {
                    string tag1 = "", tag2 = "";
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(dgvStudentList);
                    row.Cells[0].Value = studentView[rowIndex].ClassName;
                    row.Cells[1].Value = studentView[rowIndex].SeatNo;
                    row.Cells[2].Value = studentView[rowIndex].StudentNumber;
                    row.Cells[3].Value = studentView[rowIndex].Name;
                    row.Cells[4].Value = studentView[rowIndex].RankGradeYear;
                    row.Cells[5].Value = studentView[rowIndex].RankClassName;
                    if (studentTag1List.Select(x => x.RefStudentID).Contains(studentView[rowIndex].studentId))
                    {
                        row.Cells[6].Value = cboStudentTag1.Text;
                        tag1 = cboStudentTag1.Text;
                    }
                    if (studentTag2List.Select(x => x.RefStudentID).Contains(studentView[rowIndex].studentId))
                    {
                        row.Cells[7].Value = cboStudentTag2.Text;
                        tag2 = cboStudentTag2.Text;
                    }

                    rowList.Add(row);

                    #region 每一筆學生先組好先加進List裡
                    studentListSQL.Add(@"
    SELECT
        '" + studentView[rowIndex].studentId + @"'::BIGINT AS student_id
        ,'" + studentView[rowIndex].Name + @"'::TEXT AS student_name
        ,'" + studentView[rowIndex].RankGradeYear.Trim('年', '級') + @"'::INT AS rank_grade_year
        ,'" + studentView[rowIndex].RankClassName + @"'::TEXT AS rank_class_name
        ,'" + tag1 + @"'::TEXT AS tag1
        ,'" + tag2 + @"'::TEXT AS tag2
     ");
                    #endregion

                }
                dgvStudentList.Rows.AddRange(rowList.ToArray());

                #region 將剛剛裝學生sql的list拆開
                _StudentListSql = @"
WITH student_list AS 
(
    " + string.Join(@"
    UNION ALL", studentListSQL) + @"
)";
                #endregion

                btnPrevious.Enabled = true;
                #endregion
            };

            bkw.RunWorkerAsync(); 
            #endregion
        }

        private void btnCacluate_Click(object sender, EventArgs e)
        {
            string schoolYear = lbSchoolYear.Text;
            string semester = lbSemester.Text;
            string examName = lbExam.Text;
            string tag1 = cboStudentTag1.Text;
            string tag2 = cboStudentTag2.Text;
            List<int> gradeYearList = new List<int>();
            foreach (CheckBox checkBox in _CheckBoxList)
            {
                gradeYearList.Add(Convert.ToInt32(checkBox.Text.Trim('年', '級')));
            }

            Exception bkwException = null;
            BackgroundWorker bkw = new BackgroundWorker();
            bkw.WorkerReportsProgress = true;

            #region 更新is_alive欄位的sql字串
            string updateString = @"
UPDATE rank_matrix SET is_alive = null";
            #endregion

            UpdateHelper updateHelper = new UpdateHelper();
            updateHelper.Execute(updateString);

            bkw.ProgressChanged += delegate (object s1, ProgressChangedEventArgs e1)
            {
                MotherForm.SetStatusBarMessage("計算成績中", e1.ProgressPercentage);
            };

            bkw.DoWork += delegate
            {
                try
                {
                    for (int index = 0; index < gradeYearList.Count; index++)
                    {
                        string sql = "";
                        bkw.ReportProgress(1);

                        #region 計算校排班排的sql字串
                        string insertGradeYearClassRankSql = @"
" + _StudentListSql + @"
, raw AS
(
	SELECT
		'" + gradeYearList[index] + @"'::TEXT  AS grade_year
		, '" + schoolYear + @"'::TEXT AS school_year
		, '" + semester + @"'::TEXT AS semester
		, '" + examName + @"'::TEXT AS exam_name
)
, weight AS
(
	SELECT
		id AS template_id
		, name
		, ('0'||unnest(xpath('/Extension/ScorePercentage/text()',xmlparse(content extension)))::text)::Decimal AS exam_weight
		,  100::Decimal- ('0'||unnest(xpath('/Extension/ScorePercentage/text()',xmlparse(content extension)))::text)::Decimal AS assignment_weight
	FROM  
		exam_template
)
,score_detail AS
( 
	SELECT
		student_list.student_id
   		,student_list.student_name
		,sc_attend.id AS sc_attend_id
		,course.course_name
		,course.school_year
		,course.semester
		,course.subject
		,course.domain
		,course.credit
		,weight.template_id
		,weight.name
		,weight.exam_weight
		,weight.assignment_weight
		, exam.id AS exam_id
		, exam.exam_name
		, student_list.rank_class_name
		, student_list.rank_grade_year	
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
		LEFT JOIN weight
			ON  weight.template_id=course.ref_exam_template_id
		INNER JOIN raw
			ON course.school_year = raw.school_year::int
			AND course.semester = raw.semester::int
			AND student_list.rank_grade_year = raw.grade_year::int
			AND exam.exam_name= raw.exam_name
)
,score_detail_avge AS
(
	SELECT	
		score_detail.*
		,(
			CASE 
				WHEN exam_score IS NOT NULL	
				THEN exam_score::Decimal
				ELSE  0
		 	END
			   * exam_weight::Decimal
			+
			CASE
				WHEN assignment_score IS NOT NULL  
				THEN	assignment_score::decimal
				ELSE 0
			END
			*assignment_weight
		)/(
			CASE 
				WHEN exam_score IS NOT NULL 
				THEN exam_weight
				ELSE 0
			END
			+
			CASE
				WHEN assignment_score IS NOT NULL 
				THEN assignment_weight
				ELSE 0
			END
		) AS score
	FROM 
		score_detail
	WHERE 
		(
			exam_score IS NOT NULL
			OR assignment_score IS NOT NULL
		)
		AND template_id IS NOT NULL
)
-----領域排名所需成績
,group_score AS
(
	SELECT 
		student_id
		,student_name
		,domain
		,SUM
		(
			CASE 
				WHEN  score_detail_avge.score IS NOT NULL THEN score_detail_avge.score::decimal * score_detail_avge.credit::decimal  
				ELSE 0
			END
		) / 
		SUM
		( 
			CASE 
				WHEN  score_detail_avge.score IS NOT NULL THEN score_detail_avge.credit::decimal
				ELSE 0
			END
		)  AS score
	FROM  score_detail_avge
	GROUP BY   domain, student_id,student_name 
)
------加權平均排名所需成績
,scoreWavge AS
(
	SELECT
		score_detail_avge.student_id
		,SUM
		(
			CASE 
				WHEN  score_detail_avge.score IS NOT NULL THEN score_detail_avge.score::decimal * score_detail_avge.credit::decimal  
				ELSE 0
			END
		) / 
		SUM
		( 
			CASE 
				WHEN  score_detail_avge.score IS NOT NULL THEN score_detail_avge.credit::decimal
				ELSE 0
			END
		)  AS avge
	FROM 
		score_detail_avge
	GROUP BY 
		student_id
)
-------計算領域排名
,domain_rank_raw AS
(
	SELECT
		group_score.student_id
		, group_score.domain
		, student_list.rank_class_name AS class_name
		, student_list.rank_grade_year AS grade_year
		, group_score.score
		, RANK() OVER(PARTITION BY student_list.rank_grade_year ,group_score.domain ORDER BY group_score.score DESC) AS grade_rank
		, RANK() OVER(PARTITION BY student_list.rank_class_name ,group_score.domain ORDER BY group_score.score DESC) AS class_rank
		, COUNT (group_score.student_id) OVER(PARTITION BY student_list.rank_grade_year ,group_score.domain ) AS grade_count
		, COUNT (group_score.student_id) OVER(PARTITION BY student_list.rank_class_name, group_score.domain) AS class_count
	FROM 
		group_score
			INNER JOIN student_list
				ON group_score.student_id = student_list.student_id
)
, domain_rank_expand AS 
(
	SELECT  
		domain_rank_raw.*
		,FLOOR((grade_rank::DECIMAL-1)*100::DECIMAL / grade_count)+1 AS graderank_percentage
		,FLOOR((class_rank::DECIMAL-1)*100::DECIMAL / class_count)+1 AS classrank_percentage
	FROM 
		domain_rank_raw
	WHERE  
		domain IS NOT NULL	
)
-----新增校排名
, insert_domain_gradeyear_rank_matrix AS
(
	INSERT INTO rank_matrix
	(
		school_year
		, semester
		, grade_year
		, item_type
		, ref_exam_id
		, item_name
		, rank_type
		, rank_name
		, is_alive
		, matrix_count
		, avg
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
		*
	FROM
	(
		SELECT
		--DISTINCT
			raw.school_year::INT AS school_year
			, raw.semester::INT AS semester
			, domain_rank_expand.grade_year AS grade_year
			, '定期評量/領域成績'::TEXT AS item_type
			, exam.id AS ref_exam_id
			, domain_rank_expand.domain AS item_name
			, '年排名'::TEXT AS rank_type
			, '' || domain_rank_expand.grade_year || '年級' ::TEXT AS rank_name
			, true AS is_alive
			, domain_rank_expand.grade_count AS matrix_count
			, AVG(domain_rank_expand.Score::Decimal)OVER(PARTITION BY domain_rank_expand.grade_year,domain_rank_expand.domain) AS avg
			, COUNT(*) FILTER (WHERE 100::DECIMAL<=domain_rank_expand.score::DECIMAL ) OVER(PARTITION BY domain_rank_expand.grade_year,domain_rank_expand.domain)AS level_gte100 
			, COUNT(*) FILTER (WHERE 90::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <100::DECIMAL)  OVER(PARTITION BY domain_rank_expand.grade_year,domain_rank_expand.domain) AS level_90
			, COUNT(*) FILTER (WHERE 80::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <90::DECIMAL)  OVER(PARTITION BY domain_rank_expand.grade_year,domain_rank_expand.domain) AS level_80
			, COUNT(*) FILTER (WHERE 70::DECIMAL<=domain_rank_expand.score AND  domain_rank_expand.score <80::DECIMAL)  OVER(PARTITION BY domain_rank_expand.grade_year,domain_rank_expand.domain)AS level_70
			, COUNT(*) FILTER (WHERE 60::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <70::DECIMAL) OVER(PARTITION BY domain_rank_expand.grade_year,domain_rank_expand.domain) AS level_60
			, COUNT(*) FILTER (WHERE 50::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <60::DECIMAL)  OVER (PARTITION BY domain_rank_expand.grade_year,domain_rank_expand.domain)AS level_50
			, COUNT(*) FILTER (WHERE 40::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <50::DECIMAL) OVER (PARTITION BY domain_rank_expand.grade_year,domain_rank_expand.domain)AS level_40
			, COUNT(*) FILTER (WHERE 30::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <40::DECIMAL)   OVER (PARTITION BY domain_rank_expand.grade_year,domain_rank_expand.domain)AS level_30
			, COUNT(*) FILTER (WHERE 20::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <30::DECIMAL)  OVER (PARTITION BY domain_rank_expand.grade_year,domain_rank_expand.domain) AS level_20
			, COUNT(*) FILTER (WHERE 10::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <20::DECIMAL)   OVER (PARTITION BY domain_rank_expand.grade_year,domain_rank_expand.domain)AS level_10
			, COUNT(*) FILTER (WHERE domain_rank_expand.score<10::DECIMAL) OVER (PARTITION BY domain_rank_expand.grade_year,domain_rank_expand.domain)AS level_lt10 
		FROM
			domain_rank_expand
			CROSS JOIN raw
			LEFT OUTER JOIN exam
				ON exam.exam_name = raw.exam_name  
	) AS sub
	GROUP BY
		sub.school_year
		,sub.semester
		,sub.grade_year
		,sub.ref_exam_id
		,sub.item_type
		,sub.item_name
		,sub.rank_type
		,sub.rank_name
		,sub.is_alive
		,sub.matrix_count
		,sub.avg
		,sub.level_gte100
		,sub.level_90
		,sub.level_80
		,sub.level_70
		,sub.level_60
		,sub.level_50
		,sub.level_40
		,sub.level_30
		,sub.level_20
		,sub.level_10
		,sub.level_lt10

	RETURNING *
)
, insert_domain_gradeyear_rank_detail AS
(
	INSERT INTO rank_detail
	(
		ref_matrix_id
		, ref_student_id
		, score
		, rank
		, percentile
	)
	SELECT 
		insert_domain_gradeyear_rank_matrix.id AS ref_matrix_id
		, domain_rank_expand.student_id AS ref_student_id
		, domain_rank_expand.score AS score
		, domain_rank_expand.grade_rank AS rank
		, domain_rank_expand.graderank_percentage AS percentile	
	FROM
		domain_rank_expand
		LEFT OUTER JOIN insert_domain_gradeyear_rank_matrix
			ON insert_domain_gradeyear_rank_matrix.grade_year = domain_rank_expand.grade_year
			AND insert_domain_gradeyear_rank_matrix.item_name=domain_rank_expand.domain
)
-----新增班排名
, insert_domain_class_rank_matrix AS
(
	INSERT INTO rank_matrix
	(
		school_year
		, semester
		, grade_year
		, item_type
		, ref_exam_id
		, item_name
		, rank_type
		, rank_name
		, is_alive
		, matrix_count
		, avg
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
		*
	FROM
	 	(
	 		SELECT
			--DISTINCT
				raw.school_year::INT AS school_year
				, raw.semester::INT AS semester
				, domain_rank_expand.grade_year AS grade_year
				, '定期評量/領域成績'::TEXT AS item_type
				, exam.id AS ref_exam_id
				, domain_rank_expand.domain AS item_name
				, '班排名'::TEXT AS rank_type
				, domain_rank_expand.class_name AS rank_name
				, true AS is_alive
				, domain_rank_expand.class_count AS matrix_count
				, AVG(domain_rank_expand.Score::Decimal)OVER(PARTITION BY domain_rank_expand.class_name,domain_rank_expand.domain) AS avg
				, COUNT(*) FILTER (WHERE 100::DECIMAL<=domain_rank_expand.score::DECIMAL ) OVER(PARTITION BY domain_rank_expand.class_name,domain_rank_expand.domain)AS level_gte100 
				, COUNT(*) FILTER (WHERE 90::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <100::DECIMAL)OVER (PARTITION BY domain_rank_expand.class_name,domain_rank_expand.domain) AS level_90
				, COUNT(*) FILTER (WHERE 80::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <90::DECIMAL) OVER (PARTITION BY domain_rank_expand.class_name,domain_rank_expand.domain) AS level_80
				, COUNT(*) FILTER (WHERE 70::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <80::DECIMAL) OVER (PARTITION BY domain_rank_expand.class_name,domain_rank_expand.domain)AS level_70
				, COUNT(*) FILTER (WHERE 60::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <70::DECIMAL) OVER (PARTITION BY domain_rank_expand.class_name,domain_rank_expand.domain) AS level_60
				, COUNT(*) FILTER (WHERE 50::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <60::DECIMAL) OVER (PARTITION BY domain_rank_expand.class_name,domain_rank_expand.domain)AS level_50
				, COUNT(*) FILTER (WHERE 40::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <50::DECIMAL) OVER (PARTITION BY domain_rank_expand.class_name,domain_rank_expand.domain)AS level_40
				, COUNT(*) FILTER (WHERE 30::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <40::DECIMAL) OVER (PARTITION BY domain_rank_expand.class_name,domain_rank_expand.domain)AS level_30
				, COUNT(*) FILTER (WHERE 20::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <30::DECIMAL) OVER (PARTITION BY domain_rank_expand.class_name,domain_rank_expand.domain) AS level_20
				, COUNT(*) FILTER (WHERE 10::DECIMAL<=domain_rank_expand.score AND domain_rank_expand.score <20::DECIMAL) OVER (PARTITION BY domain_rank_expand.class_name,domain_rank_expand.domain)AS level_10
				, COUNT(*) FILTER (WHERE domain_rank_expand.score<10::DECIMAL) OVER (PARTITION BY domain_rank_expand.class_name,domain_rank_expand.domain)AS level_lt10 
			FROM
				domain_rank_expand
				CROSS JOIN raw
				LEFT OUTER JOIN exam
				ON exam.exam_name = raw.exam_name
		) AS sub
		GROUP BY
			sub.school_year
			,sub.semester
			,sub.grade_year
			,sub.ref_exam_id
			,sub.item_type
			,sub.item_name
			,sub.rank_type
			,sub.rank_name
			,sub.is_alive
			,sub.matrix_count
			,sub.avg
			,sub.level_gte100
			,sub.level_90
			,sub.level_80
			,sub.level_70
			,sub.level_60
			,sub.level_50
			,sub.level_40
			,sub.level_30
			,sub.level_20
			,sub.level_10
			,sub.level_lt10

	RETURNING *
)
, insert_domain_class_rank_detail AS
(
	INSERT INTO rank_detail
	(
		ref_matrix_id
		, ref_student_id
		, score
		, rank
		, percentile
	)
	SELECT 
		insert_domain_class_rank_matrix.id AS ref_matrix_id
		, domain_rank_expand.student_id AS ref_student_id
		, domain_rank_expand.score AS score
		, domain_rank_expand.class_rank AS rank
		, domain_rank_expand.classrank_percentage AS percentile	
	FROM
		domain_rank_expand
		LEFT OUTER JOIN insert_domain_class_rank_matrix
			ON insert_domain_class_rank_matrix.grade_year = domain_rank_expand.grade_year
			AND insert_domain_class_rank_matrix.item_name=domain_rank_expand.domain
			AND insert_domain_class_rank_matrix.rank_name=domain_rank_expand.class_name
)
--------計算科目排名
,subject_rank_raw AS
(
	SELECT
		score_detail_avge.student_id
		, score_detail_avge.rank_grade_year AS grade_year
		, score_detail_avge.rank_class_name AS class_name
		, score_detail_avge.subject
		, score_detail_avge.score
		, RANK() OVER(PARTITION BY score_detail_avge.rank_grade_year,score_detail_avge.subject ORDER BY score DESC) AS grade_rank
		, RANK() OVER(PARTITION BY score_detail_avge.rank_class_name ,score_detail_avge.subject ORDER BY score DESC) AS class_rank
		, COUNT (score_detail_avge.student_id) OVER(PARTITION BY score_detail_avge.rank_grade_year,score_detail_avge.subject ) AS grade_count
		, COUNT (score_detail_avge.student_id) OVER(PARTITION BY score_detail_avge.rank_class_name, score_detail_avge.subject) AS class_count
	FROM score_detail_avge
    	INNER JOIN student_list
			ON student_list.student_id = score_detail_avge.student_id
)
, subject_rank_expand AS 
(
	SELECT  
		subject_rank_raw.*
		,FLOOR((grade_rank::DECIMAL-1)*100::DECIMAL/grade_count)+1 AS graderank_percentage
		,FLOOR((class_rank::DECIMAL-1)*100::DECIMAL/class_count)+1 AS classrank_percentage
	FROM 
		subject_rank_raw
)
----新增校排
, insert_subject_gradeyear_rank_matrix AS
(
	INSERT INTO rank_matrix
	(
		school_year
		, semester
		, grade_year
		, item_type
		, ref_exam_id
		, item_name
		, rank_type
		, rank_name
		, is_alive
		, matrix_count
		, avg
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
		*
	FROM	
		(
			SELECT
				--DISTINCT
				raw.school_year::INT AS school_year
				, raw.semester::INT AS semester
				, subject_rank_expand.grade_year AS grade_year
				, '定期評量/科目成績'::TEXT AS item_type
				, exam.id AS ref_exam_id
				, subject_rank_expand.subject AS item_name
				, '年排名'::TEXT AS rank_type
				, '' || subject_rank_expand.grade_year || '年級'::TEXT AS rank_name
				, true AS is_alive
				, subject_rank_expand.grade_count AS matrix_count
				, AVG(subject_rank_expand.Score::Decimal)OVER(PARTITION BY subject_rank_expand.grade_year,subject_rank_expand.subject) AS avg
				, COUNT(*) FILTER (WHERE 100::DECIMAL<=subject_rank_expand.score::DECIMAL ) OVER(PARTITION BY subject_rank_expand.grade_year,subject_rank_expand.subject)AS level_gte100 
				, COUNT(*) FILTER (WHERE 90::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <100::DECIMAL)  OVER(PARTITION BY subject_rank_expand.grade_year,subject_rank_expand.subject) AS level_90
				, COUNT(*) FILTER (WHERE 80::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <90::DECIMAL)  OVER(PARTITION BY subject_rank_expand.grade_year,subject_rank_expand.subject) AS level_80
				, COUNT(*) FILTER (WHERE 70::DECIMAL<=subject_rank_expand.score AND  subject_rank_expand.score <80::DECIMAL)  OVER(PARTITION BY subject_rank_expand.grade_year,subject_rank_expand.subject)AS level_70
				, COUNT(*) FILTER (WHERE 60::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <70::DECIMAL) OVER(PARTITION BY subject_rank_expand.grade_year,subject_rank_expand.subject) AS level_60
				, COUNT(*) FILTER (WHERE 50::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <60::DECIMAL)  OVER (PARTITION BY subject_rank_expand.grade_year,subject_rank_expand.subject)AS level_50
				, COUNT(*) FILTER (WHERE 40::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <50::DECIMAL) OVER (PARTITION BY subject_rank_expand.grade_year,subject_rank_expand.subject)AS level_40
				, COUNT(*) FILTER (WHERE 30::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <40::DECIMAL)   OVER (PARTITION BY subject_rank_expand.grade_year,subject_rank_expand.subject)AS level_30
				, COUNT(*) FILTER (WHERE 20::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <30::DECIMAL)  OVER (PARTITION BY subject_rank_expand.grade_year,subject_rank_expand.subject) AS level_20
				, COUNT(*) FILTER (WHERE 10::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <20::DECIMAL)   OVER (PARTITION BY subject_rank_expand.grade_year,subject_rank_expand.subject)AS level_10
				, COUNT(*) FILTER (WHERE subject_rank_expand.score<10::DECIMAL) OVER (PARTITION BY subject_rank_expand.grade_year,subject_rank_expand.subject)AS level_lt10 
			FROM
				subject_rank_expand
					CROSS JOIN raw
					LEFT OUTER JOIN exam
						ON exam.exam_name = raw.exam_name
	) AS sub
	GROUP BY
		sub.school_year
		,sub.semester
		,sub.grade_year
		,sub.ref_exam_id
		,sub.item_type
		,sub.item_name
		,sub.rank_type
		,sub.rank_name
		,sub.is_alive
		,sub.matrix_count
		,sub.avg
		,sub.level_gte100
		,sub.level_90
		,sub.level_80
		,sub.level_70
		,sub.level_60
		,sub.level_50
		,sub.level_40
		,sub.level_30
		,sub.level_20
		,sub.level_10
		,sub.level_lt10
	RETURNING *
)
, insert_subject_gradeyear_rank_detail AS
(
	INSERT INTO rank_detail
	(
		ref_matrix_id
		, ref_student_id
		, score
		, rank
		, percentile
	)
	SELECT 
		insert_subject_gradeyear_rank_matrix.id AS ref_matrix_id
		, subject_rank_expand.student_id AS ref_student_id
		, subject_rank_expand.score AS score
		, subject_rank_expand.grade_rank AS rank
		, subject_rank_expand.graderank_percentage AS percentile	
	FROM
		subject_rank_expand
		LEFT OUTER JOIN insert_subject_gradeyear_rank_matrix
			ON insert_subject_gradeyear_rank_matrix.grade_year = subject_rank_expand.grade_year
			AND insert_subject_gradeyear_rank_matrix.item_name=subject_rank_expand.subject
)
----新增班排
, insert_subject_class_rank_matrix AS
(
	INSERT INTO rank_matrix(
		school_year
		, semester
		, grade_year
		, item_type
		, ref_exam_id
		, item_name
		, rank_type
		, rank_name
		, is_alive
		, matrix_count
		, avg
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
		*
	FROM(
		SELECT
			--DISTINCT
			raw.school_year::INT AS school_year
			, raw.semester::INT AS semester
			, subject_rank_expand.grade_year AS grade_year
			, '定期評量/科目成績'::TEXT AS item_type
			, exam.id AS ref_exam_id
			, subject_rank_expand.subject AS item_name
			, '班排名'::TEXT AS rank_type
			, subject_rank_expand.class_name  AS rank_name
			, true AS is_alive
			, subject_rank_expand.class_count AS matrix_count
			, AVG(subject_rank_expand.Score::Decimal)OVER(PARTITION BY subject_rank_expand.class_name,subject_rank_expand.subject) AS avg
			, COUNT(*) FILTER (WHERE 100::DECIMAL<=subject_rank_expand.score::DECIMAL ) OVER(PARTITION BY subject_rank_expand.class_name,subject_rank_expand.subject)AS level_gte100 
			, COUNT(*) FILTER (WHERE 90::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <100::DECIMAL)  OVER(PARTITION BY subject_rank_expand.class_name,subject_rank_expand.subject) AS level_90
			, COUNT(*) FILTER (WHERE 80::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <90::DECIMAL)  OVER(PARTITION BY subject_rank_expand.class_name,subject_rank_expand.subject) AS level_80
			, COUNT(*) FILTER (WHERE 70::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <80::DECIMAL)  OVER(PARTITION BY subject_rank_expand.class_name,subject_rank_expand.subject)AS level_70
			, COUNT(*) FILTER (WHERE 60::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <70::DECIMAL) OVER(PARTITION BY subject_rank_expand.class_name,subject_rank_expand.subject) AS level_60
			, COUNT(*) FILTER (WHERE 50::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <60::DECIMAL)  OVER (PARTITION BY subject_rank_expand.class_name,subject_rank_expand.subject)AS level_50
			, COUNT(*) FILTER (WHERE 40::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <50::DECIMAL) OVER (PARTITION BY subject_rank_expand.class_name,subject_rank_expand.subject)AS level_40
			, COUNT(*) FILTER (WHERE 30::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <40::DECIMAL)   OVER (PARTITION BY subject_rank_expand.class_name,subject_rank_expand.subject)AS level_30
			, COUNT(*) FILTER (WHERE 20::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <30::DECIMAL)  OVER (PARTITION BY subject_rank_expand.class_name,subject_rank_expand.subject) AS level_20
			, COUNT(*) FILTER (WHERE 10::DECIMAL<=subject_rank_expand.score AND subject_rank_expand.score <20::DECIMAL)   OVER (PARTITION BY subject_rank_expand.class_name,subject_rank_expand.subject)AS level_10
			, COUNT(*) FILTER (WHERE subject_rank_expand.score<10::DECIMAL) OVER (PARTITION BY subject_rank_expand.class_name,subject_rank_expand.subject)AS level_lt10 
	FROM
		subject_rank_expand
			CROSS JOIN raw
			LEFT OUTER JOIN exam
				ON exam.exam_name = raw.exam_name
	)AS sub
	GROUP BY
		sub.school_year
		,sub.semester
		,sub.grade_year
		,sub.ref_exam_id
		,sub.item_type
		,sub.item_name
		,sub.rank_type
		,sub.rank_name
		,sub.is_alive
		,sub.matrix_count
		,sub.avg
		,sub.level_gte100
		,sub.level_90
		,sub.level_80
		,sub.level_70
		,sub.level_60
		,sub.level_50
		,sub.level_40
		,sub.level_30
		,sub.level_20
		,sub.level_10
		,sub.level_lt10

	RETURNING *
)
, insert_subject_class_rank_detail AS
(
	INSERT INTO rank_detail
	(
		ref_matrix_id
		, ref_student_id
		, score
		, rank
		, percentile
	)
	SELECT 
		insert_subject_class_rank_matrix.id AS ref_matrix_id
		, subject_rank_expand.student_id AS ref_student_id
		, subject_rank_expand.score AS score
		, subject_rank_expand.class_rank AS rank
		, subject_rank_expand.classrank_percentage AS percentile	
	FROM
		subject_rank_expand
			LEFT OUTER JOIN insert_subject_class_rank_matrix
			ON insert_subject_class_rank_matrix.grade_year = subject_rank_expand.grade_year
				AND insert_subject_class_rank_matrix.rank_name=subject_rank_expand.class_name
				AND insert_subject_class_rank_matrix.item_name=subject_rank_expand.subject
 
)
-----------計算加權平均排名
,weigth_rank_raw AS
(
	SELECT 
		scoreWavge.student_id
		, student_list.rank_grade_year AS grade_year
		, student_list.rank_class_name AS class_name
		, scoreWavge.avge AS score
		, RANK() OVER(PARTITION BY student_list.rank_grade_year ORDER BY avge DESC) AS grade_rank
		, RANK() OVER(PARTITION BY student_list.rank_class_name ORDER BY avge DESC) AS class_rank
		, COUNT (*) OVER(PARTITION BY student_list.rank_grade_year) AS grade_count
		, COUNT (*) OVER(PARTITION BY student_list.rank_class_name) AS class_count
	FROM 
		scoreWavge
		INNER JOIN student_list
			ON scoreWavge.student_id = student_list.student_id
)
, weigth_rank_expand AS
(
	SELECT  
		weigth_rank_raw.*
		,FLOOR((grade_rank::DECIMAL-1)*100::DECIMAL/grade_count)+1 AS graderank_percentage
		,FLOOR((class_rank::DECIMAL-1)*100::DECIMAL/class_count)+1 AS classrank_percentage
	FROM 
		weigth_rank_raw
)
-----新增校排
, insert_weigth_gradeyear_rank_matrix AS
(
	INSERT INTO rank_matrix
	(
		school_year
		, semester
		, grade_year
		, item_type
		, ref_exam_id
		, item_name
		, rank_type
		, rank_name
		, is_alive
		, matrix_count
		, avg
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
		*
	FROM
	(
		SELECT
			--DISTINCT
			raw.school_year::INT AS school_year
			, raw.semester::INT AS semester
			, weigth_rank_expand.grade_year AS grade_year
			, '定期評量/總計成績'::text AS item_type
			, exam.id AS ref_exam_id
			, '加權平均'::text AS item_name
			, '年排名'::text AS rank_type
			, '' || weigth_rank_expand.grade_year || '年級' ::text AS rank_name
			, true AS is_alive
			, weigth_rank_expand.grade_count AS matrix_count
			, AVG(weigth_rank_expand.Score::Decimal)OVER(PARTITION BY weigth_rank_expand.grade_year) AS avg
			, COUNT(*) FILTER (WHERE 100::DECIMAL<=weigth_rank_expand.score::DECIMAL ) OVER(PARTITION BY weigth_rank_expand.grade_year)AS level_gte100 
			, COUNT(*) FILTER (WHERE 90::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <100::DECIMAL)  OVER(PARTITION BY weigth_rank_expand.grade_year) AS level_90
			, COUNT(*) FILTER (WHERE 80::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <90::DECIMAL)  OVER(PARTITION BY weigth_rank_expand.grade_year) AS level_80
			, COUNT(*) FILTER (WHERE 70::DECIMAL<=weigth_rank_expand.score AND  weigth_rank_expand.score <80::DECIMAL)  OVER(PARTITION BY weigth_rank_expand.grade_year)AS level_70
			, COUNT(*) FILTER (WHERE 60::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <70::DECIMAL) OVER(PARTITION BY weigth_rank_expand.grade_year) AS level_60
			, COUNT(*) FILTER (WHERE 50::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <60::DECIMAL)  OVER (PARTITION BY weigth_rank_expand.grade_year)AS level_50
			, COUNT(*) FILTER (WHERE 40::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <50::DECIMAL) OVER (PARTITION BY weigth_rank_expand.grade_year)AS level_40
			, COUNT(*) FILTER (WHERE 30::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <40::DECIMAL)   OVER (PARTITION BY weigth_rank_expand.grade_year)AS level_30
			, COUNT(*) FILTER (WHERE 20::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <30::DECIMAL)  OVER (PARTITION BY weigth_rank_expand.grade_year) AS level_20
			, COUNT(*) FILTER (WHERE 10::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <20::DECIMAL)   OVER (PARTITION BY weigth_rank_expand.grade_year)AS level_10
			, COUNT(*) FILTER (WHERE weigth_rank_expand.score<10::DECIMAL) OVER (PARTITION BY weigth_rank_expand.grade_year)AS level_lt10
		FROM
			weigth_rank_expand
			CROSS JOIN raw
			LEFT OUTER JOIN exam
				ON exam.exam_name = raw.exam_name
	)AS sub
	GROUP BY
		sub.school_year
		,sub.semester
		,sub.grade_year
		,sub.ref_exam_id
		,sub.item_type
		,sub.item_name
		,sub.rank_type
		,sub.rank_name
		,sub.is_alive
		,sub.matrix_count
		,sub.avg
		,sub.level_gte100
		,sub.level_90
		,sub.level_80
		,sub.level_70
		,sub.level_60
		,sub.level_50
		,sub.level_40
		,sub.level_30
		,sub.level_20
		,sub.level_10
		,sub.level_lt10

	RETURNING *
)
, insert_weigth_gradeyear_rank_detail AS
(
	INSERT INTO rank_detail
	(
		ref_matrix_id
		, ref_student_id
		, score
		, rank
		, percentile
	)
	SELECT 
		insert_weigth_gradeyear_rank_matrix.id AS ref_matrix_id
		, weigth_rank_expand.student_id AS ref_student_id
		, weigth_rank_expand.score AS score
		, weigth_rank_expand.grade_rank AS rank
		, weigth_rank_expand.graderank_percentage AS percentile
	FROM
		weigth_rank_expand
		LEFT OUTER JOIN insert_weigth_gradeyear_rank_matrix
			ON insert_weigth_gradeyear_rank_matrix.grade_year = weigth_rank_expand.grade_year
)
-----新增班排
, insert_weigth_class_rank_matrix AS
(
	INSERT INTO rank_matrix
	(
		school_year
		, semester
		, grade_year
		, item_type
		, ref_exam_id
		, item_name
		, rank_type
		, rank_name
		, is_alive
		, matrix_count
		, avg
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
		* 
	FROM
	(
		SELECT
		--DISTINCT
			raw.school_year::INT AS school_year
			, raw.semester::INT AS semester
			, weigth_rank_expand.grade_year AS grade_year
			, '定期評量/總計成績'::TEXT AS item_type
			, exam.id AS ref_exam_id
			, '加權平均'::TEXT AS item_name
			, '班排名'::TEXT AS rank_type
			, weigth_rank_expand.class_name AS rank_name
			, true AS is_alive
			, weigth_rank_expand.class_count AS matrix_count
			, AVG(weigth_rank_expand.score::Decimal)OVER(PARTITION BY weigth_rank_expand.class_name) AS avg
			, COUNT(*) FILTER (WHERE 100::DECIMAL<=weigth_rank_expand.score::DECIMAL ) OVER(PARTITION BY weigth_rank_expand.class_name)AS level_gte100 
			, COUNT(*) FILTER (WHERE 90::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <100::DECIMAL) OVER(PARTITION BY weigth_rank_expand.class_name) AS level_90
			, COUNT(*) FILTER (WHERE 80::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <90::DECIMAL) OVER(PARTITION BY weigth_rank_expand.class_name) AS level_80
			, COUNT(*) FILTER (WHERE 70::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <80::DECIMAL) OVER(PARTITION BY weigth_rank_expand.class_name)AS level_70
			, COUNT(*) FILTER (WHERE 60::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <70::DECIMAL) OVER(PARTITION BY weigth_rank_expand.class_name) AS level_60
			, COUNT(*) FILTER (WHERE 50::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <60::DECIMAL) OVER(PARTITION BY weigth_rank_expand.class_name)AS level_50
			, COUNT(*) FILTER (WHERE 40::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <50::DECIMAL) OVER(PARTITION BY weigth_rank_expand.class_name)AS level_40
			, COUNT(*) FILTER (WHERE 30::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <40::DECIMAL) OVER(PARTITION BY weigth_rank_expand.class_name)AS level_30
			, COUNT(*) FILTER (WHERE 20::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <30::DECIMAL) OVER(PARTITION BY weigth_rank_expand.class_name) AS level_20
			, COUNT(*) FILTER (WHERE 10::DECIMAL<=weigth_rank_expand.score AND weigth_rank_expand.score <20::DECIMAL) OVER(PARTITION BY weigth_rank_expand.class_name)AS level_10
			, COUNT(*) FILTER (WHERE weigth_rank_expand.score<10::DECIMAL) OVER (PARTITION BY weigth_rank_expand.class_name)AS level_lt10 
		FROM
			weigth_rank_expand
				CROSS JOIN raw
				LEFT OUTER JOIN exam
					ON exam.exam_name = raw.exam_name
	)AS sub
	GROUP BY
		sub.school_year
		,sub.semester
		,sub.grade_year
		,sub.ref_exam_id
		,sub.item_type
		,sub.item_name
		,sub.rank_type
		,sub.rank_name
		,sub.is_alive
		,sub.matrix_count
		,sub.avg
		,sub.level_gte100
		,sub.level_90
		,sub.level_80
		,sub.level_70
		,sub.level_60
		,sub.level_50
		,sub.level_40
		,sub.level_30
		,sub.level_20
		,sub.level_10
		,sub.level_lt10

	RETURNING *
)
, insert_weigth_class_rank_detail AS
(
	INSERT INTO rank_detail(
		ref_matrix_id
		, ref_student_id
		, score
		, rank
		, percentile
	)
	SELECT 
		insert_weigth_class_rank_matrix.id AS ref_matrix_id
		, weigth_rank_expand.student_id AS ref_student_id
		, weigth_rank_expand.score AS score
		, weigth_rank_expand.class_rank AS rank
		, weigth_rank_expand.classrank_percentage AS percentile	
	FROM
		weigth_rank_expand
		LEFT OUTER JOIN insert_weigth_class_rank_matrix
			ON insert_weigth_class_rank_matrix.grade_year = weigth_rank_expand.grade_year
			AND insert_weigth_class_rank_matrix.rank_name = weigth_rank_expand.class_name
)";
                        #endregion

                        sql += insertGradeYearClassRankSql;
                        bkw.ReportProgress(25);

                        if (!string.IsNullOrEmpty(tag1))
                        {
                            #region 計算類別一排名的Sql
                            string tag1RankSql = @"
-------計算類別一排名所需成績
,tag1_score_detail AS
( 
	SELECT
		student_list.student_id
   		,student_list.student_name
		,sc_attend.id AS sc_attend_id
		,course.course_name
		,course.school_year
		,course.semester
		,course.subject
		,course.domain
		,course.credit
		,weight.template_id
		,weight.name
		,weight.exam_weight
		,weight.assignment_weight
		,exam.id AS exam_id
		,exam.exam_name
		,student_list.rank_class_name
		,student_list.rank_grade_year	
		,student_list.tag1
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
		LEFT JOIN weight
			ON  weight.template_id=course.ref_exam_template_id
		INNER JOIN raw
			ON course.school_year = raw.school_year::int
			AND course.semester = raw.semester::int
			AND student_list.rank_grade_year = raw.grade_year::int
			AND exam.exam_name= raw.exam_name
			AND student_list.tag1 <> ''
)
,tag1_score_detail_avge AS
(
	SELECT	
		tag1_score_detail.*
		,(
			CASE 
				WHEN exam_score IS NOT NULL	
				THEN exam_score::Decimal
				ELSE  0
		 	END
			   * exam_weight::Decimal
			+
			CASE
				WHEN assignment_score IS NOT NULL  
				THEN	assignment_score::decimal
				ELSE 0
			END
			*assignment_weight
		)/(
			CASE 
				WHEN exam_score IS NOT NULL 
				THEN exam_weight
				ELSE 0
			END
			+
			CASE
				WHEN assignment_score IS NOT NULL 
				THEN assignment_weight
				ELSE 0
			END
		) AS score
	FROM 
		tag1_score_detail
	WHERE 
		(
			exam_score IS NOT NULL
			OR assignment_score IS NOT NULL
		)
		AND template_id IS NOT NULL	
)
-----領域成績類別排名所需成績
,tag1_group_score AS
(
	SELECT 
		student_id
		,student_name
		,domain
		,tag1
		,SUM
		(
			CASE 
				WHEN  tag1_score_detail_avge.score IS NOT NULL THEN tag1_score_detail_avge.score::decimal * tag1_score_detail_avge.credit::decimal  
				ELSE 0
			END
		) / 
		SUM
		( 
			CASE 
				WHEN  tag1_score_detail_avge.score IS NOT NULL THEN tag1_score_detail_avge.credit::decimal
				ELSE 0
			END
		)  AS score
	FROM  tag1_score_detail_avge
	GROUP BY	tag1, domain, student_id,student_name 
)
------加權平均成績類別排名所需成績
,tag1_scoreWavge AS
(
	SELECT
		tag1_score_detail_avge.student_id
		, tag1_score_detail_avge.tag1
		,SUM
		(
			CASE 
				WHEN  tag1_score_detail_avge.score IS NOT NULL THEN tag1_score_detail_avge.score::decimal * tag1_score_detail_avge.credit::decimal  
				ELSE 0
			END
		) / 
		SUM
		( 
			CASE 
				WHEN  tag1_score_detail_avge.score IS NOT NULL THEN tag1_score_detail_avge.credit::decimal
				ELSE 0
			END
		)  AS avge
	FROM 
		tag1_score_detail_avge
	GROUP BY 
		student_id
		, tag1_score_detail_avge.tag1
)
-------計算領域成績類別排名
,domain_tag1_rank_raw AS
(
	SELECT
		tag1_group_score.student_id
		, tag1_group_score.domain
		, tag1_group_score.tag1
		, student_list.rank_class_name AS class_name
		, student_list.rank_grade_year AS grade_year
		, tag1_group_score.score
		, RANK() OVER(PARTITION BY tag1_group_score.tag1, tag1_group_score.domain ORDER BY tag1_group_score.score DESC) AS category_rank
		, COUNT (tag1_group_score.student_id) OVER(PARTITION BY tag1_group_score.tag1, tag1_group_score.domain ) AS category_count
	FROM 
		tag1_group_score
			INNER JOIN student_list
				ON tag1_group_score.student_id = student_list.student_id
)
, domain_tag1_rank_expand AS 
(
	SELECT  
		domain_tag1_rank_raw.*
		,FLOOR((category_rank::DECIMAL-1)*100::DECIMAL / category_count)+1 AS categoryrank_percentage
	FROM 
		domain_tag1_rank_raw
	WHERE  
		domain IS NOT NULL	
)
-----新增領域成績類別一排名
, insert_domain_category1_rank_matrix AS
(
	INSERT INTO rank_matrix
	(
		school_year
		, semester
		, grade_year
		, item_type
		, ref_exam_id
		, item_name
		, rank_type
		, rank_name
		, is_alive
		, matrix_count
		, avg
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
		*
	FROM
	(
		SELECT
		--DISTINCT
			raw.school_year::INT AS school_year
			, raw.semester::INT AS semester
			, domain_tag1_rank_expand.grade_year AS grade_year
			, '定期評量/領域成績'::TEXT AS item_type
			, exam.id AS ref_exam_id
			, domain_tag1_rank_expand.domain AS item_name
			, '類別1排名'::TEXT AS rank_type
			, domain_tag1_rank_expand.tag1 AS rank_name
			, true AS is_alive
			, domain_tag1_rank_expand.category_count AS matrix_count
			, AVG(domain_tag1_rank_expand.Score::Decimal)OVER(PARTITION BY domain_tag1_rank_expand.tag1, domain_tag1_rank_expand.domain) AS avg
			, COUNT(*) FILTER (WHERE 100::DECIMAL<=domain_tag1_rank_expand.score::DECIMAL ) OVER(PARTITION BY domain_tag1_rank_expand.domain)AS level_gte100 
			, COUNT(*) FILTER (WHERE 90::DECIMAL<=domain_tag1_rank_expand.score AND domain_tag1_rank_expand.score <100::DECIMAL) OVER(PARTITION BY domain_tag1_rank_expand.tag1, domain_tag1_rank_expand.domain) AS level_90
			, COUNT(*) FILTER (WHERE 80::DECIMAL<=domain_tag1_rank_expand.score AND domain_tag1_rank_expand.score <90::DECIMAL)  OVER(PARTITION BY domain_tag1_rank_expand.tag1, domain_tag1_rank_expand.domain) AS level_80
			, COUNT(*) FILTER (WHERE 70::DECIMAL<=domain_tag1_rank_expand.score AND  domain_tag1_rank_expand.score <80::DECIMAL) OVER(PARTITION BY domain_tag1_rank_expand.tag1, domain_tag1_rank_expand.domain)AS level_70
			, COUNT(*) FILTER (WHERE 60::DECIMAL<=domain_tag1_rank_expand.score AND domain_tag1_rank_expand.score <70::DECIMAL)  OVER(PARTITION BY domain_tag1_rank_expand.tag1, domain_tag1_rank_expand.domain) AS level_60
			, COUNT(*) FILTER (WHERE 50::DECIMAL<=domain_tag1_rank_expand.score AND domain_tag1_rank_expand.score <60::DECIMAL)  OVER(PARTITION BY domain_tag1_rank_expand.tag1, domain_tag1_rank_expand.domain)AS level_50
			, COUNT(*) FILTER (WHERE 40::DECIMAL<=domain_tag1_rank_expand.score AND domain_tag1_rank_expand.score <50::DECIMAL)  OVER(PARTITION BY domain_tag1_rank_expand.tag1, domain_tag1_rank_expand.domain)AS level_40
			, COUNT(*) FILTER (WHERE 30::DECIMAL<=domain_tag1_rank_expand.score AND domain_tag1_rank_expand.score <40::DECIMAL)  OVER(PARTITION BY domain_tag1_rank_expand.tag1, domain_tag1_rank_expand.domain)AS level_30
			, COUNT(*) FILTER (WHERE 20::DECIMAL<=domain_tag1_rank_expand.score AND domain_tag1_rank_expand.score <30::DECIMAL)  OVER(PARTITION BY domain_tag1_rank_expand.tag1, domain_tag1_rank_expand.domain) AS level_20
			, COUNT(*) FILTER (WHERE 10::DECIMAL<=domain_tag1_rank_expand.score AND domain_tag1_rank_expand.score <20::DECIMAL)  OVER(PARTITION BY domain_tag1_rank_expand.tag1, domain_tag1_rank_expand.domain)AS level_10
			, COUNT(*) FILTER (WHERE domain_tag1_rank_expand.score<10::DECIMAL) OVER (PARTITION BY domain_tag1_rank_expand.tag1, domain_tag1_rank_expand.domain)AS level_lt10 
		FROM
			domain_tag1_rank_expand
			CROSS JOIN raw
			LEFT OUTER JOIN exam
				ON exam.exam_name = raw.exam_name  
	) AS sub
	GROUP BY
		sub.school_year
		,sub.semester
		,sub.grade_year
		,sub.ref_exam_id
		,sub.item_type
		,sub.item_name
		,sub.rank_type
		,sub.rank_name
		,sub.is_alive
		,sub.matrix_count
		,sub.avg
		,sub.level_gte100
		,sub.level_90
		,sub.level_80
		,sub.level_70
		,sub.level_60
		,sub.level_50
		,sub.level_40
		,sub.level_30
		,sub.level_20
		,sub.level_10
		,sub.level_lt10

	RETURNING *
)
, insert_domain_category1_rank_detail AS
(
	INSERT INTO rank_detail
	(
		ref_matrix_id
		, ref_student_id
		, score
		, rank
		, percentile
	)
	SELECT 
		insert_domain_category1_rank_matrix.id AS ref_matrix_id
		, domain_tag1_rank_expand.student_id AS ref_student_id
		, domain_tag1_rank_expand.score AS score
		, domain_tag1_rank_expand.category_rank AS rank
		, categoryrank_percentage AS percentile	
	FROM
		domain_tag1_rank_expand
		LEFT OUTER JOIN insert_domain_category1_rank_matrix
			ON insert_domain_category1_rank_matrix.grade_year = domain_tag1_rank_expand.grade_year
			AND insert_domain_category1_rank_matrix.item_name = domain_tag1_rank_expand.domain
)
--------計算科目成績類別排名
,subject_tag1_rank_raw AS
(
	SELECT
		tag1_score_detail_avge.student_id
		, tag1_score_detail_avge.rank_grade_year AS grade_year
		, tag1_score_detail_avge.rank_class_name AS class_name
		, tag1_score_detail_avge.subject
		, student_list.tag1
		, tag1_score_detail_avge.score
		, RANK() OVER(PARTITION BY student_list.tag1, tag1_score_detail_avge.subject ORDER BY score DESC) AS category_rank
		, COUNT (tag1_score_detail_avge.student_id) OVER(PARTITION BY student_list.tag1, tag1_score_detail_avge.subject ) AS category_count
	FROM tag1_score_detail_avge
    	INNER JOIN student_list
			ON student_list.student_id = tag1_score_detail_avge.student_id
)
, subject_tag1_rank_expand AS 
(
	SELECT  
		subject_tag1_rank_raw.*
		,FLOOR((category_rank::DECIMAL-1)*100::DECIMAL/category_count)+1 AS categoryrank_percentage
	FROM 
		subject_tag1_rank_raw
)
----新增科目成績類別一排名
, insert_subject_category1_rank_matrix AS
(
	INSERT INTO rank_matrix
	(
		school_year
		, semester
		, grade_year
		, item_type
		, ref_exam_id
		, item_name
		, rank_type
		, rank_name
		, is_alive
		, matrix_count
		, avg
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
		*
	FROM	
		(
			SELECT
				--DISTINCT
				raw.school_year::INT AS school_year
				, raw.semester::INT AS semester
				, subject_tag1_rank_expand.grade_year AS grade_year
				, '定期評量/科目成績'::TEXT AS item_type
				, exam.id AS ref_exam_id
				, subject_tag1_rank_expand.subject AS item_name
				, '類別1排名'::TEXT AS rank_type
				, subject_tag1_rank_expand.tag1 AS rank_name
				, true AS is_alive
				, subject_tag1_rank_expand.category_count AS matrix_count
				, AVG(subject_tag1_rank_expand.Score::Decimal)OVER(PARTITION BY subject_tag1_rank_expand.tag1, subject_tag1_rank_expand.subject) AS avg
				, COUNT(*) FILTER (WHERE 100::DECIMAL<=subject_tag1_rank_expand.score::DECIMAL ) OVER(PARTITION BY subject_tag1_rank_expand.subject)AS level_gte100 
				, COUNT(*) FILTER (WHERE 90::DECIMAL<=subject_tag1_rank_expand.score AND subject_tag1_rank_expand.score <100::DECIMAL) OVER(PARTITION BY subject_tag1_rank_expand.tag1, subject_tag1_rank_expand.subject) AS level_90
				, COUNT(*) FILTER (WHERE 80::DECIMAL<=subject_tag1_rank_expand.score AND subject_tag1_rank_expand.score <90::DECIMAL)  OVER(PARTITION BY subject_tag1_rank_expand.tag1, subject_tag1_rank_expand.subject) AS level_80
				, COUNT(*) FILTER (WHERE 70::DECIMAL<=subject_tag1_rank_expand.score AND  subject_tag1_rank_expand.score <80::DECIMAL) OVER(PARTITION BY subject_tag1_rank_expand.tag1, subject_tag1_rank_expand.subject)AS level_70
				, COUNT(*) FILTER (WHERE 60::DECIMAL<=subject_tag1_rank_expand.score AND subject_tag1_rank_expand.score <70::DECIMAL)  OVER(PARTITION BY subject_tag1_rank_expand.tag1, subject_tag1_rank_expand.subject) AS level_60
				, COUNT(*) FILTER (WHERE 50::DECIMAL<=subject_tag1_rank_expand.score AND subject_tag1_rank_expand.score <60::DECIMAL)  OVER(PARTITION BY subject_tag1_rank_expand.tag1, subject_tag1_rank_expand.subject)AS level_50
				, COUNT(*) FILTER (WHERE 40::DECIMAL<=subject_tag1_rank_expand.score AND subject_tag1_rank_expand.score <50::DECIMAL)  OVER(PARTITION BY subject_tag1_rank_expand.tag1, subject_tag1_rank_expand.subject)AS level_40
				, COUNT(*) FILTER (WHERE 30::DECIMAL<=subject_tag1_rank_expand.score AND subject_tag1_rank_expand.score <40::DECIMAL)  OVER(PARTITION BY subject_tag1_rank_expand.tag1, subject_tag1_rank_expand.subject)AS level_30
				, COUNT(*) FILTER (WHERE 20::DECIMAL<=subject_tag1_rank_expand.score AND subject_tag1_rank_expand.score <30::DECIMAL)  OVER(PARTITION BY subject_tag1_rank_expand.tag1, subject_tag1_rank_expand.subject) AS level_20
				, COUNT(*) FILTER (WHERE 10::DECIMAL<=subject_tag1_rank_expand.score AND subject_tag1_rank_expand.score <20::DECIMAL)  OVER(PARTITION BY subject_tag1_rank_expand.tag1, subject_tag1_rank_expand.subject)AS level_10
				, COUNT(*) FILTER (WHERE subject_tag1_rank_expand.score<10::DECIMAL) OVER (PARTITION BY subject_tag1_rank_expand.tag1, subject_tag1_rank_expand.subject)AS level_lt10 
			FROM
				subject_tag1_rank_expand
					CROSS JOIN raw
					LEFT OUTER JOIN exam
						ON exam.exam_name = raw.exam_name
	) AS sub
	GROUP BY
		sub.school_year
		,sub.semester
		,sub.grade_year
		,sub.ref_exam_id
		,sub.item_type
		,sub.item_name
		,sub.rank_type
		,sub.rank_name
		,sub.is_alive
		,sub.matrix_count
		,sub.avg
		,sub.level_gte100
		,sub.level_90
		,sub.level_80
		,sub.level_70
		,sub.level_60
		,sub.level_50
		,sub.level_40
		,sub.level_30
		,sub.level_20
		,sub.level_10
		,sub.level_lt10
	RETURNING *
)
, insert_subject_category1_rank_detail AS
(
	INSERT INTO rank_detail
	(
		ref_matrix_id
		, ref_student_id
		, score
		, rank
		, percentile
	)
	SELECT 
		insert_subject_category1_rank_matrix.id AS ref_matrix_id
		, subject_tag1_rank_expand.student_id AS ref_student_id
		, subject_tag1_rank_expand.score AS score
		, subject_tag1_rank_expand.category_rank AS rank
		, subject_tag1_rank_expand.categoryrank_percentage AS percentile	
	FROM
		subject_tag1_rank_expand
		LEFT OUTER JOIN insert_subject_category1_rank_matrix
			ON insert_subject_category1_rank_matrix.grade_year = subject_tag1_rank_expand.grade_year
			AND insert_subject_category1_rank_matrix.item_name = subject_tag1_rank_expand.subject
)
-----------計算加權平均成績類別排名
,weight_tag1_rank_raw AS
(
	SELECT 
		tag1_scoreWavge.student_id
		, student_list.rank_grade_year AS grade_year
		, student_list.rank_class_name AS class_name
		, tag1_scoreWavge.avge AS score
		, student_list.tag1
		, RANK() OVER(ORDER BY avge DESC) AS category_rank
		, COUNT (*) OVER(PARTITION BY student_list.tag1) AS category_count
	FROM 
		tag1_scoreWavge
		INNER JOIN student_list
			ON tag1_scoreWavge.student_id = student_list.student_id
)
, weight_tag1_rank_expand AS
(
	SELECT  
		weight_tag1_rank_raw.*
		,FLOOR((category_rank::DECIMAL-1)*100::DECIMAL/category_count)+1 AS categoryrank_percentage
	FROM 
		weight_tag1_rank_raw
)
-----新增加權平均成績類別一排名
, insert_weigth_category1_rank_matrix AS
(
	INSERT INTO rank_matrix
	(
		school_year
		, semester
		, grade_year
		, item_type
		, ref_exam_id
		, item_name
		, rank_type
		, rank_name
		, is_alive
		, matrix_count
		, avg
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
		*
	FROM
	(
		SELECT
			--DISTINCT
			raw.school_year::INT AS school_year
			, raw.semester::INT AS semester
			, weight_tag1_rank_expand.grade_year AS grade_year
			, '定期評量/總計成績'::text AS item_type
			, exam.id AS ref_exam_id
			, '加權平均'::text AS item_name
			, '類別1排名'::text AS rank_type
			, weight_tag1_rank_expand.tag1 AS rank_name
			, true AS is_alive
			, weight_tag1_rank_expand.category_count AS matrix_count
			, AVG(weight_tag1_rank_expand.Score::Decimal)OVER(PARTITION BY weight_tag1_rank_expand.tag1) AS avg
			, COUNT(*) FILTER (WHERE 100::DECIMAL<=weight_tag1_rank_expand.score::DECIMAL ) OVER(PARTITION BY weight_tag1_rank_expand.tag1)AS level_gte100 
			, COUNT(*) FILTER (WHERE 90::DECIMAL<=weight_tag1_rank_expand.score AND weight_tag1_rank_expand.score <100::DECIMAL) OVER(PARTITION BY weight_tag1_rank_expand.tag1) AS level_90
			, COUNT(*) FILTER (WHERE 80::DECIMAL<=weight_tag1_rank_expand.score AND weight_tag1_rank_expand.score <90::DECIMAL)  OVER(PARTITION BY weight_tag1_rank_expand.tag1) AS level_80
			, COUNT(*) FILTER (WHERE 70::DECIMAL<=weight_tag1_rank_expand.score AND weight_tag1_rank_expand.score <80::DECIMAL)  OVER(PARTITION BY weight_tag1_rank_expand.tag1)AS level_70
			, COUNT(*) FILTER (WHERE 60::DECIMAL<=weight_tag1_rank_expand.score AND weight_tag1_rank_expand.score <70::DECIMAL)  OVER(PARTITION BY weight_tag1_rank_expand.tag1) AS level_60
			, COUNT(*) FILTER (WHERE 50::DECIMAL<=weight_tag1_rank_expand.score AND weight_tag1_rank_expand.score <60::DECIMAL)  OVER(PARTITION BY weight_tag1_rank_expand.tag1)AS level_50
			, COUNT(*) FILTER (WHERE 40::DECIMAL<=weight_tag1_rank_expand.score AND weight_tag1_rank_expand.score <50::DECIMAL)  OVER(PARTITION BY weight_tag1_rank_expand.tag1)AS level_40
			, COUNT(*) FILTER (WHERE 30::DECIMAL<=weight_tag1_rank_expand.score AND weight_tag1_rank_expand.score <40::DECIMAL)  OVER(PARTITION BY weight_tag1_rank_expand.tag1)AS level_30
			, COUNT(*) FILTER (WHERE 20::DECIMAL<=weight_tag1_rank_expand.score AND weight_tag1_rank_expand.score <30::DECIMAL)  OVER(PARTITION BY weight_tag1_rank_expand.tag1) AS level_20
			, COUNT(*) FILTER (WHERE 10::DECIMAL<=weight_tag1_rank_expand.score AND weight_tag1_rank_expand.score <20::DECIMAL)  OVER(PARTITION BY weight_tag1_rank_expand.tag1)AS level_10
			, COUNT(*) FILTER (WHERE weight_tag1_rank_expand.score<10::DECIMAL) OVER (PARTITION BY weight_tag1_rank_expand.tag1)AS level_lt10
		FROM
			weight_tag1_rank_expand
			CROSS JOIN raw
			LEFT OUTER JOIN exam
				ON exam.exam_name = raw.exam_name
	)AS sub
	GROUP BY
	sub.school_year
	,sub.semester
	,sub.grade_year
	,sub.ref_exam_id
	,sub.item_type
	,sub.item_name
	,sub.rank_type
	,sub.rank_name
	,sub.is_alive
	,sub.matrix_count
	,sub.avg
	,sub.level_gte100
	,sub.level_90
	,sub.level_80
	,sub.level_70
	,sub.level_60
	,sub.level_50
	,sub.level_40
	,sub.level_30
	,sub.level_20
	,sub.level_10
	,sub.level_lt10

	RETURNING *
)
, insert_weigth_category1_rank_detail AS
(
	INSERT INTO rank_detail
	(
		ref_matrix_id
		, ref_student_id
		, score
		, rank
		, percentile
	)
	SELECT 
		insert_weigth_category1_rank_matrix.id AS ref_matrix_id
		, weight_tag1_rank_expand.student_id AS ref_student_id
		, weight_tag1_rank_expand.score AS score
		, weight_tag1_rank_expand.category_rank AS rank
		, weight_tag1_rank_expand.categoryrank_percentage AS percentile
	FROM
		weight_tag1_rank_expand
		LEFT OUTER JOIN insert_weigth_category1_rank_matrix
			ON insert_weigth_category1_rank_matrix.grade_year = weight_tag1_rank_expand.grade_year
)";
                            #endregion

                            sql += tag1RankSql;
                        }
                        bkw.ReportProgress(50);

                        if (!string.IsNullOrEmpty(tag2))
                        {
                            #region 計算類別二排名的Sql
                            string tag2RankSql = @"
-------計算類別二排名所需成績
,tag2_score_detail AS
( 
	SELECT
		student_list.student_id
   		,student_list.student_name
		,sc_attend.id AS sc_attend_id
		,course.course_name
		,course.school_year
		,course.semester
		,course.subject
		,course.domain
		,course.credit
		,weight.template_id
		,weight.name
		,weight.exam_weight
		,weight.assignment_weight
		,exam.id AS exam_id
		,exam.exam_name
		,student_list.rank_class_name
		,student_list.rank_grade_year
		,student_list.tag2
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
		LEFT JOIN weight
			ON  weight.template_id=course.ref_exam_template_id
		INNER JOIN raw
			ON course.school_year = raw.school_year::int
			AND course.semester = raw.semester::int
			AND student_list.rank_grade_year = raw.grade_year::int
			AND exam.exam_name= raw.exam_name
			AND student_list.tag2 <> ''
)
,tag2_score_detail_avge AS
(
	SELECT	
		tag2_score_detail.*
		,(
			CASE 
				WHEN exam_score IS NOT NULL	
				THEN exam_score::Decimal
				ELSE  0
		 	END
			   * exam_weight::Decimal
			+
			CASE
				WHEN assignment_score IS NOT NULL  
				THEN	assignment_score::decimal
				ELSE 0
			END
			*assignment_weight
		)/(
			CASE 
				WHEN exam_score IS NOT NULL 
				THEN exam_weight
				ELSE 0
			END
			+
			CASE
				WHEN assignment_score IS NOT NULL 
				THEN assignment_weight
				ELSE 0
			END
		) AS score
	FROM 
		tag2_score_detail
	WHERE 
		(
			exam_score IS NOT NULL
			OR assignment_score IS NOT NULL
		)
		AND template_id IS NOT NULL	
)
-----領域成績類別排名所需成績
,tag2_group_score AS
(
	SELECT 
		student_id
		,student_name
		,domain
		,tag2
		,SUM
		(
			CASE 
				WHEN  tag2_score_detail_avge.score IS NOT NULL THEN tag2_score_detail_avge.score::decimal * tag2_score_detail_avge.credit::decimal  
				ELSE 0
			END
		) / 
		SUM
		( 
			CASE 
				WHEN  tag2_score_detail_avge.score IS NOT NULL THEN tag2_score_detail_avge.credit::decimal
				ELSE 0
			END
		)  AS score
	FROM  tag2_score_detail_avge
	GROUP BY   tag2, domain, student_id,student_name 
)
------加權平均成績類別排名所需成績
,tag2_scoreWavge AS
(
	SELECT
		tag2_score_detail_avge.student_id
		, tag2_score_detail_avge.tag2
		,SUM
		(
			CASE 
				WHEN  tag2_score_detail_avge.score IS NOT NULL THEN tag2_score_detail_avge.score::decimal * tag2_score_detail_avge.credit::decimal  
				ELSE 0
			END
		) / 
		SUM
		( 
			CASE 
				WHEN  tag2_score_detail_avge.score IS NOT NULL THEN tag2_score_detail_avge.credit::decimal
				ELSE 0
			END
		)  AS avge
	FROM 
		tag2_score_detail_avge
	GROUP BY 
		student_id
		, tag2_score_detail_avge.tag2
)
-------計算領域成績類別排名
,domain_tag2_rank_raw AS
(
	SELECT
		tag2_group_score.student_id
		, tag2_group_score.domain
		, student_list.rank_class_name AS class_name
		, student_list.rank_grade_year AS grade_year
		, student_list.tag2
		, tag2_group_score.score
		, RANK() OVER(PARTITION BY tag2_group_score.tag2, tag2_group_score.domain ORDER BY tag2_group_score.score DESC) AS category_rank
		, COUNT (tag2_group_score.student_id) OVER(PARTITION BY tag2_group_score.tag2, tag2_group_score.domain ) AS category_count
	FROM 
		tag2_group_score
		INNER JOIN student_list
			ON tag2_group_score.student_id = student_list.student_id
)
, domain_tag2_rank_expand AS 
(
	SELECT  
		domain_tag2_rank_raw.*
		,FLOOR((category_rank::DECIMAL-1)*100::DECIMAL / category_count)+1 AS categoryrank_percentage
	FROM 
		domain_tag2_rank_raw
	WHERE  
		domain IS NOT NULL	
)
-----新增領域成績類別二排名
, insert_domain_category2_rank_matrix AS
(
	INSERT INTO rank_matrix
	(
		school_year
		, semester
		, grade_year
		, item_type
		, ref_exam_id
		, item_name
		, rank_type
		, rank_name
		, is_alive
		, matrix_count
		, avg
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
		*
	FROM
	(
		SELECT
		--DISTINCT
			raw.school_year::INT AS school_year
			, raw.semester::INT AS semester
			, domain_tag2_rank_expand.grade_year AS grade_year
			, '定期評量/領域成績'::TEXT AS item_type
			, exam.id AS ref_exam_id
			, domain_tag2_rank_expand.domain AS item_name
			, '類別2排名'::TEXT AS rank_type
			, domain_tag2_rank_expand.tag2 AS rank_name
			, true AS is_alive
			, domain_tag2_rank_expand.category_count AS matrix_count
			, AVG(domain_tag2_rank_expand.Score::Decimal)OVER(PARTITION BY domain_tag2_rank_expand.tag2, domain_tag2_rank_expand.domain) AS avg
			, COUNT(*) FILTER (WHERE 100::DECIMAL<=domain_tag2_rank_expand.score::DECIMAL ) OVER(PARTITION BY domain_tag2_rank_expand.tag2, domain_tag2_rank_expand.domain)AS level_gte100 
			, COUNT(*) FILTER (WHERE 90::DECIMAL<=domain_tag2_rank_expand.score AND domain_tag2_rank_expand.score <100::DECIMAL) OVER(PARTITION BY domain_tag2_rank_expand.tag2, domain_tag2_rank_expand.domain) AS level_90
			, COUNT(*) FILTER (WHERE 80::DECIMAL<=domain_tag2_rank_expand.score AND domain_tag2_rank_expand.score <90::DECIMAL)  OVER(PARTITION BY domain_tag2_rank_expand.tag2, domain_tag2_rank_expand.domain) AS level_80
			, COUNT(*) FILTER (WHERE 70::DECIMAL<=domain_tag2_rank_expand.score AND domain_tag2_rank_expand.score <80::DECIMAL)  OVER(PARTITION BY domain_tag2_rank_expand.tag2, domain_tag2_rank_expand.domain)AS level_70
			, COUNT(*) FILTER (WHERE 60::DECIMAL<=domain_tag2_rank_expand.score AND domain_tag2_rank_expand.score <70::DECIMAL)  OVER(PARTITION BY domain_tag2_rank_expand.tag2, domain_tag2_rank_expand.domain) AS level_60
			, COUNT(*) FILTER (WHERE 50::DECIMAL<=domain_tag2_rank_expand.score AND domain_tag2_rank_expand.score <60::DECIMAL)  OVER(PARTITION BY domain_tag2_rank_expand.tag2, domain_tag2_rank_expand.domain)AS level_50
			, COUNT(*) FILTER (WHERE 40::DECIMAL<=domain_tag2_rank_expand.score AND domain_tag2_rank_expand.score <50::DECIMAL)  OVER(PARTITION BY domain_tag2_rank_expand.tag2, domain_tag2_rank_expand.domain)AS level_40
			, COUNT(*) FILTER (WHERE 30::DECIMAL<=domain_tag2_rank_expand.score AND domain_tag2_rank_expand.score <40::DECIMAL)  OVER(PARTITION BY domain_tag2_rank_expand.tag2, domain_tag2_rank_expand.domain)AS level_30
			, COUNT(*) FILTER (WHERE 20::DECIMAL<=domain_tag2_rank_expand.score AND domain_tag2_rank_expand.score <30::DECIMAL)  OVER(PARTITION BY domain_tag2_rank_expand.tag2, domain_tag2_rank_expand.domain) AS level_20
			, COUNT(*) FILTER (WHERE 10::DECIMAL<=domain_tag2_rank_expand.score AND domain_tag2_rank_expand.score <20::DECIMAL)  OVER(PARTITION BY domain_tag2_rank_expand.tag2, domain_tag2_rank_expand.domain)AS level_10
			, COUNT(*) FILTER (WHERE domain_tag2_rank_expand.score<10::DECIMAL) OVER (PARTITION BY domain_tag2_rank_expand.tag2, domain_tag2_rank_expand.domain)AS level_lt10 
		FROM
			domain_tag2_rank_expand
			CROSS JOIN raw
			LEFT OUTER JOIN exam
				ON exam.exam_name = raw.exam_name  
	) AS sub
	GROUP BY
		sub.school_year
		,sub.semester
		,sub.grade_year
		,sub.ref_exam_id
		,sub.item_type
		,sub.item_name
		,sub.rank_type
		,sub.rank_name
		,sub.is_alive
		,sub.matrix_count
		,sub.avg
		,sub.level_gte100
		,sub.level_90
		,sub.level_80
		,sub.level_70
		,sub.level_60
		,sub.level_50
		,sub.level_40
		,sub.level_30
		,sub.level_20
		,sub.level_10
		,sub.level_lt10

	RETURNING *
)
, insert_domain_category2_rank_detail AS
(
	INSERT INTO rank_detail
	(
		ref_matrix_id
		, ref_student_id
		, score
		, rank
		, percentile
	)
	SELECT 
		insert_domain_category2_rank_matrix.id AS ref_matrix_id
		, domain_tag2_rank_expand.student_id AS ref_student_id
		, domain_tag2_rank_expand.score AS score
		, domain_tag2_rank_expand.category_rank AS rank
		, categoryrank_percentage AS percentile	
	FROM
		domain_tag2_rank_expand
		LEFT OUTER JOIN insert_domain_category2_rank_matrix
			ON insert_domain_category2_rank_matrix.grade_year = domain_tag2_rank_expand.grade_year
			AND insert_domain_category2_rank_matrix.item_name=domain_tag2_rank_expand.domain
)
--------計算科目成績類別排名
,subject_tag2_rank_raw AS
(
	SELECT
		tag2_score_detail_avge.student_id
		, tag2_score_detail_avge.rank_grade_year AS grade_year
		, tag2_score_detail_avge.rank_class_name AS class_name
		, tag2_score_detail_avge.subject
		, tag2_score_detail_avge.tag2
		, tag2_score_detail_avge.score
		, RANK() OVER(PARTITION BY tag2_score_detail_avge.tag2, tag2_score_detail_avge.subject ORDER BY score DESC) AS category_rank
		, COUNT (tag2_score_detail_avge.student_id) OVER(PARTITION BY tag2_score_detail_avge.tag2, tag2_score_detail_avge.subject ) AS category_count
	FROM 
		tag2_score_detail_avge
    	INNER JOIN student_list
			ON tag2_score_detail_avge.student_id = student_list.student_id
)
, subject_tag2_rank_expand AS 
(
	SELECT  
		subject_tag2_rank_raw.*
		,FLOOR((category_rank::DECIMAL-1)*100::DECIMAL/category_count)+1 AS categoryrank_percentage
	FROM 
		subject_tag2_rank_raw
)
----新增科目排名類別二排名
, insert_subject_category2_rank_matrix AS
(
	INSERT INTO rank_matrix
	(
		school_year
		, semester
		, grade_year
		, item_type
		, ref_exam_id
		, item_name
		, rank_type
		, rank_name
		, is_alive
		, matrix_count
		, avg
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
		*
	FROM	
		(
			SELECT
				--DISTINCT
				raw.school_year::INT AS school_year
				, raw.semester::INT AS semester
				, subject_tag2_rank_expand.grade_year AS grade_year
				, '定期評量/科目成績'::TEXT AS item_type
				, exam.id AS ref_exam_id
				, subject_tag2_rank_expand.subject AS item_name
				, '類別2排名'::TEXT AS rank_type
				, subject_tag2_rank_expand.tag2 AS rank_name
				, true AS is_alive
				, subject_tag2_rank_expand.category_count AS matrix_count
				, AVG(subject_tag2_rank_expand.Score::Decimal)OVER(PARTITION BY subject_tag2_rank_expand.tag2, subject_tag2_rank_expand.subject) AS avg
				, COUNT(*) FILTER (WHERE 100::DECIMAL<=subject_tag2_rank_expand.score::DECIMAL ) OVER(PARTITION BY subject_tag2_rank_expand.tag2, subject_tag2_rank_expand.subject)AS level_gte100 
				, COUNT(*) FILTER (WHERE 90::DECIMAL<=subject_tag2_rank_expand.score AND subject_tag2_rank_expand.score <100::DECIMAL) OVER(PARTITION BY subject_tag2_rank_expand.tag2, subject_tag2_rank_expand.subject) AS level_90
				, COUNT(*) FILTER (WHERE 80::DECIMAL<=subject_tag2_rank_expand.score AND subject_tag2_rank_expand.score <90::DECIMAL)  OVER(PARTITION BY subject_tag2_rank_expand.tag2, subject_tag2_rank_expand.subject) AS level_80
				, COUNT(*) FILTER (WHERE 70::DECIMAL<=subject_tag2_rank_expand.score AND subject_tag2_rank_expand.score <80::DECIMAL)  OVER(PARTITION BY subject_tag2_rank_expand.tag2, subject_tag2_rank_expand.subject)AS level_70
				, COUNT(*) FILTER (WHERE 60::DECIMAL<=subject_tag2_rank_expand.score AND subject_tag2_rank_expand.score <70::DECIMAL)  OVER(PARTITION BY subject_tag2_rank_expand.tag2, subject_tag2_rank_expand.subject) AS level_60
				, COUNT(*) FILTER (WHERE 50::DECIMAL<=subject_tag2_rank_expand.score AND subject_tag2_rank_expand.score <60::DECIMAL)  OVER(PARTITION BY subject_tag2_rank_expand.tag2, subject_tag2_rank_expand.subject)AS level_50
				, COUNT(*) FILTER (WHERE 40::DECIMAL<=subject_tag2_rank_expand.score AND subject_tag2_rank_expand.score <50::DECIMAL)  OVER(PARTITION BY subject_tag2_rank_expand.tag2, subject_tag2_rank_expand.subject)AS level_40
				, COUNT(*) FILTER (WHERE 30::DECIMAL<=subject_tag2_rank_expand.score AND subject_tag2_rank_expand.score <40::DECIMAL)  OVER(PARTITION BY subject_tag2_rank_expand.tag2, subject_tag2_rank_expand.subject)AS level_30
				, COUNT(*) FILTER (WHERE 20::DECIMAL<=subject_tag2_rank_expand.score AND subject_tag2_rank_expand.score <30::DECIMAL)  OVER(PARTITION BY subject_tag2_rank_expand.tag2, subject_tag2_rank_expand.subject) AS level_20
				, COUNT(*) FILTER (WHERE 10::DECIMAL<=subject_tag2_rank_expand.score AND subject_tag2_rank_expand.score <20::DECIMAL)  OVER(PARTITION BY subject_tag2_rank_expand.tag2, subject_tag2_rank_expand.subject)AS level_10
				, COUNT(*) FILTER (WHERE subject_tag2_rank_expand.score<10::DECIMAL) OVER (PARTITION BY subject_tag2_rank_expand.tag2, subject_tag2_rank_expand.subject)AS level_lt10 
			FROM
				subject_tag2_rank_expand
					CROSS JOIN raw
					LEFT OUTER JOIN exam
						ON exam.exam_name = raw.exam_name
	) AS sub
	GROUP BY
		sub.school_year
		,sub.semester
		,sub.grade_year
		,sub.ref_exam_id
		,sub.item_type
		,sub.item_name
		,sub.rank_type
		,sub.rank_name
		,sub.is_alive
		,sub.matrix_count
		,sub.avg
		,sub.level_gte100
		,sub.level_90
		,sub.level_80
		,sub.level_70
		,sub.level_60
		,sub.level_50
		,sub.level_40
		,sub.level_30
		,sub.level_20
		,sub.level_10
		,sub.level_lt10
	RETURNING *
)
, insert_subject_category2_rank_detail AS
(
	INSERT INTO rank_detail
	(
		ref_matrix_id
		, ref_student_id
		, score
		, rank
		, percentile
	)
	SELECT 
		insert_subject_category2_rank_matrix.id AS ref_matrix_id
		, subject_tag2_rank_expand.student_id AS ref_student_id
		, subject_tag2_rank_expand.score AS score
		, subject_tag2_rank_expand.category_rank AS rank
		, subject_tag2_rank_expand.categoryrank_percentage AS percentile	
	FROM
		subject_tag2_rank_expand
		LEFT OUTER JOIN insert_subject_category2_rank_matrix
			ON insert_subject_category2_rank_matrix.grade_year = subject_tag2_rank_expand.grade_year
			AND insert_subject_category2_rank_matrix.item_name = subject_tag2_rank_expand.subject
)
-----------計算加權平均成績類別排名
,weight_tag2_rank_raw AS
(
	SELECT 
		tag2_scoreWavge.student_id
		, student_list.rank_grade_year AS grade_year
		, student_list.rank_class_name AS class_name
		, tag2_scoreWavge.avge AS score
		, tag2_scoreWavge.tag2
		, RANK() OVER(ORDER BY avge DESC) AS category_rank
		, COUNT (*) OVER(PARTITION BY tag2_scoreWavge.tag2) AS category_count
	FROM 
		tag2_scoreWavge
		INNER JOIN student_list
			ON tag2_scoreWavge.student_id = student_list.student_id
)
, weight_tag2_rank_expand AS
(
	SELECT  
		weight_tag2_rank_raw.*
		,FLOOR((category_rank::DECIMAL-1)*100::DECIMAL/category_count)+1 AS categoryrank_percentage
	FROM 
		weight_tag2_rank_raw
)
-----新增加權平均類別二排名
, insert_weigth_category2_rank_matrix AS
(
	INSERT INTO rank_matrix
	(
		school_year
		, semester
		, grade_year
		, item_type
		, ref_exam_id
		, item_name
		, rank_type
		, rank_name
		, is_alive
		, matrix_count
		, avg
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
		* 
	FROM
	(
		SELECT
		--DISTINCT
			raw.school_year::INT AS school_year
			, raw.semester::INT AS semester
			, weight_tag2_rank_expand.grade_year AS grade_year
			, '定期評量/總計成績'::TEXT AS item_type
			, exam.id AS ref_exam_id
			, '加權平均'::TEXT AS item_name
			, '類別2排名'::TEXT AS rank_type
			, weight_tag2_rank_expand.tag2 AS rank_name
			, true AS is_alive
			, weight_tag2_rank_expand.category_count AS matrix_count
			, AVG(weight_tag2_rank_expand.score::Decimal)OVER(PARTITION BY weight_tag2_rank_expand.tag2) AS avg
			, COUNT(*) FILTER (WHERE 100::DECIMAL<=weight_tag2_rank_expand.score::DECIMAL ) OVER(PARTITION BY weight_tag2_rank_expand.tag2)AS level_gte100 
			, COUNT(*) FILTER (WHERE 90::DECIMAL<=weight_tag2_rank_expand.score AND weight_tag2_rank_expand.score <100::DECIMAL) OVER(PARTITION BY weight_tag2_rank_expand.tag2) AS level_90
			, COUNT(*) FILTER (WHERE 80::DECIMAL<=weight_tag2_rank_expand.score AND weight_tag2_rank_expand.score <90::DECIMAL)  OVER(PARTITION BY weight_tag2_rank_expand.tag2) AS level_80
			, COUNT(*) FILTER (WHERE 70::DECIMAL<=weight_tag2_rank_expand.score AND weight_tag2_rank_expand.score <80::DECIMAL)  OVER(PARTITION BY weight_tag2_rank_expand.tag2)AS level_70
			, COUNT(*) FILTER (WHERE 60::DECIMAL<=weight_tag2_rank_expand.score AND weight_tag2_rank_expand.score <70::DECIMAL)  OVER(PARTITION BY weight_tag2_rank_expand.tag2) AS level_60
			, COUNT(*) FILTER (WHERE 50::DECIMAL<=weight_tag2_rank_expand.score AND weight_tag2_rank_expand.score <60::DECIMAL)  OVER(PARTITION BY weight_tag2_rank_expand.tag2)AS level_50
			, COUNT(*) FILTER (WHERE 40::DECIMAL<=weight_tag2_rank_expand.score AND weight_tag2_rank_expand.score <50::DECIMAL)  OVER(PARTITION BY weight_tag2_rank_expand.tag2)AS level_40
			, COUNT(*) FILTER (WHERE 30::DECIMAL<=weight_tag2_rank_expand.score AND weight_tag2_rank_expand.score <40::DECIMAL)  OVER(PARTITION BY weight_tag2_rank_expand.tag2)AS level_30
			, COUNT(*) FILTER (WHERE 20::DECIMAL<=weight_tag2_rank_expand.score AND weight_tag2_rank_expand.score <30::DECIMAL)  OVER(PARTITION BY weight_tag2_rank_expand.tag2) AS level_20
			, COUNT(*) FILTER (WHERE 10::DECIMAL<=weight_tag2_rank_expand.score AND weight_tag2_rank_expand.score <20::DECIMAL)  OVER(PARTITION BY weight_tag2_rank_expand.tag2)AS level_10
			, COUNT(*) FILTER (WHERE weight_tag2_rank_expand.score<10::DECIMAL) OVER (PARTITION BY weight_tag2_rank_expand.tag2)AS level_lt10 
		FROM
			weight_tag2_rank_expand
				CROSS JOIN raw
				LEFT OUTER JOIN exam
					ON exam.exam_name = raw.exam_name
	)AS sub
	GROUP BY
		sub.school_year
		,sub.semester
		,sub.grade_year
		,sub.ref_exam_id
		,sub.item_type
		,sub.item_name
		,sub.rank_type
		,sub.rank_name
		,sub.is_alive
		,sub.matrix_count
		,sub.avg
		,sub.level_gte100
		,sub.level_90
		,sub.level_80
		,sub.level_70
		,sub.level_60
		,sub.level_50
		,sub.level_40
		,sub.level_30
		,sub.level_20
		,sub.level_10
		,sub.level_lt10

	RETURNING *
)
, insert_weigth_category2_rank_detail AS
(
	INSERT INTO rank_detail(
		ref_matrix_id
		, ref_student_id
		, score
		, rank
		, percentile
	)
	SELECT 
		insert_weigth_category2_rank_matrix.id AS ref_matrix_id
		, weight_tag2_rank_expand.student_id AS ref_student_id
		, weight_tag2_rank_expand.score AS score
		, weight_tag2_rank_expand.category_rank AS rank
		, weight_tag2_rank_expand.categoryrank_percentage AS percentile	
	FROM
		weight_tag2_rank_expand
		LEFT OUTER JOIN insert_weigth_category2_rank_matrix
			ON insert_weigth_category2_rank_matrix.grade_year = weight_tag2_rank_expand.grade_year
			AND insert_weigth_category2_rank_matrix.rank_name = weight_tag2_rank_expand.tag2
)";
                            #endregion

                            sql += tag2RankSql;
                        }
                        bkw.ReportProgress(75);

                        #region 接在最後的SELECT字串
                        string selectSql = @"
SELECT * FROM rank_matrix";
                        #endregion

                        sql += selectSql;

                        QueryHelper queryHelper = new QueryHelper();
                        queryHelper.Select(sql);
                        bkw.ReportProgress(100);
                    }
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
                    throw new Exception("計算成績失敗", bkwException);
                }
                MessageBox.Show("計算完成");
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

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            #region 清除Panel裡的CheckBox
            foreach (CheckBox checkBox in _CheckBoxList)
            {
                plStudentView.Controls.Remove(checkBox);
            }
            #endregion

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
