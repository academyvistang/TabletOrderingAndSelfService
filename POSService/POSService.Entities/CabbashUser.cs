using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSService.Entities
{
    public class CabbashUser
    {
        public int PersonID { get; set; }

        public int PersonTypeId { get; set; }

        public string Username { get; set; }

        public string MiddleName { get; set; }

        public string PicturePath { get; set; }


        public string PersonTypeName { get; set; } 

        public bool FullMember { get; set; }


    }
}
