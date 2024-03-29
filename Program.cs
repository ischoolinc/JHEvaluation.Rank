﻿using FISCA;
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
            MotherForm.RibbonBarItems["教務作業", "批次作業/檢視"]["成績排名"].Size = RibbonBarButton.MenuButtonSize.Large;
            MotherForm.RibbonBarItems["教務作業", "批次作業/檢視"]["成績排名"].Image = Properties.Resources.icon;
            {
                var key = "529ABB39-A819-4E50-8BC9-9302B2E89D06";
                RoleAclSource.Instance["教務作業"]["功能按鈕"].Add(new RibbonFeature(key, "定期評量排名計算"));
                MotherForm.RibbonBarItems["教務作業", "批次作業/檢視"]["成績排名"]["定期評量排名計算"].Enable = FISCA.Permission.UserAcl.Current[key].Executable;
                MotherForm.RibbonBarItems["教務作業", "批次作業/檢視"]["成績排名"]["定期評量排名計算"].Click += delegate
                {
                    CalculateRegularAssessmentRank cacluateRegularAssessmentRank = new CalculateRegularAssessmentRank();
                    cacluateRegularAssessmentRank.ShowDialog();
                };
            }
            {
                var key = "CC081AC9-49EB-4D5E-B37B-FC345B14EED4";
                RoleAclSource.Instance["教務作業"]["功能按鈕"].Add(new RibbonFeature(key, "定期評量排名資料檢索"));
                FISCA.Presentation.MotherForm.RibbonBarItems["教務作業", "批次作業/檢視"]["成績排名"]["定期評量排名資料檢索"].Enable = FISCA.Permission.UserAcl.Current[key].Executable;
                FISCA.Presentation.MotherForm.RibbonBarItems["教務作業", "批次作業/檢視"]["成績排名"]["定期評量排名資料檢索"].Click += delegate
                {
                    RegularAssessmentRankSelect rankSelect = new RegularAssessmentRankSelect();
                    rankSelect.ShowDialog();
                };
            }
            {
                var key = "9E64FFB0-370A-4027-9E18-55B7E717C474";
                RoleAclSource.Instance["教務作業"]["功能按鈕"].Add(new RibbonFeature(key, "學期成績排名計算"));
                MotherForm.RibbonBarItems["教務作業", "批次作業/檢視"]["成績排名"]["學期成績排名計算"].Enable = FISCA.Permission.UserAcl.Current[key].Executable;
                MotherForm.RibbonBarItems["教務作業", "批次作業/檢視"]["成績排名"]["學期成績排名計算"].Click += delegate
                {
                    CalculateSemesterAssessmentRank calculateSemesterAssessmentRank = new CalculateSemesterAssessmentRank();
                    calculateSemesterAssessmentRank.ShowDialog();
                };
            }
            {
                var key = "BCABCDF8-37F3-443E-9B53-0810033CC1E0";
                RoleAclSource.Instance["教務作業"]["功能按鈕"].Add(new RibbonFeature(key, "學期成績排名資料檢索"));
                FISCA.Presentation.MotherForm.RibbonBarItems["教務作業", "批次作業/檢視"]["成績排名"]["學期成績排名資料檢索"].Enable = FISCA.Permission.UserAcl.Current[key].Executable;
                FISCA.Presentation.MotherForm.RibbonBarItems["教務作業", "批次作業/檢視"]["成績排名"]["學期成績排名資料檢索"].Click += delegate
                {
                    SemesterAssessmentRankSelect semesterAssessmentRankSelect = new SemesterAssessmentRankSelect();
                    semesterAssessmentRankSelect.ShowDialog();
                };
            }
        }
    }
}
