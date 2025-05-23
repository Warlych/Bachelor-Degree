using System.Linq.Expressions;

namespace RailwaySections.Persistence.Abstractions;

public interface IExpressionBuilder
{
    (string whereQuery, IDictionary<string, object> parameters) Build(Expression expression);
}
