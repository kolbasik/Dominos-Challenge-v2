using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Services.Voucher.Models;

namespace Services.Voucher.Repository
{
  public sealed class InMemoryLuceneVoucherSearch : IVoucherSearch
  {
    private const LuceneVersion Version = LuceneVersion.LUCENE_48;

    private readonly Analyzer _analyzer;
    private readonly IndexSearcher _lucene;
    private readonly Dictionary<Guid, VoucherModel> _entities;

    public InMemoryLuceneVoucherSearch(List<VoucherModel> vouchers)
    {
      _entities = vouchers.ToDictionary(it => it.Id);
      _analyzer = new StandardAnalyzer(Version);
      Directory directory = new RAMDirectory();
      using (var writer = new IndexWriter(directory, new IndexWriterConfig(Version, _analyzer)))
      {
        var storedField = new FieldType { IsStored = true };
        foreach (var voucher in vouchers)
        {
          var document = new Document
          {
            new Field("id", new BytesRef(voucher.Id.ToByteArray()), storedField),
            new Field("name", voucher.Name, TextField.TYPE_STORED)
          };
          writer.AddDocument(document);
        }

        writer.Flush(false, false);
      }

      _lucene = new IndexSearcher(DirectoryReader.Open(directory));

    }

    public IEnumerable<VoucherModel> Search(string pattern, int count)
    {
      var parser = new QueryParser(Version, "name", _analyzer);
      var query = parser.Parse(QueryParserBase.Escape(pattern));
      var hits = _lucene.Search(query, null, count).ScoreDocs;
      return hits.Select(hit => _entities[new Guid(_lucene.Doc(hit.Doc).GetBinaryValue("id").Bytes)]);
    }
  }
}
