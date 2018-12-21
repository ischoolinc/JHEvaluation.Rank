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

            List<StudentRecord> studentList = new List<StudentRecord>();
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
                        studentList.AddRange(_StudentRecord.Where(x => x.Class.GradeYear == Convert.ToInt32(gradeYear.Trim('年', '級'))).ToList());
                    }

                    bkw.ReportProgress(25);
                    if (!string.IsNullOrEmpty(studentFilter))
                    {
                        studentList = studentList.Where(x => x.EnrollmentCategory != studentFilter).ToList();
                    }

                    bkw.ReportProgress(50);
                    if (!string.IsNullOrEmpty(studentTag1) && !string.IsNullOrEmpty(studentTag2))
                    {
                        studentList = studentList.Where(x => x.EnrollmentCategory == studentTag1 || x.EnrollmentCategory == studentTag2).ToList();
                    }
                    else if (!string.IsNullOrEmpty(studentTag1))
                    {
                        studentList = studentList.Where(x => x.EnrollmentCategory == studentTag1).ToList();
                    }
                    else if (!string.IsNullOrEmpty(studentTag2))
                    {
                        studentList = studentList.Where(x => x.EnrollmentCategory == studentTag2).ToList();
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

                if (studentList.Count == 0)
                {
                    btnCacluate.Enabled = false;
                    MessageBox.Show("沒有找到符合條件的學生");
                    btnPrevious.Enabled = true;
                    return;
                }

                var studentView = (from s in studentList
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

                dgvStudentList.DataSource = studentView.ToList();
                btnPrevious.Enabled = true;
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
        }
    }
}
