using System;
using Zer.Entities;
using Zer.Framework.Attributes;
using Zer.Framework.Dto;
using Zer.Framework.Export.Attributes;
using Zer.Framework.Extensions;

namespace Zer.GytDto
{
    [Serializable]
    public class LngAllowanceInfoDto : DtoBase
    {
        [ExportSort(1)]
        [ImprotIgnore]
        [ExportDisplayName("编号")]
        public string Id { get; set; }

        [ExportIgnore]
        [ImprotIgnore]
        public int CompanyId { get; set; }

        [ExportSort(2)]
        [ImportSort(1)]
        [ExportDisplayName("公司名称")]
        public string CompanyName { get; set; }

        [ExportSort(3)]
        [ImportSort(2)]
        [ExportDisplayName("批次")]
        public int LotId { get; set; }

        [ExportSort(4)]
        [ImportSort(3)]
        [ExportDisplayName("车牌号")]
        public string TruckNo { get; set; }

        [ExportSort(5)]
        [ImportSort(4)]
        [ExportDisplayName("发动机号")]
        public string EngineId { get; set; }

        [ExportSort(6)]
        [ImportSort(5)]
        [ExportDisplayName("汽缸1编号")]
        public string CylinderDefaultId { get; set; }

        [ExportSort(7)]
        [ImportSort(6)]
        [ExportDisplayName("汽缸2编号")]
        public string CylinderSeconedId { get; set; }

        [ExportSort(8)]
        [ImportSort(7)]
        [ExportDisplayName("创建日期")]
        public DateTime CreateTime { get; set; }
        
        [ExportSort(9)]
        [ImportSort(8)]
        [ExportDisplayName("补贴状态")]
        public LngStatus Status { get; set; }


        [ExportSort(10)]
        [ImprotIgnore]
        [ExportDisplayName("是否强制导入")]
        public bool IsForceImport { get; set; }

        [ExportSort(11)]
        [ImportSort(9)]
        [ExportDisplayName("备注")]
        public string Description { get; set; }
    }
}