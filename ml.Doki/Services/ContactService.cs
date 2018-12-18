using System;
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
                contact = await contactStore.GetContactAsync(contact.Id);
            }

            return contact;
        }

        public async Task<Contact> GetContactByDisplayNameAsync(string name)
        {
            var contactStore = await ContactManager.RequestStoreAsync();
            var contacts = await contactStore.FindContactsAsync();

            var selectedContact = contacts.FirstOrDefault(c => c.DisplayName.ToLower() == name.ToLower());
            return selectedContact;
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
