﻿/*
Copyright (c) 2014 <a href="http://www.gutgames.com">James Craig</a>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.*/

#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Utilities.DataTypes;
using Utilities.IoC.Interfaces;
using Utilities.ORM.Interfaces;
using Utilities.ORM.Manager.Aspect.Interfaces;
using Utilities.ORM.Manager.Mapper.Interfaces;
using Utilities.ORM.Manager.QueryProvider.Interfaces;
using Utilities.ORM.Manager.Schema.Enums;
using Utilities.ORM.Manager.Schema.Interfaces;
using Utilities.ORM.Manager.SourceProvider.Interfaces;

#endregion Usings

namespace Utilities.ORM.Manager
{
    /// <summary>
    /// Session object
    /// </summary>
    public class Session
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Session()
        {
            QueryProvider = IoC.Manager.Bootstrapper.Resolve<QueryProvider.Manager>();
            SourceProvider = IoC.Manager.Bootstrapper.Resolve<SourceProvider.Manager>();
            MapperProvider = IoC.Manager.Bootstrapper.Resolve<Mapper.Manager>();
        }

        /// <summary>
        /// Mapper provider
        /// </summary>
        private Mapper.Manager MapperProvider { get; set; }

        /// <summary>
        /// Query provider
        /// </summary>
        private QueryProvider.Manager QueryProvider { get; set; }

        /// <summary>
        /// Source provider
        /// </summary>
        private SourceProvider.Manager SourceProvider { get; set; }

        /// <summary>
        /// Returns all items that match the criteria
        /// </summary>
        /// <typeparam name="ObjectType">Type of the object</typeparam>
        /// <param name="Parameters">Parameters used in the where clause</param>
        /// <returns>All items that match the criteria</returns>
        public IEnumerable<ObjectType> All<ObjectType>(params IParameter[] Parameters)
            where ObjectType : class,new()
        {
            List<ObjectType> ReturnValue = new List<ObjectType>();
            foreach (ISourceInfo Source in SourceProvider.Where(x => x.Readable).OrderBy(x => x.Order))
            {
                IMapping Mapping = MapperProvider[typeof(ObjectType)].FirstOrDefault(x => x.DatabaseConfigType == Source.Database.GetType());
                foreach (dynamic Item in QueryProvider.Generate<ObjectType>(Source).All(Parameters).Execute().FirstOrDefault())
                {
                    ObjectType Temp = Item;
                    IProperty<ObjectType> IDProperty = (IProperty<ObjectType>)Mapping.IDProperties.FirstOrDefault();
                    object IDValue = IDProperty.GetValue(Temp);
                    ObjectType Value = ReturnValue.FirstOrDefault(x => IDProperty.GetValue(x) == IDValue);
                    if (Value == default(ObjectType))
                        ReturnValue.Add(Temp);
                    else
                    {
                        Item.CopyTo(Value);
                    }
                }
            }
            return ReturnValue;
        }

        /// <summary>
        /// Returns a single item matching the criteria
        /// </summary>
        /// <typeparam name="ObjectType">Type of the object</typeparam>
        /// <param name="Parameters">Parameters used in the where clause</param>
        /// <returns>A single object matching the criteria</returns>
        public ObjectType Any<ObjectType>(params IParameter[] Parameters)
            where ObjectType : class,new()
        {
            ObjectType ReturnValue = null;
            foreach (ISourceInfo Source in SourceProvider.Where(x => x.Readable).OrderBy(x => x.Order))
            {
                dynamic Value = QueryProvider.Generate<ObjectType>(Source).Any(Parameters).Execute().FirstOrDefault().FirstOrDefault();
                if (ReturnValue == null)
                    ReturnValue = Value;
                else
                    Value.CopyTo(ReturnValue);
            }
            return ReturnValue;
        }

        /// <summary>
        /// Deletes an object from the database
        /// </summary>
        /// <typeparam name="ObjectType">Object type</typeparam>
        /// <param name="Object">Object to delete</param>
        public void Delete<ObjectType>(ObjectType Object)
            where ObjectType : class,new()
        {
            foreach (ISourceInfo Source in SourceProvider.Where(x => x.Writable).OrderBy(x => x.Order))
            {
                QueryProvider.Generate<ObjectType>(Source)
                    .Delete(Object)
                    .Execute();
            }
        }

        /// <summary>
        /// Loads a property (primarily used internally for lazy loading)
        /// </summary>
        /// <typeparam name="ObjectType">Object type</typeparam>
        /// <typeparam name="DataType">Data type</typeparam>
        /// <param name="Object">Object</param>
        /// <param name="PropertyName">Property name</param>
        /// <returns>The appropriate property value</returns>
        public List<DataType> LoadProperties<ObjectType, DataType>(ObjectType Object, string PropertyName)
            where ObjectType : class,new()
            where DataType : class,new()
        {
            System.Collections.Generic.List<DataType> ReturnValue = new System.Collections.Generic.List<DataType>();
            foreach (ISourceInfo Source in SourceProvider.Where(x => x.Readable).OrderBy(x => x.Order))
            {
                IMapping Mapping = MapperProvider[typeof(ObjectType)].FirstOrDefault(x => x.DatabaseConfigType == Source.Database.GetType());
                IProperty Property = Mapping.Properties.FirstOrDefault(x => x.Name == PropertyName);
                if (Mapping != null && Property != null)
                {
                    foreach (dynamic Item in QueryProvider.Generate<ObjectType>(Source)
                        .LoadProperty(Object, (IProperty<ObjectType, DataType>)Property)
                        .Execute()
                        .FirstOrDefault())
                    {
                        DataType Temp = Item;
                        IProperty<DataType> IDProperty = (IProperty<DataType>)Property.Mapping.IDProperties.FirstOrDefault();
                        object IDValue = IDProperty.GetValue(Temp);
                        DataType Value = ReturnValue.FirstOrDefault(x => IDProperty.GetValue(x) == IDValue);
                        if (Value == default(DataType))
                            ReturnValue.Add(Temp);
                        else
                        {
                            Item.CopyTo(Value);
                        }
                    }
                }
            }

            foreach (IORMObject TempItem in ReturnValue)
            {
                if (TempItem != null)
                    TempItem.Session0 = this;
            }

            return ReturnValue;
        }

        /// <summary>
        /// Loads a property (primarily used internally for lazy loading)
        /// </summary>
        /// <typeparam name="ObjectType">Object type</typeparam>
        /// <typeparam name="DataType">Data type</typeparam>
        /// <param name="Object">Object</param>
        /// <param name="PropertyName">Property name</param>
        /// <returns>The appropriate property value</returns>
        public DataType LoadProperty<ObjectType, DataType>(ObjectType Object, string PropertyName)
            where ObjectType : class,new()
            where DataType : class,new()
        {
            return LoadProperties<ObjectType, DataType>(Object, PropertyName).FirstOrDefault();
        }

        /// <summary>
        /// Gets the number of pages based on the specified
        /// </summary>
        /// <param name="PageSize">Page size</param>
        /// <param name="Parameters">Parameters to search by</param>
        /// <typeparam name="ObjectType">Object type to get the page count of</typeparam>
        /// <returns>The number of pages that the table contains for the specified page size</returns>
        public int PageCount<ObjectType>(int PageSize = 25, params IParameter[] Parameters)
            where ObjectType : class,new()
        {
            foreach (ISourceInfo Source in SourceProvider.Where(x => x.Readable).OrderBy(x => x.Order))
            {
                int Count = QueryProvider.Generate<ObjectType>(Source)
                    .PageCount(PageSize, Parameters)
                    .Execute()
                    .FirstOrDefault()
                    .FirstOrDefault()
                    .Total;
                if (Count > 0)
                    return Count;
            }
            return 0;
        }

        /// <summary>
        /// Returns a paged list of items
        /// </summary>
        /// <typeparam name="ObjectType">Object type</typeparam>
        /// <param name="PageSize">Page size</param>
        /// <param name="CurrentPage">Current page (starting with 0)</param>
        /// <param name="Parameters">Parameters used in the where clause</param>
        /// <returns>A paged list of items that match the criteria</returns>
        public IEnumerable<ObjectType> Paged<ObjectType>(int PageSize = 25, int CurrentPage = 0, params IParameter[] Parameters)
            where ObjectType : class,new()
        {
            System.Collections.Generic.List<ObjectType> ReturnValue = new System.Collections.Generic.List<ObjectType>();
            foreach (ISourceInfo Source in SourceProvider.Where(x => x.Readable).OrderBy(x => x.Order))
            {
                foreach (dynamic Item in QueryProvider.Generate<ObjectType>(Source)
                    .Paged(PageSize, CurrentPage, Parameters)
                    .Execute()
                    .FirstOrDefault())
                {
                    ObjectType Temp = Item;
                    IMapping Mapping = MapperProvider[typeof(ObjectType)].FirstOrDefault(x => x.DatabaseConfigType == Source.Database.GetType());
                    IProperty<ObjectType> IDProperty = (IProperty<ObjectType>)Mapping.IDProperties.FirstOrDefault();
                    object IDValue = IDProperty.GetValue(Temp);
                    ObjectType Value = ReturnValue.FirstOrDefault(x => IDProperty.GetValue(x) == IDValue);
                    if (Value == default(ObjectType))
                        ReturnValue.Add(Temp);
                    else
                    {
                        Item.CopyTo(Value);
                    }
                }
            }

            foreach (IORMObject TempItem in ReturnValue)
            {
                if (TempItem != null)
                    TempItem.Session0 = this;
            }

            return ReturnValue;
        }

        /// <summary>
        /// Saves an object to the database
        /// </summary>
        /// <typeparam name="ObjectType">Object type</typeparam>
        /// <typeparam name="PrimaryKeyType">Primary key type</typeparam>
        /// <param name="Object">Object to save</param>
        public void Save<ObjectType, PrimaryKeyType>(ObjectType Object)
            where ObjectType : class,new()
        {
            foreach (ISourceInfo Source in SourceProvider.Where(x => x.Writable).OrderBy(x => x.Order))
            {
                QueryProvider.Generate<ObjectType>(Source)
                    .Save<ObjectType>(Object);
            }
        }
    }
}