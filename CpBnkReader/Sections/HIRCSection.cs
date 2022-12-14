using System;

namespace CpBnkReader;

public class HIRCSection : ISection
{
    private readonly BnkFile _parent;

    public HIRCSection(BnkFile parent)
    {
        _parent = parent;
    }

    public List<BaseHIRCObject> Entries { get; } = new();

    public List<T> FindEntry<T>(T obj) where T : BaseHIRCObject
    {
        var result = new List<T>();

        foreach (var entry in Entries)
        {
            if (entry.GetType() == typeof(T) && entry.Id == obj.Id)
            {
                result.Add((T) entry);
            }
        }

        return result;
    }

    public void Read(BinaryReader br, uint length)
    {
        var objCount = br.ReadUInt32();
        for (int i = 0; i < objCount; i++)
        {
            var type = br.ReadByte();
            var objLength = br.ReadUInt32();

            var startPos = br.BaseStream.Position;

            var id = br.ReadUInt32();

            BaseHIRCObject? obj = null;
            switch (type)
            {
                case 2: // Sound
                    obj = new SoundObject(id);
                    break;

                case 3: // Action
                    obj = new ActionObject(id);
                    break;

                case 4: // Event
                    obj = new EventObject(id);
                    break;

                case 5: // RanSeqCntr
                    obj = new RanSeqCntrObject(id);
                    break;

                case 10: // MusicSegment
                    obj = new MusicSegmentObject(id);
                    break;

                case 11: // MusicTrack
                    obj = new MusicTrackObject(id);
                    break;

                case 12: // MusicSwitchCntr
                    obj = new MusicSwitchCntrObject(id);
                    break;

                case 13: // MusicRanSeqCntr
                    obj = new MusicRanSeqCntrObject(id);
                    break;
            }

            if (obj != null)
            {
                obj.Read(br);

                Entries.Add(obj);
            }

            br.BaseStream.Position = startPos + objLength;
        }
    }
}

public abstract class BaseHIRCObject
{
    protected BaseHIRCObject(uint id)
    {
        Id = id;
    }

    public uint Id { get; }

    public abstract void Read(BinaryReader br);
}

public class SoundObject : BaseHIRCObject
{
    public SoundObject(uint id) : base(id) {}

    public uint SourceId { get; set; }
    public override void Read(BinaryReader br)
    {
        br.BaseStream.Position += 5;

        SourceId = br.ReadUInt32();
    }
}

public class ActionObject : BaseHIRCObject
{
    public ActionObject(uint id) : base(id) { }

    public byte Type { get; set; }
    public uint GameObjectReferenceId { get; set; }
    public override void Read(BinaryReader br)
    {
        br.BaseStream.Position += 1;

        Type = br.ReadByte();
        GameObjectReferenceId = br.ReadUInt32();
    }
}

public class EventObject : BaseHIRCObject
{
    public EventObject(uint id) : base(id) { }
    public List<uint> Events { get; } = new();

    public override void Read(BinaryReader br)
    {
        var objCount = br.ReadByte();
        for (int i = 0; i < objCount; i++)
        {
            Events.Add(br.ReadUInt32());
        }
    }
}

public class RanSeqCntrObject : BaseHIRCObject
{
    public RanSeqCntrObject(uint id) : base(id) { }
    public List<uint> Children { get; } = new();
    public override void Read(BinaryReader br)
    {
        HIRCHelper.ReadNodeBaseParams(br);

        br.BaseStream.Position += 24;

        var propsCount = br.ReadUInt32();
        for (int i = 0; i < propsCount; i++)
        {
            Children.Add(br.ReadUInt32());
        }
    }
}

public class MusicSegmentObject : BaseHIRCObject
{
    public MusicSegmentObject(uint id) : base(id) { }
    public List<uint> Children { get; } = new();
    public override void Read(BinaryReader br)
    {
        br.BaseStream.Position += 1;

        HIRCHelper.ReadNodeBaseParams(br);

        var propsCount = br.ReadUInt32();
        for (int i = 0; i < propsCount; i++)
        {
            Children.Add(br.ReadUInt32());
        }
    }
}

public class MusicTrackObject : BaseHIRCObject
{
    public MusicTrackObject(uint id) : base(id) { }
    public List<uint> Sources { get; } = new();
    public override void Read(BinaryReader br)
    {
        br.BaseStream.Position += 1;

        var numSources = br.ReadUInt32();
        for (int i = 0; i < numSources; i++)
        {
            br.BaseStream.Position += 5;
            Sources.Add(br.ReadUInt32());
        }
    }
}

public class MusicSwitchCntrObject : BaseHIRCObject
{
    public MusicSwitchCntrObject(uint id) : base(id) { }
    public List<uint> Children { get; } = new();
    public override void Read(BinaryReader br)
    {
        br.BaseStream.Position += 1;

        HIRCHelper.ReadNodeBaseParams(br);

        var propsCount = br.ReadUInt32();
        for (int i = 0; i < propsCount; i++)
        {
            Children.Add(br.ReadUInt32());
        }
    }
}

public class MusicRanSeqCntrObject : BaseHIRCObject
{
    public MusicRanSeqCntrObject(uint id) : base(id) { }
    public List<uint> Children { get; } = new();
    public override void Read(BinaryReader br)
    {
        br.BaseStream.Position += 1;

        HIRCHelper.ReadNodeBaseParams(br);

        var propsCount = br.ReadUInt32();
        for (int i = 0; i < propsCount; i++)
        {
            Children.Add(br.ReadUInt32());
        }
    }
}