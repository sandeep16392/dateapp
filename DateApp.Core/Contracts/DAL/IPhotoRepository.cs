using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DateApp.Core.EntityModels;

namespace DateApp.Core.Contracts.DAL
{
    public interface IPhotoRepository : IBaseRepository
    {
        Task<Photo> GetPhoto(int id);
        Task<IEnumerable<Photo>> GetPhotos();
        Task<Photo> GetMainPhoto(int userId);
    }
}
