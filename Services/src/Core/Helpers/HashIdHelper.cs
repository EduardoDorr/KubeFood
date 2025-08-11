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
    /// Decodes a hashed Id string into a strongly-typed value.
    /// Supports decoding to <see cref="int"/>, <see cref="string"/>, or <see cref="ObjectId"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The target type to decode to:
    /// <list type="bullet">
    /// <item><description><see cref="int"/> – Decodes directly to an integer Id.</description></item>
    /// <item><description><see cref="string"/> – Decodes an integer from the hash and converts it back to its original UTF-8 string representation.</description></item>
    /// <item><description><see cref="ObjectId"/> – Decodes from Base64-like hash into a MongoDB ObjectId.</description></item>
    /// </list>
    /// </typeparam>
    /// <param name="hashId">The hashed Id string to decode.</param>
    /// <returns>The decoded value of type <typeparamref name="T"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <param name="hashId"> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the decoded numeric Id is less than or equal to zero.</exception>
    /// <exception cref="NotSupportedException">Thrown when the target type <typeparamref name="T"/> is not supported.</exception>
    public static T DecodeHashId<T>(this string hashId)
    {
        ArgumentNullException.ThrowIfNull(hashId, nameof(hashId));

        if (typeof(T) == typeof(int))
        {
            var decodedId = _hashids.Decode(hashId).FirstOrDefault();
            if (decodedId <= 0)
                throw new ArgumentOutOfRangeException(nameof(hashId), "Id must be greater than zero.");

            return (T)(object)decodedId;
        }

        if (typeof(T) == typeof(string))
        {
            var numbers = _hashids.Decode(hashId).FirstOrDefault();
            if (numbers <= 0)
                throw new ArgumentOutOfRangeException(nameof(hashId), "Id must be greater than zero.");

            var bytes = BitConverter.GetBytes(numbers);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            var decodedId = Encoding.UTF8.GetString(bytes);
            return (T)(object)decodedId;
        }

        if (typeof(T) == typeof(ObjectId))
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
            var objectId = new ObjectId(bytes);

            return (T)(object)objectId;
        }

        throw new NotSupportedException($"Decode type is not supported: {typeof(T)}");
    }
}