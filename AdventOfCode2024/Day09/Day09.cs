using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day09
{
    public class Day09 : BaseAocV2
    {
        public override object PartOne(List<string> lines)
        {
            var diskMap = lines[0].Select((c, i) => new MapEntry(i/2 ,c - '0', i % 2 == 0)).ToList();
            int rightCursor = diskMap.Count - 1;

            if (!diskMap[rightCursor].IsFile)
            {
                rightCursor--;
            }

            int blockPosition = 0;

            int leftCursor = 0;

            long checksum = 0;

            while (leftCursor < diskMap.Count && rightCursor >= 0)
            {
                //if (leftCursor == rightCursor)
                //{
                //    Debugger.Break();
                //}

                MapEntry leftEntry = diskMap[leftCursor];

                if (leftEntry.Size == 0)
                {
                    leftCursor++;
                    continue;
                }

                if (leftEntry.IsFile)
                {
                    Write($"{leftEntry.Id}");
                    checksum += blockPosition * leftEntry.Id;
                    blockPosition++;
                    leftEntry.Size--;

                    continue;
                }
                else
                {
                    var rightEntry = diskMap[rightCursor];
                    if (rightEntry.Size == 0)
                    {
                        rightCursor -= 2;
                        continue;
                    }

                    leftEntry.Size--;
                    rightEntry.Size--;

                    Write($"{rightEntry.Id}");
                    checksum += blockPosition * rightEntry.Id;
                    blockPosition++;
                }
            }
            WriteLine();

            return checksum;
        }

        public override object PartTwo(List<string> lines)
        {
            long blockCounter = 0;
            var diskMap = lines[0].Select((c, i) =>
            {
                var size = c - '0';
                var entry = new MapEntry(i / 2, size, i % 2 == 0, blockCounter);
                blockCounter += size;
                return entry;
            }).ToList();
            WriteDisk(diskMap);
            int rightCursor = diskMap.Count - 1;

            if (!diskMap[rightCursor].IsFile)
            {
                rightCursor--;
            }

            while (rightCursor > 0)
            {
                var rightEntry = diskMap[rightCursor];

                if (!rightEntry.IsFile)
                {
                    rightCursor--;
                    continue;
                }
                for (int i = 0; i < diskMap.Count; i++)
                {
                    var entry = diskMap[i];

                    if (entry.StartIndex >= rightEntry.StartIndex)
                    {
                        break;
                    }
                    if (!entry.IsFile && entry.Size >= rightEntry.Size)
                    {
                        int remainder = entry.Size - rightEntry.Size;

                        if (remainder > 0)
                        {
                            diskMap.Insert(i + 1, new MapEntry(0, remainder, false, entry.StartIndex + rightEntry.Size));
                        }

                        entry.Size = rightEntry.Size;
                        entry.Id = rightEntry.Id;
                        entry.IsFile = true;

                        rightEntry.IsFile = false;
                        break;
                    }
                }
                rightCursor--;
            }
            WriteDisk(diskMap);

            return diskMap.Sum(e => e.IsFile ? Enumerations.Range(e.StartIndex, 1, e.Size).Sum(i => i * e.Id) : 0);

            throw new NotImplementedException();
        }

        void WriteDisk(List<MapEntry> diskMap)
        {
            if (!this.IsTrace) return;

            foreach (var entry in diskMap.OrderBy(e => e.StartIndex))
            {
                for (int i = 0; i < entry.Size; i++)
                {
                    Write(entry.IsFile ? $"{entry.Id}" : ".");
                }
            }
            WriteLine();
        }
    }

    class MapEntry(long id, int size, bool isFile, long startIndex = 0)
    {
        public long Id { get; set; } = id;
        public int Size { get; set; } = size;

        public bool IsFile { get; set; } = isFile;

        public long StartIndex { get; set; } = startIndex;
        public long EndIndex => StartIndex + Size - 1;
    }
}
