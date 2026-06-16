using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractLoader.Interfaces
{
    public interface IExcelParser
    {
        public ExcelRecord[] Parse(string path);
    }
}
