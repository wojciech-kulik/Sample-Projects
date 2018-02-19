using System.Collections.Generic;

namespace DynamicTableView.Models
{
    public class Section
    {
        public string Header { get; set; }
        public IList<ISectionRow> Rows { get; } = new List<ISectionRow>();
    }
}