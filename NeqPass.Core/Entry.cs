using System.Collections.Generic;

namespace NeqPass.Core
{
    public class Entry
    {
        public string Name { get; set; }
        public string SiteUrl { get; set; }
        public string Comment { get; set; }
        public string IconBase64 { get; set; }
        public List<Account> Accounts { get; set; }
    }

    public class Account
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Comment { get; set; }
    }
}
