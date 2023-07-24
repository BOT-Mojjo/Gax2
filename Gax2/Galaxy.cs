using System;
using System.Numerics;
public class Galaxy
{
    Random rand = new Random();
    public Dictionary<int, Node> Nodes; //Key is index of creation. starts at 0
    public Dictionary<int, Stream> Streams; //Key is the HashCode of Value Stream
    private byte[,] NodeNoiseMap;
    public Galaxy(int count, double StreamDistance, float size)
    {           //star count, maximum "autoconnect distance
        Vector3 center = new(size/2,size/2, 0);
        Nodes = new();
        Streams = new();
        NodeNoiseMap = new Byte[count/5,count/5];

        NoiseMapPreperation(NodeNoiseMap.GetLength(0));

        NodeGeneration(count, size);

    //     StreamGeneration(StreamDistance);

    //     IsolatedNodePrevention();

    //     IsolatedClusterPrevention();
    }

    void NoiseMapPreperation(int mapSize)
    {
        Vector2 center = new(mapSize/2, mapSize/2);
        float max = (mapSize/2)*0.95f;
        float min = (mapSize/2)*0.2f;
        for(int x = 0; x < mapSize; x++)
        {
            for(int y = 0; y < mapSize; y++)
            {
                float distance = Vector2.Distance(new(x,y), center);
                if(distance > max || distance < min) NodeNoiseMap[x,y] = 255;
            }
        }
    }

    public void NodeGeneration(int amount, float size)
    {   //TODO: something clearly isn't working here. Nodes generate too close to eachother
        Console.Write("Node Generation");
        double noiseMapScale = (amount/5)/size;
        int abstractTooClose = (int) ((amount/15f)/size);
        while(Nodes.Count < amount)
        {
            Vector3 newPoint = PointGeneration((int)size);
            (int x, int y) abstractPoint = ((int) (newPoint.X*noiseMapScale), (int) (newPoint.Y*noiseMapScale));
            if(NodeNoiseMap[abstractPoint.x, abstractPoint.y] > rand.Next(0,255))
            {
                continue;
            }

            int XMin = abstractPoint.x - abstractTooClose;
            if( XMin < 0) XMin = 0;

            int YMin = abstractPoint.y - abstractTooClose;
            if( YMin < 0) XMin = 0;

            int XMax = abstractPoint.x + abstractTooClose;
            if( XMax > NodeNoiseMap.GetLength(0)) XMax = 0;

            int YMax = abstractPoint.y + abstractTooClose;
            if( YMax > NodeNoiseMap.GetLength(1)) XMax = 0;


            for(int x = XMin; x < XMax; x++)
            {
                for(int y = YMin; y < YMax; y++)
                {
                    NodeNoiseMap[x, y] = 255;
                }   
            }
            int key = Nodes.Count+1;
            Nodes.Add(key, new(key, newPoint));
        }
        Console.WriteLine(" ... Done!");
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
                if (Vector3.Distance(node.Value.point, node2.Value.point) > s) continue;
                if (node.Value.Streams.Count > 2 || node2.Value.Streams.Count > 2) continue;
                if (node.Value.Equals(node2.Value)) continue;

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

            // List<Node> Isolatedcluster = new();
            // while(Isolatedcluster.Count == 0)
            // {
            //     int i = rand.Next(0, Nodes.Count);
            //     if (cluster.Contains(Nodes[i])) continue;
            //     Isolatedcluster.Add(Nodes[i]);
            // }
            // //will never trigger?
            // if (Isolatedcluster.Count == 0)
            //     Isolatedcluster = FloodCluster(Isolatedcluster[0]);
            
            List<Stream> potentialStreams = new();
            //can cause a System.OverflowException some how if the cluster size is too small????
            Stream[] newStreams = new Stream[(int)Math.Ceiling((double) cluster.Count/(cluster.Count/4))];
            
            //Limits the new streams.
            //can somehow result in an integer overflow exception??
            //Keeps track of what nodes already has new streams
            //In hopes of limiting chokepoints
            List<Node> NodeRecord = new();
            byte nullBreaker = 0;
            foreach (Node node in cluster)
            {   //creates streams between the clusters
                // Nodes.ForEach(node2 => potentialStreams.Add(new(node, node2)));
                foreach (KeyValuePair<int, Node> node2 in Nodes)
                {
                    if(node.Equals(node2.Value) || cluster.Contains(node2.Value)) continue;
                    Stream newStream = new(node, node2.Value);
                    for (int i = 0; i < newStreams.Length; i++)
                    {
                        if(nullBreaker < newStreams.Count())
                        {
                            newStreams[i] = newStream;
                            break;
                        }
                        if(newStream.Distance < newStreams[i].Distance && (!(NodeRecord.Contains(newStream.root0) || NodeRecord.Contains(newStream.root1))))
                        {
                            Stream tempStream = newStream;
                            newStream = newStreams[i];
                            newStreams[i] = tempStream;
                        }
                    }
                }
            }
            // for (int i = 0; i < newStreams.Length; i++)
            // {
            //     newStreams[i] = potentialStreams[0];
            //     foreach (Stream stream in potentialStreams)
            //     {   //if the potential new stream is smaller than the current new stream and it isn't connected to a current node 
            //         if (newStreams[i].Distance > stream.Distance && (!(NodeRecord.Contains(stream.root0) || NodeRecord.Contains(stream.root1))))
            //         {
            //             newStreams[i] = stream;
            //         }
            //     }
            //     NodeRecord.Add(newStreams[i].root0);
            //     NodeRecord.Add(newStreams[i].root1);
            // }
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

    private Vector3 PointGeneration(int mapSize) => new((float)rand.NextDouble() * mapSize, (float) rand.NextDouble() * mapSize, (float)rand.NextDouble() - 0.5f);
}
