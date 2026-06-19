namespace Practice.Patterns
{
    public class DocumentMemento
    {
        public string State { get; private set; }

        public DocumentMemento(string state)
        {
            State = state;
        }
    }
}