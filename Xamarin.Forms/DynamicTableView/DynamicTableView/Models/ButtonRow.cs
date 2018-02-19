using System;

namespace DynamicTableView.Models
{
    public class ButtonRow : ISectionRow
    {
        public string Title { get; set; }
        public Action OnClickAction { get; set; }
    }
}