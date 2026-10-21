using Microsoft.EntityFrameworkCore;

namespace PCBuilder.Models
{
    /// <summary>Informacje o aktualnej stronie wyników (używane przez widoki i partial _Pager).</summary>
    public class PageInfo
    {
        public const int DefaultPageSize = 30;

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = DefaultPageSize;
        public int TotalCount { get; set; }
        public int TotalPages { get; set; } = 1;

        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;

        /// <summary>Numer pierwszego wyświetlanego rekordu (0, gdy brak wyników).</summary>
        public int FirstItem => TotalCount == 0 ? 0 : (Page - 1) * PageSize + 1;

        /// <summary>Numer ostatniego wyświetlanego rekordu.</summary>
        public int LastItem => Math.Min(Page * PageSize, TotalCount);
    }

    /// <summary>Bazowa klasa filtrów – dodaje numer strony (?Page=2).</summary>
    public abstract class PagedFilter
    {
        public int Page { get; set; } = 1;
    }

    /// <summary>Bazowa klasa modeli widoku list – niesie informacje o paginacji.</summary>
    public abstract class PagedViewModel
    {
        public PageInfo Paging { get; set; } = new();
    }

    public static class QueryablePagingExtensions
    {
        /// <summary>
        /// Pobiera z bazy wyłącznie jedną stronę wyników (COUNT + OFFSET/LIMIT),
        /// zamiast ładować całą tabelę do pamięci.
        /// Zapytanie powinno być posortowane w sposób deterministyczny.
        /// </summary>
        public static async Task<(List<T> Items, PageInfo Paging)> ToPagedAsync<T>(
            this IQueryable<T> query, int page, int pageSize = PageInfo.DefaultPageSize) where T : class
        {
            var total = await query.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
            page = Math.Clamp(page, 1, totalPages);

            var items = await query
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, new PageInfo
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = totalPages
            });
        }
    }
}
