using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSService.Entities
{
    public class BarTable
    {
        public int Id { get; set; }

        public bool Printed { get; set; } 

        public string TableName { get; set; }
        public string Telephone { get; set; }

        public string TableAlias { get; set; }
        public bool IsActive { get; set; }
        public int GuestId { get; set; }
        public int GuestNumber { get; set; }

        public int TableId { get; set; }
        public int StaffId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string GuestGuid { get; set; }
        public bool Takeaway { get; set; }

        public decimal TableTotal { get; set; }
    }
}
