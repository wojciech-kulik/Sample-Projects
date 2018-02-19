using DynamicTableView.Models;
using Xamarin.Forms;

namespace DynamicTableView.Utils
{
    public static class SectionsFactory
    {
        public static TableSection CreateSection(Section section)
        {
            var tableSection = new TableSection(section.Header);

            foreach (var row in section.Rows)
            {
                if (row is TextValueRow)
                {
                    tableSection.Add(GetTextValueRow(row as TextValueRow));
                }
                else if (row is ButtonRow)
                {
                    tableSection.Add(GetButtonRow(row as ButtonRow));
                }
                else if (row is EditorRow)
                {
                    tableSection.Add(GetEditorRow(row as EditorRow));
                }
            }

            return tableSection;
        }

        private static Cell GetTextValueRow(TextValueRow row)
        {
            var grid = new Grid
            {
                BackgroundColor = Color.White,
                Padding = new Thickness(15, 0),
                HeightRequest = 45
            };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) });

            grid.Children.Add(new Label
            {
                Text = row.Title,
                VerticalTextAlignment = TextAlignment.Center
            }, 0, 0);
            grid.Children.Add(new Label
            {
                Text = row.Value,
                TextColor = Color.DarkGray,
                VerticalTextAlignment = TextAlignment.Center
            }, 1, 0);

            return new ViewCell { View = grid };
        }

        private static Cell GetButtonRow(ButtonRow row)
        {
            var button = new Button 
            { 
                Text = row.Title, 
                BackgroundColor = Color.White,
                HeightRequest = 45
            };
            button.Clicked += (sender, e) => row.OnClickAction?.Invoke();

            return new ViewCell { View = button };
        }

        private static Cell GetEditorRow(EditorRow row)
        {
            return new ViewCell
            {
                Height = 140,
                View = new Grid 
                {
                    BackgroundColor = Color.White,
                    Children = 
                    { 
                        new Editor 
                        { 
                            Text = row.Text, 
                            Margin = new Thickness(10, 0, 0, 0), 
                            IsEnabled = false 
                        } 
                    }
                } 
            };
        }
    }
}
