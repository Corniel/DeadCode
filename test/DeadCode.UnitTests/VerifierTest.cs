﻿using DeadCode.CodeAnalysis;
using NUnit.Framework;
using System;
using System.IO;

namespace DeadCode.UnitTests
{
	public class VerifierTest
	{
		[Test]
		public void Verify_XampleSln()
		{
			var root = new FileInfo(GetType().Assembly.Location).Directory;
			var solution = new FileInfo(Path.Combine(root.FullName, @"..\..\..\..\xample\Xample.sln"));
			var parts = new CodeParts();
			var context = new VerifyContext(solution, parts);
			Verifier.Verify(context);

			foreach (var part in parts)
			{
				Console.WriteLine($"{part}, {part.Count}");
			}
		}
	}
}
