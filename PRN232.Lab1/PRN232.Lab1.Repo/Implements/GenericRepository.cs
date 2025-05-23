using BusinessObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using PRN232.Lab1.Repo.Interfaces;
using PRN232.Lab1.Repo.Paging;
using System.Linq.Expressions;

namespace PRN232.Lab1.Repo.Implements;

public class GenericRepository<TEntity, TEntityId> : IGenericRepository<TEntity, TEntityId>
    where TEntity : BaseEntity<TEntityId>
    where TEntityId : notnull
{
    protected readonly Lab1PharmaceuticalDbContext _context;

    public GenericRepository(Lab1PharmaceuticalDbContext context)
    {
        _context = context;
    }
    //public Task<IQueryable<T>> GetAll()
    //{// hàm này là lấy tất cả các entity của thực thể nhưng không truy vấn ngay lâpj tức => mục đích là để sử dụng cho việc truy vấn sau này
    //    return _context.Set<T>();
    //}

    public async Task<ICollection<TEntity>> GetAllAsync(
        bool noTracking = true,
        params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>();

        if (noTracking)
        {
            query = query.AsNoTracking();
        }

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.ToListAsync();
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        EntityEntry<TEntity> entityTracker = await _context.Set<TEntity>().AddAsync(entity);
        return entityTracker.Entity;
    }

    public int Count()
    {// thàng này là đếm số lượng  
        return _context.Set<TEntity>().Count();
    }

    public async Task<int> CountAsync()
    {// đếm nhưng có thểm bất đồng bộ
        return await _context.Set<TEntity>().CountAsync();
    }

    public void Delete(TEntity entity)
    {// xóa entity dựa vào entity đó => khác với thằng xóa ở dưới là xóa trực tiếp entity ko cần phải tìm => cải thiện hiệu xuất
        _context.Set<TEntity>().Remove(entity);
    }

    public void Delete(TEntityId id)
    {// cũng là xóa nhưng phải thêm 1 bước ttifm => hiệu xuât chậm 
        var entity = _context.Set<TEntity>().Find(id);
        if (entity != null)
        {
            _context.Set<TEntity>().Remove(entity);
        }
    }

    public bool Exists(TEntityId id)
    {// Kiểm tra xem là thhanwgf này có tồn tại trong db hay không dựa vào id
        return _context.Set<TEntity>().Any(e => e.Id.Equals(id));
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
    {// cũng là tra nhưng kiểm tra xem có có điều kiện => và thực thể này phải thỏa mản điêuf kiện predicate
        return await _context.Set<TEntity>().AnyAsync(predicate);
    }

    public TEntity? Find(Expression<Func<TEntity, bool>> match)
    {// hàm này là tìm kiếm entity dựa vào điều kiện match => nếu tìm thấy thì trả về entity đó còn không thì trả về null
        return _context.Set<TEntity>().SingleOrDefault(match);
    }

    public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> match)
    {// cũng là tìm nhưng bất đồng bộ
        return await _context.Set<TEntity>().SingleOrDefaultAsync(match);
    }

    public IQueryable<TEntity> GetAll()
    {// hàm này là lấy tất cả các entity của thực thể nhưng không truy vấn ngay lâpj tức => mục đích là để sử dụng cho việc truy vấn sau này
        return _context.Set<TEntity>();
    }

    public TEntity? GetById(TEntityId id)
    {// hàm này là là lấy entity theo id và có thể sửa đổi entity đó và sử dụng savechanges để lưu thay đổi
        return _context.Set<TEntity>().Find(id);
    }

    public TEntity? GetByIdAsDetached(TEntityId id)
    {// còn hàm này thì khác với hàm trên ở chỗ nó sẽ lấy và chỉ xem không có sửa đổi=> mục đích để sử dụng là để Db context ko còn dính dán gì sau khi lấy thực thể => bản chất là sao khi lấy thực thể thì có thì thằng DB Con text sẽ ăn ram khá nhiều => hiệu suât chậm  
        var entity = _context.Set<TEntity>().Find(id);
        if (entity != null)
        {
            _context.Entry(entity).State = EntityState.Detached;
        }
        return entity;
    }

    public async Task<TEntity?> GetByIdAsync(TEntityId id)
    {// hàm này khá giống với trên nhưng thêm bất đồng bộ 
        return await _context.Set<TEntity>().FindAsync(id);
    }

    public async Task<TEntity?> GetByIdAsync(TEntityId id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>> includeProperties)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>();

        if (includeProperties != null)
        {
            query = includeProperties(query);
        }

        return await query.FirstOrDefaultAsync(x => x.Id.Equals(id));
    }

    public async Task<IReadOnlyCollection<TEntity>> ToListAsReadOnly()
    {// hàm này là lấy tất cả các entity của thực thể nhưng bất đồng bộ. Thực thi ngay lập tức khi gọi phương thức.
        return await _context.Set<TEntity>().ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> ListAsync()
    {// như trên nhưng lấy dựa vào id đê lấy tất cả
        return await _context.Set<TEntity>().ToListAsync();
    }
    public async Task<TEntity?> GetByConditionAsync(
        Expression<Func<TEntity, bool>> filter,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeProperties = null)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>();

        if (includeProperties != null)
        {
            query = includeProperties(query);
        }

        return await query.FirstOrDefaultAsync(filter);
    }


    public async Task<IEnumerable<TEntity>> ListAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        bool noTracking = true
    )
    {// thằng này dùng để lọc và sắp xếp sau khi lấy 1 list entity từ db => Tham số filter: Đây là một biểu thức lambda cho phép bạn chỉ định điều kiện lọc cho truy vấn (ví dụ: e => e.IsActive == true).
     // Tham số orderBy: Đây là một hàm chức năng cho phép bạn chỉ định cách sắp xếp kết quả, ví dụ: query => query.OrderBy(e => e.Name).
        IQueryable<TEntity> query = _context.Set<TEntity>();

        if (noTracking)
        {
            query = query.AsNoTracking();
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> ListAsync(
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? includeProperties,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        bool noTracking = true
    )
    {// cũng như thằng trên nhưng thêm includeProperties để include các thực thể khác 
        IQueryable<TEntity> query = _context.Set<TEntity>();

        if (noTracking)
        {
            query = query.AsNoTracking();
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (includeProperties != null)
        {
            query = includeProperties(query);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }
        return await query.ToListAsync();
    }

    public TEntity Update(TEntity entity)
    {
        EntityEntry<TEntity> entityTracker = _context.Set<TEntity>().Update(entity);
        return entityTracker.Entity;
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await _context.AddRangeAsync(entities);
    }

    public Task DeleteRange(IEnumerable<TEntity> entities)
    {
        _context.Set<TEntity>().RemoveRange(entities);
        return Task.CompletedTask;
    }

    public async Task<PaginationResult<TEntity>> AsPaginated(
        int page,
        int pageSize,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? includes = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null)
    {
        IEnumerable<TEntity> items = await ListAsync(includeProperties: includes, filter: filter, orderBy: orderBy);

        Console.WriteLine(items.Count());

        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        return new PaginationResult<TEntity>
        {
            Content = items.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
            ItemAmount = items.Count(),
            CurrentPage = page,
            PageSize = pageSize,
        };
    }
}
