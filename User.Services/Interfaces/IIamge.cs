using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Services.DataTransferObjects.images;

namespace User.Services.Interfaces
{
    public  interface IIamge
    {
        public Task<bool> addProfileImage(ImageDto image);
        public Task<bool> editProfileImage(ImageDto image);
        public Task<bool> deleteProfileImage();
        public Task<bool> addCoverImage(ImageDto image);
        public Task<bool> deleteCoverImage();
        public Task<bool> editCoverImage(ImageDto image);

    }
}
