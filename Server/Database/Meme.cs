using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemeIT.Server.Database
{
    [PrimaryKey(nameof(MemeId))]
    public partial class Meme
    {
        public Guid MemeId { get; set; }

        public string Description { get; set; } = default!;

        public Guid? CreatorId { get; set; }
        public virtual ApplicationUser? Creator { get; set; }
    }
}
