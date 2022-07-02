using System.Linq.Expressions;

namespace Rtrw.Client.Wasm.Components.Extensions
{
    public static class ExpressionExtensions
    {
        internal static string GetFullPathOfMember<T>(this Expression<Func<T>> property)
        {
            var resultingString = string.Empty;
            var p = property.Body as MemberExpression;

            while (p != null)
            {
                if (p.Expression is MemberExpression)
                {
                    resultingString = p.Member.Name + (resultingString != string.Empty ? "." : string.Empty) + resultingString;
                }
                p = p.Expression as MemberExpression;
            }
            return resultingString;
        }
    }
}
