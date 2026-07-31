using System;
using System.Buffers;
using System.Text;

namespace OnnxRuntimeSharp.Test;

static class TestOnnxModels
{
    public static byte[] TwoInputTwoOutput { get; } = CreateTwoInputTwoOutput();

    static byte[] CreateTwoInputTwoOutput()
    {
        var model = new ProtoWriter();
        model.WriteInt64(1, 8);
        model.WriteString(2, "OnnxRuntimeSharp.Test");
        model.WriteString(4, "test");
        model.WriteInt64(5, 1);
        model.WriteString(6, "Two independent identity operations.");
        model.WriteMessage(7, graph =>
        {
            graph.WriteMessage(1, node => WriteIdentityNode(node, "first", "first_output"));
            graph.WriteMessage(1, node => WriteIdentityNode(node, "second", "second_output"));
            graph.WriteString(2, "TwoInputTwoOutput");
            graph.WriteMessage(11, value => WriteFloatValueInfo(value, "first"));
            graph.WriteMessage(11, value => WriteFloatValueInfo(value, "second"));
            graph.WriteMessage(12, value => WriteFloatValueInfo(value, "first_output"));
            graph.WriteMessage(12, value => WriteFloatValueInfo(value, "second_output"));
        });
        model.WriteMessage(8, opset => opset.WriteInt64(2, 13));
        model.WriteMessage(14, metadata =>
        {
            metadata.WriteString(1, "purpose");
            metadata.WriteString(2, "coverage");
        });
        return model.ToArray();
    }

    static void WriteIdentityNode(ProtoWriter node, string input, string output)
    {
        node.WriteString(1, input);
        node.WriteString(2, output);
        node.WriteString(4, "Identity");
    }

    static void WriteFloatValueInfo(ProtoWriter valueInfo, string name)
    {
        valueInfo.WriteString(1, name);
        valueInfo.WriteMessage(2, type =>
            type.WriteMessage(1, tensor =>
            {
                tensor.WriteInt64(1, 1);
                tensor.WriteMessage(2, shape =>
                    shape.WriteMessage(1, dimension => dimension.WriteInt64(1, 1)));
            }));
    }

    sealed class ProtoWriter
    {
        readonly ArrayBufferWriter<byte> _buffer = new();

        public void WriteInt64(int fieldNumber, long value)
        {
            WriteTag(fieldNumber, 0);
            WriteVarint(unchecked((ulong)value));
        }

        public void WriteString(int fieldNumber, string value)
        {
            WriteTag(fieldNumber, 2);
            var byteCount = Encoding.UTF8.GetByteCount(value);
            WriteVarint((ulong)byteCount);
            Encoding.UTF8.GetBytes(value, _buffer.GetSpan(byteCount));
            _buffer.Advance(byteCount);
        }

        public void WriteMessage(int fieldNumber, Action<ProtoWriter> write)
        {
            var message = new ProtoWriter();
            write(message);
            var bytes = message._buffer.WrittenSpan;
            WriteTag(fieldNumber, 2);
            WriteVarint((ulong)bytes.Length);
            bytes.CopyTo(_buffer.GetSpan(bytes.Length));
            _buffer.Advance(bytes.Length);
        }

        public byte[] ToArray() => _buffer.WrittenSpan.ToArray();

        void WriteTag(int fieldNumber, int wireType) =>
            WriteVarint((ulong)((fieldNumber << 3) | wireType));

        void WriteVarint(ulong value)
        {
            do
            {
                var next = (byte)(value & 0x7f);
                value >>= 7;
                if (value != 0)
                {
                    next |= 0x80;
                }
                _buffer.GetSpan(1)[0] = next;
                _buffer.Advance(1);
            }
            while (value != 0);
        }
    }
}
