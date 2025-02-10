using Org.BouncyCastle.Bcpg.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Services.DataTransferObjects.images;

namespace User.Services.Interfaces
{
    public  interface ICVService
    {
        public Task<bool> UpdateCv(ImageDto cv);
        public Task<bool> DeleteCv();

    }
}
