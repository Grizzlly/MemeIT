using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeIT.Shared.Models
{
    public record struct MemeDto(Guid? MemeId, string? Description, Guid? CreatorId, string? CreatorUsername);
}
