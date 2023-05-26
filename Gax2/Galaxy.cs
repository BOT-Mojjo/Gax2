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
        Console.Write("Node Generation");
        for (int i = 0; i < count; i++) //Node Gen
        {
            //creates stars with a random position inbetween 0,0,-0.5 to 10,10,0.5
            Nodes.Add(i, new(i, new((float)rand.NextDouble() * 10f, (float)rand.NextDouble() * 10f, (float)rand.NextDouble() - 0.5f)));
        }
        Console.WriteLine(" ... Done!");

        StreamGeneration(StreamDistance);

        IsolatedNodePrevention();

        IsolatedClusterPrevention();
    }

    public void CreateStream(Node n1, Node n2)
    {   //this could and wwould cause shenanigans with pathfinding
        if (n1.Equals(n2)) throw new($"Stream from Origin Node to Origin Node prohibited.\n{n1.ToString}\n");
        Stream tempStream = new(n1, n2);
        //if it's a new stream or an existing one
        if (!Streams.ContainsKey(tempStream.GetHashCode())) Streams.Add(tempStream.GetHashCode(), tempStream);
        //adds the stream to the stars known streams if it isn't already there
        if (!n1.Streams.Contains(tempStream)) n1.Streams.Add(tempStream);
        if (!n2.Streams.Contains(tempStream)) n2.Streams.Add(tempStream);
    }

    private void StreamGeneration(double s)
    {
        Console.Write("Stream Generation");
        foreach (KeyValuePair<int, Node> node in Nodes)
        {
            foreach (KeyValuePair<int, Node> node2 in Nodes)
            {
                if (node.Value.Equals(node2.Value)) continue;
                if (Vector3.Distance(node.Value.point, node2.Value.point) > s) continue;
                if (node.Value.Streams.Count > 2 || node2.Value.Streams.Count > 2) continue;

                CreateStream(node.Value, node2.Value);
            }
        }
        Console.WriteLine(" ... Done!");
    }

    private void IsolatedNodePrevention()
    {
        Console.Write("Node Isolation Prevention");
        foreach (KeyValuePair<int, Node> node in Nodes)
        {
            if (node.Value.Streams.Count > 0) continue;
            Node tempNode = node.Value;
            double distance = double.MaxValue;
            //simple sort to find closest node
            foreach (KeyValuePair<int, Node> node2 in Nodes)
            {
                //skipping the origin node
                if (node.Equals(node2)) continue;

                double tempDistance = Vector3.Distance(node.Value.point, node2.Value.point);
                //if the distance is more than the current lest, skip
                if (tempDistance > distance) continue;
                tempNode = node2.Value;
                distance = tempDistance;
            }
            CreateStream(node.Value, tempNode);
        }
        Console.WriteLine(" ... Done!");
    }

    private void IsolatedClusterPrevention()
    {
        Console.Write("Isolated Cluster Prevention ");
        List<Node> cluster = new();
        while (true)
        {
            cluster = FloodCluster(Nodes[rand.Next(0, Nodes.Count)]);
            if (cluster.Count == Nodes.Count) break; //It only goes past this point if there is an isolated cluster

            List<Node> Isolatedcluster = new();
            while(Isolatedcluster.Count == 0)
            {
                int i = rand.Next(0, Nodes.Count);
                if (cluster.Contains(Nodes[i])) continue;
                Isolatedcluster.Add(Nodes[i]);
            }
            //will never trigger?
            if (Isolatedcluster.Count == 0)
                Isolatedcluster = FloodCluster(Isolatedcluster[0]);
            
            List<Stream> potentialStreams = new();
            foreach (Node node in cluster)
            {   //creates streams between the clusters
                Isolatedcluster.ForEach(node2 => potentialStreams.Add(new(node, node2)));
            }
            //Limits the new streams to a 5th of the smaller cluster.
            Stream[] newStreams = new Stream[(int)Math.Ceiling((double)Math.Min(cluster.Count, Isolatedcluster.Count) / 5)];
            //Keeps track of what nodes already has new streams
            //In hopes of limiting chokepoints
            List<Node> NodeRecord = new();
            for (int i = 0; i < newStreams.Length; i++)
            {
                newStreams[i] = potentialStreams[0];
                foreach (Stream stream in potentialStreams)
                {   //if the potential new stream is smaller than the current new stream and it isn't connected to a current node 
                    if (newStreams[i].Distance > stream.Distance && !(NodeRecord.Contains(stream.root0) || NodeRecord.Contains(stream.root1)))
                    {
                        newStreams[i] = stream;
                    }
                }
                NodeRecord.Add(newStreams[i].root0);
                NodeRecord.Add(newStreams[i].root1);
            }
            foreach (Stream stream in newStreams) //creates the new streams connecting the clusters
            {
                CreateStream(stream.root0, stream.root1);
            }
            Console.Write(".");
        }
        Console.WriteLine(" Done!");
    }


    private List<Node> FloodCluster(Node seed)
    {
        List<Node> cluster = new();
        cluster.Add(seed);
        while (true)
        {
            int clusterSize = cluster.Count; //Fills the cluster with stars
            List<Node> clusterAdditions = new();
            foreach (Node star in cluster)
            {
                foreach (Stream stream in star.Streams)
                {
                    Node tempStar = stream.GetOpposite(star);
                    if (!cluster.Contains(tempStar) && !clusterAdditions.Contains(tempStar)) clusterAdditions.Add(tempStar);
                }
            }
            clusterAdditions.ForEach(node => cluster.Add(node));
            if (cluster.Count == clusterSize) break;
        }
        return cluster;
    }
}
