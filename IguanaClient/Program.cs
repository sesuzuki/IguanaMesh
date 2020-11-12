using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Iguana.IguanaMesh.ICreators;
using Iguana.IguanaMesh.ITypes;
using Iguana.IguanaMesh.IUtils;
using Iguana.IguanaMesh.IWrappers;

namespace IguanaClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //Example.T3();

            int eID = 70000000;
            int hf1 = 9789;
            int hf2 = 15000;

            Int64 sib = IHelpers.PackTripleKey(eID, hf1, hf2);
            Console.WriteLine(sib);

            int p1 = IHelpers.UnpackFirst32BitsOnTripleKey(sib);
            int p2 = IHelpers.UnpackSecond16BitsOnTripleKey(sib);
            int p3 = IHelpers.UnpackThird16BitsOnTripleKey(sib);
            //IHelpers.UnpackTripleKey(sib, out p1, out p2, out p3);
            Console.WriteLine(p1 + " ::"  + p2 + " :: " + p3);

            Console.ReadLine();
        }     
    }
}
