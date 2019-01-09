using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Collections;
using System;

namespace yeetong_DataStorage
{
    /// <summary>
    /// 扩展属性
    /// </summary>
    public static class Extensions
    {
        #region 数据集转换泛型集合扩展
        /// <summary>         
        /// DataSet转换为泛型集合         
        /// </summary>         
        /// <typeparam name="T">泛型类型</typeparam>         
        /// <param name="ds">DataSet数据集</param>         
        /// <param name="tableIndex">待转换数据表索引,默认第0张表</param>         
        /// <returns>返回泛型集合</returns>         
        public static IList<T> ToList<T>(this DataSet ds, int tableIndex = 0)
        {
            if (ds == null || ds.Tables.Count < 0) return null;
            if (tableIndex > ds.Tables.Count - 1)
                return null;
            if (tableIndex < 0)
                tableIndex = 0;
            DataTable dt = ds.Tables[tableIndex];
            // 返回值初始化             
            IList<T> result = new List<T>();
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                T _t = (T)Activator.CreateInstance(typeof(T));
                PropertyInfo[] propertys = _t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        // 属性与字段名称一致的进行赋值                         
                        if (pi.Name.Equals(dt.Columns[i].ColumnName))
                        {
                            // 数据库NULL值单独处理                             
                            if (dt.Rows[j][i] != DBNull.Value)
                                pi.SetValue(_t, dt.Rows[j][i], null);
                            else
                                pi.SetValue(_t, null, null);
                            break;
                        }
                    }
                }
                result.Add(_t);
            }
            return result;
        }
        /// <summary>         
        /// DataTable转换为泛型集合         
        /// </summary>         
        /// <typeparam name="T">泛型类型</typeparam>         
        /// <param name="dt">DataTable数据表</param>         
        /// <returns>返回泛型集合</returns>         
        public static IList<T> ToList<T>(this DataTable dt) where T:class,new()
        {
            if (dt == null || dt.Rows.Count <= 0) return null;
            List<T> list =new List<T>();
            #region 方法一：
            //T model;
            //Type infos = typeof(T);
            ////object tempValue;
            //foreach (DataRow dr in dt.Rows)
            //{
            //    model = new T();
            //    1.
            //    infos.GetProperties().ToList().ForEach(p =>p.SetValue(model, dr[p.Name], null));
            //     2.
            //    //infos.GetProperties().ToList().ForEach(p =>
            //    //{
            //    //    tempValue = dr[p.Name];
            //    //    if (!string.IsNullOrEmpty(tempValue.ToString()))
            //    //        p.SetValue(model, tempValue, null);
            //    //});
            //    list.Add(model);
            //}
            #endregion
            #region 方法二：    比方法一快
            PropertyInfo[] propertys = typeof(T).GetProperties();
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                T _t = (T)Activator.CreateInstance(typeof(T));
                foreach (PropertyInfo pi in propertys)
                {
                    // 属性与字段名称一致的进行赋值                         
                    if (pi.Name.Equals(dt.Columns[pi.Name].ColumnName))
                    {
                        if (dt.Rows[j][pi.Name] != DBNull.Value)
                            pi.SetValue(_t, dt.Rows[j][pi.Name].ToString(), null);
                        else
                            pi.SetValue(_t, null, null);
                    }
                }
                list.Add(_t);
            }
            #endregion
            return list;
        }
        /// <summary>         
        /// DataSet转换为泛型集合         
        /// </summary>         
        /// <typeparam name="T">泛型类型</typeparam>         
        /// <param name="ds">DataSet数据集</param>         
        /// <param name="tableName">待转换数据表名称,名称为空时默认第0张表</param>         
        /// <returns>返回泛型集合</returns>         
        public static IList<T> ToList<T>(this DataSet ds, string tableName)
        {
            int _TableIndex = 0;
            if (ds == null || ds.Tables.Count < 0)
                return null;
            if (string.IsNullOrEmpty(tableName))
                return ToList<T>(ds, 0);
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                // 获取Table名称在Tables集合中的索引值                 
                if (ds.Tables[i].TableName.Equals(tableName))
                {
                    _TableIndex = i;
                    break;
                }
            }
            return ToList<T>(ds, _TableIndex);
        }
#endregion
        #region 泛型集合转换为数据集扩展
        /// <summary>
        /// 将泛型集合转换成DataSet数据集
        /// </summary>
        /// <typeparam name="T">T类型</typeparam>
        /// <param name="list">泛型集合</param>
        /// <returns>DataSet数据集</returns>
        public static DataSet ToDataSet<T>(this IList<T> list)
        {
            Type elementType = typeof(T);
            var ds = new DataSet();
            var t = new DataTable();
            ds.Tables.Add(t);
            elementType.GetProperties().ToList().ForEach(propInfo => t.Columns.Add(propInfo.Name, Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType));
            foreach (T item in list)
            {
                var row = t.NewRow();
                elementType.GetProperties().ToList().ForEach(propInfo => row[propInfo.Name] = propInfo.GetValue(item, null) ?? DBNull.Value);
                t.Rows.Add(row);
            }
            return ds;
        }
        /// <summary>
        /// 泛型集合转换成DataTable表
        /// </summary>
        /// <typeparam name="T">T类型</typeparam>
        /// <param name="list">泛型集合</param>
        /// <returns>DataTable表</returns>
        public static DataTable ToDataTable<T>(this IList<T> list)
        {
            return ToDataTable(list, null);
        }
        /// <summary>
        /// 泛型集合转换成DataTable表
        /// </summary>
        /// <typeparam name="T">T类型</typeparam>
        /// <param name="list">泛型集合</param>
        /// <param name="_tableName">Table名称</param>
        /// <returns>DataTable表</returns>
        public static DataTable ToDataTable<T>(this IList<T> list, string _tableName)
        {
            Type elementType = typeof(T);
            var dt = new DataTable();
            if (_tableName == "null")
                dt.TableName = _tableName;
            elementType.GetProperties().ToList().ForEach(propInfo => dt.Columns.Add(propInfo.Name, Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType));
            foreach (T item in list)
            {
                var row = dt.NewRow();
                elementType.GetProperties().ToList().ForEach(propInfo => row[propInfo.Name] = propInfo.GetValue(item, null) ?? DBNull.Value);
                dt.Rows.Add(row);
            }
            return dt;
        }
        #endregion
        #region 以下为IEnumerable扩展实现
        /// <summary>
        /// 给非强类型的IEnumerable返回头一个元素。
        /// </summary>
        public static object First(this IEnumerable col)
        {
            foreach (var item in col)
                return item;
            throw new IndexOutOfRangeException();
        }
        /// <summary>
        /// 给非强类型的IEnumerable返回头一个强类型的元素
        /// </summary>
        public static object First<T>(this IEnumerable col)
        {
            return (T)col.First();
        }
        /// <summary>
        /// 基本上和List<T>的ForEach方法一致。
        /// </summary>
        public static void Each<T>(this IEnumerable<T> col, Action<T> handler)
        {
            foreach (var item in col)
                handler(item);
        }
        /// <summary>
        /// 带索引的遍历方法。
        /// </summary>
        public static void Each<T>(this IEnumerable<T> col, Action<T, int> handler)
        {
            int index = 0;
            foreach (var item in col)
                handler(item, index++);
        }
        /// <summary>
        /// 可以半途中断执行的遍历方法。
        /// </summary>
        public static void Each<T>(this IEnumerable<T> col, Func<T, bool> handler)
        {
            foreach (var item in col)
                if (!handler(item)) break;
        }
        /// <summary>
        /// 可以半途中段的带索引的遍历方法。
        /// </summary>
        public static void Each<T>(this IEnumerable<T> col, Func<T, int, bool> handler)
        {
            int index = 0;
            foreach (var item in col)
                if (!handler(item, index++)) break;
        }
        #endregion
        #region 以下为IEnumerable<T>的非泛型实现
        public static void Each<T>(this IEnumerable col, Action<object> handler)
        {
            foreach (var item in col)
                handler(item);
        }
        public static void Each<T>(this IEnumerable col, Action<object, int> handler)
        {
            int index = 0;
            foreach (var item in col)
                handler(item, index++);
        }
        public static void Each<T>(this IEnumerable col, Func<object, bool> handler)
        {
            foreach (var item in col)
                if (!handler(item)) break;
        }
        public static void Each<T>(this IEnumerable col, Func<object, int, bool> handler)
        {
            int index = 0;
            foreach (var item in col)
                if (!handler(item, index++)) break;
        }
        #endregion
    }
}