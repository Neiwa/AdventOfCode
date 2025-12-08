using Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2025.Day08;

public class Day08 : BaseAocV2
{
    public record Network(int Id, HashSet<LongPoint3D> Nodes);

    public override object PartOne(List<string> lines)
    {
        var points = lines.Select(line => line.Split(',')).Select(arr => new LongPoint3D(long.Parse(arr[0]), long.Parse(arr[1]), long.Parse(arr[2]))).ToList();

        var tuples = points.SelfJoin((l, r) => (l, r, Math.Sqrt(Math.Pow(l.X - r.X, 2) + Math.Pow(l.Y - r.Y, 2) + Math.Pow(l.Z - r.Z, 2))));

        var sortedTuples = tuples.OrderBy(t => t.Item3).ToList();

        List<Network> networks = [];
        int network = 0;

        var limit = this.FileName == "example.txt" ? 10 : 1_000;

        for (int i = 0; i < limit; i++)
        {
            var (p1, p2, d) = sortedTuples[i];

            WriteLine($"{p1.X},{p1.Y},{p1.Z} =+= {p2.X},{p2.Y},{p2.Z} ({d})");

            var ns = networks.Where(n => n.Nodes.Contains(p1) || n.Nodes.Contains(p2)).ToList();

            switch (ns.Count)
            {
                case 0:
                    WriteLine("New network");
                    networks.Add(new Network(++network, [p1, p2]));
                    break;
                case 1:
                    WriteLine($"Network {ns[0].Id}");
                    ns[0].Nodes.AddRange([p1, p2]);
                    break;
                case 2:
                    {
                        WriteLine($"Network {ns[0].Id} =+= Network {ns[1].Id}");
                        networks.Remove(ns[1]);
                        ns[0].Nodes.AddRange(ns[1].Nodes);
                        ns[0].Nodes.AddRange([p1, p2]);
                        break;
                    }
                default:
                    throw new InvalidOperationException();
            }
        }

        return networks.OrderByDescending(n => n.Nodes.Count).Take(3).Aggregate(1L, (l, r) => l * r.Nodes.Count);
    }

    public override object PartTwo(List<string> lines)
    {
        var points = lines.Select(line => line.Split(',')).Select(arr => (new LongPoint3D(long.Parse(arr[0]), long.Parse(arr[1]), long.Parse(arr[2])))).ToList();

        var tuples = points.SelfJoin((l, r) => (l, r, Math.Sqrt(Math.Pow(l.X - r.X, 2) + Math.Pow(l.Y - r.Y, 2) + Math.Pow(l.Z - r.Z, 2))));

        var sortedTuples = tuples.OrderBy(t => t.Item3).ToList();

        List<Network> networks = [];
        Dictionary<LongPoint3D, Network?> junctionBoxes = [];
        int network = 0;

        for (int i = 0; i < sortedTuples.Count; i++)
        {
            var (p1, p2, d) = sortedTuples[i];

            WriteLine($"{p1.X},{p1.Y},{p1.Z} =+= {p2.X},{p2.Y},{p2.Z} ({d})");

            var p1Network = junctionBoxes.GetValueOrDefault(p1, null);
            var p2Network = junctionBoxes.GetValueOrDefault(p2, null);

            if (p1Network is null && p2Network is null)
            {
                var n = new Network(++network, [p1, p2]);
                junctionBoxes[p1] = n;
                junctionBoxes[p2] = n;
                networks.Add(n);

                WriteLine($"New network {n.Id}", indent: 1);
            }
            else if (p1Network is not null && p2Network is not null)
            {
                networks.Remove(p2Network);
                foreach (var node in p2Network.Nodes.ToList())
                {
                    junctionBoxes[node] = p1Network;
                    p1Network.Nodes.Add(node);
                }
                WriteLine($"Merged network {p2Network.Id} into network {p1Network.Id}", indent: 1);
            }
            else if (p2Network is null && p1Network is not null)
            {
                junctionBoxes[p2] = p1Network;
                p1Network.Nodes.Add(p2);
                WriteLine($"Added to network {p1Network.Id}", indent: 1);
            }
            else if (p1Network is null && p2Network is not null)
            {
                junctionBoxes[p1] = p2Network;
                p2Network.Nodes.Add(p1);
                p1Network = p2Network;
                WriteLine($"Added to network {p2Network.Id}", indent: 1);
            }

            if (p1Network is not null && p1Network.Nodes.Count == lines.Count)
            {
                return p1.X * p2.X;
            }
        }

        return -1;
    }
}
