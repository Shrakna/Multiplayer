﻿using LiteNetLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Multiplayer.Common
{
    public static class Extensions
    {
        public static V AddOrGet<K, V>(this Dictionary<K, V> dict, K obj, V defaultValue)
        {
            if (!dict.TryGetValue(obj, out V value))
            {
                value = defaultValue;
                dict[obj] = value;
            }

            return value;
        }

        public static V GetOrAddDefault<K, V>(this Dictionary<K, V> dict, K obj) where V : new()
        {
            return AddOrGet(dict, obj, new V());
        }

        public static IEnumerable<T> ToEnumerable<T>(this T input)
        {
            yield return input;
        }

        public static int Combine(this int i1, int i2)
        {
            return i1 ^ (i2 << 16 | (i2 >> 16));
        }

        public static T[] Append<T>(this T[] arr1, params T[] arr2)
        {
            T[] result = new T[arr1.Length + arr2.Length];
            Array.Copy(arr1, 0, result, 0, arr1.Length);
            Array.Copy(arr2, 0, result, arr1.Length, arr2.Length);
            return result;
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static MpNetConnection GetConnection(this NetPeer peer)
        {
            return (MpNetConnection)peer.Tag;
        }

        public static void Write(this MemoryStream stream, byte[] arr)
        {
            if (arr.Length > 0)
                stream.Write(arr, 0, arr.Length);
        }

        public static IEnumerable<T> AllAttributes<T>(this MemberInfo member) where T : Attribute
        {
            return Attribute.GetCustomAttributes(member, typeof(T)).Cast<T>();
        }

        public static void Restart(this Stopwatch watch)
        {
            watch.Reset();
            watch.Start();
        }

        public static bool HasFlag(this Enum on, Enum flag)
        {
            ulong num = Convert.ToUInt64(flag);
            return (Convert.ToUInt64(on) & num) == num;
        }

        public static int FindIndex<T>(this T[] arr, T t)
        {
            return Array.IndexOf(arr, t);
        }

        public static double ElapsedMillisDouble(this Stopwatch watch)
        {
            return (double)watch.ElapsedTicks / Stopwatch.Frequency * 1000;
        }
    }

    public static class EnumerableHelper
    {
        public static void ProcessCombined<T, U>(IEnumerable<T> first, IEnumerable<U> second, Action<T, U> action)
        {
            using (var firstEnumerator = first.GetEnumerator())
            using (var secondEnumerator = second.GetEnumerator())
            {
                bool hasFirst = true;
                bool hasSecond = true;

                while ((hasFirst && (hasFirst = firstEnumerator.MoveNext())) | (hasSecond && (hasSecond = secondEnumerator.MoveNext())))
                {
                    action(hasFirst ? firstEnumerator.Current : default(T), hasSecond ? secondEnumerator.Current : default(U));
                }
            }
        }
    }

    public static class XmlExtensions
    {
        public static void SelectAndRemove(this XmlNode node, string xpath)
        {
            XmlNodeList nodes = node.SelectNodes(xpath);
            foreach (XmlNode selected in nodes)
                selected.RemoveFromParent();
        }

        public static void RemoveChildIfPresent(this XmlNode node, string child)
        {
            XmlNode childNode = node[child];
            if (childNode != null)
                node.RemoveChild(childNode);
        }

        public static void RemoveFromParent(this XmlNode node)
        {
            if (node == null) return;
            node.ParentNode.RemoveChild(node);
        }

        public static void AddNode(this XmlNode parent, string name, string value)
        {
            XmlNode node = parent.OwnerDocument.CreateElement(name);
            node.InnerText = value;
            parent.AppendChild(node);
        }
    }

    public static class Utils
    {
        public static byte[] GetMD5(IEnumerable<string> strings)
        {
            using (var hash = MD5.Create())
            {
                foreach (string s in strings)
                {
                    byte[] data = Encoding.UTF8.GetBytes(s);
                    hash.TransformBlock(data, 0, data.Length, null, 0);
                }

                hash.TransformFinalBlock(new byte[0], 0, 0);

                return hash.Hash;
            }
        }
    }

}