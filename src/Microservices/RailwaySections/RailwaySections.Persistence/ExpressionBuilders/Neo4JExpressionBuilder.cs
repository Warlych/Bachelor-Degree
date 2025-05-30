using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using RailwaySections.Domain.RailwaySections.ValueObjects.RailwaySections;
using RailwaySections.Persistence.Abstractions;

namespace RailwaySections.Persistence.ExpressionBuilders;

internal sealed class Neo4JExpressionBuilder : ExpressionVisitor, IExpressionBuilder
{
    private readonly StringBuilder _builder = new();
    private readonly Dictionary<string, object> _parameters = new();
    private int _paramCounter = 0;

    public (string whereQuery, IDictionary<string, object> parameters) Build(Expression expression)
    {
        if (expression is null)
        {
            return ("true", _parameters);
        }
        
        Visit(expression);
        
        return (_builder.ToString(), _parameters);
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        _builder.Append("(");
        
        Visit(node.Left);
        
        _builder.Append($" {GetOperator(node.NodeType)} ");
        
        Visit(node.Right);
        
        _builder.Append(")");
        
        return node;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Expression is MemberExpression inner && inner.Expression?.NodeType == ExpressionType.Parameter)
        {
            var propertyPath = $"{inner.Member.Name}.{node.Member.Name}";
            var dbProperty = MapPropertyPath(propertyPath);
            
            _builder.Append($"r.{dbProperty}");
        }
        else if (node.Expression?.NodeType == ExpressionType.Parameter)
        {
            _builder.Append($"r.{node.Member.Name}");
        }
        else
        {
            var value = Expression.Lambda(node).Compile().DynamicInvoke();
            var paramName = AddParameter(value);
            
            _builder.Append($"${paramName}");
        }

        return node;
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        var paramName = AddParameter(node.Value);
        _builder.Append($"${paramName}");
        
        return node;
    }

    private string AddParameter(object? value)
    {
        if (value is RailwaySectionId id)
        {
            value = id.Identity.ToString();
        }
        
        var name = $"p{_paramCounter++}";
        _parameters[name] = value!;
        
        return name;
    }

    private static string GetOperator(ExpressionType type) => type switch
    {
        ExpressionType.Equal => "=",
        ExpressionType.NotEqual => "<>",
        ExpressionType.LessThan => "<",
        ExpressionType.LessThanOrEqual => "<=",
        ExpressionType.GreaterThan => ">",
        ExpressionType.GreaterThanOrEqual => ">=",
        _ => throw new NotSupportedException($"Operator {type} not supported")
    };
    
    private static string MapPropertyPath(string path) => path switch
    {
        "Parameters.RailwayCode" => "railwayCode",
        "Parameters.UnifiedNetworkMarking" => "unifiedNetworkMarking",
        _ => throw new NotSupportedException($"Property path '{path}' is not supported.")
    };
}
