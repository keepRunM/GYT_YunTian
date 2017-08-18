﻿
using System;
using System.Web.Mvc;
using Zer.AppServices;
using Zer.Entities;
using Zer.Framework.Exception;
using Zer.Framework.Extensions;
using Zer.Framework.Mvc.Logs;
using Zer.Framework.Mvc.Logs.Attributes;
using Zer.GytDto;
using Zer.GytDto.Users;
using ActionType = Zer.Entities.ActionType;


namespace com.gyt.ms.Controllers
{
    public class UserController : BaseController
    {
        public UserController()
        {

        }

        private readonly IUserInfoService _userInfoService;
        public UserController(IUserInfoService userInfoService)
        {
            _userInfoService = userInfoService;
        }

        [UserActionLog("新增用户", ActionType.新增)]
        public JsonResult Regist(UserInfoDto userInfoDto)
        {
            if (userInfoDto.UserName.IsNullOrEmpty() ||
                userInfoDto.Password.IsNullOrEmpty())
            {
                throw new ArgumentException("用户名或密码不能为空！");
            }

            if (
                userInfoDto.UserName.Length <= 6 ||
                userInfoDto.Password.Length < 6)
            {
                throw new ArgumentException("用户名或密码的长度不能小于6！");
            }

            userInfoDto.Password = userInfoDto.Password.Md5Encoding();

            var registResult = _userInfoService.Regist(userInfoDto);

            if (registResult == RegistResult.UserNameExists)
            {
                return Fail();
            }

            return Success();
        }

        [UnValidateLogin]
        [UnLog]
        [HttpPost]
        public ActionResult Login(string userName, string password)
        {
            var loginResult = _userInfoService.VerifyUserNameAndPassword(userName, password.Md5Encoding());

            UserActionLogger.Instance.Info(new LogInfoDto()
            {
                ActionModel = "登录",
                ActionType =  ActionType.查询,
                Content = string.Format("UserName:{0}, Result:{1}",userName,loginResult),
                CreateTime = DateTime.Now
            });

            switch (loginResult)
            {
                case LoginStatus.IncorrectPassword: return Fail("密码错误.");
                case LoginStatus.UserFrozen: return Fail("用户被冻结.");
                case LoginStatus.UserNameNotExists: return Fail("用户名不存在.");
                default:
                {
                    var userinfoDto = _userInfoService.GetByUserName(userName);

                    Session["UserInfo"] = userinfoDto;
                    return Success();
                }
            }
        }

        public ActionResult Logout()
        {
            var userInfoDto = GetValueFromSession<UserInfoDto>("UserInfo");

            UserActionLogger.Instance.Info(new LogInfoDto()
            {
                ActionModel = "登出/注销",
                ActionType = ActionType.更改状态,
                Content = string.Format("用户名:{0}, 真实姓名:{1}", userInfoDto.UserName, userInfoDto.DisplayName),
                CreateTime = DateTime.Now
            });

            Session["UserInfo"] = null;
            return RedirectToAction("Login", "Home");
        }

        [UserActionLog("用户状态变更-->冻结", ActionType.更改状态)]
        public JsonResult Frozon(int userId)
        {
            var userInfoDto = _userInfoService.GetById(userId);

            if (userInfoDto.State != UserState.Active)
            {
                return Fail("用户已经是冻结状态！");
            }

            var frozonResult = _userInfoService.LetUserFrozen(userId);

            return frozonResult == FrozenResult.Success ? Success() : Fail();
        }

        [UserActionLog("用户状态变更-->激活", ActionType.更改状态)]
        public JsonResult Thaw(int userId)
        {
            var userInfoDto = _userInfoService.GetById(userId);

            if (userInfoDto.State != UserState.Frozen)
            {
                return Fail("用户已经是活动状态！");
            }

            var frozonResult = _userInfoService.LetUserThaw(userId);

            if (frozonResult == ThawResult.Success)
            {
                return Success();
            }

            return Fail();
        }

        [UserActionLog("用户修改密码", ActionType.编辑)]
        public JsonResult ChangePasswrod(string newPassword, string currentPassword)
        {
            newPassword = newPassword.Md5Encoding();
            currentPassword = currentPassword.Md5Encoding();

            if (currentPassword != CurrentUser.Password)
            {
                return Fail("当前密码错误！");
            }

            if (newPassword.IsNullOrEmpty() || newPassword.Length < 6)
            {
                throw new ArgumentException("密码长度小于6！");
            }

            var userInfoDto = _userInfoService.GetById(CurrentUser.UserId);

            if (userInfoDto.Password == newPassword)
            {
                return Fail("修改失败，与旧密码相同！");
            }

            var changePasswordResult = _userInfoService.ChangePassword(CurrentUser.UserId, newPassword);

            if (changePasswordResult == ChangePasswordResult.Success)
            {
                return Success();
            }

            return Fail();
        }

        public ActionResult ChangePassword(int activeId = 0)
        {
            ViewBag.ActiveId = activeId;
            return View();
        }

        public ActionResult AccountManage(int activeId = 0)
        {
            ViewBag.ActiveId = activeId;
            ViewBag.Result = _userInfoService.GetAll();
            return View();
        }

        [UserActionLog("查看用户详细信息", ActionType.编辑)]
        public ActionResult AccountInfo(int activeId = 0, int userId = 0)
        {
            ViewBag.ActiveId = activeId;
            ViewBag.UserInfo = _userInfoService.GetById(userId);
            return View();
        }

        public ActionResult AddUserInfo(int activeId = 0)
        {
            ViewBag.ActiveId = activeId;
            return View();
        }

        public ActionResult EditUserInfo(int activeId = 0, int userId = 0)
        {
            ViewBag.ActiveId = activeId;
            ViewBag.Result = _userInfoService.GetById(userId);
            return View();
        }

        [UserActionLog("编辑个人信息", ActionType.编辑)]
        public JsonResult Edit(UserInfoDto userInfoDto)
        {
            if (userInfoDto.UserName.IsNullOrEmpty())
            {
                throw new ArgumentException("用户名不能为空！");
            }

            if (userInfoDto.UserName.Length <= 6)
            {
                throw new ArgumentException("用户名的长度不能小于6！");
            }

            var dto = _userInfoService.GetById(userInfoDto.UserId);

            if (dto == null)
            {
                throw new ArgumentException("未找到对应的用户！");
            }

            dto.UserName = userInfoDto.UserName;
            dto.DisplayName = userInfoDto.DisplayName;
            dto.Email = userInfoDto.Email;
            dto.MobilePhone = userInfoDto.MobilePhone;

            _userInfoService.Edit(dto);

            return Success();
        }
    }
}