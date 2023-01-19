using System.Reflection;
using System.Text.Json;

namespace Flattiverse
{
    internal class CommandParameter
    {
        public readonly string Name;
        public readonly CommandParameterKind Kind;
        public readonly bool CanBeNull;

        public CommandParameter(ParameterInfo parameterInfo)
        {
            if (parameterInfo.Name == null)
                throw new ArgumentNullException("parameterInfo.Name", "parameterInfo.Name shouldn't be null.");

            Name = parameterInfo.Name;

            if (parameterInfo.ParameterType == typeof(string))
                Kind = CommandParameterKind.String;
            else if (parameterInfo.ParameterType == typeof(int))
                Kind = CommandParameterKind.Integer;
            else if (parameterInfo.ParameterType == typeof(double))
                Kind = CommandParameterKind.Double;
            else if (parameterInfo.ParameterType == typeof(Vector))
                Kind = CommandParameterKind.Vector;
            else if (parameterInfo.ParameterType == typeof(JsonElement))
                Kind = CommandParameterKind.Json;
            else
                throw new InvalidProgramException("The command router does only understand string, int, double, Vector or JsonElement.");

            CanBeNull = parameterInfo.ParameterType.IsGenericType &&
                parameterInfo.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>);

            Console.WriteLine($" * Parameter {Name} is Nullable? {CanBeNull}.");
        }
    }
}
