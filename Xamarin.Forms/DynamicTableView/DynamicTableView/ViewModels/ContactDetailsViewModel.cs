using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DynamicTableView.Models;
using Xamarin.Forms;

namespace DynamicTableView.ViewModels
{
    public class ContactDetailsViewModel : BindableObject
    {
        private List<Contact> contacts;
        public ObservableCollection<Section> Sections { get; private set; }

        public ContactDetailsViewModel()
        {
            this.SetUpContacts();
            this.SetContact(0);
        }

        private void SetUpContacts()
        {
            this.contacts = new List<Contact>
            {
                new Contact
                {
                    FirstName = "Jack",
                    LastName = "Smith",
                    Company = "Google",
                    Height = 183,
                    Notes = "First contact note\nSecond line",
                    YearOfBirth = 1985
                },
                new Contact
                {
                    FirstName = "Jack",
                    LastName = "Smith",
                },
                new Contact
                {
                    FirstName = "Jack",
                    LastName = "Smith",
                    Company = "Google",
                    Height = 183,
                    YearOfBirth = 1985
                },
                new Contact
                {
                    Company = "Google"
                }
            };
        }

        private void SetContact(int index)
        {
            var contact = this.contacts[index];

            var basicSection = new Section { Header = "Basic Information" };
            var personalSection = new Section { Header = "Personal Details" };
            var notesSection = new Section { Header = "Notes" };
            var buttonSection = new Section { Header = "Actions" };

            // basic information
            this.AddTextRowIfNotEmpty(basicSection.Rows, "First Name", contact.FirstName);
            this.AddTextRowIfNotEmpty(basicSection.Rows, "Last Name", contact.LastName);
            this.AddTextRowIfNotEmpty(basicSection.Rows, "Company", contact.Company);

            // personal details
            this.AddTextRowIfNotEmpty(personalSection.Rows, "Year of Birth", contact.YearOfBirth?.ToString());
            this.AddTextRowIfNotEmpty(personalSection.Rows, "Height", contact.Height?.ToString());

            // notes
            if (!string.IsNullOrEmpty(contact.Notes))
            {
                notesSection.Rows.Add(new EditorRow { Text = contact.Notes });
            }

            // actions
            buttonSection.Rows.Add(new ButtonRow 
            { 
                Title = "Next Contact",
                OnClickAction = () => this.SetContact((index + 1) % this.contacts.Count)
            });

            var sections = new List<Section>
            {
                basicSection,
                personalSection,
                notesSection,
                buttonSection
            };

            var nonEmptySections = sections.Where(x => x.Rows.Any());
            this.Sections = new ObservableCollection<Section>(nonEmptySections);

            // notify view, that sections have been changed
            this.OnPropertyChanged(nameof(this.Sections));
        }

        private void AddTextRowIfNotEmpty(IList<ISectionRow> rows, string title, string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                rows.Add(new TextValueRow { Title = title, Value = text });
            }
        }
    }
}
