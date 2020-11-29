using System;
using System.Collections.Generic;
using System.IO;
using SystemHalf;

namespace Nav2Parser
{
    class Nav2
    {
        public struct Header
        {
            public uint version;
            public uint fileLength;
            public uint entriesOffset;
            public uint entryCount;
            public uint navSystemOffset;
            public byte fileIndex;
            public byte u0a;
            public byte u0b;
            public byte u0c;
            public uint section2Offset;
            public uint o6;
            public Vector3 origin;

            public uint section3Offset; //Offset?
            public uint u1b;
            public uint manifsetOffset;
            public uint manifestLength;
            public uint u1c;
            public uint u1d;
            public ushort xDivisor;
            public ushort yDivisor;
            public ushort zDivisor;
            public ushort u1h; //Offset/Count?
            public byte n7;
            public byte section2EntryCount;
            public ushort n8; // Probably padding
            public Vector4Half unknown8;
            public Vector4Half unknown9;
            public uint manifestEntryCount;
        } //Header

        private struct ManifestEntry
        {
            public byte groupId;
            public byte u1b;
            public ushort u2;
            public uint payloadOffset;
            public byte entrySize;
            public ushort n4;
        } //ManifestEntry

        private struct Manifest
        {
            public ManifestEntry[] manifestEntries; //Always 3?
        } //Manifest

        public struct Entry
        {
            public ushort typeEnum;
            public ushort u1;
            public uint nextEntryRelativeOffset;
            public uint payloadRelativeOffset;
            public byte groupId;
            public byte u2;
            public ushort n2;
        } //Entry

        public struct NavWorld
        {
            public uint pointArraySectionOffset;
            public uint subsection2Offset;
            public uint subsection4Offset;
            public uint subsection3Offset; //Not read by exe?
            public uint u1; //Not read by exe? Always 0?
            public uint u2; //Always 0?
            public uint subsection5Offset; //Not read by exe?
            public uint u3; //Not read by exe? Always 0?
            public uint subsection6Offset;
            public ushort pointCount;
            public ushort edgeCount;
            public ushort subsection5EntryCount;
            public NavWorldPoint[] navWorldPoints;
            public NavWorldPointAdjacency[] navworldPointAdjacencies;
            public NavWorldSubsection3Entry[] navWorldSubsection3Entries;
            public NavWorldEdge[] navWorldEdges;
            public NavWorldEdgeFlags[] navWorldEdgeFlags;
            public short[] navWorldSubsection6Entries;
        } //NavWorld

        public struct NavWorldPoint
        {
            public ushort x;
            public ushort y;
            public ushort z;
        } //NavWorldSubsection1Entry

        public struct NavWorldPointAdjacency
        {
            public short navWorldSubsection3Index;
            public short u2;
            public byte countA;
            public byte countB;
        } //NavWorldSubsection2Entry

        public struct NavWorldSubsection3Entry
        {
            public ushort[] adjacentNodeIndices;
            public ushort[] edgeIndices;
            public ushort[] u3;
        }

        public struct NavWorldEdge
        {
            public ushort weight;
            public ushort subsection5Index;
            public byte from;
            public byte to;
        } //NavWorldSubsection4Entry

        public struct NavWorldEdgeFlags
        {
            public ushort flags;
        } //NavWorldSubsection5Entry

        public struct NavmeshChunk
        {
            public uint subsection1Offset;
            public uint subsection2Offset;
            public uint subsection3Offset;

            public ushort uu1;
            public ushort uu2;
            public ushort uu3;
            public ushort uu4;
            public ushort uu5;
            public ushort uu6;

            public ushort numFaces;
            public ushort numVertices;

            public ushort u4;
            public ushort u5;

            public NavmeshChunkSubsection1Entry[] navmeshChunkSubsection1Entries;
            public NavmeshChunkFaceOffset[] navmeshChunkFaceOffsets;
            public NavmeshChunkSubsection3Entry[] navmeshChunkSubsection3Entries;
        }

        public struct NavmeshChunkSubsection1Entry
        {
            public ushort x;
            public ushort y;
            public ushort z;
        }

        public struct NavmeshChunkFaceOffset
        {
            public uint offset;
        }

        public struct NavmeshChunkSubsection3Entry
        {
            public short adjacentFace1;
            public short adjacentFace2;
            public short adjacentFace3;
            public short adjacentFace4;

            public byte vertex1;
            public byte vertex2;
            public byte vertex3;
            public byte vertex4;

            public byte edgeIndex1;
            public byte edgeIndex2;
            public byte edgeIndex3;
            public byte edgeIndex4;
        }

        public struct NavworldSegmentGraph
        {
            public uint subsection1Offset;
            public uint subsection2Offset;
            public uint subsection3Offset;

            public uint uu1;
            public uint totalSize;

            public uint subsection1EntryCount;

            public ushort uu3;

            public uint totalEdges;

            public ushort padding;

            public NavworldSegmentGraphSubsection1Entry[] navworldSegmentGraphSubsection1Entries;
            public NavworldSegmentGraphSubsection2Entry[] navworldSegmentGraphSubsection2Entries;
            public NavworldSegmentGraphSubsection3Entry[] navworldSegmentGraphSubsection3Entries;
        }

        public struct NavworldSegmentGraphSubsection1Entry
        {
            public ushort x;
            public ushort y;
            public ushort z;
        }

        public struct NavworldSegmentGraphSubsection2Entry
        {
            public uint subsection3Index;
            public byte nEdges;
            public short u3;
            public ushort u4;
            public byte u5;
            public byte u6;
            public byte offGroupEdges;
            public byte offMeshEdges;
        }
        public struct NavworldSegmentGraphSubsection3Entry
        {
            public NavworldSegmentGraphType1Edge[] type1Edges;
            public NavworldSegmentGraphType2Edge[] type2Edges;
            public NavworldSegmentGraphType3Edge[] type3Edges;
        }

        public struct NavworldSegmentGraphType1Edge
        {
            public ushort weight;
            public ushort adjacentNode;
            public byte adjacentEdgeCount;
            public byte u1;
            public byte[] adjacentEdges;
        }

        public struct NavworldSegmentGraphType2Edge
        {
            public ushort groupId;
            public ushort weight;
            public ushort adjacentNode;
            public byte adjacentEdgeCount;
            public byte u5;
            public byte[] adjacentEdges;
        }

        public struct NavworldSegmentGraphType3Edge
        {
            public ushort u1;
            public ushort groupId;
            public ushort adjacentNode;
            public ushort u4;
            public byte adjacentEdgeCount;
            public byte u6;
            public byte[] adjacentEdges;
        }

        public struct SegmentChunk
        {
            public uint subsection1Offset;
            public uint subsection2Offset;
            public uint totalSize;
            public uint entryCount;

            public SegmentChunkSubsection1Entry[] segmentChunkSubsection1Entries;
            public SegmentChunkSubsection2Entry[] segmentChunkSubsection2Entries;
        }

        public struct SegmentChunkSubsection1Entry
        {
            public short x1;
            public short y1;
            public short z1;

            public short x2;
            public short y2;
            public short z2;
        }

        public struct SegmentChunkSubsection2Entry
        {
            public short vertexIndexOffset;
            public short navmeshChunkSubsection2EntryIndex;
            public short u3;
            public short u4;
            public byte verts;
            public byte faces;
            public byte u7;
            public byte edges;
        }

        public struct Section2Entry
        {
            public ushort nextEntryRelativeOffset;
            public ushort subsection1RelativeOffset;
            public ushort subEntryCount;
            public ushort subsection2RelativeOffset;
            public ushort subsection3RelativeOffset;
            public ushort subsection4RelativeOffset;
            public ushort subsection5RelativeOffset;
            public ushort subsection6RelativeOffset;

            public Section2Subsection1Entry[] subsection1Entries;
            public Section2Subsection2Entry[] subsection2Entries;

        }

        public struct Section2Subsection1Entry
        {
            public ushort x;
            public ushort y;
            public ushort z;

            public byte u1;
            public byte u2;
            public ushort subsection2Offset;
            public ushort countA;
            public ushort countB;
        }
        public struct Section2Subsection2Entry
        {
            public ushort[] u1;
            public ushort[] u2;
            public ushort[] u3;
        }

        /****************************************************************
         * VARIABLES
         ****************************************************************/
        public Header header;
        Manifest[] manifests;
        public Entry[] entries;
        public Dictionary<byte, NavmeshChunk> navmeshChunks;
        public Dictionary<byte, SegmentChunk> segmentChunks;
        public Dictionary<byte, NavworldSegmentGraph> segmentGraphs;
        public Dictionary<byte, NavWorld> navWorlds;
        public Section2Entry[] section2Entries;

        /****************************************************************
         * PUBLIC FUNCTIONS
         ****************************************************************/
        public void Read(string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                try
                {
                    BinaryReader reader = new BinaryReader(stream);
                    header = new Header();
                    navWorlds = new Dictionary<byte, NavWorld>();
                    navmeshChunks = new Dictionary<byte, NavmeshChunk>();
                    segmentChunks = new Dictionary<byte, SegmentChunk>();
                    segmentGraphs = new Dictionary<byte, NavworldSegmentGraph>();

                    ReadHeader(reader);
                    reader.BaseStream.Position = header.manifsetOffset;
                    ReadManifest(reader);
                    reader.BaseStream.Position = header.entriesOffset;
                    ReadEntries(reader);
                    if (header.section2Offset != 0)
                    {
                        reader.BaseStream.Position = header.section2Offset;
                        ReadSection2Entries(reader);
                    }
                } //try
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    Console.Write(e.StackTrace);
                } //catch
                finally
                {
                    stream.Close();
                } //finally
            } //using
        } //Read

        private void ReadSection2Entries(BinaryReader reader)
        {
            section2Entries = new Section2Entry[header.section2EntryCount];

            for (int i = 0; i < header.section2EntryCount; i++)
            {
                var startPosition = reader.BaseStream.Position;

                var entry = section2Entries[i];
                entry.nextEntryRelativeOffset = reader.ReadUInt16();
                entry.subsection1RelativeOffset = reader.ReadUInt16();
                entry.subEntryCount = reader.ReadUInt16();
                entry.subsection2RelativeOffset = reader.ReadUInt16();
                entry.subsection3RelativeOffset = reader.ReadUInt16();
                entry.subsection4RelativeOffset = reader.ReadUInt16();
                entry.subsection5RelativeOffset = reader.ReadUInt16();
                entry.subsection6RelativeOffset = reader.ReadUInt16();

                entry.subsection1Entries = new Section2Subsection1Entry[entry.subEntryCount];
                entry.subsection2Entries = new Section2Subsection2Entry[entry.subEntryCount];

                reader.BaseStream.Position = startPosition + entry.subsection1RelativeOffset;

                for (int j = 0; j < entry.subEntryCount; j++)
                {
                    var subentry = entry.subsection1Entries[j];

                    subentry.x = reader.ReadUInt16();
                    subentry.y = reader.ReadUInt16();
                    subentry.z = reader.ReadUInt16();

                    subentry.u1 = reader.ReadByte();
                    subentry.u2 = reader.ReadByte();
                    subentry.subsection2Offset = reader.ReadUInt16();
                    subentry.countB = reader.ReadUInt16();
                    subentry.countA = reader.ReadUInt16();

                    entry.subsection1Entries[j] = subentry;
                }

                reader.BaseStream.Position = startPosition + entry.subsection2RelativeOffset;

                for (int j = 0; j < entry.subEntryCount; j++)
                {
                    var subentry = entry.subsection2Entries[j];
                    subentry.u1 = new ushort[entry.subsection1Entries[j].countA];
                    subentry.u2 = new ushort[entry.subsection1Entries[j].countA];
                    subentry.u3 = new ushort[entry.subsection1Entries[j].countB];

                    for (int k = 0; k < entry.subsection1Entries[j].countA; k++)
                    {
                        subentry.u1[k] = reader.ReadUInt16();
                        subentry.u2[k] = reader.ReadUInt16();
                    }

                    for (int k = 0; k < entry.subsection1Entries[j].countB; k++)
                    {
                        subentry.u3[k] = reader.ReadUInt16();
                    }

                    entry.subsection2Entries[j] = subentry;
                }

                section2Entries[i] = entry;

                reader.BaseStream.Position = startPosition + entry.nextEntryRelativeOffset;
            }
        }

        /****************************************************************
         * PRIVATE FUNCTIONS
         ****************************************************************/
        private void ReadHeader(BinaryReader reader)
        {
            header.version = reader.ReadUInt32();
            header.fileLength = reader.ReadUInt32();
            header.entriesOffset = reader.ReadUInt32();
            header.entryCount = reader.ReadUInt32();
            header.navSystemOffset = reader.ReadUInt32();
            header.fileIndex = reader.ReadByte();
            header.u0a = reader.ReadByte();
            header.u0b = reader.ReadByte();
            header.u0c = reader.ReadByte();
            header.section2Offset = reader.ReadUInt32();
            header.o6 = reader.ReadUInt32();
            header.origin = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            header.section3Offset = reader.ReadUInt32();
            header.u1b = reader.ReadUInt32();
            header.manifsetOffset = reader.ReadUInt32();
            header.manifestLength = reader.ReadUInt32();
            header.u1c = reader.ReadUInt32();
            header.u1d = reader.ReadUInt32();
            header.xDivisor = reader.ReadUInt16();
            header.yDivisor = reader.ReadUInt16();
            header.zDivisor = reader.ReadUInt16();
            header.u1h = reader.ReadUInt16();
            header.n7 = reader.ReadByte();
            header.section2EntryCount = reader.ReadByte();
            header.n8 = reader.ReadUInt16();
            header.unknown8 = new Vector4Half(Half.ToHalf(reader.ReadUInt16()), Half.ToHalf(reader.ReadUInt16()), Half.ToHalf(reader.ReadUInt16()), Half.ToHalf(reader.ReadUInt16()));
            header.unknown9 = new Vector4Half(Half.ToHalf(reader.ReadUInt16()), Half.ToHalf(reader.ReadUInt16()), Half.ToHalf(reader.ReadUInt16()), Half.ToHalf(reader.ReadUInt16()));
        } //ReadHeader

        private void ReadManifest(BinaryReader reader)
        {
            header.manifestEntryCount = reader.ReadUInt32();

            int manifestEntryCount = (int)header.manifestEntryCount;
            manifests = new Manifest[manifestEntryCount];

            for (int i = 0; i < manifestEntryCount; i++)
            {
                Manifest manifest = manifests[i];
                manifest.manifestEntries = new ManifestEntry[3];
                int manifestEntriesLength = manifest.manifestEntries.Length;

                for (int j = 0; j < manifestEntriesLength; j++)
                {
                    ManifestEntry entry = manifest.manifestEntries[j];

                    entry.groupId = reader.ReadByte();
                    entry.u1b = reader.ReadByte();
                    entry.u2 = reader.ReadUInt16();
                    entry.payloadOffset = reader.ReadUInt32();
                    entry.entrySize = reader.ReadByte();
                    entry.n4 = reader.ReadUInt16();

                    manifest.manifestEntries[j] = entry;
                } //for

                manifests[i] = manifest;
            } //for
        } //ReadManifest

        private void ReadEntries(BinaryReader reader)
        {
            int entryCount = (int)header.entryCount;
            entries = new Entry[entryCount];

            //Should be to entryCount; but will cause errors if it is in its current state.
            for (int i = 0; i < entryCount; i++)
            {
                var startPos = reader.BaseStream.Position;
                Entry entry = entries[i];

                entry.typeEnum = reader.ReadUInt16();
                entry.u1 = reader.ReadUInt16();
                entry.nextEntryRelativeOffset = reader.ReadUInt32();
                entry.payloadRelativeOffset = reader.ReadUInt32();
                entry.groupId = reader.ReadByte();
                entry.u2 = reader.ReadByte();
                entry.n2 = reader.ReadUInt16();

                switch (entry.typeEnum)
                {
                    case 0:
                        ReadNavWorld(reader, entry.groupId);
                        break;
                    case 1:
                        ReadNavmeshChunk(reader, entry.groupId);
                        break;
                    case 3:
                        ReadNavworldSegmentGraph(reader, entry.groupId);
                        break;
                    case 4:
                        ReadSegmentChunk(reader, entry.groupId);
                        break;
                    default:
                        break;
                } //switch

                reader.BaseStream.Position = startPos + entry.nextEntryRelativeOffset;

                entries[i] = entry;
            } //for
        } //ReadEntries

        private void ReadNavworldSegmentGraph(BinaryReader reader, byte groupId)
        {
            NavworldSegmentGraph chunk = new NavworldSegmentGraph();
            long startPosition = reader.BaseStream.Position;

            chunk.subsection1Offset = reader.ReadUInt32();
            chunk.subsection2Offset = reader.ReadUInt32();
            chunk.subsection3Offset = reader.ReadUInt32();

            chunk.uu1 = reader.ReadUInt32();

            chunk.totalSize = reader.ReadUInt32();
            chunk.subsection1EntryCount = reader.ReadUInt32();

            chunk.navworldSegmentGraphSubsection1Entries = new NavworldSegmentGraphSubsection1Entry[chunk.subsection1EntryCount];
            chunk.navworldSegmentGraphSubsection2Entries = new NavworldSegmentGraphSubsection2Entry[chunk.subsection1EntryCount];
            chunk.navworldSegmentGraphSubsection3Entries = new NavworldSegmentGraphSubsection3Entry[chunk.subsection1EntryCount];

            chunk.uu3 = reader.ReadUInt16();
            chunk.totalEdges = reader.ReadUInt32();
            chunk.padding = reader.ReadUInt16();

            for (int i = 0; i < chunk.subsection1EntryCount; i++)
            {
                NavworldSegmentGraphSubsection1Entry navworldSegmentGraphSubsection1Entry = chunk.navworldSegmentGraphSubsection1Entries[i];
                navworldSegmentGraphSubsection1Entry.x = reader.ReadUInt16();
                navworldSegmentGraphSubsection1Entry.y = reader.ReadUInt16();
                navworldSegmentGraphSubsection1Entry.z = reader.ReadUInt16();

                chunk.navworldSegmentGraphSubsection1Entries[i] = navworldSegmentGraphSubsection1Entry;
            }

            reader.BaseStream.Position = startPosition + chunk.subsection2Offset;

            for (int i = 0; i < chunk.subsection1EntryCount; i++)
            {
                NavworldSegmentGraphSubsection2Entry navworldSegmentGraphSubsection2Entry = chunk.navworldSegmentGraphSubsection2Entries[i];
                navworldSegmentGraphSubsection2Entry.subsection3Index = reader.ReadUInt32();
                navworldSegmentGraphSubsection2Entry.nEdges = (byte)(navworldSegmentGraphSubsection2Entry.subsection3Index & 0xff);
                navworldSegmentGraphSubsection2Entry.subsection3Index &= 0xffffff00;

                navworldSegmentGraphSubsection2Entry.u3 = reader.ReadInt16();
                navworldSegmentGraphSubsection2Entry.u4 = reader.ReadUInt16();
                navworldSegmentGraphSubsection2Entry.u5 = reader.ReadByte();
                navworldSegmentGraphSubsection2Entry.u6 = reader.ReadByte();
                navworldSegmentGraphSubsection2Entry.offGroupEdges = reader.ReadByte();
                navworldSegmentGraphSubsection2Entry.offMeshEdges = reader.ReadByte();

                chunk.navworldSegmentGraphSubsection2Entries[i] = navworldSegmentGraphSubsection2Entry;
            }

            reader.BaseStream.Position = startPosition + chunk.subsection3Offset;

            for (int i = 0; i < chunk.subsection1EntryCount; i++)
            {
                var navworldSegmentGraphSubsection3Entry = chunk.navworldSegmentGraphSubsection3Entries[i];

                var type1EdgeCount = chunk.navworldSegmentGraphSubsection2Entries[i].nEdges;
                var type2EdgeCount = chunk.navworldSegmentGraphSubsection2Entries[i].offGroupEdges;
                var type3EdgeCount = chunk.navworldSegmentGraphSubsection2Entries[i].offMeshEdges;
                navworldSegmentGraphSubsection3Entry.type1Edges = new NavworldSegmentGraphType1Edge[type1EdgeCount];
                navworldSegmentGraphSubsection3Entry.type2Edges = new NavworldSegmentGraphType2Edge[type2EdgeCount];
                navworldSegmentGraphSubsection3Entry.type3Edges = new NavworldSegmentGraphType3Edge[type3EdgeCount];

                for (int j = 0; j < type1EdgeCount; j++)
                {
                    var navworldSegmentGraphType1Edge = navworldSegmentGraphSubsection3Entry.type1Edges[j];
                    navworldSegmentGraphType1Edge.weight = reader.ReadUInt16();
                    navworldSegmentGraphType1Edge.adjacentNode = reader.ReadUInt16();
                    navworldSegmentGraphType1Edge.adjacentEdgeCount = reader.ReadByte();
                    navworldSegmentGraphType1Edge.adjacentEdges = new byte[navworldSegmentGraphType1Edge.adjacentEdgeCount];
                    navworldSegmentGraphType1Edge.u1 = reader.ReadByte();

                    navworldSegmentGraphSubsection3Entry.type1Edges[j] = navworldSegmentGraphType1Edge;
                }

                for (int j = 0; j < type2EdgeCount; j++)
                {
                    var navworldSegmentGraphType2Edge = navworldSegmentGraphSubsection3Entry.type2Edges[j];
                    navworldSegmentGraphType2Edge.groupId = reader.ReadUInt16();
                    navworldSegmentGraphType2Edge.weight = reader.ReadUInt16();
                    navworldSegmentGraphType2Edge.adjacentNode = reader.ReadUInt16();
                    navworldSegmentGraphType2Edge.adjacentEdgeCount = reader.ReadByte();
                    navworldSegmentGraphType2Edge.adjacentEdges = new byte[navworldSegmentGraphType2Edge.adjacentEdgeCount];
                    navworldSegmentGraphType2Edge.u5 = reader.ReadByte();

                    navworldSegmentGraphSubsection3Entry.type2Edges[j] = navworldSegmentGraphType2Edge;
                }

                for (int j = 0; j < type3EdgeCount; j++)
                {
                    var navworldSegmentGraphType3Edge = navworldSegmentGraphSubsection3Entry.type3Edges[j];
                    navworldSegmentGraphType3Edge.u1 = reader.ReadUInt16();
                    navworldSegmentGraphType3Edge.groupId = reader.ReadUInt16();
                    navworldSegmentGraphType3Edge.adjacentNode = reader.ReadUInt16();
                    navworldSegmentGraphType3Edge.u4 = reader.ReadUInt16();
                    navworldSegmentGraphType3Edge.adjacentEdgeCount = reader.ReadByte();
                    navworldSegmentGraphType3Edge.adjacentEdges = new byte[navworldSegmentGraphType3Edge.adjacentEdgeCount];
                    navworldSegmentGraphType3Edge.u6 = reader.ReadByte();

                    navworldSegmentGraphSubsection3Entry.type3Edges[j] = navworldSegmentGraphType3Edge;
                }

                for (int j = 0; j < type1EdgeCount; j++)
                {
                    for (int k = 0; k < navworldSegmentGraphSubsection3Entry.type1Edges[j].adjacentEdgeCount; k++)
                    {
                        navworldSegmentGraphSubsection3Entry.type1Edges[j].adjacentEdges[k] = reader.ReadByte();
                    }
                }

                for (int j = 0; j < type2EdgeCount; j++)
                {
                    for (int k = 0; k < navworldSegmentGraphSubsection3Entry.type2Edges[j].adjacentEdgeCount; k++)
                    {
                        navworldSegmentGraphSubsection3Entry.type2Edges[j].adjacentEdges[k] = reader.ReadByte();
                    }
                }

                for (int j = 0; j < type3EdgeCount; j++)
                {
                    for (int k = 0; k < navworldSegmentGraphSubsection3Entry.type3Edges[j].adjacentEdgeCount; k++)
                    {
                        navworldSegmentGraphSubsection3Entry.type3Edges[j].adjacentEdges[k] = reader.ReadByte();
                    }
                }

                chunk.navworldSegmentGraphSubsection3Entries[i] = navworldSegmentGraphSubsection3Entry;
            }

            segmentGraphs.Add(groupId, chunk);
        }

        private void ReadSegmentChunk(BinaryReader reader, byte group_id)
        {
            SegmentChunk chunk = new SegmentChunk();
            long startPosition = reader.BaseStream.Position;

            chunk.subsection1Offset = reader.ReadUInt32();
            chunk.subsection2Offset = reader.ReadUInt32();

            chunk.totalSize = reader.ReadUInt32();
            chunk.entryCount = reader.ReadUInt32();

            chunk.segmentChunkSubsection1Entries = new SegmentChunkSubsection1Entry[chunk.entryCount];
            chunk.segmentChunkSubsection2Entries = new SegmentChunkSubsection2Entry[chunk.entryCount];

            reader.BaseStream.Position = startPosition + chunk.subsection1Offset;

            for (int i = 0; i < chunk.entryCount; i++)
            {
                SegmentChunkSubsection1Entry segmentChunkSubsection1Entry = chunk.segmentChunkSubsection1Entries[i];
                segmentChunkSubsection1Entry.x1 = reader.ReadInt16();
                segmentChunkSubsection1Entry.y1 = reader.ReadInt16();
                segmentChunkSubsection1Entry.z1 = reader.ReadInt16();
                segmentChunkSubsection1Entry.x2 = reader.ReadInt16();
                segmentChunkSubsection1Entry.y2 = reader.ReadInt16();
                segmentChunkSubsection1Entry.z2 = reader.ReadInt16();

                chunk.segmentChunkSubsection1Entries[i] = segmentChunkSubsection1Entry;
            }

            reader.BaseStream.Position = startPosition + chunk.subsection2Offset;

            for (int i = 0; i < chunk.entryCount; i++)
            {
                SegmentChunkSubsection2Entry segmentChunkSubsection2Entry = chunk.segmentChunkSubsection2Entries[i];
                segmentChunkSubsection2Entry.vertexIndexOffset = reader.ReadInt16();
                segmentChunkSubsection2Entry.navmeshChunkSubsection2EntryIndex = reader.ReadInt16();
                segmentChunkSubsection2Entry.u3 = reader.ReadInt16();
                segmentChunkSubsection2Entry.u4 = reader.ReadInt16();
                segmentChunkSubsection2Entry.verts = reader.ReadByte();
                segmentChunkSubsection2Entry.faces = reader.ReadByte();
                segmentChunkSubsection2Entry.u7 = reader.ReadByte();
                segmentChunkSubsection2Entry.edges = reader.ReadByte();

                chunk.segmentChunkSubsection2Entries[i] = segmentChunkSubsection2Entry;
            }

            segmentChunks.Add(group_id, chunk);
        }

        private void ReadNavmeshChunk(BinaryReader reader, byte group_id)
        {
            NavmeshChunk chunk = new NavmeshChunk();
            long startPosition = reader.BaseStream.Position;

            chunk.subsection1Offset = reader.ReadUInt32();
            chunk.subsection2Offset = reader.ReadUInt32();
            chunk.subsection3Offset = reader.ReadUInt32();

            chunk.uu1 = reader.ReadUInt16();
            chunk.uu2 = reader.ReadUInt16();
            chunk.uu3 = reader.ReadUInt16();
            chunk.uu4 = reader.ReadUInt16();
            chunk.uu5 = reader.ReadUInt16();
            chunk.uu6 = reader.ReadUInt16();

            chunk.numFaces = reader.ReadUInt16();
            chunk.numVertices = reader.ReadUInt16();

            chunk.u4 = reader.ReadUInt16();
            chunk.u5 = reader.ReadUInt16();

            chunk.navmeshChunkSubsection1Entries = new NavmeshChunkSubsection1Entry[chunk.numVertices];
            chunk.navmeshChunkFaceOffsets = new NavmeshChunkFaceOffset[chunk.numFaces];
            chunk.navmeshChunkSubsection3Entries = new NavmeshChunkSubsection3Entry[chunk.numFaces];

            reader.BaseStream.Position = startPosition + chunk.subsection1Offset;

            for (int i = 0; i < chunk.numVertices; i++)
            {
                NavmeshChunkSubsection1Entry navmeshChunkSubsection1Entry = chunk.navmeshChunkSubsection1Entries[i];
                navmeshChunkSubsection1Entry.x = reader.ReadUInt16();
                navmeshChunkSubsection1Entry.y = reader.ReadUInt16();
                navmeshChunkSubsection1Entry.z = reader.ReadUInt16();

                float f1 = (float)navmeshChunkSubsection1Entry.x / (float)header.xDivisor;
                float f2 = (float)navmeshChunkSubsection1Entry.y / (float)header.yDivisor;
                float f3 = (float)navmeshChunkSubsection1Entry.z / (float)header.zDivisor;

                chunk.navmeshChunkSubsection1Entries[i] = navmeshChunkSubsection1Entry;
            }

            reader.BaseStream.Position = startPosition + chunk.subsection2Offset;

            for (int i = 0; i < chunk.numFaces; i++)
            {
                NavmeshChunkFaceOffset navmeshChunkSubsection2Entry = chunk.navmeshChunkFaceOffsets[i];
                navmeshChunkSubsection2Entry.offset = reader.ReadUInt32();

                chunk.navmeshChunkFaceOffsets[i] = navmeshChunkSubsection2Entry;
            }

            reader.BaseStream.Position = startPosition + chunk.subsection3Offset;

            for (int i = 0; i < chunk.numFaces; i++)
            {
                reader.BaseStream.Position = startPosition + chunk.subsection3Offset + ((chunk.navmeshChunkFaceOffsets[i].offset & 0x3ffff) * 2);
                NavmeshChunkSubsection3Entry navmeshChunkSubsection3Entry = chunk.navmeshChunkSubsection3Entries[i];
                navmeshChunkSubsection3Entry.adjacentFace1 = reader.ReadInt16();
                navmeshChunkSubsection3Entry.adjacentFace2 = reader.ReadInt16();
                navmeshChunkSubsection3Entry.adjacentFace3 = reader.ReadInt16();

                var size = (chunk.navmeshChunkFaceOffsets[i].offset >> 0x12) & 1;
                if (size != 0)
                {
                    navmeshChunkSubsection3Entry.adjacentFace4 = reader.ReadInt16();
                }

                navmeshChunkSubsection3Entry.vertex1 = reader.ReadByte();
                navmeshChunkSubsection3Entry.vertex2 = reader.ReadByte();
                navmeshChunkSubsection3Entry.vertex3 = reader.ReadByte();
                if (size != 0)
                {
                    navmeshChunkSubsection3Entry.vertex4 = reader.ReadByte();
                }

                navmeshChunkSubsection3Entry.edgeIndex1 = reader.ReadByte();
                navmeshChunkSubsection3Entry.edgeIndex2 = reader.ReadByte();
                navmeshChunkSubsection3Entry.edgeIndex3 = reader.ReadByte();
                if (size != 0)
                {
                    navmeshChunkSubsection3Entry.edgeIndex4 = reader.ReadByte();
                }

                chunk.navmeshChunkSubsection3Entries[i] = navmeshChunkSubsection3Entry;
            }

            navmeshChunks.Add(group_id, chunk);
        }

        private void ReadNavWorld(BinaryReader reader, byte groupId)
        {
            NavWorld navWorld = new NavWorld();
            long startPosition = reader.BaseStream.Position;

            navWorld.pointArraySectionOffset = reader.ReadUInt32();
            navWorld.subsection2Offset = reader.ReadUInt32();
            navWorld.subsection4Offset = reader.ReadUInt32();
            navWorld.subsection3Offset = reader.ReadUInt32();
            navWorld.u1 = reader.ReadUInt32();
            navWorld.u2 = reader.ReadUInt32();
            navWorld.subsection5Offset = reader.ReadUInt32();
            navWorld.u3 = reader.ReadUInt32();
            navWorld.subsection6Offset = reader.ReadUInt32();
            navWorld.pointCount = reader.ReadUInt16();
            navWorld.edgeCount = reader.ReadUInt16();
            reader.BaseStream.Position += 6;
            navWorld.subsection5EntryCount = reader.ReadUInt16();

            int count = navWorld.pointCount;
            navWorld.navWorldPoints = new NavWorldPoint[count];
            navWorld.navworldPointAdjacencies = new NavWorldPointAdjacency[count];
            navWorld.navWorldSubsection3Entries = new NavWorldSubsection3Entry[count];

            reader.BaseStream.Position = startPosition + navWorld.pointArraySectionOffset;

            for (int i = 0; i < count; i++)
            {
                NavWorldPoint navWorldSubsection1Entry = navWorld.navWorldPoints[i];

                navWorldSubsection1Entry.x = reader.ReadUInt16();
                navWorldSubsection1Entry.y = reader.ReadUInt16(); 
                navWorldSubsection1Entry.z = reader.ReadUInt16();

                navWorld.navWorldPoints[i] = navWorldSubsection1Entry;
            } //for

            reader.BaseStream.Position = startPosition + navWorld.subsection2Offset;

            for (int i = 0; i < count; i++)
            {
                NavWorldPointAdjacency navWorldSubsection2 = navWorld.navworldPointAdjacencies[i];

                navWorldSubsection2.navWorldSubsection3Index = reader.ReadInt16();
                navWorldSubsection2.u2 = reader.ReadInt16();
                navWorldSubsection2.countA = reader.ReadByte();
                navWorldSubsection2.countB = reader.ReadByte();

                navWorld.navworldPointAdjacencies[i] = navWorldSubsection2;
            } //for

            reader.BaseStream.Position = startPosition + navWorld.subsection3Offset;

            for (int i = 0; i < count; i++)
            {
                var navWorldSubsection2 = navWorld.navworldPointAdjacencies[i];

                NavWorldSubsection3Entry navWorldSubsection3 = navWorld.navWorldSubsection3Entries[i];
                navWorldSubsection3.adjacentNodeIndices = new ushort[navWorldSubsection2.countA];
                navWorldSubsection3.edgeIndices = new ushort[navWorldSubsection2.countA];
                navWorldSubsection3.u3 = new ushort[navWorldSubsection2.countB];

                reader.BaseStream.Position = startPosition + navWorld.subsection3Offset + (navWorld.navworldPointAdjacencies[i].navWorldSubsection3Index * 2);

                for (int j = 0; j < navWorldSubsection2.countA; j++)
                {
                    navWorldSubsection3.adjacentNodeIndices[j] = reader.ReadUInt16();
                    navWorldSubsection3.edgeIndices[j] = reader.ReadUInt16();
                }

                for (int j = 0; j < navWorldSubsection2.countB; j++)
                {
                    navWorldSubsection3.u3[j] = reader.ReadUInt16();
                }

                navWorld.navWorldSubsection3Entries[i] = navWorldSubsection3;
            }

            count = navWorld.edgeCount;
            navWorld.navWorldEdges = new NavWorldEdge[count];

            reader.BaseStream.Position = startPosition + navWorld.subsection4Offset;

            for (int i = 0; i < count; i++)
            {
                NavWorldEdge navWorldSubsection4Entry = navWorld.navWorldEdges[i];

                navWorldSubsection4Entry.weight = reader.ReadUInt16();
                navWorldSubsection4Entry.subsection5Index = reader.ReadUInt16();
                navWorldSubsection4Entry.from = reader.ReadByte();
                navWorldSubsection4Entry.to = reader.ReadByte();

                navWorld.navWorldEdges[i] = navWorldSubsection4Entry;
            } //for

            count = navWorld.subsection5EntryCount;
            navWorld.navWorldEdgeFlags = new NavWorldEdgeFlags[count];

            reader.BaseStream.Position = startPosition + navWorld.subsection5Offset;

            for (int i = 0; i < count; i++)
            {
                NavWorldEdgeFlags navWorldSubsection5Entry = navWorld.navWorldEdgeFlags[i];

                navWorldSubsection5Entry.flags = reader.ReadUInt16();
                navWorld.navWorldEdgeFlags[i] = navWorldSubsection5Entry;
            } //for

            count = navWorld.pointCount;
            navWorld.navWorldSubsection6Entries = new short[count];

            reader.BaseStream.Position = startPosition + navWorld.subsection6Offset;

            for (int i = 0; i < count; i++)
            {
                navWorld.navWorldSubsection6Entries[i] = reader.ReadInt16();
            } //for

            navWorlds.Add(groupId, navWorld);
        } //ReadNavWorld

        /****************************************************************
         * DEBUG FUNCTIONS
         ****************************************************************/
        public void DisplayHeaderInfo()
        {
            Console.WriteLine($"\nFile Version: {header.version}");
            Console.WriteLine($"File Length: 0x{header.fileLength.ToString("x")}");
            Console.WriteLine($"Entries Offset: 0x{header.entriesOffset.ToString("x")}");
            Console.WriteLine($"Entry Count: 0x{header.entryCount.ToString("x")}");
            Console.WriteLine($"fileIndex: 0x{header.fileIndex.ToString("x")}");
            Console.WriteLine($"u0a: 0x{header.u0a.ToString("x")}");
            Console.WriteLine($"u0b: 0x{header.u0b.ToString("x")}");
            Console.WriteLine($"u0c: 0x{header.u0c.ToString("x")}");
            Console.WriteLine($"Section 2 Offset: 0x{header.section2Offset.ToString("x")}");
            Console.WriteLine($"o6: 0x{header.o6.ToString("x")}");
            Console.WriteLine($"Origin: X: {header.origin.x}, Y: {header.origin.y}, Z: {header.origin.z}");
            Console.WriteLine($"Section 3 Offset: 0x{header.section3Offset.ToString("x")}");
            Console.WriteLine($"u1b: 0x{header.u1b.ToString("x")}");
            Console.WriteLine($"Manifest Offset: 0x{header.manifsetOffset.ToString("x")}");
            Console.WriteLine($"Manifest Length: 0x{header.manifestLength.ToString("x")}");
            Console.WriteLine($"u1c: 0x{header.u1c.ToString("x")}");
            Console.WriteLine($"u1d: 0x{header.u1d.ToString("x")}");
            Console.WriteLine($"X Divisor: {header.xDivisor}");
            Console.WriteLine($"Y Divisor: {header.yDivisor}");
            Console.WriteLine($"Z Divisor: {header.zDivisor}");
            Console.WriteLine($"u1h: 0x{header.u1h.ToString("x")}");
            Console.WriteLine($"n7: 0x{header.n7.ToString("x")}");
            Console.WriteLine($"Section 2 Entry Count: 0x{header.section2EntryCount.ToString("x")}");
            Console.WriteLine($"n8: 0x{header.n8.ToString("x")}");
            Console.WriteLine($"Unknown 8: X: {header.unknown8.x}, Y: {header.unknown8.y}, Z: {header.unknown8.z}, W: {header.unknown8.w}");
            Console.WriteLine($"Unknown 9: X: {header.unknown9.x}, Y: {header.unknown9.y}, Z: {header.unknown9.z}, W: {header.unknown9.w}");
            Console.WriteLine($"Manifest Entry Count: 0x{header.manifestEntryCount.ToString("x")}\n");
        } //DisplayHeaderInfo

        public void DisplayManifestInfo()
        {
            int manifestCount = manifests.Length;

            for (int i = 0; i < manifestCount; i++)
            {
                Manifest manifest = manifests[i];
                int entryCount = manifest.manifestEntries.Length;

                Console.WriteLine($"\nManifest: {i}");
                Console.WriteLine("================================================================");

                for(int j = 0; j < entryCount; j++)
                {
                    ManifestEntry entry = manifest.manifestEntries[j];

                    Console.WriteLine($"\nEntry: {j}");
                    Console.WriteLine("----------------------------------------------------------------");
                    Console.WriteLine($"Group Id: 0x{entry.groupId.ToString("x")}");
                    Console.WriteLine($"u1b: 0x{entry.u1b.ToString("x")}");
                    Console.WriteLine($"u2: 0x{entry.u2.ToString("x")}");
                    Console.WriteLine($"Payload Offset: 0x{entry.payloadOffset.ToString("x")}");
                    Console.WriteLine($"Entry Size: 0x{entry.entrySize.ToString("x")}");
                    Console.WriteLine($"n4: 0x{entry.n4.ToString("x")}\n");
                } //for
            } //for
        } //DisplayManifestInfo

        public void DisplaySection2Info()
        {
            for (int i = 0; i < section2Entries.Length; i++)
            {
                Section2Entry entry = section2Entries[i];

                Console.WriteLine($"\nSection 2: {i}");
                Console.WriteLine("================================================================");

                for(int j = 0; j < entry.subEntryCount; j++)
                {
                    var subentry = entry.subsection1Entries[j];
                    Console.WriteLine($"\nEntry: {j}");
                    Console.WriteLine("----------------------------------------------------------------");
                    Console.WriteLine($"X: {(float)subentry.x / (float)header.xDivisor}, Y: {(float)subentry.y / (float)header.yDivisor}, Z: {(float)subentry.z / (float)header.zDivisor}");

                }
            }
        }

        public void DisplayNavWorldInfo()
        {
            foreach (KeyValuePair<byte, NavWorld> world in navWorlds)
            {
                var navWorld = world.Value;
                var i = world.Key;

                Console.WriteLine($"\nNavWorld: {i}");
                Console.WriteLine("================================================================");
                Console.WriteLine($"Point Array Offset: 0x{navWorld.pointArraySectionOffset.ToString("x")}");
                Console.WriteLine($"Subsection 2 Offset: 0x{navWorld.subsection2Offset.ToString("x")}");
                Console.WriteLine($"Subsection 4 Offset: 0x{navWorld.subsection4Offset.ToString("x")}");
                Console.WriteLine($"Subsection 3 Offset: 0x{navWorld.subsection3Offset.ToString("x")}");
                Console.WriteLine($"u1: 0x{navWorld.u1.ToString("x")}");
                Console.WriteLine($"u2: 0x{navWorld.u2.ToString("x")}");
                Console.WriteLine($"Subsection 5 Offset: 0x{navWorld.subsection5Offset.ToString("x")}");
                Console.WriteLine($"u3: 0x{navWorld.u3.ToString("x")}");
                Console.WriteLine($"Subsection 6 Offset: 0x{navWorld.subsection6Offset.ToString("x")}");
                Console.WriteLine($"Num Points: {navWorld.pointCount}");
                Console.WriteLine($"Num Edges: {navWorld.edgeCount}");
                Console.WriteLine($"Subsection 5 Entry Count: 0x{navWorld.subsection5EntryCount.ToString("x")}");

                int count = navWorld.navWorldPoints.Length;

                for(int j = 0; j < count; j++)
                {
                    NavWorldPoint entry = navWorld.navWorldPoints[j];

                    Console.WriteLine($"\nPoint Entry: {j}");
                    Console.WriteLine("----------------------------------------------------------------");
                    Console.WriteLine($"x: {entry.x}");
                    Console.WriteLine($"y: {entry.y}");
                    Console.WriteLine($"z: {entry.z}");
                } //for

                count = navWorld.navworldPointAdjacencies.Length;

                for (int j = 0; j < count; j++)
                {
                    NavWorldPointAdjacency entry = navWorld.navworldPointAdjacencies[j];

                    Console.WriteLine($"\nPoint Adjacency Entry: {j}");
                    Console.WriteLine("----------------------------------------------------------------");
                    Console.WriteLine($"navWorldSubsection3Index: {entry.navWorldSubsection3Index}");
                    Console.WriteLine($"u2: 0x{entry.u2.ToString("x")}");
                    Console.WriteLine($"countA: {entry.countA}");
                    Console.WriteLine($"countB: {entry.countB}\n");
                } //for

                count = navWorld.navWorldSubsection3Entries.Length;

                Console.WriteLine($"\nSubsection 3 Entries");
                Console.WriteLine("----------------------------------------------------------------");

                for (int j = 0; j < count; j++)
                {
                    Console.WriteLine($"\nSubsection 3 Entry: {j}");
                    Console.WriteLine("----------------------------------------------------------------");
                    Console.WriteLine($"Adjacent Node Indices: {string.Join(", ", navWorld.navWorldSubsection3Entries[j].adjacentNodeIndices)}");
                    Console.WriteLine($"Edge Indices: {string.Join(", ", navWorld.navWorldSubsection3Entries[j].edgeIndices)}");
                    Console.WriteLine($"u3: {string.Join(", ", navWorld.navWorldSubsection3Entries[j].u3)}");

                } //for

                Console.WriteLine($"\n");

                count = navWorld.navWorldEdges.Length;

                for (int j = 0; j < count; j++)
                {
                    NavWorldEdge entry = navWorld.navWorldEdges[j];

                    Console.WriteLine($"\nEdge Entry: {j}");
                    Console.WriteLine("----------------------------------------------------------------");
                    Console.WriteLine($"Weight: {entry.weight}");
                    Console.WriteLine($"Subsection 5 Index: {entry.subsection5Index}");
                    Console.WriteLine($"From: {entry.from}");
                    Console.WriteLine($"To: {entry.to}\n");
                } //for

                count = navWorld.navWorldEdgeFlags.Length;

                for (int j = 0; j < count; j++)
                {
                    NavWorldEdgeFlags entry = navWorld.navWorldEdgeFlags[j];

                    Console.WriteLine($"\nFlags Entry: {j}");
                    Console.WriteLine("----------------------------------------------------------------");
                    Console.WriteLine($"flags: 0x{entry.flags.ToString("x")}\n");
                } //for

                count = navWorld.navWorldSubsection6Entries.Length;

                Console.WriteLine($"\nSubsection 6 Entries");
                Console.WriteLine("----------------------------------------------------------------");

                for (int j = 0; j < count; j++)
                {
                    Console.Write($"0x{navWorld.navWorldSubsection6Entries[j].ToString("x")}, ");
                } //for

                Console.WriteLine($"\n");
            } //for
        } //DisplayNavWorldInfo
    } //class
} //namespace
