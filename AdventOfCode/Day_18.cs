using System.Text.Json;

namespace AdventOfCode;

public class Day_18 : BaseDay
{
    private readonly string _input;
    private readonly BinaryTree<long>[] _mathHomework;

    public Day_18()
    {
        _input = File.ReadAllText(InputFilePath);

        _mathHomework = _input.SplitNewLine().Select(ParseBinaryTree).ToArray();
    }

    public void UseTests()
    {
        var a = Equals(PrintBinaryTree(ReduceNumber(ParseBinaryTree("[[[[[9,8],1],2],3],4]"))), "[[[[0,9],2],3],4]");
        var b = Equals(PrintBinaryTree(ReduceNumber(ParseBinaryTree("[7,[6,[5,[4,[3,2]]]]]"))), "[7,[6,[5,[7,0]]]]");
        var c = Equals(PrintBinaryTree(ReduceNumber(ParseBinaryTree("[[6,[5,[4,[3,2]]]],1]"))), "[[6,[5,[7,0]]],3]");
        var d = Equals(PrintBinaryTree(ReduceNumber(ParseBinaryTree("[[3,[2,[1,[7,3]]]],[6,[5,[4,[3,2]]]]]"))), "[[3,[2,[8,0]]],[9,[5,[7,0]]]]");
        var e = Equals(PrintBinaryTree(ReduceNumber(ParseBinaryTree("[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]"))), "[[3,[2,[8,0]]],[9,[5,[7,0]]]]");
        var f = Equals(PrintBinaryTree(ReduceNumber(ParseBinaryTree("[[[[[4,3],4],4],[7,[[8,4],9]]],[1,1]]"))), "[[[[0,7],4],[[7,8],[6,0]]],[8,1]]");
    }

    private BinaryTree<long> ParseBinaryTree(string arg)
    {
        var doc = JsonDocument.Parse(arg);
        var tree = new BinaryTree<long>
        {
            Root = ParseBinaryTreeRecurse<long>(doc.RootElement),
        };
        tree.RebuildNodes();
        return tree;
    }

    private BinaryTreeNode<long> ParseBinaryTreeRecurse<T>(JsonElement docRootElement, BinaryTreeNode<long> parent = null, int depth = 0)
    {
        var tree = new BinaryTreeNode<long>
        {
            Depth = depth,
            Parent = parent,
        };
        if (docRootElement.ValueKind == JsonValueKind.Array)
        {
            tree.Left = ParseBinaryTreeRecurse<T>(docRootElement[0], tree, depth + 1);
            tree.Right = ParseBinaryTreeRecurse<T>(docRootElement[1], tree, depth + 1);
        }
        else
        {
            tree.Value = docRootElement.GetInt32();
        }

        return tree;
    }

    private string PrintBinaryTree<T>(BinaryTree<T> tree)
        => PrintBinaryTreeRecurse(tree?.Root);

    private string PrintBinaryTreeRecurse<T>(BinaryTreeNode<T> node)
    {
        if (node == default)
        {
            return string.Empty;
        }
        if (node.Left == default && node.Right == default)
        {
            return node.Value.ToString();
        }

        return $"[{PrintBinaryTreeRecurse(node.Left)},{PrintBinaryTreeRecurse(node.Right)}]";
    }

    public override ValueTask<string> Solve_1()
    {
        BinaryTree<long> emptyBinaryTree = null;
        var result = _mathHomework.Aggregate(emptyBinaryTree, (curr, next) =>
        {
            return curr == null ? next : ConcatTree(curr, next);
        });
        return new ValueTask<string>(CalculateMagnitude(result).ToString());
    }

    private long CalculateMagnitude(BinaryTree<long> result)
        => CalculateMagnitude(result.Root);

    private long CalculateMagnitude(BinaryTreeNode<long> resultRoot)
    {
        if (resultRoot.IsValueNode)
        {
            return resultRoot.Value;
        }

        return CalculateMagnitude(resultRoot.Left) * 3 + CalculateMagnitude(resultRoot.Right) * 2;
    }

    private BinaryTree<long> ConcatTree(BinaryTree<long> left, BinaryTree<long> right)
    {
        var nodes = new List<BinaryTreeNode<long>>();
        nodes.AddRange(left.Nodes);
        nodes.AddRange(right.Nodes);
        var tree = new BinaryTree<long>
        {
            Nodes = nodes,
            Root = new BinaryTreeNode<long>
            {
                Left = left.Root,
                Right = right.Root,
            },
        };
        tree.ResetDepth();
        tree.RebuildNodes();
        ReduceNumber(tree);
        return tree;
    }

    private BinaryTree<long> ReduceNumber(BinaryTree<long> input)
    {
        while (true)
        {
            if (ApplyExplode(input) || ApplySplit(input))
            {
                continue;
            }

            //Done
            break;
        }

        return input;
    }

    private bool ApplySplit(BinaryTree<long> input)
    {
        var nodeOfValueGt = input.Nodes.FirstOrDefault(x => x.IsValueNode && x.Value >= 10);

        if (nodeOfValueGt != default)
        {
            var indexNodeOfValueGt = input.Nodes.IndexOf(nodeOfValueGt);

            nodeOfValueGt.Left = new BinaryTreeNode<long>
            {
                Value = (long)Math.Floor(nodeOfValueGt.Value / 2.0),
                Parent = nodeOfValueGt,
                Depth = nodeOfValueGt.Depth + 1,
            };
            nodeOfValueGt.Right = new BinaryTreeNode<long>
            {
                Value = (long)Math.Ceiling(nodeOfValueGt.Value / 2.0),
                Parent = nodeOfValueGt,
                Depth = nodeOfValueGt.Depth + 1,
            };
            nodeOfValueGt.Value = 0;
            input.Nodes.Insert(indexNodeOfValueGt + 1, nodeOfValueGt.Right);
            input.Nodes.Insert(indexNodeOfValueGt + 1, nodeOfValueGt.Left);
            return true;
        }

        return false;
    }

    private bool ApplyExplode(BinaryTree<long> input)
    {
        var nodeOfDepth4 = input.Nodes.FirstOrDefault(x => x.Depth == 4 && !x.IsValueNode);

        if (nodeOfDepth4 != default)
        {
            //input.RebuildNodes();
            var indexOfNode4 = input.Nodes.IndexOf(nodeOfDepth4);

            var leftNodeOf = input.Nodes.GetRange(0, indexOfNode4).LastOrDefault(x => x.IsValueNode);
            //.Take(indexOfNode4).LastOrDefault(x => x.IsValueNode); //TakeWhile(x => x != nodeOfDepth4).LastOrDefault(x => x.IsValueNode);//GetRange(0, indexOfNode4).LastOrDefault(x => x.IsValueNode);
            var rightNodeOf = input.Nodes.GetRange(indexOfNode4 + 3, input.Nodes.Count - indexOfNode4 - 3).FirstOrDefault(x => x.IsValueNode);
            //.Skip(indexOfNode4).Where(x => x != nodeOfDepth4.Left && x != nodeOfDepth4.Right).FirstOrDefault(x => x.IsValueNode); //GetRange(indexOfNode4 + 2, input.Nodes.Count - indexOfNode4 - 3).FirstOrDefault(x => x.IsValueNode);

            if (leftNodeOf != default)
            {
                leftNodeOf.Value += nodeOfDepth4.Left.Value;
            }

            if (rightNodeOf != default)
            {
                rightNodeOf.Value += nodeOfDepth4.Right.Value;
            }

            if (nodeOfDepth4.Left != default)
            {
                input.Nodes.Remove(nodeOfDepth4.Left);
                nodeOfDepth4.Left = default;
            }

            if (nodeOfDepth4.Right != default)
            {
                input.Nodes.Remove(nodeOfDepth4.Right);
                nodeOfDepth4.Right = default;
            }

            nodeOfDepth4.Value = 0;

            return true;
        }

        return false;
    }

    public override ValueTask<string> Solve_2() => new(string.Empty);

    public class BinaryTree<T>
    {
        public BinaryTreeNode<T> Root { get; set; }

        public List<BinaryTreeNode<T>> Nodes { get; set; } = new();

        public void RebuildNodes()
        {
            List<BinaryTreeNode<T>> nodes = new();
            AddNodes(nodes, Root);
            Nodes = nodes;
        }

        public void ResetDepth()
        {
            UpdateDepth(Root, 0);
        }

        private void UpdateDepth(BinaryTreeNode<T> root, int depth)
        {
            if (root == null)
            {
                return;
            }

            root.Depth = depth;
            UpdateDepth(root.Left, depth + 1);
            UpdateDepth(root.Right, depth + 1);
        }

        private void AddNodes(List<BinaryTreeNode<T>> nodes, BinaryTreeNode<T> root)
        {
            if (root == null)
            {
                return;
            }

            nodes.Add(root);
            AddNodes(nodes, root.Left);
            AddNodes(nodes, root.Right);
        }
    }

    public class BinaryTreeNode<T>
    {
        public BinaryTreeNode<T> Left { get; set; }
        public BinaryTreeNode<T> Right { get; set; }

        public T Value { get; set; }

        public int Depth { get; set; } = 1;

        public BinaryTreeNode<T> Parent { get; set; }

        public bool IsValueNode => Left == default && Right == default;
    }
}