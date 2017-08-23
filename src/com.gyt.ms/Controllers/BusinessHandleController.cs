﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using com.gyt.ms.Models;
using Zer.AppServices;
using Zer.Entities;
using Zer.Framework.Cache;
using Zer.Framework.Exception;
using Zer.Framework.Extensions;
using Zer.Framework.Mvc.Logs.Attributes;
using Zer.GytDto;
using Zer.GytDto.SearchFilters;
using Zer.GytDto.Users;

namespace com.gyt.ms.Controllers
{
    public class BusinessHandleController : BaseController
    {
        private readonly IPeccancyRecrodService _peccancyRecrodService;
        private readonly IGYTInfoService _gytInfoService;
        private readonly ICompanyService _companyService;
        private readonly ITruckInfoService _truckInfoService;

        public BusinessHandleController(IPeccancyRecrodService peccancyRecrodService, IGYTInfoService gytInfoService, ICompanyService companyService, ITruckInfoService truckInfoService)
        {
            _peccancyRecrodService = peccancyRecrodService;
            _gytInfoService = gytInfoService;
            _companyService = companyService;
            _truckInfoService = truckInfoService;
        }

        // GET: BusinessHandle
        [UserActionLog("天然气车辆业务办理", ActionType.查询)]
        public ActionResult Index()
        {
            return View();
        }

        // GET: BusinessHandle
        [UserActionLog("天然气车辆业务办理", ActionType.查询)]
        public ActionResult Gas()
        {
            ViewBag.ProvinceList = CacheHelper.GetCache("Province").ToString().PartString(',');
            ViewBag.CharacterList = CacheHelper.GetCache("Character").ToString().PartString(',');
            return View();
        }

        // GET: BusinessHandle
        [UserActionLog("过户车辆业务办理", ActionType.查询)]
        public ActionResult Transfer()
        {
            ViewBag.ProvinceList = CacheHelper.GetCache("Province").ToString().PartString(',');
            ViewBag.CharacterList = CacheHelper.GetCache("Character").ToString().PartString(',');
            return View();
        }

        // GET: BusinessHandle
        [UserActionLog("已旧换新车辆业务办理", ActionType.查询)]
        public ActionResult New()
        {
            ViewBag.ProvinceList = CacheHelper.GetCache("Province").ToString().PartString(',');
            ViewBag.CharacterList = CacheHelper.GetCache("Character").ToString().PartString(',');
            return View();
        }

        /// <summary>
        /// 检查申办企业是否有违法记录
        /// </summary>
        /// <param name="companyName"></param>
        /// <returns></returns>
        [UnLog]
        public JsonResult CompanyPeccancyCheck(string companyName)
        {
            var result = _peccancyRecrodService.ExistsCompanyName(companyName);
            if (result)
            {
                return Fail();
            }
            return Success();
        }

        /// <summary>
        /// 已旧换新指标是否被使用
        /// </summary>
        /// <param name="truckNo"></param>
        /// <returns></returns>
        [UnLog]
        public JsonResult TruckRepetitionCheck(string truckNo)
        {
            var result = _gytInfoService.TargetIsUse(truckNo);
            return result ? Fail() : Success();
        }

        [UserActionLog("业务办理", ActionType.新增)]
        public JsonResult Commit(GYTInfoDto dto)
        {
            var validateResult = CommonValidate(dto);
            switch (dto.BusinessType)
            {
                case BusinessType.天然气车辆:
                    validateResult.AddRange(ValidateWithGasBusiness(dto));
                    break;

                case BusinessType.以旧换新车辆:
                    validateResult.AddRange(ValidateWithReplaceBusiness(dto));
                    break;

                case BusinessType.过户车辆:
                    validateResult.AddRange(ValidateWithTransferBusiness(dto));
                    break;

                default: return Fail("不正常的业务提交，请重试.");
            }

            if (validateResult.Any())
            {
                return Fail("办理条件不符合", validateResult);
            }

            var result = SaveCommitedData(dto);

            return result == null ? Fail("数据保存失败，请重试.") : Success(result);
        }

        private List<string> CommonValidate(GYTInfoDto dto)
        {
            var result = new List<string>();

            if (dto.BidCompanyName.IsNullOrEmpty())
            {
                result.Add("申办企业名称不能为空");
                return result;
            }

            if (dto.BidTruckNo.Substring(2).IsNullOrEmpty())
            {
                result.Add("申办车牌号不能为空");
                return result;
            }

            // 申办企业不能有违法记录
            if (_peccancyRecrodService.ExistsCompanyName(dto.BidCompanyName))
            {
                result.Add("申办企业存在超载超限记录,不符合办理条件");
            }

            // 新申请车牌不能存在已办理记录
            var recordDto = _gytInfoService.GetByBidTruckNo(dto.BidTruckNo);
            if (recordDto != null)
            {
                result.Add(string.Format("申办车牌号[{0}]已经办理港运通,编号为[{1}]", recordDto.BidTruckNo, recordDto.Id));
            }

            return result;
        }

        private List<string> ValidateWithGasBusiness(GYTInfoDto dto)
        {
            // 天然气业务
            return new List<string>();
        }

        private List<string> ValidateWithReplaceBusiness(GYTInfoDto dto)
        {
            // 以旧换新业务
            var result = new List<string>();

            // 旧车必须有办理记录
            if (_gytInfoService.GetByBidTruckNo(dto.OriginalTruckNo) == null)
            {
                result.Add("原车牌不存在港运通办理记录，不能办理以旧换新业务");
            }

            return result;
        }

        private List<string> ValidateWithTransferBusiness(GYTInfoDto dto)
        {
            // 车辆过户业务
            var result = new List<string>();

            // 旧车必须有办理记录
            if (_gytInfoService.GetByBidTruckNo(dto.OriginalTruckNo) == null)
            {
                result.Add("原车牌不存在港运通办理记录，不能办理车辆过户业务");
            }

            return result;
        }

        private GYTInfoDto SaveCommitedData(GYTInfoDto dto)
        {
            //检测公司是否存在
            var companyList = new List<CompanyInfoDto>
            {
                new CompanyInfoDto() {CompanyName = dto.BidCompanyName},
                new CompanyInfoDto() {CompanyName = dto.OriginalCompanyName}
            };

            companyList = _companyService.QueryAfterValidateAndRegist(companyList);

            var bidCompanyInfo = companyList.FirstOrDefault(x => x.CompanyName.Trim() == dto.BidCompanyName.Trim());

            dto.BidCompanyId = bidCompanyInfo != null ? bidCompanyInfo.Id : 0;

            if (!dto.OriginalCompanyName.IsNullOrEmpty())
            {
                var originalCompanyInfo = companyList.FirstOrDefault(x => x.CompanyName.Trim() == dto.OriginalCompanyName.Trim());

                dto.OriginalCompanyId = originalCompanyInfo != null ? originalCompanyInfo.Id : 0;
            }

            // 检测车辆信息
            var waitForRegistTruckInfo = new List<TruckInfoDto>
            {
                new TruckInfoDto(){FrontTruckNo =  dto.BidTruckNo,CompanyId = dto.BidCompanyId,CompanyName = dto.BidCompanyName}
            };

            if (dto.BusinessType.ToInt() > 0)
            {
                waitForRegistTruckInfo.Add(new TruckInfoDto
                {
                    FrontTruckNo = dto.OriginalTruckNo,
                    CompanyId = dto.OriginalCompanyId ?? 0,
                    CompanyName = dto.OriginalCompanyName
                });
            }

            _truckInfoService.QueryAfterValidateAndRegist(waitForRegistTruckInfo);

            var userInfoDto = GetValueFromSession<UserInfoDto>("UserInfo");

            dto.BidName = userInfoDto.UserName;
            dto.BidDisplayName = userInfoDto.DisplayName;
            dto.Status = BusinessState.已办理;
            dto.BidDate = DateTime.Now;

            return _gytInfoService.Add(dto);
        }

        /// <summary>
        /// 检查是否重复办理
        /// </summary>
        /// <param name="truckNo"></param>
        /// <returns></returns>
        [UnLog]
        public JsonResult TruckNoExistsHandle(string truckNo)
        {
            var isExists = _gytInfoService.Exists(truckNo);
            if (isExists)
            {
                return Fail();
            }

            return Success();
        }
    }
}