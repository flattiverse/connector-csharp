using System.Text.Json;

namespace Flattiverse
{
    internal class ClientCommandParameter
    {
        public readonly string Name;

        private string? text;
        private int? number;
        private double? doubleNumber;
        private Vector? vector;

        private CommandParameterKind kind;

        public CommandParameterKind Kind => kind;

        public ClientCommandParameter(string name)
        {
            Name = name;
        }

        public void SetValue(string text)
        {
            this.text = text;
            kind = CommandParameterKind.String;
        }

        public void SetValue(int number)
        {
            this.number = number;
            kind = CommandParameterKind.Integer;
        }

        public void SetValue(double doubleNumber)
        {
            this.doubleNumber = doubleNumber;
            kind = CommandParameterKind.Double;
        }

        public void SetValue(Vector vector)
        {
            this.vector = vector;
            kind = CommandParameterKind.Vector;
        }

        public void SetJsonValue(string text)
        {
            this.text = text;
            kind = CommandParameterKind.Json;
        }

        internal void WriteJson(Utf8JsonWriter writer)
        {
            switch (kind)
            {
                case CommandParameterKind.String:
                    writer.WriteString(Name, text);
                    break;
                case CommandParameterKind.Integer:
                    writer.WriteNumber(Name, (decimal)number!);
                    break;
                case CommandParameterKind.Double:
                    writer.WriteNumber(Name, (decimal)doubleNumber!);
                    break;
                case CommandParameterKind.Vector:
                    writer.WritePropertyName(Name);
                    vector!.WriteJson(writer);
                    break;
                case CommandParameterKind.Json:
                    writer.WritePropertyName(Name);
                    writer.WriteRawValue(text!);
                    break;
            }
        }
    }
}
