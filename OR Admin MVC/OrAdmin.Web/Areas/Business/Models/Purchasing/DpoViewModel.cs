using OrAdmin.Entities.Purchasing;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web.Security;
using OrAdmin.Core.Enums.Purchasing;

namespace OrAdmin.Web.Areas.Business.Models.Purchasing
{
    public class DpoViewModel
    {
        public Types.FormMode? FormMode { get; set; }
        public Request Request { get; set; }
        public Request LastRequest { get; set; }
        public Vendor Vendor { get; set; }
        public ShipToAddress Address { get; set; }
        public IEnumerable<DpoItem> DpoItems { get; set; }
        public IEnumerable<PiApproval> PIApprovals { get; set; }
        public IEnumerable<Attachment> Attachments { get; set; }

        public SelectList ApproverList { get; set; }
        public SelectList PurchaserList { get; set; }
        public SelectList RequestedAccountList { get; set; }
        public SelectList ShippingMethodList { get; set; }
        public SelectList VendorList { get; set; }
        public SelectList ShipToAddressList { get; set; }
    }
}