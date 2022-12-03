// LTRLib.IO.SafeBinaryReader
using System.IO;
using System.Text;

namespace LTRLib.IO;
public class SafeBinaryReader : BinaryReader
{
    public SafeBinaryReader(Stream input)
        : base(new SafeReadStream(input))
    {
    }

    public SafeBinaryReader(Stream input, Encoding encoding)
        : base(new SafeReadStream(input), encoding)
    {
    }
}
