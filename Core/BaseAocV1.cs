namespace Core
{
    public abstract class BaseAocV1 : BaseAoc
    {
        public override string PartOne(List<string> lines)
        {
            PartOneV1(lines);

            return string.Empty;
        }
        public override string PartTwo(List<string> lines)
        {
            PartTwoV1(lines);

            return string.Empty;
        }

        public abstract void PartOneV1(List<string> lines);

        public abstract void PartTwoV1(List<string> lines);
    }
}