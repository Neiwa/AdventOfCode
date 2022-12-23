namespace Core
{
    public abstract class BaseAocV2 : BaseAoc<List<string>>
    {
        public bool Debug
        {
            get => IsDebug;
            protected set => Level = value ? ActionLevel.Debug : ActionLevel.Info;
        }
        protected override List<string> ParseInput(List<string> lines)
        {
            return lines;
        }

        public abstract override string PartOne(List<string> lines);

        public abstract override string PartTwo(List<string> lines);
    }
}