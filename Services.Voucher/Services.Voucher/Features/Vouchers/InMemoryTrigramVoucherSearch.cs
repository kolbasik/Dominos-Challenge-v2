using System;
using System.Collections.Generic;
using System.Linq;
using Services.Voucher.Contracts;
using Services.Voucher.Contracts.Models;

namespace Services.Voucher.Features.Vouchers
{
  public sealed class InMemoryTrigramVoucherSearch : IVoucherSearch
  {
    private const string IndexedCharacters = "abcdefghijklmnopqrstuvwxyz01234567890";
    private readonly Dictionary<string, List<int>> _buckets = new ();
    private readonly Dictionary<int, VoucherModel> _entities = new ();

    public InMemoryTrigramVoucherSearch(List<VoucherModel> vouchers)
    {
      foreach (var voucher in vouchers)
      {
        var index = _entities.Count;
        _entities.Add(index, voucher);
        foreach (var trigram in ListTrigrams(voucher.Name))
        {
          if (!_buckets.TryGetValue(trigram, out var bucket))
          {
            _buckets.Add(trigram, bucket = new List<int>());
          }
          bucket.Add(index);
        }
      }
    }

    private static IEnumerable<string> ListTrigrams(string value)
    {
      var chars = value.ToLowerInvariant().Where(IndexedCharacters.Contains).ToArray();
      var trigrams = new HashSet<string>();
      for (var i = 0; i < chars.Length - 2; i++)
      {
        trigrams.Add("" + chars[i] + chars[i + 1] + chars[i + 2]);
      }
      return trigrams;
    }

    public IEnumerable<VoucherModel> Search(string pattern, int count)
    {
      const int capacity = 1000;
      var scores = new Dictionary<int, double>(capacity);

      var orderedBuckets = ListTrigrams(pattern)
        .Select(t => _buckets.TryGetValue(t, out var b) ? b : null)
        .Where(b => b is { Count: > 0 })
        .OrderBy(b => b.Count);

      foreach (var bucket in orderedBuckets)
      {
        foreach (var value in bucket)
        {
          scores.TryGetValue(value, out var score);
          if (scores.Count < capacity || score > 0)
          {
            scores[value] = score + 1 / Math.Sqrt(bucket.Count + 20);
          }
        }
      }

      return scores
        .OrderByDescending(p => p.Value)
        .Select(p => _entities[p.Key])
        .Take(count)
        .ToArray();
    }
  }
}
