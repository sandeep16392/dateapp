using System.Collections.Generic;

namespace DateApp.Core.DataModels
{
    public class PaginateResponseDm
    {
        public IEnumerable<UserListDm> Users { get; set; }
        public PaginationDm Pagination { get; set; }
    }
}
