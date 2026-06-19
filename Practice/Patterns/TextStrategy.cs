namespace Practice.Patterns
{
    public interface ITextStrategy
    {
        string ProcessText(string text);
    }

    public class UpperCaseStrategy : ITextStrategy
    {
        public string ProcessText(string text) => text.ToUpper();
    }

    public class LowerCaseStrategy : ITextStrategy
    {
        public string ProcessText(string text) => text.ToLower();
    }

    public class TextProcessor
    {
        private ITextStrategy _strategy;
        public void SetStrategy(ITextStrategy strategy) => _strategy = strategy;
        public string Execute(string text) => _strategy != null ? _strategy.ProcessText(text) : text;
    }
}