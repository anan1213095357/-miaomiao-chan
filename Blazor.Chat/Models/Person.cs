using FreeSql.DataAnnotations;
using System.ComponentModel;
using System.Data;
using System.Security;

namespace Blazor.Chat.Models
{

    [Index("uk_phone", "Phone", true)]
    [Index("uk_account", "Account", true)]
    public class Person
    {
        [DisplayName("ID"),Column(IsIdentity = true,IsPrimary = true)]
        public long ID { get; set; }

        [DisplayName("名称")]
        public string Name { get; set; } = default!;

        [DisplayName("邮箱")]
        public string? Email { get; set; }

        [DisplayName("角色")]
        public string? Roles { get; set; }


        [DisplayName("账户")]
        public string Account { get; set; } = default!;

        [DisplayName("密码")]
        public string Password { get; set; } = default!;


        [DisplayName("手机号码")]
        public string? Phone { get; set; }

        [DisplayName("余额")]
        public decimal Balance { get; set; } = 0m;
    }
}
