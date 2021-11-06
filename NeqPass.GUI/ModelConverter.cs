using NeqPass.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NeqPass.GUI
{
    public class ModelConverter
    {
        public static List<Entry> ConvertToSave(ObservableCollection<EntryModel> entries)
        {
            List<Entry> output = new List<Entry>();

            for (int i = 0; i < entries.Count; i++)
            {
                var converted = new Entry()
                {
                    Name = entries[i].Name,
                    Comment = entries[i].Comment,
                    SiteUrl = entries[i].SiteUrl,
                    Accounts = new List<Account>(),
                    IconBase64 = entries[i].IconBase64,
                };

                for (int k = 0; k < entries[i].Accounts.Count; k++)
                {
                    converted.Accounts.Add(new Account()
                    {
                        Login = entries[i].Accounts[k].Login,
                        Comment = entries[i].Accounts[k].Comment,
                        Password = entries[i].Accounts[k].Password,
                    });
                }

                output.Add(converted);
            }

            return output;
        }

        public static ObservableCollection<EntryModel> Convert(List<Entry> entries)
        {
            ObservableCollection<EntryModel> output = new ObservableCollection<EntryModel>();

            for (int i = 0; i < entries.Count; i++)
            {
                var converted = new EntryModel()
                {
                    Name = entries[i].Name,
                    Comment = entries[i].Comment,
                    SiteUrl = entries[i].SiteUrl,
                    Accounts = new ObservableCollection<AccountModel>(),
                    IconBase64 = entries[i].IconBase64,
                };

                for (int k = 0; k < entries[i].Accounts.Count; k++)
                {
                    converted.Accounts.Add(new AccountModel()
                    {
                        Login = entries[i].Accounts[k].Login,
                        Comment = entries[i].Accounts[k].Comment,
                        Password = entries[i].Accounts[k].Password,
                    });
                }

                output.Add(converted);
            }

            return output;
        }
    }
}
