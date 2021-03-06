using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Zer.Entities;
using Zer.Framework.Helpers;
using Zer.GytDto;
using Zer.GytDto.Users;

namespace Zer.Framework.Mvc.Logs.Attributes
{
    public class UserActionLogAttribute : ActionFilterAttribute
    {
        public UserActionLogAttribute(string actionModel, ActionType actionType)
        {
            ActionModel = actionModel;
            ActionType = actionType;
        }

        private string ActionModel { get; set; }
        private ActionType ActionType { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(UnLogAttribute), false).Any())
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            var logInfoDto = new LogInfoDto();

            logInfoDto.UserId = -1;
            logInfoDto.DisplayName = "�Ƿ��û�";

            if (filterContext.HttpContext.Session != null)
            {
                var userInfoDto = filterContext.HttpContext.Session["UserInfo"] as UserInfoDto;
                if (userInfoDto == null)
                {
                    filterContext.Result = new RedirectResult("~/home/login");
                    return;
                }

                logInfoDto.UserId = userInfoDto.UserId;
                logInfoDto.DisplayName = userInfoDto.DisplayName;
            }

            logInfoDto.ActionModel = ActionModel;
            logInfoDto.ActionType = ActionType;

            string content = string.Empty;

            try
            {
                JavaScriptSerializer json = new JavaScriptSerializer();

                content = json.Serialize(filterContext.ActionParameters);
            }
            catch
            {
                
            }

            logInfoDto.Content = content;
            logInfoDto.CreateTime = DateTime.Now;
            logInfoDto.IP = IpHelper.GetWebClientIp();

            UserActionLogger.Instance.Info(logInfoDto);
            
            base.OnActionExecuting(filterContext);
        }
    }
}

