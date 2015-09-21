﻿// Copyright © 2015 Mimeo, Inc.

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace SendWithUs.Client
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using Newtonsoft.Json.Linq;

    public class CollectionResponse<TItem> : BaseResponse<JArray>, ICollectionResponse<TItem>
        where TItem : class, ICollectionItem
    {
        protected Type ItemType { get; set; }

        #region ICollectionResponse<TItem> Members

        public IEnumerable<TItem> Items { get; set; }

        #endregion

        #region ICollectionResponse Members

        public ICollectionResponse Initialize(IResponseFactory responseFactory, HttpStatusCode statusCode, JToken json, Type itemType)
        {
            // The given item type must implement the class parameter item type (which is probably an interface).
            if (!typeof(TItem).GetTypeInfo().IsAssignableFrom(itemType.GetTypeInfo()))
            {
                throw new InvalidOperationException(
                    String.Format(CultureInfo.InvariantCulture, "Type '{0}' does not implement {1}.", itemType.FullName, typeof(TItem).FullName));
            }

            this.ItemType = itemType;
            base.Initialize(responseFactory, statusCode, json);
            return this;
        }

        #endregion

        #region Base class overrides

        protected internal override void Populate(JArray json)
        {
            if (json != null)
            {
                this.Items = json.Select(jt => this.ResponseFactory.CreateItem(this.ItemType, jt) as TItem).ToList();
            }
            else
            {
                this.Items = Enumerable.Empty<TItem>();
            }
        }

        #endregion
    }
}