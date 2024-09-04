using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.IO.Compression;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Globalization;
using System.Text.Json.Serialization;
using Redis.OM;
using Redis.OM.Searching;
using Redis.OM.Modeling;
using System.Transactions;
using Redis.OM.Aggregation;

namespace ReadExcelSheet
{
    public class GetSonuclar
    {
        private readonly IRedisCollection<NumaraDTO> _NumaraDTO;
        private readonly RedisAggregationSet<NumaraDTO> _NumaraAggregation;
        //private readonly RedisConnectionProvider _provider;


        public GetSonuclar()
        {
            
            var _provider = new RedisConnectionProvider("redis://default:4P9LZ96T7hhV0oKZCzjYABgDSKbvlmOJ@redis-11361.c84.us-east-1-2.ec2.redns.redis-cloud.com:11361");
            _NumaraDTO = _provider.RedisCollection<NumaraDTO>();

            var res = _provider.Connection.DropIndex(typeof(NumaraDTO));
            res = _provider.Connection.CreateIndex(typeof(NumaraDTO));

            _NumaraAggregation = _provider.AggregationSet<NumaraDTO>();

        }




        private async IAsyncEnumerable<NumaraDTO> GetNumbers(string LotoName, DateTime startDate)
        {
            var _lastdate = DateTime.Now;
            var _curdadate = startDate;

            while (_curdadate <= _lastdate)
            {
                string dataStr = _curdadate.ToString("MM.yyyy");
                _curdadate = _curdadate.AddMonths(1);
                await foreach (var item in GetMonthNumber(LotoName, dataStr))
                {
                    yield return item;
                }

            }

        }

        private async IAsyncEnumerable<NumaraDTO> GetMonthNumber(string lotoName, string datestr)
        {
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                using (var client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri("https://www.millipiyangoonline.com");
                    /*
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36");
                    client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("tr-TR,tr;q=0.9,en-US;q=0.8,en;q=0.7");
                    */
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    client.DefaultRequestHeaders.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                    client.DefaultRequestHeaders.Add("accept-language", "tr-TR,tr;q=0.9,en-US;q=0.8,en;q=0.7");
                    client.DefaultRequestHeaders.Add("cache-control", "no-cache");
                    client.DefaultRequestHeaders.Add("pragma", "no-cache");
                    client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Not)A;Brand\";v=\"99\", \"Google Chrome\";v=\"127\", \"Chromium\";v=\"127\"");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                    client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
                    client.DefaultRequestHeaders.Add("sec-fetch-dest", "document");
                    client.DefaultRequestHeaders.Add("sec-fetch-mode", "navigate");
                    client.DefaultRequestHeaders.Add("sec-fetch-site", "none");
                    client.DefaultRequestHeaders.Add("sec-fetch-user", "?1");
                    client.DefaultRequestHeaders.Add("upgrade-insecure-requests", "1");
                    client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/127.0.0.0 Safari/537.36");


                    //client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("YourAppName", "1.0"));



                    List<NumaraDTO>? resultLst = new();
                    HttpResponseMessage response;

                    //var endpoint = $"sisalsans/result.sayisaloto.{datestr}.json?cache=false";
                    try
                    {
                        var endpoint = $"sisalsans/result.{lotoName}.{datestr}.json?cache=false";
                        response = await client.GetAsync(endpoint);
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                    

                    if (response.IsSuccessStatusCode)
                    {
                        var compressedData = await response.Content.ReadAsByteArrayAsync();

                        using (var decompressedStream = new MemoryStream())
                        using (var decompressionStream = new GZipStream(new MemoryStream(compressedData), CompressionMode.Decompress))
                        {
                            decompressionStream.CopyTo(decompressedStream);
                            var result = Encoding.UTF8.GetString(decompressedStream.ToArray());

                            var options = new JsonSerializerOptions
                            {
                                Converters = { new CustomDateTimeConverter(), new CustomDateOnlyConverter(), new CustomTimeOnlyConverter() }
                            };

                            resultLst = JsonSerializer.Deserialize<List<NumaraDTO>>(result, options);
                            if (resultLst != null)
                                foreach (var item in resultLst)
                                {
                                    yield return item;
                                }

                        }

                    }
                    else
                    {
                        // Hata durumunu ele al
                    }

                }
            }

        }

        public async Task GetCSL()
        {

            Console.WriteLine($"---CSL Versiyon2");

            var NumaraDtoLst = await GetCekilisSonucu("SAYISAL", "sayisaloto");

            var SonCekilis = NumaraDtoLst.Where(x => x.drawNumbers.Count() > 0).OrderByDescending(x => x.drawDateTime).Take(1).FirstOrDefault();
            var HaricNumaralar = SonCekilis?.drawNumbers.Concat(SonCekilis.numberJolly).ToList() ?? new List<Int32>();

            //var aaaV1Redis = getAdatV1Redis(result, HaricNumaralar, 90, 6);


            var aaaV4 = getAdatV4(NumaraDtoLst, HaricNumaralar, 90, 6, true);
            Console.WriteLine($"   --- {aaaV4.OrderByDescending(x => x.cikmaSayisi).FirstOrDefault()?.number ?? 0} ilişkili: ({string.Join(",", aaaV4.Select(x => x.number).ToList())})");

            var aaaV1 = getAdatV1(NumaraDtoLst, HaricNumaralar, 90, 6,true);
            var aaaV2 = getAdatV2(NumaraDtoLst, HaricNumaralar, 90, 6,true);
            var aaaV3 = getAdatV3(NumaraDtoLst, HaricNumaralar, 90, 6, true);
            
            
            Console.WriteLine($"   ---En Büyük Adat: ({string.Join(",", aaaV1.Select(x => x.number).ToList())}), ({string.Join(",", aaaV2.Select(x => x.number).ToList())}), ({string.Join(",", aaaV3.Select(x => x.number).ToList())})");

            HaricNumaralar = HaricNumaralar.Concat(aaaV1.Select(x => x.number)).ToList();
            aaaV1 = getAdatV1(NumaraDtoLst, HaricNumaralar, 90, 6, true);
            aaaV2 = getAdatV2(NumaraDtoLst, HaricNumaralar, 90, 6, true);
            aaaV3 = getAdatV3(NumaraDtoLst, HaricNumaralar, 90, 6, true);
            Console.WriteLine($"   ---En Büyük Adat: ({string.Join(",", aaaV1.Select(x => x.number).ToList())}), ({string.Join(",", aaaV2.Select(x => x.number).ToList())}), ({string.Join(",", aaaV3.Select(x => x.number).ToList())})");

            HaricNumaralar = HaricNumaralar.Concat(aaaV1.Select(x => x.number)).ToList();
            aaaV1 = getAdatV1(NumaraDtoLst, HaricNumaralar, 90, 6, true);
            aaaV2 = getAdatV2(NumaraDtoLst, HaricNumaralar, 90, 6, true);
            aaaV3 = getAdatV3(NumaraDtoLst, HaricNumaralar, 90, 6, true);
            Console.WriteLine($"   ---En Büyük Adat: ({string.Join(",", aaaV1.Select(x => x.number).ToList())}), ({string.Join(",", aaaV2.Select(x => x.number).ToList())}), ({string.Join(",", aaaV3.Select(x => x.number).ToList())})");

            //var aaaV4 = getAdatV4(NumaraDtoLst, HaricNumaralar, 90, 6, true);
            //Console.WriteLine($"   --- {aaaV4.OrderByDescending(x=> x.cikmaSayisi).FirstOrDefault()?.number ?? 0} ilişkili: ({string.Join(",", aaaV4.Select(x => x.number).ToList())})");

            Console.WriteLine($"--------------------------------------------------------------------------");
        }

        public async Task GetSL()
        {


            Console.WriteLine($"---SL Versiyon2");

            var NumaraDtoLst = await GetCekilisSonucu("SUPERLOTO", "superloto");

            var SonCekilis = NumaraDtoLst.Where(x => x.drawNumbers.Count() > 0).OrderByDescending(x => x.drawDateTime).Take(1).FirstOrDefault();
            var HaricNumaralar = SonCekilis?.drawNumbers.Concat(SonCekilis.numberJolly).ToList() ?? new List<Int32>();

            var aaaV4 = getAdatV4(NumaraDtoLst, HaricNumaralar, 60, 6, true);
            Console.WriteLine($"   --- {aaaV4.OrderByDescending(x => x.cikmaSayisi).FirstOrDefault()?.number ?? 0} ilişkili: ({string.Join(",", aaaV4.Select(x => x.number).ToList())})");

            var aaaV1 = getAdatV1(NumaraDtoLst, HaricNumaralar, 60, 6, false);
            var aaaV2 = getAdatV2(NumaraDtoLst, HaricNumaralar, 60, 6, false);
            var aaaV3 = getAdatV3(NumaraDtoLst, HaricNumaralar, 60, 6, false);
            Console.WriteLine($"   ---En Büyük Adat: ({string.Join(",", aaaV1.Select(x => x.number).ToList())}), ({string.Join(",", aaaV2.Select(x => x.number).ToList())}), ({string.Join(",", aaaV3.Select(x => x.number).ToList())})");

            HaricNumaralar = HaricNumaralar.Concat(aaaV1.Select(x => x.number)).ToList();
            aaaV1 = getAdatV1(NumaraDtoLst, HaricNumaralar, 60, 6, false);
            aaaV2 = getAdatV2(NumaraDtoLst, HaricNumaralar, 60, 6, false);
            aaaV3 = getAdatV3(NumaraDtoLst, HaricNumaralar, 60, 6, false);
            Console.WriteLine($"   ---En Büyük Adat: ({string.Join(",", aaaV1.Select(x => x.number).ToList())}), ({string.Join(",", aaaV2.Select(x => x.number).ToList())}), ({string.Join(",", aaaV3.Select(x => x.number).ToList())})");

            HaricNumaralar = HaricNumaralar.Concat(aaaV1.Select(x => x.number)).ToList();
            aaaV1 = getAdatV1(NumaraDtoLst, HaricNumaralar, 90, 6, false);
            aaaV2 = getAdatV2(NumaraDtoLst, HaricNumaralar, 90, 6, false);
            aaaV3 = getAdatV3(NumaraDtoLst, HaricNumaralar, 90, 6, false);
            Console.WriteLine($"   ---En Büyük Adat: ({string.Join(",", aaaV1.Select(x => x.number).ToList())}), ({string.Join(",", aaaV2.Select(x => x.number).ToList())}), ({string.Join(",", aaaV3.Select(x => x.number).ToList())})");

            Console.WriteLine($"--------------------------------------------------------------------------");
        }

        private async Task<List<NumaraDTO>> GetCekilisSonucu(string lotteryname, string sayfa)
        {

            List<NumaraDTO> NumaraDtoLst = new();

            //var aaa = _NumaraAggregation.GroupBy(x => x.RecordShell.lotteryName).Sum(x => x.RecordShell.drawnNr ).Max(x => x.RecordShell.drawnNr).Sum(x=> x.RecordShell.drawYear).ToList();
            //var aaa = _NumaraAggregation.GroupBy(x => x.RecordShell.lotteryName).Sum(x => x.RecordShell.drawYear).ToList();
            //var aaa = _NumaraAggregation.GroupBy(x => x.RecordShell.lotteryName).ToList();

            //var aaa = _NumaraDTO.ToList();

            NumaraDtoLst = _NumaraDTO.Where(x => x.lotteryName == lotteryname).ToList();

            TimeSpan ts = new TimeSpan();
            if (NumaraDtoLst.Count == 0)
            {
                await foreach (var numaralar in GetNumbers(sayfa, new DateTime(2020, 08, 01)))
                {
                    NumaraDtoLst.Add(numaralar);
                    ts = new DateTime(numaralar.nextDrawDate, numaralar.drawTime) - DateTime.Now;
                    //var aaa = await _NumaraDTO.InsertAsync(numaralar, ts);
                }
                try
                {
                    var result = await _NumaraDTO.InsertAsync(NumaraDtoLst, ts);
                }
                catch (Exception)
                {

                    Console.WriteLine("Cache işlemi yapılamadı");
                }
                
            }            
            
            return NumaraDtoLst;
        }
        public async Task GetST()
        {


            Console.WriteLine($"---ST Versiyon2");



            var NumaraDtoLst = await GetCekilisSonucu("SANSTOPU", "sanstopu");


            var SonCekilis = NumaraDtoLst.Where(x => x.drawNumbers.Count() > 0).OrderByDescending(x => x.drawDateTime).Take(1).FirstOrDefault();
            var HaricNumaralar = SonCekilis?.drawNumbers.ToList() ?? new List<Int32>();

            var aaaV1 = getAdatV1(NumaraDtoLst, HaricNumaralar, 34, 5,false);
            var aaaV2 = getAdatV2(NumaraDtoLst, HaricNumaralar, 34, 5,false);
            var aaaV3 = getAdatV3(NumaraDtoLst, HaricNumaralar, 34, 5,false);
            Console.WriteLine($"   ---En Büyük Adat: ({string.Join(",", aaaV1.Select(x => x.number).ToList())}), ({string.Join(",", aaaV2.Select(x => x.number).ToList())}), ({string.Join(",", aaaV3.Select(x => x.number).ToList())})");

            HaricNumaralar = HaricNumaralar.Concat(aaaV1.Select(x => x.number)).ToList();
            aaaV1 = getAdatV1(NumaraDtoLst, HaricNumaralar, 34, 5,false);
            aaaV2 = getAdatV2(NumaraDtoLst, HaricNumaralar, 34, 5, false);
            aaaV3 = getAdatV3(NumaraDtoLst, HaricNumaralar, 34, 5, false);
            Console.WriteLine($"   ---En Büyük Adat: ({string.Join(",", aaaV1.Select(x => x.number).ToList())}), ({string.Join(",", aaaV2.Select(x => x.number).ToList())}), ({string.Join(",", aaaV3.Select(x => x.number).ToList())})");

            HaricNumaralar = HaricNumaralar.Concat(aaaV1.Select(x => x.number)).ToList();
            aaaV1 = getAdatV1(NumaraDtoLst, HaricNumaralar, 34, 5, false);
            aaaV2 = getAdatV2(NumaraDtoLst, HaricNumaralar, 34, 5, false);
            aaaV3 = getAdatV3(NumaraDtoLst, HaricNumaralar, 34, 5, false);
            Console.WriteLine($"   ---En Büyük Adat: ({string.Join(",", aaaV1.Select(x => x.number).ToList())}), ({string.Join(",", aaaV2.Select(x => x.number).ToList())}), ({string.Join(",", aaaV3.Select(x => x.number).ToList())})");


            SonCekilis = NumaraDtoLst.Where(x => x.numberJolly.Count() > 0).OrderByDescending(x => x.drawDateTime).Take(1).FirstOrDefault();
            HaricNumaralar = SonCekilis?.numberJolly.ToList() ?? new List<Int32>();

            aaaV1 = getJolyV1(NumaraDtoLst, HaricNumaralar, 14, 6);
            Console.WriteLine($"   ---Süper Numara En Büyük Adat: ({string.Join(",", aaaV1.Select(x => x.number).ToList())})");

            Console.WriteLine($"--------------------------------------------------------------------------");
        }

        //private List<AdatNumara> getAdatV1Redis(IRedisCollection<NumaraDTO> ListNumara, List<int> HaricTut, int totalCount, int take)
        //{
        //    var resultLsttst = _NumaraDTO.Where(x => x.drawNumbers.Contains(1)).ToList();

        //    foreach (var item in resultLsttst)
        //    {
        //        var aaa = item;
        //    }

        //    DateTime CurDate = DateTime.Now;
        //    List<AdatNumara> ResulList = new List<AdatNumara>();
        //    for (int i = 1; i <= totalCount; i++)
        //    {

        //        var resultLst = ListNumara.Where(x => x.drawNumbers.Contains(i) || x.numberJolly.Contains(i)).Select(x => new { x.drawDateTime })
        //            .ToList();
        //        int TopAdat = 0;
        //        foreach (var item in resultLst)
        //        {
        //            var adatday = CurDate.Subtract(item.drawDateTime).Days;
        //            TopAdat += adatday;
        //        }

        //        ResulList.Add(new AdatNumara() { number = i, adat = resultLst.Count() > 0 ? TopAdat / resultLst.Count() : 0 });

        //    }
        //    return ResulList.Where(x => !HaricTut.Contains(x.number)).OrderByDescending(x => x.adat).Take(take).OrderBy(x => x.number).ToList();



        //}


        private List<AdatNumara> getJolyV1(List<NumaraDTO> ListNumara, List<Int32> HaricTut, int totalCount, int take)
        {
            DateTime CurDate = DateTime.Now;
            List<AdatNumara> ResulList = new List<AdatNumara>();
            for (int i = 1; i <= totalCount; i++)
            {
                List<DateTime> resultLst;
                
                    resultLst = ListNumara.Where(x => x.numberJolly.Contains(i)).Select(x => x.drawDateTime)
                        .ToList();
                

                int TopAdat = 0;
                foreach (var item in resultLst)
                {
                    var adatday = CurDate.Subtract(item).Days;
                    TopAdat += adatday;
                }

                ResulList.Add(new AdatNumara() { number = i, adat = resultLst.Count() > 0 ? TopAdat / resultLst.Count() : 0 });

            }
            return ResulList.Where(x => !HaricTut.Contains(x.number)).OrderByDescending(x => x.adat).Take(take).ToList();


        }
        private List<AdatNumara> getAdatV1(List<NumaraDTO> ListNumara, List<Int32> HaricTut, int totalCount, int take, bool jolyDahil)
        {
            DateTime CurDate = DateTime.Now;
            List<AdatNumara> ResulList = new List<AdatNumara>();

            for (int i = 1; i <= totalCount; i++)
            {
                List<NumaraDTO> resultLst;
                if (jolyDahil)
                {
                    //resultLst = ListNumara.Where(x => x.drawNumbers.Contains(i) || x.numberJolly.Contains(i)).Select(x => x.drawDateTime)
                    resultLst = ListNumara.Where(x => x.drawNumbers.Contains(i) || x.numberJolly.Contains(i)).ToList();
                        
                }
                else
                {
                    resultLst = ListNumara.Where(x => x.drawNumbers.Contains(i)).ToList();
                }

                int TopAdat = 0;
                foreach (var item in resultLst)
                {
                    var adatday = CurDate.Subtract(item.drawDateTime).Days;
                    TopAdat += adatday;
                }
                
                ResulList.Add(new AdatNumara() { number = i, adat =  Math.Round(resultLst.Count() > 0 ? (decimal)TopAdat / (decimal)resultLst.Count() : 0, 5),cikmaSayisi = resultLst.Count()});

            }

            var aaa = ResulList.Where(x => !HaricTut.Contains(x.number)).OrderByDescending(x => x.adat).ToList();
            var bbb = ResulList.OrderBy(x => x.cikmaSayisi).ToList();


            return ResulList.Where(x => !HaricTut.Contains(x.number)).OrderByDescending(x => x.adat).Take(take).OrderBy(x => x.number).ToList();



        }
        private List<AdatNumara> getAdatV2(List<NumaraDTO> ListNumara, List<Int32> HaricTut, int totalCount, int take, bool jolyDahil)
        {
            var Resut = getAdatV1(ListNumara, HaricTut, totalCount, take, jolyDahil);
            List<int> TmpHaricTut = new List<int>();

            for (int i = 1; i < Resut.Count; i++)
            {

                if (Resut[i].number - Resut[i - 1].number == 1)
                {

                    TmpHaricTut.Add(Resut[i].adat < Resut[i - 1].adat ? Resut[i].number : Resut[i - 1].number);
                    Resut = getAdatV1(ListNumara, HaricTut.Concat(TmpHaricTut).ToList(), totalCount, take, jolyDahil);
                    i = 0;
                    continue;
                }

                var TmpCount = TmpHaricTut.Count;
                for (int y = i - 1; y >= 0; y--)
                {
                    if ((Resut[i].number - Resut[y].number) % 10 == 0)
                    {
                        TmpHaricTut.Add(Resut[i].adat < Resut[y].adat ? Resut[i].number : Resut[y].number);

                        break;
                    }
                }
                if (TmpCount != TmpHaricTut.Count)
                {
                    Resut = getAdatV1(ListNumara, HaricTut.Concat(TmpHaricTut).ToList(), totalCount, take, jolyDahil);
                    i = 0;
                    continue;
                }


            }


            return Resut;
        }

        private List<AdatNumara> getAdatV3(List<NumaraDTO> ListNumara, List<int> HaricTut, int totalCount, int take, bool jolyDahil)
        {
            var Resut = getAdatV1(ListNumara, HaricTut, totalCount, take, jolyDahil);
            List<int> TmpHaricTut = new List<int>();

            for (int i = 0; i < Resut.Count - 1; i++)
            {
                TmpHaricTut.Add(Resut[i].number);
                TmpHaricTut.Add(Resut[i].number + 1);

                for (int j = Resut[i].number + 10; j <= 90; j = j + 10)
                {
                    TmpHaricTut.Add(j);
                }
                Resut.RemoveRange(i + 1, (take - 1) - i);
                Resut = Resut.Concat(getAdatV1(ListNumara, HaricTut.Concat(TmpHaricTut).ToList(), totalCount, (take - 1) - i, jolyDahil)).ToList();

            }


            return Resut.OrderBy(x => x.number).ToList(); ;
        }

        private List<AdatNumara> getAdatV4(List<NumaraDTO> ListNumara, List<Int32> HaricTut, int totalCount, int take, bool jolyDahil)
        {
            var adatnumbers = getAdatV1(ListNumara, new List<Int32>(), totalCount, totalCount, jolyDahil)
                .OrderByDescending(x => x.adat).ToList();

            var FirstNumber = adatnumbers.Where(x => !HaricTut.Contains(x.number))
                .OrderByDescending(x => x.adat)
                .FirstOrDefault()?.number ?? 0 ;

            List<NumaraDTO> resultLst;
            if (jolyDahil)
            {                
                resultLst = ListNumara
                    .Where(x => x.drawNumbers.Contains(FirstNumber) || x.numberJolly.Contains(FirstNumber))                   
                    .ToList();
            }
            else
            {
                resultLst = ListNumara.Where(x => x.drawNumbers.Contains(FirstNumber)).ToList();
            }
            List<AdatNumara> ResulList = new List<AdatNumara>();

            foreach (var item in resultLst)
            {
                foreach (var item2 in item.drawNumbers) {
                    var nmr = adatnumbers.Where(x => x.number == item2).FirstOrDefault();

                    var adatnmr = ResulList.Where(x=> x.number == item2).FirstOrDefault();
                    if (adatnmr == null)
                    {
                        ResulList.Add(new AdatNumara()
                        {
                            number = nmr.number,
                            adat = nmr.adat,
                            cikmaSayisi = 1
                        });
                    }
                    else
                    {
                        adatnmr.cikmaSayisi += 1;
                    }
                }

                foreach (var item2 in item.numberJolly)
                {
                    var nmr = adatnumbers.Where(x => x.number == item2).FirstOrDefault();

                    var adatnmr = ResulList.Where(x => x.number == item2).FirstOrDefault();
                    if (adatnmr == null)
                    {
                        ResulList.Add(new AdatNumara()
                        {
                            number = nmr.number,
                            adat = nmr.adat,
                            cikmaSayisi = 1
                        });
                    }
                    else
                    {
                        adatnmr.cikmaSayisi += 1;
                    }
                }
            }

            //var aaa = ResulList.Where(x => !HaricTut.Contains(x.number)).OrderByDescending(x => x.cikmaSayisi).ThenByDescending(x=> x.adat).ToList();
            //var bbb = ResulList.OrderBy(x => x.cikmaSayisi).ToList();
            return ResulList.Where(x => !HaricTut.Contains(x.number)).OrderByDescending(x => x.cikmaSayisi).ThenByDescending(x => x.adat).Take(take).OrderBy(x => x.number).ToList();




            
        }
    }

    public class AdatNumara
    {
        public int number;
        public decimal adat;
        public int cikmaSayisi;

    }

    [Document(StorageType = StorageType.Json, Prefixes = new[] { "Super01" })]

    public class NumaraDTO
    {

        [RedisIdField] public Guid RedisId { get; } = Guid.NewGuid();
        public string id { get; set; }
        [Indexed(Aggregatable = true)] public string lotteryName { get; set; }
        [Indexed(Aggregatable = true)] public int drawnNr { get; set; }
        [Indexed(Aggregatable = true)] public int drawYear { get; set; }
        public DateOnly drawDate { get; set; }
        public TimeOnly drawTime { get; set; }
        [Indexed(Aggregatable = true)] public List<Int32> drawNumbers { get; set; } = new List<Int32>();
        public List<int> drawNumbersOnNumaraL1 { get; set; }
        public List<int> drawNumbersOnNumaraL2 { get; set; }
        public List<int> drawNumbersOnNumaraL3 { get; set; }
        [Indexed] public List<Int32> numberJolly { get; set; } = new List<Int32>();
        public List<int> superstar { get; set; }
        public string status { get; set; }
        public DateTime drawDateTime { get; set; }
        public string currentDate { get; set; }
        public string jackpot { get; set; }
        public DateOnly nextDrawDate { get; set; }
        public long drawTimestamp { get; set; }
    }
    public class CustomDateTimeConverter : JsonConverter<DateTime>
    {
        private const string DateTimeFormat1 = "MM/dd/yyyy";
        private const string DateTimeFormat2 = "yyyy-MM-dd HH:mm:ss.f";
        private const string DateTimeFormat3 = "yyyy-mm-ddTHH:mm:ss.ffffff";
                                                

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (DateTime.TryParseExact(reader.GetString(), DateTimeFormat1, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
                {
                    return dateTime;
                }
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                if (DateTime.TryParseExact(reader.GetString(), DateTimeFormat2, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
                {
                    return dateTime;
                }
            }
            if (reader.TokenType == JsonTokenType.String)
            {
                if (DateTime.TryParseExact(reader.GetString(), DateTimeFormat3, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
                {
                    return dateTime;
                }
            }

            throw new JsonException($"Unable to convert value to {typeToConvert}");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateTimeFormat1));
        }
    }
    public class CustomDateOnlyConverter : JsonConverter<DateOnly>
    {
        private const string DateTimeFormat1 = "MM/dd/yyyy";


        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (DateOnly.TryParseExact(reader.GetString(), DateTimeFormat1, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOnly))
                {
                    return dateOnly;
                }
                                 
            }

            var dateOnlyToday = DateOnly.FromDateTime(DateTime.Now);
            return dateOnlyToday;

            throw new JsonException($"Unable to convert value to {typeToConvert}");
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateTimeFormat1));
        }
    }

    public class CustomTimeOnlyConverter : JsonConverter<TimeOnly>
    {
        private const string TimeFormat1 = "HH:mm";


        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (TimeOnly.TryParseExact(reader.GetString(), TimeFormat1, CultureInfo.InvariantCulture, DateTimeStyles.None, out var timeOnly))
                {
                    return timeOnly;
                }
            }


            throw new JsonException($"Unable to convert value to {typeToConvert}");
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(TimeFormat1));
        }
    }
}
