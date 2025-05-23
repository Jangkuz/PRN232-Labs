using BusinessObject;
using BusinessObject.Filter;
using LinqKit;
using PRN232.Lab1.Repo.Interfaces;
using PRN232.Lab1.Repo.Paging;
using System.Linq.Expressions;

namespace PRN232.Lab1.Repo.Implements;

public class ManufactureRepository : GenericRepository<Manufacturer, string>, IManufactureRepository
{
    public ManufactureRepository(Lab1PharmaceuticalDbContext context) : base(context)
    {
    }

    public async Task<PaginationResult<Manufacturer>> GetAllFilteredAsync(ManufacturerFilter filter, int pageNumber, int pageSize)
    {
        //Expression<Func<Manufacturer, bool>>? predicate = null;
        var predicate = PredicateBuilder.New<Manufacturer>(true);

        if (!string.IsNullOrEmpty(filter.CountryOfOrigin))
            predicate = predicate.And(m => m.CountryofOrigin.Contains(filter.CountryOfOrigin));

        if (filter.MinYearEstablished.HasValue)
            predicate = predicate.And(m => m.YearEstablished > filter.MinYearEstablished);

        if (filter.MaxYearEstablished.HasValue)
            predicate = predicate.And(m => m.YearEstablished < filter.MaxYearEstablished);

        //predicate = m => (
        //    (!string.IsNullOrEmpty(filter.CountryOfOrigin) ? m.CountryofOrigin.Contains(filter.CountryOfOrigin) : true)
        //    && (filter.MinYearEstablished.HasValue ? m.YearEstablished > filter.MinYearEstablished : true)
        //    && (filter.MaxYearEstablished.HasValue ? m.YearEstablished < filter.MaxYearEstablished : true)
        //    );


        Func<IQueryable<Manufacturer>, IOrderedQueryable<Manufacturer>>? sortBy = null;

        if (!string.IsNullOrEmpty(filter.SortBy))
        {
            sortBy = query =>
            {
                return filter.SortBy.ToLower() switch
                {
                    "name" => filter.SortDesc ? query.OrderByDescending(m => m.ManufacturerName) : query.OrderBy(m => m.ManufacturerName),
                    "year" => filter.SortDesc ? query.OrderByDescending(m => m.YearEstablished) : query.OrderBy(m => m.YearEstablished),
                    _ => query.OrderBy(m => m.Id)
                };
            };
        }

        return await AsPaginated(
            filter: predicate,
            orderBy: sortBy,
            page: pageNumber,
            pageSize: pageSize);
    }
}
