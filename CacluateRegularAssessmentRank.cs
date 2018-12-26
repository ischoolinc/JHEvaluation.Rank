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

            #region 查詢學年度、學期和年級
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
                _ExamList = K12.Data.Exam.SelectAll();
                _TagConfigRecord = K12.Data.TagConfig.SelectByCategory(TagCategory.Student);
                _StudentRecord = K12.Data.Student.SelectAll();
            }
            catch (Exception ex)
            {
                throw new Exception("資料讀取失敗", ex);
            }
        }

        DataTable _SchoolYearTable = new DataTable();
        string _DefaultSchoolYear = "";
        List<ExamRecord> _ExamList = new List<ExamRecord>();
        List<TagConfigRecord> _TagConfigRecord = new List<TagConfigRecord>();
        List<StudentRecord> _StudentRecord = new List<StudentRecord>();
        List<CheckBox> _CheckBoxList = new List<CheckBox>();
        List<StudentRecord> _FilterStudentLIst = new List<StudentRecord>();
        int _FormWidth = 0, _FormHeight = 0;

        private void CacluateRegularAssessmentRank_Load(object sender, EventArgs e)
        {
            //讓Form回到起始狀態
            plStudentView.Visible = false;
            this.Width = 580;
            this.Height = 330;

            #region 篩選資料
            _StudentRecord = _StudentRecord.Where(x => !string.IsNullOrEmpty(x.RefClassID)
                                                    && (x.Status == StudentRecord.StudentStatus.一般 || x.Status == StudentRecord.StudentStatus.延修)
                                                    && x.Class.GradeYear != null).ToList();
            #endregion

            #region 填資料進ComboBox
            cboSchoolYear.Items.Clear();
            cboSemester.Items.Clear();
            cboExamType.Items.Clear();
            cboStudentFIlter.Items.Clear();
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
            cboSemester.SelectedIndex = 0;
            cboSchoolYear.SelectedIndex = 0;

            cboStudentFIlter.Items.Add("");
            cboStudentTag1.Items.Add("");
            cboStudentTag2.Items.Add("");
            foreach (var item in _TagConfigRecord.Select(x => x.Name).Distinct())
            {
                cboStudentFIlter.Items.Add(item);
                cboStudentTag1.Items.Add(item);
                cboStudentTag2.Items.Add(item);
            }
            cboStudentFIlter.SelectedIndex = 0;
            cboStudentTag1.SelectedIndex = 0;
            cboStudentTag2.SelectedIndex = 0;

            foreach (var item in _ExamList.Select(x => x.Name).Distinct())
            {
                cboExamType.Items.Add(item);
            }
            cboExamType.SelectedIndex = 0;
            #endregion

            List<int> gradeList = _StudentRecord.Select(x => Convert.ToInt32(x.Class.GradeYear)).Distinct().OrderBy(x => x).ToList();//整理年級的清單

            #region 動態產生年級的CheckBox
            this.Height += 32 * (((gradeList.Count % 4) == 0 ? 0 : gradeList.Count / 4) + 1);//每多一排checkBox+32
            for (int i = 0; i < gradeList.Count; i++)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.AutoSize = true;
                checkBox.Location = new System.Drawing.Point(13 + (133 * (i % 4)), 8 + (32 * (i / 4))); //兩個checkBox的X差距133，兩個checkBox的Y差距32
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
            _FormWidth = this.Width;
            _FormHeight = this.Height;

            plStudentView.Visible = true;
            this.Width = 810;
            this.Height = 510;
            lbExam.Text = cboExamType.Text;
            lbSemester.Text = cboSemester.Text;
            lbSchoolYear.Text = cboSchoolYear.Text;

            #region 依據勾選的項目產生CheckBox
            int checkBoxCount = 0;
            foreach (CheckBox checkBox in gpRankPeople.Controls.OfType<CheckBox>())
            {
                if (checkBox.Checked == true)
                {
                    CheckBox newCheckBox = new CheckBox();
                    newCheckBox.Location = new System.Drawing.Point(65 + (97 * (checkBoxCount % 3)), 44 + (27 * (checkBoxCount / 3)));
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
            #endregion

            //每多一排CheckBox就把Form的長加23
            this.Height += 23 * ((checkBoxCount % 3) == 0 ? 0 : checkBoxCount / 3);

            _FilterStudentLIst = new List<StudentRecord>();
            string studentFilter = cboStudentFIlter.Text;
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
                    bkw.ReportProgress(1);
                    foreach (string gradeYear in _CheckBoxList.Select(x => x.Text))
                    {
                        _FilterStudentLIst.AddRange(_StudentRecord.Where(x => x.Class.GradeYear == Convert.ToInt32(gradeYear.Trim('年', '級'))).ToList());
                    }

                    bkw.ReportProgress(25);
                    if (!string.IsNullOrEmpty(studentFilter))
                    {
                        _FilterStudentLIst = _FilterStudentLIst.Where(x => x.EnrollmentCategory != studentFilter).ToList();
                    }

                    bkw.ReportProgress(50);
                    if (!string.IsNullOrEmpty(studentTag1) && !string.IsNullOrEmpty(studentTag2))
                    {
                        _FilterStudentLIst = _FilterStudentLIst.Where(x => x.EnrollmentCategory == studentTag1 || x.EnrollmentCategory == studentTag2).ToList();
                    }
                    else if (!string.IsNullOrEmpty(studentTag1))
                    {
                        _FilterStudentLIst = _FilterStudentLIst.Where(x => x.EnrollmentCategory == studentTag1).ToList();
                    }
                    else if (!string.IsNullOrEmpty(studentTag2))
                    {
                        _FilterStudentLIst = _FilterStudentLIst.Where(x => x.EnrollmentCategory == studentTag2).ToList();
                    }
                    bkw.ReportProgress(100);
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

                if (_FilterStudentLIst.Count == 0)
                {
                    btnCacluate.Enabled = false;
                    MessageBox.Show("沒有找到符合條件的學生");
                    btnPrevious.Enabled = true;
                    return;
                }

                var studentView = (from s in _FilterStudentLIst
                                   select new
                                   {
                                       ClassName = s.Class.Name,
                                       s.SeatNo,
                                       s.StudentNumber,
                                       s.Name,
                                       RankGradeYear = "" + s.Class.GradeYear + "年級",
                                       RankClassName = s.Class.Name,
                                       StudentTag1 = cboStudentTag1.Text,
                                       StudentTag2 = cboStudentTag2.Text,
                                   }).ToList();

                List<DataGridViewRow> rowList = new List<DataGridViewRow>();
                for (int rowIndex = 0; rowIndex < studentView.Count; rowIndex++)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(dgvStudentList);
                    row.Cells[0].Value = studentView[rowIndex].ClassName;
                    row.Cells[1].Value = studentView[rowIndex].SeatNo;
                    row.Cells[2].Value = studentView[rowIndex].StudentNumber;
                    row.Cells[3].Value = studentView[rowIndex].Name;
                    row.Cells[4].Value = studentView[rowIndex].RankGradeYear;
                    row.Cells[5].Value = studentView[rowIndex].RankClassName;
                    row.Cells[6].Value = studentView[rowIndex].StudentTag1;
                    row.Cells[7].Value = studentView[rowIndex].StudentTag2;
                    rowList.Add(row);
                }
                dgvStudentList.Rows.AddRange(rowList.ToArray());

                btnPrevious.Enabled = true;
            };

            bkw.RunWorkerAsync();
        }

        private void btnCacluate_Click(object sender, EventArgs e)
        {
            string schoolYear = lbSchoolYear.Text;
            string semester = lbSemester.Text;
            string examName = lbExam.Text;
            List<int> gradeYearList = new List<int>();
            foreach (CheckBox checkBox in _CheckBoxList)
            {
                gradeYearList.Add(Convert.ToInt32(checkBox.Text.Trim('年', '級')));
            }
            string studentIDs = string.Join(",", _FilterStudentLIst.Select(x => x.ID).ToList());
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
                        #region 新增成績的sql字串
                        string insertString = @"
WITH raw AS
(
	SELECT
		'" + gradeYearList[index] + @"'::TEXT  AS grade_year
		, '" + schoolYear + @"'::TEXT AS school_year
		, '" + semester + @"'::TEXT AS semester
		, '" + examName + @"'::TEXT AS exam_name
)
,weight AS
(
	SELECT
		template_id
		, name
		, ('0'||unnest(xpath('/Extension/ScorePercentage/text()',extension_xml))::text)::Decimal AS exam_weight
		,  100::Decimal- ('0'||unnest(xpath('/Extension/ScorePercentage/text()',extension_xml))::text)::Decimal AS assignment_weight
	FROM
	(
		SELECT  
			id AS template_id
			, name,unnest(xpath('/root/Extension',xmlparse(content'<root>'||extension||'</root>'))) AS extension_xml  
		FROM  exam_template
	) AS Extension
)
,score_detail AS
( 
	SELECT
		student.id AS student_id
   		,student.name AS student_name
		,sc_attend.id AS sc_attend_id
		,course.course_name
		,course.school_year
		,course.semester
		,course.domain
		,course.credit
		,weight.template_id
		,weight.name
		,weight.exam_weight
		,weight.assignment_weight
		,exam.exam_name
		,class.class_name
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
		LEFT JOIN student
			ON sc_attend.ref_student_id = student.id
		LEFT JOIN class
			ON student.ref_class_id = class.id
		LEFT JOIN weight
			ON  weight.template_id=course.ref_exam_template_id
		INNER JOIN raw
			ON course.school_year = raw.school_year::int
			AND course.semester = raw.semester::int
			AND class.grade_year = raw.grade_year::int
			AND exam.exam_name= raw.exam_name
	WHERE sc_attend.ref_student_id in 
	(
		" + studentIDs + @"
	)
)
,score_detail_avge AS
(
	SELECT	
		score_detail.*
		,CASE
			WHEN exam_score IS NOT NULL OR assignment_score IS NOT NULL
			THEN(
					(
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
						* assignment_weight
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
					)
				)
		ELSE NULL
		END
		AS score
	FROM 
		score_detail
	WHERE exam_score IS NOT NULL
	   OR assignment_score IS NOT NULL
)
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
		)/ 
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
,rank_raw AS
(
	SELECT
		group_score.student_id
		, group_score.domain
		, class.class_name
		, class.grade_year
		, group_score.score
		, RANK() OVER(PARTITION BY class.grade_year,group_score.domain ORDER BY group_score.score DESC) AS grade_rank
		, RANK() OVER(PARTITION BY class.class_name ,group_score.domain ORDER BY group_score.score DESC) AS class_rank
		, COUNT (group_score.student_id) OVER(PARTITION BY class.grade_year,group_score.domain ) AS grade_count
		, COUNT (group_score.student_id) OVER(PARTITION BY class.class_name, group_score.domain) AS class_count
	FROM 
		group_score
			LEFT OUTER JOIN student
				ON student.id = group_score.student_id
			LEFT OUTER JOIN class
				ON class.id = student.ref_class_id
)
, rank_expand AS 
(
	SELECT  
		rank_raw.*
		,FLOOR((grade_rank::DECIMAL-1)*100::DECIMAL / grade_count)+1 AS graderank_percentage
		,FLOOR((class_rank::DECIMAL-1)*100::DECIMAL / class_count)+1 AS classrank_percentage
	FROM 
		rank_raw
	WHERE  
		domain IS NOT NULL	
)
, insert_gradeyear_rank_matrix AS
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
			, rank_expand.grade_year AS grade_year
			, '定期評量/領域成績'::TEXT AS item_type
			, exam.id AS ref_exam_id
			, rank_expand.domain AS item_name
			, '年排名'::TEXT AS rank_type
			, '' || rank_expand.grade_year || '年級' ::TEXT AS rank_name
			, true AS is_alive
			, rank_expand.grade_count AS matrix_count
			, AVG(rank_expand.Score::Decimal)OVER(PARTITION BY rank_expand.grade_year,rank_expand.domain) AS avg
			, COUNT(*) FILTER (WHERE 100::DECIMAL<=rank_expand.score::DECIMAL ) OVER(PARTITION BY rank_expand.grade_year,rank_expand.domain)AS level_gte100 
			, COUNT(*) FILTER (WHERE 90::DECIMAL<=rank_expand.score AND rank_expand.score <100::DECIMAL)  OVER(PARTITION BY rank_expand.grade_year,rank_expand.domain) AS level_90
			, COUNT(*) FILTER (WHERE 80::DECIMAL<=rank_expand.score AND rank_expand.score <90::DECIMAL)  OVER(PARTITION BY rank_expand.grade_year,rank_expand.domain) AS level_80
			, COUNT(*) FILTER (WHERE 70::DECIMAL<=rank_expand.score AND  rank_expand.score <80::DECIMAL)  OVER(PARTITION BY rank_expand.grade_year,rank_expand.domain)AS level_70
			, COUNT(*) FILTER (WHERE 60::DECIMAL<=rank_expand.score AND rank_expand.score <70::DECIMAL) OVER(PARTITION BY rank_expand.grade_year,rank_expand.domain) AS level_60
			, COUNT(*) FILTER (WHERE 50::DECIMAL<=rank_expand.score AND rank_expand.score <60::DECIMAL)  OVER (PARTITION BY rank_expand.grade_year,rank_expand.domain)AS level_50
			, COUNT(*) FILTER (WHERE 40::DECIMAL<=rank_expand.score AND rank_expand.score <50::DECIMAL) OVER (PARTITION BY rank_expand.grade_year,rank_expand.domain)AS level_40
			, COUNT(*) FILTER (WHERE 30::DECIMAL<=rank_expand.score AND rank_expand.score <40::DECIMAL)   OVER (PARTITION BY rank_expand.grade_year,rank_expand.domain)AS level_30
			, COUNT(*) FILTER (WHERE 20::DECIMAL<=rank_expand.score AND rank_expand.score <30::DECIMAL)  OVER (PARTITION BY rank_expand.grade_year,rank_expand.domain) AS level_20
			, COUNT(*) FILTER (WHERE 10::DECIMAL<=rank_expand.score AND rank_expand.score <20::DECIMAL)   OVER (PARTITION BY rank_expand.grade_year,rank_expand.domain)AS level_10
			, COUNT(*) FILTER (WHERE rank_expand.score<10::DECIMAL) OVER (PARTITION BY rank_expand.grade_year,rank_expand.domain)AS level_lt10 
		FROM
			rank_expand
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
), insert_gradeyear_rank_detail AS(
	INSERT INTO rank_detail(
		ref_matrix_id
		, ref_student_id
		, score
		, rank
		, percentile
	)
	SELECT 
		insert_gradeyear_rank_matrix.id AS ref_matrix_id
		, rank_expand.student_id AS ref_student_id
		, rank_expand.score AS score
		, rank_expand.grade_rank AS rank
		, rank_expand.graderank_percentage AS percentile	
	FROM
		rank_expand
		LEFT OUTER JOIN insert_gradeyear_rank_matrix
			ON insert_gradeyear_rank_matrix.grade_year = rank_expand.grade_year
			AND insert_gradeyear_rank_matrix.item_name=rank_expand.domain
)
, insert_class_rank_matrix AS(
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
			, rank_expand.grade_year AS grade_year
			, '定期評量/領域成績'::TEXT AS item_type
			, exam.id AS ref_exam_id
			, rank_expand.domain AS item_name
			, '班排名'::TEXT AS rank_type
			, rank_expand.class_name AS rank_name
			, true AS is_alive
			, rank_expand.class_count AS matrix_count
			, AVG(rank_expand.Score::Decimal)OVER(PARTITION BY rank_expand.class_name,rank_expand.domain) AS avg
			, COUNT(*) FILTER (WHERE 100::DECIMAL<=rank_expand.score::DECIMAL ) OVER(PARTITION BY rank_expand.class_name,rank_expand.domain)AS level_gte100 
			, COUNT(*) FILTER (WHERE 90::DECIMAL<=rank_expand.score AND rank_expand.score <100::DECIMAL)OVER (PARTITION BY rank_expand.class_name,rank_expand.domain) AS level_90
			, COUNT(*) FILTER (WHERE 80::DECIMAL<=rank_expand.score AND rank_expand.score <90::DECIMAL) OVER (PARTITION BY rank_expand.class_name,rank_expand.domain) AS level_80
			, COUNT(*) FILTER (WHERE 70::DECIMAL<=rank_expand.score AND rank_expand.score <80::DECIMAL) OVER (PARTITION BY rank_expand.class_name,rank_expand.domain)AS level_70
			, COUNT(*) FILTER (WHERE 60::DECIMAL<=rank_expand.score AND rank_expand.score <70::DECIMAL) OVER (PARTITION BY rank_expand.class_name,rank_expand.domain) AS level_60
			, COUNT(*) FILTER (WHERE 50::DECIMAL<=rank_expand.score AND rank_expand.score <60::DECIMAL) OVER (PARTITION BY rank_expand.class_name,rank_expand.domain)AS level_50
			, COUNT(*) FILTER (WHERE 40::DECIMAL<=rank_expand.score AND rank_expand.score <50::DECIMAL) OVER (PARTITION BY rank_expand.class_name,rank_expand.domain)AS level_40
			, COUNT(*) FILTER (WHERE 30::DECIMAL<=rank_expand.score AND rank_expand.score <40::DECIMAL) OVER (PARTITION BY rank_expand.class_name,rank_expand.domain)AS level_30
			, COUNT(*) FILTER (WHERE 20::DECIMAL<=rank_expand.score AND rank_expand.score <30::DECIMAL) OVER (PARTITION BY rank_expand.class_name,rank_expand.domain) AS level_20
			, COUNT(*) FILTER (WHERE 10::DECIMAL<=rank_expand.score AND rank_expand.score <20::DECIMAL) OVER (PARTITION BY rank_expand.class_name,rank_expand.domain)AS level_10
			, COUNT(*) FILTER (WHERE rank_expand.score<10::DECIMAL) OVER (PARTITION BY rank_expand.class_name,rank_expand.domain)AS level_lt10 
		FROM
			rank_expand
			CROSS JOIN raw
			LEFT OUTER JOIN exam
			ON exam.exam_name = raw.exam_name) AS sub
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
, insert_class_rank_detail AS
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
		insert_class_rank_matrix.id AS ref_matrix_id
		, rank_expand.student_id AS ref_student_id
		, rank_expand.score AS score
		, rank_expand.class_rank AS rank
		, rank_expand.classrank_percentage AS percentile	
	FROM
		rank_expand
		LEFT OUTER JOIN insert_class_rank_matrix
			ON insert_class_rank_matrix.grade_year = rank_expand.grade_year
			AND insert_class_rank_matrix.item_name=rank_expand.domain
			AND insert_class_rank_matrix.rank_name=rank_expand.class_name
)
SELECT * FROM insert_gradeyear_rank_matrix";
                        #endregion

                        QueryHelper queryHelper = new QueryHelper();
                        queryHelper.Select(insertString);
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
            };

            bkw.RunWorkerAsync();
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
