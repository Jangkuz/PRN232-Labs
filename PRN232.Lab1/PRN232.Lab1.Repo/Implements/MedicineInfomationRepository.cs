using BusinessObject;
using BusinessObject.Filter;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using PRN232.Lab1.Repo.Interfaces;
using PRN232.Lab1.Repo.Paging;
using System.Linq.Expressions;

namespace PRN232.Lab1.Repo.Implements;

public class MedicineInfomationRepository : GenericRepository<MedicineInformation, string>, IMedicineInfomationReposiroty
{
    public MedicineInfomationRepository(Lab1PharmaceuticalDbContext context) : base(context)
    {
    }

    public async Task<PaginationResult<MedicineInformation>> GetAllFilteredAsync(MedicineFilter filter, int pageNumber, int pageSize)
    {
        //Expression<Func<MedicineInformation, bool>>? predicate = null;
        var predicate = PredicateBuilder.New<MedicineInformation>(true);

#pragma warning disable IDE0075 // Simplify conditional expression
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        if (!string.IsNullOrEmpty(filter.ManufacturerId))
            predicate = predicate.And(m => m.ManufacturerId.Contains(filter.ManufacturerId));

        if (!string.IsNullOrEmpty(filter.ActiveIngredient))
            predicate = predicate.And(m => m.ActiveIngredients.Contains(filter.ActiveIngredient));

        if (!string.IsNullOrEmpty(filter.MedicineName))
            predicate = predicate.And(m => m.MedicineName.Contains(filter.MedicineName));

        //predicate = m => (
        //    (!string.IsNullOrEmpty(filter.ManufacturerId) ? m.ManufacturerId.Contains(filter.ManufacturerId) : true)
        //    && (!string.IsNullOrEmpty(filter.ActiveIngredient) ? m.ActiveIngredients.Contains(filter.ActiveIngredient) : true)
        //    && (!string.IsNullOrEmpty(filter.MedicineName) ? m.MedicineName.Contains(filter.MedicineName) : true)
        //    );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore IDE0075 // Simplify conditional expression


        Func<IQueryable<MedicineInformation>, IOrderedQueryable<MedicineInformation>>? sortBy = null;

        if (!string.IsNullOrEmpty(filter.SortBy))
        {
            sortBy = query =>
            {
                return filter.SortBy.ToLower() switch
                {
                    "name" => filter.SortDesc ? query.OrderByDescending(m => m.MedicineName) : query.OrderBy(m => m.MedicineName),
                    "activeingredient" => filter.SortDesc ? query.OrderByDescending(m => m.ActiveIngredients) : query.OrderBy(m => m.ActiveIngredients),
                    "manufacturername" => filter.SortDesc ? query.OrderByDescending(m => m.Manufacturer!.ManufacturerName) : query.OrderBy(m => m.Manufacturer!.ManufacturerName),
                    _ => query.OrderBy(m => m.Id)
                };
            };
        }

        return await AsPaginated(
            includes: x => x.Include(m => m.Manufacturer),
            filter: predicate,
            orderBy: sortBy,
            page: pageNumber,
            pageSize: pageSize);
    }
}
