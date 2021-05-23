using System;
using System.Collections.Generic;
using Monad.FLParser;

namespace flpzvoog {
	class Program {
		static void Main(string[] args) {
			if (args.Length > 0) {
				testAll(args[0]);
			}
		}
		static void testAll(string filePath) {
			Project project1 = Project.Load(filePath, false);
			dumpProj(project1, filePath);
		}
		static string qu(string txt) {
			return "'" + txt + "'";
		}
		static void dumpPlugin(Plugin plugin) {
			if (plugin == null) {
				Console.WriteLine("				,'Plugin': null");
			} else {
				Console.WriteLine("				,'Plugin': {");
				Console.WriteLine("					'FileName': '" + plugin.FileName + "'");
				Console.WriteLine("					,'Flags': '" + plugin.Flags + "'");
				Console.WriteLine("					,'Guid': '" + plugin.Guid + "'");
				Console.WriteLine("					,'InfoKind': '" + plugin.InfoKind + "'");
				Console.WriteLine("					,'InputInfo': '" + plugin.InputInfo + "'");
				Console.WriteLine("					,'MidiInPort': '" + plugin.MidiInPort + "'");
				Console.WriteLine("					,'MidiOutPort': '" + plugin.MidiOutPort + "'");
				Console.WriteLine("					,'Name': '" + plugin.Name + "'");
				Console.WriteLine("					,'NumInputs': '" + plugin.NumInputs + "'");
				Console.WriteLine("					,'NumOutputs': '" + plugin.NumOutputs + "'");
				Console.WriteLine("					,'OutputInfo': '" + plugin.OutputInfo + "'");
				Console.WriteLine("					,'PitchBendRange': '" + plugin.PitchBendRange + "'");
				Console.WriteLine("					,'State': '" + plugin.State + "'");
				Console.WriteLine("					,'VendorName': '" + plugin.VendorName + "'");
				Console.WriteLine("					,'VstId': '" + plugin.VstId + "'");
				Console.WriteLine("					,'VstNumber': '" + plugin.VstNumber + "'");
				Console.WriteLine("					}");
			}
		}
		static void dumpGenerator(GeneratorData gd) {
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
			dumpPlugin(gd.Plugin);
			Console.WriteLine("				,'PluginSettings': '" + gd.PluginSettings + "'");
			Console.WriteLine("				,'SampleAmp': '" + gd.SampleAmp + "'");
			Console.WriteLine("				,'SampleFileName': '" + gd.SampleFileName + "'");
			Console.WriteLine("				,'SampleReversed': '" + gd.SampleReversed + "'");
			Console.WriteLine("				,'SampleReverseStereo': '" + gd.SampleReverseStereo + "'");
			Console.WriteLine("				,'SampleUseLoopPoints': '" + gd.SampleUseLoopPoints + "'");
			Console.WriteLine("				,'Volume ': '" + gd.Volume + "'");
			Console.WriteLine("				}");
		}
		static void dumpKeyFrame(AutomationKeyframe kf, string zp) {
			Console.WriteLine("					" + zp + "{'Value':'" + kf.Value + "'"
					+ ", 'Position':'" + kf.Position + "'"
					+ ", 'Tension':'" + kf.Tension + "'}");
		}
		static void dumpAutomationData(AutomationData aut) {
			Console.WriteLine("			,'automation': {");
			if (aut.Channel == null) {
				Console.WriteLine("				'Channel': null");
			} else {
				Console.WriteLine("				'Channel': '" + aut.Channel.Id + "'");
			}
			Console.WriteLine("				'InsertId': '" + aut.InsertId + "'");
			Console.WriteLine("				'Keyframes': [");
			string zp = "";
			for (int kk = 0; kk < aut.Keyframes.Length; kk++) {
				dumpKeyFrame(aut.Keyframes[kk], zp);
				zp = ",";
			}
			Console.WriteLine("					]");
			Console.WriteLine("				'Parameter': '" + aut.Parameter + "'");
			Console.WriteLine("				'SlotId': '" + aut.SlotId + "'");
			Console.WriteLine("				'VstParameter': '" + aut.VstParameter + "'");
			Console.WriteLine("				}");
		}
		static void dumpChannel(Channel singleChan, string dlmtr) {
			Console.WriteLine("		" + dlmtr + "{'Id': '" + singleChan.Id + "', 'Name': '" + singleChan.Name + "' ,'Color':'" + singleChan.Color + "'");
			if (singleChan.Data.GetType() == typeof(GeneratorData)) {
				dumpGenerator((GeneratorData)singleChan.Data);
				Console.WriteLine("			,'automation': null");
			} else {
				if (singleChan.Data.GetType() == typeof(AutomationData)) {
					AutomationData aut = (AutomationData)singleChan.Data;
					Console.WriteLine("			,'generator': null");
					dumpAutomationData(aut);
				} else {
					Console.WriteLine("			,'generator': null, 'automation': null");
				}
			}
			Console.WriteLine("			}");
		}
		static void dumpSlot(InsertSlot insertSlot, string idlmtr) {
			Console.WriteLine("				" + idlmtr + "{");
			Console.WriteLine("					'DryWet':'" + insertSlot.DryWet + "'");
			dumpPlugin(insertSlot.Plugin);
			/*if (insertSlot.Plugin == null) {
				Console.WriteLine("					,'Plugin': null");
			} else {
				Console.WriteLine("					,'Plugin': {");
				Console.WriteLine("						'FileName': '" + insertSlot.Plugin.FileName + "'");
				Console.WriteLine("						,'Flags': '" + insertSlot.Plugin.Flags + "'");
				Console.WriteLine("						,'Guid': '" + insertSlot.Plugin.Guid + "'");
				Console.WriteLine("						,'InfoKind': '" + insertSlot.Plugin.InfoKind + "'");
				Console.WriteLine("						,'InputInfo': '" + insertSlot.Plugin.InputInfo + "'");
				Console.WriteLine("						,'MidiInPort': '" + insertSlot.Plugin.MidiInPort + "'");
				Console.WriteLine("						,'MidiOutPort': '" + insertSlot.Plugin.MidiOutPort + "'");
				Console.WriteLine("						,'Name': '" + insertSlot.Plugin.Name + "'");
				Console.WriteLine("						,'NumInputs': '" + insertSlot.Plugin.NumInputs + "'");
				Console.WriteLine("						,'NumOutputs': '" + insertSlot.Plugin.NumOutputs + "'");
				Console.WriteLine("						,'OutputInfo': '" + insertSlot.Plugin.OutputInfo + "'");
				Console.WriteLine("						,'PitchBendRange': '" + insertSlot.Plugin.PitchBendRange + "'");
				Console.WriteLine("						,'State': '" + insertSlot.Plugin.State + "'");
				Console.WriteLine("						,'VendorName': '" + insertSlot.Plugin.VendorName + "'");
				Console.WriteLine("						,'VstId': '" + insertSlot.Plugin.VstId + "'");
				Console.WriteLine("						,'VstNumber': '" + insertSlot.Plugin.VstNumber + "'");
				Console.WriteLine("						}");
			}*/
			Console.WriteLine("					,'PluginSettings':'" + insertSlot.PluginSettings + "'");
			Console.WriteLine("					,'State':'" + insertSlot.State + "'");
			Console.WriteLine("					,'Volume':'" + insertSlot.Volume + "'");
			Console.WriteLine("					}");
		}
		static void dumpInsert(Insert singleInsert, string dlmtr) {
			Console.WriteLine("		" + dlmtr + "{");
			Console.WriteLine("			Id: '" + singleInsert.Id + "'");
			Console.WriteLine("			,BandFreq : '" + singleInsert.BandFreq + "'");
			Console.WriteLine("			,BandLevel : '" + singleInsert.BandLevel + "'");
			Console.WriteLine("			,BandWidth : '" + singleInsert.BandWidth + "'");
			Console.WriteLine("			,'Color':'" + singleInsert.Color + "'");
			Console.WriteLine("			,Flags : '" + singleInsert.Flags + "'");
			Console.WriteLine("			,HighFreq : '" + singleInsert.HighFreq + "'");
			Console.WriteLine("			,HighLevel : '" + singleInsert.HighLevel + "'");
			Console.WriteLine("			,HighWidth : '" + singleInsert.HighWidth + "'");
			Console.WriteLine("			,LowFreq : '" + singleInsert.LowFreq + "'");
			Console.WriteLine("			,LowLevel: '" + singleInsert.LowLevel + "'");
			Console.WriteLine("			,LowWidth : '" + singleInsert.LowWidth + "'");
			Console.WriteLine("			,Name : '" + singleInsert.Name + "'");
			Console.WriteLine("			,Pan : '" + singleInsert.Pan + "'");
			string idlmtr = "";
			string rts = "";
			for (int rr = 0; rr < singleInsert.Routes.Length; rr++) {
				rts = rts + idlmtr + singleInsert.Routes[rr];
				idlmtr = ",";
			}
			Console.WriteLine("			,Routes: [" + rts + "]");
			idlmtr = "";
			rts = "";
			for (int rr = 0; rr < singleInsert.RouteVolumes.Length; rr++) {
				rts = rts + idlmtr + singleInsert.RouteVolumes[rr];
				idlmtr = ",";
			}
			Console.WriteLine("			,RouteVolumes: [" + rts + "]");
			Console.WriteLine("			,Slots: [");
			idlmtr = "";
			for (int rr = 0; rr < singleInsert.Slots.Length; rr++) {
				InsertSlot insertSlot = singleInsert.Slots[rr];
				dumpSlot(insertSlot, idlmtr);
				idlmtr = ",";
			}
			Console.WriteLine("				]");
			Console.WriteLine("			,StereoSep : '" + singleInsert.StereoSep + "'");
			Console.WriteLine("			,Volume : '" + singleInsert.Volume + "'");
			Console.WriteLine("			}");
		}
		static void dumpPlayListItem(IPlaylistItem itm,string  itdel) {
			Console.WriteLine("				" + itdel + "{");
			Console.WriteLine("					'EndOffset': '" + itm.EndOffset + "'");
			Console.WriteLine("					,'Length': '" + itm.Length + "'");
			Console.WriteLine("					,'Position': '" + itm.Position + "'");
			Console.WriteLine("					,'StartOffset': '" + itm.StartOffset + "'");
			Console.WriteLine("					}");
		}
		static void dumpTrack(Track track, string dlmtr,int tt) {
			Console.WriteLine("		" + dlmtr + "{");
			Console.WriteLine("			'Items':[");
			string itdel = "";
			for (int ii = 0; ii < track.Items.Count; ii++) {
				IPlaylistItem itm = track.Items[ii];
				dumpPlayListItem(itm, itdel);
				itdel = ",";
			}
			Console.WriteLine("				]");
			Console.WriteLine("			,'Name':'" + track.Name + "'");
			Console.WriteLine("			,'order':'" + tt + "'");
			Console.WriteLine("			}");
		}
		static void dumpPattern(Pattern pattern,string dlmtr) {
			Console.WriteLine("		" + dlmtr + "{");
			Console.WriteLine("			,'Id':'" + pattern.Id + "'");
			Console.WriteLine("			,'Name':'" + pattern.Name + "'");
			Console.WriteLine("			,'Notes':'" + pattern.Notes.Count + "'");
			Console.WriteLine("			}");
		}
		static void dumpProj(Project project, string filePath) {
			string dlmtr = "";
			Console.WriteLine("{");
			Console.WriteLine("	'dump': '1.0.1'");
			Console.WriteLine("	,'origin': '" + filePath + "'");
			Console.WriteLine("	,'Author': '" + project.Author + "'");
			List<Channel> chans = project.Channels;
			Console.WriteLine("	,'Channels': [");
			dlmtr = "";
			for (int ch = 0; ch < chans.Count; ch++) {
				Channel singleChan = chans[ch];
				dumpChannel(singleChan, dlmtr);
				dlmtr = ",";
			}
			Console.WriteLine("		]");
			Console.WriteLine("	,'Comment': '" + project.Comment.Replace('\n', ' ').Replace('\r', ' ') + "'");
			Console.WriteLine("	,'Genre': '" + project.Genre + "'");
			Console.WriteLine("	,'Inserts': [");
			dlmtr = "";
			for (int ch = 0; ch < project.Inserts.Length; ch++) {
				Insert singleInsert = project.Inserts[ch];
				if (singleInsert.Flags > 0) {
					dumpInsert(singleInsert, dlmtr);
					dlmtr = ",";
				}
			}
			Console.WriteLine("		]");
			Console.WriteLine("	,'MainPitch': '" + project.MainPitch + "'");
			Console.WriteLine("	,'MainVolume': '" + project.MainVolume + "'");
			Console.WriteLine("	,'PlayTruncatedNotes': '" + project.PlayTruncatedNotes + "'");
			Console.WriteLine("	,'Ppq': '" + project.Ppq + "'");
			Console.WriteLine("	,'ProjectTitle': '" + project.ProjectTitle + "'");
			Console.WriteLine("	,'Tempo': '" + project.Tempo + "'");
			Console.WriteLine("	,'Tracks': [");
			dlmtr = "";
			for (int tt = 0; tt < project.Tracks.Length; tt++) {
				Track track = project.Tracks[tt];
				if (track.Items.Count > 0) {
					dumpTrack(track,dlmtr,tt);
					dlmtr = ",";
				}
			}
			Console.WriteLine("		]");
			Console.WriteLine("	,'Version': '" + project.Version + "'");
			Console.WriteLine("	,'VersionString': '" + project.VersionString + "'");
			Console.WriteLine("	,'Patterns': [");
			dlmtr = "";
			for (int tt = 0; tt < project.Patterns.Count; tt++) {
				Pattern pattern = project.Patterns[tt];
				dumpPattern(pattern, dlmtr);
				dlmtr = ",";
			}
			Console.WriteLine("}");
		}
	}
}
