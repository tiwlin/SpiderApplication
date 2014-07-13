using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

using Microsoft.Practices.Unity.InterceptionExtension;

namespace DALHelper
{
    public static class TransactionFactroy<T> where T:class
    {
        private static Dictionary<string, T> cacheDic = new Dictionary<string, T>();

        /// <summary>
        /// Cache了,经测试性能并无多大提升,可不用   不灵活
        /// </summary>
        /// <returns></returns>
        public static T CreateTransactionInstanceCache()
        {
            string key=typeof(T).Name;
            if (!cacheDic.ContainsKey(key))
            { 
                cacheDic.Add(key,Intercept.NewInstance<T>(new VirtualMethodInterceptor(), new[] { new TransactionBehavior() }));
            }

            
           return cacheDic[key];
        }

        /// <summary>
        /// 对进行事务调用的对象进行包装(拦截)   很灵活
        /// </summary>
        /// <returns></returns>
        public static T CreateTransactionInstance()
        {
            return Intercept.NewInstance<T>(new VirtualMethodInterceptor(), new[] { new TransactionBehavior() });
        }
    }

    /// <summary>
    /// 实现事务的注入
    /// </summary>
    public class TransactionBehavior : IInterceptionBehavior, IDisposable
    {
        public TransactionBehavior() { }

        #region IInterceptionBehavior 成员

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input">被调用方法的信息wrap对象</param>
        /// <param name="getNext">委托连</param>
        /// <returns></returns>
        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                IMethodReturn methodReturn = getNext().Invoke(input, getNext);
                if (methodReturn.Exception == null)
                {
                    scope.Complete();
                }
                return methodReturn;
            }
        }

        public bool WillExecute
        {
            get { return true; }
        }



        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
           
        }

        #endregion
    }
}
