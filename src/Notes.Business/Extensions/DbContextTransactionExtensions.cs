using Microsoft.EntityFrameworkCore.Storage;

namespace Notes.Business.Extensions
{
    /// <summary>
    /// Extends <see cref="IDbContextTransaction"/>
    /// </summary>
    public static class DbContextTransactionExtensions
    {
        public static async Task HandleTryResultAsync<T>(this IDbContextTransaction txn, TryResult<T> tryResult)
            where T: class
        {
            if (tryResult.StatusCode == 200)
            {
                await txn.CommitAsync();
            } 
            else
            {
                await txn.RollbackAsync();
            }
        }
    }
}
