using ExcelDataReader;
using System.Data;
using System.Linq;

namespace ReadExcelSheet
{
    public static class ExcelReadSheet
    {

        enum SheetName
        {
            CSL_Sonuclari = 0,
            SL_Sonuclari = 1,
            ST_Sonuclari = 2
        }

        enum CSLKolonlari
        {
            Hafta = 0,
            Tarih = 1,
            Kolon1 = 2,
            Kolon2 = 3,
            Kolon3 = 4,
            Kolon4 = 5,
            Kolon5 = 6,
            Kolon6 = 7,
            Joker = 8,
            SuperStar = 9
        }

        enum STKolonlari
        {
            Hafta = 0,
            Tarih = 1,
            Kolon1 = 2,
            Kolon2 = 3,
            Kolon3 = 4,
            Kolon4 = 5,
            Kolon5 = 6,
            SansTopu = 7

        }

        enum SLKolonlari
        {
            Hafta = 0,
            Tarih = 1,
            Kolon1 = 2,
            Kolon2 = 3,
            Kolon3 = 4,
            Kolon4 = 5,
            Kolon5 = 6,
            Kolon6 = 7,
        }
        public static void Read02(string xlsxPath)
        {

            List<CSLCekilisSonucu> CSLCekilisSonuclariListesi;
            List<SLCekilisSonucu> SLCekilisSonuclariListesi;
            List<STCekilisSonucu> STCekilisSonuclariListesi;
            using (DataSet ds = GetExcelToDataSet(xlsxPath))
            {
                CSLCekilisSonuclariListesi = GetCSLCekilisSonuclariListesi(ds.Tables[(int)SheetName.CSL_Sonuclari]);
                SLCekilisSonuclariListesi = getSLCekilisSonuclariListesi(ds.Tables[(int)SheetName.SL_Sonuclari]);
                STCekilisSonuclariListesi = GetSTCekilisSonuclariListesi(ds.Tables[(int)SheetName.ST_Sonuclari]);
            }


            var CSLSonNumaralar = getCSLCikanNumaraListesi(CSLCekilisSonuclariListesi.OrderByDescending(x => x.Tarih).Take(1).ToList());
            var SLSonNumaralar = getSLCikanNumaraListesi(SLCekilisSonuclariListesi.OrderByDescending(x => x.Tarih).Take(1).ToList());
            var STSonNumaralar = getSTCikanNumaraListesi(STCekilisSonuclariListesi.OrderByDescending(x => x.Tarih).Take(1).ToList());

            List<CikanNumara> CSLCikanNumaraListesi = getCSLCikanNumaraListesi(CSLCekilisSonuclariListesi, CSLSonNumaralar);
            List<CikanNumara> SLCikanNumaraListesi = getSLCikanNumaraListesi(SLCekilisSonuclariListesi, SLSonNumaralar);
            List<CikanNumara> STCikanNumaraListesi = getSTCikanNumaraListesi(STCekilisSonuclariListesi, STSonNumaralar);

            Console.WriteLine($"---Çılgın Sayısal Loto Hesaplanan Hafta: {CSLCekilisSonuclariListesi.Count}---");



            //var KolonAdat = getCSLKolonAdat(CSLCikanNumaraListesi, CSLSonNumaralar);
            //Console.WriteLine($"   ---Kolon En Az Çıkma Adatına göre: {string.Join(",", KolonAdat)}");

            //var KolonAdat = getCSLKolonAdatTarih(CSLCikanNumaraListesi);
            //Console.WriteLine($"   ---Kolon En Az Çıkma Adatına göre Tarih : {string.Join(",", KolonAdat)}");

            //KolonAdat = getCSLKolonAdatDagitimli(CSLCikanNumaraListesi);
            //Console.WriteLine($"   ---Kolon En Az Çıkma Adatına göre dağılımlı: {string.Join(",", KolonAdat)}");


            //var KolonveJokerAdat = getCSLKolonveJokerAdat(CSLCikanNumaraListesi);
            //Console.WriteLine($"   ---Kolon ve Joker En Az Çıkma Adatına göre: {string.Join(",", KolonveJokerAdat)}");

            var KolonveJokerAdat = getCSLKolonveJokerAdatTarih(CSLCikanNumaraListesi);
            Console.WriteLine($"   ---Kolon ve Joker En Az Çıkma ADAT: {string.Join(",", KolonveJokerAdat)}");

            var KolonveJokersayi = getCSLKolonveJokerCikmaSayisi(CSLCikanNumaraListesi, KolonveJokerAdat);
            Console.WriteLine($"   ---Kolon ve Joker En Az Çıkma SAYI: {string.Join(",", KolonveJokersayi)}");
            
            //var KolonveJokerAdatdagitim = getCSLKolonveJokerAdatDagitimli(CSLCikanNumaraListesi, KolonveJokerAdat.Concat(KolonveJokersayi));
            //Console.WriteLine($"   ---Kolon ve Joker En Az Çıkma ADAT Dağıtım: {string.Join(",", KolonveJokerAdatdagitim)}");

            var KolonveJokerAdatAzCok = getCSLKolonveJokerAdatEncokEnAz(CSLCikanNumaraListesi, KolonveJokerAdat.Concat(KolonveJokersayi));
            Console.WriteLine($"   ---Kolon ve Joker En Az, En Çok Çıkma ADAT: {string.Join(",", KolonveJokerAdatAzCok)}");

            

            var ary = GetCSLSuperStarAdat(CSLCikanNumaraListesi, KolonveJokerAdat);
            Console.WriteLine($"   ---SuperStar En Az Çıkma Adatına göre: {string.Join(",", ary)}");

            //var ary = GetCSLSuperStarAdatTarih(CSLCikanNumaraListesi, KolonveJokerAdat);
            //Console.WriteLine($"   ---SuperStar En Az Çıkma ADAT: {string.Join(",", ary)}");

            //var Tumu = getCSLTumuAdat(CSLCikanNumaraListesi);
            //Console.WriteLine($"   ---Tümü  En Az Çıkma Adatına göre: {string.Join(",", Tumu)}");

            Console.WriteLine("");

            //var KolonSayi = getCSLKolonSayi(CSLCikanNumaraListesi);
            //Console.WriteLine($"   ---Kolon En Az Çıkma Sayısına göre: {string.Join(",", KolonSayi)}");

            //var KolonveJokersayi = getCSLKolonveJokerSayi(CSLCikanNumaraListesi);
            //Console.WriteLine($"   ---Kolon ve Joker En Az Çıkma Sayısına göre: {string.Join(",", KolonveJokersayi)}");


            //ary = GetCSLSuperStarSayi(CSLCikanNumaraListesi, KolonSayi);
            //Console.WriteLine($"   ---SuperStar En Az Çıkma Sayısına göre: {string.Join(",", ary)}");

            //ary = getCSLTumuSayi(CSLCikanNumaraListesi);
            //Console.WriteLine($"   ---Tümü  En Az Çıkma Sayısına göre: {string.Join(",", ary)}");

            Console.WriteLine("");
            Console.WriteLine($"---Süper Loto Hesaplanan Hafta: {SLCekilisSonuclariListesi.Count}---");


            //var SLTumu = getSLTumuAdat(SLCikanNumaraListesi);
            //Console.WriteLine($"   ---Süper Loto En Az Çıkma Adatına göre: {string.Join(",", SLTumu)}");

            var SLTumuEnAz = getSLTumuAdatTarih(SLCikanNumaraListesi);

            var SLTumuEnCok = getSLTumuAdatTarihEncok(SLCikanNumaraListesi, 6);
            Console.WriteLine($"   ---Süper Loto En Az Çıkma Adatına göre Tarih: {string.Join(",", SLTumuEnAz)} ({string.Join(",", SLTumuEnCok)}) ");

            var enazlst = SLTumuEnCok.ToList();
            enazlst.AddRange(SLTumuEnAz);
            Kombinasyon.Kombinasyonlar(enazlst, 6,false);

            Console.WriteLine("");
            Console.WriteLine($"---Şans Topu Hesaplanan Hafta: {STCekilisSonuclariListesi.Count}---");
            var STKolonAdat = getSTKolonAdatTarih(STCikanNumaraListesi);
            var STKolonAdatEnCok = getSTKolonAdatTarihEnCok(STCikanNumaraListesi);           
            Console.WriteLine($"   ---Kolon En Az Çıkma Adatına göre Tarih: {string.Join(",", STKolonAdat)} ({string.Join(",", STKolonAdatEnCok)})");
            //var stenazlst = STKolonAdat.ToList();
            //stenazlst.AddRange(STKolonAdatEnCok);
            //Kombinasyon.Kombinasyonlar(stenazlst, 5, true);



            STKolonAdat = getSTSansTopuAdatTarih(STCikanNumaraListesi);
            Console.WriteLine($"   ---Şans Topu En Az Çıkma Adatına göre Tarih: {string.Join(",", STKolonAdat)}");



            //SLTumu = getSLTumuAdatDagitimli(SLCikanNumaraListesi);
            //Console.WriteLine($"   ---Süper Loto En Az Çıkma Adatına göre Dağıtımlı: {string.Join(",", SLTumu)}");

            //SLTumu = getSLTumuAdatCarpma(SLCikanNumaraListesi);
            //Console.WriteLine($"   ---Süper Loto En Az Çıkma Adatına göre Çarpma: {string.Join(",", SLTumu)}");

            //SLTumu = getSLTumuSayi(SLCikanNumaraListesi);
            //Console.WriteLine($"   ---Süper Loto En Az Çıkma sayısına göre: {string.Join(",", SLTumu)}");

            Console.ReadLine();
        }


        public static DataSet GetExcelToDataSet(string xlsxPath)
        {

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = File.Open(xlsxPath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {

                    return reader.AsDataSet(
                        new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true,
                            }
                        }
                        );
                }
            }
        }


        public static List<CSLCekilisSonucu> GetCSLCekilisSonuclariListesi(DataTable dt)
        {
            return dt
            .AsEnumerable()
            .Select(x => new CSLCekilisSonucu
            {
                Hafta = x.Field<double>(CSLKolonlari.Hafta.ToString()),
                Tarih = DateOnly.Parse(x.Field<DateTime>(CSLKolonlari.Tarih.ToString()).ToString("dd.MM.yyy")),
                Kolon1 = x.Field<double>(CSLKolonlari.Kolon1.ToString()),
                Kolon2 = x.Field<double>(CSLKolonlari.Kolon2.ToString()),
                Kolon3 = x.Field<double>(CSLKolonlari.Kolon3.ToString()),
                Kolon4 = x.Field<double>(CSLKolonlari.Kolon4.ToString()),
                Kolon5 = x.Field<double>(CSLKolonlari.Kolon5.ToString()),
                Kolon6 = x.Field<double>(CSLKolonlari.Kolon6.ToString()),
                Joker = x.Field<double>(CSLKolonlari.Joker.ToString()),
                SuperStar = double.TryParse(x[CSLKolonlari.SuperStar.ToString()]?.ToString(), out double val) ? val : 0
            })
            .ToList();
        }

        public static List<STCekilisSonucu> GetSTCekilisSonuclariListesi(DataTable dt)
        {
            return dt
            .AsEnumerable()
            .Select(x => new STCekilisSonucu
            {
                Hafta = x.Field<double>(STKolonlari.Hafta.ToString()),
                Tarih = DateOnly.Parse(x.Field<DateTime>(STKolonlari.Tarih.ToString()).ToString("dd.MM.yyy")),
                Kolon1 = x.Field<double>(STKolonlari.Kolon1.ToString()),
                Kolon2 = x.Field<double>(STKolonlari.Kolon2.ToString()),
                Kolon3 = x.Field<double>(STKolonlari.Kolon3.ToString()),
                Kolon4 = x.Field<double>(STKolonlari.Kolon4.ToString()),
                Kolon5 = x.Field<double>(STKolonlari.Kolon5.ToString()),                
                SansTopu = x.Field<double>(STKolonlari.SansTopu.ToString())
                
            })
            .ToList();
        }

        public static List<SLCekilisSonucu> getSLCekilisSonuclariListesi(DataTable dt)
        {
            return dt
           .AsEnumerable()
           .Select(x => new SLCekilisSonucu
           {
               Hafta = x.Field<double>(SLKolonlari.Hafta.ToString()),
               Tarih = DateOnly.Parse(x.Field<DateTime>(CSLKolonlari.Tarih.ToString()).ToString("dd.MM.yyy")),
               Kolon1 = x.Field<double>(SLKolonlari.Kolon1.ToString()),
               Kolon2 = x.Field<double>(SLKolonlari.Kolon2.ToString()),
               Kolon3 = x.Field<double>(SLKolonlari.Kolon3.ToString()),
               Kolon4 = x.Field<double>(SLKolonlari.Kolon4.ToString()),
               Kolon5 = x.Field<double>(SLKolonlari.Kolon5.ToString()),
               Kolon6 = x.Field<double>(SLKolonlari.Kolon6.ToString())
           })
           .ToList();
        }

        public static List<CikanNumara> getCSLCikanNumaraListesi(List<CSLCekilisSonucu> CSLCekilisSonuclariListesi, List<CikanNumara>? CSLSonNumara = null)
        {
            List<CikanNumara> CSLCikanNumaraListesi = new List<CikanNumara>();
            var katsayi = 1;
            CSLCekilisSonuclariListesi
                .OrderBy(x => x.Tarih).ToList().ForEach(x =>
            {
                var Hafta = double.Parse(x.GetType().GetProperty("Hafta")?.GetValue(x)?.ToString() ?? "0");
                var CikmaTarihi = DateTime.Parse(x.GetType().GetProperty("Tarih")?.GetValue(x)?.ToString() ?? "0");

                var TarihFark = (DateTime.Today - CikmaTarihi).TotalDays;


                x.GetType()
                .GetProperties()
                .ToList()
                .ForEach(y =>
                {
                    string KolonTipi = "";
                    CikanNumara? cikanno;
                    if (y.Name == CSLKolonlari.SuperStar.ToString())
                        KolonTipi = CSLKolonlari.SuperStar.ToString();
                    else if (y.Name == CSLKolonlari.Joker.ToString())
                        KolonTipi = CSLKolonlari.Joker.ToString();
                    else if (y.Name.Contains("Kolon"))
                        KolonTipi = "Kolon";

                    if (!string.IsNullOrEmpty(KolonTipi))
                    {
                        var numara = (double)(y.GetValue(x) ?? 0);
                        if (numara != 0 && (!CSLSonNumara?.Any(x => x.Numara == numara) ?? true))
                        {

                            var a = (CSLSonNumara?.Any(x => x.Numara == numara) ?? true);
                            cikanno = CSLCikanNumaraListesi
                                                        .Where(z => z.Numara == numara && z.KolonTipi == KolonTipi)
                                                        .FirstOrDefault();
                            if (cikanno == null)
                            {
                                var yeniNo = new CikanNumara()
                                {
                                    KolonTipi = KolonTipi,
                                    Numara = numara,
                                    CikmaAdati = Hafta,
                                    CikmaTarihAdati = TarihFark,
                                    CikmaSayisi = 1,
                                    KatSayi = katsayi++
                                };
                                CSLCikanNumaraListesi.Add(yeniNo);
                            }
                            else
                            {
                                cikanno.CikmaSayisi++;
                                cikanno.CikmaAdati += Hafta;
                                cikanno.CikmaTarihAdati += TarihFark;
                                cikanno.KatSayi += katsayi++;
                            }
                        }
                    }
                });
            });
            return CSLCikanNumaraListesi;
        }
        public static List<CikanNumara> getSTCikanNumaraListesi(List<STCekilisSonucu> STCekilisSonuclariListesi, List<CikanNumara>? STSonNumara = null)
        {
            List<CikanNumara> STCikanNumaraListesi = new List<CikanNumara>();
            var katsayi = 1;
            STCekilisSonuclariListesi
                .OrderBy(x => x.Tarih).ToList().ForEach(x =>
                {
                    var Hafta = double.Parse(x.GetType().GetProperty("Hafta")?.GetValue(x)?.ToString() ?? "0");
                    var CikmaTarihi = DateTime.Parse(x.GetType().GetProperty("Tarih")?.GetValue(x)?.ToString() ?? "0");

                    var TarihFark = (DateTime.Today - CikmaTarihi).TotalDays;


                    x.GetType()
                    .GetProperties()
                    .ToList()
                    .ForEach(y =>
                    {
                        string KolonTipi = "";
                        CikanNumara? cikanno;
                        if (y.Name == STKolonlari.SansTopu.ToString())
                            KolonTipi = STKolonlari.SansTopu.ToString();
                        if (y.Name.Contains("Kolon"))
                            KolonTipi = "Kolon";

                        if (!string.IsNullOrEmpty(KolonTipi))
                        {
                            var numara = (double)(y.GetValue(x) ?? 0);
                            if (numara != 0 && (!STSonNumara?.Any(x => x.Numara == numara) ?? true))
                            {

                                var a = (STSonNumara?.Any(x => x.Numara == numara) ?? true);
                                cikanno = STCikanNumaraListesi
                                                            .Where(z => z.Numara == numara && z.KolonTipi == KolonTipi)
                                                            .FirstOrDefault();
                                if (cikanno == null)
                                {
                                    var yeniNo = new CikanNumara()
                                    {
                                        KolonTipi = KolonTipi,
                                        Numara = numara,
                                        CikmaAdati = Hafta,
                                        CikmaTarihAdati = TarihFark,
                                        CikmaSayisi = 1,
                                        KatSayi = katsayi++
                                    };
                                    STCikanNumaraListesi.Add(yeniNo);
                                }
                                else
                                {
                                    cikanno.CikmaSayisi++;
                                    cikanno.CikmaAdati += Hafta;
                                    cikanno.CikmaTarihAdati += TarihFark;
                                    cikanno.KatSayi += katsayi++;
                                }
                            }
                        }
                    });
                });
            return STCikanNumaraListesi;
        }

        public static IEnumerable<string> getCSLKolonAdat(List<CikanNumara> CSLCikanNumaraListesi, List<CikanNumara> CSLSonNumara)
        {
            return CSLCikanNumaraListesi.Where(x => x.KolonTipi == "Kolon" && !CSLSonNumara.Any(y => y.Numara == x.Numara))
                   .OrderBy(x => (x.CikmaAdati / x.CikmaSayisi))
                   //.OrderBy(x => (x.CikmaAdati / x.CikmaSayisi) * x.KatSayi)
                   //.OrderBy(x => x.CikmaAdati)
                   .Take(6)
                   .OrderBy(x => x.Numara)
                   .Select(x => x.Numara.ToString());
        }

        public static IEnumerable<string> getCSLKolonAdatTarih(List<CikanNumara> CSLCikanNumaraListesi)
        {
            return CSLCikanNumaraListesi.Where(x => x.KolonTipi == "Kolon")
                   //.OrderBy(x => (x.CikmaTarihAdati / x.CikmaSayisi) * x.KatSayi)
                   //.OrderBy(x => x.CikmaTarihAdati)
                   .OrderBy(x => (x.CikmaTarihAdati / x.CikmaSayisi))
                   .Take(6)
                   .OrderBy(x => x.Numara)
                   .Select(x => x.Numara.ToString());
        }

        public static IEnumerable<string> getCSLKolonAdatDagitimli(List<CikanNumara> CSLCikanNumaraListesi)
        {
            List<string> numaralar = new List<string>();

            for (int i = 0; i < 6; i++)

                numaralar.Add(CSLCikanNumaraListesi.Where(x => x.KolonTipi == "Kolon")
                       .OrderBy(x => x.Numara)
                       .Skip(i * 15)
                       .Take(15) //ilk 15
                                 //.OrderBy(x => (x.CikmaAdati / x.CikmaSayisi) * x.KatSayi)
                                 .OrderBy(x => (x.CikmaAdati / x.CikmaSayisi))
                       //.OrderBy(x => x.CikmaAdati)
                       .Take(1)
                       .Select(x => x.Numara.ToString()).FirstOrDefault(""));

            return numaralar;
        }

        public static IEnumerable<string> getCSLKolonSayi(List<CikanNumara> CSLCikanNumaraListesi)
        {
            return CSLCikanNumaraListesi.Where(x => x.KolonTipi == "Kolon")
                       .OrderBy(x => x.CikmaSayisi)
                       .Take(6)
                       .OrderBy(x => x.Numara)
                       .Select(x => x.Numara.ToString());
        }

        public static IEnumerable<string> getCSLKolonveJokerAdat(List<CikanNumara> CSLCikanNumaraListesi)
        {
            return CSLCikanNumaraListesi.Where(x => x.KolonTipi == "Kolon" || x.KolonTipi == CSLKolonlari.Joker.ToString())
                .GroupBy(x => x.Numara)
                .Select(group => new
                {
                    Numara = group.Key,
                    CikmaAdati = group.Sum(item => item.CikmaAdati),
                    CikmaSayisi = group.Sum(item => item.CikmaSayisi),
                    KatSayi = group.Sum(item => item.KatSayi)
                }
                                        )
                   //.OrderBy(x => (x.CikmaAdati / x.CikmaSayisi) * x.KatSayi)
                   .OrderBy(x => (x.CikmaAdati / x.CikmaSayisi))
                   //.OrderBy(x => x.CikmaAdati)
                   .Take(6)
                   .OrderBy(x => x.Numara)
                   .Select(x => x.Numara.ToString());

        }

        public static IEnumerable<string> getCSLKolonveJokerAdatTarih(List<CikanNumara> CSLCikanNumaraListesi)
        {

            
            

            return CSLCikanNumaraListesi.Where(x => x.KolonTipi ==  "Kolon" || x.KolonTipi == CSLKolonlari.Joker.ToString())
                .GroupBy(x => x.Numara)
                .Select(group => new
                {
                    Numara = group.Key,
                    CikmaAdati = group.Sum(item => item.CikmaTarihAdati),
                    CikmaSayisi = group.Sum(item => item.CikmaSayisi),
                    KatSayi = group.Sum(item => item.KatSayi)
                })
                   //.OrderBy(x => (x.CikmaAdati / x.CikmaSayisi) * x.KatSayi)
                   .OrderBy(x => (x.CikmaAdati / x.CikmaSayisi))
                   //.OrderBy(x => x.CikmaAdati)
                   .Take(6)
                   .OrderBy(x => x.Numara)
                   .Select(x => x.Numara.ToString());

        }

        public static IEnumerable<string> getCSLKolonveJokerAdatEncokEnAz(List<CikanNumara> CSLCikanNumaraListesi, IEnumerable<string> DahilEtme)
        {
           var enaz =  CSLCikanNumaraListesi.Where(x => (x.KolonTipi == "Kolon" || x.KolonTipi == CSLKolonlari.Joker.ToString()) && !DahilEtme.Any(a => a == x.Numara.ToString()))
                .GroupBy(x => x.Numara)
                .Select(group => new
                {
                    Numara = group.Key,
                    CikmaAdati = group.Sum(item => item.CikmaTarihAdati),
                    CikmaSayisi = group.Sum(item => item.CikmaSayisi),
                    KatSayi = group.Sum(item => item.KatSayi)
                })
                   //.OrderBy(x => (x.CikmaAdati / x.CikmaSayisi) * x.KatSayi)
                   .OrderBy(x => (x.CikmaAdati / x.CikmaSayisi))
                   //.OrderBy(x => x.CikmaAdati)
                   .Take(3)
                   .OrderBy(x => x.Numara)
                   .Select(x => x.Numara.ToString())
                   .ToList();

            var encok = CSLCikanNumaraListesi.Where(x => x.KolonTipi == "Kolon" || x.KolonTipi == CSLKolonlari.SuperStar.ToString())
                .GroupBy(x => x.Numara)
                .Select(group => new
                {
                    Numara = group.Key,
                    CikmaAdati = group.Sum(item => item.CikmaTarihAdati),
                    CikmaSayisi = group.Sum(item => item.CikmaSayisi),
                    KatSayi = group.Sum(item => item.KatSayi)
                })
                   //.OrderBy(x => (x.CikmaAdati / x.CikmaSayisi) * x.KatSayi)
                   .OrderByDescending(x => (x.CikmaAdati / x.CikmaSayisi))
                   //.OrderBy(x => x.CikmaAdati)
                   .Take(3)
                   .OrderBy(x => x.Numara)
                   .Select(x => x.Numara.ToString())
                   .ToList();

            return enaz.Concat(encok).OrderBy(x=>  int.Parse(x));
            

        }

        public static IEnumerable<string> getCSLKolonveJokerCikmaSayisi(List<CikanNumara> CSLCikanNumaraListesi, IEnumerable<string> DahilEtme )
        {
            
            return CSLCikanNumaraListesi.Where(x => (x.KolonTipi == "Kolon" || x.KolonTipi == CSLKolonlari.Joker.ToString()) && !DahilEtme.Any(a => a == x.Numara.ToString()))
                .GroupBy(x => x.Numara)
                .Select(group => new
                {
                    Numara = group.Key,
                    CikmaAdati = group.Sum(item => item.CikmaTarihAdati),
                    CikmaSayisi = group.Sum(item => item.CikmaSayisi),
                    KatSayi = group.Sum(item => item.KatSayi)
                })
                   //.OrderBy(x => (x.CikmaAdati / x.CikmaSayisi) * x.KatSayi)
                   .OrderBy(x => x.CikmaSayisi)
                   //.OrderBy(x => x.CikmaAdati)
                   .Take(6)
                   .OrderBy(x => x.Numara)
                   .Select(x => x.Numara.ToString());

        }

        public static IEnumerable<string> getSTKolonAdatTarih(List<CikanNumara> StCikanNumaraListesi)
        {
            return StCikanNumaraListesi.Where(x => x.KolonTipi == "Kolon")
                .GroupBy(x => x.Numara)
                .Select(group => new
                {
                    Numara = group.Key,
                    CikmaAdati = group.Sum(item => item.CikmaTarihAdati),
                    CikmaSayisi = group.Sum(item => item.CikmaSayisi),
                    KatSayi = group.Sum(item => item.KatSayi)
                })
                   //.OrderBy(x => (x.CikmaAdati / x.CikmaSayisi) * x.KatSayi)
                   .OrderBy(x => (x.CikmaAdati / x.CikmaSayisi))
                   //.OrderBy(x => x.CikmaAdati)
                   .Take(5)
                   .OrderBy(x => x.Numara)
                   .Select(x => x.Numara.ToString());

        }
        public static IEnumerable<string> getSTKolonAdatTarihEnCok(List<CikanNumara> StCikanNumaraListesi)
        {
            return StCikanNumaraListesi.Where(x => x.KolonTipi == "Kolon")
                .GroupBy(x => x.Numara)
                .Select(group => new
                {
                    Numara = group.Key,
                    CikmaAdati = group.Sum(item => item.CikmaTarihAdati),
                    CikmaSayisi = group.Sum(item => item.CikmaSayisi),
                    KatSayi = group.Sum(item => item.KatSayi)
                })
                   //.OrderBy(x => (x.CikmaAdati / x.CikmaSayisi) * x.KatSayi)
                   .OrderByDescending(x => (x.CikmaAdati / x.CikmaSayisi))
                   //.OrderBy(x => x.CikmaAdati)
                   .Take(5)
                   
                   .Select(x => x.Numara.ToString());

        }

        public static IEnumerable<string> getSTSansTopuAdatTarih(List<CikanNumara> STCikanNumaraListesi)
        {
            return STCikanNumaraListesi.Where(x => x.KolonTipi == "SansTopu")
                .GroupBy(x => x.Numara)
                .Select(group => new
                {
                    Numara = group.Key,
                    CikmaAdati = group.Sum(item => item.CikmaTarihAdati),
                    CikmaSayisi = group.Sum(item => item.CikmaSayisi),
                    KatSayi = group.Sum(item => item.KatSayi)
                })                   
                   .OrderBy(x => (x.CikmaAdati / x.CikmaSayisi))                  
                   .Take(5)                   
                   .Select(x => x.Numara.ToString());

        }
        public static IEnumerable<string> getCSLKolonveJokerAdatDagitimli(List<CikanNumara> CSLCikanNumaraListesi, IEnumerable<string> DahilEtme)
        {

            List<string> numaralar = new List<string>();

            for (int i = 0; i < 6; i++)

                numaralar.Add(CSLCikanNumaraListesi.Where(x => (x.KolonTipi == "Kolon" || x.KolonTipi == CSLKolonlari.Joker.ToString()) && !DahilEtme.Any(a => a == x.Numara.ToString()))
                .GroupBy(x => x.Numara)
                .Select(group => new
                {
                    Numara = group.Key,
                    CikmaAdati = group.Sum(item => item.CikmaAdati),
                    CikmaSayisi = group.Sum(item => item.CikmaSayisi),
                    KatSayi = group.Sum(item => item.KatSayi)
                })
                .OrderBy(x => x.Numara)
                .Skip(i * 15)
                .Take(15)
                //.OrderBy(x => (x.CikmaAdati / x.CikmaSayisi) * x.KatSayi)
                .OrderBy(x => (x.CikmaAdati / x.CikmaSayisi))
                //.OrderBy(x => x.CikmaAdati)
                .Take(1)
                .Select(x => x.Numara.ToString()).FirstOrDefault(""));

            return numaralar;



        }
        public static IEnumerable<string> getCSLKolonveJokerSayi(List<CikanNumara> CSLCikanNumaraListesi)
        {
            return CSLCikanNumaraListesi.Where(x => x.KolonTipi == "Kolon" || x.KolonTipi == CSLKolonlari.Joker.ToString())
                .GroupBy(x => x.Numara)
                .Select(group => new
                {
                    Numara = group.Key,
                    CikmaAdati = group.Sum(item => item.CikmaAdati),
                    CikmaSayisi = group.Sum(item => item.CikmaSayisi),
                    KatSayi = group.Sum(item => item.KatSayi)
                })
                .OrderBy(x => x.CikmaSayisi)
                .Take(6)
                .OrderBy(x => x.Numara)
                .Select(x => x.Numara.ToString());

        }
        public static IEnumerable<string> GetCSLSuperStarAdat(List<CikanNumara> CSLCikanNumaraListesi, IEnumerable<string> KolonAdat)
        {
            return CSLCikanNumaraListesi.Where(x => x.KolonTipi == CSLKolonlari.SuperStar.ToString() && !KolonAdat.Any(a => a == x.Numara.ToString()))
                               //.OrderBy(x => (x.CikmaAdati / x.CikmaSayisi) * x.KatSayi)
                               .OrderBy(x => (x.CikmaAdati / x.CikmaSayisi))
                               //.OrderBy(x => x.CikmaAdati)
                               .Take(6)
                               .Select(x => x.Numara.ToString());
        }

        public static IEnumerable<string> GetCSLSuperStarAdatTarih(List<CikanNumara> CSLCikanNumaraListesi, IEnumerable<string> KolonAdat)
        {
            return CSLCikanNumaraListesi.Where(x => x.KolonTipi == CSLKolonlari.SuperStar.ToString() && !KolonAdat.Any(a => a == x.Numara.ToString()))
                               //.OrderBy(x => x.CikmaTarihAdati / x.KatSayi)
                               .OrderBy(x => x.CikmaTarihAdati)
                               //.OrderBy(x => x.CikmaTarihAdati)
                               .Take(6)
                               .Select(x => x.Numara.ToString());
        }

        public static IEnumerable<string> GetCSLSuperStarSayi(List<CikanNumara> CSLCikanNumaraListesi, IEnumerable<string> KolonSayi)
        {
            return CSLCikanNumaraListesi.Where(x => x.KolonTipi == CSLKolonlari.SuperStar.ToString() && !KolonSayi.Any(a => a == x.Numara.ToString()))
                       .OrderBy(x => x.CikmaSayisi)
                       .Take(6)
                       .Select(x => x.Numara.ToString());
        }

        public static IEnumerable<string> getCSLTumuAdat(List<CikanNumara> CSLCikanNumaraListesi)
        {
            return CSLCikanNumaraListesi
                .GroupBy(x => x.Numara)
                .Select(group => new
                {
                    Numara = group.Key,
                    CikmaAdati = group.Sum(item => item.CikmaAdati),
                    CikmaSayisi = group.Sum(item => item.CikmaSayisi),
                    KatSayi = group.Sum(item => item.CikmaSayisi)
                })
                .OrderBy(x => (x.CikmaAdati / x.CikmaSayisi))
                //.OrderBy(x => x.CikmaAdati)
                .Take(6)
                .OrderBy(x => x.Numara)
                .Select(x => x.Numara.ToString());
        }
        public static IEnumerable<string> getCSLTumuSayi(List<CikanNumara> CSLCikanNumaraListesi)
        {
            return CSLCikanNumaraListesi
                .GroupBy(x => x.Numara)
                .Select(group => new
                {
                    Numara = group.Key,
                    CikmaAdati = group.Sum(item => item.CikmaAdati),
                    CikmaSayisi = group.Sum(item => item.CikmaSayisi),
                    KatSayi = group.Sum(item => item.KatSayi)
                })
                .OrderBy(x => x.CikmaSayisi)
                .Take(6)
                .OrderBy(x => x.Numara)
                .Select(x => x.Numara.ToString());
        }

        public static List<CikanNumara> getSLCikanNumaraListesi(List<SLCekilisSonucu> SLCekilisSonuclariListesi, List<CikanNumara>? SLSonNumara = null)
        {
            List<CikanNumara> SLCikanNumaraListesi = new List<CikanNumara>();

            var KatSayi = 1;

            SLCekilisSonuclariListesi.OrderBy(x => x.Tarih).ToList().ForEach(x =>
            {
                var Hafta = double.Parse(x.GetType().GetProperty("Hafta")?.GetValue(x)?.ToString() ?? "0");
                var CikmaTarihi = DateTime.Parse(x.GetType().GetProperty("Tarih")?.GetValue(x)?.ToString() ?? "0");
                var TarihFark = (DateTime.Today - CikmaTarihi).TotalDays;

                x.GetType()
                .GetProperties()
                .ToList()
                .ForEach(y =>
                {
                    string KolonTipi = "";
                    CikanNumara? cikanno;

                    if (y.Name.Contains("Kolon"))
                        KolonTipi = "Kolon";

                    if (!string.IsNullOrEmpty(KolonTipi))
                    {
                        var numara = (double)(y.GetValue(x) ?? 0);
                        if (numara != 0 && (!SLSonNumara?.Any(x => x.Numara == numara) ?? true))
                        {
                            cikanno = SLCikanNumaraListesi
                                                        .Where(z => z.Numara == numara && z.KolonTipi == KolonTipi)
                                                        .FirstOrDefault();
                            if (cikanno == null)
                            {
                                var yeniNo = new CikanNumara()
                                {
                                    KolonTipi = KolonTipi,
                                    Numara = numara,
                                    CikmaAdati = Hafta,
                                    CikmaSayisi = 1,
                                    CikmaTarihAdati = TarihFark,
                                    KatSayi = KatSayi++
                                };
                                SLCikanNumaraListesi.Add(yeniNo);
                            }
                            else
                            {
                                cikanno.CikmaSayisi++;
                                cikanno.CikmaAdati += Hafta;
                                cikanno.CikmaTarihAdati += TarihFark;
                                cikanno.KatSayi += KatSayi++;
                            }
                        }
                    }
                });
            });
            return SLCikanNumaraListesi;
        }

        public static IEnumerable<string> getSLTumuAdat(List<CikanNumara> SLCikanNumaraListesi)
        {
            return SLCikanNumaraListesi
                               //.OrderBy(x => (x.CikmaAdati / x.CikmaSayisi) * x.KatSayi)
                               .OrderBy(x => (x.CikmaAdati / x.CikmaSayisi))
                               //.OrderBy(x => x.CikmaAdati)
                               .Take(6)
                               .OrderBy(x => x.Numara)
                               .Select(x => x.Numara.ToString());
        }

        public static IEnumerable<string> getSLTumuAdatTarih(List<CikanNumara> SLCikanNumaraListesi)
        {
            return SLCikanNumaraListesi
                               //.OrderBy(x => (x.CikmaTarihAdati / x.CikmaSayisi) * x.KatSayi)
                               .OrderBy(x => (x.CikmaTarihAdati / x.CikmaSayisi))
                               //.OrderBy(x => x.CikmaTarihAdati)
                               .Take(6)
                               .OrderBy(x => x.Numara)
                               .Select(x => x.Numara.ToString());
        }

        public static IEnumerable<string> getSLTumuAdatTarihEncok(List<CikanNumara> SLCikanNumaraListesi, int take = 1)
        {
            return SLCikanNumaraListesi
                               //.OrderBy(x => (x.CikmaTarihAdati / x.CikmaSayisi) * x.KatSayi)
                               .OrderByDescending(x => (x.CikmaTarihAdati / x.CikmaSayisi))
                               //.OrderBy(x => x.CikmaTarihAdati)
                               .Take(take)
                               //.OrderBy(x => x.Numara)
                               .Select(x => x.Numara.ToString());
        }

        public static IEnumerable<string> getSLTumuAdatCarpma(List<CikanNumara> SLCikanNumaraListesi)
        {
            return SLCikanNumaraListesi
                               .OrderBy(x => x.CikmaAdati * x.CikmaSayisi)
                               .Take(6)
                               .OrderBy(x => x.Numara)
                               .Select(x => x.Numara.ToString());
        }

        public static IEnumerable<string> getSLTumuAdatDagitimli(List<CikanNumara> SLCikanNumaraListesi)
        {
            List<string> numaralar = new List<string>();

            for (int i = 0; i < 6; i++)
            {
                numaralar.Add(SLCikanNumaraListesi
                               .OrderBy(x => x.Numara)
                               .Skip(i * 10)
                               .Take(10)
                               //.OrderBy(x => (x.CikmaAdati / x.CikmaSayisi) * x.KatSayi)
                               .OrderBy(x => (x.CikmaAdati / x.CikmaSayisi))
                               //.OrderBy(x => x.CikmaAdati)
                               .Take(1)
                               .Select(x => x.Numara.ToString()).FirstOrDefault(""));

            }
            return numaralar;
        }


        public static IEnumerable<string> getSLTumuSayi(List<CikanNumara> SLCikanNumaraListesi)
        {
            return SLCikanNumaraListesi
                       .OrderBy(x => x.CikmaSayisi)
                       .Take(6)
                       .OrderBy(x => x.Numara)
                       .Select(x => x.Numara.ToString());
        }
    }
}


