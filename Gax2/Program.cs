using System.Collections.Generic;
// HashSet <Stream> test = new();
// HashSet <int> test2 = new();
// Dictionary <int, string> dic = new();
// dic.Add(12,"this is rare");
// dic.Add(14, dic[12]);

// for(int i = 0; i < 10; i++)
// {
//     Stream temp = new(2,1);
//     Console.WriteLine(temp.GetHashCode());
//     test.Add(temp);
    
//     test2.Add(2);
// }
// test.Add(new(4,7));
// foreach(Stream testee in test)
// {
//     Console.WriteLine(testee.ToString());
// }

Galaxy testGax = new Galaxy(5, 2);
Stream temp = new(testGax.Nodes[1], testGax.Nodes[2]);

// Console.WriteLine(test.Count());
Console.WriteLine("done");
Console.ReadLine();
