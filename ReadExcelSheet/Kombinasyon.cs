using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelSheet
{
    public static class Kombinasyon
    {
        

        static IEnumerable<IEnumerable<T>>GetKCombs<T>(IEnumerable<T> list, int length) where T : IComparable
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetKCombs(list, length - 1)
                .SelectMany(t => list.Where(o => o.CompareTo(t.Last()) > 0),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        public static void Kombinasyonlar(IEnumerable<string> sayilar, int uyeSayisi)
        {
            var r2 = GetKCombs<string>(sayilar, uyeSayisi);

            r2.ToList()
                .ForEach(x=> Console.WriteLine(string.Join(",",x.ToArray()))
                );
        }
    }
}
