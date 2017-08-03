﻿using System.Collections.Generic;
using Zer.Entities;

namespace Zer.Services
{
    public interface IDomainService<TEntityDto,in TPrimaryKey>
    {
        TEntityDto GetById(TPrimaryKey id);
        List<TEntityDto> GetAll();

        bool Add(TEntityDto model);

        bool AddRange(List<TEntityDto> list);

    }

}