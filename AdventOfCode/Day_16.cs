using System.Collections;

namespace AdventOfCode;

public class Day_16 : BaseDay
{
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
        var b = (new PacketDecoder("38006F45291200".HexStringToBitArray()).Parse() as OperationalValue).Values.Length == 2;
        var c = (new PacketDecoder("EE00D40C823060".HexStringToBitArray()).Parse() as OperationalValue).Values.Length == 3;
        var d = new PacketDecoder("8A004A801A8002F478".HexStringToBitArray()).Parse() as OperationalValue;
    }

    public override ValueTask<string> Solve_1() => new(new PacketDecoder(_parsedInput).Parse().VersionSum().ToString());

    public override ValueTask<string> Solve_2() => new(string.Empty);


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
            var packetType = Read(3);

            return packetType switch
            {
                4 => ParseLiteralValue(version, packetType),
                _ => ParseOperatorValue(version, packetType),
            };
        }

        private BasePacket ParseOperatorValue(int version, int packetType)
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
            // IDK

            return new OperationalValue(version, packetType, packages.ToArray(), 0);
        }

        private LiteralValue ParseLiteralValue(int version, int packetType)
        {
            var literal = 0;

            bool moreDigits;
            do
            {
                moreDigits = Read() == 1;
                literal <<= 4;
                literal |= Read(4);
            } while (moreDigits);


            //var ignore = Read(3);
            var ignore = 0;
            return new LiteralValue(version, packetType, literal, ignore);
        }
    }

    public abstract record BasePacket(int Version, int TypeId)
    {
        public abstract int VersionSum();
    }

    public record LiteralValue(int Version, int TypeId, int literal, int ignore) : BasePacket(Version, TypeId)
    {
        public override int VersionSum()
            => Version;
    }

    public record OperationalValue(int Version, int TypeId, BasePacket[] Values, int ignore) : BasePacket(Version, TypeId)
    {
        public override int VersionSum()
            => Version + Values.Sum(x => x.VersionSum());
    }
}