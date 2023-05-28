using Raylib_cs;
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
float size = 20;
Galaxy testGax = new Galaxy(500, 1, size);
Stream temp = new(testGax.Nodes[1], testGax.Nodes[2]);
foreach(KeyValuePair<int, Node> node in testGax.Nodes)
{
    if(node.Value.Streams.Count > 3) Console.WriteLine(node.Value.Streams.Count +" "+ node.Value.key);
}
Console.WriteLine("Finished");
int windowSize = 900;
Raylib.InitWindow(windowSize, windowSize,"Test Galaxy");
while(!Raylib.WindowShouldClose())
{
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.BLACK);
    foreach(KeyValuePair<int, Node> Node in testGax.Nodes)
    {
        Raylib.DrawCircle((int) ((Node.Value.point.X/size)*windowSize), (int)((Node.Value.point.Y/size)*windowSize), 2.5f, Color.WHITE);
    }
    foreach(KeyValuePair<int, Stream> Stream in testGax.Streams)
    {
        Raylib.DrawLine((int) ((Stream.Value.root0.point.X/size)*windowSize), (int)((Stream.Value.root0.point.Y/size)*windowSize), (int) ((Stream.Value.root1.point.X/size)*windowSize), (int)((Stream.Value.root1.point.Y/size)*windowSize), Color.GRAY);
    }
    Raylib.EndDrawing();
}

// Console.WriteLine(test.Count());

