﻿using System;
using Zer.Entities;
using Zer.Framework.Attributes;
using Zer.Framework.Dto;
using Zer.Framework.Export.Attributes;

namespace Zer.GytDto
{
    public class CardsInfoDto:DtoBase
    {
        [ExportSort(1)]
        [ExportDisplayName("办理编号")]
        public string Id { get; set; }

        /// <summary>
        /// 原企业编号
        /// </summary>
        [ExportIgnore]
        [ImprotIgnore]
        public int OriginalCompanyId { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        [ExportSort(2)]
        [ExportDisplayName("业务类型")]
        public int BusinessType { get; set; }

        [ExportSort(3)]
        [ExportDisplayName("原企业")]
        public int OriginalCompanyName { get; set; }

        /// <summary>
        ///原车牌号
        /// </summary>
        [ExportSort(4)]
        [ExportDisplayName("原车牌号")]
        public string OriginalTruckNo { get; set; }

        /// <summary>
        /// 申办企业编号
        /// </summary>
        [ExportIgnore]
        public int RequestCompanyId { get; set; }

        /// <summary>
        /// 申办企业名称
        /// </summary>
        [ExportSort(5)]
        [ExportDisplayName("申办企业名称")]
        public int RequestCompanyNamed { get; set; }

        /// <summary>
        /// 申报车牌号
        /// </summary>
        [ExportSort(6)]
        [ExportDisplayName("申报车牌号")]
        public string RequestTruckNo { get; set; }

        /// <summary>
        /// 申办日期
        /// </summary>
        [ExportSort(7)]
        [ExportDisplayName("申办日期")]
        public DateTime RequestDateTime { get; set; }

        /// <summary>
        /// 经办人
        /// </summary>
        [ExportSort(8)]
        [ExportDisplayName("经办人")]
        public string Handler { get; set; }

        /// <summary>
        /// 业务状态
        /// </summary>
        [ExportSort(9)]
        [ExportDisplayName("业务状态")]
        public BusinessState BusinessState { get; set; }

        /// <summary>
        /// 初审人编号
        /// </summary>
        [ExportIgnore]
        public int FristAuditorId { get; set; }

        /// <summary>
        /// 再审人编号
        /// </summary>
        [ExportIgnore]
        public int AgainAuditorId { get; set; }

        /// <summary>
        /// 终审人编号
        /// </summary>
        [ExportIgnore]
        public int LastAuditorId { get; set; }
    }


    
}
