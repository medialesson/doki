﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Xaml.Media.Imaging;

namespace ml.Doki.Services
{
    public class ContactService
    {
        public async Task<ContactStore> PromptForPermissionsAsync()
        {
            return await ContactManager.RequestStoreAsync(ContactStoreAccessType.AllContactsReadOnly);
        }

        public async Task<Contact> PromptUserForContactAsync()
        {
            // Open the prompt
            var contactPicker = new ContactPicker()
            {
                SelectionMode = ContactSelectionMode.Contacts
            };

            Contact contact = await contactPicker.PickContactAsync();

            if (contact != null)
            {
                // Fetch all the details
                ContactStore contactStore = await PromptForPermissionsAsync();

                if(contactStore != null)
                    contact = await contactStore.GetContactAsync(contact.Id);
            }

            return contact;
        }

        public async Task<Contact> GetContactByDisplayNameAsync(string name)
        {
            var contactStore = await ContactManager.RequestStoreAsync();
            if(contactStore != null)
            {
                var contacts = await contactStore.FindContactsAsync();

                var selectedContact = contacts.FirstOrDefault(c => c.DisplayName.ToLower() == name.ToLower());
                return selectedContact;
            }
            else
            {
                return new Contact
                {
                    DisplayNameOverride = name
                };
            }
        }

        public async Task<IList<Contact>> SearchContactsByNameAsync(string query, bool distinct = false)
        {
            var contactStore = await ContactManager.RequestStoreAsync();
            if(contactStore == null)
            {
                return new List<Contact>();
            }

            var contacts = await contactStore.FindContactsAsync();

            var results = contacts.Where(c =>
                c.DisplayName.ToLower().Contains(query.ToLower()));

            if (distinct)
                results = results.Distinct();

            return results.ToList();
        }

        public async Task<BitmapImage> LoadContactAvatarToBitmapAsync(Contact contact)
        {
            if (contact?.SourceDisplayPicture != null)
            {
                var bitmap = new BitmapImage();
                bitmap.SetSource(await contact.SourceDisplayPicture.OpenReadAsync());
                return bitmap;
            }

            return new BitmapImage();
        }
    }
}
