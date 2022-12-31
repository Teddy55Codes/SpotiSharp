﻿using System.Runtime.CompilerServices;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Http;

namespace SpotiSharp.Models;

// This is a slightly modified version of the default SimplePaginator (delays have been added) more to that under https://johnnycrazy.github.io/SpotifyAPI-NET/docs/pagination
public class CustomPaginator : IPaginator
{
    protected virtual Task<bool> ShouldContinue<T>(List<T> results, IPaginatable<T> page)
    {
        return Task.FromResult(true);
    }

    protected virtual Task<bool> ShouldContinue<T, TNext>(List<T> results, IPaginatable<T, TNext> page)
    {
        return Task.FromResult(true);
    }

    public async Task<IList<T>> PaginateAll<T>(IPaginatable<T> firstPage, IAPIConnector connector, CancellationToken cancel = default)
    {
        Ensure.ArgumentNotNull(firstPage, nameof(firstPage));
        Ensure.ArgumentNotNull(connector, nameof(connector));

        var page = firstPage;
        var results = new List<T>();
        if (page.Items != null)
        {
            results.AddRange(page.Items);
        }

        while (page.Next != null && await ShouldContinue(results, page).ConfigureAwait(false))
        {
            page = await connector.Get<Paging<T>>(new Uri(page.Next, UriKind.Absolute), cancel).ConfigureAwait(false);
            if (page.Items != null)
            {
                results.AddRange(page.Items);
            }
            await Task.Delay(100, cancel);
        }

        return results;
    }

    public async Task<IList<T>> PaginateAll<T, TNext>(
        IPaginatable<T, TNext> firstPage, Func<TNext, IPaginatable<T, TNext>> mapper, IAPIConnector connector,
        CancellationToken cancel = default
    )
    {
        Ensure.ArgumentNotNull(firstPage, nameof(firstPage));
        Ensure.ArgumentNotNull(mapper, nameof(mapper));
        Ensure.ArgumentNotNull(connector, nameof(connector));

        var page = firstPage;
        var results = new List<T>();
        if (page.Items != null)
        {
            results.AddRange(page.Items);
        }

        while (page.Next != null && await ShouldContinue(results, page).ConfigureAwait(false))
        {
            var next = await connector.Get<TNext>(new Uri(page.Next, UriKind.Absolute), cancel).ConfigureAwait(false);
            page = mapper(next);
            if (page.Items != null)
            {
                results.AddRange(page.Items);
            }
            await Task.Delay(100, cancel);
        }

        return results;
    }

    public async IAsyncEnumerable<T> Paginate<T>(
        IPaginatable<T> firstPage,
        IAPIConnector connector,
        [EnumeratorCancellation] CancellationToken cancel = default)
    {
        Ensure.ArgumentNotNull(firstPage, nameof(firstPage));
        Ensure.ArgumentNotNull(connector, nameof(connector));
        if (firstPage.Items == null)
        {
            throw new ArgumentException("The first page has to contain an Items list!", nameof(firstPage));
        }

        var page = firstPage;
        foreach (var item in page.Items)
        {
            yield return item;
        }

        while (page.Next != null)
        {
            page = await connector.Get<Paging<T>>(new Uri(page.Next, UriKind.Absolute), cancel).ConfigureAwait(false);
            foreach (var item in page.Items!)
            {
                yield return item;
            }
        }
    }

    public async IAsyncEnumerable<T> Paginate<T, TNext>(
        IPaginatable<T, TNext> firstPage,
        Func<TNext, IPaginatable<T, TNext>> mapper,
        IAPIConnector connector,
        [EnumeratorCancellation] CancellationToken cancel = default)
    {
        Ensure.ArgumentNotNull(firstPage, nameof(firstPage));
        Ensure.ArgumentNotNull(mapper, nameof(mapper));
        Ensure.ArgumentNotNull(connector, nameof(connector));
        if (firstPage.Items == null)
        {
            throw new ArgumentException("The first page has to contain an Items list!", nameof(firstPage));
        }

        var page = firstPage;
        foreach (var item in page.Items)
        {
            yield return item;
        }

        while (page.Next != null)
        {
            var next = await connector.Get<TNext>(new Uri(page.Next, UriKind.Absolute), cancel).ConfigureAwait(false);
            page = mapper(next);
            foreach (var item in page.Items!)
            {
                yield return item;
            }
        }
    }

    private static class Ensure
    {
        public static void ArgumentNotNull(object value, string name)
        {
            if (value != null)
            {
                return;
            }

            throw new ArgumentNullException(name);
        }
    }
}