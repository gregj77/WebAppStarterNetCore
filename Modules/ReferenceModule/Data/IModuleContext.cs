using System.Linq;
using System.Threading.Tasks;
using ReferenceModule.Models;

namespace ReferenceModule.Data
{
    internal interface IModuleContext
    {
        IQueryable<Model> Models { get; }

        Task<int> CreateModel(Model model);
        Task<Model> UpdateModel(Model model);
        Task<Model> DeleteModel(Model model);
    }
}