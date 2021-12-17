using System.Collections;

namespace AdventOfCode;

public class Day_16 : BaseDay
{
    public enum TypeId
    {
        Sum = 0,
        Product = 1,
        Minimum = 2,
        Maximum = 3,
        Litteral = 4,
        GreaterThan = 5,
        LessThan = 6,
        EqualTo = 7,
    }

    private readonly string _input;
    private readonly BitArray _parsedInput;

    public Day_16()
    {
        _input = File.ReadAllText(InputFilePath);

        _parsedInput = _input.HexStringToBitArray();
    }

    private void UseTests()
    {
        var a = (new PacketDecoder("D2FE28".HexStringToBitArray()).Parse() as LiteralValue).literal == 2021;
        var b = (new PacketDecoder("38006F45291200".HexStringToBitArray()).Parse() as OperationalValue).SubPackets.Length == 2;
        var c = (new PacketDecoder("EE00D40C823060".HexStringToBitArray()).Parse() as OperationalValue).SubPackets.Length == 3;
        var d = new PacketDecoder("8A004A801A8002F478".HexStringToBitArray()).Parse() as OperationalValue;
    }

    public override ValueTask<string> Solve_1() => new(new PacketDecoder(_parsedInput).Parse().VersionSum().ToString());

    public override ValueTask<string> Solve_2() => new(new PacketDecoder(_parsedInput).Parse().Value().ToString());


    public class PacketDecoder
    {
        private readonly BitArray _bitArray;
        private int _position;

        public PacketDecoder(BitArray bitArray)
        {
            _bitArray = bitArray;
        }

        public int Read(int amount = 1)
        {
            var result = 0;
            for (var i = 0; i < amount; i++)
            {
                result <<= 1;
                if (_bitArray.Get(_position + i))
                {
                    result |= 1;
                }
            }

            _position += amount;
            return result;
        }

        public BasePacket Parse()
        {
            var version = Read(3);
            var packetType = (TypeId)Read(3);

            return packetType switch
            {
                TypeId.Litteral => ParseLiteralValue(version, packetType),
                _ => ParseOperatorValue(version, packetType),
            };
        }

        private BasePacket ParseOperatorValue(int version, TypeId packetType)
        {
            var lengthTypeId = Read();

            var packages = new List<BasePacket>();

            if (lengthTypeId == 0)
            {
                var subPacketsLengthToRead = 15;
                var subPacketsLength = Read(subPacketsLengthToRead);
                var maxPos = _position + subPacketsLength;
                while (_position < maxPos)
                {
                    packages.Add(Parse());
                }
            }
            else
            {
                var subPacketsLengthToRead = 11;
                var subPacketsAmount = Read(subPacketsLengthToRead);
                for (var i = 0; i < subPacketsAmount; i++)
                {
                    packages.Add(Parse());
                }
            }

            return new OperationalValue(version, packetType, packages.ToArray());
        }

        private LiteralValue ParseLiteralValue(int version, TypeId packetType)
        {
            var literal = 0l;

            bool moreDigits;
            do
            {
                moreDigits = Read() == 1;
                literal <<= 4;
                literal |= Read(4);
            } while (moreDigits);


            //var ignore = Read(3);
            var ignore = 0;
            return new LiteralValue(version, packetType, literal);
        }
    }

    public abstract record BasePacket(int Version, TypeId TypeId)
    {
        public abstract int VersionSum();

        public abstract long Value();
    }

    public record LiteralValue(int Version, TypeId TypeId, long literal) : BasePacket(Version, TypeId)
    {
        public override int VersionSum()
            => Version;

        public override long Value()
            => literal;
    }

    public record OperationalValue(int Version, TypeId TypeId, BasePacket[] SubPackets) : BasePacket(Version, TypeId)
    {
        public override int VersionSum()
            => Version + SubPackets.Sum(x => x.VersionSum());

        public override long Value()
            => TypeId switch
            {
                TypeId.Sum => SubPackets.Sum(x => x.Value()),
                TypeId.Product => SubPackets.Aggregate(1l, (curr, next) => curr * next.Value()),
                TypeId.Minimum => SubPackets.Min(x => x.Value()),
                TypeId.Maximum => SubPackets.Max(x => x.Value()),
                //TypeId.Litteral =>x,
                TypeId.GreaterThan => SubPackets[0].Value() > SubPackets[1].Value() ? 1 : 0,
                TypeId.LessThan => SubPackets[0].Value() < SubPackets[1].Value() ? 1 : 0,
                TypeId.EqualTo => SubPackets[0].Value() == SubPackets[1].Value() ? 1 : 0,
                _ => throw new ArgumentOutOfRangeException(),
            };
    }
}