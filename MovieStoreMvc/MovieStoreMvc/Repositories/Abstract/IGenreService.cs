using ksiegarnia.Models.Domain;
using ksiegarnia.Models.DTO;

namespace ksiegarnia.Repositories.Abstract
{
    public interface IGenreService
    {
       bool Add(Genre model);
       bool Update(Genre model);
       Genre GetById(int id);
       bool Delete(int id);
       IQueryable<Genre> List();

    }
}
