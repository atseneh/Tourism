using CNET_V7_Domain.Domain.AccountingSchema;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.ConsigneeSchema;
using CNET_V7_Domain.Domain.SecuritySchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Domain.ViewSchema;

namespace Ministry_of_Tourism_pro.Models
{
    public class ConsigneeBuffer
    {
        public string? loggedinTin { get; set; }
        public ConsigneeDTO? consignee { get; set; }
        public List<ConsigneeUnitDTO>? consigneeUnits { get; set; }
        public List<IdentificationDTO>? Identifications { get; set; }

        public List<ObjectStateDTO>? ObjectStates { get; set; }
        public List<ValueFactorDTO>? ValueFactors { get; set; }
        public List<AccountMapDTO>? AccountMaps { get; set; }
        public List<RelationDTO>? Relations { get; set; }
        public List<AttachmentDTO>? Attachments { get; set; }
        public List<ActivityDTO>? Activity { get; set; }
        
    }
}
