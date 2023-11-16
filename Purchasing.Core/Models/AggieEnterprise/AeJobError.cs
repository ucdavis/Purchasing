using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchasing.Core.Models.AggieEnterprise
{
    public class AeJobError
    {
        public AeJobErrorDetail[] G_1 { get; set; }
    }

    public class AeJobErrorSingle
    {
        public AeJobErrorDetail G_1 { get; set; }
    }

    public class AeJobErrorDetail
    {
        public string COLUMN_NAME { get; set; }
        public object COLUMN_VALUE { get; set; }
        public string ERROR_MESSAGE { get; set; }
        public string ERROR_CODE { get; set; }
    }

    public class AeJobErrorDetailCleaned
    {
        public string ColumnName { get; set; }
        public string ColumnValue { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorCode { get; set; }
    }
}
