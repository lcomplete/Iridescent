using System;
using System.Collections.Generic;
using System.Data;

namespace Iridescent.Utils.Common
{
    public sealed class OrmUtils
    {
         public static TEntity ReadToEntity<TEntity>(IDataReader reader) where TEntity:class 
         {
             if (reader.Read())
             {
                 TEntity entity = (TEntity)Activator.CreateInstance(typeof(TEntity));
                 for (int i = 0; i < reader.FieldCount; i++)
                 {
                     if (!reader.IsDBNull(i))
                     {
                         ReflectHelper.SetPropertyValue(entity, reader.GetName(i), reader.GetValue(i));
                     }
                 }
                 return entity;
             }
             return null;
         }

         public static IList<TEntity> ReadToEntityList<TEntity>(IDataReader reader) where TEntity:class 
         {
             IList<TEntity> list = new List<TEntity>();
             TEntity entity;
             while ((entity = ReadToEntity<TEntity>(reader)) != null)
             {
                 list.Add(entity);
             }
             return list;
         }
    }
}