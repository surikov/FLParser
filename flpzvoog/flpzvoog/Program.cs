using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monad.FLParser;

namespace flpzvoog
{
	class Program
	{
		static void Main(string[] args)
		{
			// The code provided will print ‘Hello World’ to the console.
			// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.
			//Console.WriteLine("Hello World!");
			//Console.ReadKey();

			// Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 
			if (args.Length > 0)
			{

				testAll(args[0]);
			}
		}
		static void testAll(string filePath)
		{
			//Console.WriteLine("start");
			//Console.WriteLine(filePath);
			Project project1 = Project.Load(filePath, false);
			dumpProj(project1, filePath);
			//Console.WriteLine("end");
		}
		static string qu(string txt)
		{
			return "'" + txt + "'";
		}
		static void dumpProj(Project project, string filePath)
		{
			string dlmtr = "";

			Console.WriteLine("{");
			Console.WriteLine("	'dump': '1.0.1'");
			Console.WriteLine("	,'origin': '" + filePath + "'");

			Console.WriteLine("	,'Author': '" + project.Author + "'");
			//Console.WriteLine("	,'Channels': '" + project.Channels.Count + "'");
			List<Channel> chans = project.Channels;
			Console.WriteLine("	,'Channels': [");
			dlmtr = "";
			for (int ch = 0; ch < chans.Count; ch++)
			{
				Channel singleChan = chans[ch];
				Console.WriteLine("		" + dlmtr + "{'Id': '" + singleChan.Id + "', 'Name': '" + singleChan.Name + "'");

				if (singleChan.Data.GetType() == typeof(GeneratorData))
				{
					GeneratorData gd = (GeneratorData)singleChan.Data;
					Console.WriteLine("			,'generator': {");
					Console.WriteLine("				'ArpChord': '" + gd.ArpChord + "'");
					Console.WriteLine("				'ArpDir': '" + gd.ArpDir + "'");
					Console.WriteLine("				}");
					Console.WriteLine("			,'automation': null");
				}
				else
				{
					if (singleChan.Data.GetType() == typeof(AutomationData))
					{
						AutomationData aut = (AutomationData)singleChan.Data;
						Console.WriteLine("			,'generator': null");
						Console.WriteLine("			,'automation': {");
						if (aut.Channel == null)
						{
							Console.WriteLine("				'Channel': null");
						}
						else
						{
							Console.WriteLine("				'Channel': '" + aut.Channel.Id + "'");
						}
						Console.WriteLine("				'InsertId ': '" + aut.InsertId + "'");
						Console.WriteLine("				}");
					}
					else
					{
						Console.WriteLine("			,'generator': null, 'automation': null");
					}
				}
				Console.WriteLine("			}");
				dlmtr = ",";
			}
			Console.WriteLine("		]");
			Console.WriteLine("	,'Comment': '" + project.Comment + "'");
			Console.WriteLine("	,'Genre': '" + project.Genre + "'");
			//Console.WriteLine("	,'Inserts': '" + project.Inserts.Length + "'");
			/*
			Console.WriteLine("	,'Inserts': [");
			dlmtr = "";
			for (int ch = 0; ch < project.Inserts.Length; ch++)
			{
				Insert singleInsert = project.Inserts[ch];
				if (singleInsert.Flags > 0)
				{
					Console.WriteLine("		" + dlmtr + "{");
					Console.WriteLine("			Id: '" + singleInsert.Id + "'");
					Console.WriteLine("			,BandFreq : '" + singleInsert.BandFreq + "'");
					Console.WriteLine("			,BandLevel : '" + singleInsert.BandLevel + "'");
					Console.WriteLine("			,BandWidth : '" + singleInsert.BandWidth + "'");
					Console.WriteLine("			,Flags : '" + singleInsert.Flags + "'");
					Console.WriteLine("			,HighFreq : '" + singleInsert.HighFreq + "'");
					Console.WriteLine("			,HighLevel : '" + singleInsert.HighLevel + "'");
					Console.WriteLine("			,HighWidth : '" + singleInsert.HighWidth + "'");
					Console.WriteLine("			,LowFreq : '" + singleInsert.LowFreq + "'");
					Console.WriteLine("			,LowLevel: '" + singleInsert.LowLevel + "'");
					Console.WriteLine("			,LowWidth : '" + singleInsert.LowWidth + "'");
					Console.WriteLine("			,Name : '" + singleInsert.Name + "'");
					Console.WriteLine("			,Pan : '" + singleInsert.Pan + "'");
					Console.WriteLine("			,Routes: '" + singleInsert.Routes.Length + "'");
					Console.WriteLine("			,RouteVolumes : '" + singleInsert.RouteVolumes.Length + "'");
					Console.WriteLine("			,Slots: '" + singleInsert.Slots.Length + "'");
					Console.WriteLine("			,StereoSep : '" + singleInsert.StereoSep + "'");
					Console.WriteLine("			,Volume : '" + singleInsert.Volume + "'");
					Console.WriteLine("			}");
					dlmtr = ",";
				}
			}
			Console.WriteLine("		]");
			*/
			Console.WriteLine("	,'MainPitch': '" + project.MainPitch + "'");
			Console.WriteLine("	,'MainVolume': '" + project.MainVolume + "'");
			Console.WriteLine("	,'PlayTruncatedNotes': '" + project.PlayTruncatedNotes + "'");
			Console.WriteLine("	,'Ppq': '" + project.Ppq + "'");
			Console.WriteLine("	,'ProjectTitle': '" + project.ProjectTitle + "'");
			Console.WriteLine("	,'Tempo': '" + project.Tempo + "'");
			Console.WriteLine("	,'Tracks': '" + project.Tracks.Length + "'");
			Console.WriteLine("	,'Version': '" + project.Version + "'");
			Console.WriteLine("	,'VersionString': '" + project.VersionString + "'");
			Console.WriteLine("	,'Patterns': '" + project.Patterns.Count + "'");

			Console.WriteLine("}");
		}
	}
}
