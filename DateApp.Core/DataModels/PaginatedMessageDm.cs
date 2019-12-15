using System.Collections.Generic;

namespace DateApp.Core.DataModels
{
    public class PaginatedMessageDm
    {
        public IEnumerable<MessageToReturnDm> Messages { get; set; }
        public PaginationDm Pagination { get; set; }
    }
}
