using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StateManagerLib.Extensions
{
    public static class ExpressionExt
    {
        public static string GetPropertyPathStr<T>(Expression<Func<T, object>> member) where T : class
        {
            Expression expression = null;
            if (member.Body is MemberExpression)
            {
                expression = (MemberExpression)member.Body;
            }
            else if (member.Body is UnaryExpression)
            {
                UnaryExpression unaryExpression = (UnaryExpression)member.Body;
                expression = unaryExpression.Operand;
            }

            if (expression == null)
            {
                return "";
            }

            IEnumerable<MemberExpression> source = ExpressionRecursive(expression);
            string[] value = (from a in source.Reverse()
                              select a.Member.Name).ToArray();
            return string.Join(".", value);
        }

        public static PropertyInfo? GetPropertyInfoFromPathStr(this Type type, string propertyPath)
        {
            ParameterExpression parameterExpression = Expression.Parameter(type);
            Expression expressionFieldFromType = type.GetExpressionFieldFromType(propertyPath, parameterExpression);
            LambdaExpression lambdaExpression = Expression.Lambda(expressionFieldFromType, parameterExpression);
            MemberExpression memberExpression = ((!(lambdaExpression.Body is UnaryExpression unaryExpression) || unaryExpression == null) ? ((MemberExpression)lambdaExpression.Body) : ((MemberExpression)unaryExpression.Operand));
            return (PropertyInfo)memberExpression.Member;
        }

        public static Expression GetExpressionFieldFromType(this Type type, string propertyPath, ParameterExpression param)
        {
            Expression expression = param;
            string[] array = propertyPath.Split('.');
            foreach (string propertyOrFieldName in array)
            {
                expression = Expression.PropertyOrField(expression, propertyOrFieldName);
            }

            return expression;
        }

        public static IEnumerable<MemberExpression> ExpressionRecursive(Expression expresion)
        {
            MemberExpression member = expresion as MemberExpression;
            if (member == null || member == null || member.Expression == null)
            {
                yield break;
            }

            yield return member;
            IEnumerable<MemberExpression> list = ExpressionRecursive(member.Expression);
            foreach (MemberExpression item in list)
            {
                yield return item;
            }
        }

        public static object? GetValueFromPropertyPath<T>(this T instance, string propertyPath) where T : class
        {
            object obj = instance;
            string[] array = propertyPath.Split('.');
            foreach (string name in array)
            {
                if (obj == null)
                {
                    return null;
                }

                obj = obj.GetType().GetProperty(name)?.GetValue(obj, null);
            }

            return obj;
        }
    }
}
