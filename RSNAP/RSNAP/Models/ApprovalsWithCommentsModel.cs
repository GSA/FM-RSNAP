using RSNAP.EFModels;
using System.Collections.Generic;

namespace RSNAP.Models
{
    public class ApprovalsWithCommentsModel : ApprovalsModel
    {
        public List<PendingroCommentLog> AllComments { get; set; }
    }
}