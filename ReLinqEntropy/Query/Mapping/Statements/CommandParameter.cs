namespace ReLinqEntropy.Query.Mapping.Statements
{
    public class CommandParameter
    {
        public CommandParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public object Value { get; }

        public override string ToString() => $"{Name}={Value}";
    }
}
