using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqExtenssions
{
    public static class LinqExtenssion
    {
        /// <summary>
        /// 重複リストを返す
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<TKey> FindDuplication<T,TKey>(this IEnumerable<T> source, Func<T, TKey> predicate)
        {
            return source.GroupBy(predicate).Where(t=>t.Count()>1).Select(t=>t.Key);
        }
        /// <summary>
        /// NULL除外
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> source)
        {
            if (source == null) return Enumerable.Empty<T>();
            return source.Where(x => x != null)!;
        }
        /// <summary>
        /// キャストした値をOutにセットして､値を返す
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool IfDeclare<TResult>(this TResult value, out TResult result)
        {
            result = value;
            return result is not null;
        }
    }
}
