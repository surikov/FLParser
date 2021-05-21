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
					Console.WriteLine("				,'ArpDir': '" + gd.ArpDir + "'");
					Console.WriteLine("				,'ArpGate': '" + gd.ArpGate + "'");
					Console.WriteLine("				,'ArpRange': '" + gd.ArpRange + "'");
					Console.WriteLine("				,'ArpRepeat': '" + gd.ArpRepeat + "'");
					Console.WriteLine("				,'ArpSlide': '" + gd.ArpSlide + "'");
					Console.WriteLine("				,'ArpTime': '" + gd.ArpTime + "'");
					Console.WriteLine("				,'BaseNote': '" + gd.BaseNote + "'");
					Console.WriteLine("				,'GeneratorName': '" + gd.GeneratorName + "'");
					Console.WriteLine("				,'Insert': '" + gd.Insert + "'");
					Console.WriteLine("				,'LayerParent': '" + gd.LayerParent + "'");
					Console.WriteLine("				,'Panning': '" + gd.Panning + "'");
					if (gd.Plugin == null)
					{
						Console.WriteLine("				,'Plugin': null");
					}
					else
					{
						Console.WriteLine("				,'Plugin': {");
						Console.WriteLine("					'FileName': '" + gd.Plugin.FileName + "'");
						Console.WriteLine("					,'Flags': '" + gd.Plugin.Flags + "'");
						Console.WriteLine("					,'Guid': '" + gd.Plugin.Guid + "'");
						Console.WriteLine("					,'InfoKind': '" + gd.Plugin.InfoKind + "'");
						Console.WriteLine("					,'InputInfo': '" + gd.Plugin.InputInfo + "'");
						Console.WriteLine("					,'MidiInPort': '" + gd.Plugin.MidiInPort + "'");
						Console.WriteLine("					,'MidiOutPort': '" + gd.Plugin.MidiOutPort + "'");
						Console.WriteLine("					,'Name': '" + gd.Plugin.Name + "'");
						Console.WriteLine("					,'NumInputs': '" + gd.Plugin.NumInputs + "'");
						Console.WriteLine("					,'NumOutputs': '" + gd.Plugin.NumOutputs + "'");
						Console.WriteLine("					,'OutputInfo': '" + gd.Plugin.OutputInfo + "'");
						Console.WriteLine("					,'PitchBendRange': '" + gd.Plugin.PitchBendRange + "'");
						Console.WriteLine("					,'State': '" + gd.Plugin.State + "'");
						Console.WriteLine("					,'VendorName': '" + gd.Plugin.VendorName + "'");
						Console.WriteLine("					,'VstId': '" + gd.Plugin.VstId + "'");
						Console.WriteLine("					,'VstNumber': '" + gd.Plugin.VstNumber + "'");
						Console.WriteLine("					}");
					}
					Console.WriteLine("				,'PluginSettings': '" + gd.PluginSettings + "'");
					Console.WriteLine("				,'SampleAmp': '" + gd.SampleAmp + "'");
					Console.WriteLine("				,'SampleFileName': '" + gd.SampleFileName + "'");
					Console.WriteLine("				,'SampleReversed': '" + gd.SampleReversed + "'");
					Console.WriteLine("				,'SampleReverseStereo': '" + gd.SampleReverseStereo + "'");
					Console.WriteLine("				,'SampleUseLoopPoints': '" + gd.SampleUseLoopPoints + "'");
					Console.WriteLine("				,'Volume ': '" + gd.Volume + "'");
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
