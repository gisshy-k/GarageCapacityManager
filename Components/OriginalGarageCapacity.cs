using Colossal.Serialization.Entities;
using Unity.Entities;

namespace GarageCapacityManager.Components
{
    /// <summary>
    /// Custom component used to backup the original vanilla vehicle capacity.
    /// Implements ISerializable to ensure the true vanilla baseline survives save/load cycles,
    /// strictly satisfying the Vanilla Fallback Principle.
    /// </summary>
    public struct OriginalGarageCapacity : IComponentData, IQueryTypeParameter, ISerializable
    {
        public ushort VanillaCapacity;

        // セーブ時に実行され、本来のバニラ値をセーブファイルに焼き付ける
        public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
        {
            writer.Write(VanillaCapacity);
        }

        // ロード時に実行され、セーブファイルから本来のバニラ値を復元する
        public void Deserialize<TReader>(TReader reader) where TReader : IReader
        {
            reader.Read(out VanillaCapacity);
        }
    }
}