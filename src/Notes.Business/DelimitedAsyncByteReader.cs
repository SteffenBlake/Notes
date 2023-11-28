using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;

namespace Notes.Business;
    
public class DelimitedAsyncByteReader(Stream stream, byte delimiter = 0x0a)
{
    public async IAsyncEnumerable<byte[]> ReadAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var pipe = PipeReader.Create(stream);
        while (!cancellationToken.IsCancellationRequested)
        {
            var next = await pipe.ReadAsync(cancellationToken);

            Console.WriteLine("Ping");

            var (data, terminated, position) = TryParse(next.Buffer, delimiter);
            if (!data.Any())
            {
                continue;
            }

            foreach (var result in data)
            {
                yield return result;
            }
            if (terminated)
            {
                yield break;
            }

            pipe.AdvanceTo(position);
        }
    }

    private static ParseResult TryParse(
        in ReadOnlySequence<byte> data, 
        in byte delimiter
    )
    {
        var dataOut = new List<byte[]>();

        var reader = new SequenceReader<byte>(data);
        var terminated = false;
        while (!terminated && reader.TryReadToAny(out ReadOnlySpan<byte> sequence, [delimiter], true))
        {
            dataOut.Add(sequence.ToArray());
            terminated |= reader.TryPeek(out var peek) && peek == 0x04;
        }

        return new (dataOut, terminated, reader.Position);
    }

    private readonly record struct ParseResult(List<byte[]> data, bool terminated, SequencePosition position);
}

