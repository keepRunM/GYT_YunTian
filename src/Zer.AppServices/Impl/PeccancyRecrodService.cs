﻿using System.Collections.Generic;
using System.Linq;
using Zer.Entities;
using Zer.Framework.Dto;
using Zer.Framework.Extensions;
using Zer.GytDataService;
using Zer.GytDto;
using Zer.GytDto.Extensions;
using Zer.GytDto.SearchFilters;

namespace Zer.AppServices.Impl
{
    public class PeccancyRecrodService : IPeccancyRecrodService
    {
        private readonly IPeccancyRecrodDataService _peccancyRecrodDataService;
        private readonly ICompanyService _companyService;

        public PeccancyRecrodService(IPeccancyRecrodDataService peccancyRecrodDataService, ICompanyService companyService)
        {
            _peccancyRecrodDataService = peccancyRecrodDataService;
            _companyService = companyService;
        }

        public PeccancyRecrodDto GetById(int id)
        {
            throw new System.NotImplementedException();
        }

        public List<PeccancyRecrodDto> GetAll()
        {
            return _peccancyRecrodDataService.GetAll().Map<PeccancyRecrodDto>().ToList();
        }

        public PeccancyRecrodDto Add(PeccancyRecrodDto model)
        {
            throw new System.NotImplementedException();
        }

        public List<PeccancyRecrodDto> AddRange(List<PeccancyRecrodDto> list)
        {
            var existsList = list.Where(Exists).ToList();
            var noexistsList = list.Where(x => !Exists(x)).ToList();
            var insertList= _peccancyRecrodDataService.AddRange(noexistsList.Map<PeccancyInfo>()).Map<PeccancyRecrodDto>().ToList();

            insertList.AddRange(existsList);
            return insertList;
        }

        public List<PeccancyRecrodDto> GetListByFilterMatch(FilterMatchInputDto filterMatch)
        {
            throw new System.NotImplementedException();
        }

        public bool ChangeStatusById(string id)
        {
            var overLoadInfoDto = _peccancyRecrodDataService.Single(x=>x.PeccancyId == id);
            overLoadInfoDto.Status = Status.已整改;
            return _peccancyRecrodDataService.Update(overLoadInfoDto.Map<PeccancyInfo>()).Map<PeccancyRecrodDto>().Status == Status.已整改;
        }

        public List<PeccancyRecrodDto> GetListByIds(int[] ids)
        {
            throw new System.NotImplementedException();
        }

        public bool Exists(PeccancyRecrodDto overloadRecrodDto)
        {
            return _peccancyRecrodDataService.GetAll()
                .Any(x => x.PeccancyId == overloadRecrodDto.PeccancyId);
        }

        public List<PeccancyRecrodDto> GetList(PeccancySearchDto searchDto)
        {
            var query = _peccancyRecrodDataService.GetAll();

            if (searchDto == null) return query.Map<PeccancyRecrodDto>().ToList();

            query = Filter(searchDto, query);

            query = query.ToPageQuery(searchDto);

            return query.Map<PeccancyRecrodDto>().ToList();
        }

        private IQueryable<PeccancyInfo> Filter(PeccancySearchDto searchDto, IQueryable<PeccancyInfo> query)
        {
            if (!searchDto.CompanyName.IsNullOrEmpty())
            {
                query = query.Where(x => x.CompanyName.Contains(searchDto.CompanyName));
            }

            if (!searchDto.TruckNo.IsNullOrEmpty())
            {
                query = query.Where(x => x.BehindTruckNo == searchDto.TruckNo || x.FrontTruckNo == searchDto.TruckNo);
            }

            if (searchDto.Status.HasValue)
            {
                query = query.Where(x => x.Status == searchDto.Status);
            }

            if (searchDto.StartDate.HasValue)
            {
                query = query.Where(x => x.PeccancyDate >= searchDto.StartDate);
            }

            if (searchDto.EndDate.HasValue)
            {
                query = query.Where(x => x.PeccancyDate <= searchDto.EndDate);
            }

            return query;
        }

        public bool ExistsCompanyName(string companyName)
        {
            return _peccancyRecrodDataService.GetAll()
                .Any(x => x.CompanyName == companyName && x.Status == Status.未整改);
        }
    }
}