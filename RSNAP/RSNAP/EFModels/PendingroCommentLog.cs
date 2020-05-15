using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RSNAP.EFModels
{
    public partial class PendingroCommentLog
    {
        public string ProCommentId { get; set; }
        public string ProId { get; set; }
        public DateTime CommentDate { get; set; }
        public string UserId { get; set; }
        public string ProComment { get; set; }
    }
}
