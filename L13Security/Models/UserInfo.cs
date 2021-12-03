using System;
using System.Collections.Generic;

#nullable disable

namespace L13Security.Models
{
    public partial class UserInfo
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CreditCard { get; set; }
        public string Email { get; set; }
        public long? DocNumber { get; set; }
    }
}
