﻿using System;

namespace JDanielSmith.Runtime.InteropServices
{
	/// <summary>
	/// Indicates that the attributed method is exposed by an unmanaged dynamic-link
	/// library (DLL) as a static entry point.
	/// 
	/// Intented to be a clone of System.Runtime.InteropServices.DllImportAttribute,
	/// but can be used on methods declared in an interface.
    ///
    /// "The DllImport attribute must be specified on a method marked 'static' and 'extern'"
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public sealed class DllImportAttribute : Attribute
	{
		readonly System.Runtime.InteropServices.DllImportAttribute dllImportAttribute;

		/// <summary>
		///  Indicates the name or ordinal of the DLL entry point to be called.
		/// </summary>
		public string EntryPoint
		{
			get => dllImportAttribute.EntryPoint;
			set => dllImportAttribute.EntryPoint = value;
		}

		/// <summary>
		///  Indicates how to marshal string parameters to the method and controls name mangling.
		/// </summary>
		public System.Runtime.InteropServices.CharSet CharSet
		{
			get => dllImportAttribute.CharSet;
			set => dllImportAttribute.CharSet = value;
		}

		/// <summary>
		/// Indicates whether the callee calls the SetLastError Win32 API function before
		///     returning from the attributed method.
		/// </summary>
		public bool SetLastError
		{
			get => dllImportAttribute.SetLastError;
			set => dllImportAttribute.SetLastError = value;
		}

		/// <summary>
		/// Controls whether the System.Runtime.InteropServices.DllImportAttribute.CharSet
		/// field causes the common language runtime to search an unmanaged DLL for entry-point
		/// names other than the one specified.
		/// </summary>
		public bool ExactSpelling
		{
			get => dllImportAttribute.ExactSpelling;
			set => dllImportAttribute.ExactSpelling = value;
		}

		/// <summary>
		/// Indicates whether unmanaged methods that have HRESULT or retval return values
		/// are directly translated or whether HRESULT or retval return values are automatically
		/// converted to exceptions.
		/// </summary>
		public bool PreserveSig
		{
			get => dllImportAttribute.PreserveSig;
			set => dllImportAttribute.PreserveSig = value;
		}

		/// <summary>
		/// Indicates the calling convention of an entry point.
		/// </summary>
		public System.Runtime.InteropServices.CallingConvention CallingConvention
		{
			get => dllImportAttribute.CallingConvention;
			set => dllImportAttribute.CallingConvention = value;
		}

		/// <summary>
		/// Enables or disables best-fit mapping behavior when converting Unicode characters
		/// to ANSI characters.
		/// </summary>
		public bool BestFitMapping
		{
			get => dllImportAttribute.BestFitMapping;
			set => dllImportAttribute.BestFitMapping = value;
		}

		/// <summary>
		/// Enables or disables the throwing of an exception on an unmappable Unicode character
		/// that is converted to an ANSI "?" character.
		/// </summary>
		public bool ThrowOnUnmappableChar
		{
			get => dllImportAttribute.ThrowOnUnmappableChar;
			set => dllImportAttribute.ThrowOnUnmappableChar = value;
		}

		/// <summary>
		/// Initializes a new instance of the System.Runtime.InteropServices.DllImportAttribute
		/// class with the name of the DLL containing the method to import.
		/// </summary>
		/// <param name="dllName">The name of the DLL that contains the unmanaged method. This can include an assembly display name, if the DLL is included in an assembly.</param>
		public DllImportAttribute(string dllName)
		{
			dllImportAttribute = new System.Runtime.InteropServices.DllImportAttribute(dllName);
		}

		/// <summary>
		/// Gets the name of the DLL file that contains the entry point.
		/// </summary>
		public string Value
		{
			get => dllImportAttribute.Value;
		}
	}
}

