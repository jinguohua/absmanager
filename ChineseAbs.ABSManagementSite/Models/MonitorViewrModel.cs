using ChineseAbs.ABSManagement.Models;
using ChineseAbs.ABSManagementSite.Common;
using ChineseAbs.Logic.Object;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChineseAbs.ABSManagementSite.Models
{
    public class MonitorViewModel
    {
        public MonitorViewModel()
        {
            NewsDetail = new PagedNews();
            Agencys = new Agencies();
            Projectinfo = new ProjectInfo();
        }
        public PagedNews NewsDetail;
        public Agencies Agencys;
        public ProjectInfo Projectinfo;
        public ProjectList Projects;
    }

    public class NewsDetail
    {
        public int Count { get; set; }
        public List<News> News { get; set; }
    }

    public class Agencies
    {
        [Display(Name = "机构列表")]
        public Contacts agencies { get; set; }
    }

    public class ProjectInfo
    {
        [Display(Name ="工程guid")]
        public string guid { get; set; }

        [Display(Name = "产品全称")]
        public string fullName { get; set; }

        [Display(Name = "起息日")]
        public DateTime? closingDate { get; set; }

        [Display(Name = "法定到期日")]
        public DateTime? legalMaturityDate { get; set; }

        [Display(Name = "首次偿付日")]
        public DateTime? firstPaymentDate { get; set; }

        [Display(Name = "支付频率")]
        public string paymentFrequency { get; set; }

        [Display(Name = "监管机构")]
        public string regulator { get; set; }

        [Display(Name = "产品类型")]
        public string productType { get; set; }
    }

    public class ProjectList
    {
        public List<Project> projects;
    }
    public static class MonitorConvertion
    {
        public static ProjectList ConvertProjectList(List<Project> ps)
        {
            ProjectList pl = new ProjectList();
            pl.projects = ps;
            return pl;
        }
        public static ProjectInfo ConvertProjectInfo(DealDataV2 data, string guid)
        {
            ProjectInfo pi = new ProjectInfo();
            pi.fullName = data.DealNameChinese;
            pi.closingDate = data.ClosingDate;
            pi.legalMaturityDate = data.LegalMaturity;
            pi.firstPaymentDate = data.FirstPaymentDate;
            pi.paymentFrequency = Toolkit.PaymentFrequency(data.Frequency);
            pi.regulator = data.Regulator;
            pi.productType = data.ProductType;
            pi.guid = guid;
            return pi;
        }

        public static Agencies ConvertAgencies(Contacts contacts)
        {
            Agencies ags = new Agencies();
            ags.agencies = contacts;
            return ags;
        }
    }
}