﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zer.Framework.Entities;

namespace Zer.Entities
{
    public class GYTInfo:EntityBase
    {
        public BusinessType BusinessType { get; set; }

        public int OriginalCompanyId { get; set; }

        [MaxLength(100)]
        public string OriginalCompanyName { get; set; }

        [MaxLength(20)]
        public string OriginalTruckNo { get; set; }

        public int BidCompanyId { get; set; }

        [MaxLength(100)]
        public string BidCompanyName { get; set; }

        [MaxLength(20)]
        public string BidTruckNo { get; set; }

        public DateTime? BidDate { get; set; }

        [MaxLength(20)]
        public string BidName { get; set; }

        public BusinessState Status { get; set; }
    }

    /// <summary>
    /// 业务类型
    /// </summary>
    public enum BusinessType
    {
        天然气车辆 = 0,
        过户车辆 = 1,
        以旧换新车辆 = 2
    }

    /// <summary>
    /// 信息状态
    /// </summary>
    public enum BusinessState
    {
        初始 = 0,
        未通过 = 4,
        已通过 = 8
    }
}