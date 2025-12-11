namespace Helpers;

public static class Search
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    /// <param name="h"></param>
    /// <param name="compare">Compares two nodes for equality.</param>
    /// <param name="getNeighbors"></param>
    /// <param name="edgeWeight">Returns the weight of the edge between two nodes. (Default: 1)</param>
    /// <returns></returns>
    public static List<T> AStar<T>(T start, T goal, Func<T, long> h, Func<T, T, bool> compare, Func<T, IEnumerable<T>> getNeighbors, Func<T, T, long>? edgeWeight = null, IEqualityComparer<T>? equalityComparer = null) where T : notnull
    {
        return AStar(start, state => compare(state, goal), h, compare, getNeighbors, edgeWeight, equalityComparer);
    }

    public static List<T> AStar<T>(T start, Func<T, bool> goalReached, Func<T, long> h, Func<T, T, bool> compare, Func<T, IEnumerable<T>> getNeighbors, Func<T, T, long>? edgeWeight = null, IEqualityComparer<T>? equalityComparer = null) where T : notnull
    {
        if (edgeWeight == null)
        {
            edgeWeight = (_, _) => 1;
        }

        List<T> openSet = new() { start };
        Dictionary<T, T> cameFrom = new(equalityComparer);
        Dictionary<T, long> gScore = new(equalityComparer) { { start, 0 } };
        Dictionary<T, long> fScore = new(equalityComparer) { { start, h(start) } };

        long getGScore(T pos)
        {
            if (gScore.TryGetValue(pos, out var value))
            {
                return value;
            }
            return int.MaxValue;
        }
        long getFScore(T pos)
        {
            if (fScore.TryGetValue(pos, out var value))
            {
                return value;
            }
            return int.MaxValue;
        }

        while (openSet.Count > 0)
        {
            var current = openSet.MinBy(n => getFScore(n));
            if (goalReached(current))
            {
                List<T> path = new() { current };
                while (cameFrom.Keys.Any(n => compare(n, current)))
                {
                    current = cameFrom[current];
                    path.Add(current);
                }
                path.Reverse();
                return path;
            }

            openSet.Remove(current);
            foreach (var neighbor in getNeighbors(current))
            {
                var tentative_gScore = getGScore(current) + edgeWeight(current, neighbor);
                if (tentative_gScore < getGScore(neighbor))
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentative_gScore;
                    fScore[neighbor] = tentative_gScore + h(neighbor);
                    if (!openSet.Any(n => compare(n, neighbor)))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
        return new();
    }

    public static (List<T> Goals, Dictionary<T, List<T>> CameFrom) AStarAll<T>(T start, Func<T, bool> goalReached, Func<T, long> h, Func<T, T, bool> compare, Func<T, IEnumerable<T>> getNeighbors, Func<T, T, long>? edgeWeight = null) where T : notnull
    {
        if (edgeWeight == null)
        {
            edgeWeight = (_, _) => 1;
        }

        List<T> openSet = new() { start };
        ValueCreationDictionary<T, List<T>> cameFrom = new();
        Dictionary<T, long> gScore = new() { { start, 0 } };
        Dictionary<T, long> fScore = new() { { start, h(start) } };

        long getGScore(T pos)
        {
            if (gScore.TryGetValue(pos, out var value))
            {
                return value;
            }
            return int.MaxValue;
        }
        long getFScore(T pos)
        {
            if (fScore.TryGetValue(pos, out var value))
            {
                return value;
            }
            return int.MaxValue;
        }

        List<T> goals = [];
        long? minScore = null;

        while (openSet.Count > 0)
        {
            var current = openSet.MinBy(n => getFScore(n));
            if (goalReached(current))
            {
                var score = getFScore(current);
                if (minScore is null || score == minScore)
                {
                    minScore = score;
                    goals.Add(current);
                }
                else if (minScore is not null && score > minScore)
                {
                    return (goals, cameFrom);
                }
            }

            openSet.Remove(current);
            foreach (var neighbor in getNeighbors(current))
            {
                var tentative_gScore = getGScore(current) + edgeWeight(current, neighbor);
                var neighbor_gScore = getGScore(neighbor);
                if (tentative_gScore <= neighbor_gScore)
                {
                    if (tentative_gScore == neighbor_gScore)
                    {
                        cameFrom[neighbor].Add(current);
                    }
                    else
                    {
                        cameFrom[neighbor] = [current];
                    }

                    gScore[neighbor] = tentative_gScore;
                    fScore[neighbor] = tentative_gScore + h(neighbor);
                    if (!openSet.Any(n => compare(n, neighbor)))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
        return ([], []);
    }
}
