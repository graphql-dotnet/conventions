using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
                ThrowArgumentNull(nameof(methodInfo));
            //the following check is more descriptive than the NullReferenceException that would otherwise occur
            // ReSharper disable once PossibleNullReferenceException
            if (!methodInfo.IsStatic && instance == null)
                ThrowArgumentNull(nameof(instance), "Instance is required for non static methods");
            //the lambda ignores extra arguments so the following check is necessary
            if (methodInfo.GetParameters().Length != (arguments?.Length ?? 0))
                ThrowArgument("Invalid number of arguments for this method", nameof(arguments));
            //type errors will be thrown as InvalidCastException inside the lambda

            var lambda = MethodDictionary.GetOrAdd(methodInfo, InvokeEnhancedUncachedMethod);
            return lambda(instance, arguments);
        }

        private static void ThrowArgumentNull(string name, string description = null)
        {
            if (description == null)
                throw new ArgumentNullException(name);
            throw new ArgumentNullException(name, description);
        }

        private static void ThrowArgument(string message, string name) =>
            throw new ArgumentException(message, name);

        private static readonly ConcurrentDictionary<MethodInfo, Func<object, object[], object>> MethodDictionary = new ConcurrentDictionary<MethodInfo, Func<object, object[], object>>();

        private static Func<object, object[], object> InvokeEnhancedUncachedMethod(MethodInfo methodInfo)
        {
            var instanceParameter = Expression.Parameter(typeof(object));
            var argumentsParameter = Expression.Parameter(typeof(object[]));
            // ReSharper disable once AssignNullToNotNullAttribute
            var instanceExpression = methodInfo.IsStatic ? null : Expression.Convert(instanceParameter, methodInfo.DeclaringType);
            var methodParameters = methodInfo.GetParameters();
            var parameters = methodParameters.Select((param, index) => Expression.Convert(Expression.ArrayAccess(argumentsParameter, Expression.Constant(index)), param.ParameterType));
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

        /// <summary>
        /// Invokes a constructor represented by a specified <see cref="ConstructorInfo"/>, using
        /// the specified parameters.
        /// </summary>
        /// <param name="constructorInfo">
        /// The constructor representation. Must not be a static constructor.
        /// </param>
        /// <param name="arguments">
        /// An argument list for the invoked constructor. This is an array of objects
        /// with the same number, order, and type as the parameters of the constructor
        /// to be invoked. If there are no parameters, parameters should be null.
        /// Ref/out parameters are not supported.
        /// </param>
        /// <returns>
        /// The constructed object.
        /// </returns>
        public static object InvokeEnhanced(this ConstructorInfo constructorInfo, object[] arguments)
        {
            //just good practice
            if (constructorInfo == null)
                throw new ArgumentNullException(nameof(constructorInfo));
            //the lambda ignores extra arguments so the following check is necessary
            if (constructorInfo.GetParameters().Length != (arguments?.Length ?? 0))
                throw new ArgumentException("Invalid number of arguments for this constructor", "arguments");
            //type errors will be thrown as InvalidCastException inside the lambda

            var lambda = _constructorDictionary.GetOrAdd(constructorInfo, InvokeEnhancedUncachedConstructor);

            return lambda(arguments);
        }
        private static ConcurrentDictionary<ConstructorInfo, Func<object[], object>> _constructorDictionary = new ConcurrentDictionary<ConstructorInfo, Func<object[], object>>();
        private static Func<object[], object> InvokeEnhancedUncachedConstructor(ConstructorInfo constructorInfo)
        {
            if (constructorInfo.IsStatic)
                throw new ArgumentException("Constructor must not be static", nameof(constructorInfo));
            var argumentsParameter = Expression.Parameter(typeof(object[]));
            var constructorParameters = constructorInfo.GetParameters();
            var parameters = constructorParameters.Select((param, index) =>
            {
                return Expression.Convert(Expression.ArrayAccess(argumentsParameter, Expression.Constant(index)), param.ParameterType);
            });
            var call = Expression.New(constructorInfo, parameters);
            var body = Expression.Convert(call, typeof(object));
            var lambda = Expression.Lambda<Func<object[], object>>(body, argumentsParameter);
            return lambda.Compile();
        }
    }
}
