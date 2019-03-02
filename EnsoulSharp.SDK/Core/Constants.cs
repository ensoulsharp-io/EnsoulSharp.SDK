// <copyright file="Constants.cs" company="EnsoulSharp">
//    Copyright (c) 2019 EnsoulSharp.
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/
// </copyright>

namespace EnsoulSharp.SDK
{
    using System;
    using System.IO;

    using SharpDX.Direct3D9;

    /// <summary>
    ///     Constant values of the EnsoulSharp development kit.
    /// </summary>
    public static class Constants
    {
        #region Static Fields

        /// <summary>
        ///     EnsoulSharp Application Data folder.
        /// </summary>
        public static readonly string EnsoulSharpAppData =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "LS" + Environment.UserName.GetHashCode().ToString("X"));

        /// <summary>
        ///     EnsoulSharp SDK Log directory.
        /// </summary>
        public static readonly string LogDirectory = Path.Combine(EnsoulSharpAppData, "Logs", "SDK");

        /// <summary>
        ///     EnsoulSharp SDK Session Log file name.
        /// </summary>
        public static readonly string LogFileName = DateTime.Now.ToString("d").Replace('/', '-') + ".log";

        /// <summary>
        ///     EnsoulSharp SDK Font.
        /// </summary>
        private static Font ensoulSharpFont;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the ensoul sharp font.
        /// </summary>
        public static Font EnsoulSharpFont
        {
            get
            {
                if (ensoulSharpFont != null && !ensoulSharpFont.IsDisposed)
                {
                    return ensoulSharpFont;
                }

                return
                    ensoulSharpFont =
                    new Font(
                        Drawing.Direct3DDevice,
                        16,
                        0,
                        FontWeight.DoNotCare,
                        0,
                        false,
                        FontCharacterSet.Default,
                        FontPrecision.TrueType,
                        FontQuality.ClearTypeNatural,
                        FontPitchAndFamily.DontCare | FontPitchAndFamily.Decorative | FontPitchAndFamily.Modern,
                        "Calibri");
            }
        }

        #endregion
    }
}