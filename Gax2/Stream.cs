using System;
using System.Numerics;
public struct Stream : IEquatable<Stream>
{
    public readonly Node root0, root1;
    public double Strength { get; set; } //how hard/easy it is to travers. a distance multiplier
    public double Distance { get; set; }
    public bool Direction { get; set; } //true is towards root 0, false is towards root 1.
    public double Flow { get; set; } //How much direction affects travesal rate

    public Stream(Node p1, Node p2)
    {
        if (p1.key < p2.key)
        {
            root0 = p1;
            root1 = p2;
        }
        else
        {
            root0 = p2;
            root1 = p1;
        }
        Strength = 1.0f;
        Direction = false;
        Flow = 1.0f;
        Distance = Vector3.Distance(root0.point, root1.point);
    }

    public Node GetOpposite(Node n)
    {
        if (n.Equals(root0)) return root1;
        if (n.Equals(root1)) return root0;
        throw new("Stream unconneted to Node");
    }

    public override int GetHashCode()
    {
        return ((root0.key + 1) * (root1.key + 1) * 6967) - 1;
    }

    public override bool Equals(object obj)
    {
        return obj is Stream && Equals((Stream)obj);
    }

    public bool Equals(Stream l)
    {
        return root0.Equals(l.root0) && root1.Equals(l.root1);
    }

    public override string ToString() => $"Roots: Node {root0.key}, Node {root1.key}\nDistance: {Distance}\nDirection: {Direction}\nFlow: {Flow}\nStrength: {Strength}";
}