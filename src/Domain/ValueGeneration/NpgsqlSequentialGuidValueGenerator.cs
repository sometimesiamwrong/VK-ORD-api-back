using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Buffers.Binary;
using System.Security.Cryptography;

namespace Domain.ValueGeneration
{
    public class NpgsqlSequentialGuidValueGenerator : ValueGenerator<Guid>
    {
        public override bool GeneratesTemporaryValues => false;

        public override Guid Next(EntityEntry entry) => UuidV7.Generate();
    }

    public static class UuidV7
    {
        public static Guid Generate()
        {
            Span<byte> bytes = stackalloc byte[16];

            RandomNumberGenerator.Fill(bytes);

            bytes[6] = (byte)((bytes[6] & 0x0F) | 0x70); // version 7

            bytes[8] = (byte)((bytes[8] & 0x3F) | 0x80); // variant 10

            ulong timestamp = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            BinaryPrimitives.WriteUInt32LittleEndian(bytes.Slice(0, 4), (uint)timestamp);

            BinaryPrimitives.WriteUInt16LittleEndian(bytes.Slice(4, 2), (ushort)(timestamp >> 32));

            // Clear the next 12 bits to 0
            bytes[6] &= 0xF0;
            bytes[7] = 0;

            return new Guid(bytes);
        }
    }
}
