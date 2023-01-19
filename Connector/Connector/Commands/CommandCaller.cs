using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Flattiverse
{
    internal class CommandCaller
    {
        private MethodInfo method;
        private CommandParameter[] parameters;

        public CommandCaller(MethodInfo method)
        {
            this.method = method;
            List<CommandParameter> parameters = new List<CommandParameter>();

            foreach (ParameterInfo parameter in method.GetParameters())
                parameters.Add(new CommandParameter(parameter));

            this.parameters = parameters.ToArray();
        }

        public async Task<bool> Call(Connection connection, JsonElement request)
        {
            JsonElement subElement;

            List<object?> callParameters = new List<object?>(parameters.Length);

            foreach (CommandParameter parameter in parameters)
            {
                JsonElement parameterElement;

                if (!request.TryGetProperty(parameter.Name, out parameterElement))
                {
                    if (!parameter.CanBeNull)
                    {
                        await connection.payloadExceptionSocketClose(new Exception($"Missing required parameter {parameter.Name} ({parameter.Kind})."));
                        return false;
                    }

                    callParameters.Add(null);

                    continue;
                }

                if (parameter.CanBeNull && (parameterElement.ValueKind == JsonValueKind.Null || parameterElement.ValueKind == JsonValueKind.Undefined))
                {
                    callParameters.Add(null);

                    continue;
                }

                switch (parameter.Kind)
                {
                    case CommandParameterKind.String:
                        if (parameterElement.ValueKind != JsonValueKind.String)
                        {
                            await connection.payloadExceptionSocketClose(new Exception($"Parameter {parameter.Name} must be {parameter.Kind}, got {parameterElement.ValueKind} instead."));
                            return false;
                        }

                        string? strValue = parameterElement.GetString();

                        if (strValue == null && !parameter.CanBeNull)
                        {
                            await connection.payloadExceptionSocketClose(new Exception($"Parameter {parameter.Name} can't be empty string."));
                            return false;
                        }

                        callParameters.Add(strValue);
                        break;
                    case CommandParameterKind.Integer:
                        if (parameterElement.ValueKind != JsonValueKind.Number)
                        {
                            await connection.payloadExceptionSocketClose(new Exception($"Parameter {parameter.Name} must be {parameter.Kind}, got {parameterElement.ValueKind} instead."));
                            return false;
                        }

                        int intValue;

                        if (!parameterElement.TryGetInt32(out intValue))
                        {
                            await connection.payloadExceptionSocketClose(new Exception($"Parameter {parameter.Name} couldn't be parsed as integer."));
                            return false;
                        }

                        callParameters.Add(intValue);
                        break;
                    case CommandParameterKind.Double:
                        if (parameterElement.ValueKind != JsonValueKind.Number)
                        {
                            await connection.payloadExceptionSocketClose(new Exception($"Parameter {parameter.Name} must be {parameter.Kind}, got {parameterElement.ValueKind} instead."));
                            return false;
                        }

                        double doubleValue;

                        if (!parameterElement.TryGetDouble(out doubleValue))
                        {
                            await connection.payloadExceptionSocketClose(new Exception($"Parameter {parameter.Name} couldn't be parsed as double."));
                            return false;
                        }

                        if (double.IsInfinity(doubleValue) || double.IsNaN(doubleValue))
                        {
                            await connection.payloadExceptionSocketClose(new Exception($"Parameter {parameter.Name} can't be special kind of double like Infinity or NaN."));
                            return false;
                        }

                        callParameters.Add(doubleValue);
                        break;
                    case CommandParameterKind.Vector:
                        if (parameterElement.ValueKind != JsonValueKind.Object)
                        {
                            await connection.payloadExceptionSocketClose(new Exception($"Parameter {parameter.Name} must be JSON object, got {parameterElement.ValueKind} instead."));
                            return false;
                        }

                        double x;
                        double y;

                        if (!parameterElement.TryGetProperty("x", out subElement))
                        {
                            await connection.payloadExceptionSocketClose(new Exception($"Parameter {parameter.Name} doesn't contain x sub-parameter."));
                            return false;
                        }

                        if (!subElement.TryGetDouble(out x))
                        {
                            await connection.payloadExceptionSocketClose(new Exception($"Parameter {parameter.Name} sub-parameter x doesn't contain a double."));
                            return false;
                        }

                        if (!parameterElement.TryGetProperty("y", out subElement))
                        {
                            await connection.payloadExceptionSocketClose(new Exception($"Parameter {parameter.Name} doesn't contain y sub-parameter."));
                            return false;
                        }

                        if (!subElement.TryGetDouble(out y))
                        {
                            await connection.payloadExceptionSocketClose(new Exception($"Parameter {parameter.Name} sub-parameter y doesn't contain a double."));
                            return false;
                        }

                        Vector vector = new Vector(x, y);

                        if (vector.IsDamaged)
                        {
                            await connection.payloadExceptionSocketClose(new Exception($"Vectors can't contain a special kind of double like Infinity or NaN.\"."));
                            return false;
                        }

                        callParameters.Add(vector);
                        break;
                    case CommandParameterKind.Json:
                        if (parameterElement.ValueKind != JsonValueKind.Object)
                        {
                            await connection.payloadExceptionSocketClose(new Exception($"Parameter {parameter.Name} must be a JSON object, got {parameterElement.ValueKind} instead."));
                            return false;
                        }

                        callParameters.Add(parameterElement);

                        break;
                    default:
                        await connection.payloadExceptionSocketClose(new Exception($"This error shouldn't happen. Something went utterly wrong. (ErrorCode 3bEA72dt)"));
                        return false;
                }
            }

            object? result = method.Invoke(connection, callParameters.ToArray());

            if (result is bool res)
                return res;
            else if (result is Task<bool> taskRes)
                return taskRes.Result;

            await connection.payloadExceptionSocketClose(new Exception($"This error shouldn't happen. Something went utterly wrong. (ErrorCode 8DFj328a)"));
            return false;         
        }
    }
}
