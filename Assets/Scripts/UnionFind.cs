
public class UnionFind
{
    int[] parent;

    public UnionFind(int count)
    {
        parent = new int[count];
        MakeSet(count);
    }

    public void MakeSet(int count)
    {
        for (int i = 0; i < count; i++)
            parent[i] = i;
    }

    public int Find(int x) 
    { 
        while (x != parent[x])
        {
            parent[x] = parent[parent[x]];
            x = parent[x];
        }
        return x;
    }

    public void Union(int a, int b)
    {
        a = Find(a);
        b = Find(b);

        if (a == b)
            return;

        parent[b] = a;
    }
}
