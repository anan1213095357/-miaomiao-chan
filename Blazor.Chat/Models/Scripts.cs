using FreeSql.DataAnnotations;
using System.ComponentModel;
using System.Data;
using System.Security;

namespace Blazor.Chat.Models
{
    public enum ScriptAuditStatus
    {
        待审核,
        带商讨,
        已通过,
        未通过
    }


    [Index("uk_phone", "Phone", true)]
    [Index("uk_account", "Account", true)]
    public class Scripts
    {
        [DisplayName("ID"), Column(IsIdentity = true, IsPrimary = true)]
        public long ID { get; set; }

        [DisplayName("代码路径")]
        public string? CodeFilePath { get; set; }


        public long PersonID { get; set; }

        [Navigate(nameof(PersonID))]
        [DisplayName("人员")]
        public Person? Person { get; set; }

        [DisplayName("脚本审核状态")]
        public ScriptAuditStatus ScriptAuditStatus { get; set; } = default!;

    }
}
