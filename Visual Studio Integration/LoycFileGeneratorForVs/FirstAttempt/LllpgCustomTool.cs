﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Loyc;
using Loyc.Collections;
using Loyc.LLParserGenerator;
using Loyc.Syntax;
using Loyc.Utilities;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace Loyc
{
	[Guid("8B5B79F9-D223-46DE-8AF4-4A37040A5783")]
	[CustomTool("LLLPG", "Processes LLLPG or other lexical macros (in C# project).", 
		"FAE04EC1-301F-11D3-BF4B-00C04F79EFBC", // identifies the C# language service (I think)
		"9.0", "10.0", "11.0", "12.0", "13.0")]
	public class LllpgCustomToolForCs : LllpgCustomTool
	{
		protected override string DefaultExtension()
		{
			return ".cs";
		}
		public override int Generate(string inputFilePath, string inputFileContents, string defaultNamespace, IntPtr[] outputFileContents, out uint outputSize, IVsGeneratorProgress progressCallback)
		{
			using (LNode.PushPrinter(Ecs.EcsNodePrinter.PrintPlainCSharp))
 				return base.Generate(inputFilePath, inputFileContents, defaultNamespace, outputFileContents, out outputSize, progressCallback);
		}
		[ComRegisterFunction]
		public static void RegisterClass(Type t)
		{
			// the argument is redundant, we know what it is
			Debug.Assert(t == typeof(LllpgCustomToolForCs));
			RegisterCustomTool.Register(t);
		}
		[ComUnregisterFunction]
		public static void UnregisterClass(Type t)
		{
			RegisterCustomTool.Unregister(t);
		}
	}

	[Guid("ECD6C726-DC44-49B0-9F88-098801AF9889")]
	[CustomTool("LLLPG2LES", "Processes LLLPG or other lexical macros and outputs LES (in C# project).",
		"FAE04EC1-301F-11D3-BF4B-00C04F79EFBC", // identifies the C# language service (I think)
		"9.0", "10.0", "11.0", "12.0", "13.0")]
	public class LllpgCustomToolForLes : LllpgCustomTool
	{
		protected override string DefaultExtension()
		{
			return ".les";
		}
		public override int Generate(string inputFilePath, string inputFileContents, string defaultNamespace, IntPtr[] outputFileContents, out uint outputSize, IVsGeneratorProgress progressCallback)
		{
			using (LNode.PushPrinter(Loyc.Syntax.Les.LesNodePrinter.Printer))
				return base.Generate(inputFilePath, inputFileContents, defaultNamespace, outputFileContents, out outputSize, progressCallback);
		}
		[ComRegisterFunction]
		public static void RegisterClass(Type t)
		{
			// the argument is redundant, we know what it is
			Debug.Assert(t == typeof(LllpgCustomToolForLes));
			RegisterCustomTool.Register(t);
		}
		[ComUnregisterFunction]
		public static void UnregisterClass(Type t)
		{
			RegisterCustomTool.Unregister(t);
		}
	}

	public abstract class LllpgCustomTool : IVsSingleFileGenerator
	{
		protected abstract string DefaultExtension();
		public int DefaultExtension(out string defExt)
		{
			return (defExt = DefaultExtension()).Length;
		}

		class Compiler : LEL.Compiler
		{
			public Compiler(IMessageSink sink, ISourceFile file)
				: base(sink, new [] { file }, typeof(LEL.Prelude.Macros)) { }

			public StringBuilder Output = new StringBuilder();

			protected override void WriteOutput(ISourceFile file, RVList<LNode> results)
			{
				var printer = LNode.Printer;
				Output.Append("// Generated by LLLPG custom tool. Note: you can specify command-line arguments\n"
					+ "// to the tool in the 'Custom Tool Namespace', e.g. --macros=FileName.dll\n"
					+ "// to use custom macros. LLLPG version: " + typeof(Rule).Assembly.GetName().Version.ToString());
				foreach (LNode node in results)
				{
					printer(node, Output, Sink, null, IndentString, NewlineString);
					Output.Append(NewlineString);
				}
			}
		}

		public virtual int Generate(string inputFilePath, string inputFileContents, string defaultNamespace, IntPtr[] outputFileContents, out uint outputSize, IVsGeneratorProgress progressCallback)
		{
			string inputFolder = Path.GetDirectoryName(inputFilePath);
			string oldCurDir = Environment.CurrentDirectory;
			try {
 				Environment.CurrentDirectory = inputFolder; // --macros should be relative to file being processed

				var sourceFile = new StringCharSourceFile(inputFileContents, inputFilePath);
				var sink = ToMessageSink(progressCallback);
			
				var c = new Compiler(sink, sourceFile);

				var options = new BMultiMap<string, string>();
				var argList = G.SplitCommandLineArguments(defaultNamespace);
				UG.ProcessCommandLineArguments(argList, options, "", LEL.Compiler.ShortOptions, LEL.Compiler.TwoArgOptions);

				string _;
				var KnownOptions = LEL.Compiler.KnownOptions;
				if (options.TryGetValue("help", out _) || options.TryGetValue("?", out _))
					LEL.Compiler.ShowHelp(KnownOptions);

				Symbol minSeverity = MessageSink.Note;
				var filter = new SeverityMessageFilter(MessageSink.Console, minSeverity);

				if (LEL.Compiler.ProcessArguments(c, options)) {
					LEL.Compiler.WarnAboutUnknownOptions(options, MessageSink.Console, KnownOptions);
					if (c != null)
					{
						c.MacroProcessor.PreOpenedNamespaces.Add(GSymbol.Get("LEL.Prelude"));
						c.MacroProcessor.PreOpenedNamespaces.Add(GSymbol.Get("Loyc.LLParserGenerator"));
						c.AddMacros(typeof(Loyc.LLParserGenerator.Macros).Assembly);
						c.Run();
					}

					var outputBytes = Encoding.UTF8.GetBytes(c.Output.ToString());
					c.Output = null; // no longer needed
					outputSize = (uint)outputBytes.Length;
					outputFileContents[0] = Marshal.AllocCoTaskMem(outputBytes.Length);
					Marshal.Copy(outputBytes, 0, outputFileContents[0], outputBytes.Length);
				}
				else
				{
					outputFileContents[0] = IntPtr.Zero;
					outputSize = 0;
				}
				return VSConstants.S_OK;
			} finally {
				Environment.CurrentDirectory = oldCurDir;
			}
		}

		private static MessageSinkFromDelegate ToMessageSink(IVsGeneratorProgress progressCallback)
		{
			var sink = new MessageSinkFromDelegate(
				(Symbol severity, object context, string message, object[] args) =>
				{
					if (MessageSink.GetSeverity(severity) >= MessageSink.GetSeverity(MessageSink.Warning))
					{
						int line = 0, col = 0;
						if (context is LNode)
						{
							var range = ((LNode)context).Range;
							line = range.Begin.Line;
							col = range.Begin.PosInLine;
						}
						progressCallback.GeneratorError(severity == MessageSink.Warning ? 1 : 0, 0u,
							Localize.From(message, args), (uint)line - 1u, (uint)col);
					}
					else
						MessageSink.Console.Write(severity, context, message, args);
				});
			return sink;
		}
	}
}
