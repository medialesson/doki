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
        public async Task PromptForPermissionsAsync()
        {
            await ContactManager.RequestStoreAsync();
        }

        public async Task<Contact> GetContactByDisplayNameAsync(string name)
        {
            var contactStore = await ContactManager.RequestStoreAsync();
            var contacts = await contactStore.FindContactsAsync();

            var selectedContact = contacts.FirstOrDefault(c => c.DisplayName == name);
            return selectedContact;
        }

        public async Task<BitmapImage> LoadContactAvatarToBitmapAsync(Contact contact)
        {
            if (contact?.SourceDisplayPicture != null)
            {
                var bitmap = new BitmapImage();
                bitmap.SetSource(await contact.Thumbnail.OpenReadAsync());
                return bitmap;
            }

            return new BitmapImage();
        }
    }
}