﻿
using System;
using System.Web.Helpers;
using System.Web.Mvc;
using Zer.Services.Users;
using Zer.Services.Users.Dto;
using Zer.Framework.Extensions;


namespace com.gyt.ms.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserInfoService _userInfoService;
        public UserController(IUserInfoService userInfoService)
        {
            _userInfoService = userInfoService;
        }

        public JsonResult Regist(string userName, string password)
        {
            if (userName.IsNullOrEmpty() ||
                password.IsNullOrEmpty())
            {
                throw new ArgumentException("用户名或密码不能为空！");
            }

            ValidataInputString(userName, password);

            if (
                userName.Length <= 6 ||
                password.Length < 6)
            {
                throw new ArgumentException("用户名或密码的长度不能小于6！");
            }

            var registResult = _userInfoService.Regist(userName, password);

            if (registResult == RegistResult.UserNameExists)
            {
                return Fail();
            }

            return Success();
        }

        public ActionResult Login(string userName, string password)
        {
            ValidataInputString(userName, password);

            var loginResult = _userInfoService.VerifyUserNameAndPassword(userName, password);

            if (loginResult != LoginStatus.Success)
            {
                return View("Error");
            }
            var userinfoDto = _userInfoService.GetByUserName(userName);

            Session["UserInfo"] = userinfoDto;
            return View("Success");
        }

        public JsonResult Logout()
        {
            // TODO: unit test coding
            Session["UserInfo"] = null;
            return Success();
        }

        public JsonResult Frozon(int userId)
        {
            var frozonResult = _userInfoService.LetUserFrozen(userId);

            return frozonResult == FrozenResult.Success ? Success() : Fail();
        }

        public JsonResult Thaw(int userId)
        {
            var frozonResult = _userInfoService.LetUserThaw(userId);

            if (frozonResult == ThawResult.Success)
            {
                return Success();
            }

            return Fail();
        }

        public JsonResult ChangePasswrod(int userId, string newPassword)
        {
            ValidataInputString(newPassword);

            if (newPassword.IsNullOrEmpty() || newPassword.Length < 6)
            {
                throw new ArgumentException("密码长度小于6！");
            }

            var changePasswordResult = _userInfoService.ChangePassword(userId, newPassword);

            if (changePasswordResult == ChangePasswordResult.Success)
            {
                return Success();
            }

            return Fail();
        }
    }
}