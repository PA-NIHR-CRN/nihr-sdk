using Microsoft.EntityFrameworkCore;

namespace NIHR.Infrastructure.EntityFrameworkCore.Extensions;

public static class IQueryableExtensions
{
    public static IQueryable<T> Randomise<T>(this IQueryable<T> source) => source.OrderBy(x => EF.Functions.Random());
}
