using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PensionDisbursement
{
    public class PensionerDetail
    {
        
        public int Allowances { get; set; }
        public int SalaryEarned { get; set; }
        public PensionType PensionType { get; set; }
    }

    public enum PensionType
    {
        Self=1,
        Family = 2
    }

    public class Result
    {
        public string Message { get; set; }
        public PensionerDetail result { get; set; }
    }
}
