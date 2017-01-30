using System;
using System.Diagnostics;
using System.IO;

namespace DeadCode
{
	/// <summary>Supplies input parameter guarding.</summary>
	internal static class Guard
	{
		/// <summary>Guards the parameter if not null, otherwise throws an argument (null) exception.</summary>
		/// <typeparam name="T">
		/// The type to guard, can not be a structure.
		/// </typeparam>
		/// <param name="param">
		/// The parameter to guard.
		/// </param>
		/// <param name="paramName">
		/// The name of the parameter.
		/// </param>
		[DebuggerStepThrough]
		public static T NotNull<T>([ValidatedNotNull]T param, string paramName) where T : class
		{
			if (ReferenceEquals(param, null))
			{
				throw new ArgumentNullException(paramName);
			}
			return param;
		}

		/// <summary>Guards the parameter if not null or an empty string, otherwise throws an argument (null) exception.</summary>
		/// <param name="param">
		/// The parameter to guard.
		/// </param>
		/// <param name="paramName">
		/// The name of the parameter.
		/// </param>
		[DebuggerStepThrough]
		public static string NotNullOrEmpty([ValidatedNotNull]string param, string paramName)
		{
			NotNull(param, paramName);
			if (string.Empty == param)
			{
				throw new ArgumentException("Value cannot be an empty string.", paramName);
			}
			return param;
		}
		
		/// <summary>Guards the parameter if not null or an empty array, otherwise throws an argument (null) exception.</summary>
		/// <param name="param">
		/// The parameter to guard.
		/// </param>
		/// <param name="paramName">
		/// The name of the parameter.
		/// </param>
		[DebuggerStepThrough]
		public static T[] NotNullOrEmpty<T>([ValidatedNotNull]T[] param, string paramName)
		{
			NotNull(param, paramName);
			if (param.Length == 0)
			{
				throw new ArgumentException("Value cannot be an empty array.", paramName);
			}
			return param;
		}

		/// <summary>Guards the directory if not null and existing, otherwise throws an argument (null) exception.</summary>
		/// <param name"directory">
		/// The directory to guard.
		/// </param>
		/// <param name="paramName">
		/// The name of the parameter.
		/// </param>
		[DebuggerStepThrough]
		public static DirectoryInfo Exists([ValidatedNotNull]DirectoryInfo directory, string paramName)
		{
			NotNull(directory, paramName);
			if(!directory.Exists)
			{
				throw new ArgumentException(string.Format("The directory '{0}' should exist.", directory.FullName), paramName);
			}
			return directory;
		}

		/// <summary>Guards the file if not null and existing, otherwise throws an argument (null) exception.</summary>
		/// <param name"file">
		/// The file to guard.
		/// </param>
		/// <param name="paramName">
		/// The name of the parameter.
		/// </param>
		[DebuggerStepThrough]
		public static FileInfo Exists([ValidatedNotNull]FileInfo file, string paramName)
		{
			NotNull(file, paramName);
			if (!file.Exists)
			{
				throw new ArgumentException(string.Format("The file '{0}' should exist.", file.FullName), paramName);
			}
			return file;
		}

		/// <summary>Marks the NotNull argument as being validated for not being null,
		/// to satisfy the static code analysis.
		/// </summary>
		/// <remarks>
		/// Notice that it does not matter what this attribute does, as long as
		/// it is named ValidatedNotNullAttribute.
		/// </remarks>
		[AttributeUsage(AttributeTargets.Parameter)]
		sealed class ValidatedNotNullAttribute : Attribute { }
	}
}
