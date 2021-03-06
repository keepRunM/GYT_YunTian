﻿using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Zer.GytDataService;
using Zer.GytDto;

namespace Zer.AppServices.Impl
{
    public class LogsService : ILogsService
    {
        private readonly ILogDataService _logDataService;

        public LogsService(ILogDataService logDataService)
        {
            _logDataService = logDataService;
        }

        public LogsDto GetById(int id)
        {
            throw new System.NotImplementedException();
        }

        public List<LogsDto> GetAll()
        {
            return Mapper.Map<List<LogsDto>>(_logDataService.GetAll());
        }

        public bool Add(LogsDto model)
        {
            throw new System.NotImplementedException();
        }

        public bool AddRange(List<LogsDto> list)
        {
            throw new System.NotImplementedException();
        }

        public List<LogsDto> GetListByFilterMatch(FilterMatchInputDto filterMatch)
        {
            throw new System.NotImplementedException();
        }

        public List<LogsDto> GetListByIds(int[] ids)
        {
            return Mapper.Map<List<LogsDto>>(_logDataService.GetAll().Where(x => ids.Contains(x.Id)).ToList());
        }
    }
}