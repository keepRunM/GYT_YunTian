﻿using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Zer.Entities;
using Zer.GytDataService;
using Zer.GytDto;
using Zer.GytDto.Extensions;

namespace Zer.AppServices.Impl
{
    public class LngAllowanceService : ILngAllowanceService
    {
        private readonly ILngAllowanceInfoDataService _lngAllowanceInfoDataService;

        public LngAllowanceService(ILngAllowanceInfoDataService lngAllowanceInfoDataService)
        {
            _lngAllowanceInfoDataService = lngAllowanceInfoDataService;
        }

        public LngAllowanceInfoDto GetById(int id)
        {
            return Mapper.Map<LngAllowanceInfoDto>(_lngAllowanceInfoDataService.GetById(id));
        }

        public List<LngAllowanceInfoDto> GetAll()
        {
            return _lngAllowanceInfoDataService.GetAll().Map<LngAllowanceInfoDto>().ToList();
        }

        public LngAllowanceInfoDto Add(LngAllowanceInfoDto model)
        {
            return _lngAllowanceInfoDataService.Insert(model.Map<LngAllowanceInfo>()).Map<LngAllowanceInfoDto>();
        }

        public List<LngAllowanceInfoDto> AddRange(List<LngAllowanceInfoDto> list)
        {
            return _lngAllowanceInfoDataService.AddRange(list.Map<LngAllowanceInfo>()).Map<LngAllowanceInfoDto>().ToList();
        }

        public List<LngAllowanceInfoDto> GetList(FilterMatchInputDto filterMatchInput)
        {
            throw new System.NotImplementedException();
        }

        public List<LngAllowanceInfoDto> GetList(int[] idList)
        {
            throw new System.NotImplementedException();
        }

        public bool Exists(LngAllowanceInfoDto lngAllowanceInfoDto)
        {
            return _lngAllowanceInfoDataService.GetAll()
                .Any(x => x.TruckNo == lngAllowanceInfoDto.TruckNo ||
                          x.CylinderDefaultId == lngAllowanceInfoDto.CylinderDefaultId);
        }
    }
}