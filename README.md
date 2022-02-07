# Coding Challenge

The instruction could be found [here](./Services.Voucher/Instructions.md).

The application is served by [Heroku](https://heroku.com):

- swagger: <https://sk-dominos-pizza-vouchers-api.herokuapp.com/swagger/index.html>
- api-key: `034ede0baffa42e4a1186f88ff2b825b`

## OpenAPI Documentation

[Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) library provides the [OpenAPI Specification](https://en.wikipedia.org/wiki/OpenAPI_Specification) previously known as the [Swagger Specification](https://swagger.io/specification/) to serve a contract of API.

## Authorization

The service uses the custom implementation of the API-KEY authentication in conjunction with the Role Based authorization in order to limit the access to API. It allows to handle a different set of roles based on keys.
You can use the following API key for testing purposes: `034ede0baffa42e4a1186f88ff2b825b`

## FullText Search

As the main full text search engine is used [Lucene.NET](https://lucenenet.apache.org/docs/4.8.0-beta00015/). Also as an alternative it was implemented a custom engine based on Trigrams. The advantage of the [Trigrams engine](./Services.Voucher/Services.Voucher/Repository/InMemoryTrigramVoucherSearch.cs) is simplicity when the disadvantage is  twice the memory and CPU consumption compared to [Lucene.NET engine](./Services.Voucher/Services.Voucher/Repository/InMemoryLuceneVoucherSearch.cs).

## Continuous Integration

[Github Actions](https://github.com/features/actions) was chosen to build the code and to run all verifications.

## Continuous Delivery

[Docker](https://www.docker.com/) packs the service a container.

```bash
docker build -t dominos-pizza-vouchers-api -f Dockerfile .
docker run -it -p 5000:80 dominos-pizza-vouchers-api
```

[Heroku](http://heroku.com/) serves the docker container.

```bash
heroku container:login
heroku container:push web -a dominos-pizza-vouchers-api
heroku container:release web -a dominos-pizza-vouchers-api
```

Service: <https://sk-dominos-pizza-vouchers-api.herokuapp.com/swagger/index.html>

## Testing

### Performance

[BenchmarkDotNet](https://benchmarkdotnet.org/articles/overview.html) checks the performance of in-memory implementations: VoucherRepository, LuceneVouchersSearch and TrigramVoucherSearch. In order to get the local results you can run the following tests [here](C:\albelli\self\Dominos-Challenge-v2\Services.Voucher\Services.Voucher.Test.Performance\Benchmarks\Program.cs).

#### Vouchers Repository

It is very fast and does not consume memory.

|                          Method |     N |      Mean |    Error |   StdDev | Ratio |  Gen 0 | Allocated |
|-------------------------------- |------ |----------:|---------:|---------:|------:|-------:|----------:|
|                     GetVouchers |   100 |  60.68 ns | 1.274 ns | 1.743 ns |  1.00 | 0.0153 |     128 B |
|                                 |       |           |          |          |       |        |           |
|                  GetVoucherById |   100 |  10.22 ns | 0.053 ns | 0.044 ns |  1.00 |      - |         - |
|                                 |       |           |          |          |       |        |           |
|               GetVouchersByName |   100 | 112.12 ns | 2.075 ns | 1.941 ns |  1.00 | 0.0191 |     160 B |
|                                 |       |           |          |          |       |        |           |
| GetCheapestVoucherByProductCode |   100 |  16.17 ns | 0.091 ns | 0.085 ns |  1.00 |      - |         - |
|                                 |       |           |          |          |       |        |           |
|                     GetVouchers |  1000 | 195.88 ns | 3.984 ns | 6.202 ns |  1.00 | 0.0420 |     352 B |
|                                 |       |           |          |          |       |        |           |
|                  GetVoucherById |  1000 |  10.19 ns | 0.265 ns | 0.261 ns |  1.00 |      - |         - |
|                                 |       |           |          |          |       |        |           |
|               GetVouchersByName |  1000 | 115.20 ns | 2.344 ns | 2.407 ns |  1.00 | 0.0191 |     160 B |
|                                 |       |           |          |          |       |        |           |
| GetCheapestVoucherByProductCode |  1000 |  16.59 ns | 0.398 ns | 0.517 ns |  1.00 |      - |         - |
|                                 |       |           |          |          |       |        |           |
|                     GetVouchers | 10000 | 185.55 ns | 3.757 ns | 4.885 ns |  1.00 | 0.0420 |     352 B |
|                                 |       |           |          |          |       |        |           |
|                  GetVoucherById | 10000 |  10.38 ns | 0.108 ns | 0.096 ns |  1.00 |      - |         - |
|                                 |       |           |          |          |       |        |           |
|               GetVouchersByName | 10000 | 107.44 ns | 1.465 ns | 1.299 ns |  1.00 | 0.0191 |     160 B |
|                                 |       |           |          |          |       |        |           |
| GetCheapestVoucherByProductCode | 10000 |  16.97 ns | 0.330 ns | 0.258 ns |  1.00 |      - |         - |

#### Lucene Vouchers Search

It is very fast and consumes little memory.

| Method |      N |     Mean |    Error |   StdDev | Ratio |   Gen 0 |  Gen 1 | Allocated |
|------- |------- |---------:|---------:|---------:|------:|--------:|-------:|----------:|
| Search |   1000 | 73.57 us | 1.434 us | 1.962 us |  1.00 | 19.7754 | 4.8828 |    162 KB |
|        |        |          |          |          |       |         |        |           |
| Search |  10000 | 69.75 us | 0.876 us | 0.820 us |  1.00 | 19.8975 | 3.6621 |    163 KB |
|        |        |          |          |          |       |         |        |           |
| Search | 100000 | 92.98 us | 1.803 us | 1.771 us |  1.00 | 35.1563 | 7.3242 |    288 KB |

#### Trigrams Vouchers Search

It is very fast and consumes less memory per operation comparing to Lucene, but twice the overall memory required.

| Method |      N |     Mean |     Error |    StdDev | Ratio |  Gen 0 |  Gen 1 | Allocated |
|------- |------- |---------:|----------:|----------:|------:|-------:|-------:|----------:|
| Search |   1000 | 3.566 us | 0.0253 us | 0.0211 us |  1.00 | 4.1809 | 0.4158 |     34 KB |
|        |        |          |           |           |       |        |        |           |
| Search |  10000 | 3.549 us | 0.0116 us | 0.0102 us |  1.00 | 4.1809 | 0.4158 |     34 KB |
|        |        |          |           |           |       |        |        |           |
| Search | 100000 | 3.563 us | 0.0200 us | 0.0177 us |  1.00 | 4.1809 | 0.4158 |     34 KB |

### High Load

The [k6](https://k6.io/docs/getting-started/running-k6) is very powerfull to run the load tests. There are 2 tests to check throughput and latency:

To measure getting all vouchers.

Script:

```bash
k6 run -u 15 -d 15s ./k6/vouchers/search.js
```

Result:

```plain
http_req_waiting...............: avg=457.51µs min=0s med=506.7µs max=11.35ms p(90)=1ms p(95)=1ms
http_reqs......................: 355528  23697.385807/s
```

To measure searching vouchers.

Script:

```bash
k6 run -u 15 -d 15s ./k6/vouchers/search.js
```

Result using Lucene:

```plain
http_req_waiting...............: avg=9.42ms  min=513.29µs med=9.47ms max=98.39ms  p(90)=13.67ms p(95)=14.75ms
http_reqs......................: 23286   1551.494041/s
```

Result using Trigrams:

```plain
http_req_waiting...............: avg=13.37ms min=0s med=13.16ms max=107.11ms p(90)=20.5ms  p(95)=22.91ms
http_reqs......................: 16508   1099.754487/s
```

According to the results, Lucene 1.5x handles search queries better than Trigrams.
