using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractLoader
{
    public struct ExcelRecord
    {
        public string? FileGuid { get; set; }
        public string? DocumentGuid { get; set; }
        public string? PathToFile { get; set; }
        public string? FileModifiedDate { get; set; }
        public string? FileModifiedUser { get; set; }
        public string? LoadResult { get; set; }
    }

}
