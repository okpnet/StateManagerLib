using System.Linq.Expressions;
using System.Reflection;

namespace LinqExtenssions
{
    /// <summary>
    /// Expressionファクトリ拡張クラス
    /// </summary>
    public static class ExpressionExtension
    {
        /// <summary>
        /// ラムダ式からパス取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public static string GetPropertyPathStr<T>(Expression<Func<T, object>> member) where T : class
        {
            Expression? expression = null;
            if (member.Body is MemberExpression)
            {
                expression = (MemberExpression)member.Body;
            }
            else if (member.Body is UnaryExpression)
            {
                var mb = (UnaryExpression)member.Body;
                expression = mb.Operand;
            }
            if (expression == null) return "";

            var list = ExpressionRecursive(expression);

            var strBuffers = list.Reverse().Select(a => a.Member.Name).ToArray();
            return string.Join(".", strBuffers);
        }
        /// <summary>
        /// メンバーパスからプロパティインフォ取得
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="propertyPath"></param>
        /// <returns></returns>
        public static PropertyInfo? GetPropertyInfoFromPathStr(this Type type, string propertyPath)
        {
            var param = Expression.Parameter(type);
            Expression body = type.GetExpressionFieldFromType(propertyPath,param);

            var lambda = Expression.Lambda(body, param);
            MemberExpression memberex;
            if (lambda.Body is UnaryExpression unary && unary != null)
            {
                memberex = (MemberExpression)unary.Operand;
            }
            else
            {
                memberex = (MemberExpression)lambda.Body;
            }

            var propInf = (PropertyInfo)memberex.Member;
            return propInf;
        }
        /// <summary>
        /// タイプとパスからフィールド式を取得
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="propertyPath"></param>
        /// <returns></returns>
        public static Expression GetExpressionFieldFromType(this Type type, string propertyPath,ParameterExpression param)
        {
            //var param = Expression.Parameter(type,"_arg_"+type.Name);
            Expression body = param;
            foreach (var member in propertyPath.Split('.'))
            {
                body = Expression.PropertyOrField(body, member);
            };
            return body;
        }
        /// <summary>
        /// メンバー式取得の再帰
        /// </summary>
        /// <param name="expresion"></param>
        /// <returns></returns>
        public static IEnumerable<MemberExpression> ExpressionRecursive(Expression expresion)
        {
            if (expresion is MemberExpression member && member != null)
            {
                if (member.Expression == null) yield break;
                yield return member;
                var list = ExpressionRecursive(member.Expression);
                foreach (var item in list)
                    yield return item;
            }
            yield break;
        }
        /// <summary>
        /// インスタンスとパスから、子プロパティを含めて検索してオブジェクトを取り出す
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyPath"></param>
        /// <returns></returns>
        public static object? GetValueFromPropertyPath<T>(this T instance, string propertyPath) where T:class
        {
            object? value = instance;
            foreach (var propName in propertyPath.Split('.'))
            {
                if (value is null) return null;
                var info = value.GetType().GetProperty(propName);
                value = info?.GetValue(value, null);
            }
            return value;
        }
    }
}
