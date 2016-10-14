﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prometheus.Models
{
    public class ProjectFAViewModules
    {
        public ProjectFAViewModules()
        { }
        public ProjectFAViewModules(IssueViewModels im, ProjectTestData pd)
        {
            IssueData = im;
            TestData = pd;
        }

        public IssueViewModels IssueData { set; get; }
        public ProjectTestData TestData { set; get; }

        public static List<ProjectFAViewModules> RetrievePendingFAData(string pjkey)
        {
            var ret = new List<ProjectFAViewModules>();
            var issuedict = IssueViewModels.RRetrieveFAByPjkey(pjkey, Resolute.Working,50);
            foreach (var d in issuedict)
            {
                if (d.Summary.Contains("@Burn-In Step"))
                {
                    var pd = new ProjectTestData();
                    var splitinfo = d.Summary.Split(new string[] { " " }, StringSplitOptions.None);
                    if (splitinfo.Length > 4)
                    {
                        pd.ModuleSerialNum = splitinfo[1];
                        pd.ErrAbbr = splitinfo[4];
                        pd.ProjectKey = d.ProjectKey;
                        ret.Add(new ProjectFAViewModules(d, pd));
                    }
                }
                else
                {
                    var pjdata = ProjectTestData.RetrieveProjectTestData(d.IssueKey);
                    if (pjdata.Count > 0)
                    {
                        ret.Add(new ProjectFAViewModules(d, pjdata[0]));
                    }
                }
            }

            issuedict = IssueViewModels.RRetrieveFAByPjkey(pjkey, Resolute.Pending,500);
            foreach (var d in issuedict)
            {
                if (d.Summary.Contains("@Burn-In Step"))
                {
                    var pd = new ProjectTestData();
                    var splitinfo = d.Summary.Split(new string[] { " " }, StringSplitOptions.None);
                    if (splitinfo.Length > 4) {
                        pd.ModuleSerialNum = splitinfo[1];
                        pd.ErrAbbr = splitinfo[4];
                        pd.ProjectKey = d.ProjectKey;
                        ret.Add(new ProjectFAViewModules(d, pd));
                    }

                }
                else
                {
                    var pjdata = ProjectTestData.RetrieveProjectTestData(d.IssueKey);
                    if (pjdata.Count > 0)
                    {
                        ret.Add(new ProjectFAViewModules(d, pjdata[0]));
                    }
                }
            }

            return ret;
        }

        public static List<ProjectFAViewModules> RetrieveDoneFAData(string pjkey)
        {
            var ret = new List<ProjectFAViewModules>();
            var pjdata = ProjectTestData.RetrieveProjectFailedTestData(100000, pjkey);

            var issuedict = IssueViewModels.RRetrieveFADictByPjkey(pjkey, Resolute.Done,100000);
            foreach (var d in pjdata)
            {
                if (issuedict.ContainsKey(d.DataID))
                {
                    ret.Add(new ProjectFAViewModules(issuedict[d.DataID], d));
                }
            }

            return ret;
        }

        public static int RetrieveFADataCount(string pjkey,bool pending=true)
        {
            if (pending)
            {
                return IssueViewModels.RRetrieveFAStatusByPjkey(pjkey, Resolute.Working)
                                + IssueViewModels.RRetrieveFAStatusByPjkey(pjkey, Resolute.Pending);
            }
            else
            {
                return IssueViewModels.RRetrieveFAStatusByPjkey(pjkey, Resolute.Done);
            }
        }

        public static List<ProjectFAViewModules> RetrieveFADataWithErrAbbr(string ProjectKey, string ErrAbbr)
        {
            var ret = new List<ProjectFAViewModules>();
            var pjdata = ProjectTestData.RetrieveProjectTestDataWithErrAbbr(100000, ProjectKey, ErrAbbr);
            foreach (var d in pjdata)
            {
                var im = IssueViewModels.RetrieveIssueByIssueKey(d.DataID);
                if (im != null)
                {
                    ret.Add(new ProjectFAViewModules(im, d));
                }
            }
            return ret;
        }

    }
}