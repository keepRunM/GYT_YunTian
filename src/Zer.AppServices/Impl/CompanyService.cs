﻿using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Zer.Entities;
using Zer.Framework.Dto;
using Zer.Framework.Exception;
using Zer.Framework.Extensions;
using Zer.GytDataService;
using Zer.GytDto;
using Zer.GytDto.Extensions;
using Zer.GytDto.SearchFilters;

namespace Zer.AppServices.Impl
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyInfoDataService _companyInfoDataService;


        public CompanyService(
            ICompanyInfoDataService companyInfoDataService)
        {
            _companyInfoDataService = companyInfoDataService;
        }

        public CompanyInfoDto GetById(int id)
        {
            return Mapper.Map<CompanyInfoDto>(_companyInfoDataService.GetById(id));
        }

        public List<CompanyInfoDto> GetAll()
        {
            return _companyInfoDataService.GetAllList().Map<CompanyInfoDto>().ToList();
        }

        public CompanyInfoDto Add(CompanyInfoDto model)
        {
            if (Exists(model.CompanyName))
            {
                throw new CustomException(
                    "公司信息已经存在",
                    new Dictionary<string, string>()
                    {
                        {"CompanyName", model.CompanyName}
                    });
            }

            var companyInfo = Mapper.Map<CompanyInfo>(model);
            return _companyInfoDataService.Insert(companyInfo).Map<CompanyInfoDto>();
        }

        /// <summary>
        /// 进行批量新增时，请检查是否在数据库中已经存在数据
        /// </summary>
        public List<CompanyInfoDto> AddRange(List<CompanyInfoDto> list)
        {
            var entities = list.Map<CompanyInfo>();
            return _companyInfoDataService.AddRange(entities).Map<CompanyInfoDto>().ToList();
        }

        public CompanyInfoDto Edit(CompanyInfoDto model)
        {
            throw new System.NotImplementedException();
        }

        public List<CompanyInfoDto> GetByLikeName(string likeName)
        {
            return _companyInfoDataService
                .GetAll()
                .Where(x => x.CompanyName.Contains(likeName))
                .Map<CompanyInfoDto>().ToList();
        }

        public bool Exists(string companyFullName)
        {
            return _companyInfoDataService.GetAll().Any(x => x.CompanyName == companyFullName.Trim());
        }

        public void Add(CompanyInfo model)
        {
            _companyInfoDataService.Insert(model);
        }


        /// <summary>
        /// 检查公司是否存在，如果不存在新增，在完成所有新增操作后，查询并返回参数指定公司信息
        /// </summary>
        /// <param name="companyInfoDtos"></param>
        /// <returns><see cref="List{CompanyInfoDto}"/></returns>
        public List<CompanyInfoDto> QueryAfterValidateAndRegist(List<CompanyInfoDto> companyInfoDtos)
        {
            var companyNameList = companyInfoDtos.Where(x=>!x.CompanyName.IsNullOrEmpty()).Select(x => x.CompanyName.Trim()).Distinct().ToList();

            var notExistsCompanyName = companyNameList.Where(x => !Exists(x));
            var newRegistCompanyInfoDtoList = notExistsCompanyName.Select(name =>
                new CompanyInfoDto
                {
                    CompanyName = name,
                    TraderRange = companyInfoDtos.Where(x => x.CompanyName == name).Select(x => x.TraderRange)
                        .FirstOrDefault()
                }).ToList();

            // 新增所有不存在的公司
            AddRange(newRegistCompanyInfoDtoList.ToList());

            var companyInfoDtoList = GetAll().Where(x => companyNameList.Contains(x.CompanyName)).ToList();
            return companyInfoDtoList;
        }

        public CompanyInfoDto QueryAfterValidateAndRegist(string companyName)
        {
            var companyInfo = _companyInfoDataService.FirstOrDefault(x => x.CompanyName == companyName);
            if (companyInfo == null)
            {
                companyInfo = _companyInfoDataService.Insert(new CompanyInfo(){CompanyName = companyName,TraderRange = string.Empty});
            }
            return companyInfo.Map<CompanyInfoDto>();
        }

        public List<CompanyInfoDto> GetWithPeccancyRecored(CompanySearchDto filter)
        {
            var query = _companyInfoDataService.GetAll();
            query = Filter(filter, query);
            query = query.ToPageQuery(filter);
            return query.Map<CompanyInfoDto>().ToList();
        }

        private IQueryable<CompanyInfo> Filter(CompanySearchDto searchDto, IQueryable<CompanyInfo> query)
        {
            if (!searchDto.CompanyIdList.IsNullOrEmpty())
            {
                query = query.Where(x => searchDto.CompanyIdList.Contains(x.Id));
            }
            return query;
        }


    }
}
