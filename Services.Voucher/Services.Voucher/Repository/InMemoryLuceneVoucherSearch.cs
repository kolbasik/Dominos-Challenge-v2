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
  public sealed class InMemoryLuceneVoucherSearch
  {
    private readonly IndexSearcher _lucene;
    private readonly QueryParser _nameParser;

    public InMemoryLuceneVoucherSearch(List<VoucherModel> vouchers)
    {
      const LuceneVersion version = LuceneVersion.LUCENE_48;
      Analyzer analyzer = new StandardAnalyzer(version);
      Directory directory = new RAMDirectory();
      using (var writer = new IndexWriter(directory, new IndexWriterConfig(version, analyzer)))
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
      _nameParser = new QueryParser(version, "name", analyzer);
    }

    public IEnumerable<Guid> Search(string pattern, int count)
    {
      var query = _nameParser.Parse(QueryParserBase.Escape(pattern));
      var hits = _lucene.Search(query, null, count).ScoreDocs;
      return hits.Select(hit => new Guid(_lucene.Doc(hit.Doc).GetBinaryValue("id").Bytes));
    }
  }
}
