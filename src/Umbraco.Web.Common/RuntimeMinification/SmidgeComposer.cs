﻿using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Runtime;
using Umbraco.Core.WebAssets;

namespace Umbraco.Web.Common.RuntimeMinification
{
    public sealed class SmidgeComposer : IComposer
    {
        public void Compose(Composition composition)
        {
            composition.RegisterUnique<IRuntimeMinifier, SmidgeRuntimeMinifier>();
            composition.RegisterUnique<SmidgeHelperAccessor>();
        }
    }
}
