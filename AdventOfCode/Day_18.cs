﻿using System.Text.Json;

namespace AdventOfCode;

public class Day_18 : BaseDay
{
    private readonly string _input;
    private readonly List<BinaryTree<long>> _mathHomeworkPart1;
    private readonly List<(BinaryTree<long> A, BinaryTree<long> B)> _mathHomeworkPart2;


    public Day_18()
    {
        _input = File.ReadAllText(InputFilePath);

        var _rawMathHomework = _input.SplitNewLine();
        _mathHomeworkPart1 = _rawMathHomework.Select(ParseBinaryTree).ToList();

        _mathHomeworkPart2 = new List<(BinaryTree<long> A, BinaryTree<long> B)>();
        for (var i = 0; i < _rawMathHomework.Length; i++)
        {
            for (var y = 0; y < _rawMathHomework.Length; y++)
            {
                if (i != y)
                {
                    _mathHomeworkPart2.Add((ParseBinaryTree(_rawMathHomework[i]), ParseBinaryTree(_rawMathHomework[y])));
                }
            }
        }
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
        var result = _mathHomeworkPart1.Aggregate(emptyBinaryTree, static (curr, next) => curr == null ? next : ConcatTree(curr, next));
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

    private static BinaryTree<long> ConcatTree(BinaryTree<long> left, BinaryTree<long> right)
    {
        var tree = new BinaryTree<long>
        {
            Root = new BinaryTreeNode<long>
            {
                Left = left.Root,
                Right = right.Root,
            },
        };
        // nodes.AddRange(left.Nodes.Skip(1));
        tree.ResetDepth();
        tree.RebuildNodes();
        ReduceNumber(tree);
        return tree;
    }

    private static BinaryTree<long> ReduceNumber(BinaryTree<long> input)
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

    private static bool ApplySplit(BinaryTree<long> input)
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

    private static bool ApplyExplode(BinaryTree<long> input)
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

    public override ValueTask<string> Solve_2()
    {
        return new ValueTask<string>(_mathHomeworkPart2.Select(x => CalculateMagnitude(ConcatTree(x.A, x.B))).Max().ToString());
    }

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