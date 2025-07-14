using System.Text;

using HashidsNet;

namespace Core.Helpers;

public static class HashIdHelper
{
    private static readonly Hashids _hashids = new("your-salt", 8);

    /// <summary>
    /// Encode a hash Id from a given integer Id.
    /// </summary>
    /// <param name="id">The integer Id to hash.</param>
    /// <returns>A string representing the hashed Id.</returns>
    public static string EncodeId(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");

        return _hashids.Encode(id);
    }

    /// <summary>
    /// Encode a hash Id from a given integer Id.
    /// </summary>
    /// <param name="id">The integer Id to hash.</param>
    /// <returns>A string representing the hashed Id.</returns>
    public static string EncodeId(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));

        var bytes = Encoding.UTF8.GetBytes(id);
        var number = BitConverter.ToInt32(bytes, 0);

        return _hashids.Encode(number);
    }

    /// <summary>
    /// Decode an integer Id from a given hash Id.
    /// </summary>
    /// <param name="hashId">The sting hashed Id to decode to an integer.</param>
    /// <returns>A integer Id.</returns>
    public static int DecodeIntegerId(string hashId)
    {
        var decodedId = _hashids.Decode(hashId).FirstOrDefault();

        if (decodedId <= 0)
            throw new ArgumentOutOfRangeException(nameof(hashId), "Id must be greater than zero.");


        return decodedId;
    }

    /// <summary>
    /// Decode an integer Id from a given hash Id.
    /// </summary>
    /// <param name="hashId">The sting hashed Id to decode to an integer.</param>
    /// <returns>A integer Id.</returns>
    public static string DecodeStringId(string hashId)
    {
        var numbers = _hashids.Decode(hashId).FirstOrDefault();

        if (numbers <= 0)
            throw new ArgumentOutOfRangeException(nameof(hashId), "Id must be greater than zero.");

        var bytes = BitConverter.GetBytes(numbers);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        var decodedId = Encoding.UTF8.GetString(bytes);

        return decodedId;
    }
}