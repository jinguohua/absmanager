using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ABS.Core.DTO;
using ABS.Core.Models;
using ABS.Infrastructure.Models;
using AutoMapper;
using SAFS.Core.Permissions.Identity.Models;

namespace ABS.ABSManagementSite.AutoMapper
{
    public class AutoMapperConfig
    {
        //volatile可以防止编译器对代码进行优化，确保每一次都进行读取
        private static volatile bool _mappedBefore;
        private static readonly object _lock = new object();

        public static void CreateMappings()
        {
            lock (_lock)
            {
                if (_mappedBefore)
                {
                    return;
                }

                CreateMappingInsternal();

                _mappedBefore = true;
            }
        }
        private static void CreateMappingInsternal()
        {
            Mapper.Initialize(o =>
            {
                #region  Menu<=> MenuViewModel
                o.CreateMap<Menu, MenuViewModel>();
                o.CreateMap<MenuViewModel, Menu>();
                #endregion

                #region  Company <=> CompanyViewModel
                o.CreateMap<Company, CompanyViewModel>()
                .ForMember(d => d.Category, opt => opt.MapFrom(s => s.Category.Split(',').ToArray()));

                o.CreateMap<CompanyViewModel, Company>()
                .ForMember(d => d.Category, opt => opt.MapFrom(s => string.Join(",", s.Category)));

                #endregion

                #region  CodeCategoryViewModel=>CodeCategory
                o.CreateMap<CodeCategoryViewModel, CodeCategory>();
                #endregion

                #region  OrganizationViewModel=>Organization
                o.CreateMap<OrganizationViewModel, Organization>();

                #endregion

                #region  ProjectViewModel <=> Project
                o.CreateMap<ProjectViewModel, Project>()
                .ForMember(d => d.Creator, dto => dto.Ignore())
                 .ForMember(d => d.CreatedTime, dto => dto.Ignore())
                 .ForMember(d => d.Timestamp, dto => dto.Ignore());

                o.CreateMap<Project, ProjectViewModel>();
                #endregion  OrganizationViewModel=>Organization

                #region ProjectCompanyViewModel <=> ProjectCompany
                o.CreateMap<ProjectCompanyViewModel, ProjectCompany>()
                .ForMember(d => d.Creator, dto => dto.Ignore())  // 忽略该字段的映射 
                .ForMember(d => d.CreatedTime, dto => dto.Ignore())
                .ForMember(d => d.Timestamp, dto => dto.Ignore());

                o.CreateMap<ProjectCompany, ProjectCompanyViewModel>();

                #endregion

                #region ProjectNoteViewModel <=> ProjectNote
                o.CreateMap<ProjectNoteViewModel, ProjectNote>()
                 .ForMember(d => d.Creator, dto => dto.Ignore())
                 .ForMember(d => d.CreatedTime, dto => dto.Ignore())
                 .ForMember(d => d.Timestamp, dto => dto.Ignore());

                o.CreateMap<ProjectNote, ProjectNoteViewModel>();
                #endregion

                #region ProjectNoteViewModel <=> ProjectNote
                o.CreateMap<ProjectNoteViewModel, ProjectNote>();
                o.CreateMap<ProjectNote, ProjectNoteViewModel>();
                #endregion

                #region SpecificDateRuleViewModel <=> SpecificDateRule
                o.CreateMap<SpecificDateRuleViewModel, SpecificDateRule>();
                o.CreateMap<SpecificDateRule, SpecificDateRuleViewModel>();
                #endregion

                #region  DateOverrideViewModel => DateOverride
                o.CreateMap<DateOverrideViewModel, DateOverride>()
                 .ForMember(d => d.Creator, dto => dto.Ignore())
                 .ForMember(d => d.CreatedTime, dto => dto.Ignore())
                 .ForMember(d => d.Timestamp, dto => dto.Ignore());
                #endregion

                #region DateRuleViewModel => DateRule
                o.CreateMap<DateRuleViewModel, DateRule>()
                 .ForMember(d => d.Timestamp, dto => dto.Ignore());
                #endregion

                #region NoteRatingViewModel => NoteRating
                o.CreateMap<NoteRatingViewModel, NoteRating>()
                 .ForMember(d => d.Timestamp, dto => dto.Ignore());
                o.CreateMap<NoteRating, NoteRatingViewModel>();
                #endregion

                o.CreateMap<AccountViewModel, Account>()
                 .ForMember(d => d.Timestamp, dto => dto.Ignore());
                o.CreateMap<Account, AccountViewModel>();


            });
        }
    }
}