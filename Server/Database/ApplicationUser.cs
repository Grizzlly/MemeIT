using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MemeIT.Server.Database
{
    [PrimaryKey(nameof(Id))]
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ApplicationUser()
        {
            Memes = new HashSet<Meme>();
        }

        public virtual ICollection<Meme> Memes { get; set; }
    }
}
