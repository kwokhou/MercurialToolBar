// Guids.cs
// MUST match guids.h
using System;

namespace CIDesigns.MercurialToolbar
{
    static class GuidList
    {
        public const string guidMercurialToolbarPkgString = "323fac64-851a-44a5-87b4-4f4f21b9d960";
        public const string guidMercurialToolbarCmdSetString = "a0894de5-19ab-4288-bb6f-82cd202a107c";

        public static readonly Guid guidMercurialToolbarCmdSet = new Guid(guidMercurialToolbarCmdSetString);
    };
}