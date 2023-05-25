using System;
using System.Numerics;
public class Galaxy
{
    Random rand = new Random();
    public Dictionary<int, Node> Nodes; //Key is index of creation. starts at 0
    public Dictionary<int, Stream> Streams; //Key is the HashCode of Value Stream
    public Galaxy(int count, double StreamDistance)
    {           //star count, maximum "autoconnect distance
        Nodes = new();
        Streams = new();
        Console.WriteLine("Node generation");
        for(int i = 0; i < count; i++) //Node Gen
        {
            //creates stars with a random position inbetween 0,0,-0.5 to 10,10,0.5
            Nodes.Add(i, new(i, new((float) rand.NextDouble()*10f, (float) rand.NextDouble()*10f, (float) rand.NextDouble()-0.5f)));
        }

        StreamGeneration(StreamDistance);

        IsolatedNodePrevention();
    }

    public void CreateStream(Node n1, Node n2)
    {
        Stream tempStream = new(n1, n2);
        if(n1.Equals(n2)) throw new($"Stream from Origin Node to Origin Node prohibited.\n{n1.ToString}");
        //if it's a new stream or an existing one
        if (!Streams.ContainsKey(tempStream.GetHashCode())) Streams.Add(tempStream.GetHashCode(), tempStream);
        //adds the stream to the stars known streams if it isn't already there
        if (!n1.Streams.Contains(tempStream)) n1.Streams.Add(tempStream);
        if (!n2.Streams.Contains(tempStream)) n2.Streams.Add(tempStream);
    }

    private void StreamGeneration(double s)
    {
        foreach(KeyValuePair<int, Node> node in Nodes)
        {
            foreach(KeyValuePair<int, Node> node2 in Nodes)
            {   
                if(node.Value.Equals(node2.Value)) continue;
                if(Vector3.Distance(node.Value.point, node2.Value.point) > s) continue;

                CreateStream(node.Value, node2.Value);
            }
        }
    }

    private void IsolatedNodePrevention()
    {
        foreach(KeyValuePair<int, Node> node in Nodes)
        {
            if(node.Value.Streams.Count > 0) continue;
            Node tempNode = node.Value;
            double distance = double.MaxValue;
            //simple sort to find closest node
            foreach(KeyValuePair<int, Node> node2 in Nodes)
            {
                //skipping the origin node
                if(node.Equals(node2)) continue;

                double tempDistance = Vector3.Distance(node.Value.point, node2.Value.point);
                //if the distance is more than the current lest, skip
                if(tempDistance > distance) continue;
                tempNode = node2.Value;
                distance = tempDistance;
            }
            CreateStream(node.Value, tempNode);
        }
    }

    private void FloodPathsCheck()
    {
        List<Node> pool = new();
        pool.Add(Nodes[rand.Next(0, Nodes.Count+1)]);
        while(true)
        {
            int poolSize = pool.Count;
            foreach(Node star in pool)
            {
                foreach(Stream stream in star.Streams)
                {
                    Node tempStar = stream.GetOpposite(star);
                    if(!pool.Contains(tempStar)) pool.Add(tempStar);
                }
            }
            if(poolSize == pool.Count)
            {
                //TODO break statment and create closest stream between isolated pools
            }
        }
    }
}
