using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rtrw.Client.Wasm.Models
{
    public class Report
    {
        public string Id { get; set; }=$"report-{Guid.NewGuid():N}";
        public Warga ReportedBy { get; set; }
        public DateTime ReportedAt { get; set; }
        public string? Reason { get; set; }
    }
}
