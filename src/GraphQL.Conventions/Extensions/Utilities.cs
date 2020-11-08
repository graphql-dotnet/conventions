using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography.X509Certificates;

namespace GraphQL.Conventions.Extensions
{
    public static class Utilities
    {
        public static string IdentifierForTypeOrNull<T>(this string id) =>
            id.IsIdentifierForType<T>() ? id.IdentifierForType<T>() : null;

        public static string IdentifierForTypeOrNull<T>(this NonNull<string> id) =>
            id.IsIdentifierForType<T>() ? id.IdentifierForType<T>() : null;

        public static string IdentifierForType<T>(this string id) =>
            new Id(id).IdentifierForType<T>();

        public static string IdentifierForType<T>(this NonNull<string> id) =>
            id.Value.IdentifierForType<T>();

        public static bool IsIdentifierForType<T>(this string id) =>
            new Id(id).IsIdentifierForType<T>();

        public static bool IsIdentifierForType<T>(this NonNull<string> id) =>
            id.Value.IsIdentifierForType<T>();

        /// <summary>
        /// Invokes a method represented by a specified <see cref="MethodInfo"/>, using
        /// the specified parameters.
        /// </summary>
        /// <param name="methodInfo">
        /// The method representation.
        /// </param>
        /// <param name="instance">
        /// The object on which to invoke the method. If the method is static, this
        /// value is ignored.
        /// </param>
        /// <param name="arguments">
        /// An argument list for the invoked method. This is an array of objects
        /// with the same number, order, and type as the parameters of the method
        /// to be invoked. If there are no parameters, parameters should be null.
        /// Ref/out parameters are not supported.
        /// </param>
        /// <returns>
        /// The return value from the method, or null for methods that return void.
        /// </returns>
        public static object InvokeEnhanced(this MethodInfo methodInfo, object instance, object[] arguments)
        {
            //just good practice
            if (methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));
            //the following check is more descriptive than the NullReferenceException that would otherwise occur
            if (!methodInfo.IsStatic && instance == null)
                throw new ArgumentNullException(nameof(instance), "Instance is required for static methods");
            //the lambda ignores extra arguments so the following check is necessary
            if (methodInfo.GetParameters().Length != (arguments?.Length ?? 0))
                throw new ArgumentException("Invalid number of arguments for this method", "arguments");
            //type errors will be thrown as InvalidCastException inside the lambda

            var lambda = _methodDictionary.GetOrAdd(methodInfo, InvokeEnhancedUncached);

            return lambda(instance, arguments);
        }
        private static ConcurrentDictionary<MethodInfo, Func<object, object[], object>> _methodDictionary = new ConcurrentDictionary<MethodInfo, Func<object, object[], object>>();
        private static Func<object, object[], object> InvokeEnhancedUncached(MethodInfo methodInfo)
        {
            var instanceParameter = Expression.Parameter(typeof(object));
            var argumentsParameter = Expression.Parameter(typeof(object[]));
            var instanceExpression = methodInfo.IsStatic ? null : Expression.Convert(instanceParameter, methodInfo.DeclaringType);
            var methodParameters = methodInfo.GetParameters();
            var parameters = methodParameters.Select((param, index) => {
                return Expression.Convert(Expression.ArrayAccess(argumentsParameter, Expression.Constant(index)), param.ParameterType);
            });
            var call = Expression.Call(instanceExpression, methodInfo, parameters);
            Expression body;
            if (methodInfo.ReturnType == typeof(void))
            {
                body = Expression.Block(new Expression[] {
                    call,
                    Expression.Constant(null, typeof(object))
                });
            }
            else
            {
                body = Expression.Convert(call, typeof(object));
            }
            var lambda = Expression.Lambda<Func<object, object[], object>>(body, instanceParameter, argumentsParameter);
            return lambda.Compile();
        }

        public static object InvokeEnhanced(this ConstructorInfo constructorInfo, object[] parameters)
        {
            try
            {
                return constructorInfo.Invoke(parameters);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw ex.InnerException; //required for intellisense
            }
        }
    }
}
