using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AdmBoots.Infrastructure.Extensions {
    /// <summary>
    /// 类型<see cref="Type"/>辅助扩展方法类
    /// </summary>
    public static class TypeExtensions {
        /// <summary>
        /// 判断当前类型是否可由指定类型派生
        /// </summary>
        public static bool IsDeriveClassFrom(this Type type, Type baseType, bool canAbstract = false) {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (baseType == null)
                throw new ArgumentNullException(nameof(baseType));
            return type.IsClass && !canAbstract && !type.IsAbstract && type.IsBaseOn(baseType);
        }

        /// <summary>
        /// 判断类型是否为Nullable类型
        /// </summary>
        /// <param name="type"> 要处理的类型 </param>
        /// <returns> 是返回True，不是返回False </returns>
        public static bool IsNullableType(this Type type) {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// 通过类型转换器获取Nullable类型的基础类型
        /// </summary>
        /// <param name="type"> 要处理的类型对象 </param>
        /// <returns> </returns>
        public static Type GetUnNullableType(this Type type) {
            if (IsNullableType(type)) {
                NullableConverter nullableConverter = new NullableConverter(type);
                return nullableConverter.UnderlyingType;
            }
            return type;
        }

        /// <summary>
        /// 检查指定指定类型成员中是否存在指定的Attribute特性
        /// </summary>
        /// <typeparam name="T">要检查的Attribute特性类型</typeparam>
        /// <param name="memberInfo">要检查的类型成员</param>
        /// <param name="inherit">是否从继承中查找</param>
        /// <returns>是否存在</returns>
        public static bool HasAttribute<T>(this MemberInfo memberInfo, bool inherit = true) where T : Attribute {
            return memberInfo.IsDefined(typeof(T), inherit);
        }

        /// <summary>
        /// 从类型成员获取指定Attribute特性
        /// </summary>
        /// <typeparam name="T">Attribute特性类型</typeparam>
        /// <param name="memberInfo">类型类型成员</param>
        /// <param name="inherit">是否从继承中查找</param>
        /// <returns>存在返回第一个，不存在返回null</returns>
        public static T GetAttribute<T>(this MemberInfo memberInfo, bool inherit = true) where T : Attribute {
            //var descripts = memberInfo.GetCustomAttributes(typeof(T), inherit);
            //return descripts.FirstOrDefault() as T;
            var attrs = memberInfo.GetCustomAttributes(inherit).OfType<T>().ToArray();
            if (attrs.Length > 0) {
                return attrs[0];
            }

            attrs = memberInfo.DeclaringType.GetTypeInfo().GetCustomAttributes(inherit).OfType<T>().ToArray();
            if (attrs.Length > 0) {
                return attrs[0];
            }
            return null;
        }

        /// <summary>
        /// 判断当前泛型类型是否可由指定类型的实例填充
        /// </summary>
        /// <param name="genericType">泛型类型</param>
        /// <param name="type">指定类型</param>
        /// <returns></returns>
        public static bool IsGenericAssignableFrom(this Type genericType, Type type) {
            if (genericType == null)
                throw new ArgumentNullException(nameof(genericType));
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (!genericType.IsGenericType) {
                throw new ArgumentException("该功能只支持泛型类型的调用，非泛型类型可使用 IsAssignableFrom 方法。");
            }

            List<Type> allOthers = new List<Type> { type };
            if (genericType.IsInterface) {
                allOthers.AddRange(type.GetInterfaces());
            }

            foreach (var other in allOthers) {
                Type cur = other;
                while (cur != null) {
                    if (cur.IsGenericType) {
                        cur = cur.GetGenericTypeDefinition();
                    }
                    if (cur.IsSubclassOf(genericType) || cur == genericType) {
                        return true;
                    }
                    cur = cur.BaseType;
                }
            }
            return false;
        }

        /// <summary>
        /// 返回当前类型是否是指定基类的派生类
        /// </summary>
        /// <param name="type">当前类型</param>
        /// <param name="baseType">要判断的基类型</param>
        /// <returns></returns>
        public static bool IsBaseOn(this Type type, Type baseType) {
            if (baseType.IsGenericTypeDefinition) {
                return baseType.IsGenericAssignableFrom(type);
            }
            return baseType.IsAssignableFrom(type);
        }
    }
}
