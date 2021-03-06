﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Renderer;
using Core2D.Style;

namespace Core2D.Shapes
{
    /// <summary>
    /// Image shape.
    /// </summary>
    public class ImageShape : TextShape, IImageShape
    {
        private string _key;

        /// <inheritdoc/>
        public override Type TargetType => typeof(IImageShape);

        /// <inheritdoc/>
        public string Key
        {
            get => _key;
            set => Update(ref _key, value);
        }

        /// <inheritdoc/>
        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, object db, object r)
        {
            var record = Data?.Record ?? r;

            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                var state = base.BeginTransform(dc, renderer);

                renderer.Draw(dc, this, dx, dy, db, record);

                base.EndTransform(dc, renderer, state);

                base.Draw(dc, renderer, dx, dy, db, record);
            }
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check whether the <see cref="Key"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeKey() => !string.IsNullOrWhiteSpace(_key);
    }
}
