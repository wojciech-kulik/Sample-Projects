using System.Collections.Generic;
using DynamicTableView.Models;
using DynamicTableView.Utils;
using Xamarin.Forms;

namespace DynamicTableView.Extensions
{
    public static class DynamicTableSections
    {
        public static readonly BindableProperty SectionsProperty =
            BindableProperty.CreateAttached("SectionsProperty",
              typeof(IList<Section>),
              typeof(DynamicTableSections),
              null, BindingMode.OneWay, propertyChanged: SectionsChanged);

        private static void SectionsChanged(BindableObject source, object oldVal, object newVal)
        {
            // when sections change we need to rebuild our TableView content

            var tableView = (TableView)source;
            var newSections = (IList<Section>)newVal;

            tableView.Root.Clear();

            if (newSections == null)
            {
                return;
            }

            foreach (var section in newSections)
            {
                tableView.Root.Add(SectionsFactory.CreateSection(section));
            }
        }
    }
}
