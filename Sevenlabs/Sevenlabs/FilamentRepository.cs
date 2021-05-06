using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevenlabs
{
    public interface FilamentRepository
    {
        Task<List<FilamentModel>> GetFilaments(string creator, string type, string color);
        Task<FilamentModel> UpdateFilaments(FilamentModel model);
        Task<FilamentModel> AddFilaments(object model);
        Task<FilamentModel> DeleteFilaments(FilamentModel model);
    }
}
