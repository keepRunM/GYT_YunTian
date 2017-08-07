﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Zer.Entities;
using Zer.Framework.Entities;
using Zer.Framework.Exception;
using Zer.GytDataService;
using Zer.GytDto.Extensions;
using Zer.GytDto.Users;

namespace Zer.AppServices.Impl
{
    public class UserInfoService : IUserInfoService
    {
        private readonly IUserInfoDataService _userIbfoDataService;

        public UserInfoService(IUserInfoDataService userIbfoDataService)
        {
            _userIbfoDataService = userIbfoDataService;
        }

        public UserInfoDto GetByUserName(string userName)
        {
            throw new NotImplementedException();
        }

        public LoginStatus VerifyUserNameAndPassword(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public RegistResult Regist(UserInfoDto userInfo)
        {
            if (Exists(userInfo.UserName))
            {
                return RegistResult.UserNameExists;
            }

            var entity = Mapper.Map<UserInfo>(userInfo);

            if (_userIbfoDataService.Insert(entity) == null)
            {
                throw new CustomException("注册失败",new Dictionary<string,string>{{"UserName",userInfo.UserName}});
            }

            return RegistResult.Success;
        }

        public FrozenResult LetUserFrozen(int userId)
        {
            var entity = _userIbfoDataService.GetById(userId);

            entity.State = State.SoftDeleted;

            return _userIbfoDataService.Update(entity)!=null ? FrozenResult.Success : FrozenResult.UserIsFrzon;
        }

        public ThawResult LetUserThaw(int userId)
        {
            var entity = _userIbfoDataService.GetById(userId);

            entity.State = State.Active;

            return _userIbfoDataService.Update(entity) != null ? ThawResult.Success : ThawResult.UserIsThaw;
        }

        public ChangePasswordResult ChangePassword(int userId, string newPassword)
        {
            var entity = _userIbfoDataService.GetById(userId);

            entity.Password = newPassword;

            return _userIbfoDataService.Update(entity) != null ? ChangePasswordResult.Success : ChangePasswordResult.SameAsOldPassword;
        }

        public bool Exists(string  userName)
        {
            return _userIbfoDataService.GetAll().Any(x => x.UserName == userName);
        }

        public bool Edit(UserInfoDto userInfoDto)
        {
            var entity = _userIbfoDataService.GetById(userInfoDto.UserId);

            entity.UserName = userInfoDto.UserName;
            entity.DisplayName = userInfoDto.DisplayName;
            entity.Email = userInfoDto.Email;
            entity.MobilePhone = userInfoDto.MobilePhone;

            return _userIbfoDataService.Update(entity) != null;
        }

        public UserInfoDto GetById(int id)
        {
            return _userIbfoDataService.GetById(id).Map<UserInfoDto>();
        }

        public List<UserInfoDto> GetAll()
        {
            return Mapper.Map<List<UserInfoDto>>(_userIbfoDataService.GetAll());
        }

        public UserInfoDto Add(UserInfoDto model)
        {
            throw new NotImplementedException();
        }

        public List<UserInfoDto> AddRange(List<UserInfoDto> list)
        {
            throw new NotImplementedException();
        }
    }
}