﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ReadExcelSheet
{
    public static class Kombinasyon
    {
        

        static IEnumerable<IEnumerable<T>>GetKCombs<T>(IEnumerable<T> list, int length) where T : IComparable
        {
            if (length == 1) 
                return list.Select(t => new T[] { t });

            return GetKCombs(list, length - 1)
                .SelectMany(t => list.Where(o => o.CompareTo(t.Last()) > 0),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        public static void Kombinasyonlar(IEnumerable<string> sayilar, int uyeSayisi, bool yaz)
        {
            var r2 = GetKCombs<string>(sayilar, uyeSayisi);

            Console.WriteLine($"Kombinasyon sayısı {r2.Count()}");
            int ikitutan = 0;
            if (yaz)
                r2.ToList().ForEach(x =>Console.WriteLine(string.Join(",", x.ToArray()))); 
        }
    }
}
