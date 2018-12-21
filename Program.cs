    using FISCA;
using FISCA.Permission;
using FISCA.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JHEvaluation.Rank
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MainMethod()]
        public static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new RegularRankSelect());            
            RibbonBarItem regularRank = FISCA.Presentation.MotherForm.RibbonBarItems["教務作業", "批次作業/檢視"];
            //batchEdit["銷過"].Image = Properties.Resources.draw_pen_ok_64;
            regularRank["成績排名"]["排名資料檢索"].Enable = true;
            regularRank["成績排名"]["排名資料檢索"].Click += delegate
            {
                RegularRankSelect rankSelect = new RegularRankSelect();
                rankSelect.ShowDialog();
            };

            RibbonBarItem regularSchoolYearRank = MotherForm.RibbonBarItems["教務作業", "批次作業/檢視"];
            regularSchoolYearRank["成績排名"]["計算定期評量排名"].Enable = true;
            regularSchoolYearRank["成績排名"]["計算定期評量排名"].Click += delegate
            {
                CacluateRegularAssessmentRank cacluateRegularAssessmentRank = new CacluateRegularAssessmentRank();
                cacluateRegularAssessmentRank.ShowDialog();
            };
        }
    }
}
