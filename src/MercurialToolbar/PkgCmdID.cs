// PkgCmdID.cs
// MUST match PkgCmdID.h
using System;

namespace CIDesigns.MercurialToolbar
{
    static class PkgCmdIDList
    {
        public const uint cmdidCommit = 0x100;
        public const uint cmdidLog = 0x110;
        public const uint cmdidRevert = 0x0120;
        public const uint cmdidSync = 0x0130;
        public const uint cmdidUpdate = 0x0140;
        public const uint cmdidBranch = 0x0150;
        public const uint cmdidMerge = 0x0160;
        public const uint cmdidPush = 0x0170;
        public const uint cmdidPull = 0x0180;
        public const uint cmdidSettings = 0x0190;
        public const uint cmdidCommand = 0x0200;
        public const uint cmdidCommandCombo = 0x0210;
    };
}