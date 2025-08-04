using System.Text;

using HashidsNet;

using MongoDB.Bson;

namespace KubeFood.Core.Helpers;

public static class HashIdHelper
{
    private static readonly Hashids _hashids = new("your-salt", 8);

    /// <summary>
    /// Encode a hash Id from a given integer Id.
    /// </summary>
    /// <param name="id">The integer Id to hash.</param>
    /// <returns>A string representing the hashed Id.</returns>
    public static string EncodeId(this int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");

        return _hashids.Encode(id);
    }

    /// <summary>
    /// Encode a hash Id from a given ObjectId.
    /// </summary>
    /// <param name="id">The ObjectId to hash.</param>
    /// <returns>A string representing the hashed Id.</returns>
    public static string EncodeId(this ObjectId id)
    {
        var bytes = id.ToByteArray();

        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }

    /// <summary>
    /// Encode a hash Id from a given string Id.
    /// </summary>
    /// <param name="id">The string Id to hash.</param>
    /// <returns>A string representing the hashed Id.</returns>
    public static string EncodeId(this string id)
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
    public static int DecodeIntegerId(this string hashId)
    {
        var decodedId = _hashids.Decode(hashId).FirstOrDefault();

        if (decodedId <= 0)
            throw new ArgumentOutOfRangeException(nameof(hashId), "Id must be greater than zero.");


        return decodedId;
    }

    /// <summary>
    /// Decode an integer Id from a given hash Id.
    /// </summary>
    /// <param name="hashId">The sting hashed Id to decode to an string.</param>
    /// <returns>A string Id.</returns>
    public static string DecodeStringId(this string hashId)
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

    /// <summary>
    /// Decode an string Id from a given hash Id.
    /// </summary>
    /// <param name="hashId">The string hashed Id to decode to an ObjectId.</param>
    /// <returns>A ObjectId.</returns>
    public static ObjectId DecodeObjectId(this string hashId)
    {
        var base64 = hashId
            .Replace("-", "+")
            .Replace("_", "/");

        switch (base64.Length % 4)
        {
            case 2:
                base64 += "==";
                break;
            case 3:
                base64 += "=";
                break;
        }

        var bytes = Convert.FromBase64String(base64);

        return new ObjectId(bytes);
    }
}