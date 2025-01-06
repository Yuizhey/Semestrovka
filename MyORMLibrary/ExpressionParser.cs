using System.Linq.Expressions;

namespace MyORMLibrary;

public static class ExpressionParser<T>
{
    // internal static string ParseExpression(Expression expression)
    // {
    //     if (expression is BinaryExpression binary)
    //     {
    //         // разбираем выражение на составляющие
    //         var left = ParseExpression(binary.Left);  // Левая часть выражения
    //         var right = ParseExpression(binary.Right); // Правая часть выражения
    //         var op = GetSqlOperator(binary.NodeType);  // Оператор (например, > или =)
    //         return $"({left} {op} {right})";
    //     }
    //     else if (expression is MemberExpression member)
    //     {
    //         return member.Member.Name; // Название свойства
    //     }
    //     else if (expression is ConstantExpression constant)
    //     {
    //         return FormatConstant(constant.Value); // Значение константы
    //     }
    //     else if (expression is ParameterExpression param)
    //     {
    //         return param.Name;
    //     }
    //
    //     // TODO: можно расширить для поддержки более сложных выражений (например, методов Contains, StartsWith и т.д.).
    //     // если не поддерживается то выбрасываем исключение
    //     throw new NotSupportedException($"Unsupported expression type: {expression.GetType().Name}");
    // }
 
    internal static string ParseExpression(Expression expression)
    {
        if (expression is BinaryExpression binary)
        {
            // разбираем выражение на составляющие
            var left = ParseExpression(binary.Left);  // Левая часть выражения
            var right = ParseExpression(binary.Right); // Правая часть выражения
            var op = GetSqlOperator(binary.NodeType);  // Оператор (например, > или =)
            return $"({left} {op} {right})";
        }
        else if (expression is MemberExpression member)
        {
            if (member.Expression is ConstantExpression constant)
            {
                // Извлекаем значение объекта
                var container = constant.Value;

                // Извлекаем имя свойства, к которому идет обращение
                var propertyName = member.Member.Name;

                // Получаем значение свойства через reflection
                var propertyInfo = container.GetType().GetField(propertyName);
                if (propertyInfo != null)
                {
                    return FormatConstant(propertyInfo.GetValue(container));
                }
            }
            return member.Member.Name; // Название свойства
        }
        else if (expression is ConstantExpression constant)
        {
            return FormatConstant(constant.Value); // Значение константы
        }

        // TODO: можно расширить для поддержки более сложных выражений (например, методов Contains, StartsWith и т.д.).
        // если не поддерживается то выбрасываем исключение
        throw new NotSupportedException($"Unsupported expression type: {expression.GetType().Name}");
    }
    private static string GetSqlOperator(ExpressionType nodeType)
    {
        return nodeType switch
        {
            ExpressionType.Equal => "=",
            ExpressionType.GreaterThan => ">",
            ExpressionType.LessThan => "<",
            ExpressionType.AndAlso => "AND",
            ExpressionType.Or => "OR",
            _ => throw new NotSupportedException($"Unsupported operator: {nodeType}")
        };
    }
 
    private static string FormatConstant(object value)
    {
        return value is string ? $"'{value}'" : value.ToString();
    }

    internal static string BuildSqlQuery<T>(Expression<Func<T, bool>> predicate, bool singleResult)
    {
        var query = $"SELECT * FROM Users WHERE {ParseExpression(predicate.Body)}";
        return query;
    }

}