using Google.Protobuf;

namespace Wst.Trios.Core.Protocol;

public static class BytesExtension
{
    public static ByteString AsByteString(this Guid guid) => ByteString.CopyFrom(guid.ToByteArray());

    public static Guid AsGuid(this ByteString byteString) => new(byteString.ToByteArray());
}
