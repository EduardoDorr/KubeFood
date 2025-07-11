using System.Text;

using HashidsNet;

namespace Core.Helpers;

public static class HashIdHelper
{
    /// <summary>
    /// Encode a hash Id from a given integer Id.
    /// </summary>
    /// <param name="id">The integer Id to hash.</param>
    /// <returns>A string representing the hashed Id.</returns>
    public static string EncodeId(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");

        var hashid = new Hashids("your-salt", 8);

        return hashid.Encode(id);
    }

    /// <summary>
    /// Encode a hash Id from a given integer Id.
    /// </summary>
    /// <param name="id">The integer Id to hash.</param>
    /// <returns>A string representing the hashed Id.</returns>
    public static string EncodeId(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));

        var hashid = new Hashids("your-salt", 8);

        var bytes = Encoding.UTF8.GetBytes(id);
        var number = BitConverter.ToInt32(bytes, 0);

        return hashid.Encode(number);
    }

    /// <summary>
    /// Decode an integer Id from a given hash Id.
    /// </summary>
    /// <param name="hashId">The sting hashed Id to decode to an integer.</param>
    /// <returns>A integer Id.</returns>
    public static int DecodeHashId(string hashId)
    {
        var hashid = new Hashids("your-salt", 8);
        var decodedId = hashid.Decode(hashId).FirstOrDefault();

        if (decodedId <= 0)
            throw new ArgumentOutOfRangeException(nameof(hashId), "Id must be greater than zero.");

        return decodedId;
    }
}