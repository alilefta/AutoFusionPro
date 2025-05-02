namespace AutoFusionPro.Core.Models
{
    /// <summary>
    /// Represents the result of a paginated query.
    /// </summary>
    /// <typeparam name="T">The type of the items in the result set.</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// The items for the current page.
        /// </summary>
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

        /// <summary>
        /// The current page number (1-based).
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// The number of items requested per page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The total number of items across all pages that match the query criteria.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Calculates the total number of pages based on PageSize and TotalCount.
        /// </summary>
        public int TotalPages => (PageSize > 0) ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

        /// <summary>
        /// Indicates if there is a previous page.
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// Indicates if there is a next page.
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// Creates an empty PagedResult.
        /// </summary>
        public PagedResult() { }

        /// <summary>
        /// Creates a PagedResult instance.
        /// </summary>
        public PagedResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items ?? Enumerable.Empty<T>();
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
